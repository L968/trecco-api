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
}
