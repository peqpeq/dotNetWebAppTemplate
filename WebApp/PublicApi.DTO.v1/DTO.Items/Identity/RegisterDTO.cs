namespace PublicApi.DTO.v1.DTO.Items.Identity
{
    public class RegisterDTO
    {
        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? AvatarImg { get; set; }
        public string? Gender { get; set; }
    }

}