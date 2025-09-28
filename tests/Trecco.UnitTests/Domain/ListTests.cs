using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;

namespace Trecco.UnitTests.Domain;

public class ListTests
{
    [Fact]
    public void AddCard_ShouldAddCardAtEnd()
    {
        // Arrange
        var list = new List("List", 0);

        // Act
        Card card = list.AddCard("Task", "Desc");

        // Assert
        Assert.Single(list.Cards);
        Assert.Equal(0, card.Position);
    }

    [Fact]
    public void RemoveCard_ShouldRemoveCardAndAdjustPositions()
    {
        // Arrange
        var list = new List("List", 0);
        Card card1 = list.AddCard("Task 1", "Desc");
        Card card2 = list.AddCard("Task 2", "Desc");
        Card card3 = list.AddCard("Task 3", "Desc");

        // Act
        list.RemoveCard(card2.Id);

        // Assert
        Assert.DoesNotContain(card2, list.Cards);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card3.Position);
    }

    [Fact]
    public void InsertCard_ShouldInsertAtPositionAndShiftOthers()
    {
        // Arrange
        var list = new List("List", 0);
        Card card1 = list.AddCard("Task 1", "Desc");
        Card card2 = list.AddCard("Task 2", "Desc");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, 1);

        // Assert
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, newCard.Position);
        Assert.Equal(2, card2.Position);
        Assert.Equal(newCard, list.Cards.ElementAt(1));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new List(null!, 0));
    }

    [Fact]
    public void UpdateName_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var list = new List("List", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => list.UpdateName(""));
        Assert.Throws<ArgumentException>(() => list.UpdateName("   "));
    }

    [Fact]
    public void AddCard_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var list = new List("List", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => list.AddCard("", "Description"));
        Assert.Throws<ArgumentException>(() => list.AddCard("   ", "Description"));
    }

    [Fact]
    public void InsertCard_ShouldThrowArgumentOutOfRangeException_WhenPositionIsNegative()
    {
        // Arrange
        var list = new List("List", 0);
        var newCard = new Card("Inserted", "Desc");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertCard(newCard, -1));
    }

    [Fact]
    public void InsertCard_ShouldThrowArgumentOutOfRangeException_WhenPositionIsGreaterThanListSize()
    {
        // Arrange
        var list = new List("List", 0);
        var newCard = new Card("Inserted", "Desc");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertCard(newCard, 10));
    }

    [Fact]
    public void RemoveCard_ShouldNotThrow_WhenCardDoesNotExist()
    {
        // Arrange
        var list = new List("List", 0);
        var nonExistentCardId = Guid.NewGuid();

        // Act & Assert
        Exception? exception = Record.Exception(() => list.RemoveCard(nonExistentCardId));
        Assert.Null(exception);
    }

    [Fact]
    public void AddCard_ShouldAddMultipleCardsWithCorrectPositions()
    {
        // Arrange
        var list = new List("List", 0);

        // Act
        Card card1 = list.AddCard("Task 1", "Desc");
        Card card2 = list.AddCard("Task 2", "Desc");
        Card card3 = list.AddCard("Task 3", "Desc");

        // Assert
        Assert.Equal(3, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
        Assert.Equal(2, card3.Position);
    }
}
