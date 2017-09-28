using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data;
using System.Text.RegularExpressions;

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
                        textBox.Name = innerNode.Attributes["type"].Value + "_" + innerNode.Attributes["name"].Value;

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
        private void cServiceNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateRadioButtonsForMethods(); //Generate the methods

            bHttpPostButton.Enabled = false; //Fresh start, disable post button
            
            lblSoapResponseValue.Text = "";
        }


        //
        //Button click event
        //
        private void bHttpPostButton_Click(object sender, EventArgs e)
        {
            //Get selected method
            var rButtonChecked = layoutMethodNames.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            bool isDataValid = true;
            List<string> errorList = new List<string>(); 

            //Get input parameters
            Dictionary<string, object> paramMap = new Dictionary<string, object>();

            foreach (Control paramControl in layoutParameterNames.Controls)
            {              
                if (!((paramControl.GetType()).Equals(typeof(Label))))
                {
                    //First validate it
                    string dataType = paramControl.Name.Split('_')[0];
                    string ctrlName = paramControl.Name.Split('_')[1];

                    //Check to see if the field is empty
                    if (paramControl.Text.Trim() == "")
                    {
                        isDataValid = false;
                        errorList.Add(string.Format("[{0}] parameter requires a non-empty value!", ctrlName));
                    }
                    else
                    {
                        //Check if the proper value has in put in
                        if (dataType == "string")
                        {
                            if(Regex.IsMatch(paramControl.Text, "(\\d+)|[_,+/\\?!=@#$%^&*();<>\"':.]"))
                            {
                                isDataValid = false;
                                errorList.Add(string.Format("[{0}] parameter may only contain alphabets!", ctrlName));
                            }
                        }
                        else if (dataType == "int" || dataType == "double")
                        {
                            if (Regex.IsMatch(paramControl.Text, "\\D+"))
                            {
                                isDataValid = false;
                                errorList.Add(string.Format("[{0}] parameter may only contain numbers!", ctrlName));
                            }
                        }
                        else if (dataType == "boolean")
                        {
                            if (!(paramControl.Text == "true" || paramControl.Text == "false"))
                            {
                                isDataValid = false;
                                errorList.Add(string.Format("[{0}] parameter may only contain true or false!", ctrlName));
                            }
                        }
                    }

                    paramMap.Add(ctrlName, paramControl.Text); //Use ctrlName because it's the same name as the parameter in the WSDL
                }
            }

            if(isDataValid == true)
            {
                httpSoapClient.SoapRequestAndResponse(origConfigFile, cServiceNames.Text, rButtonChecked.Text, paramMap, ref this.tvDisplayResponse); //Post SOAP response message
            }
            else
            {
                //Display error due to invalid data
                string errMessage = "";

                foreach(string str in errorList)
                {
                    errMessage += str + '\n';
                }
                
                MessageBox.Show(errMessage);
            }
        }


    }
}
