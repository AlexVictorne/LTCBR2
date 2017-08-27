using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LTCBR2.Types;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LTCBR2.Keeper
{
    public class NoSqlWorker
    {
        #region Initialization

        private string _connectionString = "";
        private string _databaseName = "LTCBR";

        private IMongoClient _client;
        private IMongoDatabase _database;

        public void Initialization()
        {
            this._client = new MongoClient();
            this._database = _client.GetDatabase(this._databaseName);
        }

        #endregion

        #region Commands


        public void Insert(Situation situation)
        {
            var collection = this._database.GetCollection<Situation>("Situations");
            collection.InsertOne(situation);
        }

        public void Delete()
        {
            var collection = _database.GetCollection<Situation>("Situations");
            var filter = Builders<Situation>.Filter.Eq("adress0", "10");
            var result = collection.DeleteMany(filter);
        }

        public List<Situation> Select()
        {
            var sitList = new List<Situation>();
            var collection = this._database.GetCollection<Situation>("Situations");
            var regex = new BsonRegularExpression("^d*");
            var filter = new BsonDocument();
            using (var cursor = collection.FindSync(filter))
            {
                while (cursor.MoveNext())
                {
                    sitList.AddRange(cursor.Current);
                }
            }
            return sitList;
        }

        public async Task<List<Situation>> SelectByFilter(string fieldName, string valueName, string fieldType,
            string valueType, string fieldDate, DateTime valueDate)
        {
            //var sitList = new List<Situation>();
            var collection = this._database.GetCollection<Situation>("Situations");
            var filter = Builders<Situation>.Filter.Empty;
            if ((fieldName != "") && (valueName != ""))
                filter = Builders<Situation>.Filter.Eq(fieldName, valueName);
            if ((fieldType != "") && (valueType != ""))
                filter = filter & Builders<Situation>.Filter.Eq(fieldType, valueType);
            if (fieldDate != "")
                filter = filter & Builders<Situation>.Filter.Eq(fieldDate, valueDate);

            var resultList = await collection.Find(filter).ToListAsync();
            return resultList;
        }

        #endregion


        public Situation GetSituation(int id)
        {
            Initialization();
            var sl = Select();
            return
                sl.First(
                    x =>
                        x.id == id);
        }

    }
}
