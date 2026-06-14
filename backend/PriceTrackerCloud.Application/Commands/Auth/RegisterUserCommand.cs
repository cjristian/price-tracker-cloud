using MediatR;
using PriceTrackerCloud.Application.DTOs.Auth;

namespace PriceTrackerCloud.Application.Commands.Auth;

public record RegisterUserCommand(string Name, string Email, string Password) : IRequest<AuthResponseDto>;
