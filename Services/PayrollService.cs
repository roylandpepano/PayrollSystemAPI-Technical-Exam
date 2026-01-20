using PayrollSystemAPI___Technical_Exam.Models;
using PayrollSystemAPI___Technical_Exam.Models.DTOs;
using PayrollSystemAPI___Technical_Exam.Repositories;
using System.Text;

namespace PayrollSystemAPI___Technical_Exam.Services
{
    public class PayrollService
    {
        private readonly EmployeeRepository _repository;

        public PayrollService(EmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PayrollResponse?> ComputeTakeHomePay(ComputePayrollRequest request)
        {
            var employee = await _repository.GetByEmployeeNumberAsync(request.EmployeeNumber);
            if (employee == null) return null;

            var computationDetails = new List<PayrollComputationDetail>();
            decimal totalPay = CalculatePayWithDetails(employee, request.StartDate, request.EndDate, computationDetails);

            var description = BuildComputationDescription(employee, request.StartDate, request.EndDate, computationDetails, totalPay);

            return new PayrollResponse
            {
                EmployeeNumber = employee.EmployeeNumber,
                EmployeeName = $"{employee.LastName}, {employee.FirstName}",
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TakeHomePay = totalPay,
                ComputationDescription = description,
                ComputationDetails = computationDetails
            };
        }

        public decimal CalculatePay(Employee emp, DateTime start, DateTime end)
        {
            var details = new List<PayrollComputationDetail>();
            return CalculatePayWithDetails(emp, start, end, details);
        }

        private decimal CalculatePayWithDetails(Employee emp, DateTime start, DateTime end, List<PayrollComputationDetail> details)
        {
            decimal totalPay = 0;

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                bool isWorkDay = IsScheduled(emp.WorkingDays, date.DayOfWeek);
                bool isBirthday = date.Month == emp.BirthDate.Month && date.Day == emp.BirthDate.Day;

                if (isWorkDay)
                {
                    decimal amount = emp.DailyRate * 2;
                    totalPay += amount;
                    details.Add(new PayrollComputationDetail
                    {
                        Date = date,
                        DayOfWeek = date.DayOfWeek.ToString(),
                        PayType = "Work Day",
                        DailyRate = emp.DailyRate,
                        Amount = amount,
                        Description = $"Work Day: {emp.DailyRate:N2} × 2 = {amount:N2}"
                    });
                }
                
                if (isBirthday)
                {
                    decimal amount = emp.DailyRate;
                    totalPay += amount;
                    details.Add(new PayrollComputationDetail
                    {
                        Date = date,
                        DayOfWeek = date.DayOfWeek.ToString(),
                        PayType = "Birthday",
                        DailyRate = emp.DailyRate,
                        Amount = amount,
                        Description = $"Birthday Bonus: {emp.DailyRate:N2}"
                    });
                }
            }
            return totalPay;
        }

        private string BuildComputationDescription(Employee emp, DateTime start, DateTime end, List<PayrollComputationDetail> details, decimal total)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"Pay Computation for {emp.LastName}, {emp.FirstName} (Employee #{emp.EmployeeNumber})");
            sb.AppendLine($"Period: {start:MMM dd, yyyy} to {end:MMM dd, yyyy}");
            sb.AppendLine($"Daily Rate: Php {emp.DailyRate:N2}");
            sb.AppendLine($"Working Schedule: {emp.WorkingDays}");
            sb.AppendLine();
            sb.AppendLine("Breakdown:");

            var workDays = details.Where(d => d.PayType == "Work Day").ToList();
            var birthdays = details.Where(d => d.PayType == "Birthday").ToList();

            if (workDays.Any())
            {
                sb.AppendLine($"  Work Days ({workDays.Count} day(s) × Daily Rate × 2):");
                foreach (var day in workDays)
                {
                    sb.AppendLine($"    {day.Date:MMM dd, yyyy} ({day.DayOfWeek}): Php {day.DailyRate:N2} × 2 = Php {day.Amount:N2}");
                }
                sb.AppendLine($"  Subtotal: Php {workDays.Sum(d => d.Amount):N2}");
                sb.AppendLine();
            }

            if (birthdays.Any())
            {
                sb.AppendLine($"  Birthday Bonus ({birthdays.Count} day(s)):");
                foreach (var day in birthdays)
                {
                    sb.AppendLine($"    {day.Date:MMM dd, yyyy} ({day.DayOfWeek}): Php {day.Amount:N2}");
                }
                sb.AppendLine($"  Subtotal: Php {birthdays.Sum(d => d.Amount):N2}");
                sb.AppendLine();
            }

            sb.AppendLine($"Total Take Home Pay: Php {total:N2}");

            return sb.ToString();
        }

        private static bool IsScheduled(string schedule, DayOfWeek day)
        {
            return schedule switch
            {
                "MWF" => day == DayOfWeek.Monday || day == DayOfWeek.Wednesday || day == DayOfWeek.Friday,
                "TTHS" => day == DayOfWeek.Tuesday || day == DayOfWeek.Thursday || day == DayOfWeek.Saturday,
                _ => false
            };
        }
    }
}
