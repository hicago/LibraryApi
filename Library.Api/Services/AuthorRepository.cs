using Library.Api.Data;
using Library.Api.Entities;
using Library.Api.Helpers;
using Library.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Services
{
    public class AuthorRepository : RepositoryBase<Author, Guid>, IAuthorRepository
    {
        public AuthorRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<PagedList<Author>> GetAllAsync(AuthorResourceParameters parameters)
        {
            IQueryable<Author> queryableAuthors = DbContext.Set<Author>();
            if(! string.IsNullOrWhiteSpace(parameters.BirthPlace))
            {
                queryableAuthors = queryableAuthors.Where(m => m.BirthPlace.ToLower() ==
                    parameters.BirthPlace);
            }
            if(!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                queryableAuthors = queryableAuthors.Where(
                    m => m.BirthPlace.ToLower().Contains(parameters.SearchQuery.ToLower())
                     || m.Name.ToLower().Contains(parameters.SearchQuery.ToLower()));
            }
            return PagedList<Author>.CreateAsync(queryableAuthors, parameters.pageNumber, parameters.PageSize);
        }
    }
}
