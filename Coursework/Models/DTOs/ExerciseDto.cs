using System.ComponentModel.DataAnnotations;
using Coursework.Models.Entities;
using Coursework.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Coursework.Models.DTOs;

public class ExerciseDto : IName
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Название упражнения обязательно для заполнения.")]
    [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Уровень сложности обязателен.")]
    public long DifficultyId { get; set; }
    
    [Required(ErrorMessage = "Язык программирования обязателен.")]
    public long LanguageId { get; set; }
    
    
    [Range(1, 25000, ErrorMessage = "Количество очков должно быть от 0 до 25000.")]
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
    public string? S3KeyTests { get; set; }
    
    [ValidateNever]
    public long AuthorSolutionId { get; set; }
    
    [Required(ErrorMessage = "Ключ S3 для тестов обязателен.")]
    [StringLength(1024, ErrorMessage = "Ключ S3 для решения автора не должен превышать 1024 символа.")]
    public string S3KeyAuthorSolution { get; set; }
    
    [ValidateNever]
    public DifficultyLevel? Difficulty { get; set; }
    
    [ValidateNever]
    public ProgrammingLanguage? Language { get; set; }

    [ValidateNever] 
    public IList<Solution> Solutions { get; set; } = new List<Solution>();
    
    [ValidateNever]
    public IList<Framework> Frameworks { get; set; } = new List<Framework>();

    [ValidateNever]
    public IList<Hint> Hints { get; set; } = new List<Hint>();
    
    [ValidateNever]
    public long AuthorId { get; set; }
    
    [ValidateNever]
    public User? Author { get; set; }
    
    [ValidateNever]
    public IList<User> LikedByUsers { get; set; } = new List<User>();
    
    [ValidateNever]
    public bool IsLikedByCurrentUser { get; set; }
}