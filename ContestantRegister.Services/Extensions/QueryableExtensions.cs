using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestantRegister.Services.Extensions
{
    //абстракция IReadRepository протекает. там, где вызываются Async методы типа ToListAsync, AnyAsync - это вызываются экстеншн-методы EF
    //в принципе, в этом нет ничего страшного, ибо смысл существования IReadRepository - спрятать от query-хендлеров методы EF контекста типа Delete и SaveChanges, которые дожны использоваться колько в комманд-хендлерах
    //но для чистоты архитектуры эту дыру закрываем. на учебном проекте можно себе такое позволить, чтобы посмотреть, что из этого выйдет. но в продакшене так заморачиваться уже не стоит )
    public interface IOrmExtensionsHider
    {
        Task<List<T>> ToListAsync<T>(IQueryable<T> source);
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source);
        Task<T> SingleAsync<T>(IQueryable<T> source);
        Task<T> SingleAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source);
        Task<int> CountAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate);
        IQueryable<TEntity> Include<TEntity, TProperty>(IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class;
        IQueryable<TEntity> Include<TEntity>(IQueryable<TEntity> source, string navigationPropertyPath) where TEntity : class;
    }

    public static class QueryableExtensions
    {
        public static IOrmExtensionsHider OrmExtensionsHider { get; set; }

        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return OrmExtensionsHider.ToListAsync(source);
        }

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source)
        {
            return OrmExtensionsHider.SingleOrDefaultAsync(source);
        }

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return OrmExtensionsHider.SingleOrDefaultAsync(source, predicate);
        }

        public static Task<T> SingleAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return OrmExtensionsHider.SingleAsync(source, predicate);
        }

        public static Task<T> SingleAsync<T>(this IQueryable<T> source)
        {
            return OrmExtensionsHider.SingleAsync(source);
        }

        public static Task<bool> AnyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return OrmExtensionsHider.AnyAsync(source, predicate);
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return OrmExtensionsHider.FirstOrDefaultAsync(source, predicate);
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source)
        {
            return OrmExtensionsHider.FirstOrDefaultAsync(source);
        }

        public static Task<int> CountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return OrmExtensionsHider.CountAsync(source, predicate);
        }

        public static IQueryable<TEntity> Include<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class
        {
            return OrmExtensionsHider.Include(source, navigationPropertyPath);
        }

        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, string navigationPropertyPath) where TEntity : class
        {
            return OrmExtensionsHider.Include(source, navigationPropertyPath);
        }
    }
}
