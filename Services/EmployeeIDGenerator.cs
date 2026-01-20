namespace PayrollSystemAPI___Technical_Exam.Services
{
    public static class EmployeeIDGenerator
    {
        private static readonly Random _random = new();

        public static string Generate(string lastName, DateTime dob)
        {
            string cleanedName = lastName.Replace(" ", "").ToUpper();
            string prefix = cleanedName.Length >= 3 
                ? cleanedName[..3] 
                : cleanedName.PadRight(3, '*');
            
            string randomDigits = _random.Next(0, 100000).ToString("D5");
            string dobPart = dob.ToString("ddMMMyyyy").ToUpper();
            
            return $"{prefix}-{randomDigits}-{dobPart}";
        }
    }
}