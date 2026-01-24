using FarmSystemProject.DTOs.Users;

namespace FarmSystemProject.Interfaces;

public interface IUserService
{
    Task<UserResponse> Create(CreateUserRequest request);
    Task<UserResponse?> GetById(int id);
}
