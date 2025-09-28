using Trecco.Application.Common.Results;
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
        Result result = board.MoveCard(card2.Id, targetList.Id, targetPosition);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(card2, sourceList.Cards);
        Assert.Contains(card2, targetList.Cards);
        Assert.Equal(targetPosition, card2.Position);

        Assert.Equal(0, card4.Position);
        Assert.Equal(2, card5.Position);

        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card3.Position);
    }

    [Fact]
    public void MoveCard_ShouldReturnFailure_WhenCardNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List targetList = board.AddList("Done");
        var nonExistentCardId = Guid.NewGuid();

        // Act
        Result result = board.MoveCard(nonExistentCardId, targetList.Id, 0);

        // Assert
        Assert.True(result.IsFailure);
        Assert.StartsWith(CardErrors.NotFound(nonExistentCardId).Description, result.Error.Description);
    }

    [Fact]
    public void MoveCard_ShouldReturnFailure_WhenTargetListNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List sourceList = board.AddList("To Do");
        Card card = sourceList.AddCard("Task", "Desc");
        var nonExistentListId = Guid.NewGuid();

        // Act
        Result result = board.MoveCard(card.Id, nonExistentListId, 0);

        // Assert
        Assert.True(result.IsFailure);
        Assert.StartsWith(ListErrors.NotFound(nonExistentListId).Description, result.Error.Description);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Board(null!, Guid.NewGuid()));
    }

    [Fact]
    public void UpdateName_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => board.UpdateName(""));
        Assert.Throws<ArgumentException>(() => board.UpdateName("   "));
    }

    [Fact]
    public void HasAccess_ShouldReturnTrue_WhenUserIsOwner()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test", ownerId);

        // Act
        bool hasAccess = board.HasAccess(ownerId);

        // Assert
        Assert.True(hasAccess);
    }

    [Fact]
    public void HasAccess_ShouldReturnTrue_WhenUserIsMember()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var board = new Board("Test", ownerId);
        board.AddMember(memberId);

        // Act
        bool hasAccess = board.HasAccess(memberId);

        // Assert
        Assert.True(hasAccess);
    }

    [Fact]
    public void HasAccess_ShouldReturnFalse_WhenUserIsNotOwnerOrMember()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Test", ownerId);

        // Act
        bool hasAccess = board.HasAccess(unauthorizedUserId);

        // Assert
        Assert.False(hasAccess);
    }

    [Fact]
    public void GetCardById_ShouldReturnNull_WhenCardDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        var nonExistentCardId = Guid.NewGuid();

        // Act
        Card? card = board.GetCardById(nonExistentCardId);

        // Assert
        Assert.Null(card);
    }

    [Fact]
    public void DeleteCard_ShouldNotThrow_WhenCardDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        var nonExistentCardId = Guid.NewGuid();

        // Act & Assert
        Exception? exception = Record.Exception(() => board.DeleteCard(nonExistentCardId));
        Assert.Null(exception);
    }

    [Fact]
    public void RemoveList_ShouldNotThrow_WhenListDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        var nonExistentListId = Guid.NewGuid();

        // Act & Assert
        Exception? exception = Record.Exception(() => board.RemoveList(nonExistentListId));
        Assert.Null(exception);
    }

    [Fact]
    public void MoveCard_ShouldReturnSuccess_WhenCardMovedSuccessfully()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List sourceList = board.AddList("To Do");
        List targetList = board.AddList("Done");
        Card card = sourceList.AddCard("Task", "Desc");

        // Act
        Result result = board.MoveCard(card.Id, targetList.Id, 0);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(card, sourceList.Cards);
        Assert.Contains(card, targetList.Cards);
        Assert.Equal(0, card.Position);
    }
}
