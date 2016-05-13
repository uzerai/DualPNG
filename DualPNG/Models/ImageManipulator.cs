using Hjg.Pngcs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace DualPNG.Models
{
    public class ImageManipulator
    { 

        private Dictionary<string, double> Opts { get; set; }
        private string PathOne { get; set; }
        private string PathTwo { get; set; }
        private PngReader ReaderOne { get; set; }
        private PngReader ReaderTwo { get; set; }

        /// <summary>
        /// Generates an image that displays differently when rendered in different clients,
        /// via manipulating PNG gamma metadata.
        /// </summary>
        /// <param name="imgOne"></param>
        /// <param name="imgTwo"></param>
        /// <param name="directory"></param>
        public ImageManipulator(string imgOne, string imgTwo, Dictionary<string, double> options)
        {
            PathOne = imgOne;
            PathTwo = imgTwo;
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

        public void GenerateImage(string targetPath)
        {
            GenerateBlackBlank(targetPath);
            PngReader outRead = FileHelper.CreatePngReader(targetPath);
            PngWriter outWrite = FileHelper.CreatePngWriter(targetPath, outRead.ImgInfo, true);
            ImageInfo outInfo = outRead.ImgInfo;
            ImageLine outLine = new ImageLine(outInfo);

            Debug.WriteLine(outLine.ElementsPerRow + " " + outLine.Rown);
            //for (int x = 0; x <= outLine.ElementsPerRow; x++){
            //  for (int y = 0; y <= outLine.Rown; y++)
            //{

            //}
            //}
        }

        /// <summary>
        /// Generates a plain black 
        /// </summary>
        /// <returns></returns>
        private void GenerateBlackBlank(string targetPath)
        {
            Image a = Image.FromFile(PathOne);
            Bitmap b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.Black);
            Bitmap blank = new Bitmap(b, (a.Width * 2), (a.Height * 2));
            b.Save(targetPath, ImageFormat.Png); //saves the image to later retrieve it.
        }

        /// <summary>
        /// Checks the image sizes and fails if they are not equal.
        /// </summary>
        /// <returns></returns>
        private Boolean IdenticalSizes()
        {
            return (Image.FromFile(PathOne).PhysicalDimension == Image.FromFile(PathTwo).PhysicalDimension) ? true : false;
        }

        /// <summary>
        /// Initializes the .png readers.
        /// </summary>
        private void InitializeReaders()
        {
            ReaderOne = FileHelper.CreatePngReader(PathOne);
            ReaderOne = FileHelper.CreatePngReader(PathTwo);
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

        //TODO: FIND A DECENT FUCKING PNG EDITOR HOLY FUCKING SHIT I FUCNVJSNDFBJSDNFGBKSDFNG B
    }
}