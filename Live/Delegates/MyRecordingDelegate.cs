using System;
using AVFoundation;
using Foundation;
using UIKit;
using AssetsLibrary;
using System.IO;

namespace Live
{
	public class MyRecordingDelegate : AVCaptureFileOutputRecordingDelegate {
		public override void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			if(UIVideo.IsCompatibleWithSavedPhotosAlbum(outputFileUrl.Path))
			{
				var library = new ALAssetsLibrary();
				library.WriteVideoToSavedPhotosAlbum(outputFileUrl, (path, e2) => {
					if(e2 != null)
					{
						new UIAlertView("Error", e2.ToString(), null, "OK", null).Show();
					}
					else
					{
						new UIAlertView("Saved", "Saved to Photos", null, "OK", null).Show();
						File.Delete(outputFileUrl.Path);
					}
				});
			}
			else
			{
				new UIAlertView("Incompatible", "Incompatible", null, "OK", null).Show();
			}

		}
	}
}

