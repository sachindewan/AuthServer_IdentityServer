namespace MvcWebClient.Infrastructure
{
    public static class API
    {
        public static string GetJob(string baseUri, int jobId)
        {
            return $"{baseUri}/{jobId}";
        }
        public static string GetAllJobs(string baseUri)
        {
            return baseUri;
        }
    }
}
