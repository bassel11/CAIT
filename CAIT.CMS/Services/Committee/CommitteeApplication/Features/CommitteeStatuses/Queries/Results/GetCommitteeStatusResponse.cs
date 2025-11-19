namespace CommitteeApplication.Features.CommitteeStatuses.Queries.Results
{
    public class GetCommitteeStatusResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
