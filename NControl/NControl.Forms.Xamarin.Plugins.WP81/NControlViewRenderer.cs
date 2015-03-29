﻿/************************************************************************
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2025 - Christian Falch
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the 
 * "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to 
 * permit persons to whom the Software is furnished to do so, subject 
 * to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 ************************************************************************/

using Microsoft.Phone.Controls;
using NControl.Plugins.Abstractions;
using NControl.Plugins.WP81;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(NControlView), typeof(NControlViewRenderer))]
namespace NControl.Plugins.WP81
{    
	/// <summary>
	/// NControlView renderer.
	/// </summary>
    public class NControlViewRenderer : ViewRenderer<NControlView, NControlNativeView>
	{
        /// <summary>
        /// Canvas element
        /// </summary>
        private Canvas _canvas;

		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init() { }

        /// <summary>
        /// Raises the element changed event.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<NControlView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                e.OldElement.OnInvalidate -= HandleInvalidate;

            if (e.NewElement != null)
            {
                e.NewElement.OnInvalidate += HandleInvalidate;                              
            }

            if (Control == null)
            {
                var b = new NControlNativeView();
                _canvas = new Canvas();                                
                b.Children.Add(_canvas);

                SetNativeControl(b);

                UpdateClip();
                
                Touch.FrameReported += Touch_FrameReported;

                RedrawControl();
            }
        }

        /// <summary>
        /// Redraw when background color changes
        /// </summary>
        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            RedrawControl();
        }

        /// <summary>
        /// Raises the element property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            if (Control != null && Control.Clip == null)
            {
                if (e.PropertyName == NControlView.ClipProperty.PropertyName)
                    UpdateClip();
            }

            // Redraw when height/width changes
            if (e.PropertyName == NControlView.HeightProperty.PropertyName ||
                e.PropertyName == NControlView.WidthProperty.PropertyName)
            {
                UpdateClip();
                RedrawControl();                
            }
        }

        #region Drawing

        /// <summary>
        /// Redraws the control by clearing the canvas element and adding new elements
        /// </summary>
        private void RedrawControl()
        {
            if (Element.Width == -1 || Element.Height == -1)
                return;

            _canvas.Children.Clear();
            var canvas = new CanvasCanvas(_canvas);

            Element.Draw(canvas, new NGraphics.Rect(0, 0, Element.Width, Element.Height));
        }

        #endregion

        #region Touch Handlers

        /// <summary>
        /// Touch handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (parent != null)
            {
                if (parent is PhoneApplicationPage)
                {
                    var page = parent as PhoneApplicationPage;

                    // Get this' position on screen
                    var transform = this.TransformToVisual(page);
                    var absolutePosition = transform.Transform(new System.Windows.Point(0, 0));
                    var ourRect = new Xamarin.Forms.Rectangle(absolutePosition.X, absolutePosition.Y, this.Width, this.Height);

                    // Get main touch point
                    var mainTouchPoint = e.GetPrimaryTouchPoint(page);

                    // Make sure our control is actually in the touch zone                    
                    if (!ourRect.Contains(mainTouchPoint.Position.X, mainTouchPoint.Position.Y))
                        return;

                    var touches = e.GetTouchPoints(page).Select(t => new NGraphics.Point(t.Position.X, t.Position.Y));

                    if (mainTouchPoint.Action == TouchAction.Move)
                    {
                        Element.TouchesMoved(touches);
                    }
                    else if (mainTouchPoint.Action == TouchAction.Down)
                    {
                        Element.TouchesBegan(touches);
                    }
                    else if (mainTouchPoint.Action == TouchAction.Up)
                    {
                        Element.TouchesEnded(touches);
                    }
                    else if (mainTouchPoint.Action == TouchAction.Leave)
                    {
                        Element.TouchesCancelled(touches);
                    }

                    break;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Updates clic on the element
        /// </summary>
        private void UpdateClip()
        {
            if (Element.Width == -1 || Element.Height == -1)
                return;

            Control.Clip = Element.Clip ?
                new RectangleGeometry { Rect = new Rect(0, 0, Element.Width, Element.Height) } : 
                null;
        }

        /// <summary>
        /// Handles the invalidate.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void HandleInvalidate(object sender, System.EventArgs args)
        {
            // Invalidate control
            RedrawControl();
        }

        #endregion
	}
}



