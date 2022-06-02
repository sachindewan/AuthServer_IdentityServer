using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobsApi.Data
{
    public class JobsContext:DbContext
    {
        public JobsContext(DbContextOptions<JobsContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().ToTable("Jobs");
            //base.OnModelCreating(modelBuilder);
        }

        public DbSet<Job> Jobs { get; set; }
    }
}
