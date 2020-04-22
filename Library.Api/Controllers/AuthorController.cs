using AutoMapper;
using Library.Api.Entities;
using Library.Api.Helpers;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public IMapper Mapper { get; }
        public IRepositoryWrapper RepositoryWrapper { get; }
        public ILogger<AuthorController> Logger { get; }

        public AuthorController(IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILogger<AuthorController> logger)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
            Logger = logger;
        }

        [HttpGet(Name = nameof(GetAuthorsAsync))]
        public async Task<ActionResult<ResourceCollection<AuthorDto>>> GetAuthorsAsync(
            [FromQuery] AuthorResourceParameters parameters)
        {
            var pagedList = await RepositoryWrapper.Author.GetAllAsync(parameters);
            var paginationMetadata = new
            {
                totalCount = pagedList.TotalCount,
                pageSize = pagedList.PageSize,
                currentPage = pagedList.CurrentPage,
                totalPages = pagedList.TotalPages,
                previousPageLink = pagedList.HasPrevious ? Url.Link(nameof(GetAuthorsAsync),
                new
                {
                    pageNumber = pagedList.CurrentPage - 1,
                    pageSize = pagedList.PageSize,
                    birthPlace = parameters.BirthPlace,
                    searchQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy
                }) : null,
                nextPageLink = pagedList.HasNext ? Url.Link(nameof(GetAuthorsAsync),
                new
                {
                    pageNumber = pagedList.CurrentPage + 1,
                    pageSize = pagedList.PageSize,
                    birthPlace = parameters.BirthPlace,
                    searchQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy
                }) : null
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var authorDtoList = Mapper.Map<IEnumerable<AuthorDto>>(pagedList);
            authorDtoList = authorDtoList.Select(author => CreateLinksForAuthor(author));
            var resourceList = new ResourceCollection<AuthorDto>(authorDtoList.ToList());
            return CreateLinksForAuthors(resourceList, parameters, paginationMetadata);
        }

        [HttpGet("{authorID}", Name = nameof(GetAuthorAsync))]
        public async Task<ActionResult<AuthorDto>> GetAuthorAsync(Guid authorId)
        {
            var author = await RepositoryWrapper.Author.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            var authorDto = Mapper.Map<AuthorDto>(author);
            return CreateLinksForAuthor(authorDto);
        }

        [HttpPost(Name = nameof(CreateAuthorAsync))]
        public async Task<ActionResult> CreateAuthorAsync(AuthorForCreationDto authorForCreationDto)
        {
            var author = Mapper.Map<Author>(authorForCreationDto);

            RepositoryWrapper.Author.Create(author);
            var result = await RepositoryWrapper.Author.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源author失败");
            }

            var authorCreated = Mapper.Map<AuthorDto>(author);
            return CreatedAtRoute(nameof(GetAuthorAsync),
                new { authorId = authorCreated.Id },
                CreateLinksForAuthor(authorCreated));
        }

        [HttpDelete("{authorId}", Name = nameof(DeleteAuthorAsync))]
        public async Task<ActionResult> DeleteAuthorAsync(Guid authorId)
        {
            var author = await RepositoryWrapper.Author.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            RepositoryWrapper.Author.Delete(author);
            var result = await RepositoryWrapper.Author.SaveAsync();
            if (!result)
            {
                throw new Exception("删除资源author失败");
            }

            return NoContent();
        }

        private AuthorDto CreateLinksForAuthor(AuthorDto author)
        {
            author.Links.Clear();
            author.Links.Add(new Link(HttpMethods.Get,
                "self",
                Url.Link(nameof(GetAuthorAsync), new { authorId = author.Id })));
            author.Links.Add(new Link(HttpMethods.Delete,
                "delete author",
                Url.Link(nameof(DeleteAuthorAsync), new { authorId = author.Id })));
            author.Links.Add(new Link(HttpMethods.Get,
                "author's books",
                Url.Link(nameof(BookController.GetBooksAsync), new { authorId = author.Id })));

            return author;
        }

        private ResourceCollection<AuthorDto> CreateLinksForAuthors(
            ResourceCollection<AuthorDto> authors,
            AuthorResourceParameters parameters = null,
            dynamic paginationData = null)
        {
            authors.Links.Clear();
            authors.Links.Add(new Link(HttpMethods.Get,
                "self",
                Url.Link(nameof(GetAuthorsAsync), parameters)));

            authors.Links.Add(new Link(HttpMethods.Post,
                "Create author",
                Url.Link(nameof(CreateAuthorAsync), null)));

            if (paginationData != null)
            {
                if (paginationData.previousPageLink != null)
                {
                    authors.Links.Add(new Link(HttpMethods.Get,
                        "previous page",
                        paginationData.previousPageLink));
                }
            }

            if (paginationData.nextPageLink != null)
            {
                authors.Links.Add(new Link(HttpMethods.Get,
                    "next page",
                    paginationData.nextPageLink));
            }

            return authors;
        }
    }
}