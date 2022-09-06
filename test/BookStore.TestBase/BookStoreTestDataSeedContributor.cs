using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace BookStore;

public class BookStoreTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityUserManager _userManager;
    private readonly TestData _testData;
    private readonly OrganizationUnitManager _organizationUnitManager;
    private readonly IOrganizationUnitRepository _organizationUnitRepository;

    private OrganizationUnit _ou1;
    private OrganizationUnit _ou21;
    private OrganizationUnit _ou111;
    private OrganizationUnit _ou112;

    public BookStoreTestDataSeedContributor(
        IdentityUserManager userManager,
        IGuidGenerator guidGenerator,
        OrganizationUnitManager organizationUnitManager,
        IOrganizationUnitRepository organizationUnitRepository,
        IIdentityUserRepository userRepository,
        TestData testData)
    {
        _userManager = userManager;
        _guidGenerator = guidGenerator;
        _organizationUnitManager = organizationUnitManager;
        _organizationUnitRepository = organizationUnitRepository;
        _userRepository = userRepository;
        _testData = testData;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await AddOrganizationUnits();
        await AddUsers();
    }

    /* Creates OU tree as shown below:
     *
     * - OU1
     *   - OU11
     *     - OU111
     *     - OU112
     *   - OU12
     * - OU2
     *   - OU21
     */
    private async Task AddOrganizationUnits()
    {
        _ou1 = await CreateOU("OU1", OrganizationUnit.CreateCode(1));
        var ou11 = await CreateOU("OU11", OrganizationUnit.CreateCode(1, 1), _ou1.Id);
        _ou112 = await CreateOU("OU112", OrganizationUnit.CreateCode(1, 1, 2), ou11.Id);
        var ou12 = await CreateOU("OU12", OrganizationUnit.CreateCode(1, 2), _ou1.Id);
        var ou2 = await CreateOU("OU2", OrganizationUnit.CreateCode(2));
        _ou21 = await CreateOU("OU21", OrganizationUnit.CreateCode(2, 1), ou2.Id);

        _ou111 = await CreateOU("OU111", OrganizationUnit.CreateCode(1, 1, 1), ou11.Id);
        await _organizationUnitRepository.InsertAsync(_ou111);
    }

    private async Task AddUsers()
    {
        var john = new IdentityUser(_testData.UserJohnId, "john.nash", "john.nash@abp.io");
        john.AddOrganizationUnit(_ou111.Id);
        john.AddOrganizationUnit(_ou112.Id);
        await _userRepository.InsertAsync(john);

        var david = new IdentityUser(_testData.UserDavidId, "david", "david@abp.io");
        david.AddOrganizationUnit(_ou1.Id);
        david.AddOrganizationUnit(_ou21.Id);
        await _userRepository.InsertAsync(david);

        var neo = new IdentityUser(_testData.UserNeoId, "neo", "neo@abp.io");
        neo.AddOrganizationUnit(_ou111.Id);
        await _userRepository.InsertAsync(neo);
    }

    private async Task<OrganizationUnit> CreateOU(string displayName, string code, Guid? parentId = null)
    {
        var ouEntity = new OrganizationUnit(_guidGenerator.Create(), displayName, parentId);
        ObjectHelper.TrySetProperty(ouEntity, t => t.Code, () => code);
        var ou = await _organizationUnitRepository.InsertAsync(ouEntity);

        return ou;
    }
}