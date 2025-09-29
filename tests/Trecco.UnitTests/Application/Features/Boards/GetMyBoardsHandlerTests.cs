using Trecco.Application.Domain.Boards;
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
        var ownerId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(ownerId);
        _repositoryMock
            .Setup(r => r.GetBoardsByUserAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetBoardsByUserAsync(ownerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBoards_WhenBoardsFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(ownerId);
        var boards = new List<GetMyBoardsResponse>
        {
            new(Guid.NewGuid(), "Board 1", 0),
            new(Guid.NewGuid(), "Board 2", 0),
            new(Guid.NewGuid(), "Board 3", 0)
        };
        _repositoryMock
            .Setup(r => r.GetBoardsByUserAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, b => b.Name == "Board 1");
        Assert.Contains(result, b => b.Name == "Board 2");
        Assert.Contains(result, b => b.Name == "Board 3");
        _repositoryMock.Verify(r => r.GetBoardsByUserAsync(ownerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleBoard_WhenOneBoardFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(ownerId);
        var boards = new List<GetMyBoardsResponse>
        {
            new(boardId, "Single Board", 0)
        };
        _repositoryMock
            .Setup(r => r.GetBoardsByUserAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        GetMyBoardsResponse board = result.First();
        Assert.Equal(boardId, board.Id);
        Assert.Equal("Single Board", board.Name);
        _repositoryMock.Verify(r => r.GetBoardsByUserAsync(ownerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBoard_WhenUserIsMemberEvenIfNotOwner()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(memberId);
        var boards = new List<GetMyBoardsResponse>
        {
            new(boardId, "Member Board", 1) // Usuário é membro, não owner
        };
        _repositoryMock
            .Setup(r => r.GetBoardsByUserAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        IEnumerable<GetMyBoardsResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        GetMyBoardsResponse board = result.First();
        Assert.Equal(boardId, board.Id);
        Assert.Equal("Member Board", board.Name);
        _repositoryMock.Verify(r => r.GetBoardsByUserAsync(memberId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var query = new GetMyBoardsQuery(ownerId);
        var cancellationToken = new CancellationToken();
        _repositoryMock
            .Setup(r => r.GetBoardsByUserAsync(ownerId, cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetBoardsByUserAsync(ownerId, cancellationToken), Times.Once);
    }
}
