using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Chinook.Model
{
    public static class ObjectQueryExtensions
    {
        /// <summary>
        /// Type safe include from here: http://blogs.microsoft.co.il/blogs/shimmy/archive/2010/08/06/say-goodbye-to-the-hard-coded-objectquery-t-include-calls.aspx
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ObjectQuery<TSource> Include<TSource, TResult>(this ObjectQuery<TSource> query, Expression<Func<TSource, TResult>> path)
        {
            if (query == null) throw new ArgumentException("query");
            var properties = new List<string>();
            Action<string> add = (str) => properties.Insert(0, str);
            var expression = path.Body;
            do
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var member = (MemberExpression)expression;
                        if (member.Member.MemberType != MemberTypes.Property)
                            throw new ArgumentException("The selected member must be a property.", "selector");
                        add(member.Member.Name);
                        expression = member.Expression;
                        break;
                    case ExpressionType.Call:
                        var method = (MethodCallExpression)expression;
                        if (method.Method.Name != SingleMethodName || method.Method.DeclaringType != EnumerableType)
                            throw new ArgumentException(string.Format("Method '{0}' is not supported, only method '{1}' is supported to singularize navigation properties.", string.Join(Type.Delimiter.ToString(), method.Method.DeclaringType.FullName, method.Method.Name), string.Join(Type.Delimiter.ToString(), EnumerableType.FullName, SingleMethodName)), "selector");
                        expression = (MemberExpression)method.Arguments.Single();
                        break;
                    default:
                        throw new ArgumentException("The property selector expression has an incorrect format.", "selector", new FormatException());
                }
            }
            while (expression.NodeType != ExpressionType.Parameter);
            return query.Include(string.Join(Type.Delimiter.ToString(), properties));
        }
        private static readonly Type EnumerableType = typeof(Enumerable);
        private const string SingleMethodName = "Single";


        //Below is from http://landman-code.blogspot.com/2008/11/linq-to-entities-string-based-dynamic.html

        #region Private expression tree helpers

        private static LambdaExpression GenerateSelector<TEntity>(String propertyName, out Type resultType) where TEntity : class
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                String[] childProperties = propertyName.Split('.');
                property = typeof(TEntity).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (int i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(TEntity).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }
        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, String fieldName) where TEntity : class
        {
            Type type = typeof(TEntity);
            Type selectorResultType;
            LambdaExpression selector = GenerateSelector<TEntity>(fieldName, out selectorResultType);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
                            new Type[] { type, selectorResultType },
                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }
        #endregion
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "ThenBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            MethodCallExpression resultExp = GenerateMethodCall<TEntity>(source, "ThenByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }


        /// <summary>
        /// Accepts: "Employee.LastName, Freight DESC"
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderUsingSortExpression<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            String[] orderFields = sortExpression.Split(',');
            IOrderedQueryable<TEntity> result = null;
            for (int currentFieldIndex = 0; currentFieldIndex < orderFields.Length; currentFieldIndex++)
            {
                String[] expressionPart = orderFields[currentFieldIndex].Trim().Split(' ');
                String sortField = expressionPart[0];
                Boolean sortDescending = (expressionPart.Length == 2) && (expressionPart[1].Equals("DESC", StringComparison.OrdinalIgnoreCase));
                if (sortDescending)
                {
                    result = currentFieldIndex == 0 ? source.OrderByDescending(sortField) : result.ThenByDescending(sortField);
                }
                else
                {
                    result = currentFieldIndex == 0 ? source.OrderBy(sortField) : result.ThenBy(sortField);
                }
            }
            return result;
        }

    }
}
