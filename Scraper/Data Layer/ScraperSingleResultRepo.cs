using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Data_Layer
{
    public class ScraperSingleResultRepo
    {
        public ScraperSingleResultRepo(ScraperContext _ctx)
        {
            ctx = _ctx;
        }
        private DbContextOptionsBuilder optionsBuilder;
        private readonly ScraperContext ctx;

        public bool saveResult(singleResult result)
        {

            try
            {
                ctx.singleResults.Add(result);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
