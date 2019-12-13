using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLinguini.Models.DTO
{
    public class DetalleComprobante
    {
        [JsonProperty("idDetalleComprobante")]
        public int idDetalleComprobante { get; set; }

        [JsonProperty("cantidadOrden")]
        public int cantidadOrden { get; set; }

        [JsonProperty("precioDetalleComprobante")]
        public int precioDetalleComprobante { get; set; }

        [JsonProperty("nombreCarta")]
        public string nombreCarta { get; set; }

        [JsonProperty("idComprobante")]
        public int idComprobante { get; set; }

    }
}