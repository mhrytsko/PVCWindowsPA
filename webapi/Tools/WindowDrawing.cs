using SkiaSharp;

namespace webapi.Tools
{
    public class WindowDrawing
    {
        public void Draw(SKSurface surface, int canvasWidth, int canvasHeight, int windowWidth, int windowHeight, int profileWidth, List<Pane> panes)
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var scale = Math.Min((float)canvasWidth / windowWidth, (float)canvasHeight / windowHeight);
            canvas.Scale(scale);

            var numRows = panes.Max(pane => pane.Row) + 1;
            var paneMatrix = new List<List<Pane>>(numRows);

            for (int i = 0; i < numRows; i++)
            {
                paneMatrix.Add(new List<Pane>());
            }

            // Inicializar o paneMatrix com dados das folhas
            foreach (var pane in panes)
            {
                paneMatrix[pane.Row].Add(pane);
            }

            // Draw window frame
            using (var paint = new SKPaint { Color = SKColors.White })
            {
                canvas.DrawRect(0, 0, windowWidth, windowHeight, paint);
            }

            for (int rowIndex = 0; rowIndex < paneMatrix.Count; rowIndex++)
            {
                var row = paneMatrix[rowIndex];

                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    var pane = row[colIndex];
                    var paneBorder = pane.Type == PaneType.Fixed || pane.ClosingSystem == 0 ? 0 : profileWidth / 3;

                    var x = row.Take(colIndex).Sum(p => p.Width) + paneBorder;
                    var y = paneMatrix.Take(rowIndex).Select(row => row.Max(p => p.Height)).Sum() + paneBorder;

                    var paneWidth = pane.Width - paneBorder - paneBorder;
                    var paneHeight = pane.Height - paneBorder - paneBorder;

                    // Draw pane
                    using (var paint = new SKPaint { Color = SKColors.White })
                    {
                        canvas.DrawRect(x, y, paneWidth, paneHeight, paint);
                    }

                    // Draw glass
                    using (var paint = new SKPaint { Color = SKColors.LightGray })
                    {
                        var glassX = x + profileWidth;
                        var glassY = y + profileWidth;
                        var glassWidth = paneWidth - 2 * profileWidth;
                        var glassHeight = paneHeight - 2 * profileWidth;

                        canvas.DrawRect(glassX, glassY, glassWidth, glassHeight, paint);
                    }

                    // Draw handle if necessary
                    if (pane.Type != PaneType.Fixed)
                    {
                        var handleWidth = profileWidth / 2;
                        var handleWidthHalf = handleWidth / 2;

                        if (pane.Direction == PaneDirection.Right)
                        {
                            using (var paint = new SKPaint { Color = SKColors.White })
                            {
                                canvas.DrawRect(x + handleWidthHalf, y + paneHeight / 2, handleWidth, 20, paint); // left handle
                                canvas.DrawRect(x + handleWidthHalf, y + paneHeight / 2, handleWidth, 5, paint); // left handle
                            }
                        }
                        else if (pane.Direction == PaneDirection.Left)
                        {
                            using (var paint = new SKPaint { Color = SKColors.White })
                            {
                                canvas.DrawRect(x + paneWidth - (handleWidth + handleWidthHalf), y + paneHeight / 2, handleWidth, 20, paint); // right handle
                                canvas.DrawRect(x + paneWidth - (handleWidth + handleWidthHalf), y + paneHeight / 2, handleWidth, 5, paint); // right handle
                            }
                        }
                    }

                    // Draw opening direction lines
                    if (pane.Type == PaneType.Normal || pane.Type == PaneType.Oscilobatente)
                    {
                        using (var paint = new SKPaint { Color = SKColors.Black })
                        {
                            var glassX = x + profileWidth;
                            var glassY = y + profileWidth;
                            var glassWidth = paneWidth - 2 * profileWidth;
                            var glassHeight = paneHeight - 2 * profileWidth;

                            if (pane.Direction == PaneDirection.Left)
                            {
                                canvas.DrawLine(glassX, glassY, glassX + glassWidth, glassY + glassHeight / 2, paint);
                                canvas.DrawLine(glassX, glassY + glassHeight, glassX + glassWidth, glassY + glassHeight / 2, paint);
                            }
                            else if (pane.Direction == PaneDirection.Right)
                            {
                                canvas.DrawLine(glassX + glassWidth, glassY, glassX, glassY + glassHeight / 2, paint);
                                canvas.DrawLine(glassX + glassWidth, glassY + glassHeight, glassX, glassY + glassHeight / 2, paint);
                            }

                            if (pane.Type == PaneType.Oscilobatente)
                            {
                                canvas.DrawLine(glassX, glassY + glassHeight, glassX + glassWidth / 2, glassY, paint);
                                canvas.DrawLine(glassX + glassWidth, glassY + glassHeight, glassX + glassWidth / 2, glassY, paint);
                            }
                        }
                    }
                }
            }
        }
    }

    public enum PaneType
    {
        Fixed,
        Normal,
        Oscilobatente
    }

    public enum PaneDirection
    {
        Left,
        Right
    }

    public class Pane
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public PaneType Type { get; set; }
        public PaneDirection Direction { get; set; }
        public int ClosingSystem { get; set; }
    }
}
