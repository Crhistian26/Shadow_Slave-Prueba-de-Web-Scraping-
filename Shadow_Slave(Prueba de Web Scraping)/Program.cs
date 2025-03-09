using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Media;
using System.Net;
using System.Text.Json;


namespace Shadow_Slave_Prueba_de_Web_Scraping_
{
    internal class Program
    {
        public static Capitulo SearchCap(int start,int cap, string url)
        {
            Capitulo capitulo = new Capitulo();
            for (int i = start; i < cap; i++)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                string URLNext = doc.DocumentNode.CssSelect("a").Where(a => a.InnerText == "Capitulo Siguiente").Select(a => a).First().GetAttributeValue("href");


                if (i == cap - 1)
                {
                    var nodes = doc.DocumentNode.CssSelect(".entry-title");

                    string titulo = nodes.First().InnerText;

                    var nodeContent = doc.DocumentNode.CssSelect(".entry-content");

                    URLNext = doc.DocumentNode.CssSelect("a").Where(a => a.InnerText == "Capitulo Siguiente").Select(a => a).First().GetAttributeValue("href");

                    capitulo = new Capitulo(url, titulo, URLNext);
                    foreach (var p in nodeContent.CssSelect("p"))
                    {
                        capitulo.AgregarContenido(WebUtility.HtmlDecode(p.InnerText));
                    }
                }
                url = URLNext;
            }

            return capitulo;

        }
        public static Capitulo GetCapitulo(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            var nodes = doc.DocumentNode.CssSelect(".entry-title");

            string titulo = nodes.First().InnerText;

            var nodeContent = doc.DocumentNode.CssSelect(".entry-content");

            string URLNext = doc.DocumentNode.CssSelect("a").Where(a => a.InnerText == "Capitulo Siguiente").Select(a => a).First().GetAttributeValue("href");

            Capitulo capitulo = new Capitulo(url, titulo, URLNext);

            foreach (var p in nodeContent.CssSelect("p"))
            {
                capitulo.AgregarContenido(WebUtility.HtmlDecode(p.InnerText));
            }

            return capitulo;
        }
        static async Task Main(string[] args)
        {
            string url = "https://devilnovels.com/esclavo-de-las-sombras/shadow-slave-capitulo-1-2/116637/";

            Capitulo capitulo = new Capitulo();
            string UltimoCapitulo;
            bool esta = false;

            string b;
            string a = Directory.GetCurrentDirectory();

            a = Directory.GetParent(a).FullName;
            a = Directory.GetParent(a).FullName;
            a = Directory.GetParent(a).FullName;
            b = a;

            a = Path.Combine(a, "Capitulos");

            

            List<int> exists_caps = new List<int>();

            string[] h = Directory.GetFiles(a);
            for (int i = 1; i <= h.Length; i++)
            {
                int number = Convert.ToInt32(Path.GetFileName(h[i - 1]).Replace(".txt", ""));
                exists_caps.Add(number);
            }

            Console.WriteLine("Bienvenido a la libreria de Shadow Slave");
            if (exists_caps.Count > 0)
            {
                int caps = 0;
                for (int i = 0; i < exists_caps.Count; i++)
                {
                    caps++;
                }

                try
                {
                    UltimoCapitulo = File.ReadAllText(Path.Combine(b, "Ultimo" + ".txt"));
                }
                catch (Exception)
                {
                    UltimoCapitulo = "Ninguno";

                    File.WriteAllText(Path.Combine(b, "Ultimo" + ".txt"), "Ninguno");
                }
                

                Console.WriteLine("Capitulos Disponibles: " + caps);
                
                Console.WriteLine("Capitulo mas reciente: " +  exists_caps.Max());

                Console.WriteLine("Ultimo capitulo leido: " + UltimoCapitulo);

            }
            else
            {
                Console.WriteLine("No tienes capitulos descargados");
            }

            Console.WriteLine("Escribe el numero del capitulo que deseas leer: ");
            int cap = Convert.ToInt32(Console.ReadLine());

            ConsoleKeyInfo key;
            do
            {

                Console.Clear();

                if (exists_caps.Contains(cap))
                {
                    var file = File.ReadAllLines(Path.Combine(a, cap + ".txt"));
                    capitulo = JsonSerializer.Deserialize<Capitulo>(file[0]);
                    esta = true;
                }
                else
                {
                    if (cap > 1)
                    {
                        int start = exists_caps.Where(caps => caps < cap).Max();

                        Capitulo referencia = new Capitulo();
                        var file = File.ReadAllLines(Path.Combine(a, start + ".txt"));
                        referencia = JsonSerializer.Deserialize<Capitulo>(file[0]);

                        capitulo = SearchCap(start, cap, referencia.Siguiente_URL);
                    }
                    else
                    {
                        capitulo = GetCapitulo(url);
                    }
                }

                Console.SetCursorPosition(0, 1);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(capitulo.Titulo + "\n");
                Console.ForegroundColor = ConsoleColor.White;


                foreach (var c in capitulo.Contenido)
                {
                    Console.WriteLine(c + "\n");
                }

                if (!esta)
                {
                    string CapSerialise = JsonSerializer.Serialize(capitulo);
                    string capnumber = new string(capitulo.Titulo.Where(char.IsDigit).ToArray());
                    string ruth = Path.Combine(a, capnumber + ".txt");
                    File.WriteAllText(ruth, CapSerialise);
                }

                File.WriteAllText(Path.Combine(b, "Ultimo" + ".txt"),cap.ToString());

                do
                { 
                    key = Console.ReadKey();
                } while (key.Key != ConsoleKey.LeftArrow && key.Key != ConsoleKey.RightArrow && key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.Enter);

                if (key.Key == ConsoleKey.RightArrow)
                {
                    cap++;
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    cap--;
                }

                Console.Clear();

            } while (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow);

        }
    }
}
