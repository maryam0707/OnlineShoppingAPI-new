using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace OnlineShoppingAPI.Collection
{
    public class Orders
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [Required]
        public int OrderId { get; set; }
        
        [BsonElement("ProductId")]
        public int ProductId { get; set; }
        [Required]
        [BsonElement("ProductName")]
        public string ProductName { get; set; }
        [Required]
        [BsonElement("Loginid")]
        public string? Loginid { get; set; }
    }
}
