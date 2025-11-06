using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.Register;

[TestFixture]
public class RegisterHandlerTests : TestBase
{
    private RegisterHandler _handler = null!;
    private IPasswordHashService _passwordHashService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new RegisterHandler(_passwordHashService, _roleRepository, _unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RegisterCommand
        {
            Name = TestDataHelper.GenerateRandomName(),
            Email = TestDataHelper.GenerateRandomEmail(),
            Password = TestDataHelper.GenerateValidPassword(),
            HasAcceptedTermsOfUse = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        
        var userExists = await _userRepository.ExistsByEmailAsync(command.Email, CancellationToken.None);
        userExists.Should().BeTrue();
    }

    [Test]
    public void Handle_WithoutAcceptingTerms_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = TestDataHelper.GenerateRandomName(),
            Email = TestDataHelper.GenerateRandomEmail(),
            Password = TestDataHelper.GenerateValidPassword(),
            HasAcceptedTermsOfUse = false
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WithExistingEmail_ShouldThrowConflictException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var existingUser = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(existingUser, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RegisterCommand
        {
            Name = TestDataHelper.GenerateRandomName(),
            Email = email,
            Password = TestDataHelper.GenerateValidPassword(),
            HasAcceptedTermsOfUse = true
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.ConflictException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_WithoutDefaultRole_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = TestDataHelper.GenerateRandomName(),
            Email = TestDataHelper.GenerateRandomEmail(),
            Password = TestDataHelper.GenerateValidPassword(),
            HasAcceptedTermsOfUse = true
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}
