
using System.ComponentModel.DataAnnotations;

namespace DataVSOperation.App.Models
{
    public class CalculateDataContent
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
