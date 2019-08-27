using System.Threading.Tasks;
using EntityFramework.Study.Domain;
using EntityFramework.Study.Domain.Common;
using EntityFramework.Study.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFramework.Study.Test
{
    public class InMemoryDbContextTest
    {
        [Fact]
        public async Task Test()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase("database")
                .Options;

            using (var db = new ApplicationDb(options))
            {
                db.Database.EnsureCreated();

                db.Users.Add(new User
                {
                    Email = "test@test.com",
                    DisplayName = "Jinseoung Lee"
                });
                await db.SaveChangesAsync();

                var user = await db.Users.FirstAsync();

                user.CreatedAt.Should().NotBe(default);
                user.UpdatedAt.Should().NotBe(default);
                user.Status.Should().Be(Status.Created);

                user.Email = "other@something.com";
                await Task.Delay(500);
                await db.SaveChangesAsync();

                user.UpdatedAt.Should().NotBe(user.CreatedAt);
                user.Status.Should().Be(Status.Updated);

                db.Users.Remove(user);
                await db.SaveChangesAsync();

                user.Status.Should().Be(Status.Deleted);
                var count = await db.Users.CountAsync();
                count.Should().Be(0);
            }
        }
    }
}