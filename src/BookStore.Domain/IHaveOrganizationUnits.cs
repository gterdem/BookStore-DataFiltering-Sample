using JetBrains.Annotations;

namespace BookStore;

public interface IHaveOrganizationUnits
{
    [CanBeNull]
    string OuCodes { get; }
}