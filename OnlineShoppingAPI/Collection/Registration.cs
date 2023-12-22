using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineShopping.Collection
{
    public class Registration
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        [Required]
        [BsonElement("Fname")]
        public string? Fname { get; set; }
        [Required]
        [BsonElement("Lname")]
        public string? Lname { get; set; }
        [Required]
        [BsonElement("Email")]
        public string? Email { get; set; }
        [Required]
        [BsonElement("Loginid")]
        public string? Loginid { get; set; }
        [Required]
        [BsonElement("Password")]
        public string? Password { get; set; }
        [Required]
        [BsonElement("Confirmpassword")]
        public string? Confirmpassword { get; set; }
        [Required]
        [BsonElement("Contactno")]
        public int Contactno { get; set; }
        [BsonDefaultValue("User")]
        [BsonElement("Role")]
        public string? Role { get; set; }
        public Registration()
        {
            Role = "User";
        }


    }
}
