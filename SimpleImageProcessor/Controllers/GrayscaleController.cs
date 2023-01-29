using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Mvc;
using SimpleImageProcessor.Models;
using System.Drawing;
using System.IO;

namespace SimpleImageProcessor.Controllers
{
    public class GrayscaleController : Controller
    {

        public void grayscaleImage(FileUpload fileUpload, string path)
        {
            string filePath = path + fileUpload.file.FileName;

            //Get the image path
            var origImagePath = Path.Combine(filePath);

            //Initialize GcBitmap           
            GcBitmap origBmp = new GcBitmap(origImagePath);

            //Apply grayscale to image
            origBmp.ApplyEffect(GrayscaleEffect.Get(GrayscaleStandard.BT709));
            

            //Save scaled image to file
            origBmp.SaveAsJpeg(filePath);
        }

        public string grayscale(FileUpload fileUpload, string operation, string path)
        {

            //do image processing operation
            grayscaleImage(fileUpload, path);

            return "Converting to grayscale complete";
        }
    }
}
