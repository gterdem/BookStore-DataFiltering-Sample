using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStore.Books;
using BookStore.EntityFrameworkCore;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace BookStore;

public class Books_Tests : BookStoreEntityFrameworkCoreTestBase
{
    private readonly TestData _testData;
    private readonly IIdentityUserRepository _userRepository;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
    private readonly IBookRepository _bookRepository;

    public Books_Tests()
    {
        _bookRepository = GetRequiredService<IBookRepository>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        _userRepository = GetRequiredService<IIdentityUserRepository>();
        _testData = GetRequiredService<TestData>();
    }

    [Fact]
    public async Task Test_Seeded_Data()
    {
        var result = await _userRepository.GetOrganizationUnitsAsync(_testData.UserJohnId);
        result.Count.ShouldNotBe(0);
        result.ShouldContain(t => t.DisplayName == "OU111");
    }

    [Fact]
    public async Task User_Book_Creation()
    {
        var johnUser = new ClaimsPrincipal(
            new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(AbpClaimTypes.UserId, _testData.UserJohnId.ToString()),
                    new Claim(AbpClaimTypes.UserName, "john"),
                }
            )
        );
        
        var hostBook = await _bookRepository.InsertAsync(
            new Book()
            {
                Name = "Host Book",
                Price = 15.5f,
                Type = BookType.Adventure
            }
        );
        
        using (_currentPrincipalAccessor.Change(johnUser))
        {
            var testBook = await _bookRepository.InsertAsync(
                new Book()
                {
                    Name = "Test Book",
                    Price = 15.5f,
                    Type = BookType.Adventure
                }
            );
            testBook.OuCodes.ShouldNotBeEmpty();
        }
    }
}