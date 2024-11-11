using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Shared
{
    public class Usuarios
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonIgnore]
        public string StringId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Activo")]
        public bool Activo { get; set; }

        [BsonElement("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
