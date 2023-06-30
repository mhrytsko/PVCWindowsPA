using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


namespace webapi.Tools
{
    /*
     //var path = "C:\\Users\\Maksim\\source\\repos\\ProtoPG\\test_data\\my_window_2.jpg";

        //var image = System.IO.File.ReadAllBytes(path);

        

        //var ods = new ObjectDetectionService();
        //Task.Run(() => ods.DetectObjects(image));
     */

    public class ObjectDetectionService
    {
        private readonly ApiKeyServiceClientCredentials credentials;
        private readonly string endpoint;

        private readonly int maxWidth = 10000;
        private readonly int maxHeight = 10000;
        private readonly long maxSizeInBytes = 4 * 1024 * 1024; // 4 MB

        public ObjectDetectionService()
        {
            // Add your Computer Vision subscription key and endpoint to your environment variables.
            string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY") ?? "6084a85aa8d44377b397ee8c2c456b40";
            this.credentials = new ApiKeyServiceClientCredentials(subscriptionKey);
            this.endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT") ?? "https://proto-window-size.cognitiveservices.azure.com/";
        }

        public async Task DetectObjects(byte[] imageBytes)
        {
            // Create a client object and authenticate with your key and endpoint.
            ComputerVisionClient client = new ComputerVisionClient(this.credentials)
            {
                Endpoint = this.endpoint
            };

            var featureTypes = new List<VisualFeatureTypes> { 
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Objects
            };

            IList<VisualFeatureTypes?> nullableFeatures = featureTypes
                .Select(feature => (VisualFeatureTypes?)feature)
                .ToList();


            using var stream = new MemoryStream(imageBytes);
            

            var resizedImageStream = ResizeImageIfRequired(stream, maxWidth, maxHeight, maxSizeInBytes);


            using (FileStream fileStream = new FileStream(@"C:\Users\Maksim\source\repos\ProtoPG\test_data\resized\" + Guid.NewGuid().ToString() + ".jpeg", FileMode.Create, FileAccess.Write))
            {
                resizedImageStream.WriteTo(fileStream);
            }

            //var response = await client.DetectObjectsInStreamAsync(resizedImageStream);

            var response = await client.AnalyzeImageInStreamAsync(resizedImageStream, nullableFeatures);

            /*
             // Call the API with the URL of the image to detect objects.
            var response = await client.DetectObjectsAsync(imageUrl);
             */


            // Get the detected objects from the response.
            var objects = response.Objects;

            // Loop through the detected objects and print their properties.
            //foreach (var obj in objects)
            //{
             //   Console.WriteLine($"{obj.ObjectProperty}: {obj.Rectangle.X}, {obj.Rectangle.Y}, {obj.Rectangle.W}, {obj.Rectangle.H}");
            //}




        }

        public static MemoryStream ResizeImageIfRequired(Stream inputStream, int maxWidth, int maxHeight, long maxSizeInBytes)
        {
            var outputStream = new MemoryStream();

            using (var image = Image.Load(inputStream))
            {
                // Verifica se a imagem excede os limites de tamanho (em bytes)
                if (inputStream.Length > maxSizeInBytes)
                {
                    var options = new ResizeOptions
                    {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.Max
                    };

                    image.Mutate(x => x.Resize(options));
                }

                // Salva a imagem no outputStream com qualidade ajustada, se necessário
                int quality = 100;
                do
                {
                    outputStream.SetLength(0);
                    image.Save(outputStream, new JpegEncoder() { Quality = quality });
                    quality -= 5;
                }
                while (outputStream.Length > maxSizeInBytes && quality > 0);

                // Salva a imagem no outputStream, independentemente de ter sido redimensionada ou não
                //image.Save(outputStream, new JpegEncoder());
            }

            outputStream.Position = 0;
            return outputStream;
        }

        public static MemoryStream ValidateAndResizeImage(Stream inputStream, int maxWidth, int maxHeight, long maxSizeInBytes)
        {
            if (inputStream.Length > maxSizeInBytes)
            {
                throw new ArgumentException($"A imagem é maior que o tamanho máximo permitido ({maxSizeInBytes / (1024 * 1024)} MB)");
            }

            var outputStream = new MemoryStream();
            using (var image = Image.Load(inputStream))
            {
                // Verifique se a imagem precisa ser redimensionada
                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    var options = new ResizeOptions
                    {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.Max
                    };

                    image.Mutate(x => x.Resize(options));
                }

                image.Save(outputStream, new JpegEncoder());
            }

            outputStream.Position = 0;
            return outputStream;
        }


        public static MemoryStream ResizeImage(Stream inputStream, int maxWidth, int maxHeight)
        {
            var outputStream = new MemoryStream();

            using (var image = Image.Load(inputStream))
            {
                var options = new ResizeOptions
                {
                    Size = new Size(maxWidth, maxHeight),
                    Mode = ResizeMode.Max
                };

                image.Mutate(x => x.Resize(options));
                image.Save(outputStream, new JpegEncoder());
            }

            outputStream.Position = 0;
            return outputStream;
        }
    }
}
