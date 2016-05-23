using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using LTCBR2.Keeper;
using Newtonsoft.Json;
using LTCBR2.Types;
using LTCBR2.Utils;
using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2
{
    /// <summary>
    /// Summary description for LTCBR
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Ltcbr : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveSituation(string situationInJson)
        {
            var newSituation = JsonConvert.DeserializeObject<Situation>(situationInJson);
            newSituation.create_date = DateTime.Today;
            newSituation.id = newSituation.GetHashCode();
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            noSqlWorker.Insert(newSituation);
            return new JavaScriptSerializer().Serialize(newSituation);
        }

        class liteBundleElement
        {
            [BsonElement]
            public int id { get; set; }
            [BsonElement]
            public string name { get; set; }
            [BsonElement]
            public string type { get; set; }
            [BsonElement]
            public string date { get; set; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSituationBundle(string filter)
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var bundle = noSqlWorker.Select();
            var liteBundle = bundle.Select(situation => new liteBundleElement
            {
                id = situation.id, name = situation.name, type = situation.type, date = situation.create_date.ToShortDateString()
            }).ToList();
            return new JavaScriptSerializer().Serialize(liteBundle);
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOneSituation(string idInJson)
        {
            var id = JsonConvert.DeserializeObject<int>(idInJson);
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(id);
            return new JavaScriptSerializer().Serialize(selectedSituation);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOneSituationNewCoordinate(string idInJson)
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();

            var id = JsonConvert.DeserializeObject<int>(idInJson);
            var selectedSituation = noSqlWorker.GetSituation(id);
            //graph
            var gw = new GraphWorker();
            var output = gw.SituationToGraph(selectedSituation.participants);
            var outputWithCoordinates = gw.MakeGraph(output);
            var outputSituation = gw.FillCoordinates(selectedSituation, outputWithCoordinates);
            return new JavaScriptSerializer().Serialize(outputSituation);
        }
    }
}
