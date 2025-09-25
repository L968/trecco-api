using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Lists.Commands.CreateList;

namespace Trecco.UnitTests.Application.Features.Lists;

public class CreateListHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateListHandler>> _loggerMock;
    private readonly CreateListHandler _handler;

    public CreateListHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<CreateListHandler>>();
        _handler = new CreateListHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateListCommand(boardId, "New List");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result<CreateListResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(boardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenListCreated()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var board = new Board("Test Board", Guid.NewGuid());
        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        var command = new CreateListCommand(boardId, "New List");

        // Act
        Result<CreateListResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Once());
    }
}
