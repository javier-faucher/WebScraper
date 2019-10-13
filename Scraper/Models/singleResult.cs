using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Models
{
    public class singleResult
    {
        public int Id { get; set; }
        public string url { get; set; }

        public Session session { get; set; }

    }
}
