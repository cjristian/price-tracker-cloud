namespace PriceTrackerCloud.Application.DTOs.Auth;

public record AuthResponseDto(string Token, string Name, string Email, string Role);
