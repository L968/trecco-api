using System.Reflection;

namespace Trecco.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
}
