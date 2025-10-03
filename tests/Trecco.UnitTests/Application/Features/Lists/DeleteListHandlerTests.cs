using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Features.Lists.DeleteList;
using Trecco.Domain.Lists;

namespace Trecco.UnitTests.Application.Features.Lists;

public class DeleteListHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly Mock<ILogger<DeleteListHandler>> _loggerMock;
    private readonly DeleteListHandler _handler;

    public DeleteListHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _dispatcherMock = new Mock<IDomainEventDispatcher>();
        _loggerMock = new Mock<ILogger<DeleteListHandler>>();
        _handler = new DeleteListHandler(_repositoryMock.Object, _dispatcherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(command.BoardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        List list = board.AddList("List");

        var command = new DeleteListCommand(board.Id, list.Id, unauthorizedUserId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteList_AndDispatchDomainEvents_WhenAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        Trecco.Domain.Lists.List list = board.AddList("List");
        var command = new DeleteListCommand(board.Id, list.Id, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcherMock.Verify(d => d.DispatchAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Once());
    }
}



