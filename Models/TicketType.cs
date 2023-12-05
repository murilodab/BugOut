using System.ComponentModel;

namespace BugOut.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string? Name { get; set; }



    }
}
