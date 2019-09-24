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
}
