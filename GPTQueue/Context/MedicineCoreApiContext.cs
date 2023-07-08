using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;


    public class MedicineCoreApiContext : DbContext
    {

        public DbSet<PostMedicine> Medicine { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DBFarmacos;Trusted_Connection=True;persist security info=True;");
        }
    
    }

