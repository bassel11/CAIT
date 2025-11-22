using Identity.Application.DTOs.Users;
using Identity.GrpcContracts;

namespace Identity.Infrastructure.Grpc.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToProto(this UserGrpcDto dto)
        {
            var p = new UserResponse
            {
                UserId = dto.UserId.ToString(),
                FirstName = dto.FirstName ?? "",
                LastName = dto.LastName ?? "",
                Email = dto.Email ?? ""
            };
            p.Roles.AddRange(dto.Roles);
            return p;
        }

        public static UserGrpcDto ToDto(this UserResponse proto)
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
