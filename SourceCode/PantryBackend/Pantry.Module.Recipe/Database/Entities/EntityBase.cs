using System.ComponentModel.DataAnnotations;

namespace Pantry.Module.Recipe.Database.Entities;

public class EntityBase
{
    [Key]
    public Guid Id { get; set; }
}
