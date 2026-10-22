using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public class UserTests
{
    private static Email CreateEmail()
        => Email.Create("john.doe@example.com");

    private static User CreateUser(
        string firstName = "John",
        string lastName = "Doe",
        string password = "Password123")
    {
        return User.Create(
            firstName,
            lastName,
            password,
            CreateEmail());
    }

    // =========================
    // Create
    // =========================

    [Fact]
    public void Create_ShouldCreateUser_WhenValidDataIsProvided()
    {
        var user = CreateUser();

        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Password123", user.Password);
        Assert.Equal("john.doe@example.com", user.Email.Value);
        Assert.Equal(PlatformRole.NormalUser, user.PlatformRole);
        Assert.Null(user.DeletedAt);
    }

    [Fact]
    public void Create_ShouldTrimFirstName_WhenFirstNameContainsWhitespace()
    {
        var user = CreateUser(firstName: "  John  ");

        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public void Create_ShouldTrimLastName_WhenLastNameContainsWhitespace()
    {
        var user = CreateUser(lastName: "  Doe  ");

        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void Create_ShouldThrow_WhenFirstNameIsEmpty()
    {
        var act = () => CreateUser(firstName: "");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal("First name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenFirstNameIsTooShort()
    {
        var act = () => CreateUser(firstName: "Jo");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "First name must be at least 3 characters long.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenLastNameIsEmpty()
    {
        var act = () => CreateUser(lastName: "");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal("Last name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenLastNameIsTooShort()
    {
        var act = () => CreateUser(lastName: "Do");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Last name must be at least 3 characters long.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenPasswordIsEmpty()
    {
        var act = () => CreateUser(password: "");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal("Password cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenPasswordIsTooShort()
    {
        var act = () => CreateUser(password: "1234567");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Password must be at least 8 characters long.",
            exception.Message);
    }

    // =========================
    // Create Platform Admin
    // =========================

    [Fact]
    public void CreatePlatformAdmin_ShouldCreatePlatformAdmin_WhenValidDataIsProvided()
    {
        var user = User.CreatePlatformAdmin(
            "John",
            "Doe",
            "Password123",
            CreateEmail());

        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Password123", user.Password);
        Assert.Equal(PlatformRole.PlatformAdmin, user.PlatformRole);
        Assert.Null(user.DeletedAt);
    }

    [Fact]
    public void CreatePlatformAdmin_ShouldThrow_WhenFirstNameIsInvalid()
    {
        var act = () => User.CreatePlatformAdmin(
            "Jo",
            "Doe",
            "Password123",
            CreateEmail());

        Assert.Throws<DomainRuleViolationException>(act);
    }

    // =========================
    // FullName
    // =========================

    [Fact]
    public void FullName_ShouldReturnFirstNameAndLastName()
    {
        var user = CreateUser(
            firstName: "John",
            lastName: "Doe");

        Assert.Equal("John Doe", user.FullName);
    }

    // =========================
    // Update First Name
    // =========================

    [Fact]
    public void UpdateFirstName_ShouldChangeFirstName_WhenValid()
    {
        var user = CreateUser();

        user.UpdateFirstName("Michael");

        Assert.Equal("Michael", user.FirstName);
    }

    [Fact]
    public void UpdateFirstName_ShouldTrimValue()
    {
        var user = CreateUser();

        user.UpdateFirstName("  Michael  ");

        Assert.Equal("Michael", user.FirstName);
    }

    [Fact]
    public void UpdateFirstName_ShouldThrow_WhenNewFirstNameIsInvalid()
    {
        var user = CreateUser();

        var act = () => user.UpdateFirstName("Jo");

        Assert.Throws<DomainRuleViolationException>(act);
    }

    [Fact]
    public void UpdateFirstName_ShouldNotChangeValue_WhenSameNameIsProvided()
    {
        var user = CreateUser();

        user.UpdateFirstName("John");

        Assert.Equal("John", user.FirstName);
    }

    // =========================
    // Update Last Name
    // =========================

    [Fact]
    public void UpdateLastName_ShouldChangeLastName_WhenValid()
    {
        var user = CreateUser();

        user.UpdateLastName("Smith");

        Assert.Equal("Smith", user.LastName);
    }

    [Fact]
    public void UpdateLastName_ShouldThrow_WhenNewLastNameIsInvalid()
    {
        var user = CreateUser();

        var act = () => user.UpdateLastName("Sm");

        Assert.Throws<DomainRuleViolationException>(act);
    }

    // =========================
    // Update Password
    // =========================

    [Fact]
    public void UpdatePassword_ShouldChangePassword_WhenValid()
    {
        var user = CreateUser();

        user.UpdatePassword("NewPassword123");

        Assert.Equal("NewPassword123", user.Password);
    }

    [Fact]
    public void UpdatePassword_ShouldThrow_WhenPasswordIsTooShort()
    {
        var user = CreateUser();

        var act = () => user.UpdatePassword("1234567");

        Assert.Throws<DomainRuleViolationException>(act);
    }

    [Fact]
    public void UpdatePassword_ShouldNotChangeValue_WhenSamePasswordIsProvided()
    {
        var user = CreateUser();

        user.UpdatePassword("Password123");

        Assert.Equal("Password123", user.Password);
    }

    // =========================
    // Soft Delete
    // =========================

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt_WhenUserIsNotDeleted()
    {
        var user = CreateUser();

        user.SoftDelete();

        Assert.NotNull(user.DeletedAt);
    }

    [Fact]
    public void SoftDelete_ShouldBeIdempotent_WhenUserIsAlreadyDeleted()
    {
        var user = CreateUser();

        user.SoftDelete();
        var deletedAt = user.DeletedAt;

        user.SoftDelete();

        Assert.Equal(deletedAt, user.DeletedAt);
    }

    // =========================
    // Deleted User Protection
    // =========================

    [Fact]
    public void UpdateFirstName_ShouldThrow_WhenUserIsDeleted()
    {
        var user = CreateUser();
        user.SoftDelete();

        var act = () => user.UpdateFirstName("Michael");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Cannot update first name for deleted user.",
            exception.Message);
    }

    [Fact]
    public void UpdateLastName_ShouldThrow_WhenUserIsDeleted()
    {
        var user = CreateUser();
        user.SoftDelete();

        var act = () => user.UpdateLastName("Smith");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Cannot update last name for deleted user.",
            exception.Message);
    }

    [Fact]
    public void UpdatePassword_ShouldThrow_WhenUserIsDeleted()
    {
        var user = CreateUser();
        user.SoftDelete();

        var act = () => user.UpdatePassword("NewPassword123");

        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Cannot update password for deleted user.",
            exception.Message);
    }
}