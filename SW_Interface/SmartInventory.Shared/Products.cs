using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Shared
{
    public class Products
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string StringId { get; set; }


        [BsonElement("Nombre")]
        public string Nombre { get; set; }

        [BsonElement("Descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("Precio")]
        public decimal Precio { get; set; }

        [BsonElement("Stock")]
        public int Stock { get; set; }

        [BsonElement("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
