using System.ComponentModel.DataAnnotations;

namespace employee_management_backend.Dtos;

public class EmployeeDto
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [MaxLength(100, ErrorMessage = "First Name must be 100 characters or less")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required")]
    [MaxLength(100, ErrorMessage = "Last Name must be 100 characters or less")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email must be 255 characters or less")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of Birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    // Read-only property for auto-calculated age
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    [Required(ErrorMessage = "Salary is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than 0")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "Department is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid department")]
    public int DepartmentId { get; set; }

    public string? DepartmentName { get; set; }
    public string? DepartmentCode { get; set; }
}