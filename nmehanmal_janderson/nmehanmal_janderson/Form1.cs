using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace nmehanmal_janderson
{
    public partial class Form1 : Form
    {
        private HttpWebRequest httpWebRequest;
        private string SOAPXMLSTRING = "";


        public Form1()
        {
            InitializeComponent();

            //Test for now
            SOAPXMLSTRING =
            "<? xml version = \"1.0\" encoding = \"utf-8\" ?>" + 
            "< soap : Envelope xmlns: xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns: xsd = \"http://www.w3.org/2001/XMLSchema\" xmlns: soap = \"http://schemas.xmlsoap.org/soap/envelope/\" >" +            
            "< soap:Body >" +
            "< GetAirportInformationByCountry xmlns = \"http://www.webserviceX.NET\" >" +
            "< country > Canada </ country >" +
            "</ GetAirportInformationByCountry >" +
            "</ soap:Body >" +
            "</ soap:Envelope >";

            InvokeWebService("http://www.webserviceX.NET", "", "", SOAPXMLSTRING);


        }



        private XmlDocument InvokeWebService(string url, string method, string operation, string payload)
        {
            StringBuilder message = new StringBuilder();
            XmlDocument   xmlSoap = new XmlDocument();
            byte[] byteData;
            string result = "";
            
            //POST request
            httpWebRequest = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/xml";

            //Data to send to web service
            message.Append(payload); //What goes into the SOAP body

            byteData = Encoding.UTF8.GetBytes(message.ToString()); //UTF8 Character Set

            httpWebRequest.ContentLength = byteData.Length; //Byte data length in the POST header

            //Write data to request
            using (Stream request = httpWebRequest.GetRequestStream())
            {
                request.Write(byteData, 0, byteData.Length);
            }

            //Get response data in XML format
            try
            {
                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }

                xmlSoap.LoadXml(result);
            }
            catch(Exception ex)
            {
                Debug.Write(ex.Message);
            }


            return xmlSoap;
        }



    }
}
