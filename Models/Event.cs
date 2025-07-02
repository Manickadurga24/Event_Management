using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementUpdatedProject.Models
{
    public class Event
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Location { get; set; }
        public required string OrganizerName { get; set; }
    }
}
