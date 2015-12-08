using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace Live
{
	public class FriendsViewModel : BaseViewModel
	{
		ObservableCollection<User> friends;

		public FriendsViewModel ()
		{
			friends = FriendService.Instance.Friends;
		}

		public ObservableCollection<User> Friends
		{
			get { return friends; }
			set { friends = value; } 
		}

		public async Task ExecuteFetchFriends ()
		{
			if (IsBusy) {
				return;
			}

			IsBusy = true;

			try
			{
				if (await ConnectivityService.IsConnected ()) {
					await FriendService.Instance.RefreshFriendsList ();
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
	}
}