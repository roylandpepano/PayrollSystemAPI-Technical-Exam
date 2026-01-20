namespace PayrollSystemAPI___Technical_Exam.Models.DTOs
{
    public class PayrollResponse
    {
        public string EmployeeNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TakeHomePay { get; set; }
        public string FormattedPay => $"Php {TakeHomePay:N2}";
    }
}
