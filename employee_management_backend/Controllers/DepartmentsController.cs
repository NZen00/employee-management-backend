using employee_management_backend.Dtos;
using employee_management_backend.Services;
using employee_management_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace employee_management_backend.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var dept = await _service.GetByIdAsync(id);
            return Ok(dept);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving department." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] DepartmentDto department)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.AddAsync(department);
            return CreatedAtAction(nameof(GetById), new { id = department.DepartmentId }, department);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("DepartmentCode", ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error adding department." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentDto department)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(id, department);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("DepartmentCode", ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating department." });
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
            return StatusCode(500, new { message = "Error deleting department." });
        }
    }
}
