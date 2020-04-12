using Library.Api.Models;
using Library.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Services
{
    public class BookRepository : RepositoryBase<Book, Guid>, IBookRepository
    {
        public BookRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Book> GetBookAsync(Guid authorId, Guid bookId)
        {
            return await DbContext.Set<Book>()
                .SingleOrDefaultAsync(book => book.AuthorId == authorId && book.Id == bookId);
        }

        public Task<IEnumerable<Book>> GetBooksAsync(Guid authorId)
        {
            return Task.FromResult(DbContext.Set<Book>().Where(book => book.AuthorId == authorId).AsEnumerable());
        }

        public async Task<bool> IsExistAsync(Guid authorId, Guid bookId)
        {
            return await DbContext.Set<Book>().
                AnyAsync(book => book.AuthorId == authorId && book.Id == bookId);
        }
    }
}
