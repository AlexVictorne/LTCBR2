using System;
using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.Types
{
    [Serializable]
    public class Attribute
    {
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public string value { get; set; }
    }
}
