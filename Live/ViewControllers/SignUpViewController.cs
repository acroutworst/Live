using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;

namespace Live
{
	partial class SignUpViewController : UIViewController
	{
		string firstName;
		string lastName;
		string username;
		string password;
		string email;
		// UIImagePickerController imagePicker;
		UIImageView imageView;
		UIActivityIndicatorView activitySpinner;

		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; OnPropertyChanged ("FirstName"); }
		}

		public string LastName
		{
			get { return lastName; }
			set { lastName = value; OnPropertyChanged ("LastName"); }
		}

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

		public string Email 
		{
			get { return email; }
			set { email = value; OnPropertyChanged ("Email"); }
		}

		public SignUpViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad() 
		{
			base.ViewDidLoad ();

			SetupUserInterface ();
			SetupEventHandlers ();
		}

		private void SetupUserInterface ()
		{
			
		}

		private void SetupEventHandlers ()
		{
			
		}

		private async Task ExecuteSignUpUser ()
		{
			if (IsBusy) {
				return;
			}

			IsBusy = true;

			var user = new User {
				Name = string.Format ("{0} {1}", FirstName, LastName),
				ProfileImage = imageView
			};

			var account = new Account {
				Username = Username,
				Password = Password,
				Email = Email,
				UserId = user.Id
			};

			try
			{
				activitySpinner.StartAnimating();
				if (await ConnectivityService.IsConnected ()) {
					await CreateAccount (account, user);

					await SignIn (account);
					// NavigateToMainUI ();

					activitySpinner.StopAnimating();
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
				Console.WriteLine ();
			}

			IsBusy = false;
		}

		private async Task CreateAccount (Account account, User user)
		{
			await AccountService.Instance.Register (account, user);
		}

		private async Task SignIn (Account account)
		{
			await AccountService.Instance.Login (account);
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
