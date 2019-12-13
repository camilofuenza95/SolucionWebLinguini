using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLinguini.Models.DTO
{
    public class Ganancia
            
    {
        [JsonProperty("ganancia")]
        public int ganancia { get; set; }
    }
}