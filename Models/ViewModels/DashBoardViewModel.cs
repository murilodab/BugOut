using Org.BouncyCastle.Bcpg.OpenPgp;

namespace BugOut.Models.ViewModels
{
    public class DashBoardViewModel
    {
        public  Company Company { get; set; }
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<AppUser> Members { get; set; }
    }
}
