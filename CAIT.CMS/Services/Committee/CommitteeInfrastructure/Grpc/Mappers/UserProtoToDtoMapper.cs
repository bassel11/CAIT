using CommitteeApplication.Grpc.Models;
using Identity.GrpcContracts;

namespace CommitteeInfrastructure.Grpc.Mappers
{
    public static class UserProtoToDtoMapper
    {
        public static UserGrpcDto ToModel(this UserResponse proto)
        {
            return new UserGrpcDto
            {
                UserId = Guid.TryParse(proto.UserId, out var g) ? g : Guid.Empty,
                FirstName = proto.FirstName,
                LastName = proto.LastName,
                Email = proto.Email,
                Roles = proto.Roles.ToList()
            };
        }
    }
}
