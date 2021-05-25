using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChurchDataShow.Services
{
    class VersiculoService
    {
        public string Texto;
        public string Referencia;

        public VersiculoService()
        {
            Texto = null;
            Referencia = null;
        }

        public void BuscarVersiculo()
        {
            try
            {

                string urlAddress = "https://www.bibliaon.com/versiculo_do_dia/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (String.IsNullOrWhiteSpace(response.CharacterSet))
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream,
                            Encoding.GetEncoding(response.CharacterSet));

                    string data = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();

                    MinerarDados(data);
                }
            }
            catch
            {
                Texto = null;
                Referencia = null;
            }
        }

        private void MinerarDados(string HtmlText)
        {
            var array = HtmlText.Split(new string[] { "versiculo_hoje" }, StringSplitOptions.None);
            var element = array[1];

            Texto = element.Split(new string[] { "\">" }, StringSplitOptions.None)[1].Trim();
            Texto = Texto.Split(new string[] { " <br" }, StringSplitOptions.None)[0].Trim();

            Referencia = element.Split(new string[] { "\">" }, StringSplitOptions.None)[2].Trim();
            Referencia = Referencia.Split(new string[] { "</a>" }, StringSplitOptions.None)[0].Trim();
        }
    }
}
