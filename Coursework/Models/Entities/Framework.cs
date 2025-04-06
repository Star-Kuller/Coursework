using System.ComponentModel.DataAnnotations;
using Coursework.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Coursework.Models.Entities;

public class Framework : Entity, IName
{
    [Required(ErrorMessage = "Название фреймворка обязательно для заполнения.")]
    [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Описание фреймворка обязательно для заполнения.")]
    [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Язык программирования обязателен.")]
    public long? LanguageId { get; set; }

    [ValidateNever]
    public ProgrammingLanguage? Language { get; set; }
}