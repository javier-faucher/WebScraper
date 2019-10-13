using Microsoft.AspNetCore.Mvc;
using Scraper.Data_Layer;
using Scraper.Models;
using Scraper.Service_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Controllers
{


    [Route("api/[controller]")]
    public class ScraperController: Controller
    {
        private ScraperContext Ctx;
        public ScraperController(ScraperContext ctx)
        {
            Ctx=ctx;
            scraperService = new ScraperService(Ctx);
            sessionRepo = new SessionRepo(Ctx);

        }
        ScraperService scraperService;
        private SessionRepo sessionRepo;
        public String Index()
        {
            return "Something went wrong";
        }

        [HttpPost("[action]")]
        public Object scrape([FromBody]requestBody request)
        {
            Dictionary<string, Object> response = scraperService.check(request);
            return response;
        }

        [HttpGet("[action]")]
        public Object fetch()
        {
            Object response = sessionRepo.getAllSessions();
            return response;
        }


    }
}
