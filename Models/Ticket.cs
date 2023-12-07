using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugOut.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Title")]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Description")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }

        [DisplayName("Arquived")]
        public bool? Archived { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; } //fk

        [DisplayName("Ticket Type")]
        public int TicketTypeId { get; set; } //fk

        [DisplayName("Ticket Priority")]
        public int TicketPriorityId { get; set; } //fk

        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; } //fk

        [DisplayName("Ticket Owner")]
        public string? OwnerUserId { get; set; } //fk

        [DisplayName("Ticket Developer")]
        public string? DeveloperUserId { get; set; } //fk

        //Navigation Properties

        public virtual Project? Project { get; set; }
        public virtual TicketType? TicketType { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }
        public virtual AppUser? OwnerUser {  get; set; }
        public virtual AppUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
    }
}
