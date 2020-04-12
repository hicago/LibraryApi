using Library.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Services
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IAuthorRepository _authorRepository = null;
        private IBookRepository _bookRepository = null;
        public LibraryDbContext LibraryDbContext { get; }

        public IAuthorRepository Author => _authorRepository ?? new AuthorRepository(LibraryDbContext);
        public IBookRepository Book => _bookRepository ?? new BookRepository(LibraryDbContext);

        public RepositoryWrapper(LibraryDbContext libraryDbContext)
        {
            LibraryDbContext = libraryDbContext;
        }
    }
}
