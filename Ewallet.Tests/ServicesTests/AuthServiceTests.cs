using Ewallet.Models;
using Ewallet.Services.Implementations;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ewallet.Tests.ServicesTests
{  
    public class AuthServiceTests : IClassFixture<TestFixture<Startup>>
    {
        private IAuthService Service { get; }

        // Arrange

        private const string Email = "tyty";
        private const string Password = "@Merlino07";
        private const bool RememberMe = true;

        public AuthServiceTests(TestFixture<Startup> fixture)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = "abideklove@gmail.com",
                UserName = "minato"
            };

            var fakeUserManager = new Mock<FakeUserManager>();

            fakeUserManager.Setup(x => x.FindByEmailAsync(Email)).ReturnsAsync(user);
            fakeUserManager.Setup(x => x.CheckPasswordAsync(user, Password)).ReturnsAsync(true);

            var fakeSignInManager = new Mock<FakeSignInManager>();

            fakeSignInManager.Setup(
                    x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var jwtService = new Mock<IJWTService>();

            //SERVICES CONFIGURATIONS
            Service = new AuthService(fakeUserManager.Object, fakeSignInManager.Object, jwtService.Object);

        }

        [Fact]
        public async Task ShouldLogin()
        {

            // Act

            var login = await Service.Login("tyty", Password, RememberMe);

            // Assert

            Assert.True(login.Status);
        }

        [Theory]
        [InlineData("email", Password, RememberMe)]
        [InlineData(Email, "", RememberMe)]
        public async Task ShouldLoginFail(string email, string password, bool rememberMe)
        {
            // Act

            var login = await Service.Login(email, password, rememberMe);

            // Assert

            Assert.False(login.Status);
        }

    }
    
}
