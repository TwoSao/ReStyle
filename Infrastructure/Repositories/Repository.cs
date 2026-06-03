using Microsoft.EntityFrameworkCore;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ReStyleDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ReStyleDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public Task UpdateAsync(T entity) { _dbSet.Update(entity); return Task.CompletedTask; }
    public Task DeleteAsync(T entity) { _dbSet.Remove(entity); return Task.CompletedTask; }
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
