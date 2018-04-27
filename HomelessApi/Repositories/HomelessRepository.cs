using System.Collections.Generic;
using HomelessApi.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
namespace HomelessApi.Repositories
{
	public class HomelessRepository : MongoRepository<CustomPin>
    {
		public HomelessRepository( IMongoDatabase db) : base(db, "homeless")
        {
        }

        public IEnumerable<CustomPin> GetAll()
        {
            return Query();
        }

        public async Task<CustomPin> GetOneAsync(string Id)
        {
            var filter = Builders<CustomPin>.Filter.Eq(nameof(CustomPin.Id), Id);
            return await Collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(CustomPin customPin)
        {
            await Collection.InsertOneAsync(customPin);
        }

        public async Task<CustomPin> UpdateAsync(CustomPin customPin)
        {
            var filter = Builders<CustomPin>.Filter.Eq(c => c.Id, customPin.Id);
            var options = new FindOneAndReplaceOptions<CustomPin>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await Collection.FindOneAndReplaceAsync(filter, customPin, options);
        }

        public async Task<CustomPin> RemoveAsync(string Id)
        {
            var filter = Builders<CustomPin>.Filter.Eq(nameof(CustomPin.Id), Id);
            return await Collection.FindOneAndDeleteAsync(filter, new FindOneAndDeleteOptions<CustomPin, CustomPin>());
        }
    }
}
