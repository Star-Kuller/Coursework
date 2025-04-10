using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Coursework.Models.Entities;

public class Solution : Entity
{
    [Required(ErrorMessage = "Ключ S3 обязателен.")]
    [StringLength(1024, ErrorMessage = "Ключ S3 не должен превышать 1024 символа.")]
    public string S3Key { get; set; }
    
    [Required(ErrorMessage = "Id упражнения обязателено.")]
    public long ExerciseId { get; set; }
    
    [Required(ErrorMessage = "Id владельца обязателен.")]
    public long AuthorId { get; set; }
    
    [ValidateNever]
    public Exercise? Exercise { get; set; }
    
    [ValidateNever]
    public User? Author { get; set; }
}