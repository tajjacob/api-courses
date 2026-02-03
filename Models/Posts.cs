using static System.Runtime.InteropServices.JavaScript.JSType;
namespace DotnetAPI.Models
{
    public partial class Posts
    {
        public int PostId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";
        public DateTime PostCreated { get; set; } = DateTime.Now;
        public DateTime PostUpdated { get; set; } = DateTime.Now;
    }
}
