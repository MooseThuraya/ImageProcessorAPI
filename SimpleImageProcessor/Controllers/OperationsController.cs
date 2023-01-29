using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SimpleImageProcessor.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SimpleImageProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class OperationsController : Controller
    {
            public static IWebHostEnvironment _webHostEnvironment;
            string path;
            string filePath;

            public OperationsController(IWebHostEnvironment webHostEnvironment)
            {
                _webHostEnvironment = webHostEnvironment;
            }
        /// <summary>
        /// Process operations on image
        /// </summary>
        /// <remarks>
        /// Sample Image Upload <br></br>
        /// Operations format: <br></br>
        ///     - <b>Note</b>: frequency is an integer [0,10] <br></br>
        ///     - <b>Flip</b>: flip + 'vertical' or 'horizontal' + frequency <br></br>
        ///     - <b>Rotate</b>: rotate + 'left' or 'right' or '+/-degrees' + frequency <br></br>
        ///     - <b>Resize</b>: resize + width + height <br></br>
        ///     - <b>Grayscale</b>: grayscale <br></br>
        ///     - <b>Thumbnail</b>: thumbnail
        /// </remarks>
        /// <response code="200">Successful</response>
        /// <response code="408">Operations exceeded 10</response>
        /// <response code="409">There was no inputted image</response>
        /// <response code="410">Inputted image larger than 400KB</response>
        /// <response code="411">Image operation is null</response>
        /// <response code="412">invalid frequency</response>
        /// <response code="413">The inputted operation is invalid</response>
        /// <response code="414">The uploaded image no longer exists</response>
        /// <response code="415">Input image is not acceptable</response>
        /// <response code="416">An input caused an exception to be thrown</response>
        /// <param name="fileUpload"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        [HttpPost]
            public async Task<IActionResult> operations([FromForm] FileUpload fileUpload, [FromForm] IEnumerable<string> operations)
            {
                try
                {
                    //check for operations count
                    if (operations.Count() > 10)
                    {
                        return StatusCode(408, "Operations exceeded 10");

                    }

                    //check if there is no inputted image
                    if (fileUpload.file == null)
                    {
                        return StatusCode(409, "There was no inputted image");
                    }
   
                    if(fileUpload.file.Length > 0)
                    {
                        path = _webHostEnvironment.WebRootPath + "\\uploads\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }//end of if

                        var filePath = path + fileUpload.file.FileName;

                        using (FileStream fileStream = System.IO.File.Create(path + fileUpload.file.FileName))
                        {
                            fileUpload.file.CopyTo(fileStream);
                            fileStream.Flush();
                            fileStream.Close();
                        
                        //check for image size
                        decimal size = Math.Round(((decimal)fileUpload.file.Length / (decimal)1024), 2);

                        //check if file exists
                        if (System.IO.File.Exists(path + fileUpload.file.FileName) && size > 405)
                        {
                            //delete image after
                            System.IO.File.Delete(path + fileUpload.file.FileName);
                            return StatusCode(410, "Inputted image larger than 400KB");
                        }
                            

                        foreach (string operation in operations)
                            {
                                if(operation == null)
                                {
                                    //delete image after
                                    System.IO.File.Delete(path + fileUpload.file.FileName);
                                    return StatusCode(411, "Image operation is null");
                                }
                                //extract operation type (first word)
                                var operationType = operation.Split(' ')[0];

                                // creates consistent capitalization for case checking
                                operationType = operationType.ToLower();


                            switch (operationType)
                                {
                                    case "flip":
                                        FlipController flip = new FlipController();

                                        //call flip method
                                        string result = flip.flip(fileUpload, operation, path);

                                        if (result.Contains("operation is invalid"))
                                        {
                                            return StatusCode(413, result);
                                        }

                                    if (result == "invalid frequency")
                                        {
                                            System.IO.File.Delete(filePath);
                                            return StatusCode(412, "invalid frequency");
                                        }
                                        break;

                                    case "rotate":
                                        RotateController rotate = new RotateController();

                                        
                                        // enter the rest of the strings including frequency
                                        result = rotate.rotate(fileUpload, operation, path);
                                        if (result.Contains("operation is invalid"))
                                        {
                                            return StatusCode(413, result);
                                        }

                                        if (result == "invalid frequency")
                                        {
                                            System.IO.File.Delete(filePath);
                                            return StatusCode(412, "invalid frequency");
                                        }
                                        break;

                                    case "resize":
                                        ResizeController resize = new ResizeController();

                                        // enter the rest of the strings including frequency
                                        result = resize.resize(fileUpload, operation, path);
                                        if (result.Contains("The inputted operation is invalid"))
                                        {
                                            return StatusCode(413, "The width or height is invalid");
                                        }
                                    break;

                                    case "grayscale":
                                        GrayscaleController grayscale = new GrayscaleController();
                                        
                                        grayscale.grayscale(fileUpload, operation, path);
                                    break;

                                    case "thumbnail":
                                        ThumbnailController thumbnail = new ThumbnailController();

                                        thumbnail.thumbnail(fileUpload, operation, path);
                                        break;

                                    default:
                                        //if operation does not exist, continue as specified in documentation
                                        System.IO.File.Delete(filePath);
                                        return StatusCode(413, "The inputted operation is invalid");

                                }//end switch

                            }//end of foreach

                    }//end of using #1

                        filePath = path + fileUpload.file.FileName;

                        if (System.IO.File.Exists(filePath))
                        {
                            byte[] b = System.IO.File.ReadAllBytes(filePath);

                            //delete image since it is already cached into "b"
                            System.IO.File.Delete(filePath);
                            return File(b, "image/png");
                        }
                        return StatusCode(414, "The uploaded image no longer exists");

                }// end of if
                    
                else
                {
                    return StatusCode(415, "Input image is not acceptable");
                }// end of else

                }//end of try

                catch (Exception ex)
                {
                    System.IO.File.Delete(path + fileUpload.file.FileName);
                    return StatusCode(416, "An input caused an exception to be thrown");
                }//end of catch
            }// end of Post    
    }
}
