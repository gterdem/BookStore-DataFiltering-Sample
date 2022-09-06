using System.Threading.Tasks;
using BookStore.EntityFrameworkCore;
using Shouldly;
using Volo.Abp.Identity;
using Xunit;

namespace BookStore;

public class Books_Tests : BookStoreEntityFrameworkCoreTestBase
{
    private readonly TestData _testData;
    private readonly IIdentityUserRepository _userRepository;

    public Books_Tests()
    {
        _userRepository = GetRequiredService<IIdentityUserRepository>();
        _testData = GetRequiredService<TestData>();
    }

    [Fact]
    public async Task Test_Seeded_Data()
    {
        var result = await _userRepository.GetOrganizationUnitsAsync(_testData.UserJohnId);
        result.Count.ShouldNotBe(0);
        result.ShouldContain(t=>t.DisplayName == "OU111");
    }
}