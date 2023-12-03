using System.ComponentModel.DataAnnotations;

namespace Pantry.Recipe.Api.Database.Entities;

internal class EntityBase
{
    [Key]
    public Guid Id { get; set; }
}
