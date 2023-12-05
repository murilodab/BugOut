﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugOut.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; } //Foreign key

        [DisplayName("Updated Item")]
        public string? Property { get; set; } //Ex: description, status, etc...

        [DisplayName("Previous")]
        public string? OldValue { get; set; }

        [DisplayName("Current")]
        public string? NewValue { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of Change")]
        public string? Description { get; set; }

        [DisplayName("Team Member")]
        public string? UserId { get; set; } //Foreign Key

        //Navigation Properties
        public virtual Ticket? Ticket { get; set; }
        public virtual AppUser? User { get; set; }

    }
}
