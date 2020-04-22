using Library.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Controllers
{
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public ActionResult GetRoot()
        {
            var links = new List<Link>();
            links.Add(new Link(HttpMethods.Get,
                "self",
                Url.Link(nameof(GetRoot), null)));

            links.Add(new Link(HttpMethods.Get,
                "get authors",
                Url.Link(nameof(AuthorController.GetAuthorsAsync), null)));

            links.Add(new Link(HttpMethods.Post,
                "create author",
                Url.Link(nameof(AuthorController.CreateAuthorAsync), null)));

            return Ok(links);
        }
    }
}
