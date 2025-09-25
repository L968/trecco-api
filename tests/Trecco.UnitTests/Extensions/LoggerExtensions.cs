using Microsoft.Extensions.Logging;

namespace Trecco.UnitTests.Extensions;

internal static class LoggerExtensions
{
    public static void VerifyLog<T>(
        this Mock<ILogger<T>> logger,
        LogLevel level,
        Times times)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            times
        );
    }
}
