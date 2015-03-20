﻿using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using NControl.Plugins.Abstractions;
using NControl.Plugins.Droid;

[assembly: ExportRenderer(typeof(NControlView), typeof(NControlViewRenderer))]
namespace NControl.Plugins.Droid
{
	/// <summary>
	/// NControlView renderer.
	/// </summary>
	public class NControlViewRenderer: ViewRenderer<NControlView, NControlNativeView>
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged (ElementChangedEventArgs<NControlView> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {

                if (Control == null)
                {
                    if(e.NewElement.DrawingFunction == null)
                        SetNativeControl(new NControlNativeView((NGraphics.ICanvas canvas, NGraphics.Rect rect) => e.NewElement.Draw(canvas, rect), Context));
                    else
                        SetNativeControl(new NControlNativeView(e.NewElement.DrawingFunction, Context));
                }
			}
		}

		/// <summary>
		/// Raises the element property changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName == NControlView.CancelDefaultDrawingProperty.PropertyName)
				(Control as NControlNativeView).CancelDefaultDrawing = Element.CancelDefaultDrawing;
			else if(e.PropertyName == NControlView.TransparentProperty.PropertyName)
				(Control as NControlNativeView).Transparent = Element.CancelDefaultDrawing;
		}
	}
}

