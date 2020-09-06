using DataVSOperation.App.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DataVSOperation.App.Context
{
    public class CalculateDataDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(@"Data Source=calculateData.db");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "calculatedData.json");
            var asJObject = JObject.Parse(File.ReadAllText(filePath));

            CalculateDataCategory[] calculateDataCategories = asJObject["Categories"].ToObject<CalculateDataCategory[]>();
            modelBuilder.Entity<CalculateDataCategory>().HasData(calculateDataCategories);

            CalculateDataContent[] calculateDataContents = asJObject["Contents"].ToObject<CalculateDataContent[]>();
            modelBuilder.Entity<CalculateDataContent>().HasData(calculateDataContents);

            CalculateDataPage[] calculateDataPages = asJObject["Pages"].ToObject<CalculateDataPage[]>();
            modelBuilder.Entity<CalculateDataPage>().HasData(calculateDataPages);

        }
        public DbSet<CalculateDataCategory> CalculateDataCategories { get; set; }
        public DbSet<CalculateDataContent> CalculateDataContents { get; set; }
        public DbSet<CalculateDataPage> CalculateDataPages { get; set; }
    }
}
