using System.ComponentModel.DataAnnotations;
using Coursework.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Coursework.Models.Entities;

public class Hint : Entity
{
    [Required(ErrorMessage = "ID упражнения обязательно")]
    public long ExerciseId { get; set; }
    
    [Required(ErrorMessage = "Стоимость подсказки обязательна")]
    [Range(1, int.MaxValue, ErrorMessage = "Стоимость должна быть положительной")]
    public int Cost { get; set; }
    
    [Required(ErrorMessage = "Текст подсказки обязателен")]
    [StringLength(1000, ErrorMessage = "Текст подсказки не должен превышать 1000 символов")]
    public string Text { get; set; }
    
    [ValidateNever]
    public Exercise? Exercise { get; set; }
    
    [ValidateNever]
    public IList<User> OpenedByUsers { get; set; } = new List<User>();
}