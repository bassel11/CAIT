using CommitteeApplication.Grpc.Models;
using CommitteeApplication.Interfaces.Grpc;
using CommitteeInfrastructure.Grpc.Mappers;
using Grpc.Core;
using Identity.GrpcContracts;

namespace CommitteeInfrastructure.Grpc.Clients
{
    public class UserGrpcService : IUserGrpcService
    {
        private readonly Identity.GrpcContracts.IdentityUserService.IdentityUserServiceClient _client;

        public UserGrpcService(Identity.GrpcContracts.IdentityUserService.IdentityUserServiceClient client)
        {
            _client = client;
        }

        public async Task<UserGrpcDto?> GetUserByIdAsync(Guid id)
        {
            try
            {
                var res = await _client.GetUserByIdAsync(new UserRequest { UserId = id.ToString() });
                return res.ToModel();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<UserGrpcDto>> GetUsersByIdsAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0) return new List<UserGrpcDto>();

            var req = new UsersRequest();
            req.UserIds.AddRange(ids.Select(i => i.ToString()));
            var res = await _client.GetUsersByIdsAsync(req);
            return res.Users.Select(u => u.ToModel()).ToList();
        }
    }
}
