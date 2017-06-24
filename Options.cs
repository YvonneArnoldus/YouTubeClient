using System.ComponentModel.DataAnnotations;
using Social;

namespace Youtube
{
	public class Options : ISocialOptions
	{
		[Required]
		public string ApplicationName { get; set; }

		[Required]
		public string ApiKey { get; set; }
	}
}
