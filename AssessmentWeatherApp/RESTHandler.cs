using System.Threading.Tasks;

using RestSharp;
using Newtonsoft.Json;

namespace AssessmentWeatherApp
{
    public class RESTHandler
    {
        private string url;
        private IRestResponse response;
        private RestRequest request;

        public RESTHandler()
        {
            url = "";
        }

        public RESTHandler(string lurl)
        {
            url = lurl;
            request = new RestRequest();
        }

        public void AddParameter(string name, string value)
        {
            if (request != null)
            {
                request.AddParameter(name, value);
            }
        }

        public async Task<RootObject> ExecuteRequestAsync()
        {
            var client = new RestClient(url);

            response = await client.ExecuteTaskAsync(request);

            RootObject objRoot = new RootObject();
            objRoot = JsonConvert.DeserializeObject<RootObject>(response.Content);

            return objRoot;
        }
    }
}