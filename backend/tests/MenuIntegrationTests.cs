using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Medicare.Application.DTOs;
using Xunit;

namespace Medicare.Tests
{
    public class MenuIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MenuIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMenuRepository>();
                    services.PostConfigure<SessionOptions>(options =>
                    {
                        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    });

                    // Fake user repository (admin)
                    services.RemoveAll<IUserRepository>();
                    services.AddSingleton<IUserRepository>(new FakeUserRepository(new Domain.Entities.User
                    {
                        Id = 1,
                        Email = "admin@medicare.local",
                        FullName = "System Administrator",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        Role = UserRole.Admin,
                        IsActive = true,
                    }));

                    services.AddSingleton<IMenuRepository>(new FakeMenuRepository());
                });
            });
        }

        [Fact]
        public async Task Create_And_Get_Menu_Works_For_Admin()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                AllowAutoRedirect = false,
            });

            // Login first to obtain session cookie
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { email = "admin@medicare.local", password = "Admin123!" });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Create menu
            var createRequest = new CreateMenuRequest
            {
                DateApplied = DateTime.UtcNow.Date,
                MealType = MealType.Lunch,
                Description = "Thực đơn kiểm thử",
                Items = new System.Collections.Generic.List<MenuItemRequest>
                {
                    new MenuItemRequest { Name = "Cơm thử", Description = "Mô tả", Price = 10000m, DisplayOrder = 1 }
                }
            };

            var createResponse = await client.PostAsJsonAsync("/api/menus", createRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var payload = await createResponse.Content.ReadFromJsonAsync<MenuDto>();
            payload.Should().NotBeNull();
            payload!.Description.Should().Be(createRequest.Description);

            // Get menus (paged)
            var getResponse = await client.GetAsync("/api/menus?page=1&pageSize=10");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var page = await getResponse.Content.ReadFromJsonAsync<PagedResult<MenuDto>>();
            page.Should().NotBeNull();
            page!.Items.Should().ContainSingle(m => m.Id == payload.Id);
        }

        // Minimal fake in-memory repository for tests
        
        private class FakeMenuRepository : IMenuRepository
        {
            private readonly ConcurrentDictionary<int, Menu> _store = new();
            private int _seq = 1;

            public Task<Menu> AddAsync(Menu menu, CancellationToken cancellationToken = default)
            {
                var id = System.Threading.Interlocked.Increment(ref _seq);
                menu.Id = id;
                menu.CreatedAt = DateTime.UtcNow;
                _store[id] = menu;
                return Task.FromResult(menu);
            }

            public Task DeleteAsync(Menu menu, CancellationToken cancellationToken = default)
            {
                _store.TryRemove(menu.Id, out _);
                return Task.CompletedTask;
            }

            public Task<bool> ExistsAsync(DateTime date, MealType mealType, int? excludeMenuId = null, CancellationToken cancellationToken = default)
            {
                var exists = _store.Values.Any(m => m.DateApplied.Date == date.Date && m.MealType == mealType && (!excludeMenuId.HasValue || m.Id != excludeMenuId.Value));
                return Task.FromResult(exists);
            }

            public Task<Menu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            {
                _store.TryGetValue(id, out var menu);
                return Task.FromResult(menu);
            }

            public Task<PagedResult<Menu>> GetPagedAsync(MenuFilterRequest filter, CancellationToken cancellationToken = default)
            {
                var items = _store.Values.OrderBy(m => m.DateApplied).Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
                var result = new PagedResult<Menu>
                {
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalCount = _store.Count,
                    Items = items
                };
                return Task.FromResult(result);
            }

            public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task UpdateAsync(Menu menu, CancellationToken cancellationToken = default)
            {
                if (_store.ContainsKey(menu.Id))
                {
                    menu.UpdatedAt = DateTime.UtcNow;
                    _store[menu.Id] = menu;
                }
                return Task.CompletedTask;
            }
        }

        private class FakeUserRepository : IUserRepository
        {
            private readonly User _user;
            public FakeUserRepository(User user) => _user = user;
            public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult(_user.Id == id ? _user : null);
            public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult(string.Equals(_user.Email, email, StringComparison.OrdinalIgnoreCase) ? _user : null);
            public Task AddAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        }
    }
}
