using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.Types
{
    public class Attribute
    {
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public string value { get; set; }
    }
}
