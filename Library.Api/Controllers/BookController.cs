using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public IAuthorRepository AuthorRepository { get; }
        public IBookRepository BookRepository { get; }

        public BookController(IBookRepository bookRepository, IAuthorRepository authorRepository)
        {
            AuthorRepository = authorRepository;
            BookRepository = bookRepository;
        }

        [HttpGet]
        public ActionResult<List<BookDto>> GetBooks(Guid authorId)
        {
            if(!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }
            return BookRepository.GetBooksForAuthor(authorId).ToList();
        }

        [HttpGet("{bookid}", Name = "GetBook")]
        public ActionResult<BookDto> GetBook(Guid authorId, Guid bookId)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var targetBook = BookRepository.GetBookForAuthor(authorId, bookId);
            if(targetBook == null)
            {
                return NotFound();
            }
            return targetBook;
        }

        [HttpPost]
        public ActionResult AddBook(Guid authorId, BookForCreationDto bookForCreationDto)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var newBook = new BookDto
            {
                Id = Guid.NewGuid(),
                Title = bookForCreationDto.Title,
                Description = bookForCreationDto.Description,
                Pages = bookForCreationDto.Pages,
                AuthorId = authorId,
            };

            BookRepository.AddBook(newBook);
            return CreatedAtRoute(nameof(GetBook), new { authorid = authorId, bookId = newBook.Id }, newBook);
        }

        [HttpDelete("{bookId}")]
        public ActionResult DeleteBook(Guid authorId, Guid bookId)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NoContent();
            }

            var book = BookRepository.GetBookForAuthor(authorId, bookId);
            if(book == null)
            {
                return NotFound();
            }

            BookRepository.DeleteBook(book);
            return NoContent();
        }

        [HttpPut("{bookId}")]
        public ActionResult UpdateBook(Guid authorId, Guid bookId, BookForUpdateDto updatedBook)
        {
            if (!AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var book = BookRepository.GetBookForAuthor(authorId, bookId);
            if(book == null)
            {
                return NotFound();
            }

            BookRepository.UpdateBook(authorId, bookId, updatedBook);
            return NoContent();
        }

        [HttpPatch("{bookId}")]
        public ActionResult PartiallyUpdateBook(Guid authorId, Guid bookId,
            JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if(! AuthorRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var book = BookRepository.GetBookForAuthor(authorId, bookId);
            if(book == null)
            {
                return NotFound();
            }

            var bookToPatch = new BookForUpdateDto
            {
                Title = book.Title,
                Description = book.Description,
                Pages = book.Pages,
            };

            patchDocument.ApplyTo(bookToPatch);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookRepository.UpdateBook(authorId, bookId, bookToPatch);
            return NoContent();
        }
    }
}