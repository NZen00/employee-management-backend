using System.ComponentModel.DataAnnotations;

namespace employee_management_backend.Dtos;

public class DepartmentDto
{
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "Department Code is required")]
    [MaxLength(50, ErrorMessage = "Department Code must be 50 characters or less")]
    public string DepartmentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department Name is required")]
    [MaxLength(100, ErrorMessage = "Department Name must be 100 characters or less")]
    public string DepartmentName { get; set; } = string.Empty; 
}

public class PaginationRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}