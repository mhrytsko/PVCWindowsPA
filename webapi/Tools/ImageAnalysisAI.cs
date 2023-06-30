//using Azure;
//using Azure.AI.Vision.Common.Input;
//using Azure.AI.Vision.Common.Options;
//using Azure.AI.Vision.ImageAnalysis;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SixLabors.ImageSharp.Formats;
using PdfSharp.Drawing;

namespace webapi.Tools
{
    public class ImageAnalysisAI
    {
        private readonly ApiKeyServiceClientCredentials credentials;
        private readonly string endpoint;

        private readonly int maxWidth = 10000;
        private readonly int maxHeight = 10000;
        private readonly long maxSizeInBytes = 4 * 1024 * 1024; // 4 MB

        private const double Euro2CoinDiameter = 25.75;

        public ImageAnalysisAI()
        {
            // Add your Computer Vision subscription key and endpoint to your environment variables.
            string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY") ?? "f12d7a3d73c249e993d6510e35cb7512";
            this.credentials = new ApiKeyServiceClientCredentials(subscriptionKey);
            this.endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT") ?? "https://projetoacademico.cognitiveservices.azure.com/";
        }


        public async Task<object> DetectObjects(IFormFile file)
        {
            // Create a client object and authenticate with your key and endpoint.
            ComputerVisionClient client = new ComputerVisionClient(this.credentials)
            {
                Endpoint = this.endpoint
            };

            using var resizedImageStream = ResizeImageIfRequired(file, maxWidth, maxHeight, maxSizeInBytes);

            // Call the API to detect objects.

            /*var featureTypes = new List<VisualFeatureTypes?> {
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Objects
            };*/

            //var response = await client.AnalyzeImageInStreamAsync(resizedImageStream, featureTypes, language: "en");

            var response = await client.DetectObjectsInStreamAsync(resizedImageStream);


            // Get the detected objects from the response.
            var objects = response.Objects;

            var window = objects.Where(obj => obj.ObjectProperty == "window").OrderBy(obj => obj.Confidence).FirstOrDefault();
            var coin = objects.Where(obj => obj.ObjectProperty == "coin").OrderBy(obj => obj.Confidence).FirstOrDefault();

            double windowWidth = 0,
                windowHeight = 0;
            if(window != null && coin != null)
            {
                double scaleFactor = Euro2CoinDiameter / coin.Rectangle.W;

                windowWidth = Math.Round(window.Rectangle.W * scaleFactor, 0);
                windowHeight = Math.Round(window.Rectangle.H * scaleFactor, 0);
            }

            return new
            {
                success = window != null && coin != null,
                window,
                coin,
                windowWidth,
                windowHeight
            };
        }

        public static MemoryStream ResizeImageIfRequired(IFormFile file, int maxWidth, int maxHeight, long maxSizeInBytes)
        {
            var outputStream = new MemoryStream();

            using (var image = Image.Load(file.OpenReadStream()))
            {
                // Verifica se a imagem excede os limites de tamanho (em bytes)
                if (file.Length > maxSizeInBytes)
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
