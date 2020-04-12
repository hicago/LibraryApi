using Library.Api.Entities;
using Library.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Services
{
    public interface IBookRepository : IRepositoryBase<Book>, IRepositoryBase2<Book, Guid>
    {
        Task<bool> IsExistAsync(Guid authorId, Guid bookId);

        Task<Book> GetBookAsync(Guid authorId, Guid bookId);

        Task<IEnumerable<Book>> GetBooksAsync(Guid authorId);
    }
}
