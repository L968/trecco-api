using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Features.Lists.UpdateListName;

namespace Trecco.UnitTests.Application.Features.Lists;

public class UpdateListNameHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly Mock<ILogger<UpdateListNameHandler>> _loggerMock;
    private readonly UpdateListNameHandler _handler;

    public UpdateListNameHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _dispatcherMock = new Mock<IDomainEventDispatcher>();
        _loggerMock = new Mock<ILogger<UpdateListNameHandler>>();
        _handler = new UpdateListNameHandler(_repositoryMock.Object, _dispatcherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new UpdateListNameCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name", Guid.NewGuid());
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
        _loggerMock.VerifyLog(LogLevel.Information, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        Trecco.Domain.Lists.List list = board.AddList("List");
        var command = new UpdateListNameCommand(board.Id, list.Id, "New Name", unauthorizedUserId);

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
    public async Task Handle_ShouldRenameList_AndDispatchDomainEvents_WhenAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        Trecco.Domain.Lists.List list = board.AddList("List");
        var command = new UpdateListNameCommand(board.Id, list.Id, "New Name", ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal("New Name", board.Lists.First(l => l.Id == list.Id).Name);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _dispatcherMock.Verify(d => d.DispatchAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Once());
    }
}



