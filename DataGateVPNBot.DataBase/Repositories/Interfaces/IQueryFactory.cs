namespace DataGateVPNBot.DataBase.Repositories.Interfaces;

public interface IQueryFactory
{
    IQuery<T> GetQuery<T>() where T : class;
}