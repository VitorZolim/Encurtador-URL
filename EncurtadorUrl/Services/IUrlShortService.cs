namespace EncurtadorUrl.Services
{
    public interface IUrlShortService
    {
        Task<string> CreateShortUrlAsync(string originalUrl);
        string Encode(int id);
        Task<string?> GetOriginalUrlAsync(string shortCode);
    }
}
