using Grpc.Core;
using Identity.Application.Interfaces.Grpc;
using Identity.GrpcContracts;
using Identity.Infrastructure.Grpc.Mappers;
using Proto = Identity.GrpcContracts;

namespace Identity.Infrastructure.Grpc.Services
{
    public class UserGrpcService : Proto.IdentityUserService.IdentityUserServiceBase
    {
        private readonly IUserGrpcService _usergrpcService;

        public UserGrpcService(IUserGrpcService usergrpcService)
        {
            _usergrpcService = usergrpcService;
        }

        public override async Task<UserResponse> GetUserById(UserRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid userId"));

            var dto = await _usergrpcService.GetUserByIdAsync(id);
            if (dto == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

            return dto.ToProto();
        }

        public override async Task<UsersResponse> GetUsersByIds(UsersRequest request, ServerCallContext context)
        {
            var ids = request.UserIds.Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
            var dtos = await _usergrpcService.GetUsersByIdsAsync(ids);

            var resp = new UsersResponse();
            resp.Users.AddRange(dtos.Select(d => d.ToProto()));
            return resp;
        }
    }
}
