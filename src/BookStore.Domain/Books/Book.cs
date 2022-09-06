using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace BookStore.Books;

public class Book : AuditedAggregateRoot<Guid>, IHaveOrganizationUnits
{
    public string Name { get; set; }
    public BookType Type { get; set; }
    public DateTime PublishDate { get; set; }
    public float Price { get; set; }
    public string OuCodes { get; set; }
}