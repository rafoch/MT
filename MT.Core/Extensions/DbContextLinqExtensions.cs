using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MT.Core.Extensions
{
    public static class DbContextLinqExtensions
    {
        public static IQueryable<TEntity> Filter<TEntity, TProperty>(this IQueryable<TEntity> dbSet,
            Expression<Func<TEntity, TProperty>> property,
            TProperty value)
        {

            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {
                throw new ArgumentException("Property expected", "property");
            }

            Expression left = property.Body;
            Expression right = Expression.Constant(value, typeof(TProperty));

            Expression searchExpression = Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(left, right),
                new ParameterExpression[] { property.Parameters.Single() });

            return dbSet.Where(lambda);
        }
    }
}