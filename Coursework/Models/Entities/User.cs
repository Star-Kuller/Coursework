using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Coursework.Models.Entities;

public class User : Entity
{
    [Required(ErrorMessage = "Имя пользователя обязательно для заполнения")]
    [StringLength(100, ErrorMessage = "Имя пользователя не должно превышать 100 символов")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email обязателен для заполнения")]
    [StringLength(255, ErrorMessage = "Email не должен превышать 255 символов")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Пароль обязателен для заполнения")]
    [StringLength(255, ErrorMessage = "Хеш пароля не должен превышать 255 символов")]
    public string PasswordHash { get; set; }
    
    public string About { get; set; } = "";
    
    public long Score { get; set; } = 0;
    
    [Required(ErrorMessage = "Роль пользователя обязательна")]
    public long RoleId { get; set; }
    
    [ValidateNever]
    public Role? Role { get; set; }
    
    [ValidateNever]
    public IList<Exercise> Exercises { get; set; } = new List<Exercise>();
    
    [ValidateNever]
    public IList<Solution> Solutions { get; set; } = new List<Solution>();
}