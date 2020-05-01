using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.NewsSearch;
using TravelMonkey.Models;

namespace TravelMonkey.Services
{
    public class BingSearchService
    {
        private Random _randomResultIndex = new Random();

        private async Task<List<News>> GetNews(string destination)
        {
            try
            {
                var client = new NewsSearchClient(new Microsoft.Azure.CognitiveServices.Search.NewsSearch.ApiKeyServiceClientCredentials(ApiKeys.BingImageSearch));

                var resultNews = new List<News>();

                var result = await client.News.SearchAsync(destination);

                int currentResult = 0;
                while(currentResult < 5 || result.Value.Count < currentResult)
                {
                    resultNews.Add(new News()
                    {
                        Title = result.Value[currentResult].Name,
                        Description = result.Value[currentResult].Description,
                        ImageUrl = result.Value[currentResult].Image.Thumbnail.ContentUrl,
                        MoreInfoUrl = result.Value[currentResult].Url
                    });

                    currentResult++;
                }

                return resultNews;
            }
            catch
            {
                return new List<News> {
                    new News
                    {
                        Title = "Something went wrong :( Here is a cat instead!",
                        ImageUrl = "https://cataas.com/cat",
                        MoreInfoUrl = "https://cataas.com/"
                    }
                };
            }
        }

        public async Task<List<Destination>> GetDestinations()
        {
            var searchDestinations = new[] { "Seattle", "Maui", "Amsterdam", "Antarctica", "Buenos Aires" };

            try
            {
                var client = new ImageSearchClient(new Microsoft.Azure.CognitiveServices.Search.ImageSearch.ApiKeyServiceClientCredentials(ApiKeys.BingImageSearch));

                var resultDestinations = new List<Destination>();

                foreach (var destination in searchDestinations)
                {
                    var result = await client.Images.SearchAsync(destination, color: "blue", minWidth: 500, minHeight: 500, imageType: "Photo", license: "Public", count: 5, maxHeight: 1200, maxWidth: 1200);

                    var randomIdx = _randomResultIndex.Next(result.Value.Count);

                    resultDestinations.Add(new Destination
                    {
                        Title = destination,
                        ImageUrl = result.Value[randomIdx].ContentUrl,
                        MoreInfoUrl = result.Value[randomIdx].HostPageUrl,
                        News = await GetNews(destination)
                    });
                }

                return resultDestinations;
            }
            catch
            {
                return new List<Destination> {
                    new Destination
                    {
                        Title = "Something went wrong :( Here is a cat instead!",
                        ImageUrl = "https://cataas.com/cat",
                        MoreInfoUrl = "https://cataas.com/"
                    }
                };
            }
        }
    }
}