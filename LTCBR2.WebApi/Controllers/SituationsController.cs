using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using LTCBR2.Keeper;
using LTCBR2.Types;
using LTCBR2.Utils;
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
        [HttpPost]
        public async Task<IEnumerable<liteBundleElement>> GetAllSituations([FromBody]Filter filter)
        {
            List<Situation> bundle;
           
            if (filter.sourceType)
            {
                bundle = OwlWorker.CurrentSituationBaseFromOwl;
            }
            else
            {
                var noSqlWorker = new NoSqlWorker();
                noSqlWorker.Initialization();
                var name = filter.name != "" ? "name" : "";
                var type = filter.type != "" ? "type" : "";
                var date = filter.date != "" ? "create_date" : "";
                DateTime dateIn;
                if (DateTime.TryParse(filter.date, out dateIn))
                {
                    DateTime.TryParse(filter.date, out dateIn);
                    bundle = await noSqlWorker.SelectByFilter(name, filter.name, type, filter.type, date, dateIn);
                }
                else
                {
                    bundle = await noSqlWorker.SelectByFilter(name, filter.name, type, filter.type, date,DateTime.Now);
                }
            }
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
        public class SourceAndId
        {
            public bool sourceType { get; set; }
            public int id { get; set; }
        }
        [HttpPost]
        public IHttpActionResult GetSituation([FromBody]SourceAndId sai)
        {
            Situation selectedSituation;
            if (sai.sourceType)
            {
                selectedSituation = OwlWorker.GetSituation(sai.id);
                var gw = new GraphWorker();

                selectedSituation = gw.FillCoordinates(selectedSituation, gw.MakeGraph(gw.SituationToGraph(selectedSituation.participants)));
            }
            else
            {
                var noSqlWorker = new NoSqlWorker();
                noSqlWorker.Initialization();
                selectedSituation = noSqlWorker.GetSituation(sai.id);
                if ((selectedSituation.coordinates==null)||(selectedSituation.coordinates.Count == 0))
                {
                    selectedSituation.coordinates = new List<Coordinate>();
                    var gw = new GraphWorker();
                    selectedSituation = gw.FillCoordinates(selectedSituation, gw.MakeGraph(gw.SituationToGraph(selectedSituation.participants)));
                }
                    
            }
            
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
            newSituation = Tools.ValidateSituation(newSituation);
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            noSqlWorker.Insert(newSituation);
            return Request.CreateResponse(HttpStatusCode.OK,newSituation.id);
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostFromConstructor()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }

            var provider = new MultipartMemoryStreamProvider();
            // путь к папке на сервере
            string root = HttpContext.Current.Server.MapPath("~/App_Data/");
            await Request.Content.ReadAsMultipartAsync(provider);
            Situation newSit = null;
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var owlString = file.ReadAsStringAsync();
                ImportFromConstructor ifc = new ImportFromConstructor();
                newSit = ifc.load_pps(owlString.Result);
                newSit.create_date = DateTime.Today;
                newSit.coordinates = new List<Coordinate>();
                var gw = new GraphWorker();
                newSit = gw.FillCoordinates(newSit, gw.MakeGraph(gw.SituationToGraph(newSit.participants)));
                var noSqlWorker = new NoSqlWorker();
                noSqlWorker.Initialization();
                noSqlWorker.Insert(newSit);
                //byte[] fileArray = await file.ReadAsByteArrayAsync();

                //using (System.IO.FileStream fs = new System.IO.FileStream(root + filename, System.IO.FileMode.Create))
                //{
                //    await fs.WriteAsync(fileArray, 0, fileArray.Length);
                //}
            }
            if (newSit == null)
            {
                return NotFound();
            }
            return Ok(newSit);
        }


        public class Filter
        {
            public string name { get; set; }
            public string type { get; set; }
            public string date { get; set; }
            public bool sourceType { get; set; } //false - db, true - ontology
        }

        Filter currentFilter = new Filter();

        public HttpResponseMessage SetFilter([FromBody]Filter filter)
        {
            currentFilter = filter;
            return Request.CreateResponse(HttpStatusCode.OK);
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
