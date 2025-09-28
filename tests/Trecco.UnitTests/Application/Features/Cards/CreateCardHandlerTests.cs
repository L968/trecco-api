using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Lists;
using Trecco.Application.Features.Cards.CreateCard;

namespace Trecco.UnitTests.Application.Features.Cards;

public class CreateCardHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateCardHandler>> _loggerMock;
    private readonly CreateCardHandler _handler;

    public CreateCardHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<CreateCardHandler>>();
        _handler = new CreateCardHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new CreateCardCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", "Description", Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(command.BoardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenListNotFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        var command = new CreateCardCommand(board.Id, Guid.NewGuid(), "Title", "Description", ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ListErrors.NotFound(command.ListId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List list = board.AddList("To Do");
        var command = new CreateCardCommand(board.Id, list.Id, "Title", "Description", unauthorizedUserId);

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
    public async Task Handle_ShouldReturnSuccess_WhenCardCreated()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List list = board.AddList("To Do");
        var command = new CreateCardCommand(board.Id, list.Id, "Title", "Description", ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCardCreatedWithEmptyDescription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List list = board.AddList("To Do");
        var command = new CreateCardCommand(board.Id, list.Id, "Title", "", ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenMemberCreatesCard()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        board.AddMember(memberId);
        List list = board.AddList("To Do");
        var command = new CreateCardCommand(board.Id, list.Id, "Title", "Description", memberId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }
}
