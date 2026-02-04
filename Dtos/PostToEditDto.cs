using static System.Runtime.InteropServices.JavaScript.JSType;
namespace DotnetAPI.Dtos
{
    public partial class PostToEditDto
    {
        public int PostId { get; set; } 
        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";
     
    }
}
