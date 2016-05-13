using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using DualPNG;

namespace DualPNG.Models
{
    public class FileHandler
    {
        private HttpPostedFileBase fileOne { get; set; }
        private HttpPostedFileBase fileTwo { get; set; }
        private string basePath { get; set; }
        public string FileDir { get; set; }
        public int RandomFolderName { get; set; } = 0;

        /// <summary>
        /// A class to handle the saving and deletion of images to a given path.
        /// </summary>
        /// <param name="fileOne"></param>
        /// <param name="fileTwo"></param>
        /// <param name="path"></param>
        public FileHandler(HttpPostedFileBase fileOne, HttpPostedFileBase fileTwo, string path)
        {
            this.fileOne = fileOne;
            this.fileTwo = fileTwo;
            this.basePath = path;
            FileDir = GenerateRandomDirectory();
        }

        public void GenerateImage()
        {
            ImageManipulator manipulator =
                    new ImageManipulator(Path.Combine(FileDir, Path.GetFileName(fileOne.FileName)),
                                            Path.Combine(FileDir, Path.GetFileName(fileTwo.FileName)),
                                            null);
            manipulator.GenerateImage(FileDir);
        }

        /// <summary>
        /// Saves the files passed to the FileHandler.
        /// </summary>
        /// <returns>0 if successful</returns>
        public int SaveHandledFiles()
        {
            int successful = 1;
            try
            {
                string fileNameOne = Path.GetFileName(fileOne.FileName);
                string fileNameTwo = Path.GetFileName(fileTwo.FileName);
                fileOne.SaveAs(Path.Combine(FileDir, fileNameOne));
                fileTwo.SaveAs(Path.Combine(FileDir, fileNameTwo));
                successful = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return successful;
        }

        /// <summary>
        /// Deletes the folder and images created by the FileHandler, if they exist.
        /// </summary>
        /// <returns>0 if successful</returns>
        public int DeleteHandledFiles()
        {
            int successful = 1;
            try
            {
                if (Directory.Exists(FileDir))
                {
                    DirectoryInfo dir = new DirectoryInfo(FileDir);
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    Directory.Delete(FileDir);
                    successful = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return successful;
        }

        /// <summary>
        /// Checks if a directory with a randomly generated name exists, creates directory if doesn't.
        /// </summary>
        /// <returns></returns>
        private string GenerateRandomDirectory()
        {
            Random r = new Random();
            RandomFolderName = r.Next();
            string dir = Path.Combine(basePath, RandomFolderName.ToString());
            while (Directory.Exists(dir))
            {
                RandomFolderName = r.Next();
                dir = Path.Combine(basePath, RandomFolderName.ToString());
            }
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}