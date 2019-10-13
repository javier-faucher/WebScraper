using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Models
{
    public class requestBody
    {
        [Required]
        public string keyWords { get; set; }
        [Required]
        public string query { get; set; }

        public int numOfResults { get; set; }
    }
}
