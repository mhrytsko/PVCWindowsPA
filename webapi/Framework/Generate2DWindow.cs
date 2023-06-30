using Microsoft.CodeAnalysis.Text;
using PdfSharp.Drawing;
using SkiaSharp;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using webapi.Framework.BaseEnums;
using webapi.Tools;

namespace webapi.Framework
{
    public class Generate2DWindow
    {
        public static void GenerateImage(Models.Window janela, out MemoryStream stream, bool drawSchema = false, bool drawDimension = false, bool applyTextures = false)
        {
            // Define a largura e a altura da janela
            int windowWidth = janela.Width ?? 0;
            int windowHeight = janela.Height ?? 0;
            
            // Define a largura do perfil
            int profileWidth = 70;

            // Calcular matriz
            int numRows = janela.LeafConfigurations.Any() ? janela.LeafConfigurations.Max(p => p.X ?? 0) + 1 : 0;
            var paneMatrix = Enumerable.Range(0, numRows)
                .Select(_ => janela.LeafConfigurations.Where(p => p.X == _).OrderBy(p => p.Y).ToList()).ToList();

            // Descobrir quantos niveis de medidas da largura/altura que vão ser precisos
            int yLevels = 1 + paneMatrix.Sum(row => row.Count > 1 ? 1 : 0);
            int xLevels = numRows > 1 ? 2 : 1;

            // Definir margem por cada nivel de medidas das secções
            int margemSchema = 175;
            int increaseLine = 50;

            int increaseMargemX = margemSchema * xLevels;
            int increaseMargemY = margemSchema * yLevels;


            // Calcular tamanho da imagem/canvas
            int canvasWidth = windowWidth + (drawDimension ? increaseMargemX : 0) + (increaseLine * 2);
            int canvasHeight = windowHeight + (drawDimension ? increaseMargemY : 0) + (increaseLine * 2);

            // Estilo reutilizavel do desenho
            //var whitePVCColor = new SKColor(255, 255, 255);
            //var whitePVCShader = SKShader.CreateColor(whitePVCColor);

            using var clearPaint = new SKPaint { Color = SKColors.Transparent, BlendMode = SKBlendMode.Src };

            using var paintWindowFrameStroke = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 4 };
            using var paintWindowBackground = GetBackgroundImage();

            using var paintProfile = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill };
            using var paintProfileStroke = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };

            using var paintGlass = new SKPaint() { Color = new SKColor(110, 197, 234, 26), Style = SKPaintStyle.Fill };
            using var paintGlassFrosted = new SKPaint() { Color = new SKColor(234, 240, 240, 250), Style = SKPaintStyle.Fill };
            using var paintGlassStroke = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };

            using var paintGlassBackground = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill };

            using var handlePaint = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill };
            using var handlePaintStroke = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };

            using var linePaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
            using var paintSchema = new SKPaint { Color = SKColors.LightBlue, Style = SKPaintStyle.StrokeAndFill, StrokeWidth = 6 };


            // Aplicar texturas
            if (applyTextures)
            {
                if(janela.IndorColor != null)
                {
                    if(janela.IndorColor.ColorType == ColorType.Solid && !string.IsNullOrWhiteSpace(janela.IndorColor.HexCode))
                    {
                        var textureColor = SKColor.Parse(janela.IndorColor.HexCode);
                        paintProfile.Color = textureColor;
                    }
                    else if (janela.IndorColor.ColorType == ColorType.Pattern && janela.IndorColor.Image != null)
                    {
                        using var textureStream = new MemoryStream(janela.IndorColor.Image.File);
                        using var textureBitmap = SKBitmap.Decode(textureStream);

                        /*var rotatedBitmap = new SKBitmap(textureBitmap.Height, textureBitmap.Width);
                        using var rCcanvas = new SKCanvas(rotatedBitmap);
                        rCcanvas.Clear(SKColors.Transparent);

                        // Rotacionar 90 graus
                        rCcanvas.Translate(0, textureBitmap.Height);
                        rCcanvas.RotateDegrees(-90);

                        rCcanvas.DrawBitmap(textureBitmap, 0, 0);
                        rCcanvas.Flush();*/

                        var shader = SKShader.CreateBitmap(textureBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                        paintProfile.Shader = shader;

                    }
                }

                paintGlass.Color = new SKColor(110, 197, 234, 126);

                handlePaintStroke.StrokeWidth = 4;
                paintProfileStroke.StrokeWidth = 4;
                paintGlassStroke.StrokeWidth = 4;
            }

            // Fonte do texto
            using var fontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Expanded, SKFontStyleSlant.Upright);
            using var typeface = SKTypeface.FromFamilyName("Segoe UI", fontStyle);
            using var paintText = new SKPaint() { Color = SKColors.Black, TextSize = 100.0f, TextAlign = SKTextAlign.Center, Typeface = typeface };


            // Cria um novo bitmap
            using var bitmap = new SKBitmap(canvasWidth, canvasHeight);

            // Cria um novo canvas
            using var canvas = new SKCanvas(bitmap);

            // Limpa o canvas com a cor branca
            canvas.Clear(SKColors.Transparent);

            /*var scale = Math.Min((float)canvasWidth / windowWidth, (float)canvasHeight / windowHeight);
            if (float.IsNaN(scale))
                scale = 1;
            canvas.Scale(scale);*/


            // Draw window frame
            canvas.DrawRect(new SKRect(increaseLine, increaseLine, windowWidth + increaseLine, windowHeight + increaseLine), paintProfile);
            canvas.DrawRect(new SKRect(increaseLine, increaseLine, windowWidth + increaseLine, windowHeight + increaseLine), paintWindowFrameStroke);

            for (int rowIndex = 0; rowIndex < paneMatrix.Count; rowIndex++)
            {
                var row = paneMatrix[rowIndex];
                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    var pane = row[colIndex];

                    var paneBorder = pane.OpeningSystem == WindowOpeningType.Fixed ? 0 : profileWidth / 3;

                    var x = row.Take(colIndex).Sum(p => p.Width) + paneBorder + increaseLine;
                    var y = (rowIndex == 0 ? 0 : paneMatrix.Take(rowIndex).Sum(r => r.Max(p => p.Height))) + paneBorder + increaseLine;

                    var paneWidth = pane.Width - paneBorder - paneBorder;
                    var paneHeight = pane.Height - paneBorder - paneBorder;

                    // Draw pane
                    canvas.DrawRect(new SKRect(x, y, x + paneWidth, y + paneHeight), paintProfile);
                    canvas.DrawRect(new SKRect(x, y, x + paneWidth, y + paneHeight), paintProfileStroke);


                    // Draw glass
                    var glassX = x + profileWidth;
                    var glassY = y + profileWidth;
                    var glassWidth = paneWidth - 2 * profileWidth;
                    var glassHeight = paneHeight - 2 * profileWidth;
                    var glassRect = new SKRect(glassX, glassY, glassX + glassWidth, glassY + glassHeight);

                    canvas.DrawRect(glassRect, clearPaint);
                    if (applyTextures)
                        canvas.DrawRect(glassRect, paintWindowBackground);

                    //if (applyTextures)
                        //canvas.DrawRect(glassRect, paintGlassBackground);

                    canvas.DrawRect(glassRect, pane.Frosted ? paintGlassFrosted : paintGlass);
                    canvas.DrawRect(glassRect, paintGlassStroke);


                    // Draw handle if necessary
                    if (pane.OpeningSystem != WindowOpeningType.Fixed)
                    {
                        var profileCenter = (float)(profileWidth * 0.5);
                        var handleHeight = 140;
                        var handleWidth = 20;
                        var handleAuxHeight = 70;
                        var handleAuxWidth = 30;
                        var handleWidthHalf = (float)(handleWidth * 0.5);
                        var handleAuxWidthHalf = (float)(handleAuxWidth * 0.5);
                        var hwDiff = handleAuxWidthHalf - handleWidthHalf;

                        if (pane.OpeningDirection == WindowOpeningDirection.LeftRight)
                        {
                            float x1 = x + profileCenter - handleAuxWidthHalf,
                                y1 = y + (paneHeight / 2) - (handleAuxHeight / 2),
                                x2 = x + profileCenter - handleWidthHalf,
                                y2 = y + paneHeight / 2 - hwDiff;


                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x1, y1, x1 + handleAuxWidth, y1 + handleAuxHeight), 20), handlePaint); // left handle - aux
                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x1, y1, x1 + handleAuxWidth, y1 + handleAuxHeight), 20), handlePaintStroke); // left handle - aux
                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x2, y2, x2 + handleWidth, y2 + handleHeight), 20), handlePaint); // left handle
                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x2, y2, x2 + handleWidth, y2 + handleHeight), 20), handlePaintStroke); // left handle
                        }
                        else if (pane.OpeningDirection == WindowOpeningDirection.RightLeft)
                        {
                            float x1 = x + paneWidth - profileCenter - handleAuxWidthHalf,
                                y1 = y + paneHeight / 2 - handleAuxHeight / 2,
                                x2 = x + paneWidth - profileCenter - handleWidthHalf,
                                y2 = y + paneHeight / 2 - hwDiff;

                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x1, y1, x1 + handleAuxWidth, y1 + handleAuxHeight), 20), handlePaint); // right handle - aux
                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x1, y1, x1 + handleAuxWidth, y1 + handleAuxHeight), 20), handlePaintStroke); // right handle - aux
                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x2, y2, x2 + handleWidth, y2 + handleHeight), 20), handlePaint); // right handle
                            canvas.DrawRoundRect(new SKRoundRect(new SKRect(x2, y2, x2 + handleWidth, y2 + handleHeight), 20), handlePaintStroke); // right handle
                        }
                    }

                    if(drawSchema)
                    {
                        // Draw opening direction lines
                        if (pane.OpeningSystem == WindowOpeningType.SideHung || pane.OpeningSystem == WindowOpeningType.TiltAndTurn)
                        {
                            if (pane.OpeningDirection == WindowOpeningDirection.RightLeft)
                            {
                                canvas.DrawLine(new SKPoint(glassX, glassY), new SKPoint(glassX + glassWidth, glassY + glassHeight / 2), linePaint);
                                canvas.DrawLine(new SKPoint(glassX, glassY + glassHeight), new SKPoint(glassX + glassWidth, glassY + glassHeight / 2), linePaint);
                            }
                            else if (pane.OpeningDirection == WindowOpeningDirection.LeftRight)
                            {
                                canvas.DrawLine(new SKPoint(glassX + glassWidth, glassY), new SKPoint(glassX, glassY + glassHeight / 2), linePaint);
                                canvas.DrawLine(new SKPoint(glassX + glassWidth, glassY + glassHeight), new SKPoint(glassX, glassY + glassHeight / 2), linePaint);
                            }
                        }

                        if (pane.OpeningSystem == WindowOpeningType.TiltAndTurn || pane.OpeningSystem == WindowOpeningType.TiltOnly)
                        {
                            canvas.DrawLine(new SKPoint(glassX, glassY + glassHeight), new SKPoint(glassX + glassWidth / 2, glassY), linePaint);
                            canvas.DrawLine(new SKPoint(glassX + glassWidth, glassY + glassHeight), new SKPoint(glassX + glassWidth / 2, glassY), linePaint);
                        }
                    }
                }
            }


            // Desenhar esquema técnico
            if(drawDimension)
            {
                SKPoint rightUp = new SKPoint(windowWidth + increaseLine, increaseLine),
                        rightDown = new SKPoint(windowWidth + increaseLine, windowHeight + increaseLine),
                        leftDown = new SKPoint(increaseLine, windowHeight + increaseLine);



                // Alturas
                { // Total
                    var a1 = new SKPoint(rightUp.X + increaseMargemX, rightUp.Y);
                    var b1 = new SKPoint(rightDown.X + increaseMargemX, rightDown.Y);

                    canvas.DrawLine(rightUp, GetIncreasePoint(a1, increaseLine), paintSchema);
                    canvas.DrawLine(rightDown, GetIncreasePoint(b1, increaseLine), paintSchema);
                    canvas.DrawLine(a1, b1, paintSchema);

                    using var totalPath = GetTextPath(b1, a1);
                    canvas.DrawTextOnPath(string.Format("{0}", janela.Height), totalPath, hOffset: 0, vOffset: 25, paintText);
                }
                if(xLevels > 1)
                { // Secções
                    SKPoint prevA1 = rightUp;
                    SKPoint prevB1 = new SKPoint(rightUp.X + margemSchema, rightUp.Y);
                    foreach (var row in paneMatrix)
                    {
                        var rowHeight = row.Max(col => col.Height);
                        var a1 = new SKPoint(prevA1.X, prevA1.Y + rowHeight);
                        var b1 = new SKPoint(prevA1.X + margemSchema, prevA1.Y + rowHeight);

                        canvas.DrawLine(a1, b1, paintSchema);
                        canvas.DrawLine(prevB1, b1, paintSchema);

                        using var heightPath = GetTextPath(b1, prevB1);
                        canvas.DrawTextOnPath(string.Format("{0}", rowHeight), heightPath, hOffset: 0, vOffset: 25, paintText);

                        prevA1 = a1;
                        prevB1 = b1;
                    }
                }



                // Larguras
                {
                    var a2 = new SKPoint(leftDown.X, leftDown.Y + increaseMargemY);
                    var b2 = new SKPoint(rightDown.X, rightDown.Y + increaseMargemY);

                    canvas.DrawLine(leftDown, GetIncreasePoint(a2, 0, increaseLine), paintSchema);
                    canvas.DrawLine(rightDown, GetIncreasePoint(b2, 0, increaseLine), paintSchema);
                    canvas.DrawLine(a2, b2, paintSchema);

                    using var totalPath = GetTextPath(a2, b2);
                    canvas.DrawTextOnPath(string.Format("{0}", janela.Width), totalPath, hOffset: 0, vOffset: 25, paintText);
                }

                if (yLevels > 1)
                { // Secções
                    
                    var levelCount = 1;
                    foreach (var row in paneMatrix)
                    {
                        if (row.Count <= 1)
                            continue;

                        int levelMargem = margemSchema * levelCount++;

                        SKPoint prevA1 = leftDown;
                        SKPoint prevB1 = new SKPoint(leftDown.X, leftDown.Y + levelMargem);

                        foreach(var col in row)
                        {
                            var a1 = new SKPoint(prevA1.X + col.Width, prevA1.Y);
                            var b1 = new SKPoint(prevA1.X + col.Width, prevA1.Y + levelMargem);

                            canvas.DrawLine(a1, b1, paintSchema);
                            canvas.DrawLine(prevB1, b1, paintSchema);

                            using var widthPath = GetTextPath(prevB1, b1);
                            canvas.DrawTextOnPath(string.Format("{0}", col.Width), widthPath, hOffset: 0, vOffset: 25, paintText);

                            prevA1 = a1;
                            prevB1 = b1;
                        }
                    }
                }
            }


            // Salva o bitmap como um arquivo PNG
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            stream = new MemoryStream();
            data.SaveTo(stream);

            // Retorna o stream como a imagem
            stream.Position = 0;
        }

        private static SKPoint GetMidPoint(SKPoint point1, SKPoint point2)
        {
            float xMid = (point1.X + point2.X) / 2;
            float yMid = (point1.Y + point2.Y) / 2;

            return new SKPoint(xMid, yMid);
        }

        private static SKPoint GetDecreasePoint(SKPoint point, int decreaseX = 0, int decreaseY = 0)
        {
            return new SKPoint(point.X - decreaseX, point.Y - decreaseY);
        }

        private static SKPoint GetIncreasePoint(SKPoint point, int decreaseX = 0, int decreaseY = 0)
        {
            return new SKPoint(point.X + decreaseX, point.Y + decreaseY);
        }

        private static SKPath GetTextPath(SKPoint point1, SKPoint point2)
        {
            var path = new SKPath();
            path.MoveTo(point1);
            path.LineTo(point2);
            return path;
        }

        private static SKPaint GetBackgroundImage()
        {
            // Crie um Stream a partir do Resource
            using var stream = new MemoryStream();
            System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("webapi.Resources.lisboa_paisagem.jpg")?.CopyTo(stream);
            stream.Position = 0; //reset the position to the beginning of the stream

            // Decodifique o Stream para uma SKBitmap
            using var bitmap = SKBitmap.Decode(stream);

            // Use a SKBitmap como desejar
            // Criar um SKShader a partir da SKBitmap para usar como um background
            var shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Decal, SKShaderTileMode.Decal);

            return new SKPaint { Shader = shader };
        }

        public static string GenerateImageBase64(Models.Window janela, bool drawSchema = false, bool drawDimension = false, bool applyTextures = false)
        {
            GenerateImage(janela, out MemoryStream stream, drawSchema, drawDimension, applyTextures);

            // Obter o array de bytes do stream
            byte[] imageBytes = stream.ToArray();
            stream.Dispose();

            // Converte o array de bytes em uma string base64
            string base64Image = Convert.ToBase64String(imageBytes);

            return base64Image;
        }
    }
}
