using System;
using Newtonsoft.Json;
using UIKit;

namespace Live
{
	public class User
	{
		public string Id { get; set; }

		[JsonProperty ("name")]
		public string Name { get; set; }

		[JsonProperty ("profileImage")]
		public UIImageView ProfileImage { get; set; }

		[JsonProperty ("bio")]
		public string Bio { get; set; }

	}
}

