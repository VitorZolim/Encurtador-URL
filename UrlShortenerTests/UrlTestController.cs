using EncurtadorUrl.Context;
using EncurtadorUrl.Controllers;
using EncurtadorUrl.Models;
using EncurtadorUrl.Models.DTOs;
using EncurtadorUrl.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UrlShortenerTests
{
    public class UrlTestController
    {
        public static DbContextOptions<AppDbContext> dbContextOptions { get; }
        public static string connectionString = "DataSource=BancoDeTeste_API.db";
        static UrlTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                                .UseSqlite(connectionString).Options;
        }
        [Fact]
        public async Task Post_ShortenUrl_DeveRetornarOkComCodigo_QuandoUrlValida()
        {
            // Arrange
            using (AppDbContext context = new AppDbContext(dbContextOptions))
            {
                context.Database.EnsureCreated(); // Cria o SQLite físico para o teste

                UrlShortService service = new UrlShortService(context);
                UrlController controller = new UrlController(service);

                UrlRequest request = new UrlRequest
                {
                    Url = "https://github.com/VitorZolim"
                };

                // Act
                IActionResult result = await controller.ShortenUrl(request);

                // Assert
                OkObjectResult okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                okResult.Value.Should().NotBeNull();

                // Limpa o banco após o teste
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Get_RedirectToOriginal_DeveRetornarRedirect302_QuandoCodigoExistir()
        {
            // Arrange
            using (AppDbContext context = new AppDbContext(dbContextOptions))
            {
                context.Database.EnsureCreated();

                UrlShortService service = new UrlShortService(context);
                UrlController controller = new UrlController(service);

                string originalUrl = "https://github.com/VitorZolim";

                // Inserimos um registro diretamente para preparar o estado do banco
                ShortenedUrl novaUrl = new ShortenedUrl
                {
                    OriginalUrl = originalUrl,
                    ShortCode = "1a", // Simulando um código já convertido em Base62
                    CreatedAt = DateTime.UtcNow
                };

                context.Urls.Add(novaUrl);
                await context.SaveChangesAsync();

                // Act
                IActionResult result = await controller.RedirectUrlOriginal("1a");

                // Assert
                RedirectResult redirectResult = result.Should().BeOfType<RedirectResult>().Subject;
                redirectResult.Url.Should().Be(originalUrl);

                context.Database.EnsureDeleted();
            }
        }
    }
}
