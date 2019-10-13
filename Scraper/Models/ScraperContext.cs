using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Models
{
    public class ScraperContext : DbContext
    {
        public ScraperContext(DbContextOptions<ScraperContext> options) : base(options)
        {

        }


        public DbSet<singleResult> singleResults { get; set; }
        public DbSet<Session> sessions { get; set; }

    }
}
