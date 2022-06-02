using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobsApi.Data;
using JobsApi.Models;

namespace JobsApi
{
    public static class DatabaseInializer
    {
        public static void Initialize(JobsContext context)
        {
            context.Database.EnsureCreated();
            if (context.Jobs.Any())
            {
                return; //database has already been seeded
            }

            var jobs = new Job[]
            {

            };
            context.Jobs.AddRange(jobs);
        }
    }
}
