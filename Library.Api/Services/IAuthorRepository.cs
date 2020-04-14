using Library.Api.Entities;
using Library.Api.Helpers;
using Library.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Api.Services
{
    public interface IAuthorRepository : IRepositoryBase<Author>, IRepositoryBase2<Author, Guid>
    {
        Task<PagedList<Author>> GetAllAsync(AuthorResourceParameters parameters);
    }
}
