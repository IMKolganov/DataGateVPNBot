using System.Collections.Concurrent;
using DataGateVPNBot.DataBase.Contexts;
using DataGateVPNBot.DataBase.Repositories.Interfaces;

namespace DataGateVPNBot.DataBase.Repositories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly ApplicationDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public RepositoryFactory(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));
    }
}