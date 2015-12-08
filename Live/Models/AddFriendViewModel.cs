using System;
using System.Threading.Tasks;
using UIKit;

namespace Live
{
	public class AddFriendViewModel : BaseViewModel
	{
		string username;
		UIActivityIndicatorView activitySpinner;

		public string Username
		{
			get { return username; }
			set { username = value; OnPropertyChanged ("Username"); }
		}

		private async Task ExecuteAddFriend ()
		{
			if (IsBusy) {
				return;
			}

			IsBusy = true;

			try
			{
				activitySpinner.StartAnimating();
				if (await ConnectivityService.IsConnected ()) {
					var success = await CreateFriendship ();
					activitySpinner.StopAnimating();
					if (success) {
						UIAlertView alert = new UIAlertView() {
							Title = "Favorite Notification", Message = "Favorite Accepted"
						};
						alert.AddButton ("OK");
						alert.Show ();
					} else {
						UIAlertView alert = new UIAlertView() {
							Title = "Favorite Notification", Message = "Favorite Rejected"
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

		private async Task<bool> CreateFriendship ()
		{
			return await FriendService.Instance.CreateFriendship (Username);
		}
	}
}