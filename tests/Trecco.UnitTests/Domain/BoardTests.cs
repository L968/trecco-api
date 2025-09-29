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
        Assert.Equal(2, board.Lists.Count);
        Assert.Equal(0, list1.Position);
        Assert.Equal(1, list2.Position);
    }

    [Fact]
    public void HasAccess_ShouldReturnTrue_ForOwner()
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
    public void HasAccess_ShouldReturnTrue_ForMember()
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
    public void HasAccess_ShouldReturnFalse_ForNonMember()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();
        var board = new Board("Test", ownerId);

        // Act
        bool hasAccess = board.HasAccess(nonMemberId);

        // Assert
        Assert.False(hasAccess);
    }

    [Fact]
    public void GetCardById_ShouldReturnCard_WhenCardExists()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");
        Card card = list.AddCard("Card", "Description");

        // Act
        Card? foundCard = board.GetCardById(card.Id);

        // Assert
        Assert.NotNull(foundCard);
        Assert.Equal(card.Id, foundCard.Id);
    }

    [Fact]
    public void GetCardById_ShouldReturnNull_WhenCardDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        Card? foundCard = board.GetCardById(Guid.NewGuid());

        // Assert
        Assert.Null(foundCard);
    }

    [Fact]
    public void RemoveList_ShouldRemoveList_WhenListExists()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");

        // Act
        board.RemoveList(list.Id);

        // Assert
        Assert.Empty(board.Lists);
    }

    [Fact]
    public void RemoveList_ShouldNotThrow_WhenListDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act & Assert
        board.RemoveList(Guid.NewGuid());
        Assert.Empty(board.Lists);
    }

    [Fact]
    public void DeleteCard_ShouldRemoveCard_WhenCardExists()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");
        Card card = list.AddCard("Card", "Description");

        // Act
        board.DeleteCard(card.Id);

        // Assert
        Assert.Empty(list.Cards);
    }

    [Fact]
    public void DeleteCard_ShouldNotThrow_WhenCardDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act & Assert
        board.DeleteCard(Guid.NewGuid());
        Assert.Empty(board.Lists);
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
    public void GetCardById_ShouldReturnNull_WhenCardDoesNotExistInAnyList()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        board.AddList("List");

        // Act
        Card? foundCard = board.GetCardById(Guid.NewGuid());

        // Assert
        Assert.Null(foundCard);
    }

    [Fact]
    public void DeleteCard_ShouldNotThrow_WhenCardDoesNotExistInAnyList()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");

        // Act & Assert
        board.DeleteCard(Guid.NewGuid());
        Assert.Empty(list.Cards);
    }

    [Fact]
    public void RemoveList_ShouldNotThrow_WhenListDoesNotExistInBoard()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        board.AddList("List");

        // Act & Assert
        board.RemoveList(Guid.NewGuid());
        Assert.Single(board.Lists);
    }

    [Fact]
    public void MoveCard_ShouldReturnSuccess_WhenCardMoved()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List sourceList = board.AddList("To Do");
        List targetList = board.AddList("Done");
        Card card = sourceList.AddCard("Task", "Desc");

        // Act
        Result result = board.MoveCard(card.Id, targetList.Id, 0, Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(card, sourceList.Cards);
        Assert.Contains(card, targetList.Cards);
        Assert.Equal(0, card.Position);
    }

    [Fact]
    public void MoveCard_ShouldInsertAtEnd_WhenTargetPositionIsGreaterThanListSize()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List sourceList = board.AddList("Source");
        List targetList = board.AddList("Target");
        Card existingCard = targetList.AddCard("Existing", "Desc");
        Card cardToMove = sourceList.AddCard("To Move", "Desc");

        // Act
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 99, Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(cardToMove, sourceList.Cards);
        Assert.Contains(cardToMove, targetList.Cards);
        Assert.Equal(0, existingCard.Position);
        Assert.Equal(1, cardToMove.Position);
    }

    [Fact]
    public void MoveCard_ShouldInsertAtEnd_WhenTargetPositionEqualsListSize()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List sourceList = board.AddList("Source");
        List targetList = board.AddList("Target");
        Card existingCard = targetList.AddCard("Existing", "Desc");
        Card cardToMove = sourceList.AddCard("To Move", "Desc");

        // Act
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 1, Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(cardToMove, sourceList.Cards);
        Assert.Contains(cardToMove, targetList.Cards);
        Assert.Equal(0, existingCard.Position);
        Assert.Equal(1, cardToMove.Position);
    }

    [Fact]
    public void MoveCard_ShouldMoveToStart_AdjustPositionsCorrectly()
    {
        // Arrange
        var board = new Board("Test Board", Guid.NewGuid());
        List sourceList = board.AddList("Source");
        List targetList = board.AddList("Target");

        Card card1 = targetList.AddCard("Card 1", "Desc");
        Card card2 = targetList.AddCard("Card 2", "Desc");
        Card cardToMove = sourceList.AddCard("To Move", "Desc");

        // Act
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 0, Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(cardToMove, sourceList.Cards);
        Assert.Contains(cardToMove, targetList.Cards);
        Assert.Equal(0, cardToMove.Position);
        Assert.Equal(1, card1.Position);
        Assert.Equal(2, card2.Position);
    }

    [Fact]
    public void MoveCard_ShouldMoveToMiddle_AdjustPositionsCorrectly()
    {
        // Arrange
        var board = new Board("Test Board", Guid.NewGuid());
        List sourceList = board.AddList("Source");
        List targetList = board.AddList("Target");

        Card card1 = targetList.AddCard("Card 1", "Desc");
        Card card2 = targetList.AddCard("Card 2", "Desc");
        Card card3 = targetList.AddCard("Card 3", "Desc");
        Card cardToMove = sourceList.AddCard("To Move", "Desc");

        // Act
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 1, Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(cardToMove, sourceList.Cards);
        Assert.Contains(cardToMove, targetList.Cards);
        Assert.Equal(1, cardToMove.Position);
        Assert.Equal(0, card1.Position);
        Assert.Equal(2, card2.Position);
        Assert.Equal(3, card3.Position);
    }

    [Fact]
    public void MoveCard_ShouldMoveToEnd_AdjustPositionsCorrectly()
    {
        // Arrange
        var board = new Board("Test Board", Guid.NewGuid());
        List sourceList = board.AddList("Source");
        List targetList = board.AddList("Target");

        Card card1 = targetList.AddCard("Card 1", "Desc");
        Card card2 = targetList.AddCard("Card 2", "Desc");
        Card cardToMove = sourceList.AddCard("To Move", "Desc");

        // Act
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 99, Guid.NewGuid());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(cardToMove, sourceList.Cards);
        Assert.Contains(cardToMove, targetList.Cards);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
        Assert.Equal(2, cardToMove.Position);
    }
}
