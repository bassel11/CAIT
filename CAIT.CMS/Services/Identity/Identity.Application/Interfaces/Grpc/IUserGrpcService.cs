using Identity.Application.DTOs.Users;

namespace Identity.Application.Interfaces.Grpc
{
    public interface IUserGrpcService
    {
        Task<UserGrpcDto?> GetUserByIdAsync(Guid userId);
        Task<List<UserGrpcDto>> GetUsersByIdsAsync(List<Guid> userIds);
    }
}
