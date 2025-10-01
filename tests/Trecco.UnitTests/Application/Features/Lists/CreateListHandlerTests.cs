using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Lists.CreateList;

namespace Trecco.UnitTests.Application.Features.Lists;

public class CreateListHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly Mock<ILogger<CreateListHandler>> _loggerMock;
    private readonly CreateListHandler _handler;

    public CreateListHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _dispatcherMock = new Mock<IDomainEventDispatcher>();
        _loggerMock = new Mock<ILogger<CreateListHandler>>();
        _handler = new CreateListHandler(_repositoryMock.Object, _dispatcherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateListCommand(boardId, "New List", Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(boardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        var command = new CreateListCommand(boardId, "New List", unauthorizedUserId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenListCreated()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        var command = new CreateListCommand(boardId, "New List", ownerId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }
}
