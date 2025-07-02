namespace EventManagementUpdatedProject.DTO
{
    public class RegisterDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string ContactNumber { get; set; }
        public required string Type { get; set; }
    }
}
