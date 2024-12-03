using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static Homies.Data.Common.DataConstants;

namespace Homies.Data.DataModels
{
    [Comment("The type of an event")]
    public class Type
    {
        [Key]
        [Comment("Type identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(TypeNameMaxLength)]
        [Comment("Type name")]
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Event> Events { get; set; } = new List<Event>();
    }
}
