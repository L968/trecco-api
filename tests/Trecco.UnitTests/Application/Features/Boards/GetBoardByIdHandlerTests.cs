using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;
using Trecco.Application.Features.Boards.Queries.GetBoardById;

namespace Trecco.UnitTests.Application.Features.Boards;

public class GetBoardByIdHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<GetBoardByIdHandler>> _loggerMock;
    private readonly GetBoardByIdHandler _handler;

    public GetBoardByIdHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<GetBoardByIdHandler>>();
        _handler = new GetBoardByIdHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var query = new GetBoardByIdQuery(boardId, requesterId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result<GetBoardByIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(boardId).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        var query = new GetBoardByIdQuery(board.Id, unauthorizedUserId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result<GetBoardByIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _repositoryMock.Verify(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenOwnerRequestsBoard()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        var query = new GetBoardByIdQuery(board.Id, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result<GetBoardByIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(board.Id, result.Value.Id);
        Assert.Equal(board.Name, result.Value.Name);
        Assert.Empty(result.Value.Lists);
        _repositoryMock.Verify(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenMemberRequestsBoard()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        board.AddMember(memberId);
        var query = new GetBoardByIdQuery(board.Id, memberId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result<GetBoardByIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(board.Id, result.Value.Id);
        Assert.Equal(board.Name, result.Value.Name);
        Assert.Empty(result.Value.Lists);
        _repositoryMock.Verify(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnBoardWithListsAndCards_WhenBoardHasContent()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Test Board", ownerId);
        List list1 = board.AddList("To Do");
        List list2 = board.AddList("Done");

        list1.AddCard("Task 1", "Description 1");
        list1.AddCard("Task 2", "Description 2");
        list2.AddCard("Task 3", "Description 3");

        var query = new GetBoardByIdQuery(board.Id, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result<GetBoardByIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(board.Id, result.Value.Id);
        Assert.Equal(board.Name, result.Value.Name);
        Assert.Equal(2, result.Value.Lists.Count);

        // Verify first list
        BoardListDto firstList = result.Value.Lists.First(l => l.Name == "To Do");
        Assert.Equal(list1.Id, firstList.Id);
        Assert.Equal(0, firstList.Position);
        Assert.Equal(2, firstList.Cards.Count);
        Assert.Contains(firstList.Cards, c => c.Title == "Task 1" && c.Description == "Description 1");
        Assert.Contains(firstList.Cards, c => c.Title == "Task 2" && c.Description == "Description 2");

        // Verify second list
        BoardListDto secondList = result.Value.Lists.First(l => l.Name == "Done");
        Assert.Equal(list2.Id, secondList.Id);
        Assert.Equal(1, secondList.Position);
        Assert.Single(secondList.Cards);
        Assert.Contains(secondList.Cards, c => c.Title == "Task 3" && c.Description == "Description 3");

        _repositoryMock.Verify(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var query = new GetBoardByIdQuery(boardId, requesterId);
        var cancellationToken = new CancellationToken();

        var board = new Board("Test Board", requesterId);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(boardId, cancellationToken))
            .ReturnsAsync(board);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(boardId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBoardWithEmptyLists_WhenBoardHasNoLists()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Empty Board", ownerId);
        var query = new GetBoardByIdQuery(board.Id, ownerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result<GetBoardByIdResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(board.Id, result.Value.Id);
        Assert.Equal(board.Name, result.Value.Name);
        Assert.Empty(result.Value.Lists);
        _repositoryMock.Verify(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
