using Microsoft.AspNetCore.Mvc;
using PayrollSystemAPI___Technical_Exam.Models;
using PayrollSystemAPI___Technical_Exam.Models.DTOs;
using PayrollSystemAPI___Technical_Exam.Repositories;
using PayrollSystemAPI___Technical_Exam.Services;

namespace PayrollSystemAPI___Technical_Exam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeRepository _repository;
        private readonly PayrollService _payrollService;

        public EmployeesController(EmployeeRepository repository, PayrollService payrollService)
        {
            _repository = repository;
            _payrollService = payrollService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            var employees = await _repository.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{employeeNumber}")]
        public async Task<ActionResult<Employee>> GetByEmployeeNumber(string employeeNumber)
        {
            var employee = await _repository.GetByEmployeeNumberAsync(employeeNumber);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Create([FromBody] CreateEmployeeRequest request)
        {
            if (!IsValidWorkingDays(request.WorkingDays))
                return BadRequest(new { message = "WorkingDays must be 'MWF' or 'TTHS'" });

            var employee = new Employee
            {
                EmployeeNumber = EmployeeIDGenerator.Generate(request.LastName, request.BirthDate),
                LastName = request.LastName.ToUpper(),
                FirstName = request.FirstName.ToUpper(),
                MiddleName = request.MiddleName?.ToUpper() ?? string.Empty,
                BirthDate = request.BirthDate,
                DailyRate = request.DailyRate,
                WorkingDays = request.WorkingDays.ToUpper()
            };

            var created = await _repository.CreateAsync(employee);
            return CreatedAtAction(nameof(GetByEmployeeNumber), new { employeeNumber = created.EmployeeNumber }, created);
        }

        [HttpPut("{employeeNumber}")]
        public async Task<ActionResult<Employee>> Update(string employeeNumber, [FromBody] UpdateEmployeeRequest request)
        {
            if (!IsValidWorkingDays(request.WorkingDays))
                return BadRequest(new { message = "WorkingDays must be 'MWF' or 'TTHS'" });

            var existing = await _repository.GetByEmployeeNumberAsync(employeeNumber);
            if (existing == null)
                return NotFound(new { message = "Employee not found" });

            var employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                LastName = request.LastName.ToUpper(),
                FirstName = request.FirstName.ToUpper(),
                MiddleName = request.MiddleName?.ToUpper() ?? string.Empty,
                BirthDate = request.BirthDate,
                DailyRate = request.DailyRate,
                WorkingDays = request.WorkingDays.ToUpper()
            };

            var updated = await _repository.UpdateAsync(employeeNumber, employee);
            return Ok(updated);
        }

        [HttpDelete("{employeeNumber}")]
        public async Task<IActionResult> Delete(string employeeNumber)
        {
            var deleted = await _repository.DeleteAsync(employeeNumber);
            if (!deleted)
                return NotFound(new { message = "Employee not found" });

            return NoContent();
        }

        [HttpPost("compute-pay")]
        public async Task<ActionResult<PayrollResponse>> ComputePay([FromBody] ComputePayrollRequest request)
        {
            if (request.EndDate < request.StartDate)
                return BadRequest(new { message = "EndDate must be greater than or equal to StartDate" });

            var result = await _payrollService.ComputeTakeHomePay(request);
            if (result == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(result);
        }

        private static bool IsValidWorkingDays(string workingDays)
        {
            return workingDays.ToUpper() is "MWF" or "TTHS";
        }
    }
}
