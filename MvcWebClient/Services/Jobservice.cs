using MvcWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MvcWebClient.Config;
using MvcWebClient.Http;
using MvcWebClient.Infrastructure;
using Newtonsoft.Json;

namespace MvcWebClient.Services
{
    public class Jobservice : IJobservice
    {
        //private readonly IHttpClient apiClient;
        private readonly HttpClient _apiClient;
        private readonly ApiConfig  apiConfig;
        private readonly string _remoteServiceBaseUrl;

        public Jobservice(HttpClient apiClient, IOptionsMonitor<ApiConfig> apiConfig)
        {
            _apiClient = apiClient;
            this.apiConfig = apiConfig.CurrentValue;
            _remoteServiceBaseUrl = $"{this.apiConfig.JobsApiUrl}/jobs";
        }
        public async Task<Job> GetJob(int jobId)
        {
            var uri = API.GetJob(_remoteServiceBaseUrl, jobId);
            var responseString = await _apiClient.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<Job>(responseString);

        }

        public async Task<IEnumerable<Job>> GetJob()
        {
            var uri = API.GetAllJobs(_remoteServiceBaseUrl);
            var responseString = await _apiClient.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<IEnumerable<Job>>(responseString);
        }
    }
}
