namespace DotnetAPI.Models
{
    public partial class UserSalary // partial explanation: allows the class definition to be split across multiple files
    {
        public int UserId { get; set; }
        public decimal Salary { get; set; } 
        public decimal AvgSalary { get; set; }
    }
}