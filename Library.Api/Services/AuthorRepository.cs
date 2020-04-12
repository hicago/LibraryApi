using Library.Api.Data;
using Library.Api.Entities;
using Library.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Api.Services
{
    public class AuthorRepository : RepositoryBase<Author, Guid>, IAuthorRepository
    {
        public AuthorRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
