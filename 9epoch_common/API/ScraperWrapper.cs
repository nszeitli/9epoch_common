using System;
using System.Collections.Generic;
using System.Text;

namespace _9epoch_common.API
{
    public class ScraperWrapper
    {
        private RestClient _client;
        private string _api_key;
        public ScraperAPI()
        {
            _client = new RestClient("http://api.scraperapi.com");
            _api_key = "7747f26e7b6062b50aeaa14a68d37fa7";
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

        /*        public string GetHtml(string url, string countryCode = "au")
                {
                    _http.Request.Accept = HttpContentTypes.ApplicationJson;
                    string urlParameters = "api_key=" + _api_key + "&country_code=" + countryCode + "&url=" + url;
                    var response = _http.Get("http://api.scraperapi.com?" + urlParameters);
                    string rawHtml = response.RawText;

                    return rawHtml;
                }*/


    }
}
}
