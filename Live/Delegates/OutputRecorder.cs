using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;

namespace Live
{
	/// <summary>
	/// Helper class that pulls an image from the sample buffer and displays it in the <c>UIImageView</c>
	/// that it has been attached to.
	/// </summary>
	public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
	{
		public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			try {
				var image = ImageFromSampleBuffer (sampleBuffer);

				// Do something with the image, we just stuff it in our main view.
				//AppDelegate.liveCameraStream.BeginInvokeOnMainThread (()=> {
				//	TryDispose(AppDelegate.liveCameraStream.Image);
				//	AppDelegate.liveCameraStream.Image = image;
				//	AppDelegate.liveCameraStream.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2);
				//});
			} catch (Exception e){
				Console.WriteLine (e);
			} finally {
				sampleBuffer.Dispose ();
			}
		}

		UIImage ImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
		{
			// Get the CoreVideo image
			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
				// Lock the base address
				pixelBuffer.Lock (0);
				// Get the number of bytes per row for the pixel buffer
				var baseAddress = pixelBuffer.BaseAddress;
				int bytesPerRow = (int) pixelBuffer.BytesPerRow;
				int width = (int) pixelBuffer.Width;
				int height = (int) pixelBuffer.Height;
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
				// Create a CGImage on the RGB colorspace from the configured parameter above
				using (var cs = CGColorSpace.CreateDeviceRGB ()) {
					using (var context = new CGBitmapContext (baseAddress, width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo)flags)) {
						using (CGImage cgImage = context.ToImage ()) {
							pixelBuffer.Unlock (0);
							return UIImage.FromImage (cgImage);
						}
					}
				}
			}
		}

		void TryDispose (IDisposable obj)
		{
			if (obj != null)
				obj.Dispose ();
		}
	}

}