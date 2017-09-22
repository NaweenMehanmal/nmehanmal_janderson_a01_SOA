using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace nmehanmal_janderson
{
    class SoapClient
    {
        //Collections
        public List<string> WebServiceList { get; set; }
        public Dictionary<string, List<string>> WebServiceMethods { get; set; }

        //const
        public const string xmlns_soap = "http://www.w3.org/2003/05/soap-envelope/"; //"http://schemas.xmlsoap.org/soap/envelope/"
        public const string xmlns_xsd  = "http://www.w3.org/2001/XMLSchema";
        public const string xmlns_xsi  = "http://www.w3.org/2001/XMLSchema-instance";

        //public const string soap_encodingSytle = "http://www.w3.org/2003/05/soap-encoding";

        public SoapClient()
        {
            this.WebServiceList = new List<string>();
            this.WebServiceMethods = new Dictionary<string, List<string>>();
        }


        public void SoapRequestAndResponse(string url, XmlDocument xmlDoc, string methodName, Dictionary<string, object> paramMap)
        {
            //Generate the SOAP request message body
            XmlDocument soapEnvXml = new XmlDocument();  
            soapEnvXml = GenerateSoapResponseTemplate(xmlDoc, methodName, paramMap);

            //Through an HTTP Post request, sent the data
            try
            {
                //Http Related
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Accept = "text/xml";
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml;charset=\"utf-8\"";
                httpRequest.Headers.Add("SOAPAction", "");

                //request
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    soapEnvXml.Save(stream);
                }

                //response
                using (WebResponse response = httpRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();
                        MessageBox.Show(result);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            return;
        }



        public XmlDocument GenerateSoapResponseTemplate(XmlDocument configXml, string methodName, Dictionary<string, object> paramMap)
        {
            XmlDocument retSoapXml = new XmlDocument(); //New XML SOAP
            XmlDeclaration xmlDeclaration = retSoapXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement parent = retSoapXml.DocumentElement;
            retSoapXml.InsertBefore(xmlDeclaration, parent);

            /* SOAP Envelope */

            XmlElement soapEnvelope = retSoapXml.CreateElement("soap", "Envelope", xmlns_soap);
            soapEnvelope.SetAttribute("xmlns:xsi", xmlns_xsi); //XML instance
            soapEnvelope.SetAttribute("xmlns:xsd", xmlns_xsd); //XML xsd (schema definition)
            retSoapXml.AppendChild(soapEnvelope);

            /* SOAP Body */

            XmlElement soapBody = retSoapXml.CreateElement("soap", "Body", xmlns_soap);
            soapEnvelope.AppendChild(soapBody);

            /* SOAP Method Name */

            XmlElement soapMethod = retSoapXml.CreateElement(string.Empty, methodName, string.Empty);
            soapMethod.SetAttribute("xmlns", "random_url"); //XML instance
            soapBody.AppendChild(soapMethod);

            /* SOAP Parameter Name(s) */

            XmlElement paramMethod = retSoapXml.CreateElement(string.Empty, "random_parameter", string.Empty);
            soapMethod.AppendChild(paramMethod);

            XmlText text1 = retSoapXml.CreateTextNode("random text");
            paramMethod.AppendChild(text1);

            retSoapXml.Save(@"C:\Users\Naween\Desktop\test");

            return retSoapXml;
        }


        public void GetWebServicesAndMethods(XmlDocument xmlDoc)
        {
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("definition");

            foreach (XmlNode tmpNode in nodeList)
            {
                foreach (XmlNode serviceNode in (tmpNode.SelectNodes("service") as XmlNodeList))
                {
                    WebServiceList.Add(serviceNode.Attributes["name"].Value);
                }
            }
        }

    }

    struct SoapDefinition
    {

    }

}
