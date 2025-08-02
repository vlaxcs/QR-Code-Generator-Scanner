using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QR.Controllers
{
    [ApiController]
    [Route("qr/api/scan")]
    public class ScannerController : ControllerBase
    {
        private static int _counter = 0;

        public class Message
        {
            public required string Title { get; set; }
            public required string Body { get; set; }
        }

        public class Response
        {
            public required string Message { get; set; }
            public required int ErrorCorrectionLevel { get; set; }
            public required int Version { get; set; }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> PostAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new Message
                {
                    Title = "Invalid upload",
                    Body = "No image provided or image is empty."
                });
            }

            if (image.ContentType != "image/png")
            {
                return BadRequest(new Message
                {
                    Title = "Unsupported format",
                    Body = "Uploaded file must be a PNG image."
                });
            }

            string tempPath = Path.Combine(Path.GetTempPath(), $"{Interlocked.Increment(ref _counter)}_{Path.GetFileName(image.FileName)}");

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                QRCode code = QRCodeImageParser.Parse(tempPath);
                var decoded = QRCodeDecoder.DecodeQR(code);
                string messageText = decoded.ToString();

                Console.WriteLine(messageText);

                return Ok(new Response
                {
                    Message = messageText,
                    ErrorCorrectionLevel = code.ErrorCorrectionLevel,
                    Version = code.Version
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new Message
                {
                    Title = "Error decoding QR",
                    Body = ex.Message
                });
            }
            finally
            {
                try
                {
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"Cleanup error: {cleanupEx.Message}");
                }
            }
        }
    }
}