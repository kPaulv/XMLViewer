using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TestTask.Models
{
    public class CardContext : DbContext
    {
        public DbSet<Card> Cards { get; set; } 
    }
}