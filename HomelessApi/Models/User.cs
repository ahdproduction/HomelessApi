using System.ComponentModel.DataAnnotations;
using HomelessApi.Security;
using MongoDB.Bson.Serialization.Attributes;

namespace HomelessApi.Models
{
	[BsonIgnoreExtraElements]
    public class User : IdentityUser
    {
       
        [Required]
        [EmailAddress]
        public override string Email { get; set; }

        [BsonIgnore]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}