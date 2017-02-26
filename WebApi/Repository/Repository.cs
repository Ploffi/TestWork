using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Data;

namespace WebApi.Repository
{
    public class Repository<TEntity> 
    {
        protected List<TEntity> GetOrInsertByTCreator<TCreatorFrom>(IEnumerable<TCreatorFrom> entitysFrom,
            Func<TCreatorFrom, TEntity> creator,
            Action<TCreatorFrom,TEntity> updater,
            Func<TCreatorFrom,Query> qeuryCreator, 
            string entityCollectionName,
            string dbConfig  )
        {
            List<TEntity> entityList;
            using (var db = new LiteDatabase(dbConfig))
            {
                var entityCollection = db.GetCollection<TEntity>(entityCollectionName);
                entityList = entitysFrom.Select(entityFrom =>
                {
                    var exist = entityCollection.FindOne(qeuryCreator(entityFrom));
                    if (exist == null)
                    {
                        exist = creator(entityFrom);
                        entityCollection.Insert(exist);
                    }
                    else
                    {
                        updater(entityFrom,exist);
                        entityCollection.Update(exist);
                    }
                    return exist;
                }).ToList();
            }
            return entityList;
        }
    }
}
