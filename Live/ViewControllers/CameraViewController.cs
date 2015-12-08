using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using CoreFoundation;
using CoreImage;
using CoreAnimation;

namespace Live
{
	partial class CameraViewController : UIViewController
	{
		public CameraViewController (IntPtr handle) : base (handle)
		{
		}

		AVCaptureSession captureSession;
		AVCaptureDeviceInput captureDeviceInput;
		AVCaptureDeviceInput captureDeviceInputAudio;
		Boolean weAreRecording;
		UIButton toggleCameraButton;
		UIButton toggleFlashButton;
		UIView liveCameraStream;
		AVCaptureStillImageOutput stillImageOutput;
		UIButton takePhotoButton;
		AVCaptureMovieFileOutput output;
		UIButton btnStartRecording;
		UISwipeGestureRecognizer swipeGestureRecognizer;
		UISwipeGestureRecognizer swipeRightGestureRecognizer;
		//UISwipeGestureRecognizer swipeLeftGestureRecognizer;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PrefersStatusBarHidden ();
			SetupUserInterface ();
			SetupEventHandlers ();
			AuthorizeCameraUse ();
			SetupLiveCameraStream ();
			//SwipeToProfile ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		/// Stop recording, remove all views, and move to next page
		public override void ViewWillDisappear (bool animated)
		{
			captureSession.StopRunning ();
			this.btnStartRecording.TouchUpInside -= startStopPushed;

			foreach (var view in this.View.Subviews) {

				view.RemoveFromSuperview ();
			}
				
			base.ViewWillDisappear (animated);
		}

		/// Place color filters on live streams
		public void SwitchFilters ()
		{
			swipeGestureRecognizer = new UISwipeGestureRecognizer ();
			this.View.AddGestureRecognizer (swipeGestureRecognizer);

			if (weAreRecording)
			{
				if(swipeGestureRecognizer.Direction == UISwipeGestureRecognizerDirection.Left)
				{
					var ciimage = new CIImage (UIImage.FromFile("Default-568h.png"));

					var sepia = new CISepiaTone();
					sepia.Image = ciimage;
					sepia.Intensity = 0.5f;

					//var cifilter = new CIFilter (sepia);

				}
			}
		}

		/// Swipe right to move to profile
		public void SwipeToProfile()
		{
			/*swipeGestureRecognizer = new UISwipeGestureRecognizer();
			this.View.AddGestureRecognizer (swipeGestureRecognizer);

			//if (swipeGestureRecognizer.Direction == UISwipeGestureRecognizerDirection.Right) {
				
			//}*/

			swipeRightGestureRecognizer = new UISwipeGestureRecognizer ( () => UpdateRight()){Direction = UISwipeGestureRecognizerDirection.Right};
			this.View.AddGestureRecognizer (swipeRightGestureRecognizer);

		}

		/// Helper method for swipping to profile
		public void UpdateRight()
		{
			var storyboard = this.Storyboard;
			var profileViewController = (ProfileViewController)
				storyboard.InstantiateViewController ("ProfileViewController");
			this.PresentViewController (profileViewController, true, null);
		}

		/// Hide status bar
		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		public async void AuthorizeCameraUse ()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized) {
				await AVCaptureDevice.RequestAccessForMediaTypeAsync (AVMediaType.Video);
			}
		}

		/// Setup capture session and live stream
		public void SetupLiveCameraStream ()
		{
			weAreRecording = false;

			captureSession = new AVCaptureSession ();
			var viewLayer = liveCameraStream.Layer;
			var videoPreviewLayer = new AVCaptureVideoPreviewLayer (captureSession) {
				Frame = liveCameraStream.Bounds
			};
			liveCameraStream.Layer.AddSublayer (videoPreviewLayer);
			this.View.AddSubview (liveCameraStream);
			this.View.SendSubviewToBack (liveCameraStream);

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			ConfigureCameraForDevice (captureDevice);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice (captureDevice);

			var captureDeviceAudio = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Audio);
			captureDeviceInputAudio = AVCaptureDeviceInput.FromDevice (captureDeviceAudio);

			captureSession.AddInput (captureDeviceInput);
			captureSession.AddInput (captureDeviceInputAudio);

			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
			stillImageOutput = new AVCaptureStillImageOutput () {
				OutputSettings = new NSDictionary ()
			};

			output = new AVCaptureMovieFileOutput ();

			long totalSeconds = 10000;
			Int32 preferredTimeScale = 30;
			CMTime maxDuration = new CMTime (totalSeconds, preferredTimeScale);
			output.MinFreeDiskSpaceLimit = 1024 * 1024;
			output.MaxRecordedDuration = maxDuration;

			captureSession.AddOutput (output);

			captureSession.AddOutput (stillImageOutput);

			captureSession.StartRunning ();
		}
			
		public void ToggleFrontBackCamera ()
		{ 
			var devicePosition = captureDeviceInput.Device.Position;
			if (devicePosition == AVCaptureDevicePosition.Front) {
				devicePosition = AVCaptureDevicePosition.Back;
			} else {
				devicePosition = AVCaptureDevicePosition.Front;
			}

			var device = GetCameraForOrientation (devicePosition);
			ConfigureCameraForDevice (device);

			captureSession.BeginConfiguration ();
			captureSession.RemoveInput (captureDeviceInput);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice (device);
			captureSession.AddInput (captureDeviceInput);
			captureSession.CommitConfiguration ();
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

		public void ToggleFlash ()
		{
			var device = captureDeviceInput.Device;

			NSError error;
			if (device.HasFlash) {
				if (device.FlashMode == AVCaptureFlashMode.On) {
					device.LockForConfiguration (out error);
					device.FlashMode = AVCaptureFlashMode.Off;
					device.UnlockForConfiguration ();

					toggleFlashButton.SetBackgroundImage (UIImage.FromFile ("NoFlashButton.png"), UIControlState.Normal);
				} else {
					device.LockForConfiguration (out error);
					device.FlashMode = AVCaptureFlashMode.On;
					device.UnlockForConfiguration ();

					toggleFlashButton.SetBackgroundImage (UIImage.FromFile ("FlashButton.png"), UIControlState.Normal);
				}
			}
		}

		public AVCaptureDevice GetCameraForOrientation (AVCaptureDevicePosition orientation)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);

			foreach (var device in devices) {
				if (device.Position == orientation) {
					return device;
				}
			}

			return null;
		}

		/// <summary>
		/// Setup UIView (liveCameraStream), camera buttons, add UI elements to View
		/// </summary>
		private void SetupUserInterface ()
		{
			var centerButtonX = View.Bounds.GetMidX () - 35f;
			var centerButtonY = View.Bounds.GetMidY () - 35f;
			var topLeftX = View.Bounds.X + 25;
			var topRightX = View.Bounds.Right - 65;
			var bottomButtonY = View.Bounds.Bottom - 85;
			var topButtonY = View.Bounds.Top + 15;
			var buttonWidth = 70;
			var buttonHeight = 70;

			liveCameraStream = new UIView () {
				Frame = new CGRect (0f, 0f, 320f, View.Bounds.Height)
			};

			takePhotoButton = new UIButton () {
				Frame = new CGRect (centerButtonX, bottomButtonY, buttonWidth, buttonHeight)
			};
			takePhotoButton.SetBackgroundImage (UIImage.FromFile ("TakePhotoButton.png"), UIControlState.Normal);

			toggleCameraButton = new UIButton () {
				Frame = new CGRect (topRightX, topButtonY + 5, 35, 26)
			};
			toggleCameraButton.SetBackgroundImage (UIImage.FromFile ("ToggleCameraButton.png"), UIControlState.Normal);

			toggleFlashButton = new UIButton () {
				Frame = new CGRect (topLeftX, topButtonY, 37, 37)
			};
			toggleFlashButton.SetBackgroundImage (UIImage.FromFile ("NoFlashButton.png"), UIControlState.Normal);

			btnStartRecording = new UIButton () {
				Frame = new CGRect (centerButtonX, centerButtonY, buttonWidth, buttonHeight)
			};
			btnStartRecording.SetBackgroundImage (UIImage.FromFile ("SendMomentButton.png"), UIControlState.Normal);

			View.Add (liveCameraStream);
			View.Add (takePhotoButton);
			View.Add (toggleCameraButton);
			View.Add (toggleFlashButton);
		}

		/// <summary>
		/// Touch Event Handlers
		/// </summary>
		private void SetupEventHandlers ()
		{
			takePhotoButton.TouchUpInside += startStopPushed;

			toggleCameraButton.TouchUpInside += (object sender, EventArgs e) => ToggleFrontBackCamera ();

			toggleFlashButton.TouchUpInside += (object sender, EventArgs e) => ToggleFlash ();
		}
			
		// Start Recording 
		public void startStopPushed(object sender, EventArgs ea)
		{

			if (!weAreRecording) {

				var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
				var library = System.IO.Path.Combine (documents, "..", "Library");
				var urlpath = System.IO.Path.Combine (library, "sweetMovieFilm.mov");

				NSUrl url = new NSUrl (urlpath, false);

				NSFileManager manager = new NSFileManager ();
				NSError error = new NSError ();

				if (manager.FileExists (urlpath)) {
					Console.WriteLine ("Deleting File");
					manager.Remove (urlpath, out error);
					Console.WriteLine ("Deleted File");
				}

				MyRecordingDelegate avDel= new MyRecordingDelegate ();

				output.StartRecordingToOutputFile(url, avDel);
				Console.WriteLine (urlpath);
				weAreRecording = true;
				takePhotoButton.SetBackgroundImage (UIImage.FromFile ("RecordPhotoButton.png"), UIControlState.Normal);
			}

			// we were already recording.  Stop recording
			else {

				output.StopRecording ();
		
				weAreRecording = false;

				takePhotoButton.SetBackgroundImage (UIImage.FromFile ("TakePhotoButton.png"), UIControlState.Normal);
			}

			//var storyboard = this.Storyboard;
			//var liveStreamViewController = (LiveStreamViewController)
			//	storyboard.InstantiateViewController ("LiveStreamViewController");
			//this.PresentViewController (liveStreamViewController, true, null);
		}

		/// Unimplemented Features (Taking Photos/Still Images (Screenshots))
		public async void CapturePhoto ()
		{
			var videoConnection = stillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
			var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync (videoConnection);

			var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer);

			await SendPhoto (jpegImageAsNsData.ToArray ());
		}

		public Task SendPhoto (byte[] image)
		{
			/*var navigationPage = new NavigationPage (new DrawMomentPage (image)) {
				BarBackgroundColor = Colors.NavigationBarColor,
				BarTextColor = Colors.NavigationBarTextColor
			};

			await App.Current.MainPage.Navigation.PushModalAsync (navigationPage, false);*/

			throw new NotImplementedException ();
		}

		protected override void Dispose(bool disposing)
		{
			Console.WriteLine (String.Format ("{0} controller disposed - {1}", this.GetType (), this.GetHashCode ()));

			base.Dispose (disposing);
		}
	}

}
