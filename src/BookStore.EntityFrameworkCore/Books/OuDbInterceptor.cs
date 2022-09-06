using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace BookStore;

public class OuDbInterceptor : DbCommandInterceptor, IScopedDependency
{
    private readonly ICurrentUser _currentUser;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IIdentityUserRepository _identityUserRepository;

    public OuDbInterceptor(ICurrentUser currentUser, IdentityUserManager identityUserManager,
        IUnitOfWorkManager unitOfWorkManager, IIdentityUserRepository identityUserRepository)
    {
        _currentUser = currentUser;
        _identityUserManager = identityUserManager;
        _unitOfWorkManager = unitOfWorkManager;
        _identityUserRepository = identityUserRepository;
    }

    public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData,
        InterceptionResult<DbCommand> result)
    {
        var ouCodes = AsyncHelper.RunSync(GetUserOrganizationUnits);
        _unitOfWorkManager.Current.Items.Add("ouCodes", JsonSerializer.Serialize(ouCodes));
        return base.CommandCreating(eventData, result);
    }

    private async Task<List<string>> GetUserOrganizationUnits()
    {
        var currentUser = await _identityUserRepository.GetAsync(_currentUser.GetId());
        var organizationUnitsOfCurrentUser = await _identityUserManager.GetOrganizationUnitsAsync(currentUser);
        return organizationUnitsOfCurrentUser.Select(q => q.Code).ToList();
    }
}