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
    public void Constructor_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Board(null!, Guid.NewGuid()));
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
    public void UpdateName_ShouldThrowArgumentException_WhenNameIsEmptyOrWhitespace()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => board.UpdateName(""));
        Assert.Throws<ArgumentException>(() => board.UpdateName("   "));
    }

    [Fact]
    public void AddMember_ShouldAddUniqueMember_AndRaiseEvent()
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
        Assert.Contains(board.DomainEvents, e => e.GetType().Name == "MemberAddedDomainEvent");
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
    public void RemoveMember_ShouldNotThrow_WhenMemberNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        Exception ex = Record.Exception(() => board.RemoveMember(Guid.NewGuid()));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void CanRemoveMember_ShouldFail_WhenOwnerTriesToRemoveSelf()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test", ownerId);

        // Act
        Result result = board.CanRemoveMember(ownerId, ownerId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.CannotRemoveOwner, result.Error);
    }

    [Fact]
    public void CanRemoveMember_ShouldFail_WhenMemberTriesToRemoveAnother()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var member1 = Guid.NewGuid();
        var member2 = Guid.NewGuid();
        var board = new Board("Test", ownerId);
        board.AddMember(member1);
        board.AddMember(member2);

        // Act
        Result result = board.CanRemoveMember(member1, member2);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.CannotRemoveOtherMember, result.Error);
    }

    [Fact]
    public void CanRemoveMember_ShouldSucceed_WhenOwnerRemovesMember()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var board = new Board("Test", ownerId);
        board.AddMember(memberId);

        // Act
        Result result = board.CanRemoveMember(ownerId, memberId);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void CanRemoveMember_ShouldSucceed_WhenMemberRemovesSelf()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var board = new Board("Test", ownerId);
        board.AddMember(memberId);

        // Act
        Result result = board.CanRemoveMember(memberId, memberId);

        // Assert
        Assert.True(result.IsSuccess);
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
        var board = new Board("Test", Guid.NewGuid());

        // Act
        bool hasAccess = board.HasAccess(Guid.NewGuid());

        // Assert
        Assert.False(hasAccess);
    }

    [Fact]
    public void AddList_ShouldAddListWithCorrectPosition_AndRaiseEvent()
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
        Assert.Contains(board.DomainEvents, e => e.GetType().Name == "ListAddedDomainEvent");
    }

    [Fact]
    public void RenameList_ShouldUpdateName_AndRaiseEvent()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("Old List");

        // Act
        Result result = board.RenameList(list.Id, "New List");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New List", list.Name);
        Assert.Contains(board.DomainEvents, e => e.GetType().Name == "ListRenamedDomainEvent");
    }

    [Fact]
    public void RenameList_ShouldFail_WhenListNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        Result result = board.RenameList(Guid.NewGuid(), "New Name");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void RemoveList_ShouldRemoveList_AndRaiseEvent()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");

        // Act
        board.RemoveList(list.Id);

        // Assert
        Assert.Empty(board.Lists);
        Assert.Contains(board.DomainEvents, e => e.GetType().Name == "ListDeletedDomainEvent");
    }

    [Fact]
    public void RemoveList_ShouldNotThrow_WhenListDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        Exception ex = Record.Exception(() => board.RemoveList(Guid.NewGuid()));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void GetCardById_ShouldReturnCard_WhenExists()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");
        Card card = list.AddCard("Card", "Description");

        // Act
        Card? found = board.GetCardById(card.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(card.Id, found.Id);
    }

    [Fact]
    public void GetCardById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        Card? found = board.GetCardById(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void MoveCard_ShouldFail_WhenCardNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");

        // Act
        Result result = board.MoveCard(Guid.NewGuid(), list.Id, 0);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void MoveCard_ShouldFail_WhenTargetListNotFound()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");
        Card card = list.AddCard("Card", "Desc");

        // Act
        Result result = board.MoveCard(card.Id, Guid.NewGuid(), 0);

        // Assert
        Assert.True(result.IsFailure);
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
        Result result = board.MoveCard(card.Id, targetList.Id, 0);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(card, sourceList.Cards);
        Assert.Contains(card, targetList.Cards);
        Assert.Equal(0, card.Position);
        Assert.Contains(board.DomainEvents, e => e.GetType().Name == "CardMovedDomainEvent");
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
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 99);

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
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 1);

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
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 0);

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
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 1);

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
        Result result = board.MoveCard(cardToMove.Id, targetList.Id, 99);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(cardToMove, sourceList.Cards);
        Assert.Contains(cardToMove, targetList.Cards);
        Assert.Equal(0, card1.Position);
        Assert.Equal(1, card2.Position);
        Assert.Equal(2, cardToMove.Position);
    }

    [Fact]
    public void DeleteCard_ShouldRemoveCardAndUpdateDate()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());
        List list = board.AddList("List");
        Card card = list.AddCard("Card", "Desc");
        DateTime before = board.UpdatedAt;

        // Act
        board.DeleteCard(card.Id);

        // Assert
        Assert.Empty(list.Cards);
        Assert.True(board.UpdatedAt > before);
    }

    [Fact]
    public void DeleteCard_ShouldNotThrow_WhenCardDoesNotExist()
    {
        // Arrange
        var board = new Board("Test", Guid.NewGuid());

        // Act
        Exception ex = Record.Exception(() => board.DeleteCard(Guid.NewGuid()));

        // Assert
        Assert.Null(ex);
    }
}
