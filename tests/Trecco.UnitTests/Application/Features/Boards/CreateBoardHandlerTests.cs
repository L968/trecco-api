using Trecco.Application.Common.Results;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Boards.Commands.CreateBoard;

namespace Trecco.UnitTests.Application.Features.Boards;

public class CreateBoardHandlerTests
{
    private readonly Mock<IBoardRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateBoardHandler>> _loggerMock;
    private readonly CreateBoardHandler _handler;

    public CreateBoardHandlerTests()
    {
        _repositoryMock = new Mock<IBoardRepository>();
        _loggerMock = new Mock<ILogger<CreateBoardHandler>>();
        _handler = new CreateBoardHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBoardAlreadyExistsForUser()
    {
        // Arrange
        var ownerUserId = Guid.NewGuid();
        var command = new CreateBoardCommand("Test Board", ownerUserId);

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(command.Name, command.OwnerUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Result<CreateBoardResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(BoardErrors.BoardAlreadyExistsForUser(command.Name).Code, result.Error.Code);
        _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenBoardIsCreated()
    {
        // Arrange
        var ownerUserId = Guid.NewGuid();
        var command = new CreateBoardCommand("New Board", ownerUserId);

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(command.Name, command.OwnerUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        Result<CreateBoardResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.OwnerUserId, result.Value.OwnerUserId);

        _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Once());
    }
}
