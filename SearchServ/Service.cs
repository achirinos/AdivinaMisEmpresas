using HtmlAgilityPack;
using SearchServ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService
{
    public class Service
    {
        public ICollection<string> GetRifList(string cedula)
        {
            string url = "http://tioconejo.net/rnc/index.php";
            string param = string.Format("id={0}&tipo=C", cedula);

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
           
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
           
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(param);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); 
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
               return  query.Select(i => i.GetAttributeValue("value", "")).ToList();
            }

            return new List<string>();        
        }

        public CompanyInfo GetCompanyName(string rif)
        {
            string template = "http://rncenlinea.snc.gob.ve/reportes/resultado_busqueda?p=1&rif={0}&search=RIF";
            
            System.Net.WebRequest req = System.Net.WebRequest.Create(string.Format(template, rif));
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            var result = sr.ReadToEnd().Trim();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);

            var _rif = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[2]/td[1]/table[1]").SelectNodes("tr[2]/td")[0].InnerText.Trim();
            var nombreEmpresa = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[2]/td[1]/table[1]").SelectNodes("tr[2]/td")[1].InnerText.Trim();

            return new CompanyInfo { Rif = _rif, Name = nombreEmpresa };
        }


    }
}
