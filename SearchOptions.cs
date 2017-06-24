using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Social;

namespace Youtube
{
	public class SearchOptions
	{
		[Required]
		public List<Option> Options{ get; set; }
	}

	public class Option : ISearchOption
	{
		[Required]
		public List<string> Keywords { get; set; }

		public DateTime PublishedAfter { get; set; } = DateTime.UtcNow;

		[Required]
		public string SlackChannel { get; set; }
	}
}
