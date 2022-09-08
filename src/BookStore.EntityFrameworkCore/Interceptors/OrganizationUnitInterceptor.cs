using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Identity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace BookStore;

public class OrganizationUnitInterceptor : AbpInterceptor, IScopedDependency
{
    private readonly ICurrentUser _currentUser;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IIdentityUserRepository _identityUserRepository;

    public OrganizationUnitInterceptor(ICurrentUser currentUser, IdentityUserManager identityUserManager,
        IUnitOfWorkManager unitOfWorkManager, IIdentityUserRepository identityUserRepository)
    {
        _currentUser = currentUser;
        _identityUserManager = identityUserManager;
        _unitOfWorkManager = unitOfWorkManager;
        _identityUserRepository = identityUserRepository;
    }

    public async override Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        var ouCodes =  await GetUserOrganizationUnits();
        var topOu = ouCodes.OrderBy(q => q.Length).FirstOrDefault();
        topOu = topOu == null ? String.Empty : topOu;
        _unitOfWorkManager.Current.Items.Add("ouCode", topOu);
        await invocation.ProceedAsync();
    }

    private async Task<List<string>> GetUserOrganizationUnits()
    {
        var user = await _identityUserRepository.FindAsync(_currentUser.GetId());
        if (user == null)
        {
            return new List<string>();
        }

        var organizationUnitsOfCurrentUser = await _identityUserManager.GetOrganizationUnitsAsync(user);
        return organizationUnitsOfCurrentUser.Select(q => q.Code).ToList();
    }
}