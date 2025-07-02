namespace EventManagementUpdatedProject.DTO
{
    public class EventResponseDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required string Location { get; set; }
        public required string OrganizerName { get; set; }
    }
}
