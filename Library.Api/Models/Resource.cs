using Newtonsoft.Json;
using System.Collections.Generic;

namespace Library.Api.Models
{
    public class Resource
    {
        [JsonProperty("_link", Order= 100)]
        public List<Link> Links { get; } = new List<Link>();
    }

    public class ResourceCollection<T> : Resource
        where T : Resource
    {
        public List<T> Items { get; }

        public ResourceCollection(List<T> items)
        {
            Items = items;
        }
    }
}
