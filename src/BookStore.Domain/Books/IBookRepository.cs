﻿using System;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Books;

public interface IBookRepository : IRepository<Book, Guid>
{
}