using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadow_Slave_Prueba_de_Web_Scraping_
{
    internal class Capitulo
    {
        public string Url { get; set; }
        public string Titulo { get; set; }
        public List<string> Contenido { get; set; }
        public string Siguiente_URL { get; set; }

        public Capitulo()
        {

        }

        public Capitulo(string url, string titulo, string siguiente_url)
        {
            Url = url;
            Titulo = titulo;
            Contenido = new List<string>();
            Siguiente_URL = siguiente_url;
        }

        public void AgregarContenido(string contenido)
        {
            Contenido.Add(contenido);
        }
    }
}
