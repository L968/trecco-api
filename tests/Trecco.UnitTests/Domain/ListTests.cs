using Trecco.Domain.Cards;
using Trecco.Domain.Lists;

namespace Trecco.UnitTests.Domain;

public class ListTests
{
    [Fact]
    public void Should_CreateList_WithValidValues()
    {
        // Arrange
        string name = "Test List";
        int position = 0;

        // Act
        var list = new List(name, position);

        // Assert
        Assert.Equal(name, list.Name);
        Assert.Equal(position, list.Position);
        Assert.Empty(list.Cards);
    }

    [Fact]
    public void UpdateName_ShouldChangeName_WhenValidName()
    {
        // Arrange
        var list = new List("Old Name", 0);

        // Act
        list.UpdateName("New Name");

        // Assert
        Assert.Equal("New Name", list.Name);
    }

    [Fact]
    public void AddCard_ShouldAddCardWithCorrectPosition()
    {
        // Arrange
        var list = new List("Test", 0);

        // Act
        Card card1 = list.AddCard("Card 1", "Description");
        Card card2 = list.AddCard("Card 2", "Description");

        // Assert
        Assert.Equal(2, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
    }

    [Fact]
    public void RemoveCard_ShouldRemoveCardAndAdjustPositions()
    {
        // Arrange
        var list = new List("Test", 0);
        Card card1 = list.AddCard("Card 1", "Description");
        Card card2 = list.AddCard("Card 2", "Description");
        Card card3 = list.AddCard("Card 3", "Description");

        // Act
        list.RemoveCard(card2.Id);

        // Assert
        Assert.Equal(2, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card3.Position);
    }

    [Fact]
    public void InsertCard_ShouldInsertCardAtSpecifiedPosition()
    {
        // Arrange
        var list = new List("Test", 0);
        Card card1 = list.AddCard("Card 1", "Description");
        Card card2 = list.AddCard("Card 2", "Description");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, 1);

        // Assert
        Assert.Equal(3, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, newCard.Position);
        Assert.Equal(2, card2.Position);
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
        var list = new List("Test", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => list.UpdateName(""));
        Assert.Throws<ArgumentException>(() => list.UpdateName("   "));
    }

    [Fact]
    public void AddCard_ShouldIncrementPosition_WhenCardsAlreadyExist()
    {
        // Arrange
        var list = new List("Test", 0);
        Card firstCard = list.AddCard("First", "Desc");

        // Act
        Card secondCard = list.AddCard("Second", "Desc");

        // Assert
        Assert.Equal(0, firstCard.Position);
        Assert.Equal(1, secondCard.Position);
    }

    [Fact]
    public void AddCard_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var list = new List("Test", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => list.AddCard("", "Description"));
        Assert.Throws<ArgumentException>(() => list.AddCard("   ", "Description"));
    }

    [Fact]
    public void AddCard_ShouldThrowArgumentException_WhenTitleIsNull()
    {
        // Arrange
        var list = new List("Test", 0);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => list.AddCard(null!, "Description"));
    }

    [Fact]
    public void InsertCard_ShouldShiftPositionsCorrectly_WhenInsertingAtBeginning()
    {
        // Arrange
        var list = new List("Test", 0);
        Card card1 = list.AddCard("Card 1", "Desc");
        Card card2 = list.AddCard("Card 2", "Desc");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, 0);

        // Assert
        Assert.Equal(3, list.Cards.Count);
        Assert.Equal(0, newCard.Position);
        Assert.Equal(1, card1.Position);
        Assert.Equal(2, card2.Position);
    }

    [Fact]
    public void InsertCard_ShouldNotShiftPositionsBelowInsertedPosition()
    {
        // Arrange
        var list = new List("Test", 0);
        Card card1 = list.AddCard("Card 1", "Desc");
        Card card2 = list.AddCard("Card 2", "Desc");
        Card card3 = list.AddCard("Card 3", "Desc");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, 1);

        // Assert
        Assert.Equal(0, card1.Position);
        Assert.Equal(2, card2.Position);
        Assert.Equal(3, card3.Position);
        Assert.Equal(1, newCard.Position);
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
    public void InsertCard_ShouldInsertAtEnd_WhenPositionIsGreaterThanListSize()
    {
        // Arrange
        var list = new List("List", 0);
        Card card1 = list.AddCard("Task 1", "Desc");
        Card card2 = list.AddCard("Task 2", "Desc");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, 99);

        // Assert
        Assert.Equal(3, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
        Assert.Equal(2, newCard.Position);
        Assert.Equal(newCard, list.Cards.Last());
    }

    [Fact]
    public void InsertCard_ShouldInsertAtEnd_WhenPositionEqualsListSize()
    {
        // Arrange
        var list = new List("List", 0);
        Card card1 = list.AddCard("Task 1", "Desc");
        Card card2 = list.AddCard("Task 2", "Desc");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, 2);

        // Assert
        Assert.Equal(3, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
        Assert.Equal(2, newCard.Position);
        Assert.Equal(newCard, list.Cards.Last());
    }

    [Fact]
    public void InsertCard_ShouldInsertAtEnd_WhenPositionEqualsListCount()
    {
        // Arrange
        var list = new List("Test", 0);
        list.AddCard("Card 1", "Desc");
        list.AddCard("Card 2", "Desc");
        var newCard = new Card("Inserted", "Desc");

        // Act
        list.InsertCard(newCard, list.Cards.Count);

        // Assert
        Assert.Equal(list.Cards.Count - 1, newCard.Position);
        Assert.Equal(newCard, list.Cards.Last());
    }

    [Fact]
    public void RemoveCard_ShouldNotThrow_WhenCardDoesNotExist()
    {
        // Arrange
        var list = new List("Test", 0);

        // Act & Assert
        list.RemoveCard(Guid.NewGuid());
        Assert.Empty(list.Cards);
    }

    [Fact]
    public void AddCard_ShouldAddMultipleCards_WithCorrectPositions()
    {
        // Arrange
        var list = new List("Test", 0);

        // Act
        Card card1 = list.AddCard("Card 1", "Desc");
        Card card2 = list.AddCard("Card 2", "Desc");
        Card card3 = list.AddCard("Card 3", "Desc");

        // Assert
        Assert.Equal(3, list.Cards.Count);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
        Assert.Equal(2, card3.Position);
    }
}
