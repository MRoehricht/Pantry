using System.ComponentModel.DataAnnotations;

namespace Pantry.Module.Shared.Database;

public class EntityBase
{
    [Key] public Guid Id { get; set; }
}