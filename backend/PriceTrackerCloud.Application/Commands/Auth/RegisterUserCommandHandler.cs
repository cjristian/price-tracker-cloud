using MediatR;
using PriceTrackerCloud.Application.DTOs.Auth;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Commands.Auth;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtGenerator;

    public RegisterUserCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtGenerator)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Users.ExistsWithEmailAsync(request.Email))
            throw new InvalidOperationException($"El email '{request.Email}' ya está registrado.");

        var user = new User
        {
            Name         = request.Name,
            Email        = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync(cancellationToken);

        var token = _jwtGenerator.GenerateToken(user);
        return new AuthResponseDto(token, user.Name, user.Email, user.Role.ToString());
    }
}
