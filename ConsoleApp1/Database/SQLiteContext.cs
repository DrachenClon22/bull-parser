using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Database
{
    public partial class SQLiteContext : DbContext
    {
        public SQLiteContext()
        {
            Database.EnsureCreated();
        }
        public virtual DbSet<Bull> Bulls { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=test.db");
    }
}
