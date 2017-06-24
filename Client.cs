using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Social;

namespace Youtube
{
	public class Client
	{
		private readonly YouTubeService _youtubeService;
		private const string YouTubeBaseUrl = "https://www.youtube.com/";
		private const string YouTubeVideoBaseUrl = YouTubeBaseUrl + "watch?v=";
		private const string YouTubeChannelBaseUrl = YouTubeBaseUrl + "channel/";

		/// <summary>
		/// Creates a <see cref="Client"/>.
		/// </summary>
		/// <param name="options">The <see cref="Options"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the parameters is null.</exception>
		/// <exception cref="ArgumentException">Thrown if any of the parameters is empty.</exception>
		public Client(IOptions<Options> options)
		{
			if (options == null) 
				throw new ArgumentNullException(nameof(options));

			if (string.IsNullOrWhiteSpace(options.Value?.ApiKey))
				throw new ArgumentException("Empty value isn't allowed.", nameof(options.Value.ApiKey));

			if (string.IsNullOrWhiteSpace(options.Value?.ApplicationName))
				throw new ArgumentException("Empty value isn't allowed.", nameof(options.Value.ApplicationName));

			_youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = options.Value?.ApiKey,
				ApplicationName = options.Value?.ApplicationName
			});
		}

		public async Task<IEnumerable<Result>> SearchAsync(string keyword, DateTime? publishedAfter)
		{
			if (keyword == null) 
				throw new ArgumentNullException(nameof(keyword));
			if (string.IsNullOrWhiteSpace(keyword))
				throw new ArgumentException("Empty value isn't allowed.", nameof(keyword));

			var searchListRequest = _youtubeService.Search.List("snippet");
			searchListRequest.MaxResults = 50;
			searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
			searchListRequest.Q = keyword;
			searchListRequest.Type = "video";
			searchListRequest.PublishedAfter = publishedAfter;

			var searchListResponse = await searchListRequest.ExecuteAsync();

			return searchListResponse.Items.Select(searchResult => new Result
				{
					SocialKind = new YouTube(),
					Author = searchResult.Snippet.ChannelTitle,
					AuthorUrl = YouTubeChannelBaseUrl + searchResult.Snippet.ChannelId,
					Title = searchResult.Snippet.Title,
					Description = searchResult.Snippet.Description,
					Id = searchResult.Id.VideoId,
					ThumbnailUrl = searchResult.Snippet.Thumbnails.High.Url,
					Url = YouTubeVideoBaseUrl + searchResult.Id.VideoId,
					PublishedOn = searchResult.Snippet.PublishedAt ?? DateTime.MinValue
				})
				.OrderBy(i=> i.PublishedOn).ToList();
		}
	}
}
