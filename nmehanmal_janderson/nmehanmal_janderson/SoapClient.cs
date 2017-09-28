using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace nmehanmal_janderson
{
    class SoapClient
    {

        //const
        public const string xmlns_soap = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string xmlns_xsd = "http://www.w3.org/2001/XMLSchema";
        public const string xmlns_xsi = "http://www.w3.org/2001/XMLSchema-instance";

        public const string return_param_tag = "return_method/return_param";

        public List<string> WebServiceList { get; set; }

        public SoapClient()
        {
            WebServiceList = new List<string>();
        }


        public DataTable SoapRequestAndResponse(XmlDocument xmlDoc, string serviceName, string methodName, Dictionary<string, object> paramMap)
        {
            //Generate the SOAP request message body
            XmlDocument soapEnvXml = new XmlDocument();
            bool isSoapMessageGenerated = false;
            string soapActionUrl = "";
            string location = "";
            string url = "";
            string returnParamName = "";

            try
            {
                foreach (XmlNode defNode in xmlDoc.GetElementsByTagName("definition"))
                {
                    foreach (XmlNode serviceNode in defNode.SelectNodes("service"))
                    {
                        if (serviceNode.Attributes["name"].Value == serviceName)
                        {
                            location = serviceNode.Attributes["location"].Value; 
                            url = serviceNode.ParentNode.Attributes["targetNamespace"].Value;

                            foreach (XmlNode methodNode in serviceNode.SelectNodes("method"))
                            {
                                if (methodNode.Attributes["name"].Value == methodName)
                                {
                                    returnParamName = methodNode.SelectSingleNode(return_param_tag).Attributes["name"].Value;
                                    break;
                                }
                            }

                            soapEnvXml = GenerateSoapResponseTemplate(serviceNode, url, methodName, paramMap, out soapActionUrl);
                            isSoapMessageGenerated = true;
                            break;
                        }
                    }

                    if (isSoapMessageGenerated == true)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //Generating soap message failed
                MessageBox.Show(ex.Message);
                return null;
            }

            //Through an HTTP Post request, sent the data

            DataTable myTable = new DataTable();


            try
            {
                //Http Related
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(location);
                httpRequest.Headers.Add(@"SOAP:Action");
                httpRequest.ContentType = "text/xml;charset=\"utf-8\"";
                httpRequest.Accept = "text/xml";
                httpRequest.Method = "POST";

                //request
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    soapEnvXml.Save(stream);
                }

                //response

                string responseString = "";

                using (WebResponse response = httpRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string xmlString = reader.ReadToEnd();

                        XmlDocument responseDoc = new XmlDocument();

                        responseDoc.LoadXml(xmlString);

                        XmlNodeList nodes = responseDoc.GetElementsByTagName(returnParamName);
                        // We are expecting one node so if that is the result we proceed down the happy path.
                        if(nodes.Count == 1)
                        {
                            XmlDocument xmlResponseSoapMessage = ValidateXml(nodes[0].InnerText.Replace("\r\n", string.Empty));
                            if (xmlResponseSoapMessage != null)
                            {
                                myTable = ParseXmlSoapResponse(xmlResponseSoapMessage);
                            }
                            else
                            {
                                //Create the table row and column on the fly 
                                myTable.Columns.Add(nodes[0].Name);
                                myTable.Rows.Add(nodes[0].InnerText);
                            }
                        }
                        // If no nodes are found it could be that there is a SOAP fault so we search for that tag in a similar way.
                        else if (nodes.Count == 0)
                        {
                            // search now instead for a soap:Fault tag. if found it should contain the fault code and message.
                            nodes = responseDoc.GetElementsByTagName("soap:Fault");

                            if (nodes.Count != 1)
                            {
                                throw new InvalidDataException("Improper SOAP Response format! Cannot extract response.");
                            }
                            else
                            {
                                List<object> faultInfo = new List<object>();

                                //populate the soap fault headings into the table and capture the fault info in a list
                                foreach (XmlNode childNode in nodes[0].ChildNodes)
                                {
                                    myTable.Columns.Add(childNode.Name);
                                    faultInfo.Add(childNode.InnerText);
                                }

                                myTable.Rows.Add(faultInfo.ToArray());
                            }
                        }
                        else
                        // There should be no situation where there are multiple response nodes but i guess we can handle that in case.
                        {
                            throw new InvalidDataException("Improper SOAP Response format! Cannot extract response.");
                        }
                    }
                }

                //return responseString;
                return myTable; 
            }
            catch (XmlException xmlEx)
            {
                MessageBox.Show("SOAP Response XML Parsing Error: " + xmlEx.Message);
                return null;
            }
            catch(InvalidDataException dataEx)
            {
                MessageBox.Show("Invalid Soap Response Format: " + dataEx.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        public DataTable ParseXmlSoapResponse(XmlDocument xmlDoc)
        {
            DataTable retTable = new DataTable();
            List<object> myList = new List<object>();

            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("Table");

            //Get the column name first 
            foreach(XmlNode node in xmlList[0].ChildNodes)
            {
                retTable.Columns.Add(node.Name);
            }

            foreach(XmlNode node in xmlList)
            {

                myList.Clear();

                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    myList.Add(innerNode.InnerText);
                }

                retTable.Rows.Add(myList.ToArray());
            }


            //xmlDoc.ParentNode.Name == "NewDataSet"
            
            return retTable; 
        }



        public XmlDocument ValidateXml(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(xml);
            }
            catch
            {
                xmlDoc = null;
            }

            return xmlDoc;
        }



        public XmlDocument GenerateSoapResponseTemplate(XmlNode xNode, string targetURL, string methodName, Dictionary<string, object> paramMap, out string soapAction)
        {
            soapAction = "";

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
            soapMethod.SetAttribute("xmlns", targetURL); //XML instance
            soapBody.AppendChild(soapMethod);

            /* SOAP Parameter Name(s) */

            foreach (KeyValuePair<string, object> entry in paramMap)
            {
                // do something with entry.Value or entry.Key
                XmlElement paramMethod = retSoapXml.CreateElement(string.Empty, entry.Key, string.Empty);
                soapMethod.AppendChild(paramMethod);

                XmlText paramVal = retSoapXml.CreateTextNode(entry.Value.ToString());
                paramMethod.AppendChild(paramVal);
            }

            ////Get the soapAction 
            //foreach (XmlNode n in xNode.SelectNodes("method"))
            //{
            //    if (n.Attributes["name"].Value == methodName)
            //    {
            //        soapAction = n.Attributes["soapAction"].Value;
            //    }
            //}

            return retSoapXml;
        }


        public void GetWebServicesAndMethods(XmlDocument xmlDoc)
        {
            // in here i can deterine the name of the respone
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
}
