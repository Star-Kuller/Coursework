using Coursework.Models.Interfaces;
namespace Coursework.Models.Entities;

public class Exercise : Entity, IName
{
    public string Name { get; set; }
    
    public long DifficultyId { get; set; }
    public long LanguageId { get; set; }
    
    public long Score { get; set; }
    
    public string? ShortDescription { get; set; }

    public string FullDescription { get; set; }
    public bool IsPublished { get; set; }
    
    public string? S3KeySource { get; set; }
    
    public string S3KeyTests { get; set; }
    
    public DifficultyLevel? Difficulty { get; set; }
    public ProgrammingLanguage? Language { get; set; }
    
    public long AuthorId { get; set; }
    public User? Author { get; set; }
    
    public Solution? AuthorSolution { get; set; }
    public IList<Framework> Frameworks { get; set; } = new List<Framework>();
    public IList<Solution> Solutions { get; set; } = new List<Solution>();
    
    public IList<Hint> Hints { get; set; } = new List<Hint>();
}