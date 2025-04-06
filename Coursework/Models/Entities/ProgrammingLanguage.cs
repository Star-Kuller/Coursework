using System.ComponentModel.DataAnnotations;
using Coursework.Models.Interfaces;

namespace Coursework.Models.Entities;

public class ProgrammingLanguage : Entity, IName
{
    [Required(ErrorMessage = "Название языка обязательно для заполнения.")]
    [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Описание языка обязательно для заполнения.")]
    [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
    public string Description { get; set; }
}