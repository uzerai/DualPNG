using DualPNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DualPNG.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Stores uploaded files before converting them to the combined image.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase fileLeft, HttpPostedFileBase fileRight)
        {
            if ((fileLeft != null && fileLeft.ContentLength > 0) && (fileRight != null && fileRight.ContentLength > 0))
            {
                if ((fileLeft.FileName.EndsWith(".png")) && (fileRight.FileName.EndsWith(".png")))
                {
                    FileHandler fileHandler = new FileHandler(fileLeft, fileRight, Server.MapPath("~/Images/uploaded/"));
                    fileHandler.SaveHandledFiles();
                    fileHandler.GenerateImage();
                    return Redirect("~/Images/uploaded/" + fileHandler.RandomFolderName);
                    //fileHandler.DeleteHandledFiles();
                }

            }
            return RedirectToAction("Index");
        }
    }
}