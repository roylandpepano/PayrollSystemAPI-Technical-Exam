namespace PayrollSystemAPI___Technical_Exam.Models.DTOs
{
    public class ComputePayrollRequest
    {
        public string EmployeeNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
