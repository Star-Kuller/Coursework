using Coursework.Models.DTOs;
using Coursework.Models.Entities;

namespace Coursework.Extensions;

public static class ExerciseExtensions
{
    public static Exercise Map(this ExerciseDto exerciseDto)
    {
        return new Exercise
        {
            Id = exerciseDto.Id,
            Name = exerciseDto.Name,
            DifficultyId = exerciseDto.DifficultyId,
            LanguageId = exerciseDto.LanguageId,
            Score = exerciseDto.Score,
            ShortDescription = exerciseDto.ShortDescription,
            FullDescription = exerciseDto.FullDescription,
            IsPublished = exerciseDto.IsPublished,
            S3KeySource = exerciseDto.S3KeySource,
            S3KeyTests = exerciseDto.S3KeyTests!,
            Frameworks = exerciseDto.Frameworks,
            AuthorId = exerciseDto.AuthorId,
            AuthorSolution = new Solution
            {
                Id = exerciseDto.AuthorSolutionId,
                S3Key = exerciseDto.S3KeyAuthorSolution,
                ExerciseId = exerciseDto.Id,
                AuthorId = exerciseDto.AuthorId
            },
            Hints = exerciseDto.Hints.ToList(),
            LikedByUsers = exerciseDto.LikedByUsers
        };
    }
    
    public static ExerciseDto Map(this Exercise exercise)
    {
        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            DifficultyId = exercise.DifficultyId,
            Difficulty = exercise.Difficulty,
            LanguageId = exercise.LanguageId,
            Language = exercise.Language,
            Score = exercise.Score,
            ShortDescription = exercise.ShortDescription,
            FullDescription = exercise.FullDescription,
            IsPublished = exercise.IsPublished,
            S3KeySource = exercise.S3KeySource,
            S3KeyTests = exercise.S3KeyTests,
            Frameworks = exercise.Frameworks,
            AuthorId = exercise.AuthorId,
            Author = exercise.Author,
            AuthorSolutionId = exercise.AuthorSolution?.Id ?? default,
            S3KeyAuthorSolution = exercise.AuthorSolution?.S3Key!,
            Solutions = exercise.Solutions,
            Hints = exercise.Hints.ToList(),
            LikedByUsers = exercise.LikedByUsers
        };
    }
    
    public static ExerciseDto MapWithCurrentUser(this Exercise exercise, long currentUserId)
    {
        var dto = exercise.Map();
        dto.IsLikedByCurrentUser = exercise.LikedByUsers.Any(u => u.Id == currentUserId);
        return dto;
    }
}