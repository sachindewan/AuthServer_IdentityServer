using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobsApi
{
    public interface IConfig
    {
        public bool RunDbMigration { get; set; }
        public bool SeedDataBase { get; set; }
    }
}
