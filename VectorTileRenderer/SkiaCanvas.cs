using ClipperLib;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VectorTileRenderer
{
    public class SkiaCanvas : ICanvas
    {
        int width;
        int height;

        WriteableBitmap bitmap;
        SKSurface surface;
        SKCanvas canvas;

        public bool ClipOverflow { get; set; } = false;
        private Rect clipRectangle;
        List<IntPoint> clipRectanglePath;

        ConcurrentDictionary<string, SKTypeface> fontPairs = new ConcurrentDictionary<string, SKTypeface>();
        private static readonly Object fontLock = new Object();

        List<Rect> textRectangles = new List<Rect>();

        public void StartDrawing(double width, double height)
        {
            this.width = (int)width;
            this.height = (int)height;

            bitmap = new WriteableBitmap(this.width, this.height, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.Lock();
            var info = new SKImageInfo(this.width, this.height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            surface = SKSurface.Create(info, bitmap.BackBuffer, bitmap.BackBufferStride);
            canvas = surface.Canvas;

            double padding = -5;
            clipRectangle = new Rect(padding, padding, this.width - padding * 2, this.height - padding * 2);

            clipRectanglePath = new List<IntPoint>();
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Top, (int)clipRectangle.Left));
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Top, (int)clipRectangle.Right));
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Bottom, (int)clipRectangle.Right));
            clipRectanglePath.Add(new IntPoint((int)clipRectangle.Bottom, (int)clipRectangle.Left));
        }

        public void DrawBackground(Brush style)
        {
            canvas.Clear(new SKColor(style.Paint.BackgroundColor.R, style.Paint.BackgroundColor.G, style.Paint.BackgroundColor.B, style.Paint.BackgroundColor.A));
        }

        SKStrokeCap convertCap(PenLineCap cap)
        {
            if (cap == PenLineCap.Flat)
            {
                return SKStrokeCap.Butt;
            }
            else if (cap == PenLineCap.Round)
            {
                return SKStrokeCap.Round;
            }

            return SKStrokeCap.Square;
        }

        double clamp(double number, double min = 0, double max = 1)
        {
            return Math.Max(min, Math.Min(max, number));
        }

        List<List<Point>> clipPolygon(List<Point> geometry) // may break polygons into multiple ones
        {
            Clipper c = new Clipper();

            var polygon = new List<IntPoint>();

            foreach (var point in geometry)
            {
                polygon.Add(new IntPoint((int)point.X, (int)point.Y));
            }

            c.AddPolygon(polygon, PolyType.ptSubject);

            c.AddPolygon(clipRectanglePath, PolyType.ptClip);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            bool success = c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftNonZero, PolyFillType.pftEvenOdd);

            if (success && solution.Count > 0)
            {
                var result = solution.Select(s => s.Select(item => new Point(item.X, item.Y)).ToList()).ToList();
                return result;
            }

            return null;
        }

        List<Point> clipLine(List<Point> geometry)
        {
            return LineClipper.ClipPolyline(geometry, clipRectangle);
        }

        SKPath getPathFromGeometry(List<Point> geometry)
        {

            SKPath path = new SKPath
            {
                FillType = SKPathFillType.EvenOdd,
            };

            var firstPoint = geometry[0];

            path.MoveTo((float)firstPoint.X, (float)firstPoint.Y);
            foreach (var point in geometry.Skip(1))
            {
                var lastPoint = path.LastPoint;
                path.LineTo((float)point.X, (float)point.Y);
            }

            return path;
        }

        public void DrawLineString(List<Point> geometry, Brush style)
        {
            if (ClipOverflow)
            {
                geometry = clipLine(geometry);
                if (geometry == null)
                {
                    return;
                }
            }

            var path = getPathFromGeometry(geometry);
            if (path == null)
            {
                return;
            }

            SKPaint fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeCap = convertCap(style.Paint.LineCap),
                StrokeWidth = (float)style.Paint.LineWidth,
                Color = new SKColor(style.Paint.LineColor.R, style.Paint.LineColor.G, style.Paint.LineColor.B, (byte)clamp(style.Paint.LineColor.A * style.Paint.LineOpacity, 0, 255)),
                IsAntialias = true,
            };

            if (style.Paint.LineDashArray.Count() > 0)
            {
                var effect = SKPathEffect.CreateDash(style.Paint.LineDashArray.Select(n => (float)n).ToArray(), 0);
                fillPaint.PathEffect = effect;
            }

            canvas.DrawPath(path, fillPaint);
        }

        SKTextAlign convertAlignment(TextAlignment alignment)
        {
            if (alignment == TextAlignment.Center)
            {
                return SKTextAlign.Center;
            }
            else if (alignment == TextAlignment.Left)
            {
                return SKTextAlign.Left;
            }
            else if (alignment == TextAlignment.Right)
            {
                return SKTextAlign.Right;
            }

            return SKTextAlign.Center;
        }

        SKPaint getTextStrokePaint(Brush style)
        {
            var paint = new SKPaint()
            {
                IsStroke = true,
                StrokeWidth = (float)style.Paint.TextStrokeWidth,
                Color = new SKColor(style.Paint.TextStrokeColor.R, style.Paint.TextStrokeColor.G, style.Paint.TextStrokeColor.B, (byte)clamp(style.Paint.TextStrokeColor.A * style.Paint.TextOpacity, 0, 255)),
                TextSize = (float)style.Paint.TextSize,
                IsAntialias = true,
                TextEncoding = SKTextEncoding.Utf32,
                TextAlign = convertAlignment(style.Paint.TextJustify),
                Typeface = getFont(style.Paint.TextFont, style),
            };

            return paint;
        }

        SKPaint getTextPaint(Brush style)
        {
            var paint = new SKPaint()
            {
                Color = new SKColor(style.Paint.TextColor.R, style.Paint.TextColor.G, style.Paint.TextColor.B, (byte)clamp(style.Paint.TextColor.A * style.Paint.TextOpacity, 0, 255)),
                TextSize = (float)style.Paint.TextSize,
                IsAntialias = true,
                TextEncoding = SKTextEncoding.Utf32,
                TextAlign = convertAlignment(style.Paint.TextJustify),
                Typeface = getFont(style.Paint.TextFont, style),
                HintingLevel = SKPaintHinting.Normal,
            };

            return paint;
        }

        string transformText(string text, Brush style)
        {
            if (text.Length == 0)
            {
                return "";
            }

            if (style.Paint.TextTransform == TextTransform.Uppercase)
            {
                text = text.ToUpper();
            }
            else if (style.Paint.TextTransform == TextTransform.Lowercase)
            {
                text = text.ToLower();
            }

            var paint = getTextPaint(style);
            text = breakText(text, paint, style);

            return text;
        }

        string breakText(string input, SKPaint paint, Brush style)
        {
            var restOfText = input;
            var brokenText = "";
            do
            {
                var lineLength = paint.BreakText(restOfText, (float)(style.Paint.TextMaxWidth * style.Paint.TextSize));

                if (lineLength == restOfText.Length)
                {
                    brokenText += restOfText.Trim();
                    break;
                }

                var lastSpaceIndex = restOfText.LastIndexOf(' ', (int)(lineLength - 1));
                if (lastSpaceIndex == -1 || lastSpaceIndex == 0)
                {
                    brokenText += restOfText.Trim();
                    break;
                }

                brokenText += restOfText.Substring(0, (int)lastSpaceIndex).Trim() + "\n";

                restOfText = restOfText.Substring((int)lastSpaceIndex, restOfText.Length - (int)lastSpaceIndex);

            } while (restOfText.Length > 0);

            return brokenText.Trim();
        }

        bool textCollides(Rect rectangle)
        {
            foreach (var rect in textRectangles)
            {
                if (rect.IntersectsWith(rectangle))
                {
                    return true;
                }
            }
            return false;
        }

        SKTypeface getFont(string[] familyNames, Brush style)
        {
            lock (fontLock)
            {
                foreach (var name in familyNames)
                {
                    if (fontPairs.ContainsKey(name))
                    {
                        return fontPairs[name];
                    }

                    if (style.GlyphsDirectory != null)
                    {
                        var newType = SKTypeface.FromFile(System.IO.Path.Combine(style.GlyphsDirectory, name + ".ttf"));
                        if (newType != null)
                        {
                            fontPairs[name] = newType;
                            return newType;
                        }

                        newType = SKTypeface.FromFile(System.IO.Path.Combine(style.GlyphsDirectory, name + ".otf"));
                        if (newType != null)
                        {
                            fontPairs[name] = newType;
                            return newType;
                        }
                    }

                    var typeface = SKTypeface.FromFamilyName(name);
                    if (typeface.FamilyName == name)
                    {
                        fontPairs[name] = typeface;
                        return typeface;
                    }
                }

                var fallback = SKTypeface.FromFamilyName(familyNames.First());
                fontPairs[familyNames.First()] = fallback;
                return fallback;
            }
        }

        SKTypeface qualifyTypeface(string text, SKTypeface typeface)
        {
            var glyphs = new ushort[typeface.CountGlyphs(text)];
            if (glyphs.Length < text.Length)
            {
                var fm = SKFontManager.Default;
                var charIdx = (glyphs.Length > 0) ? glyphs.Length : 0;
                return fm.MatchCharacter(text[glyphs.Length]);
            }

            return typeface;
        }

        void qualifyTypeface(Brush style, SKPaint paint)
        {
            var glyphs = new ushort[paint.Typeface.CountGlyphs(style.Text)];
            if (glyphs.Length < style.Text.Length)
            {
                var fm = SKFontManager.Default;
                var charIdx = (glyphs.Length > 0) ? glyphs.Length : 0;
                var newTypeface = fm.MatchCharacter(style.Text[glyphs.Length]);

                if (newTypeface == null)
                {
                    return;
                }

                paint.Typeface = newTypeface;

                glyphs = new ushort[newTypeface.CountGlyphs(style.Text)];
                if (glyphs.Length < style.Text.Length)
                {
                    charIdx = (glyphs.Length > 0) ? glyphs.Length : 0;

                    style.Text = style.Text.Substring(0, charIdx);
                }
            }
        }

        public void DrawText(Point geometry, Brush style)
        {
            if (style.Paint.TextOptional)
            {
                return;
            }

            var paint = getTextPaint(style);
            qualifyTypeface(style, paint);

            var strokePaint = getTextStrokePaint(style);
            var text = transformText(style.Text, style);
            var allLines = text.Split('\n');

            if (allLines.Length > 0)
            {
                var biggestLine = allLines.OrderBy(line => line.Length).Last();
                var bytes = Encoding.UTF32.GetBytes(biggestLine);

                var width = (int)(paint.MeasureText(bytes));
                int left = (int)(geometry.X - width / 2);
                int top = (int)(geometry.Y - style.Paint.TextSize / 2 * allLines.Length);
                int height = (int)(style.Paint.TextSize * allLines.Length);

                var rectangle = new Rect(left, top, width, height);
                rectangle.Inflate(5, 5);

                if (ClipOverflow)
                {
                    if (!clipRectangle.Contains(rectangle))
                    {
                        return;
                    }
                }

                if (textCollides(rectangle))
                {
                    return;
                }
                textRectangles.Add(rectangle);
            }

            int i = 0;
            foreach (var line in allLines)
            {
                var bytes = Encoding.UTF32.GetBytes(line);
                float lineOffset = (float)(i * style.Paint.TextSize) - ((float)(allLines.Length) * (float)style.Paint.TextSize) / 2 + (float)style.Paint.TextSize;
                var position = new SKPoint((float)geometry.X + (float)(style.Paint.TextOffset.X * style.Paint.TextSize), (float)geometry.Y + (float)(style.Paint.TextOffset.Y * style.Paint.TextSize) + lineOffset);

                if (style.Paint.TextStrokeWidth != 0)
                {
                    canvas.DrawText(bytes, position, strokePaint);
                }

                canvas.DrawText(bytes, position, paint);
                i++;
            }
        }

        double getPathLength(List<Point> path)
        {
            double distance = 0;
            for (var i = 0; i < path.Count - 2; i++)
            {
                distance += (path[i] - path[i + 1]).Length;
            }

            return distance;
        }

        double getAbsoluteDiff2Angles(double x, double y, double c = Math.PI)
        {
            return c - Math.Abs((Math.Abs(x - y) % 2 * c) - c);
        }

        bool checkPathSqueezing(List<Point> path, double textHeight)
        {
            double previousAngle = 0;
            for (var i = 0; i < path.Count - 2; i++)
            {
                var vector = (path[i] - path[i + 1]);

                var angle = Math.Atan2(vector.Y, vector.X);
                var angleDiff = Math.Abs(getAbsoluteDiff2Angles(angle, previousAngle));

                if (angleDiff > Math.PI / 3)
                {
                    return true;
                }

                previousAngle = angle;
            }

            return false;
        }

        void debugRectangle(Rect rectangle, Color color)
        {
            var list = new List<Point>()
            {
                rectangle.TopLeft,
                rectangle.TopRight,
                rectangle.BottomRight,
                rectangle.BottomLeft,
            };

            var brush = new Brush();
            brush.Paint = new Paint();
            brush.Paint.FillColor = color;

            this.DrawPolygon(list, brush);
        }

        public void DrawTextOnPath(List<Point> geometry, Brush style)
        {
            geometry = clipLine(geometry);
            if (geometry == null)
            {
                return;
            }

            var path = getPathFromGeometry(geometry);
            var text = transformText(style.Text, style);

            var pathSqueezed = checkPathSqueezing(geometry, style.Paint.TextSize);

            if (pathSqueezed)
            {
                return;
            }

            var bounds = path.Bounds;

            var left = bounds.Left - style.Paint.TextSize;
            var top = bounds.Top - style.Paint.TextSize;
            var right = bounds.Right + style.Paint.TextSize;
            var bottom = bounds.Bottom + style.Paint.TextSize;

            var rectangle = new Rect(left, top, right - left, bottom - top);

            if (textCollides(rectangle))
            {
                return;
            }
            textRectangles.Add(rectangle);

            if (style.Text.Length * style.Paint.TextSize * 0.2 >= getPathLength(geometry))
            {
                return;
            }

            var offset = new SKPoint((float)style.Paint.TextOffset.X, (float)style.Paint.TextOffset.Y);
            var bytes = Encoding.UTF32.GetBytes(text);
            if (style.Paint.TextStrokeWidth != 0)
            {
                canvas.DrawTextOnPath(bytes, path, offset, getTextStrokePaint(style));
            }

            canvas.DrawTextOnPath(bytes, path, offset, getTextPaint(style));
        }

        public void DrawPoint(Point geometry, Brush style)
        {
            if (style.Paint.IconImage != null)
            {
                // draw icon here
            }
        }

        public void DrawPolygon(List<Point> geometry, Brush style)
        {
            List<List<Point>> allGeometries = null;
            if (ClipOverflow)
            {
                allGeometries = clipPolygon(geometry);
            }
            else
            {
                allGeometries = new List<List<Point>>() { geometry };
            }

            if (allGeometries == null)
            {
                return;
            }

            foreach (var geometryPart in allGeometries)
            {
                var path = getPathFromGeometry(geometryPart);
                if (path == null)
                {
                    return;
                }

                SKPaint fillPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    StrokeCap = convertCap(style.Paint.LineCap),
                    Color = new SKColor(style.Paint.FillColor.R, style.Paint.FillColor.G, style.Paint.FillColor.B, (byte)clamp(style.Paint.FillColor.A * style.Paint.FillOpacity, 0, 255)),
                    IsAntialias = true,
                };

                canvas.DrawPath(path, fillPaint);
            }
        }

        static SKImage toSKImage(BitmapSource bitmap)
        {
            var info = new SKImageInfo(bitmap.PixelWidth, bitmap.PixelHeight);
            var image = SKImage.Create(info);
            using (var pixmap = image.PeekPixels())
            {
                toSKPixmap(bitmap, pixmap);
            }
            return image;
        }

        static void toSKPixmap(BitmapSource bitmap, SKPixmap pixmap)
        {
            if (pixmap.ColorType == SKImageInfo.PlatformColorType)
            {
                var info = pixmap.Info;
                var converted = new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, null, 0);
                converted.CopyPixels(new Int32Rect(0, 0, info.Width, info.Height), pixmap.GetPixels(), info.BytesSize, info.RowBytes);
            }
            else
            {
                using (var tempImage = toSKImage(bitmap))
                {
                    tempImage.ReadPixels(pixmap, 0, 0);
                }
            }
        }

        public void DrawImage(Stream imageStream, Brush style)
        {
            string tempFilePath = System.IO.Path.GetTempFileName();
            try
            {
                using (var fileStream = File.Create(tempFilePath))
                {
                    imageStream.Position = 0;
                    imageStream.CopyTo(fileStream);
                }

                using (var codec = SKCodec.Create(tempFilePath))
                {
                    if (codec != null)
                    {
                        var info = new SKImageInfo(this.width, this.height);
                        using (var image = SKImage.FromEncodedData(tempFilePath))
                        {
                            if (image != null)
                            {
                                canvas.DrawImage(image, new SKPoint(0, 0));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        public void DrawUnknown(List<List<Point>> geometry, Brush style)
        {

        }

        public BitmapSource FinishDrawing()
        {
            bitmap.AddDirtyRect(new Int32Rect(0, 0, this.width, this.height));
            bitmap.Unlock();
            bitmap.Freeze();

            return bitmap;
        }
    }
}


