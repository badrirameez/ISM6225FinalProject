using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ISM6225FinalProject.Models
{
    public class PlanValidation
    {
        //  names of input variables in plan page

        public string from_date { get; set; }
        public string to_date { get; set; }

        public string location1 { get; set; }

        public string location2 { get; set; }

        public string nature { get; set; }

        public string city { get; set; }

        public string outdoor { get; set; }
    }
}
