using Trecco.Application.Domain.Cards;

namespace Trecco.UnitTests.Domain;

public class CardTests
{
    [Fact]
    public void UpdateTitle_ShouldChangeTitle()
    {
        // Arrange
        var card = new Card("Old", "Desc");

        // Act
        card.UpdateTitle("New");

        // Assert
        Assert.Equal("New", card.Title);
    }

    [Fact]
    public void UpdateDescription_ShouldChangeDescription()
    {
        // Arrange
        var card = new Card("Title", "Old");

        // Act
        card.UpdateDescription("New");

        // Assert
        Assert.Equal("New", card.Description);
    }

    [Fact]
    public void SetPosition_ShouldChangePosition()
    {
        // Arrange
        var card = new Card("Title", "Desc", 0);

        // Act
        card.SetPosition(5);

        // Assert
        Assert.Equal(5, card.Position);
    }

    [Fact]
    public void Constructor_ShouldCreateCard_WithValidValues()
    {
        // Arrange
        string title = "Test Title";
        string description = "Test Description";
        int position = 2;

        // Act
        var card = new Card(title, description, position);

        // Assert
        Assert.Equal(title, card.Title);
        Assert.Equal(description, card.Description);
        Assert.Equal(position, card.Position);
        Assert.NotEqual(Guid.Empty, card.Id);
        Assert.True(card.CreatedBy <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Card("", "Description"));
        Assert.Throws<ArgumentException>(() => new Card("   ", "Description"));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var card = new Card("Title", "Description");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => card.UpdateTitle(""));
        Assert.Throws<ArgumentException>(() => card.UpdateTitle("   "));
    }

    [Fact]
    public void UpdateDescription_ShouldAcceptEmptyString()
    {
        // Arrange
        var card = new Card("Title", "Description");

        // Act
        card.UpdateDescription("");

        // Assert
        Assert.Equal("", card.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldAcceptNull()
    {
        // Arrange
        var card = new Card("Title", "Description");

        // Act
        card.UpdateDescription(null!);

        // Assert
        Assert.Null(card.Description);
    }
}
