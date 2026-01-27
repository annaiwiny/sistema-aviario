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

    public async Task<UserProfileResponse> Read(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserProfileResponse
            {
                Id = u.Id,
                Name = u.Name,
                BirthDate = u.BirthDate,
                Email = u.Email,
                Cpf = u.Cpf,
                Type = u.Type.ToString(),
                State = u.State,
                City = u.City,
                Address = u.Address,
                Phone = u.Phone
            })
            .FirstOrDefaultAsync();

        return user ?? throw new NotFoundException("Usuário não encontrado");
    }

    public async Task<UserResponse> Update(int userId, UpdateUserRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("Usuário não encontrado");

        if (request.Name != null)
            user.Name = request.Name;

        if (request.BirthDate.HasValue)
        {
            if (request.BirthDate.Value > DateTime.Today)
                throw new BusinessException("Data de nascimento não pode ser no futuro");

            user.BirthDate = request.BirthDate.Value;
        }

        if (request.Email != null)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId))
                throw new BusinessException("E-mail já cadastrado por outro usuário");

            user.Email = request.Email;
        }

        if (request.Cpf != null)
        {
            if (await _context.Users.AnyAsync(u => u.Cpf == request.Cpf && u.Id != userId))
                throw new BusinessException("CPF já cadastrado por outro usuário");

            user.Cpf = request.Cpf;
        }

        if (request.State != null)
            user.State = request.State;

        if (request.City != null)
            user.City = request.City;

        if (request.Address != null)
            user.Address = request.Address;

        if (request.Phone != null)
            user.Phone = request.Phone;

        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Type = user.Type.ToString()
        };
    }

}
