using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /// <summary>
    /// Interaction logic for ElementInput.xaml
    /// </summary>
    public partial class ElementInput : Window
    {
        Network network;
        NetworkElement networkElement;
        private const int inputFieldWidth = 150;
        private const int inputFieldHeight = 30;
        private  Thickness inputFieldMargins = new Thickness(5,5,5,5);
        private bool Ediatble = true;
        public bool Updated = false;

        /********************************************************************
         * Structure of ElementInput window
         * The main window is composed from 2 panels :
         * 1. Buttom panel for operations on the network element which has a fixed size
         * 2. Up panel for attributes which has a flexible size. 
         *    This panel is composed from 2 pannels:
         *    1.Panel for the labels which has a fixed size in the left side
         *    2.Pannel for inserting values which has a flexible size in the right side
         * The height of the window is determined by the number of attributes and cnnot be changed in run time
         * The width of the window can be changed in run time
         *********************************************************************/
        
        /*
         * Constructor
         */
        public ElementInput(Network network, NetworkElement networkElement, bool ediatble = true)
        {
            this.network = network;
            this.networkElement = networkElement;
            Ediatble = ediatble;
            InitializeComponent();
            CreateAttributesControls();
        }

        /*
         * Create a lebel and an input field for each one of the network element attributes
         */
        private void CreateAttributesControls()
        {
            /*
             * The windows header
             */
            TextBox_Name.Text = "Element of type : " + networkElement.GetType().ToString();
            
            /*
             * Add Attributes controls      
             */
            foreach (var elementAttribute in networkElement[ElementDictionaries.ElementAttributes].Value )
            {
                string keyString = TypesUtility.GenerateKeyString(elementAttribute.Key);
                string attributeName = TypesUtility.GetEnumValueToString(elementAttribute.Key);
                StackPanel_AttributeNames.Children.Add(CreateLabel("Label_Attribute_key_" + keyString, attributeName));
                Control control = Attribute.CreateInputField(elementAttribute.Value.Value, TypesUtility.GetEnumValueToString(elementAttribute.Key), elementAttribute.Value.Editable, Ediatble);
                control.Name = "AttributeInput_" + keyString;
                SetControl(control);
                StackPanel_AttributeValues.Children.Add(control);
            }

            foreach (var privateAttribute in networkElement[ElementDictionaries.PrivateAttributes].Value)
            {
                string keyString = TypesUtility.GenerateKeyString(privateAttribute.Key);
                StackPanel_AttributeNames.Children.Add(CreateLabel("Label_Attribute_key_" + keyString, keyString));
                Control control = Attribute.CreateInputField(privateAttribute.Value.Value, TypesUtility.GetEnumValueToString(privateAttribute.Key), privateAttribute.Value.Editable, Ediatble);
                control.Name = "AttributeInput_" + keyString;
                SetControl(control);
                StackPanel_AttributeValues.Children.Add(control);
            }

            /*
             * Set the windows height according to the number of the attributes
             */
            SetWindowHeight();
        }

        public void SetControl(Control control)
        {
            if (control.GetType().Equals(typeof(Label))) { SetControl((Label)control); return; }
            if (control.GetType().Equals(typeof(TextBox))) { SetControl((TextBox)control); return; }
            if (control.GetType().Equals(typeof(ComboBox))) { SetControl((ComboBox)control); return; }
            if (control.GetType().Equals(typeof(Button))) { SetControl((Button)control); return; }
        }

  
        private Label CreateLabel(string labelName, string labelText)
        {
            Label label = new Label();
            DockPanel.SetDock(label, Dock.Top);
            label.Name = labelName;
            label.Height = inputFieldHeight;
            label.Margin = inputFieldMargins;
            label.Content = labelText;
            return label;
        }

        /*
         * Set a lebel
         */
        private void SetControl(Label label)
        {
            DockPanel.SetDock(label, Dock.Top);
            label.Height = inputFieldHeight;
            label.Margin = inputFieldMargins;
        }

        /*
         * Create a textBox input field
         * Note that all the text boxes has the same function for handling the LostFocus
         * of the field to activate the checking of the new value according to the checking
         * method associated with the attribute
         */
        private void SetControl(TextBox textBox)
        {
            DockPanel.SetDock(textBox, Dock.Top);
            textBox.Height = inputFieldHeight;
            //textBox.Width = inputFieldWidth;
            textBox.Margin = inputFieldMargins;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.BorderBrush = Brushes.Blue;
        }

        /*
         * Generate the ComboBox input field
         */
        private void SetControl(ComboBox comboBox)
        {
            DockPanel.SetDock(comboBox, Dock.Top);
            comboBox.Height = inputFieldHeight;
            //comboBox.Width = inputFieldWidth;
            comboBox.Margin = inputFieldMargins;
            comboBox.BorderBrush = Brushes.Blue;
            comboBox.BorderThickness = new Thickness(1);
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void SetControl(Button button)
        {
            DockPanel.SetDock(button, Dock.Top);
            button.Height = inputFieldHeight;
            //comboBox.Width = inputFieldWidth;
            button.Margin = inputFieldMargins;
            button.BorderThickness = new Thickness(1);
            button.BorderBrush = Brushes.Blue;
            if (button.Content.ToString().Contains("NetworkElement"))
            {
                button.Click += Button_EditNetworkElement;
            }
            else
            {
                button.Click += Button_EditList;
            }
        }

        /*
         * Set window height
         * The window heigh is fixed and defined by the number of attributes of the network element
         * (actually the number of the input controls)
         */
        private void SetWindowHeight()
        {
            double numberOfAttributes = networkElement[ElementDictionaries.ElementAttributes].Value.Count + networkElement[ElementDictionaries.PrivateAttributes].Value.Count;
            double attributeHeight = inputFieldHeight + inputFieldMargins.Top + inputFieldMargins.Bottom;
            double attributePannelHeight = (numberOfAttributes + 1) * attributeHeight;
            double windowHeight = TextBox_Name.Height + attributePannelHeight + StackPanel_Buttons.Height;
            this.Height = windowHeight;
            this.MinHeight = this.Height;
            this.MaxHeight = this.Height;
        }

        /*
         * Find an attribute according to the input field name
         * The attribute key is the input field without the "AttributeInput_" hearder
         * After calculating the key the attribute is sercched in the Network element dictionaries
         */
        private Attribute findAttribute(string inputFieldName)
        {
            string attributeKeyString = inputFieldName.Replace("AttributeInput_", "");
            return networkElement.FindAttribute(attributeKeyString);
        }

        private void Button_EditNetworkElement(object sender, RoutedEventArgs e)
        {
            //Get the attribute that the input field represent
            Attribute attribute = findAttribute(((Button)sender).Name);
            if (attribute != null)
            {
                ElementInput elementInput = new ElementInput(network, attribute.Value, attribute.Editable);
                elementInput.ShowDialog();
                if (elementInput.Updated)
                {
                    Updated = true;
                }
            }
        }

        private void Button_EditList(object sender, RoutedEventArgs e)
        {
            //Get the attribute that the input field represent
            Attribute attribute = findAttribute(((Button)sender).Name);
            if (attribute != null)
            {
                ListInput listInput = new ListInput(attribute.Value, network, attribute.Editable);
                listInput.ShowDialog();
                if (listInput.Updated)
                {
                    Updated = true;
                }
            }
        }

 

        /*
         * Method that is been activated when an input field of type TextBox lost focus 
         * (The insertion of the value ended)
         */
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Get the attribute that the input field represent
            Attribute attribute = findAttribute(((TextBox)sender).Name);
            if (attribute != null)
            {
                //Check if the value is leagel formatted string of the value of the attribute
                if(!attribute.CheckIfStringCanBeParssed(((TextBox)sender).Text))
                {
                    MessageBox.Show("The value inserted cannot be parsed to type " + attribute.GetTypeToString(), "Element Input Message");
                    ((TextBox)sender).Text = attribute.Value.ToString();
                    return;
                }
                //Activate the end input operation method that check if insertion is leagle
                //If the insertion is not leagle show a message box an return the text in 
                //the input control to the last one saved in the attribute
                string errorMessage;
                Attribute.EndInputOperationDelegate endInputOperation = attribute.EndInputOperation;
                if (!(endInputOperation(network, networkElement, attribute, ((TextBox)sender).Text, out errorMessage)))
                {
                    MessageBox.Show(errorMessage);
                    ((TextBox)sender).Text = attribute.Value.ToString();
                }
                else
                {
                    Updated = true;
                }
            }
        }

        /*
         * Method that is been activated when an input field of type ComboBox lost focus 
         * (The insertion of the value ended)
         */
        private void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Get the attribute that the input field present 
            //Get the value selected by the combo box
            Attribute attribute = findAttribute(((ComboBox)sender).Name);         
            string newValue = (string)((ComboBox)sender).SelectedValue;
            string errorMessage;
            if (attribute != null)
            {
                //Activate the end input operation method that check if insertion is leagle
                //If the insertion is not leagle show a message box an return the text in 
                //the input control to the last one saved in the attribute
                if (!(attribute.EndInputOperationMethod(network, networkElement, attribute, newValue, out errorMessage)))
                {
                    MessageBox.Show(errorMessage, "Element Input Message");

                    //Remove the event handler so that the event will not be fiered when the selection is changed
                    ((ComboBox)sender).SelectionChanged -= ComboBox_SelectionChanged;
                    ((ComboBox)sender).SelectedItem = attribute.GetValueToString();
                    ((ComboBox)sender).SelectionChanged += ComboBox_SelectionChanged;
                }
                else
                {
                    Updated = true;
                }
            }
        }

        private bool EditableCheck()
        {
            if (!Ediatble)
            {
                MessageBox.Show("The list cannot be updated because it is not editable", "Element Input Message");
                return false;
            }
            else
            {
                return true;
            }
        }


        /*
         * Reset the data in the input fields to what was last saved in the Network element
         * The function can be call from the GUI or from another function
         */
        private void ResetToSaved(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            Updated = false;

            //If the call is from another method the value of the Editted attribute is
            //determined by the parameter
            //Else the value is "True" - that meens eddited
            if (sender is bool)
            {
                networkElement[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Edited].Value = (bool)sender;
            }
            else
            {
                networkElement[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Edited].Value = true;
            }

            //Update the values in all the fields according to the attribute
            foreach (UIElement control in StackPanel_AttributeValues.Children)
            {
              Attribute attribute = findAttribute(((Control)control).Name);
                if (control.GetType().Equals(typeof(Label)))
                {
                    ((Label)control).Content = attribute.GetValueToString();
                }
                else if (control.GetType().Equals(typeof(TextBox)))
                {
                    ((TextBox)control).Text = attribute.GetValueToString();
                }
                else if (control.GetType().Equals(typeof(ComboBox)))
                {
                    ((ComboBox)control).SelectionChanged -= ComboBox_SelectionChanged;
                    ((ComboBox)control).SelectedItem = attribute.GetValueToString();
                    ((ComboBox)control).SelectionChanged += ComboBox_SelectionChanged;
                }
            }
            MessageBox.Show("Finnished Reset To Save", "Element Input Message");
        }

        /*
         * Find control according to name
         */
        private Control FindControl(object key)
        {
            string keyString = "AttributeInput_" + TypesUtility.GenerateKeyString(key);
            foreach (Control control in StackPanel_AttributeValues.Children)
            {
                if (control.Name == keyString)
                {
                    return control;
                }
            }
            return null;
        }

        /*
         * Apply the changes - copy the values from the input fields to the network element
         */
        private void Apply(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            Updated = false;
            networkElement[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Edited].Value = true; 
            //string editedKeyString = TypesUtility.GenerateKeyString(NetworkElement.ElementAttributeKeys.Edited);
            //Control editedControl = (Control)(StackPanel_AttributeValues.FindName("AttributeInput_" + editedKeyString));
            
            ((Label)FindControl(NetworkElement.ElementAttributeKeys.Edited)).Content = "True";
            foreach (UIElement control in StackPanel_AttributeValues.Children)
            {
                Attribute attribute = findAttribute(((Control)control).Name);
                string fieldText  = "";
                if (control.GetType().Equals(typeof(Label)))  continue; 
                if (control.GetType().Equals(typeof(TextBox)))  fieldText = ((TextBox)control).Text;
                if (control.GetType().Equals(typeof(ComboBox))) fieldText = ((ComboBox)control).SelectedItem.ToString();
                if (control.GetType().Equals(typeof(Button)))  continue;
                attribute.SetValueFromString(fieldText);
            }
            MessageBox.Show("Finnished Applying", "Element Input Message");
        }

        /*
         * Reset the values of the network element to the init of the element
         */
        private void ResetToInit(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            Updated = false;
            networkElement.Init(networkElement[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Id].Value, false);
            ResetToSaved(false, null);
            networkElement[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Edited].Value = false;
        }

        /*
         * Exit from the ElementInput window and save if needed
         */
      
        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExitAction()
        {
            if (Updated)
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("There is unsaved data do you want to exit or to apply first?", "Data not saved dialog", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    networkElement[ElementDictionaries.ElementAttributes].Value[NetworkElement.ElementAttributeKeys.Edited].Value = true;
                    Apply(null, null);
                    Updated = true;
                }
                else
                {
                    Updated = false;
                }
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitAction();
        }
    }
}
