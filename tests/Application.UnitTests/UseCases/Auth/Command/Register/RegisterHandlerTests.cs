using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Auth.Command.Register;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
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
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

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
    public async Task Handle_WithoutAcceptingTerms_ShouldThrowBadRequestException()
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
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Test]
    public async Task Handle_WithExistingEmail_ShouldThrowConflictException()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var existingUser = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
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
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Test]
    public async Task Handle_WithoutDefaultRole_ShouldThrowNotFoundException()
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
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
