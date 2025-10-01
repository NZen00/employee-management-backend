using employee_management_backend.Dtos;
using employee_management_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace employee_management_backend.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("paged")] // New endpoint
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving employees." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var employee = await _service.GetByIdAsync(id);
            return Ok(employee);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving employee." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] EmployeeDto employee)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.AddAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, employee);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error adding employee." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeDto employee)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(id, employee);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating employee." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting employee." });
        }
    }
}