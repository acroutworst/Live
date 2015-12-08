using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Live
{
	partial class ProfileViewController : UIViewController
	{
		UISwipeGestureRecognizer swipeLeftGestureRecognizer;

		public ProfileViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			SwipeToHome ();

		}

		public void SwipeToHome()
		{
			swipeLeftGestureRecognizer = new UISwipeGestureRecognizer ( () => UpdateLeft()){Direction = UISwipeGestureRecognizerDirection.Left};
			this.View.AddGestureRecognizer (swipeLeftGestureRecognizer);

		}

		void UpdateLeft()
		{
			this.DismissViewController (true, null);
		}

		public override void ViewWillDisappear (bool animated)
		{

			foreach (var view in this.View.Subviews) {

				view.RemoveFromSuperview ();
			}


			base.ViewWillDisappear (animated);
		}


	}
}
