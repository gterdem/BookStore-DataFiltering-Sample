using System;
using Volo.Abp.DependencyInjection;

namespace BookStore;

public class TestData : ISingletonDependency
{
    public Guid UserJohnId { get; } = Guid.NewGuid();
    public Guid UserDavidId { get; } = Guid.NewGuid();
    public Guid UserNeoId { get; } = Guid.NewGuid();
}