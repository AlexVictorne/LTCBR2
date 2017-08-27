using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LTCBR2.Keeper;
using LTCBR2.Searcher;
using LTCBR2.Types;
using MongoDB.Bson.Serialization.Attributes;

namespace LTCBR2.WebApi.Controllers
{
    public class SearchController : ApiController
    {
        public class LiteBundleElement
        {
            [BsonElement]
            public int id { get; set; }
            [BsonElement]
            public string name { get; set; }
            [BsonElement]
            public string type { get; set; }
            [BsonElement]
            public double rate { get; set; }
        }
        //get bundle by filter
        //search start
        public HttpResponseMessage Post([FromBody]Situation situationIn)
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var bundle = noSqlWorker.Select();
            var ss = new SituationSearcher {SituationsInBase = bundle};
            ss.SearchStart(situationIn);
            bundle = ss.SituationsInBase;
            var liteBundle = bundle.Select(situation => new LiteBundleElement
            {
                id = situation.id,
                name = situation.name,
                type = situation.type,
                rate = situation.rate
            }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, liteBundle);
        }

        public IHttpActionResult GetAllSituations(Situation situationIn)
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var bundle = noSqlWorker.Select();
            SituationSearcher ss = new SituationSearcher();
            ss.SituationsInBase = bundle;
            ss.SearchStart(situationIn);
            var liteBundle = bundle.Select(situation => new LiteBundleElement
            {
                id = situation.id,
                name = situation.name,
                type = situation.type,
                rate = situation.rate
            }).ToList();
            return Ok(liteBundle);
        }

        public IHttpActionResult GetListOfDevices()
        {
            //var gw = new GpuWorker();
            return Ok(0);//gw.GetListOfDevices());
        }

        [HttpPost]
        public HttpResponseMessage SetDevice([FromBody]string deviceName)
        {
            //var gw = new GpuWorker();
            //gw.SetDevice(deviceName);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        
    }
}
