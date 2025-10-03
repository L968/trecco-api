using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Boards.Commands.AddMember;

namespace Trecco.UnitTests.Application.Features.Boards;

public class AddMemberHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly Mock<ILogger<AddMemberHandler>> _loggerMock;
    private readonly AddMemberHandler _handler;

    public AddMemberHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<AddMemberHandler>>();
        _dispatcherMock = new Mock<IDomainEventDispatcher>();
        _handler = new AddMemberHandler(_repositoryMock.Object, _dispatcherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new AddMemberCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(command.BoardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserAlreadyMember()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        board.AddMember(userId);

        var command = new AddMemberCommand(boardId, userId, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.AlreadyMember.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);

        var command = new AddMemberCommand(boardId, userId, unauthorizedUserId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTryingToAddOwnerAsMember()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);

        var command = new AddMemberCommand(boardId, ownerId, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.Contains(ownerId, board.MemberIds);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserAdded()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);

        var command = new AddMemberCommand(boardId, userId, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }
}
