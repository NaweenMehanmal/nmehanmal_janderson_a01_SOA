using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmehanmal_janderson
{
    class SoapClient
    {
        
        public SoapClient()
        {

        }


        public void SendSoapRequest()
        {

        }


        public void ReceiveSoapResponse()
        {

        }

        //private XmlDocument InvokeWebService(string url, string method, string operation, string payload)
        //{
        //    StringBuilder message = new StringBuilder();
        //    XmlDocument xmlSoap = new XmlDocument();
        //    byte[] byteData;
        //    string result = "";

        //    //POST request
        //    httpWebRequest = WebRequest.Create(new Uri(url)) as HttpWebRequest;
        //    httpWebRequest.Method = "POST";
        //    httpWebRequest.ContentType = "application/xml";

        //    //Data to send to web service
        //    message.Append(payload); //What goes into the SOAP body

        //    byteData = Encoding.UTF8.GetBytes(message.ToString()); //UTF8 Character Set

        //    httpWebRequest.ContentLength = byteData.Length; //Byte data length in the POST header

        //    //Write data to request
        //    using (Stream request = httpWebRequest.GetRequestStream())
        //    {
        //        request.Write(byteData, 0, byteData.Length);
        //    }

        //    //Get response data in XML format
        //    try
        //    {
        //        using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
        //        {
        //            StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
        //            result = reader.ReadToEnd();
        //            reader.Close();
        //        }

        //        xmlSoap.LoadXml(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Debug.Write(ex.Message);
        //    }

        //    return xmlSoap;
        //}


    }
}
