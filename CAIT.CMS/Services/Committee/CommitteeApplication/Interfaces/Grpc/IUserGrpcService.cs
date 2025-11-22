using CommitteeApplication.Grpc.Models;

namespace CommitteeApplication.Interfaces.Grpc
{
    public interface IUserGrpcService
    {
        Task<UserGrpcDto?> GetUserByIdAsync(Guid id);
        Task<List<UserGrpcDto>> GetUsersByIdsAsync(List<Guid> ids);
    }
}
