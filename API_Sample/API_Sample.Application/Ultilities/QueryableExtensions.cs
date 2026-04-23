using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace API_Sample.Application.Ultilities
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Safe paging cho SQL Server 2008 R2 (không hỗ trợ OFFSET/FETCH).
        /// Fetch toàn bộ rồi paging in-memory. Với bảng lớn nên upgrade SQL Server 2012+.
        /// </summary>
        public static async Task<List<TDestination>> ToPagedListAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            int page,
            int record,
            IMapper mapper) where TSource : class
        {
            page = page > 0 ? page : 1;
            record = record > 0 ? record : 10;

            // SQL Server 2008 R2 không hỗ trợ OFFSET/FETCH
            // Fetch toàn bộ rồi paging in-memory
            var entities = await query.ToListAsync();

            var pagedEntities = entities
                .Skip((page - 1) * record)
                .Take(record)
                .ToList();

            return mapper.Map<List<TDestination>>(pagedEntities);
        }

        /// <summary>
        /// Safe list mapping for older SQL Server versions.
        /// Uses ToListAsync then Map instead of ProjectTo.
        /// </summary>
        public static async Task<List<TDestination>> ToMappedListAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper) where TSource : class
        {
            var entities = await query.ToListAsync();
            return mapper.Map<List<TDestination>>(entities);
        }

        /// <summary>
        /// Paging tương thích SQL Server 2008 R2 với ProjectTo.
        /// Dùng ROW_NUMBER() pattern thay vì OFFSET/FETCH không hỗ trợ.
        /// </summary>
        public static async Task<(List<TDestination> Data, int TotalCount)> ToPagedListWithCountAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            int page,
            int record,
            IConfigurationProvider mapperConfig) where TSource : class
        {
            page = page > 0 ? page : 1;
            record = record > 0 ? record : 10;

            int count = await query.CountAsync();

            if (count == 0)
                return (new List<TDestination>(), 0);

            // SQL Server 2008 R2: fetch all rồi paging in-memory
            var allData = await query
                .ProjectTo<TDestination>(mapperConfig)
                .ToListAsync();

            var pagedData = allData
                .Skip((page - 1) * record)
                .Take(record)
                .ToList();

            return (pagedData, count);
        }

        public static IOrderedQueryable<T> OrderByColumn<T>(this IQueryable<T> source, string columnPath)
            => source.OrderByColumnUsing(columnPath, "OrderBy");

        public static IOrderedQueryable<T> OrderByColumnDescending<T>(this IQueryable<T> source, string columnPath)
            => source.OrderByColumnUsing(columnPath, "OrderByDescending");

        public static IOrderedQueryable<T> ThenByColumn<T>(this IOrderedQueryable<T> source, string columnPath)
            => source.OrderByColumnUsing(columnPath, "ThenBy");

        public static IOrderedQueryable<T> ThenByColumnDescending<T>(this IOrderedQueryable<T> source, string columnPath)
            => source.OrderByColumnUsing(columnPath, "ThenByDescending");

        private static IOrderedQueryable<T> OrderByColumnUsing<T>(this IQueryable<T> source, string columnPath, string method)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            var member = columnPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);
            var keySelector = Expression.Lambda(member, parameter);
            var methodCall = Expression.Call(typeof(Queryable), method, new[]
                    { parameter.Type, member.Type },
                source.Expression, Expression.Quote(keySelector));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery(methodCall);
        }


        public static IQueryable<string> SelectFromString<T>(this IQueryable<T> query, string column)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = MakePropPath(parameter, column);

            if (property.Type != typeof(string))
            {
                if (property.Type != typeof(object))
                    property = Expression.Convert(property, typeof(object));

                property = Expression.Call(_toStringMethod, property);
            }

            var lambda = Expression.Lambda<Func<T, string>>(property, parameter);

            return query.Select(lambda);
        }

        private static Expression MakePropPath(Expression objExpression, string path)
        {
            return path.Split('.').Aggregate(objExpression, Expression.PropertyOrField);
        }

        private static MethodInfo _toStringMethod = typeof(Convert).GetMethods()
            .Single(m =>
                m.Name == nameof(Convert.ToString) && m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == typeof(object)
            );
    }
}
