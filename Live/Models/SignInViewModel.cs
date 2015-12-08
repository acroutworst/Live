using System;
using System.Threading.Tasks;
using UIKit;

namespace Live
{
	public class SignInViewModel : BaseViewModel
	{
		string username;
		string password;
		UIActivityIndicatorView activitySpinner;

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
				//Xamarin.Insights.Report (ex);
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
	}
}