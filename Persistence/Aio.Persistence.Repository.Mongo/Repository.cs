using System;
using Aio.Persistence.Model.Interface;
using Aio.Persistence.Repository.Interface;
using Aio.Persistence.Database.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aio.Persistence.Repository.Mongo
{
    public class Repository<T> : ReadRepository<T>, IRepository<T>, IMongoRepository<T>
        where T: class, IPrimaryKeyEntity
    {
        private readonly MongoDBContext _context;
        public Repository(MongoDBContext context) : base(context)
        {
            _context = context;
        }

        public void Insert(T entity)
        {
            _context.GetCollection<T>()
                .InsertOne(entity);
        }

        public void Update(T entity)
        {
            _context.GetCollection<T>()
                .ReplaceOne(Builders<T>.Filter.Eq(x => x.ID, entity.ID), entity);
        }

        public void Remove(T entity)
        {
            _context.GetCollection<T>()
                .DeleteOne(Builders<T>.Filter.Eq(x => x.ID, entity.ID));
        }

        public void Remove(string ID)
        {
            _context.GetCollection<T>()
                .DeleteOne(Builders<T>.Filter.Eq(x => x.ID, ID));
        }
    }
}