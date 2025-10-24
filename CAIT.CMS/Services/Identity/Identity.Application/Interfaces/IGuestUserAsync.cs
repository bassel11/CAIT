namespace Identity.Application.Interfaces
{
    public interface IGuestUserAsync
    {
        Task<bool> IsGuestUserAsync(string token);
    }
}
