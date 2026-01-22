using MeetingCore.Enums.MeetingEnums;

namespace MeetingCore.ValueObjects.MeetingVO
{
    public class MeetingLocation
    {
        public LocationType Type { get; private set; }
        public string? RoomName { get; private set; }
        public string? Address { get; private set; }
        public string? OnlineUrl { get; private set; }

        // EF Core Constructor
        private MeetingLocation() { }

        public static MeetingLocation Create(LocationType type, string? room, string? address, string? url)
        {
            if (type == LocationType.Physical && string.IsNullOrWhiteSpace(room) && string.IsNullOrWhiteSpace(address))
                throw new DomainException("Physical meetings require a Room or Address.");

            return new MeetingLocation
            {
                Type = type,
                RoomName = room,
                Address = address,
                OnlineUrl = url
            };
        }

        //protected override IEnumerable<object> GetEqualityComponents()
        //{
        //    yield return Type;
        //    yield return RoomName ?? string.Empty;
        //    yield return OnlineUrl ?? string.Empty;
        //}
    }
}
