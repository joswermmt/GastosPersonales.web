using GastosPersonales.Application.DTOs.Profile;

namespace GastosPersonales.Application.Services;

public interface IUserService
{
    Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}

