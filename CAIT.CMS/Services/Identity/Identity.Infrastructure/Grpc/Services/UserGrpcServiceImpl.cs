using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces.Grpc;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Grpc.Services
{
    public class UserGrpcServiceImpl : IUserGrpcService
    {
        private readonly ApplicationDbContext _context;

        public UserGrpcServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserGrpcDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new UserGrpcDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.UserRoles.Select(r => r.Role.Name!).ToList()
            };
        }

        public async Task<List<UserGrpcDto>> GetUsersByIdsAsync(List<Guid> userIds)
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            return users.Select(user => new UserGrpcDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.UserRoles.Select(r => r.Role.Name!).ToList()
            }).ToList();
        }
    }
}
