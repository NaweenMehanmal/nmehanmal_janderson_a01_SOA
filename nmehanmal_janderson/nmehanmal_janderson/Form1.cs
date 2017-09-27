using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data;

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

            origConfigFile.Load("../../WSDLConfiguration.xml"); //Obtain XML config file

            //Fill the Combobox dropdown with the available web services
            httpSoapClient.GetWebServicesAndMethods(origConfigFile);   

            //Load dropdown with list
            cServiceNames.DataSource = httpSoapClient.WebServiceList;
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

                rButton.AutoSize = true;
                rButton.Tag = tmpNode; //To be accessed from the lambda expression
                rButton.Text = tmpNode.Attributes["name"].Value;
                rButton.Click += (sender, e) => {
                    //Clear out layout
                    layoutParameterNames.Controls.Clear();

                    //Insert the validation for the parameters associated with the method
                    foreach (XmlNode innerNode in (((sender as RadioButton).Tag as XmlNode).SelectNodes("param") as XmlNodeList))
                    {
                        Label textLabel = new Label();
                        textLabel.Text = innerNode.Attributes["name"].Value + ':';

                        TextBox textBox = new TextBox();
                        textBox.Name = innerNode.Attributes["name"].Value;

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

            lblSoapResponseValue.Text = "";
        }


        //
        //Button click event
        //
        private void bHttpPostButton_Click(object sender, EventArgs e)
        {
            //Get selected method
            var rButtonChecked = layoutMethodNames.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);

            //Get input parameters
            Dictionary<string, object> paramMap = new Dictionary<string, object>();

            foreach (Control paramControl in layoutParameterNames.Controls)
            {                 
                if (!((paramControl.GetType()).Equals(typeof(Label))))
                {
                    paramMap.Add(paramControl.Name, paramControl.Text);
                }
            }

                //Post SOAP response message
                DataTable newTable = httpSoapClient.SoapRequestAndResponse(origConfigFile, cServiceNames.Text, rButtonChecked.Text, paramMap);

                dgvDataResponse.DataSource = newTable; 
        }

   
    }
}
