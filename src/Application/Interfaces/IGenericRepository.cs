using System;
using System.Linq.Expressions;

namespace admin_service.Application.Common.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, 
        CancellationToken cancellationToken = default);
    
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
}