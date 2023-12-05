using System.ComponentModel;

namespace BugOut.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [DisplayName("Priority Name")]
        public string? Name { get; set; }

    }
}
