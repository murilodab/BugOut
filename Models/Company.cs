using System.ComponentModel;

namespace BugOut.Models
{
    public class Company
    {
        public int Id { get; set; }

        [DisplayName("Company Name")]
        public string? Name { get; set; }

        [DisplayName("Company Description")]
        public string? Description { get; set; }


        //Navigation properties

        public virtual ICollection<AppUser>? Members { get; set; } = new HashSet<AppUser>();
        public virtual ICollection<Project>? Projects { get; set; } = new HashSet<Project>();


    }
}
