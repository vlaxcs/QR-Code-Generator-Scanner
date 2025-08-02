using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace QR.Controllers
{
    [ApiController]
    [Route("qr/api/generate")]
    public class GeneratorController : ControllerBase
    {
        public class Message
        {
            public required string Title { get; set; }
            public required string Body { get; set; }
        }


        public class GenerateRequest
        {
            public required string Message { get; set; }
            public int reqVersion { get; set; }
            public int reqEccLevel { get; set; }
        }

        [HttpPost]
        public IActionResult Post([FromBody] GenerateRequest request)
        {
            Console.WriteLine($"📥 Received: {request.Message} | MinV: {request.reqVersion} | ECC: {request.reqEccLevel}");

            try
            {
                QRCode code = QRCodeGenerator.Generate(request.Message, request.reqEccLevel, request.reqVersion);
                using var img = code.GetImage();
                using var ms = new MemoryStream();

                img.Save(ms, new PngEncoder());

                Response.Headers.Add("Access-Control-Expose-Headers", "usedVersion, usedEccLevel");
                Response.Headers.Add("usedVersion", code.Version.ToString());
                Response.Headers.Add("usedEccLevel", code.ErrorCorrectionLevel.ToString());
                return File(ms.ToArray(), "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}