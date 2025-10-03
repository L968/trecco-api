using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Features.Cards.MoveCard;
using Trecco.Domain.Cards;
using Trecco.Domain.DomainEvents;
using Trecco.Domain.Lists;

namespace Trecco.UnitTests.Application.Features.Cards;

public class MoveCardHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly Mock<ILogger<MoveCardHandler>> _loggerMock;
    private readonly MoveCardHandler _handler;

    public MoveCardHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _dispatcherMock = new Mock<IDomainEventDispatcher>();
        _loggerMock = new Mock<ILogger<MoveCardHandler>>();
        _handler = new MoveCardHandler(_repositoryMock.Object, _dispatcherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new MoveCardCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, Guid.NewGuid());
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(command.BoardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _dispatcherMock.Verify(m => m.DispatchAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void MoveCard_ShouldReturnFailure_WhenCardNotFound()
    {
        // Arrange
        var board = new Board("Test Board", Guid.NewGuid());
        List targetList = board.AddList("To Do");
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
        var board = new Board("Test Board", Guid.NewGuid());
        List sourceList = board.AddList("Backlog");
        Card card = sourceList.AddCard("Title", "Description");
        var nonExistentListId = Guid.NewGuid();

        // Act
        Result result = board.MoveCard(card.Id, nonExistentListId, 0);

        // Assert
        Assert.True(result.IsFailure);
        Assert.StartsWith(ListErrors.NotFound(nonExistentListId).Description, result.Error.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List sourceList = board.AddList("Backlog");
        List targetList = board.AddList("To Do");
        Card card = sourceList.AddCard("Title", "Description");

        var command = new MoveCardCommand(board.Id, card.Id, targetList.Id, 0, unauthorizedUserId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _dispatcherMock.Verify(m => m.DispatchAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCardMoved()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List sourceList = board.AddList("Backlog");
        List targetList = board.AddList("To Do");
        Card card = sourceList.AddCard("Title", "Description");

        var command = new MoveCardCommand(board.Id, card.Id, targetList.Id, 0, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(sourceList.Cards, c => c.Id == card.Id);
        Assert.Contains(targetList.Cards, c => c.Id == card.Id);
        Assert.Equal(0, card.Position);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcherMock.Verify(m => m.DispatchAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
