using System;

using UIKit;
using AVFoundation;
using System.Threading.Tasks;
using CoreGraphics;
using System.ComponentModel;
using System.Collections.Generic;

namespace Live
{
	public partial class ViewController : UIViewController
	{
		Images cancelButton;
		UIView backgroundStream;
		AVCaptureSession Session;

		string username;
		string password;
		UIActivityIndicatorView activitySpinner;

		UITextField usernameEntry;
		UITextField passwordEntry;
		UIButton loginButton;
		UIButton signupButton;
		UIImageView liveImage;

		public string Username
		{
			get { return username; }
			set { username = value; OnPropertyChanged ("Username"); }
		}

		public string Password
		{
			get { return password; }
			set { password = value; OnPropertyChanged ("Password"); }
		} 

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetupUserInterface ();
			SetupEventHandlers ();
		}

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

			/*Session = new AVCaptureSession ();
			var PreviewLayer = new AVCaptureVideoPreviewLayer (Session) {
				Frame = backgroundStream.Bounds
			};
			backgroundStream.Layer.AddSublayer (PreviewLayer);
			this.View.AddSubview (backgroundStream);
			this.View.SendSubviewToBack (backgroundStream);	*/

			usernameEntry = new UITextField () {
				Frame = new CGRect (centerButtonX, centerButtonY, 100, 25),
				Placeholder = Strings.Username
			};

			passwordEntry = new UITextField () {
				Frame = new CGRect (centerButtonX, centerButtonY - 5f, 100, 25),
				Placeholder = Strings.Password,
				SecureTextEntry = true
			};

			loginButton = new UIButton () {
				Frame = new CGRect (centerButtonX, centerButtonY - 15f, 100, 25)
			};

			loginButton.SetTitle ("Log In", UIControlState.Normal);

			signupButton = new UIButton () {
				Frame = new CGRect (centerButtonX, bottomButtonY, 100, 25)	
			};

			signupButton.SetTitle ("Sign Up", UIControlState.Normal);	

			View.Add (usernameEntry);
			View.Add (passwordEntry);
			View.Add (loginButton);
			View.Add (signupButton);
		}

		private void SetupEventHandlers () 
		{
				 
		}
			
		private async Task ExecuteSignInUser ()
		{
			if (IsBusy) {
				return;
			}

			IsBusy = true;

			try
			{
				activitySpinner.StartAnimating();
				if (await ConnectivityService.IsConnected ()) {
					var result = await SignIn ();
					activitySpinner.StopAnimating();

					if (result) {
						var storyboard = this.Storyboard;
						var cameraViewController = (CameraViewController)
							storyboard.InstantiateViewController ("CameraViewController");
						this.PresentViewController (cameraViewController, true, null);
						// NavigateToMainUI ();
					} else {
						UIAlertView alert = new UIAlertView() {
							Title = "Login Error", Message = "Invalid Login"
						};
						alert.AddButton ("OK");
						alert.Show ();
					}
				} else {
					UIAlertView alert = new UIAlertView() {
						Title = "Login Error", Message = "No Internet Connection"
					};
					alert.AddButton ("OK");
					alert.Show ();
				}
			}
			catch (Exception ex) 
			{
				return;
			}

			IsBusy = false;
		}

		private async Task<bool> SignIn ()
		{
			var account = new Account {
				Username = Username,
				Password = Password
			};

			return await AccountService.Instance.Login (account);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		private string title = string.Empty;
		private string subTitle = string.Empty;
		private string icon = null;
		private bool isBusy;

		public const string TitlePropertyName = "Title";
		public const string SubtitlePropertyName = "Subtitle";
		public const string IconPropertyName = "Icon";
		public const string IsBusyPropertyName = "IsBusy";

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public string Title
		{
			get { return title; }
			set { SetProperty (ref title, value, TitlePropertyName);}
		}

		public string Subtitle
		{
			get { return subTitle; }
			set { SetProperty (ref subTitle, value, SubtitlePropertyName);}
		}

		public string Icon
		{
			get { return icon; }
			set { SetProperty (ref icon, value, IconPropertyName);}
		}

		public bool IsBusy 
		{
			get { return isBusy; }
			set { SetProperty (ref isBusy, value, IsBusyPropertyName);}
		}

		protected void SetProperty<T> (ref T backingStore, T value, string propertyName, Action onChanged = null, Action<T> onChanging = null) 
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value)) 
				return;

			if (onChanging != null) 
				onChanging(value);

			OnPropertyChanging(propertyName);

			backingStore = value;

			if (onChanged != null) 
				onChanged();

			OnPropertyChanged(propertyName);
		}

		public void OnPropertyChanging(string propertyName)
		{
			if (PropertyChanging == null)
				return;

			PropertyChanging (this, new PropertyChangingEventArgs (propertyName));
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}
	}
}

