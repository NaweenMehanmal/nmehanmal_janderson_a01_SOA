﻿/*
    Filename: SoapClient.cs
    By: Naween M and Jeremy A
    Assignment: Assignment 2 SOA
    Date: Sept 28, 2017
    Description: Handles all SOAP messaging communication, parses the WSDL file appropriately to fit our needs and returns the data to the main form 
    to display to the user in an acceptable format 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public const string xmlns_xsd  = "http://www.w3.org/2001/XMLSchema";
        public const string xmlns_xsi  = "http://www.w3.org/2001/XMLSchema-instance";
        public const string return_param_tag = "return_method/return_param";
        public const string return_response_tag = "return_method";


        public List<string> WebServiceList { get; set; }

        public SoapClient()
        {
            WebServiceList = new List<string>();
        }


        public void SoapRequestAndResponse(XmlDocument xmlDoc, string serviceName, string methodName, Dictionary<string, object> paramMap, ref DataGridView dgView)
        {
            //Generate the SOAP request message body
            XmlDocument soapEnvXml = new XmlDocument();
            bool isSoapMessageGenerated = false;
            string soapActionUrl = "";
            string location = "";
            string url = "";
            string returnParamName = "";
            string returnResponseName = "";
            string tns = "";

            try
            {
                foreach (XmlNode defNode in xmlDoc.GetElementsByTagName("definition"))
                {                   
                    foreach (XmlNode serviceNode in defNode.SelectNodes("service"))
                    {
                        if (serviceNode.Attributes["name"].Value == serviceName)
                        {                          
                            tns = serviceNode.ParentNode.Attributes["targetNamespace"].Value;
                            location = serviceNode.Attributes["location"].Value;
                            url = serviceNode.ParentNode.Attributes["targetNamespace"].Value;

                            foreach (XmlNode methodNode in serviceNode.SelectNodes("method"))
                            {
                                if (methodNode.Attributes["name"].Value == methodName)
                                {                                    
                                    returnResponseName = methodNode.SelectSingleNode(return_response_tag).Attributes["name"].Value;
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
                return;
            }

            //Through an HTTP Post request, sent the data            
            try
            {
                //Http Related
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(location);
                httpRequest.Headers.Add(@"SOAP:Action");
                httpRequest.ContentType = "text/xml;charset=\"utf-8\"";
                httpRequest.Accept = "text/xml";
                httpRequest.Method = "POST";

                //Request
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    soapEnvXml.Save(stream);
                }

                //Response
                dgView.DataSource = null; //Clear the DataGridView

                using (WebResponse response = httpRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        XNamespace ns = tns;
                        XElement soapXML = XElement.Parse(reader.ReadToEnd());
                        XElement soapResponse = soapXML.Descendants(ns + returnParamName).First();

                        DataTable responseTable = new DataTable();
                        List<DataColumn> colList = new List<DataColumn>();
                        List<object> rowVals = new List<object>();

                        //Find out if a struct was returned
                        if (soapResponse.Descendants().Count() > 0)
                        {                            
                            foreach (XElement col in soapResponse.Descendants())
                            {
                                colList.Add(new DataColumn(col.Name.LocalName.ToString()));
                                rowVals.Add(col.Value);
                            }

                            responseTable.Columns.AddRange(colList.ToArray()); //Set the columns
                            responseTable.Rows.Add(rowVals.ToArray()); //Add the row                           
                        }
                        else
                        {
                            responseTable.Columns.Add(new DataColumn(soapResponse.Name.LocalName.ToString()));
                            responseTable.Rows.Add(soapResponse.Value);
                        }                     
                        
                        //Add the new data source
                        dgView.DataSource = responseTable;
                        dgView.AutoResizeColumns();
                        //dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (WebException webEx)
            {
                //Log the error on the client side, possible SOAP fault 
                try
                {
                    string errResponseBody = new StreamReader(webEx.Response.GetResponseStream()).ReadToEnd();
                    string soapFaultMsg  = "";
                    string soapFaultCode = "";

                    XElement soapXML = XElement.Parse(errResponseBody);

                    //Fault Code
                    IEnumerable<XElement> tmpFaultCode = soapXML.DescendantsAndSelf("faultcode");
                    soapFaultCode = string.Concat(tmpFaultCode.Nodes());
                    soapFaultCode = soapFaultCode.Replace(":", " ");

                    //Fault Message
                    IEnumerable<XElement> tmpFaultStr = soapXML.DescendantsAndSelf("faultstring");                    
                    soapFaultMsg = string.Concat(tmpFaultStr.Nodes());

                    //Determine if this is a SOAP fault or not
                    using (StreamWriter log = new StreamWriter("../../client-errors.log", true))
                    {
                        log.WriteLine("Date       : " + DateTime.Now);
                        log.WriteLine("Http Status: " + webEx.Message);
                        log.WriteLine("Fault Code : " + soapFaultCode);
                        log.WriteLine("Description: " + soapFaultMsg);
                        log.WriteLine();
                    }

                    //Display the SOAP fault on the DataGrid also
                    DataTable responseTable = new DataTable();

                    responseTable.Columns.AddRange(new DataColumn[] {
                        new DataColumn("Soap Code"),
                        new DataColumn("Soap Fault Description")
                    });

                    responseTable.Rows.Add(soapFaultCode, soapFaultMsg);
                    
                    dgView.DataSource = responseTable;
                    dgView.AutoResizeColumns();
                    //dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to parse web exception");
                }         
            }           
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return;
        }



        public void ParseXmlSoapResponse(ref TreeView tvDisplayResponse, XmlNode root, TreeNode tParentNode)
        {
            if (root is XmlElement)
            {
                if (root.HasChildNodes)
                {
                    TreeNode newTreeNode;

                    if (tvDisplayResponse.Nodes.Count == 0)
                    {
                        newTreeNode = tvDisplayResponse.Nodes.Add(root.Name);
                    }
                    else
                    {
                        newTreeNode = tParentNode.Nodes.Add(root.Name);
                    }

                    ParseXmlSoapResponse(ref tvDisplayResponse, root.FirstChild, newTreeNode);
                }

                if (root.NextSibling != null)
                {
                    ParseXmlSoapResponse(ref tvDisplayResponse, root.NextSibling, tParentNode);
                }
            }
            else if (root is XmlText)
            {
                tParentNode.Nodes.Add(root.Value);
            }

            if(tvDisplayResponse.Nodes.Count == 0)
            {
                TreeNode newNode = tvDisplayResponse.Nodes.Add(root.Name);
                string result = "No results found!";
                
                if(root.InnerText != "")
                {
                    result = root.InnerText;    
                }

                newNode.Nodes.Add(result);
            }
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

            return retSoapXml;
        }



        public void GetWebServicesAndMethods(XmlDocument xmlDoc)
        {
            //First look at the WSDL file and get all the information that is necessary 

            try
            {
                foreach (XmlNode definitionNode in xmlDoc.GetElementsByTagName("definition"))
                {
                    foreach (XmlNode serviceNode in definitionNode.SelectNodes("service"))
                    {                     
                        //Now parse through the WSDL
                        XElement root = XElement.Load(definitionNode.Attributes["wsdl"].Value); 
                        XNamespace wsdl = "http://schemas.xmlsoap.org/wsdl/";
                        XNamespace s = "http://www.w3.org/2001/XMLSchema";

                        IEnumerable<XElement> service = from element in root.Elements(wsdl + "service") where (string)element.Attribute("name") == serviceNode.Attributes["name"].Value select element; //Find relevant service tag
                        IEnumerable<XElement> port = from element in service.Elements(wsdl + "port") where (string)element.Attribute("name") == serviceNode.Attributes["port_name"].Value select element; //Find relevant port tag

                        //Get the location and add it to the service location attribute
                        var soapAddress = port.Descendants().First().Attribute("location");
                        XmlAttribute attr = xmlDoc.CreateAttribute("location");
                        attr.InnerText = soapAddress.Value.ToString();
                        serviceNode.Attributes.Append(attr);

                        //Now loop through all the methods, get the return type, data types, etc
                        foreach (XmlNode methodName in serviceNode.SelectNodes("method"))
                        {
                            string responseMethod = "";
                            string requestMethod = "";
                            string soapRequestMethodName = "";
                            string soapResponseMethodName = "";

                            //(ParamName, DataType) => Dictionary
                            Dictionary<string, string> reqParameters = new Dictionary<string, string>();
                            Dictionary<string, string> resParameters = new Dictionary<string, string>();

                            //Grab the input and output method names

                            IEnumerable<XElement> portType = from element in root.Elements(wsdl + "portType") where (string)element.Attribute("name") == serviceNode.Attributes["port_name"].Value select element; //Find relevant portType tag
                            IEnumerable<XElement> operation = from element in portType.Elements(wsdl + "operation") where (string)element.Attribute("name") == methodName.Attributes["name"].Value select element; //Find relevant operation tag

                            foreach (XElement element in operation.Descendants())
                            {
                                //Request
                                if ((XName)element.Name.LocalName == "input")
                                {
                                    //Get the value
                                    string tmp = element.Attribute("message").Value.ToString();
                                    //Get rid of the namespace
                                    requestMethod = tmp.Split(':').Last();
                                }

                                //Response
                                if ((XName)element.Name.LocalName == "output")
                                {
                                    //Get the value
                                    string tmp = element.Attribute("message").Value.ToString();
                                    //Get rid of the namespace
                                    responseMethod = tmp.Split(':').Last();
                                }
                            }

                            //Now get the parameters for the request and response method (Ones used for SOAP messages)                    
                            IEnumerable<XElement> requestMessage = from element in root.Elements(wsdl + "message") where (string)element.Attribute("name") == requestMethod select element; //Find relevant message tag

                            foreach (XElement element in requestMessage.Descendants())
                            {
                                if ((XName)element.Name.LocalName == "part")
                                {
                                    if(element.Attributes("element").Any() == true)
                                    {
                                        soapRequestMethodName = element.Attribute("element").Value.ToString().Split(':').Last();
                                    } 
                                    else if(element.Attributes("type").Any() == true)
                                    {
                                        //Since the type is mentioned here get the parameter
                                        reqParameters.Add(element.Attribute("name").Value.ToString(), element.Attribute("type").Value.ToString().Split(':').Last());
                                    }
                                }
                            }

                            IEnumerable<XElement> responseMessage = from element in root.Elements(wsdl + "message") where (string)element.Attribute("name") == responseMethod select element; //Find relevant message tag

                            foreach (XElement element in responseMessage.Descendants())
                            {
                                if ((XName)element.Name.LocalName == "part")
                                {
                                    if (element.Attributes("element").Any() == true)
                                    {
                                        soapResponseMethodName = element.Attribute("element").Value.ToString().Split(':').Last();
                                    }
                                    else if (element.Attributes("type").Any() == true)
                                    {
                                        //Since the type is mentioned here get the parameter name
                                        resParameters.Add(element.Attribute("name").Value.ToString(), element.Attribute("type").Value.ToString().Split(':').Last());
                                    }
                                }
                            }

                            //Get the parameters and it's parameters
                            //Change the namespace (s is used instead)

                            //REQUEST PARAM
                            foreach (XElement paramElements in (from element in root.Descendants(s + "element") where (string)element.Attribute("name") == soapRequestMethodName select element).Descendants())
                            {
                                if ((XName)paramElements.Name.LocalName == "element")
                                {
                                    reqParameters.Add(paramElements.Attribute("name").Value.ToString(), paramElements.Attribute("type").Value.ToString().Split(':').Last());
                                }
                            }


                            //RESPONSE PARAM
                            foreach (XElement paramElements in (from element in root.Descendants(s + "element") where (string)element.Attribute("name") == soapResponseMethodName select element).Descendants())
                            {
                                if ((XName)paramElements.Name.LocalName == "element")
                                {
                                    resParameters.Add(paramElements.Attribute("name").Value.ToString(), paramElements.Attribute("type").Value.ToString().Split(':').Last());
                                }
                            }

                            //First insert the request parameters
                            foreach (KeyValuePair<string, string> entry in reqParameters)
                            {
                                // do something with entry.Value or entry.Key
                                XmlNode xmlParam = xmlDoc.CreateNode("element", "param", "");

                                //Name
                                XmlAttribute newAttrName = xmlDoc.CreateAttribute("name");
                                newAttrName.InnerText = entry.Key;

                                //Datatype
                                XmlAttribute newAttrDataType = xmlDoc.CreateAttribute("type");
                                newAttrDataType.InnerText = entry.Value;

                                xmlParam.Attributes.Append(newAttrName);
                                xmlParam.Attributes.Append(newAttrDataType);

                                methodName.AppendChild(xmlParam);
                            }

                            //Insert return method
                            XmlNode xmlReturnMethod = xmlDoc.CreateNode("element", "return_method", "");

                            //Return method name
                            XmlAttribute newReturnMethodAttrName = xmlDoc.CreateAttribute("name");
                            newReturnMethodAttrName.InnerText = soapResponseMethodName;

                            xmlReturnMethod.Attributes.Append(newReturnMethodAttrName);

                            XmlNode retMethod = methodName.AppendChild(xmlReturnMethod);

                            //RETURN PARAM
                            foreach (KeyValuePair<string, string> entry in resParameters)
                            {
                                // do something with entry.Value or entry.Key
                                XmlNode xmlParam = xmlDoc.CreateNode("element", "return_param", "");

                                //Name
                                XmlAttribute newReturnAttrName = xmlDoc.CreateAttribute("name");
                                newReturnAttrName.InnerText = entry.Key;

                                //Datatype
                                XmlAttribute newReturnAttrDataType = xmlDoc.CreateAttribute("type");
                                newReturnAttrDataType.InnerText = entry.Value;

                                xmlParam.Attributes.Append(newReturnAttrName);
                                xmlParam.Attributes.Append(newReturnAttrDataType);

                                retMethod.AppendChild(xmlParam);
                            }


                        }  //Method tag loop
                    }  //Service tag loop
                } //Definition tag loop

                //xmlDoc.Save("test.xml");

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
            catch(Exception ex)
            {
                MessageBox.Show("Unable to parse the WSDL file.");
            }

        } //End of method



    }

}
