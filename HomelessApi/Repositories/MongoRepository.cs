using System;
using System.Linq;
using HomelessApi.Models;
using MongoDB.Driver;

namespace HomelessApi.Repositories
{
	public class MongoRepository<TDocument> : IRepository<TDocument> where TDocument : MongoDocument
    {
        public IMongoCollection<TDocument> Collection { get; }

        public MongoRepository(IMongoDatabase db, string collectionName)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            Collection = db.GetCollection<TDocument>(collectionName);
        }

        public IQueryable<TDocument> Query()
        {
            return Collection.AsQueryable();
        }
    }
}