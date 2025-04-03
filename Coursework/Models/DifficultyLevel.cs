using Coursework.Models.Interfaces;

namespace Coursework.Models;

public class DifficultyLevel : Entity, IName
{
    public string Name { get; set; }
}