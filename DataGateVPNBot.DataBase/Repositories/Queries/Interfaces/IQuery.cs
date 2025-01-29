namespace DataGateVPNBot.DataBase.Repositories.Interfaces;

public interface IQuery<T> where T : class
{
    // IQueryable<T> AsQueryable { get; }
    IQueryable<T> AsQueryable();
}