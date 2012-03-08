using System;
using System.Collections.Generic;
using PetaPoco;
using RequestReduce.Configuration;
using System.Linq;
using System.Configuration;
using RequestReduce.SqlServer.ORM;

namespace RequestReduce.SqlServer
{
    public interface IRepository : IDisposable
    {
        T SingleOrDefault<T>(object primaryKey);
        IEnumerable<T> Query<T>();
        IQueryable<T> AsQueryable<T>();
        List<T> Fetch<T>();
        int Insert(object itemToAdd);
        int Delete<T>(object primaryKeyValue);
    }
    public class Repository : IRepository
    {
        private readonly RequestReduceDB db;

        public Repository(IRRConfiguration config)
        {
            db = IsConnectionStringName(config.ConnectionStringName)
                     ? new RequestReduceDB(config.ConnectionStringName)
                     : new RequestReduceDB(config.ConnectionStringName, RequestReduceDB.DefaultProviderName);
            db.OpenSharedConnection();
        }

        public RequestReduceDB GetDatabase()
        {
            return db;
        }

        private bool IsConnectionStringName(string connectionStringName)
        {
            if (ConfigurationManager.ConnectionStrings == null)
            {
                return false;
            }

            return (ConfigurationManager.ConnectionStrings[connectionStringName] != null);
        }

        public TPassType SingleOrDefault<TPassType>(object primaryKey)
        {
            return db.SingleOrDefault<TPassType>(primaryKey);
        }
        public IEnumerable<TPassType> Query<TPassType>()
        {
            var pd = Database.PocoData.ForType(typeof(TPassType));
            var sql = "SELECT * FROM " + pd.TableInfo.TableName;
            return db.Query<TPassType>(sql);
        }
        public IQueryable<TPassType> AsQueryable<TPassType>()
        {
            return Query<TPassType>().AsQueryable<TPassType>();
        }
        public List<TPassType> Fetch<TPassType>()
        {
            var pd = Database.PocoData.ForType(typeof(TPassType));
            var sql = "SELECT * FROM " + pd.TableInfo.TableName;
            return db.Fetch<TPassType>(sql);
        }
        public int Insert(object poco)
        {
            return Convert.ToInt32(db.Insert(poco));
        }
        public int Update(object poco)
        {
            return db.Update(poco);
        }
        public int Delete<TPassType>(object pocoOrPrimaryKey)
        {
            return db.Delete<TPassType>(pocoOrPrimaryKey);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}