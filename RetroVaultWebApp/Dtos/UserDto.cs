namespace RetroVaultWebApp.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsDisabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}

public class UpdateUserDto
{
    public string? Password { get; set; }
    public string? Alias { get; set; }
    public bool? IsAdmin { get; set; }
    public bool? IsDisabled { get; set; }
}
