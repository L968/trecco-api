using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Boards.Commands.AddMember;

namespace Trecco.UnitTests.Application.Features.Boards;

public class AddMemberHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<AddMemberHandler>> _loggerMock;
    private readonly AddMemberHandler _handler;

    public AddMemberHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<AddMemberHandler>>();
        _handler = new AddMemberHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new AddMemberCommand(Guid.NewGuid(), Guid.NewGuid());
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
        var userId = Guid.NewGuid();
        var board = new Board("Test Board", Guid.NewGuid());
        board.AddMember(userId);

        var command = new AddMemberCommand(boardId, userId);

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
    public async Task Handle_ShouldReturnSuccess_WhenUserAdded()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var board = new Board("Test Board", Guid.NewGuid());

        var command = new AddMemberCommand(boardId, userId);

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
