/**********************************************************************************************//**
 * \file    UserInterface\InputDialog.xaml.cs
 *
 * Implements the input dialog.xaml class.
 **************************************************************************************************/

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

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * Interaction logic for InputDialog.xaml.
     *
     * \author  Ilan Hindy
     * \date    16/01/2017
     **************************************************************************************************/

    public partial class InputDialog : Window
    {
        /** The result. */
        public MessageBoxResult result = MessageBoxResult.Cancel;

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   text        The text.
         * \param   inputFields The input fields.
         **************************************************************************************************/

        public InputDialog(string text, Dictionary<string, string> inputFields)
        {
            InitializeComponent();
            int idx = 0;
            TextBlock_Text.Text = text;
            foreach (var entry in inputFields)
            {
                CreateInputField(entry.Key, entry.Value, idx);
                idx++;
            }
        }

        /**********************************************************************************************//**
         * Creates input field.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   fieldName       Name of the field.
         * \param   defaultValue    The default value.
         * \param   idx             The index.
         **************************************************************************************************/

        private void CreateInputField(string fieldName, string defaultValue, int idx)
        {
            RowDefinition gridRow = new RowDefinition();
            Grid_InputFields.RowDefinitions.Add(gridRow);
            Label textBlock = new Label();
            textBlock.Name = "TextBlock_" + fieldName.Replace(" ", "");
            textBlock.Content = fieldName;
            Grid.SetColumn(textBlock, 0);
            Grid.SetRow(textBlock, idx);
            Grid_InputFields.Children.Add(textBlock);
            TextBox textBox = new TextBox();
            textBox.Name = "TextBox_" + fieldName.Replace(" ", "");
            textBox.Text = defaultValue;
            Grid.SetColumn(textBox, 1);
            Grid.SetRow(textBox, idx);
            Grid_InputFields.Children.Add(textBox);
        }

        /**********************************************************************************************//**
         * Gets a value.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   fieldName   Name of the field.
         *
         * \return  The value.
         **************************************************************************************************/

        public string GetValue(string fieldName)
        {
            string textBoxName = "TextBox_" + fieldName.Replace(" ", "");
            TextBox textBox = (TextBox)(Grid_InputFields.Children.Cast<Control>().First(tb => tb.Name == textBoxName));
            return textBox.Text;
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_OK for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.OK;
            Close();
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Cancel for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            Close();
        }
    }
}
