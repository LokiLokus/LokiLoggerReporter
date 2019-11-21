using System;
using System.Net.Http;
using System.Threading.Tasks;
using lokiloggerreporter.ViewModel.Statistic;

namespace ConsoleTest
{
    class Program
    {
        public static string ServerUrl = "http://localhost:5000";
        private static HttpClient _httpClient = new HttpClient();
        static void Main(string[] args)
        {
            _httpClient.BaseAddress = new Uri(ServerUrl);
            for (int i = 0; i < 4; i++)
            {
                CallStatistic(new RestAnalyzeRequestModel()
                {
                    Resolution = 1000,
                    Ignore404 = true,
                    SourceId = "47bc8377-cc2e-4a45-b2f1-3d991e9da67f"
                }).Wait();
            }
        }

        static async Task CallStatistic(RestAnalyzeRequestModel model)
        {
            var result = await _httpClient.PostAsJsonAsync("/GetStatistic", model);
        }
    }
}