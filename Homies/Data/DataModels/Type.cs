using System.ComponentModel.DataAnnotations;
using static Homies.Data.Common.DataConstants;

namespace Homies.Data.DataModels
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TypeNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Event> Events { get; set; } = new List<Event>();
    }
}
