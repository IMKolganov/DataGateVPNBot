namespace DataGateVPNBot.DataBase.Repositories.Interfaces;

public interface IRepositoryFactory
{
    IRepository<T> GetRepository<T>() where T : class;
}