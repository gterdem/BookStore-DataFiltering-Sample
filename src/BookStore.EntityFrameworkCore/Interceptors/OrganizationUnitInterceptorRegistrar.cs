using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BookStore.Books;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Uow;

namespace BookStore;

public static class OrganizationUnitInterceptorRegistrar
{
    private static readonly List<Type> RepositoryList = new()
    {
        typeof(IBookRepository)
    };
    public static void RegisterIfNeeded(IOnServiceRegistredContext context)
    {
        if (ShouldIntercept(context.ImplementationType))
        {
            context.Interceptors.TryAdd<OrganizationUnitInterceptor>();
        }
    }

    private static bool ShouldIntercept(Type type)
    {
        return RepositoryList.Any(q => q.IsAssignableFrom(type)) && UnitOfWorkHelper.IsUnitOfWorkType(type.GetTypeInfo());
    }
}