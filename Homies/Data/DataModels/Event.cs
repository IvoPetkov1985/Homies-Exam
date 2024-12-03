using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Homies.Data.Common.DataConstants;

namespace Homies.Data.DataModels
{
    [Comment("The event, organised by homies")]
    public class Event
    {
        [Key]
        [Comment("Event identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(EventNameMaxLength)]
        [Comment("Event name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        [Comment("Detailed information about the event")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Comment("User (organiser) identifier")]
        public string OrganiserId { get; set; } = string.Empty;

        [ForeignKey(nameof(OrganiserId))]
        public IdentityUser Organiser { get; set; } = null!;

        [Required]
        [Comment("Date and time of the event creation")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Comment("When the event starts")]
        public DateTime Start { get; set; }

        [Required]
        [Comment("When the event ends")]
        public DateTime End { get; set; }

        [Required]
        [Comment("Type identifier")]
        public int TypeId { get; set; }

        [ForeignKey(nameof(TypeId))]
        public Type Type { get; set; } = null!;

        public IEnumerable<EventParticipant> EventsParticipants { get; set; } = new List<EventParticipant>();
    }
}
