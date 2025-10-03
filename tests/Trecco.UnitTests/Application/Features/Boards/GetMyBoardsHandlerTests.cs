using Moq;
using Trecco.Application.Features.Boards.Queries.GetMyBoards;

namespace Trecco.UnitTests.Application.Features.Boards;

public class GetMyBoardsHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<GetMyBoardsHandler>> _loggerMock;
    private readonly GetMyBoardsHandler _handler;

    public GetMyBoardsHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<GetMyBoardsHandler>>();
        _handler = new GetMyBoardsHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoBoardsFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(userId);
        _repositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBoards_WhenBoardsFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(userId);

        var boards = new List<Board>
    {
        new Board("Board 1", userId),
        new Board("Board 2", userId),
        new Board("Board 3", userId)
    };

        _repositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, b => b.Name == "Board 1");
        Assert.Contains(result, b => b.Name == "Board 2");
        Assert.Contains(result, b => b.Name == "Board 3");

        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleBoard_WhenOneBoardFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(ownerId);
        var board = new Board("Single Board", ownerId);

        var boards = new List<Board> { board };

        _repositoryMock
            .Setup(r => r.GetByUserIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        GetMyBoardsResponse response = result.First();
        Assert.Equal(board.Id, response.Id);
        Assert.Equal(board.Name, response.Name);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(ownerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBoard_WhenUserIsMemberEvenIfNotOwner()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(memberId);

        var board = new Board("Member Board", Guid.NewGuid());
        board.AddMember(memberId);

        var boards = new List<Board> { board };

        _repositoryMock
            .Setup(r => r.GetByUserIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        GetMyBoardsResponse response = result.First();
        Assert.Equal(board.Id, response.Id);
        Assert.Equal(board.Name, response.Name);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(memberId, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(ownerId);
        var cancellationToken = new CancellationToken();
        _repositoryMock
            .Setup(r => r.GetByUserIdAsync(ownerId, cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByUserIdAsync(ownerId, cancellationToken), Times.Once);
    }
}
