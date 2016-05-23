using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LTCBR2.Keeper;
using LTCBR2.Types;
using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.WebApi.Controllers
{
    public class SituationsController : ApiController
    {
        public class liteBundleElement
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
        //get bundle by filter
        public IEnumerable<liteBundleElement> GetAllSituations()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var bundle = noSqlWorker.Select();
            var liteBundle = bundle.Select(situation => new liteBundleElement
            {
                id = situation.id,
                name = situation.name,
                type = situation.type,
                date = situation.create_date.ToShortDateString()
            }).ToList();
            return liteBundle;
        }
        
        //remove from bundle by id

        //get situation by id
        public IHttpActionResult GetSituation(int id)
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(id);
            if (selectedSituation == null)
            {
                return NotFound();
            }
            return Ok(selectedSituation);
        }

        //save to bundle
        public HttpResponseMessage Post([FromBody]Situation situation)
        {
            var newSituation = situation;
            newSituation.create_date = DateTime.Today;
            newSituation.id = newSituation.GetHashCode();
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            noSqlWorker.Insert(newSituation);
            return Request.CreateResponse(HttpStatusCode.OK,newSituation.id);
        }
        /*public void SaveSituation()
        {
            var newSituation = JsonConvert.DeserializeObject<Situation>(situationInJson);
            newSituation.create_date = DateTime.Today;
            newSituation.id = newSituation.GetHashCode();
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            noSqlWorker.Insert(newSituation);
            return new JavaScriptSerializer().Serialize(newSituation);
        }*/
        
    }
}
