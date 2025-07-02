namespace EventManagementUpdatedProject.DTO
{
    public class EventCreateDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Location { get; set; }
        public required string OrganizerName { get; set; }
    }
}
