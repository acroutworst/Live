using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using AVFoundation;
using System.Drawing;

namespace Live
{
	partial class MainPageViewController : UIViewController
	{
		public MainPageViewController (IntPtr handle) : base (handle)
		{
		}

		public UITextField usernameEntry;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();

			SetupUserInterface ();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);

			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		private void SetupUserInterface()
		{
			var centerButtonX = View.Bounds.GetMidX () - 35f;
			var centerButtonY = View.Bounds.GetMidY () - 35f;
			var topLeftX = View.Bounds.X + 25;
			var topRightX = View.Bounds.Right - 65;
			var bottomButtonY = View.Bounds.Bottom - 85;
			var topButtonY = View.Bounds.Top + 15;
			var buttonWidth = 70;
			var buttonHeight = 70;

			UIView CameraStream = new UIView () {
				Frame = new CGRect (0f, 0f, 320f, View.Bounds.Height)
			};

			AVCaptureSession captureSession = new AVCaptureSession ();
			var viewLayer = CameraStream.Layer;
			var videoPreviewLayer = new AVCaptureVideoPreviewLayer (captureSession) {
				Frame = CameraStream.Bounds
			};

			CameraStream.Layer.AddSublayer (videoPreviewLayer);
			this.View.AddSubview (CameraStream);
			this.View.SendSubviewToBack (CameraStream);

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			ConfigureCameraForDevice (captureDevice);
			AVCaptureDeviceInput captureDeviceInput = AVCaptureDeviceInput.FromDevice (captureDevice);

			captureSession.AddInput (captureDeviceInput);

			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
			AVCaptureStillImageOutput stillImageOutput = new AVCaptureStillImageOutput () {
				OutputSettings = new NSDictionary ()
			};

			captureSession.AddOutput (stillImageOutput);
			captureSession.StartRunning ();

			usernameEntry = new UITextField () {
				Frame = new CGRect (centerButtonX, topButtonY + 200f, 150, 35),
				Placeholder = Strings.Username,
				BackgroundColor = UIColor.DarkTextColor
			};

			var uITextField = new UITextField ();
			uITextField.Frame = new CGRect (centerButtonX, topButtonY + 250f, 150, 35);
			uITextField.Placeholder = Strings.Password;
			uITextField.SecureTextEntry = true;
			UITextField passwordEntry = uITextField;
			passwordEntry.BackgroundColor = UIColor.LightTextColor;

			UIButton loginButton = new UIButton () {
				Frame = new CGRect (centerButtonX, bottomButtonY - 175f, 100, 35)
			};

			loginButton.SetTitle ("Log In", UIControlState.Normal);

			UIButton signupButton = new UIButton () {
				Frame = new CGRect (centerButtonX, bottomButtonY, 100, 35)	
			};

			signupButton.SetTitle ("Sign Up", UIControlState.Normal);	

			View.Add (CameraStream);
			View.Add (usernameEntry);
			View.Add (passwordEntry);
			View.Add (loginButton);
			View.Add (signupButton);
		}

		private void SetupEventHandlers()
		{
			
		}

		public void ConfigureCameraForDevice (AVCaptureDevice device)
		{
			NSError error;
			if (device.IsFocusModeSupported (AVCaptureFocusMode.ContinuousAutoFocus)) {
				device.LockForConfiguration (out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration ();
			} else if (device.IsExposureModeSupported (AVCaptureExposureMode.ContinuousAutoExposure)) {
				device.LockForConfiguration (out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration ();
			} else if (device.IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance)) {
				device.LockForConfiguration (out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration ();
			}
		}
	}
}
