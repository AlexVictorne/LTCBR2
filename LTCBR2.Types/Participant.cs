using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.Types
{
    public class Participant
    {
        [BsonElement]
        public int id { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public string purpose { get; set; }
        [BsonElement]
        public string className { get; set; }
        [BsonElement]
        public List<Attribute> attributes;
        [BsonElement]
        public List<int> connections;

        public override string ToString()
        {
            return name.ToString();
        }
    }
}
