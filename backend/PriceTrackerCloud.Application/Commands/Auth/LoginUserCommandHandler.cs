using MediatR;
using PriceTrackerCloud.Application.DTOs.Auth;
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Application.Commands.Auth;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtGenerator;

    public LoginUserCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtGenerator)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        var token = _jwtGenerator.GenerateToken(user);
        return new AuthResponseDto(token, user.Name, user.Email, user.Role.ToString());
    }
}
