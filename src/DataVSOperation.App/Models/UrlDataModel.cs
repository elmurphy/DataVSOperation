namespace DataVSOperation.App.Models
{
    public class UrlDataModel
    {
        public UrlDataModel()
        {

        }
        public UrlDataModel(string url, int? categoryId, int? contentId, int? pageId)
        {
            Url = url;
            ContentId = contentId;
            CategoryId = categoryId;

        }


        public string Url { get; set; }
        public int? CategoryId { get; set; }
        public int? ContentId { get; set; }
        public int? PageId { get; set; }
    }
}
