using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Mvc;
using SimpleImageProcessor.Models;
using System;
using System.Drawing;
using System.IO;

namespace SimpleImageProcessor.Controllers
{
    public class RotateController : Controller
    {
        public void rotateImageByDegrees(FileUpload fileUpload, string path, string change)
        {
            int degrees = 0;

            if (change == "right")
            {
                degrees = 90;
            }
            else if (change == "left")
            {
                degrees = 270;
            }
            else
            {
                degrees = int.Parse(change);
            }


            string filePath = path + fileUpload.file.FileName;

            byte[] photoBytes = System.IO.File.ReadAllBytes(filePath);
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(filePath)
                                    .Rotate(degrees)
                                    .Save(filePath);
                    }
                    // Do something with the stream.
                }
            }
        }

        public string rotate(FileUpload fileUpload, string operation, string path)
        {
            //extract the change corresponding to the chosen operation
            var change = operation.Split(' ')[1];

            //extract the frequency (number of times to perform the operation)
            string operationfrequecy = operation.Split(' ')[2];

            //parse to int
            int frequecy = int.Parse(operationfrequecy);

            if (change != "left" && change != "right" && !int.TryParse(change, out int value))
            {
                return change.ToString() + " operation is invalid";
            }

            //check for invalid frequency
            if (frequecy > 5 || frequecy < 0)
            {
                return "invalid frequency";
            }

            //loop by frequency
            for (var i = 0; i < frequecy; i++)
            {

                //do image processing operation
                rotateImageByDegrees(fileUpload, path, change);

            }
            return "Rotating complete";
        }
    }
}
