using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Gallery.Infrastructure.Extensions
{
    public class PaginateResult<T>
    {
        [JsonProperty("items")]
        public IEnumerable<T> Items { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("perPage")]
        public int PerPage { get; set; }

        [JsonProperty("NextPageUrl")]
        public string NextPageUrl { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage => Page < TotalPages - 1;

        [JsonProperty("hasPrevPage")]
        public bool HasPrevPage => Page > 0;

        [JsonProperty("nextPage")]
        public int NextPage => HasNextPage ? Page + 1 : Page;

        [JsonProperty("prevPage")]
        public int PrevPage => HasPrevPage ? Page - 1 : Page;

        [JsonProperty("totalPages")]
        public int TotalPages
        {
            get
            {
                if (PerPage != 0)
                    return (int)Math.Ceiling(Total / (double)PerPage);
                return 0;
            }
        }

        [JsonProperty("itemsCount")]
        public int ItemsCount => Items.Count();

        [JsonProperty("from")]
        public int From => ItemsCount > 0 ? (Page * PerPage) + 1 : 0;

        [JsonProperty("to")]
        public int To => ItemsCount > 0 ? (Page * PerPage) + ItemsCount : 0;

        public PaginateResult()
        {          
        }

        public PaginateResult(IEnumerable<T> items, int page, int perPage)
        {
            var enumerable = items as IList<T> ?? items.ToList();

            Items = enumerable.Skip(page * perPage).Take(perPage).ToList();
            Total = enumerable.Count;
            Page = page;
            PerPage = perPage;
        }
    }
}