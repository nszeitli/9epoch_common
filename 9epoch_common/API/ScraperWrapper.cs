using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _9epoch_common.API
{
    public class ScraperWrapper
    {
        private RestClient _client;
        private string _api_key;
        public ScraperWrapper()
        {
            _client = new RestClient("http://api.scraperapi.com");
            ApiKeys keys = new ApiKeys();
            _api_key = keys.KeyStore.ScraperKey;
        }


        public async Task<string> GetHtmlAsync(string url, string countryCode = "au")
        {
            var request = new RestRequest("");
            request.AddParameter("api_key", _api_key); // adds to POST or URL querystring based on Method
            request.AddParameter("country_code", countryCode); // adds to POST or URL querystring based on Method
            request.AddParameter("url", url); // adds to POST or URL querystring based on Method


            var result = await _client.ExecuteTaskAsync(request);

            return result.Content;

        }

        public string GetHtml(string url, string countryCode = "au")
        {
            var request = new RestRequest("");
            request.AddParameter("api_key", _api_key); // adds to POST or URL querystring based on Method
            request.AddParameter("country_code", countryCode); // adds to POST or URL querystring based on Method
            request.AddParameter("url", url); // adds to POST or URL querystring based on Method


            var result =  _client.Execute(request);

            return result.Content;
        }


    }

}
