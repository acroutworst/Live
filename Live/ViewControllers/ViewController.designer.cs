// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Live
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Login { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Signup { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Login != null) {
				Login.Dispose ();
				Login = null;
			}
			if (Signup != null) {
				Signup.Dispose ();
				Signup = null;
			}
		}
	}
}
