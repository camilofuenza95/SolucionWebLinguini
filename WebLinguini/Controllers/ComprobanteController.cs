using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebLinguini.Models;
using WebLinguini.Models.DTO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using WebLinguini.Models.ViewModel;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebLinguini.Controllers
{
    public class ComprobanteController : Controller
    {
        private ApiRestful comprobanteApiClient = new ApiRestful();
        static HttpClient client = new HttpClient();


        // GET: Comprobante
        public ActionResult Listar()
        {
            List<Comprobante> model = comprobanteApiClient.listarComprobantes();

            if(model == null)
            {
                ViewBag.mensaje = "No se ha podido listar los comprobantes";
            }
            else
            {
                ViewBag.data = model;
            }
            

            return View();
        }

        #region Formularios
        public ActionResult FormAgregar()
        {
            return View(new Comprobante());
        }

        public ActionResult FormBuscar()
        {
            return View(new ComprobanteViewModel());
        }
        #endregion

        #region Agregar
        [HttpPost]

        public async Task<ActionResult> Agregar(Comprobante c)
        {
            try
            {
                //ViewBag.Result1 = categoriaApiClienat.categorias();

                var result = await agregarComprobante(c);

                List<Comprobante> model = comprobanteApiClient.listarComprobantes();

                ViewBag.data = model;

                return View("Listar");
            }
            catch
            {
                return RedirectToAction("Error");
            }

        }

        public static async Task<Uri> agregarComprobante(Comprobante c)
        {
            var url = "http://localhost:8034/api/comprobante/agregarComprobante/";
            HttpResponseMessage response = await client.PostAsJsonAsync(url, c);

            return response.Headers.Location;
        }
        #endregion

        #region Buscar
        [HttpPost]
        public ActionResult Buscar(ComprobanteViewModel c)
        {
            try
            {
                //ViewBag.Result1 = categoriaApiClienat.categorias();
                DetalleComprobante dc = new DetalleComprobante();
                dc.idComprobante = c.idComprobante;

                List<DetalleComprobante> model = comprobanteApiClient.buscarDetalleComprobante(dc);

                if (model == null)
                {
                    ViewBag.error = "si";
                    ViewBag.error2 = "No se ha encontrado el comprobante.";
                }
                else
                {
                    ViewBag.error = "no";
                    ViewBag.data = model;
                }


                return View();
            }
            catch
            {
                return RedirectToAction("Error");
            }

        }


        #endregion

        #region CalcularGanancias
        [HttpPost]

        public ActionResult CalcularGanancias(PedProvViewModel c)
        {

            //ViewBag.Result1 = categoriaApiClienat.categorias();
            string fecha = DateTime.Now.ToString("dd/MM/yyyy");

            int result = calcularGanancias();

            List<Comprobante> model = comprobanteApiClient.listarComprobantes();

            ViewBag.data = model;
            if(result.Equals(""))
            {
                ViewBag.mensaje = "No se han obtenido ganancias el día de hoy";
            }
            else
            {
                ViewBag.ok = "Las ganancias del día de hoy son: $" + result;
            }
            

            return View("Listar", new Comprobante());
           

        }

        public static int calcularGanancias()
        {


            client = null;
            client = new HttpClient();
            string fecha = DateTime.Now.ToString("dd/MM/yyyy");
            Ganancia cm = new Ganancia();
            int ganancia = 0;

            var url = "http://localhost:8034/api/comprobante/" + fecha + "/calcularGanancias";


            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            /*HttpResponseMessage response = client.GetAsync(url).Result;*/

            HttpResponseMessage response = client.GetAsync(url).Result;
            var jsonString = response.Content.ReadAsStringAsync();
            /*jsonString.Wait();*/
            ganancia = int.Parse(jsonString.Result);



            return ganancia;

        }
        #endregion

        #region Exportar pdf
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(Comprobante c)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(c.Grid);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Listado-Comprobantes.pdf");
            }
        }
        #endregion
    }
}