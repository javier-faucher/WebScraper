using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Models
{
    public class Session
    {
        [Key]
        public DateTime requestTime { get; set; }
        public string keyWords { get; set; }
        public string requestedUrl{get; set;}
        public string query { get; set; }
        public string appearedList { get; set; }
        public int numberOfResults { get; set; }



    }
}
