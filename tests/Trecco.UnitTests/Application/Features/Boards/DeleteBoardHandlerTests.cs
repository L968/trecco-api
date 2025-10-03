using Trecco.Application.Features.Boards.Commands.DeleteBoard;

namespace Trecco.UnitTests.Application.Features.Boards;

public class DeleteBoardHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<DeleteBoardHandler>> _loggerMock;
    private readonly DeleteBoardHandler _handler;

    public DeleteBoardHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<DeleteBoardHandler>>();
        _handler = new DeleteBoardHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new DeleteBoardCommand(Guid.NewGuid(), Guid.NewGuid());
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(command.BoardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterIsNotOwner()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var nonOwnerId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        var command = new DeleteBoardCommand(board.Id, nonOwnerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NoPermission.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteBoard_WhenRequesterIsOwner()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        var command = new DeleteBoardCommand(board.Id, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        _repositoryMock.Verify(r => r.DeleteAsync(board.Id, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Once());
    }
}
