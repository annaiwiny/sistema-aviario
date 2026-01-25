using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Users;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponse> Create(CreateUserRequest request)
    {
        if (request.BirthDate > DateTime.Today)
            throw new BusinessException("Data de nascimento não pode ser no futuro");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new BusinessException("E-mail já cadastrado");

        if (await _context.Users.AnyAsync(u => u.Cpf == request.Cpf))
            throw new BusinessException("CPF já cadastrado");

        var user = new User
        {
            Name = request.Name,
            BirthDate = request.BirthDate,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password), // Hash da senha com Bcrypt
            Cpf = request.Cpf,
            Type = UserType.Farmer,
            State = request.State,
            City = request.City,
            Address = request.Address,
            Phone = request.Phone
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Type = user.Type.ToString()
        };
    }

    public async Task<UserResponse> GetById(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Type = u.Type.ToString()
            })
            .FirstOrDefaultAsync();

        return user ?? throw new NotFoundException("Usuário não encontrado");
    }

}
