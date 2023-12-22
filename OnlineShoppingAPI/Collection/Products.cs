using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShoppingAPI.Collection
{
    public class Products
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [BsonElement("ProductName")]
        public string ProductName { get; set; }
        [Required]
        [BsonElement("ProductDescription")]
        public string ProductDescription { get; set; }
        [Required]
        [BsonElement("Price")]
        public double? Price { get; set; }
        [Required]
        [BsonElement("Features")]
        public string Features { get; set; }
        [Required]
        [BsonElement("ProductStatus")]
        public string ProductStatus { get; set; }
        [Required]
        [BsonElement("StockCount")]
        public int StockCount { get; set; }
    }
}
