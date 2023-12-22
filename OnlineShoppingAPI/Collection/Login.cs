using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace OnlineShoppingAPI.Collection
{
    public class Login
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [Required]
        [BsonElement("Loginid")]
        public string? Loginid { get; set; }
        [Required]
        [BsonElement("Password")]
        public string? Password { get; set; }
        [BsonDefaultValue("User")]
        [BsonElement("Role")]
        public string? Role { get; set; }
        public Login()
        {
            Role = "User";
        }
    }
}
