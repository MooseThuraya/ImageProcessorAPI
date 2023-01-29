using com.sun.tools.javac.util;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Mvc;
using SimpleImageProcessor.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SimpleImageProcessor.Controllers
{
    public class ThumbnailController : Controller
    {
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public static void thumbnailImage(FileUpload fileUpload, string path, int thumbWidth, int thumbHeight)
        {

            string filePath = path + fileUpload.file.FileName;
            System.Drawing.Image orignalImage = System.Drawing.Image.FromFile(filePath);

            orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

            int newHeight = orignalImage.Height * thumbWidth / orignalImage.Width;
            int newWidth = thumbWidth;

            if (newHeight > thumbHeight)
            {
                newWidth = orignalImage.Width * thumbHeight / orignalImage.Height;
                newHeight = thumbHeight;
            }

            //Generate a thumbnail image
            System.Drawing.Image thumbImage = orignalImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            // Saveresized picture
            var qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            var quality = (long)100; //Image Quality 
            var ratio = new EncoderParameter(qualityEncoder, quality);
            var codecParams = new EncoderParameters(1);
            codecParams.Param[0] = ratio;
            //Rightnow I am saving JPEG only you can choose other formats as well
            var codecInfo = GetEncoder(ImageFormat.Jpeg);


            thumbImage.Save(filePath, codecInfo, codecParams);

            //Dispose unnecessory objects
            orignalImage.Dispose();
            thumbImage.Dispose();
        }

        public string thumbnail(FileUpload fileUpload, string operation, string path)
        {

            thumbnailImage(fileUpload, path, 100, 150);

            return "Converting to thumbnail complete";
        }


    }
}
