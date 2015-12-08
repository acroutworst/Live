using System;
using System.Net;
using Plugin.Settings;
using UIKit;


namespace Live
{
	public class ProfileViewModel : BaseViewModel
	{
		public string ProfileName
		{
			get { return CrossSettings.Current.GetValueOrDefault<string> ("profileName"); }
		}

		public UIImageView ProfileImageUrl
		{
			get { return CrossSettings.Current.GetValueOrDefault<UIImageView> ("profileImage"); }
		}
	}
}