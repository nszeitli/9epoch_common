using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace _9epoch_common.API
{
    class _9epochWrapper
    {
        public string BaseURL { get; set; }
        private string _apiKey;

        //SET TEST TO TRUE TO USE LOCAL ENDPOINTS
        public _9epochWrapper(bool test = false)
        {
            if (test) { BaseURL = "https://www.9epoch.ai/"; }
            else
            {
                BaseURL = "https://www.9epoch.ai/";
            }

            ApiKeys keys = new ApiKeys();
            _apiKey = keys.KeyStore._9epochKey;
        }

        public string GetJson(string requestStr)
        {
            //requestStr = "api/Announcement/getdate?date=" + dateStr;
            var client = new RestClient(BaseURL);

            var request = new RestRequest(requestStr, Method.GET);

            //auth
            request.AddHeader("ApiKey", _apiKey);

            var response = client.Execute(request);
            return response.Content;
        }
        public void PostJson(string json, string endpoint)
        {
            if(endpoint[0] == '/') { endpoint = endpoint.Remove(0, 1); }
            string url = BaseURL + endpoint;
            PostToURL(json, url);
        }

        private void PostToURL(string json, string url)
        {
            var client = new RestClient(BaseURL);

            var request = new RestRequest(url, Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            //auth
            request.AddHeader("ApiKey", _apiKey);

            // execute the request
            var response = client.Execute(request);
            var content = response.Content; // raw content as string
        }



    }


}
