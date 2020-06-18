using System;
using Ghoul.Data.Interface;
using Ghoul.Repository.Interface;
using Ghould.Database.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ghoul.Repository.Mongo
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

        public void Remove(Guid ID)
        {
            _context.GetCollection<T>()
                .DeleteOne(Builders<T>.Filter.Eq(x => x.ID, ID));
        }
    }
}