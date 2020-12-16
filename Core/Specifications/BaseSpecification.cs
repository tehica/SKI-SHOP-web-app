using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        // use this ctor when we want to get all products (List)
        public BaseSpecification()
        {

        }

        // use this ctor what we want to get a product with specific id
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        // OrderBy ASC and DESC
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        // three properties for pagination
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }



        #region Order By ASC / DESC methods
        // Order by ASC
        protected void AddOrderBy(Expression<Func<T, object>> orderByExperssion)
        {
            OrderBy = orderByExperssion;
        }
        // Order by DESC
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExperssion)
        {
            OrderByDescending = orderByDescExperssion;
        }
        #endregion




        #region Pagination methods
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
        #endregion
    }
}
