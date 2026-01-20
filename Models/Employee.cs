namespace PayrollSystemAPI___Technical_Exam.Models
{
    public class Employee
    {
        public string EmployeeNumber { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public decimal DailyRate { get; set; }
        public string WorkingDays { get; set; } = "MWF"; // MWF or TTHS
    }
}
