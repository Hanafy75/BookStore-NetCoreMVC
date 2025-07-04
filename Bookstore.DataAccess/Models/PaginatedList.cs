using Microsoft.EntityFrameworkCore;

namespace Bookstore.DataAccess.Models
{
    public class PaginatedList<T> where T : class
    {
        public List<T> Items { get; }
        public int TotalPages { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public PaginatedList(List<T> items, int count, int pageSize, int pageIndex)
        {
            Items = items;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public bool HasPrev => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = await source.CountAsync();
            var items = await source.Skip((pageIndex-1)*pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageSize,pageIndex);
        }
    }
}
