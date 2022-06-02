using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcWebClient.Models;

namespace MvcWebClient.Services
{
    public interface IJobservice
    {
        Task<Job> GetJob(int jobId);
        Task<IEnumerable<Job>> GetJob();
    }
}
