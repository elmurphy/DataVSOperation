using DataVSOperation.App.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataVSOperation.App.Context
{
    public class PreparedDataDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(@"Data Source=preparedData.db");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "preparedData.json");
            UrlDataModel[] AsUrlDataModel = JObject.Parse(File.ReadAllText(filePath))["Url"].ToObject<UrlDataModel[]>();

            IList<PreparedData> preparedDatas = new List<PreparedData>();

            int id = 1;

            foreach (UrlDataModel urlDataModel in AsUrlDataModel)
            {
                bool isPage = !urlDataModel.Url.Contains('/');

                if (isPage)
                {
                    int pageId = Convert.ToInt32(urlDataModel.Url.Split('-')[1]);
                    PreparedData preparedData = new PreparedData()
                    {
                        Id = id++,
                        Url = urlDataModel.Url,
                        PageId = pageId
                    };

                    preparedDatas.Add(preparedData);
                }
                else
                {
                    string[] splitedUrl = urlDataModel.Url.Split('/');

                    string categoryUrl = splitedUrl[0];
                    string contentUrl = splitedUrl[1];

                    int contentId = Convert.ToInt32(contentUrl.Split('-')[1]);
                    int categoryId = Convert.ToInt32(categoryUrl.Split('-')[1]);

                    PreparedData preparedData = new PreparedData()
                    {
                        Id = id++,
                        Url = urlDataModel.Url,
                        ContentId = contentId,
                        CategoryId = categoryId
                    };
                    preparedDatas.Add(preparedData);
                }
            }

            //IList<PreparedData> response = preparedDatas.Where(x =>
            //(x.CategoryId.HasValue && x.CategoryId.Value == 0) ||
            //(x.ContentId.HasValue && x.ContentId.Value == 0) ||
            //(x.PageId.HasValue && x.PageId.Value == 0)
            //).ToList();

            modelBuilder.Entity<PreparedData>().HasData(preparedDatas);

        }

        public DbSet<PreparedData> PreparedDatas { get; set; }
    }
}
