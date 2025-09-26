using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;

namespace Trecco.UnitTests.Domain;

public class BoardTests
{
    [Fact]
    public void Should_CreateBoard_WithValidValues()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        var board = new Board("Test Board", ownerId);

        // Assert
        Assert.Equal("Test Board", board.Name);
        Assert.Equal(ownerId, board.OwnerUserId);
        Assert.Empty(board.MemberIds);
        Assert.Empty(board.Lists);
    }

    [Fact]
    public void UpdateName_ShouldChangeNameAndUpdatedAt()
    {
        // Arrange
        var board = new Board("Old Name", Guid.NewGuid());
        DateTime beforeUpdate = board.UpdatedAt;

        // Act
        board.UpdateName("New Name");

        // Assert
        Assert.Equal("New Name", board.Name);
        Assert.True(board.UpdatedAt > beforeUpdate);
    }

    [Fact]
    public void AddMember_ShouldAddUniqueMember()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        var userId = Guid.NewGuid();

        // Act
        board.AddMember(userId);
        board.AddMember(userId);

        // Assert
        Assert.Single(board.MemberIds);
        Assert.Contains(userId, board.MemberIds);
    }

    [Fact]
    public void RemoveMember_ShouldRemoveMemberIfExists()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        var userId = Guid.NewGuid();
        board.AddMember(userId);

        // Act
        board.RemoveMember(userId);

        // Assert
        Assert.Empty(board.MemberIds);
    }

    [Fact]
    public void AddList_ShouldAddListWithCorrectPosition()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        List list1 = board.AddList("List 1");
        List list2 = board.AddList("List 2");

        // Assert
        Assert.Equal(0, list1.Position);
        Assert.Equal(1, list2.Position);
        Assert.Equal(2, board.Lists.Count);
    }

    [Fact]
    public void RemoveList_ShouldRemoveExistingList()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List 1");

        // Act
        board.RemoveList(list.Id);

        // Assert
        Assert.Empty(board.Lists);
    }

    [Fact]
    public void MoveCard_ShouldMoveCardBetweenListsAndAdjustPositions_WithMultipleCards()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        List sourceList = board.AddList("To Do");
        List targetList = board.AddList("Done");

        Card card1 = sourceList.AddCard("Task 1", "Desc");
        Card card2 = sourceList.AddCard("Task 2", "Desc");
        Card card3 = sourceList.AddCard("Task 3", "Desc");

        Card card4 = targetList.AddCard("Task 4", "Desc");
        Card card5 = targetList.AddCard("Task 5", "Desc");

        int targetPosition = 1;

        // Act
        board.MoveCard(card2.Id, targetList.Id, targetPosition);

        // Assert
        Assert.DoesNotContain(card2, sourceList.Cards);
        Assert.Contains(card2, targetList.Cards);
        Assert.Equal(targetPosition, card2.Position);

        Assert.Equal(0, card4.Position);
        Assert.Equal(2, card5.Position);

        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card3.Position);
    }

    [Fact]
    public void GetListByCardId_ShouldReturnCorrectList()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");
        Card card = list.AddCard("Task", "Desc");

        // Act
        List? foundList = board.GetListByCardId(card.Id);

        // Assert
        Assert.Equal(list, foundList);
    }

    [Fact]
    public void MoveCard_ShouldThrow_WhenCardNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List targetList = board.AddList("Done");

        // Act & Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            board.MoveCard(Guid.NewGuid(), targetList.Id, 0)
        );
        Assert.Equal("Card not found", ex.Message);
    }

    [Fact]
    public void MoveCard_ShouldThrow_WhenTargetListNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List sourceList = board.AddList("To Do");
        Card card = sourceList.AddCard("Task", "Desc");

        // Act & Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            board.MoveCard(card.Id, Guid.NewGuid(), 0)
        );
        Assert.Equal("Target list not found", ex.Message);
    }
}
