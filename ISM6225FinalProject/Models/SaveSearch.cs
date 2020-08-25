using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ISM6225FinalProject.Models
{
    public class saveSearch
    {
        [Key]
        public int Id { get; set; }
        public string user { get; set; }
        public string destination { get; set; }

    }
}

