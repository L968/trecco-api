using Trecco.Application.Common;
using Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;
using Trecco.Domain.BoardActionLogs;

namespace Trecco.UnitTests.Application.Features.Boards;

public class GetBoardActionLogsHandlerTests
{
    private readonly Mock<IBoardActionLogRepository> _logRepoMock;
    private readonly Mock<IBoardRepository> _boardRepoMock;
    private readonly Mock<ILogger<GetBoardActionLogsHandler>> _loggerMock;
    private readonly GetBoardActionLogsHandler _handler;

    public GetBoardActionLogsHandlerTests()
    {
        _logRepoMock = new Mock<IBoardActionLogRepository>();
        _boardRepoMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<GetBoardActionLogsHandler>>();
        _handler = new GetBoardActionLogsHandler(_logRepoMock.Object, _boardRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardNotFound()
    {
        // Arrange
        var query = new GetBoardActionLogsQuery(Guid.NewGuid(), Guid.NewGuid(), 1, 10, null);
        _boardRepoMock
            .Setup(r => r.GetByIdAsync(query.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        Result<PaginatedList<GetBoardActionLogsResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotFound(query.BoardId).Code, result.Error.Code);
        _logRepoMock.Verify(r => r.GetByBoardAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRequesterNotAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        var query = new GetBoardActionLogsQuery(board.Id, unauthorizedUserId, 1, 10, null);

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(query.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        // Act
        Result<PaginatedList<GetBoardActionLogsResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.NotAuthorized.Code, result.Error.Code);
        _logRepoMock.Verify(r => r.GetByBoardAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedLogs_WhenAuthorized()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var board = new Board("Board", ownerId);
        var query = new GetBoardActionLogsQuery(board.Id, ownerId, 2, 5, "search");

        var logs = new List<BoardActionLog>
        {
            new(board.Id, ownerId, "Did something"),
            new(board.Id, ownerId, "Did something else"),
        };

        _boardRepoMock
            .Setup(r => r.GetByIdAsync(query.BoardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        _logRepoMock
            .Setup(r => r.GetByBoardAsync(query.BoardId, query.Page, query.PageSize, query.SearchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        // Act
        Result<PaginatedList<GetBoardActionLogsResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.NotNull(result.Value);
        Assert.Equal(query.Page, result.Value.Page);
        Assert.Equal(query.PageSize, result.Value.PageSize);
        Assert.Equal(logs.Count, result.Value.Items.Count());
        _loggerMock.VerifyLog(LogLevel.Debug, Times.Once());
    }
}
