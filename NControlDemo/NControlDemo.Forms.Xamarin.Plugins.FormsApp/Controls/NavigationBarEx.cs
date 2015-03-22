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

using System;
using NControl.Plugins.Abstractions;
using Xamarin.Forms;
using NGraphics;

namespace NControlDemo.FormsApp.Controls
{
    /// <summary>
    /// Navigation bar ex.
    /// </summary>
    public class NavigationBarEx: NControlView
    {
        /// <summary>
        /// The title label.
        /// </summary>
        private Label _titleLabel;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NControlDemo.Forms.Xamarin.Plugins.FormsApp.Controls.NavigationBarEx"/> class.
        /// </summary>
        public NavigationBarEx()
        {
            BackgroundColor = Xamarin.Forms.Color.FromHex("FFFFFF");
            _titleLabel = new Label { 
                Text = "", 
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                XAlign = Xamarin.Forms.TextAlignment.Center, 
                YAlign = Xamarin.Forms.TextAlignment.Center 
            };

            var layout = new RelativeLayout();
            layout.Children.Add(_titleLabel, () => new Xamarin.Forms.Rectangle(0, layout.Height - 44, layout.Width, 44));
            Content = layout;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title 
        {
            get { return _titleLabel.Text; }
            set { _titleLabel.Text = value; }
        }

        /// <summary>
        /// Draw the specified canvas.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="rect">Rect.</param>
        public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect)
        {
            base.Draw(canvas, rect);
            canvas.DrawLine(0, rect.Height-0.5, rect.Width, rect.Height-0.5, NGraphics.Colors.Gray, 0.5);
        }
    }
}

