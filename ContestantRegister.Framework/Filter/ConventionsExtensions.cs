using System.Collections.Generic;
using System.Linq;

namespace ContestantRegister.Framework.Filter
{    
    public static class ConventionsExtensions
    {
        public static IQueryable<TSubject> AutoFilter<TSubject, TPredicate>(
            this IQueryable<TSubject> query, TPredicate predicate, ComposeKind composeKind = ComposeKind.And)
        {
            var filtered = Conventions<TSubject>.Filter(query, predicate, composeKind);
            return filtered;            
        }

        public static IEnumerable<TSubject> AutoFilter<TSubject, TPredicate>(
            this IEnumerable<TSubject> query, TPredicate predicate, ComposeKind composeKind = ComposeKind.And)
        {
            var filtered = Conventions<TSubject>.Filter(query, predicate, composeKind);
            return filtered;
        }

        public static IOrderedQueryable<TSubject> OrderBy<TSubject>(this IQueryable<TSubject> query, string propertyName)
            => Conventions<TSubject>.Sort(query, propertyName);
    }
    
}
