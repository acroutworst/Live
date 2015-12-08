using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin;
using UIKit;
using CoreGraphics;

namespace Live
{
	public class SignUpViewModel : BaseViewModel
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
	}
}