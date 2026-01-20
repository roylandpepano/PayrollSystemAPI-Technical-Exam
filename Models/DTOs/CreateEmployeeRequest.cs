namespace PayrollSystemAPI___Technical_Exam.Models.DTOs
{
    public class CreateEmployeeRequest
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public decimal DailyRate { get; set; }
        public string WorkingDays { get; set; } = "MWF";
    }
}
