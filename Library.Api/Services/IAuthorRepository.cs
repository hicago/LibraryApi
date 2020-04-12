using Library.Api.Entities;
using Library.Api.Models;
using System;
using System.Collections.Generic;

namespace Library.Api.Services
{
    public interface IAuthorRepository : IRepositoryBase<Author>, IRepositoryBase2<Author, Guid>
    {
        //IEnumerable<AuthorDto> GetAuthors();
        //AuthorDto GetAuthor(Guid authorID);
        //bool IsAuthorExists(Guid authorID);
        //void AddAuthor(AuthorDto author);
        //void DeleteAuthor(AuthorDto author);
    }
}
