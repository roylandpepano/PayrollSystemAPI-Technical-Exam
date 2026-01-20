using PayrollSystemAPI___Technical_Exam.Models;
using PayrollSystemAPI___Technical_Exam.Models.DTOs;
using PayrollSystemAPI___Technical_Exam.Repositories;

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

            decimal totalPay = CalculatePay(employee, request.StartDate, request.EndDate);

            return new PayrollResponse
            {
                EmployeeNumber = employee.EmployeeNumber,
                EmployeeName = $"{employee.LastName}, {employee.FirstName}",
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TakeHomePay = totalPay
            };
        }

        public decimal CalculatePay(Employee emp, DateTime start, DateTime end)
        {
            decimal totalPay = 0;

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                bool isWorkDay = IsScheduled(emp.WorkingDays, date.DayOfWeek);
                bool isBirthday = date.Month == emp.BirthDate.Month && date.Day == emp.BirthDate.Day;

                if (isWorkDay)
                {
                    totalPay += emp.DailyRate * 2;
                }
                else if (isBirthday)
                {
                    totalPay += emp.DailyRate;
                }
            }
            return totalPay;
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
