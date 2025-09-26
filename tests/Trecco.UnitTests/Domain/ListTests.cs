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
}
