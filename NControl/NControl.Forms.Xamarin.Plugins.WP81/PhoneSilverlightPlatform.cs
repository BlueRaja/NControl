﻿using NGraphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NControl.Plugins.WP81
{
    /// <summary>
    /// Windows Phone Silverlight Platform
    /// </summary>
    public class PhoneSilverlightPlatform : IPlatform
    {
        /// <summary>
        /// Returns the name of the platform
        /// </summary>
        public string Name { get { return "PhoneSilverlight"; } }

        /// <summary>
        /// Creates an object implementing the IImageCanvas interface
        /// </summary>
        /// <param name="size"></param>
        /// <param name="scale"></param>
        /// <param name="transparency"></param>
        /// <returns></returns>
        public IImageCanvas CreateImageCanvas(Size size, double scale = 1.0, bool transparency = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an object implementing the IImage interface with the given paramters
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="width"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public IImage CreateImage(NGraphics.Color[] colors, int width, double scale = 1.0)
        {
            return new PhoneSilverlightImage();
        }

        /// <summary>
        /// Loads an IImage from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public IImage LoadImage(Stream stream)
        {
            return new PhoneSilverlightImage();
        }

        /// <summary>
        /// Loads an IImage from a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IImage LoadImage(string path)
        {
            return new PhoneSilverlightImage();
        }
    }

    /// <summary>
    /// Creates a new PhoneSilverlightImageSurface
    /// </summary>
    public class CanvasCanvas : IImageCanvas
    {
        #region Private Members

        /// <summary>
        /// Wrapped Canvas
        /// </summary>
        private readonly Canvas _canvas;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        public CanvasCanvas(Canvas canvas)
        {
            _canvas = canvas;
        }

        /// <summary>
        /// Returns the canvas rendered on an IImage
        /// </summary>
        /// <returns></returns>
        public IImage GetImage()
        {
            return new PhoneSilverlightImage();
        }

        #region Properties

        /// <summary>
        /// Returns the size of the canvase
        /// </summary>
        public Size Size { get { return new Size(_canvas.Width, _canvas.Height); } }
            
        /// <summary>
        /// Returns scale, always 1.0
        /// </summary>
        public double Scale { get { return 1.0; } }

        #endregion

        #region Drawing Members

        /// <summary>
        /// Saves the state of the drawing operations
        /// </summary>
        public void SaveState()
        {
        }
            
        /// <summary>
        /// Transforms
        /// </summary>
        /// <param name="transform"></param>
        public void Transform(NGraphics.Transform transform)
        {
        }
            
        /// <summary>
        /// Restores the state
        /// </summary>
        public void RestoreState()
        {
        }
            
        /// <summary>
        /// Draws text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="frame"></param>
        /// <param name="font"></param>
        /// <param name="alignment"></param>
        /// <param name="pen"></param>
        /// <param name="brush"></param>
        public void DrawText(string text, Rect frame, Font font, TextAlignment alignment = TextAlignment.Left, 
            Pen pen = null, NGraphics.Brush brush = null)
        {
        }
            
        /// <summary>
        /// Draws a path
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="pen"></param>
        /// <param name="brush"></param>
        public void DrawPath(IEnumerable<PathOp> ops, Pen pen = null, NGraphics.Brush brush = null)
        {
            if (pen == null && brush == null)
                return;

            var pathEl = new System.Windows.Shapes.Path();

            if (brush != null)
                pathEl.Fill = GetBrush(brush);

            if (pen != null)
            {
                pathEl.Stroke = GetStroke(pen);
                pathEl.StrokeThickness = pen.Width;
            }

            var geo = new StringBuilder();
            
            foreach(var op in ops)
            {
                var mt = op as MoveTo;
                if (mt != null)
                {
                    geo.AppendFormat(" M {0},{1}", mt.Point.X, mt.Point.Y);
                    continue;
                }

                var lt = op as LineTo;
                if(lt != null)
                {
                    geo.AppendFormat(" L {0},{1}", lt.Point.X, lt.Point.Y);
                    continue;
                }

                var at = op as ArcTo;
                if (at != null)
                {
                    geo.AppendFormat(" L {0},{1}", at.Point.X, at.Point.Y);
                    continue;
                }

                var ct = op as CurveTo;
                if(ct != null)
                {
                    var p = ct.Point;
                    var c1 = ct.Control1;
                    var c2 = ct.Control2;
                    geo.AppendFormat(" C {0},{1} {2},{3} {4},{5}",
                        c1.X, c1.Y, c2.X, c2.Y, p.X, p.Y);
                    continue;
                }

                var cp = op as ClosePath;
                if(cp != null)
                {
                    geo.Append(" z");
                    continue;
                }
            }

            // Convert path string to geometry
            var b = new Binding { Source = geo.ToString() };
            BindingOperations.SetBinding(pathEl, System.Windows.Shapes.Path.DataProperty, b);

            _canvas.Children.Add(pathEl);
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="pen"></param>
        /// <param name="brush"></param>
            
        public void DrawRectangle(Rect frame, Pen pen = null, NGraphics.Brush brush = null)
        {
            var rectangleEl = new System.Windows.Shapes.Rectangle();
            rectangleEl.Width = frame.Width;
            rectangleEl.Height = frame.Height;

            if (brush != null)            
                rectangleEl.Fill = GetBrush(brush);

            if (pen != null)
            {
                rectangleEl.Stroke = GetStroke(pen);
                rectangleEl.StrokeThickness = pen.Width;
            }

            _canvas.Children.Add(rectangleEl);
            Canvas.SetLeft(rectangleEl, frame.X);
            Canvas.SetTop(rectangleEl, frame.Y);            
        }
            
        /// <summary>
        /// Draws an ellipse
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="pen"></param>
        /// <param name="brush"></param>
        public void DrawEllipse(Rect frame, Pen pen = null, NGraphics.Brush brush = null)
        {
            var ellipseEl = new System.Windows.Shapes.Ellipse();
            ellipseEl.Width = frame.Width;
            ellipseEl.Height = frame.Height;

            if (brush != null)
                ellipseEl.Fill = GetBrush(brush);

            if (pen != null)
            {
                ellipseEl.Stroke = GetStroke(pen);
                ellipseEl.StrokeThickness = pen.Width;
            }

            _canvas.Children.Add(ellipseEl);
            Canvas.SetLeft(ellipseEl, frame.X);
            Canvas.SetTop(ellipseEl, frame.Y);    
        }
            
        /// <summary>
        /// Draws an image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="frame"></param>
        /// <param name="alpha"></param>
        public void DrawImage(IImage image, Rect frame, double alpha = 1.0)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Brushes

        /// <summary>
        /// Returns a Windows brush from the NGraphics brush
        /// </summary>
        /// <param name="fromBrush"></param>
        /// <returns></returns>
        private System.Windows.Media.Brush GetBrush(NGraphics.Brush fromBrush)
        {
            var sb = fromBrush as SolidBrush;
            if(sb != null)
            {                
                return new SolidColorBrush(new System.Windows.Media.Color { 
                    A = sb.Color.A, R = sb.Color.R, G = sb.Color.G, B = sb.Color.B
                });
            }
            
            var lb = fromBrush as NGraphics.LinearGradientBrush;
            if(lb != null)
            {

            }

            var rb = fromBrush as NGraphics.RadialGradientBrush;
            if(rb != null)
            {

            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromBrush"></param>
        /// <returns></returns>
        private System.Windows.Media.Brush GetStroke(NGraphics.Pen fromPen)
        {
            return new SolidColorBrush(new System.Windows.Media.Color
                {
                    A = fromPen.Color.A,
                    R = fromPen.Color.R,
                    G = fromPen.Color.G,
                    B = fromPen.Color.B
                });            
        }

        #endregion
    }

    /// <summary>
    /// Implements the IImage interface
    /// </summary>
    public class PhoneSilverlightImage : IImage
    {
        public void SaveAsPng(string path)
        {
        }
    }    
}    

