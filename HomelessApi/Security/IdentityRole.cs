using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomelessApi.Security
{
    public class IdentityRole
    {
        public IdentityRole()
		{
			Id = ObjectId.GenerateNewId().ToString();
		}

		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
        
		public string Name { get; set; }

		public string NormalizedName { get; set; }

		public override string ToString() => Name;
	}
}