using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;
using Trecco.Application.Features.Cards.DeleteCard;

namespace Trecco.UnitTests.Application.Features.Cards;

public class DeleteCardHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<DeleteCardHandler>> _loggerMock;
    private readonly DeleteCardHandler _handler;

    public DeleteCardHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<DeleteCardHandler>>();
        _handler = new DeleteCardHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var command = new DeleteCardCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
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
    public async Task Handle_ShouldReturnSuccess_WhenCardDeleted()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List list = board.AddList("To Do");
        Card card = list.AddCard("Title", "Desc");
        var command = new DeleteCardCommand(board.Id, card.Id, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(list.Cards, c => c.Id == card.Id);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List list = board.AddList("To Do");
        Card card = list.AddCard("Title", "Desc");
        var command = new DeleteCardCommand(board.Id, card.Id, unauthorizedUserId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCardDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        board.AddList("To Do");
        var command = new DeleteCardCommand(board.Id, Guid.NewGuid(), ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }
}
