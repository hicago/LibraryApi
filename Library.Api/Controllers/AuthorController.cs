using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public IAuthorRepository AuthorRepository { get; }

        public AuthorController(IAuthorRepository authorRepository)
        {
            AuthorRepository = authorRepository;
        }

        [HttpGet]
        public ActionResult<List<AuthorDto>> GetAuthors()
        {
            return AuthorRepository.GetAuthors().ToList();
        }

        [HttpGet("{authorID}", Name = nameof(GetAuthor))]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = AuthorRepository.GetAuthor(authorId);
            if (author == null)
            {
                return NotFound();
            }
            else
            {
                return author;
            }
        }

        [HttpPost]
        public ActionResult CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorDto = new AuthorDto
            {
                Name = authorForCreationDto.Name,
                Age = authorForCreationDto.Age,
                Email = authorForCreationDto.Email
            };
            AuthorRepository.AddAuthor(authorDto);
            return CreatedAtRoute(nameof(GetAuthor), new { authorId = authorDto.Id }, authorDto);
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var author = AuthorRepository.GetAuthor(authorId);
            if(author == null)
            {
                return NotFound();
            }

            AuthorRepository.DeleteAuthor(author);
            return NoContent();
        }
    }
}