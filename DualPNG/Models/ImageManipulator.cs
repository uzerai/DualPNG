using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace DualPNG.Models
{
    public class ImageManipulator
    { 

        private Dictionary<string, double> Opts { get; set; }
        private Bitmap iOne { get; set; }
        private Bitmap iTwo { get; set; }
        private Bitmap result { get; set; }

        //private PngReader ReaderOne { get; set; }
        //private PngReader ReaderTwo { get; set; }

        /// <summary>
        /// Generates an image that displays differently when rendered in different clients,
        /// via manipulating PNG gamma metadata.
        /// </summary>
        /// <param name="imgOne"></param>
        /// <param name="imgTwo"></param>
        /// <param name="directory"></param>
        public ImageManipulator(string imgOne, string imgTwo, Dictionary<string, double> options)
        {
            iOne = new Bitmap(Bitmap.FromFile(imgOne));
            iTwo = new Bitmap(Bitmap.FromFile(imgTwo));
            if (!IdenticalSizes())
            {
                throw new Exception("Images are not the same size.");
            }
            //supports custom options.
            if (options != null)
            {
                Opts = options;
            }
            else
            {
                Opts = new Dictionary<string, double>(); // default values for the options.
                Opts["gamma"] = 0.023;
                Opts["fade1"] = 220 / 255.0;
                Opts["fade2"] = 210 / 255.0;
                Opts["shift"] = 10;
            }
        }

        /// <summary>
        /// Generates the image which only needs its gamma adjusted.
        /// </summary>
        /// <param name="targetPath"></param>
        public void GenerateImage(string targetPath)
        {
            Bitmap iOut = GenerateBlackBlank();
            for (int x = 0; x < iOut.Width; x++)
            {
                for (int y = 0; y < iOut.Height; y++)
                {
                  if((x % 2 == 0) && (y % 2 == 0))
                    {
                        Color inCol = iOne.GetPixel((x / 2), (y / 2));
                        int r = Transform(inCol.R);
                        int g = Transform(inCol.G);
                        int b = Transform(inCol.B);
                        Color outCol = Color.FromArgb(r, g, b);
                        iOut.SetPixel(x, y, outCol);
                    }
                    else
                    {
                        Color inCol = iTwo.GetPixel((x / 2), (y / 2));
                        int r = Convert.ToInt32(Math.Round(inCol.R * Opts["fade2"]));
                        int g = Convert.ToInt32(Math.Round(inCol.G * Opts["fade2"]));
                        int b = Convert.ToInt32(Math.Round(inCol.B * Opts["fade2"]));
                        Color outCol = Color.FromArgb(r, g, b);
                        iOut.SetPixel(x, y, outCol);
                    }
                }
            }
            result = iOut;
            iOut.Save( Path.Combine(targetPath, "result.png"), ImageFormat.Png);
            SetGamma(targetPath);
        }

        private void SetGamma(string targetPath)
        {
            //using the pngcs library
            int gamma = Convert.ToInt32(Opts["gamma"] * 100000); // adjusted gamma value as unsigned int
            using (Stream fileStream = FileHelper.OpenFileForReading(Path.Combine(targetPath, "result.png")),
                outStream = FileHelper.OpenFileForWriting(Path.Combine(targetPath, "result2.png"), true))
            {
                PngReader reader = new PngReader(fileStream);
                ImageInfo inf = reader.ImgInfo;
                (reader.GetChunksList().GetById(PngChunkGAMA.ID)[0] as PngChunkGAMA).SetGamma(gamma);
                Debug.WriteLine(reader.GetChunksList().ToStringFull());
                Debug.WriteLine((reader.GetChunksList().GetById(PngChunkGAMA.ID)[0] as PngChunkGAMA).GetGamma());
                PngWriter writer = new PngWriter(outStream, inf);
                writer.CopyChunksFirst(reader, ChunkCopyBehaviour.COPY_ALL);
                ImageLine line;
                for (int row = 0; row < inf.Rows; row++)
                {
                    line = reader.ReadRow(row);
                    writer.WriteRow(line.GetScanlineInt());
                }
                writer.End();
                reader.End();
            }
        }

        /// <summary>
        /// Generates a blank bitmap which is easy to write to.
        /// </summary>
        /// <returns></returns>
        private Bitmap GenerateBlackBlank()
        {
            return new Bitmap((iOne.Width * 2), (iOne.Height * 2));
        }

        /// <summary>
        /// Checks the image sizes and fails if they are not equal.
        /// </summary>
        /// <returns></returns>
        private Boolean IdenticalSizes()
        {
            return (iOne.PhysicalDimension == iOne.PhysicalDimension) ? true : false;
        }

        /// <summary>
        /// Transforms a number to one affected by the fade and gamma shift.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private int Transform(int num)
        {
            double scaled = num * Opts["fade1"] + Opts["shift"];
            return Convert.ToInt32(Math.Floor((scaled / 255.0) * (Opts["gamma"]) * 255.0));
        }
    }
}