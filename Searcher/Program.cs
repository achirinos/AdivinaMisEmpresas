using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Searcher
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Entra Cedula");
            //var cedula = Console.ReadLine();

            string url = "http://tioconejo.net/rnc/index.php";
            string param = "id={0}&tipo=C";


            string list = "8182889,6147157,7547310,7068393,7593723,7558225,6862464,7582832,6211322,8998325,8969886,9878087,6739587,6009672,7241562,9247849,7512508,7369867,10112295,6208859,9620330,9610596,9901457,8845198,10082048,9564266,7394561,8349104,10492193,9615774,94834141,9524118,9817944,8037327,8748227,741564,7566230,7110840,9998924,9597036,9987074,7223700,10378955,6299831,10798308,9378568,9576581,10095571,14352725,10382105,10498433,6213021,10383386,8758571,8158571,6721436,9683039,10041768,7993988,7993988,6517251,6976944,8102978,9415245,8638434,9291366,9275245,6110379,9695924,5912240,9284667,9975032,9975032,8828297,6519452,6519452,7997027,8820720,8820720,6330773,6330773,8970915,11514154,10734599,10734599,10035921,11155130,12510852,10054994,9416305,9860708,9665686,10491624,11117351,11838084,8267515,11906677,9870564,9970564,10865818,10369647,13510445,12572637,12572637,13582164,13773875,12259285,12025871,12724534,11893574,13127356,11772194,14892378,11478970,11865179,14496207,12237992,14349691,14349691,12607781,13535365,13324134,14469029,13078275,11470,12339155,13627358,12339155,11879,11647573,,12309024,13900250,8968668,,9647168,13597609,14390273,13908001,13849140,14996038,14371829,14312871,16393748,16233077,12752933,9537423,7943358,12769060,8498373,11913,18886379,17228246,22123075,19172542,10366635,10366635,13455249,16093810,26924002,16,19040610,19029057,23586156,17,20651997,16363536,9261152,20467743,13978229,15427546,18081114,19816517,26075326,25285406,14309558,16515,27485926,10285574,5408651,18047169,13117108,1944553,30259996,15639184,9064665,22336391,17450705,14287281,25417958,11734,17254400,14602252,14194411,14194411,20952530,17629436,18547013";

            var array = list.Split(',');


            foreach (var item in array)
            {
                string result = item.Replace(@".", "");
                Console.WriteLine(string.Format("cedula: {0}", result));
                HttpPost(url, string.Format(param, result));
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }


        public static string HttpGet(string URI)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public static string HttpPost(string URI, string Parameters)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            //Add these, as we're doing a POST
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());

            string htmlData = sr.ReadToEnd().Trim();

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(htmlData);


            var query = doc.DocumentNode.SelectNodes("//table")[0].SelectSingleNode("tbody/tr/td").SelectNodes("input[@name='id']");
            if (query != null)
            {
                var rifList = query.Select(i => i.GetAttributeValue("value", "")).ToList();

                foreach (var item in rifList)
                {
                    string template = "http://rncenlinea.snc.gob.ve/reportes/resultado_busqueda?p=1&rif={0}&search=RIF";

                    var result = HttpGet(string.Format(template, item));

                    doc.LoadHtml(result);

                    var link = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[2]/td[1]/table[1]").SelectNodes("tr[2]/td/a")[0].GetAttributeValue("href", "");
                    var rif = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[2]/td[1]/table[1]").SelectNodes("tr[2]/td")[0].InnerText.Trim();
                    var empresa = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[2]/td[1]/table[1]").SelectNodes("tr[2]/td")[1].InnerText.Trim();
                    var estatus = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[2]/td[1]/table[1]").SelectNodes("tr[2]/td")[2].InnerText.Trim();

                    var detalleUrl = "http://rncenlinea.snc.gob.ve/{0}";

                    var detalle = HttpGet(string.Format(detalleUrl, link));

                    doc.LoadHtml(detalle);

                    //var fechaRegistro = doc.DocumentNode.SelectSingleNode("/html/body/table/tbody/tr[2]/td/table/tbody/tr[8]/td/table/tbody/tr[3]/td[6]");

                    var rs = doc.DocumentNode.SelectNodes("//td");
                    var fechaRegistro = string.Empty;
                    foreach (var r in rs)
                    {
                        if (r.InnerText.Trim() == "Acta Constitutiva y Modificaciones Estatutarias")
                        {
                            fechaRegistro = r.ParentNode.ParentNode.SelectNodes("tr[3]/td")[5].InnerText.Trim();
                        }
                    }

                    Console.WriteLine(string.Format("{0} {1} Fecha de Registro: {2}", rif, empresa, fechaRegistro));
                }
            }
            

            return string.Empty;


        }
    }
}
