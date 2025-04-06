using Coursework.Models.Interfaces;

namespace Coursework.Models.Entities;

public class DifficultyLevel : Entity, IName
{
    public string Name { get; set; }
}