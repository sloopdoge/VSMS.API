﻿namespace VSMS.Domain.DTOs;

public class UserLoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool UseLongLivedToken { get; set; } = false;
}