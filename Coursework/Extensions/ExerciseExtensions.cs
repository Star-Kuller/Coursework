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
            Score = exerciseDto.Score,
            ShortDescription = exerciseDto.ShortDescription,
            FullDescription = exerciseDto.FullDescription,
            IsPublished = exerciseDto.IsPublished,
            S3KeySource = exerciseDto.S3KeySource,
            S3KeyTests = exerciseDto.S3KeyTests,
            Frameworks = exerciseDto.Frameworks,
            AuthorSolution = new Solution
            {
                ExerciseId = exerciseDto.Id,
                S3Key = exerciseDto.S3KeyAuthorSolution,
                ByExerciseAuthor = true
            }
        };
    }
    
    public static ExerciseDto Map(this Exercise exercise)
    {
        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            DifficultyId = exercise.DifficultyId,
            Score = exercise.Score,
            ShortDescription = exercise.ShortDescription,
            FullDescription = exercise.FullDescription,
            IsPublished = exercise.IsPublished,
            S3KeySource = exercise.S3KeySource,
            S3KeyTests = exercise.S3KeyTests,
            Frameworks = exercise.Frameworks,
            AuthorSolutionId = exercise.AuthorSolution?.Id,
            S3KeyAuthorSolution = exercise.AuthorSolution!.S3Key
        };
    }
}