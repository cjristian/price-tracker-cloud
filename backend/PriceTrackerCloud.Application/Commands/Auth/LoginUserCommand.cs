using MediatR;
using PriceTrackerCloud.Application.DTOs.Auth;

namespace PriceTrackerCloud.Application.Commands.Auth;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponseDto>;
