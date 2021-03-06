﻿using Core.Entities;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        // method for count number of items
        Task<int> CountAsync(ISpecification<T> spec);

        // additional methods to support updating
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
