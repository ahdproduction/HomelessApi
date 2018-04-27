using HomelessApi.Models;
using System.Linq;

namespace HomelessApi.Repositories
{
	public interface IRepository<TDocument> where TDocument : MongoDocument
    {
		IQueryable<TDocument> Query();
    }
}
