
using System.ComponentModel.DataAnnotations;

namespace DataVSOperation.App.Models
{
    public class CalculateDataCategory
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
