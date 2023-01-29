using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using Microsoft.AspNetCore.Mvc;
using SimpleImageProcessor.Models;
using System.Drawing;
using System.IO;

namespace SimpleImageProcessor.Controllers
{
    public class FlipController : Controller
    {
        public void flipImage(FileUpload fileUpload, string path, string change)
        {
            string filePath = path + fileUpload.file.FileName;

            //Get the image path
            var origImagePath = Path.Combine(filePath);

            //Initialize GcBitmap
            GcBitmap origLargeBmp = new GcBitmap();

            //Load image from file
            origLargeBmp.Load(origImagePath);

            GcBitmap rotatebmp = new GcBitmap();

            if (change == "horizontal")
            {
                rotatebmp = origLargeBmp.FlipRotate(FlipRotateAction.FlipHorizontal);
            }
            else if (change == "vertical")
            {
                rotatebmp = origLargeBmp.FlipRotate(FlipRotateAction.FlipVertical);
            }

            //Save scaled image to file
            rotatebmp.SaveAsJpeg(filePath);
        }

        public string flip(FileUpload fileUpload, string operation, string path)
        {
            //extract the change corresponding to the chosen operation
            var change = operation.Split(' ')[1];

            //extract the frequency (number of times to perform the operation)
            string operationfrequecy = operation.Split(' ')[2];

            //parse to int
            int frequecy = int.Parse(operationfrequecy);

            if (change != "horizontal" && change != "vertical")
            {
                return change.ToString()+" operation is invalid";
            }

            if (frequecy > 5 || frequecy < 0)
            {
                return "invalid frequency";
            }

            //loop by frequency
            for (var i = 0; i < frequecy; i++)
            {
                //do image processing operation
                flipImage(fileUpload, path, change);
            }

            return "Flipping complete";
        }
    }
}
