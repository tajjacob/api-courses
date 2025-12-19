namespace DotnetAPI
{
    public partial class UserJobInfo // partial explanation: allows the class definition to be split across multiple files
    {
        public int UserId { get; set; }
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";
    }
}