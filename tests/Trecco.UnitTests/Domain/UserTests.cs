using Trecco.Application.Domain.Users;

namespace Trecco.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateUser_WithValidName()
    {
        // Arrange
        string name = "John Doe";

        // Act
        var user = new User(name);

        // Assert
        Assert.Equal(name, user.Name);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new User(""));
        Assert.Throws<ArgumentException>(() => new User("   "));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenNameIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new User(null!));
    }
}
