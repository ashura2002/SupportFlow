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
        string password = "Password123",
        Roles role = Roles.Requester)
    {
        return User.Create(
            firstName,
            lastName,
            password,
            CreateEmail(),
            role);
    }

    // =========================
    // Create
    // =========================

    [Fact]
    public void Create_ShouldCreateUser_WhenValidDataIsProvided()
    {
        // Act
        var user = CreateUser();

        // Assert
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Password123", user.Password);
        Assert.Equal("john.doe@example.com", user.Email.Value);
        Assert.Equal(Roles.Requester, user.Role);
        Assert.Null(user.DeletedAt);
    }

    [Fact]
    public void Create_ShouldTrimFirstName_WhenFirstNameContainsWhitespace()
    {
        // Act
        var user = CreateUser(firstName: "  John  ");

        // Assert
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public void Create_ShouldTrimLastName_WhenLastNameContainsWhitespace()
    {
        // Act
        var user = CreateUser(lastName: "  Doe  ");

        // Assert
        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void Create_ShouldThrow_WhenFirstNameIsEmpty()
    {
        // Act
        var act = () => CreateUser(firstName: "");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "First name cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenFirstNameIsTooShort()
    {
        // Act
        var act = () => CreateUser(firstName: "Jo");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "First name must be at least 3 characters long.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenLastNameIsEmpty()
    {
        // Act
        var act = () => CreateUser(lastName: "");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal("Last name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenLastNameIsTooShort()
    {
        // Act
        var act = () => CreateUser(lastName: "Do");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Last name must be at least 3 characters long.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenPasswordIsEmpty()
    {
        // Act
        var act = () => CreateUser(password: "");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Password cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenPasswordIsTooShort()
    {
        // Act
        var act = () => CreateUser(password: "1234567");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Password must be at least 8 characters long.",
            exception.Message);
    }

    // =========================
    // FullName
    // =========================

    [Fact]
    public void FullName_ShouldReturnFirstNameAndLastName()
    {
        // Arrange
        var user = CreateUser(
            firstName: "John",
            lastName: "Doe");

        // Act
        var fullName = user.FullName;

        // Assert
        Assert.Equal("John Doe", fullName);
    }

    // =========================
    // Update First Name
    // =========================

    [Fact]
    public void UpdateFirstName_ShouldChangeFirstName_WhenValid()
    {
        // Arrange
        var user = CreateUser();

        // Act
        user.UpdateFirstName("Michael");

        // Assert
        Assert.Equal("Michael", user.FirstName);
    }

    [Fact]
    public void UpdateFirstName_ShouldTrimValue()
    {
        // Arrange
        var user = CreateUser();

        // Act
        user.UpdateFirstName("  Michael  ");

        // Assert
        Assert.Equal("Michael", user.FirstName);
    }

    [Fact]
    public void UpdateFirstName_ShouldThrow_WhenNewFirstNameIsInvalid()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var act = () => user.UpdateFirstName("Jo");

        // Assert
        Assert.Throws<DomainRuleViolationException>(act);
    }

    [Fact]
    public void UpdateFirstName_ShouldNotChangeValue_WhenSameNameIsProvided()
    {
        // Arrange
        var user = CreateUser();

        // Act
        user.UpdateFirstName("John");

        // Assert
        Assert.Equal("John", user.FirstName);
    }

    // =========================
    // Update Last Name
    // =========================

    [Fact]
    public void UpdateLastName_ShouldChangeLastName_WhenValid()
    {
        // Arrange
        var user = CreateUser();

        // Act
        user.UpdateLastName("Smith");

        // Assert
        Assert.Equal("Smith", user.LastName);
    }

    [Fact]
    public void UpdateLastName_ShouldThrow_WhenNewLastNameIsInvalid()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var act = () => user.UpdateLastName("Sm");

        // Assert
        Assert.Throws<DomainRuleViolationException>(act);
    }

    // =========================
    // Update Password
    // =========================

    [Fact]
    public void UpdatePassword_ShouldChangePassword_WhenValid()
    {
        // Arrange
        var user = CreateUser();

        // Act
        user.UpdatePassword("NewPassword123");

        // Assert
        Assert.Equal("NewPassword123", user.Password);
    }

    [Fact]
    public void UpdatePassword_ShouldThrow_WhenPasswordIsTooShort()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var act = () => user.UpdatePassword("1234567");

        // Assert
        Assert.Throws<DomainRuleViolationException>(act);
    }

    [Fact]
    public void UpdatePassword_ShouldNotChangeValue_WhenSamePasswordIsProvided()
    {
        // Arrange
        var user = CreateUser();

        // Act
        user.UpdatePassword("Password123");

        // Assert
        Assert.Equal("Password123", user.Password);
    }

    // =========================
    // Soft Delete
    // =========================

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt_WhenUserIsNotAdministrator()
    {
        // Arrange
        var user = CreateUser(role: Roles.Requester);

        // Act
        user.SoftDelete();

        // Assert
        Assert.NotNull(user.DeletedAt);
    }

    [Fact]
    public void SoftDelete_ShouldThrow_WhenUserIsAdministrator()
    {
        // Arrange
        var user = CreateUser(role: Roles.Administrator);

        // Act
        var act = () => user.SoftDelete();

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Admin account cannot be deleted.",
            exception.Message);
    }

    [Fact]
    public void SoftDelete_ShouldBeIdempotent_WhenUserIsAlreadyDeleted()
    {
        // Arrange
        var user = CreateUser();

        user.SoftDelete();
        var deletedAt = user.DeletedAt;

        // Act
        user.SoftDelete();

        // Assert
        Assert.Equal(deletedAt, user.DeletedAt);
    }

    // =========================
    // Deleted User Protection
    // =========================

    [Fact]
    public void UpdateFirstName_ShouldThrow_WhenUserIsDeleted()
    {
        // Arrange
        var user = CreateUser();
        user.SoftDelete();

        // Act
        var act = () => user.UpdateFirstName("Michael");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Cannot update first name for deleted user.",
            exception.Message);
    }

    [Fact]
    public void UpdateLastName_ShouldThrow_WhenUserIsDeleted()
    {
        // Arrange
        var user = CreateUser();
        user.SoftDelete();

        // Act
        var act = () => user.UpdateLastName("Smith");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Cannot update last name for deleted user.",
            exception.Message);
    }

    [Fact]
    public void UpdatePassword_ShouldThrow_WhenUserIsDeleted()
    {
        // Arrange
        var user = CreateUser();
        user.SoftDelete();

        // Act
        var act = () => user.UpdatePassword("NewPassword123");

        // Assert
        var exception = Assert.Throws<DomainRuleViolationException>(act);

        Assert.Equal(
            "Cannot update password for deleted user.",
            exception.Message);
    }
}