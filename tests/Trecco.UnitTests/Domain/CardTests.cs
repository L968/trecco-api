using Trecco.Application.Domain.Cards;

namespace Trecco.UnitTests.Domain;

public class CardTests
{
    [Fact]
    public void Should_CreateCard_WithValidValues()
    {
        // Arrange
        string title = "Test Card";
        string description = "Test Description";

        // Act
        var card = new Card(title, description);

        // Assert
        Assert.Equal(title, card.Title);
        Assert.Equal(description, card.Description);
        Assert.Equal(0, card.Position);
        Assert.NotEqual(Guid.Empty, card.Id);
    }

    [Fact]
    public void Should_CreateCard_WithPosition()
    {
        // Arrange
        string title = "Test Card";
        string description = "Test Description";
        int position = 5;

        // Act
        var card = new Card(title, description, position);

        // Assert
        Assert.Equal(title, card.Title);
        Assert.Equal(description, card.Description);
        Assert.Equal(position, card.Position);
    }

    [Fact]
    public void SetPosition_ShouldUpdatePosition()
    {
        // Arrange
        var card = new Card("Test", "Desc", 0);

        // Act
        card.SetPosition(5);

        // Assert
        Assert.Equal(5, card.Position);
    }

    [Fact]
    public void UpdateTitle_ShouldChangeTitle_WhenValidTitle()
    {
        // Arrange
        var card = new Card("Old Title", "Description");

        // Act
        card.UpdateTitle("New Title");

        // Assert
        Assert.Equal("New Title", card.Title);
    }

    [Fact]
    public void UpdateDescription_ShouldChangeDescription()
    {
        // Arrange
        var card = new Card("Title", "Old Description");

        // Act
        card.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", card.Description);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Card("", "Description"));
        Assert.Throws<ArgumentException>(() => new Card("   ", "Description"));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenTitleIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Card(null!, "Description"));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var card = new Card("Valid Title", "Description");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => card.UpdateTitle(""));
        Assert.Throws<ArgumentException>(() => card.UpdateTitle("   "));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowArgumentException_WhenTitleIsNull()
    {
        // Arrange
        var card = new Card("Valid Title", "Description");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => card.UpdateTitle(null!));
    }

    [Fact]
    public void Constructor_ShouldSetCreatedByToCurrentTime()
    {
        // Arrange
        DateTime beforeCreation = DateTime.UtcNow;

        // Act
        var card = new Card("Title", "Description");

        // Assert
        Assert.True(card.CreatedBy >= beforeCreation);
        Assert.True(card.CreatedBy <= DateTime.UtcNow);
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