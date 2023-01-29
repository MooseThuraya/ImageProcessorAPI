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
    public class ResizeController : Controller
    {

        public void resizeImage(FileUpload fileUpload, string path, int height, int width)
        {
            string filePath = path + fileUpload.file.FileName;

            byte[] photoBytes = System.IO.File.ReadAllBytes(filePath);
            Size size = new Size(height, width);
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(filePath)
                                    .Resize(size)
                                    .Save(filePath);
                    }
                    // Do something with the stream.
                }
            }
        }

        public string resize(FileUpload fileUpload, string operation, string path)
        {
            //extract the height
            var operationHeight = operation.Split(' ')[1];

            //extract the width
            string operationWidth = operation.Split(' ')[2];

            //check if width and height cannot be parsed
            if (!int.TryParse(operationHeight, out int valueHeight) || !int.TryParse(operationWidth, out int valueWidth))
            {
                return "The inputted operation is invalid";
            }

            //parse height to int
            int height = int.Parse(operationHeight);

            //parse width to int
            int width = int.Parse(operationWidth);

            //do image processing operation
            resizeImage(fileUpload, path, height, width);

            return "Resizing complete";
        }
    }
}
