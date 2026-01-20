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
        public string ComputationDescription { get; set; } = string.Empty;
        public List<PayrollComputationDetail> ComputationDetails { get; set; } = new();
    }

    public class PayrollComputationDetail
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string PayType { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
