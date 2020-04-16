using Library.Api.Data;
using Library.Api.Entities;
using Library.Api.Extensions;
using Library.Api.Helpers;
using Library.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Library.Api.Services
{
    public class AuthorRepository : RepositoryBase<Author, Guid>, IAuthorRepository
    {
        private Dictionary<string, PropertyMapping> mappingDict = null;

        public AuthorRepository(DbContext dbContext) : base(dbContext)
        {
            mappingDict = new Dictionary<string, PropertyMapping>(StringComparer.OrdinalIgnoreCase);
            mappingDict.Add("Name", new PropertyMapping("Name"));
            mappingDict.Add("Age", new PropertyMapping("BirthDate", true));
            mappingDict.Add("BirthPlace", new PropertyMapping("BirthPlace"));
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
            if(parameters.SortBy == "Name")
            {
                queryableAuthors = queryableAuthors.OrderBy(author => author.Name);
            }

            //queryableAuthors = queryableAuthors.OrderBy(parameters.SortBy);
            //return PagedList<Author>.CreateAsync(queryableAuthors, parameters.pageNumber, parameters.PageSize);

            var orderedAuthors = queryableAuthors.Sort(parameters.SortBy, mappingDict);
            return PagedList<Author>.CreateAsync(orderedAuthors, parameters.pageNumber, parameters.PageSize);
        }
    }
}
