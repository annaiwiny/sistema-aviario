using FarmSystemProject.DTOs.Users;

namespace FarmSystemProject.Interfaces;

public interface IUserService
{
    Task<UserResponse> Create(CreateUserRequest request);
    Task<UserProfileResponse> Read(int id);
    Task<UserResponse?> Update(int userId, UpdateUserRequest request);
}