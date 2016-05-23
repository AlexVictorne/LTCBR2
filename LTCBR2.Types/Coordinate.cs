using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.Types
{
    public class Coordinate
    {
        [BsonElement]
        public int id { get; set; }
        [BsonElement]
        public double x { get; set; }
        [BsonElement]
        public double y { get; set; }
    }
}
