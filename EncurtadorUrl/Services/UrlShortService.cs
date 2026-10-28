using EncurtadorUrl.Context;
using EncurtadorUrl.Models;
using Microsoft.EntityFrameworkCore;

namespace EncurtadorUrl.Services
{
    public class UrlShortService : IUrlShortService
    {
        private readonly AppDbContext _context;
        private const string _alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly int _base = 62;

        public UrlShortService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateShortUrlAsync(string originalUrl)
        {
            ShortenedUrl shortenedUrl = new ShortenedUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            //Salva no banco para gerar o ID e utilizar na criação da url curta
            _context.Urls.Add(shortenedUrl);
            await _context.SaveChangesAsync();

            string shortCode = Encode(shortenedUrl.Id);

            shortenedUrl.ShortCode = shortCode;
            _context.Urls.Update(shortenedUrl);
            await _context.SaveChangesAsync();

            return shortCode;
        }

        public string Encode(int id)
        {
            if (id == 0)
            {
                return _alphabet[0].ToString();
            }

            string result = string.Empty;
            int currentId = id;

            while (currentId > 0)
            {
                result = _alphabet[currentId % _base] + result;
                currentId /= _base;
            }

            return result;
        }
    }
}
