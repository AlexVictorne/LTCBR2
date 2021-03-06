﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using LTCBR2.Keeper;
using LTCBR2.Types.ForOntology;

namespace LTCBR2.WebApi.Controllers
{
    public class OntologyController : ApiController
    {
        public async Task<IEnumerable<AbstractClass>> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                //return;
            }
            
            var provider = new MultipartMemoryStreamProvider();
            // путь к папке на сервере
            string root = HttpContext.Current.Server.MapPath("~/App_Data/");
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var owlString = file.ReadAsStringAsync();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(owlString.Result);
                OwlWorker.LoadIndividuals(xml);
                var result = OwlWorker.LoadOntologyModel(xml).ToList();
                return result;
                //byte[] fileArray = await file.ReadAsByteArrayAsync();

                //using (System.IO.FileStream fs = new System.IO.FileStream(root + filename, System.IO.FileMode.Create))
                //{
                //    await fs.WriteAsync(fileArray, 0, fileArray.Length);
                //}
            }
            return null;
            //return Ok("Read success");
        }
    }
}
