using ISM6225FinalProject.Models;
using ISM6225FinalProject.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISM6225FinalProject.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserLogin> userlogin { get; set; }

        public DbSet<saveSearch> saveSearch { get; set; }


    }
}
