using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.Types
{
    public class Situation
    {
        [BsonElement]
        public int id { get; set; }
        [BsonElement]
        public DateTime create_date { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public string type { get; set; }
        [BsonElement]
        public string description { get; set; }
        [BsonElement]
        public string solution { get; set; }
        [BsonElement]
        public double rate { get; set; }


        [BsonElement]
        public List<Coordinate> coordinates;
        [BsonElement]
        public List<Participant> participants;
    }
}
