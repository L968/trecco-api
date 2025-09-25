using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Boards.Commands.RemoveMember;

namespace Trecco.UnitTests.Application.Features.Boards;

public sealed class RemoveMemberHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<RemoveMemberHandler>> _loggerMock;
    private readonly RemoveMemberHandler _handler;

    public RemoveMemberHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<RemoveMemberHandler>>();
        _handler = new RemoveMemberHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new RemoveMemberCommand(Guid.NewGuid(), Guid.NewGuid());
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(command.BoardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotMember()
    {
        // Arrange
        var board = new Board("Test Board", Guid.NewGuid());
        var command = new RemoveMemberCommand(board.Id, Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotMember.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserRemoved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var board = new Board("Test Board", Guid.NewGuid());
        board.AddMember(userId);

        var command = new RemoveMemberCommand(board.Id, userId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.DoesNotContain(userId, board.MemberIds);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Once());
    }
}
