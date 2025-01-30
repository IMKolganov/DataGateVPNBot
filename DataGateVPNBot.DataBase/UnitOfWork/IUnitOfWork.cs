using DataGateVPNBot.DataBase.Repositories.Interfaces;

namespace DataGateVPNBot.DataBase.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> GetRepository<T>() where T : class;
    IQuery<T> GetQuery<T>() where T : class;
    Task<int> SaveChangesAsync();
    void SaveChanges();
}