using DataVSOperation.App.Context;
using DataVSOperation.App.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace DataVSOperation.App
{
    class Program
    {
        static bool _calculateInsteadOfSearchOnPreparedData = false;

        static string _requestedUrl = null;
        static void setupProject(string requestUrl)
        {
            #region settingUpDatabases
            using (var db = new CalculateDataDbContext())
            {
                db.Database.EnsureCreated();
            }

            using (var db = new PreparedDataDbContext())
            {
                db.Database.EnsureCreated();
            }
            #endregion

            _requestedUrl = requestUrl;
        }

        static void Main()
        {

            setupProject("/category-5/content-3");

            UrlDataModel urlResultFromCalculate;
            UrlDataModel urlResultFromPreparedData;

            urlResultFromCalculate = calculateCorrectUrl();
            urlResultFromPreparedData = searchFromPreparedData();

            System.Console.ReadLine();

        }

        static (string, string) splitUrl(string requestUrl)
        {
            // url structure
            /*
            
            1 => "/{categoryUrl}/{contentUrl}"
            2 => "/{categoryUrl|pageUrl}"

             */

            requestUrl = requestUrl.TrimStart('/');


            /*
            
            1 => "{categoryUrl}/{contentUrl}"
            2 => "{categoryUrl|pageUrl}"

             */

            bool thisIsCategoryContent = requestUrl.Contains('/');

            if (thisIsCategoryContent)
            {
                string[] splitedCategoryNContent = requestUrl.Split('/');
                return (splitedCategoryNContent[0], splitedCategoryNContent[1]);
            }
            else
            {
                return (requestUrl, null);
            }
        }

        static UrlDataModel calculateCorrectUrl()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var (categoryOrPageUrl, contentUrl) = splitUrl(_requestedUrl);

            bool thisIsPageOrCategory = contentUrl is null;


            UrlDataModel urlDataModel = new UrlDataModel();

            using (CalculateDataDbContext calculateDataDbContext = new CalculateDataDbContext())
            {
                if (thisIsPageOrCategory)
                {
                    #region CheckPage
                    CalculateDataPage page = calculateDataDbContext.CalculateDataPages.FirstOrDefault(x => x.Url.Equals(categoryOrPageUrl));
                    if (page is null)
                    {
                        #region CheckCategory
                        CalculateDataCategory category = calculateDataDbContext.CalculateDataCategories.FirstOrDefault(x => x.Url.Equals(categoryOrPageUrl));
                        if (!(category is null))
                        {
                            urlDataModel = new UrlDataModel(_requestedUrl, category.Id, null, null);
                        }
                        else
                        {
                            getNotFoundPageIfUrlNull(ref urlDataModel);
                        }
                        #endregion
                    }
                    else
                    {
                        urlDataModel = new UrlDataModel(_requestedUrl, null, null, page.Id);
                    }
                    #endregion
                }
                else
                {
                    #region CheckContent
                    CalculateDataContent content = calculateDataDbContext.CalculateDataContents.FirstOrDefault(x => x.Url.Equals(contentUrl));
                    if (!(content is null))
                    {
                        urlDataModel = new UrlDataModel(_requestedUrl, null, content.Id, null);
                    }
                    else
                    {
                        getNotFoundPageIfUrlNull(ref urlDataModel);
                    }
                    #endregion

                    #region CheckCategory
                    CalculateDataCategory category = calculateDataDbContext.CalculateDataCategories.FirstOrDefault(x => x.Url.Equals(categoryOrPageUrl));
                    if (!(category is null))
                    {
                        urlDataModel.CategoryId = category.Id;
                    }
                    else
                    {
                        getNotFoundPageIfUrlNull(ref urlDataModel);
                    }
                    #endregion
                }
            }

            getNotFoundPageIfUrlNull(ref urlDataModel);

            sw.Stop();
            System.Console.WriteLine($"Calculate url cost us: {sw.ElapsedMilliseconds} MS, Result => {JsonConvert.SerializeObject(urlDataModel)}");

            return urlDataModel;
        }

        static UrlDataModel searchFromPreparedData()
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var requestUrl = _requestedUrl.TrimStart('/');

            UrlDataModel urlDataModel = new UrlDataModel();

            using (PreparedDataDbContext preparedDataDbContext = new PreparedDataDbContext())
            {
                PreparedData preparedData = preparedDataDbContext.PreparedDatas.FirstOrDefault(x => x.Url.Equals(requestUrl));
                if (!(preparedData is null))
                {
                    urlDataModel = new UrlDataModel(_requestedUrl, preparedData.CategoryId, preparedData.ContentId, preparedData.PageId);
                }
            }

            getNotFoundPageIfUrlNull(ref urlDataModel);

            sw.Stop();
            System.Console.WriteLine($"Search url cost us: {sw.ElapsedMilliseconds} MS, Result => {JsonConvert.SerializeObject(urlDataModel)}");

            return urlDataModel;
        }

        static void getNotFoundPageIfUrlNull(ref UrlDataModel urlDataModel)
        {
            if (urlDataModel is null) urlDataModel = new UrlDataModel("/notfound.html", null, null, 0);
        }
    }
}
