using System.ComponentModel.DataAnnotations;

namespace Pantry.Recipe.Api.Database.Entities;

public class EntityBase
{
    [Key]
    public Guid Id { get; set; }
}
