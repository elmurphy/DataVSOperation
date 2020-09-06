using System.ComponentModel.DataAnnotations;

namespace DataVSOperation.App.Models
{
    public class PreparedData
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public int? CategoryId { get; set; } = null;
        public int? ContentId { get; set; } = null;
        public int? PageId { get; set; } = null;
    }
}
