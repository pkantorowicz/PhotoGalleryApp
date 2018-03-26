using System.Collections.Generic;
using System.Linq;
using Gallery.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.App.Controllers
{
    public abstract class ApiControllerBase : Controller
    {
        protected PaginateResult<T> CreatePagedResults<T>(
            IEnumerable<T> source, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var enumerable = source as IList<T> ?? source.ToList();

            var projection = enumerable.Skip(skipAmount).Take(pageSize);

            var totalNumberOfRecords = enumerable.Count;
            var results = projection.ToList();

            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            var nextPageUrl =
                page == totalPageCount
                    ? null
                    : Url?.Link("DefaultApi", new
                    {
                        page = page + 1,
                        pageSize });

            return new PaginateResult<T>
            {
                Items = results,
                Page = page,
                PerPage = results.Count,
                Total = totalNumberOfRecords,
                NextPageUrl = nextPageUrl
            };
        }
    }
}