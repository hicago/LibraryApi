using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Helpers
{
    public class AuthorResourceParameters
    {
        public const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int pageNumber { get; set; } = 1;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        public string BirthPlace { get; set; }

        public string SearchQuery { get; set; }
    }
}
