using CandidateBackgrounder.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateBackgrounder
{
    public class Context : DbContext
    {
        public Context()
            : base()
        {
        }

        public DbSet<Candidate> Candidates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = (string)JObject.Parse(File.ReadAllText("Config.json"))["ConnectionString"];
            optionsBuilder.UseSqlServer(connectionString);
        }

    }

}
