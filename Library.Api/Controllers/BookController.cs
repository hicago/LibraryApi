using AutoMapper;
using Library.Api.Entities;
using Library.Api.Filters;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Controllers
{
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    [ServiceFilter(typeof(CheckAuthorExistFilterAttribute))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookController : ControllerBase
    {
        public IRepositoryWrapper RepositoryWrapper { get; }
        public IMapper Mapper { get; }
        public IHashFactory HashFactory { get; }

        public BookController(IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IHashFactory hashFactory)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
            HashFactory = hashFactory;
        }

        [HttpGet(Name = nameof(GetBooksAsync))]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksAsync(Guid authorId)
        {
            var books = await RepositoryWrapper.Book.GetBooksAsync(authorId);
            var bookDtoList = Mapper.Map<IEnumerable<BookDto>>(books);

            return bookDtoList.ToList();
        }

        [HttpGet("{bookId}", Name = "GetBookAsync")]
        public async Task<ActionResult<BookDto>> GetBookAsync(Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityHash;

            var bookDto = Mapper.Map<BookDto>(book);
            return bookDto;
        }

        [HttpPost]
        public async Task<ActionResult> AddBookAsync(Guid authorId, BookForCreationDto bookForCreationDto)
        {
            var book = Mapper.Map<Book>(bookForCreationDto);

            book.AuthorId = authorId;
            RepositoryWrapper.Book.Create(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("创建资源Book失败");
            }

            var bookDto = Mapper.Map<BookDto>(book);
            return CreatedAtRoute(nameof(GetBookAsync),
                new { bookId = bookDto.Id },
                bookDto);
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBookAsync(Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            RepositoryWrapper.Book.Delete(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("删除资源Book失败");
            }
            return NoContent();
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBookAsync(Guid authorId, Guid bookId, BookForUpdateDto updatedBook)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(book);
            if (Request.Headers.TryGetValue(HeaderNames.IfMatch, out var requestETag)
                && requestETag != entityHash)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }

            Mapper.Map(updatedBook, book, typeof(BookForUpdateDto), typeof(Book));
            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源Book失败");
            }

            var entityNewHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityNewHash;

            return NoContent();
        }

        [HttpPatch("{bookId}")]
        [CheckIfMatchHeaderFilter]
        public async Task<IActionResult> ParticallyUpdateBookAsync(Guid authorId, Guid bookId, JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var entityHash = HashFactory.GetHash(book);
            if (Request.Headers.TryGetValue(HeaderNames.IfMatch, out var requestETag)
                && requestETag != entityHash)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }

            var bookUpdateDto = Mapper.Map<BookForUpdateDto>(book);
            patchDocument.ApplyTo(bookUpdateDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(bookUpdateDto, book, typeof(BookForUpdateDto), typeof(Book));

            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源Book失败");
            }

            var entityNewHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityNewHash;

            return NoContent();
        }
    }
}