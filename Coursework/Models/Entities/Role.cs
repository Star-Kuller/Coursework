using System.ComponentModel.DataAnnotations;
using Coursework.Models.Interfaces;

namespace Coursework.Models.Entities;

public class Role : Entity, IName
{
    public string Name { get; set; }
}