using Tesseract;

namespace Vaccine.API.Services
{
    public interface IOcrService
    {
        string ExtractText(Stream imageStream);
    }

    public class OcrService : IOcrService
    {
        private readonly string _tessDataPath;

        public OcrService(IWebHostEnvironment env)
        {
            _tessDataPath = Path.Combine(env.WebRootPath, "tessdata");
            Console.WriteLine("Tesseract path: " + _tessDataPath);
        }

        public string ExtractText(Stream imageStream)
        {
            try
            {
                using var engine = new TesseractEngine(_tessDataPath, "vie", EngineMode.Default);
                using var img = Pix.LoadFromMemory(ReadStream(imageStream));
                using var page = engine.Process(img);
                return page.GetText();
            }
            catch (Exception ex)
            {
                Console.WriteLine("OCR error: " + ex.Message);
                return $"[ERROR] {ex.Message}";
            }
        }


        private byte[] ReadStream(Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
