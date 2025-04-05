using System.ComponentModel.DataAnnotations;
using Coursework.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Coursework.Models;

public class Exercise : Entity, IName
{
    [Required(ErrorMessage = "Название упражнения обязательно для заполнения.")]
    [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Уровень сложности обязателен.")]
    public long DifficultyId { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "Количество баллов должно быть больше 0.")]
    [Required(ErrorMessage = "Количество очков обязательно.")]
    public long Score { get; set; }

    [StringLength(150, ErrorMessage = "Краткое описание не должно превышать 150 символов.")]
    public string? ShortDescription { get; set; }

    [Required(ErrorMessage = "Текст упражнения обязателен.")]
    public string FullDescription { get; set; }
    public bool IsPublished { get; set; }

    [StringLength(1024, ErrorMessage = "Ключ S3 для материалов упражнения не должен превышать 1024 символа.")]
    public string? S3KeySource { get; set; }

    [Required(ErrorMessage = "Ключ S3 для тестов обязателен.")]
    [StringLength(1024, ErrorMessage = "Ключ S3 для тестов не должен превышать 1024 символа.")]
    public string S3KeyTests { get; set; }
    
    [ValidateNever]
    public DifficultyLevel? Difficulty { get; set; }
    [ValidateNever]
    public IList<Framework>? Frameworks { get; set; }
}