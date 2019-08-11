using System.Net.Http;

namespace CoreX.Extensions.Http
{
    public class Http
    {
        private readonly HttpClient client;

        public Http(HttpClient httpClient)
        {
            this.client = httpClient;
        }

        public string Get(string url)
        {
            var result = client.GetAsync(url).Result;
            result.EnsureSuccessStatusCode();

            return result.Content.ReadAsStringAsync().Result;
        }

        public T Get<T>(string url)
        {
            var result = client.GetAsync(url).Result;
            result.EnsureSuccessStatusCode();

            return result.Content.ReadAsAsync<T>().Result;
        }
    }
}
