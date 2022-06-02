using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobsApi.Data;
using JobsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace JobsApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly JobsContext _jobsContext;
        private readonly ILogger<JobsController> _logger;

        public JobsController(ILogger<JobsController> logger, JobsContext jobsContext)
        {
            _logger = logger;
            _jobsContext = jobsContext;
        }

        [HttpGet]
       [Authorize(Roles = "Manager")]
        public async Task<IEnumerable<Job>> Get()
        {
            return await _jobsContext.Jobs.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<Job> GetById(int id)
        {
            return await _jobsContext.Jobs.FindAsync(id);
        }
    }
}
