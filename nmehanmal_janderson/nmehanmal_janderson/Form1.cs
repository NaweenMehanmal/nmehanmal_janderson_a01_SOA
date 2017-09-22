﻿using System.Windows.Forms;
using System.Xml;

namespace nmehanmal_janderson
{
    public partial class Form1 : Form
    {
        private XmlDocument origConfigFile;
        private SoapClient  httpSoapClient;

        public Form1()
        {
            InitializeComponent();

            //Disable button
            bHttpPostButton.Enabled = false;

            origConfigFile = new XmlDocument();
            httpSoapClient = new SoapClient();

            origConfigFile.Load("WSDLConfiguration.xml"); //Obtain XML config file

            //Fill the Combobox dropdown with the available web services
            httpSoapClient.GetWebServicesAndMethods(origConfigFile);   

            //Load dropdown with list
            cServiceNames.DataSource = httpSoapClient.WebServiceList;

            httpSoapClient.SoapRequestAndResponse("", null);


            //string xmlComments = origConfigFile.InnerXml;

            ////Test for now
            //SOAPXMLSTRING =
            //"<? xml version = \"1.0\" encoding = \"utf-8\" ?>" + 
            //"< soap : Envelope xmlns: xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns: xsd = \"http://www.w3.org/2001/XMLSchema\" xmlns: soap = \"http://schemas.xmlsoap.org/soap/envelope/\" >" +            
            //"< soap:Body >" +
            //"< GetAirportInformationByCountry xmlns = \"http://www.webserviceX.NET\" >" +
            //"< country > Canada </ country >" +
            //"</ GetAirportInformationByCountry >" +
            //"</ soap:Body >" +
            //"</ soap:Envelope >";

            //InvokeWebService("http://www.webserviceX.NET", "", "", SOAPXMLSTRING);
        }



        public void GenerateRadioButtonsForMethods()
        {
            //Clean out layout
            layoutMethodNames.Controls.Clear();
            layoutParameterNames.Controls.Clear();

            //Insert method names as radio buttons to choose from
            XmlNode node = origConfigFile.SelectSingleNode(string.Format("//*[@name = '{0}']", cServiceNames.Text.ToString()));

            foreach(XmlNode tmpNode in (node.SelectNodes("method") as XmlNodeList))
            {
                RadioButton rButton = new RadioButton();

                rButton.Text = tmpNode.Attributes["name"].Value;
                rButton.AutoSize = true;
                rButton.Tag = tmpNode; //To be accessed from the lambda expression
                rButton.Click += (sender, e) => {
                    //Clear out layout
                    layoutParameterNames.Controls.Clear();

                    //Insert the validation for the parameters associated with the method
                    foreach (XmlNode innerNode in (((sender as RadioButton).Tag as XmlNode).SelectNodes("param") as XmlNodeList))
                    {
                        TextBox textBox = new TextBox();
                        Label textLabel = new Label();

                        textLabel.Text = innerNode.Attributes["name"].Value + ':';

                        //Add controls to layout
                        layoutParameterNames.Controls.AddRange(new Control[] { textLabel, textBox });
                    }

                    bHttpPostButton.Enabled = true; //True because Method is chosen and Parameters are shown
                };

                layoutMethodNames.Controls.Add(rButton);
            }        
        }



        //
        //Dropdown changed event
        //
        private void cServiceNames_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //Generate the methods 
            GenerateRadioButtonsForMethods();

            //Fresh start, disable post button
            bHttpPostButton.Enabled = false; 
        }


        //
        //Button click event
        //
        private void bHttpPostButton_Click(object sender, System.EventArgs e)
        {
            //Post SOAP message
            httpSoapClient.SoapRequestAndResponse("", null);
        }


    }
}
