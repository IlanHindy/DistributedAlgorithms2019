/**********************************************************************************************//**
 * \file    Tests\Test.xaml.cs
 *
 * Implements the test.xaml class.
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Office.Interop.Word;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * Interaction logic for Test.xaml.
     *
     * \author  Ilan Hindy
     * \date    16/01/2017
     **************************************************************************************************/

    public partial class Test : System.Windows.Window
    {
        /**********************************************************************************************//**
         * Default constructor.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         **************************************************************************************************/

        public Test()
        {
            InitializeComponent();
            application = new Microsoft.Office.Interop.Word.Application();
        }

        /** . */
        string wordFilePath = @"C:\Users\ilanh\Documents\Distributed Algorithms\Program\DistributedAlgorithms\DistributedAlgorithms\Algorithms\TestAlgorithm\";
        /** Filename of the word file. */
        string wordFileName = @"TestAlgorithm_tmp.docx";
        /** The application. */
        Microsoft.Office.Interop.Word.Application application;
        /** The temporary document. */
        Microsoft.Office.Interop.Word.Document tmpDocument;

        /**********************************************************************************************//**
         * Event handler. Called by Button_OpenWordDocument for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_OpenWordDocument_Click(object sender, RoutedEventArgs e)
        {
            if (application == null)
            {
                application = new Microsoft.Office.Interop.Word.Application();
            }
            tmpDocument = application.Documents.Open(wordFilePath + wordFileName);
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_CloseWordDocument for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_CloseWordDocument_Click(object sender, RoutedEventArgs e)
        {
            tmpDocument.Close();
            if (application.Documents.Count == 0)
            {
                application.Quit();
                application = null;
            }            
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_TOC for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_TOC_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\ilanh\Documents\Distributed Algorithms\Program\DistributedAlgorithms\DistributedAlgorithms\Algorithms Data\System\Base\Documentation\Processed\";
            string fileName = "base4 - Copy.docx";
            Document wordDocument;
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            wordDocument = word.Documents.Open(filePath + fileName);
            wordDocument.TablesOfContents[1].Update();
            wordDocument.Save();
            word.Quit();
        }

        public void Button_EnumType_Click(object sender, RoutedEventArgs e)
        {
            CustomizedMessageBox.Show(System.Drawing.KnownColor.Aqua.GetType().ToString(), "", null, Icons.Info);
            if (Type.GetType(System.Drawing.KnownColor.Aqua.GetType().ToString()) == null)
                CustomizedMessageBox.Show("null", "", null, Icons.Info);
            else
                CustomizedMessageBox.Show("not null", "", null, Icons.Info);
            if (TypesUtility.GetTypeFromString(System.Drawing.KnownColor.Aqua.GetType().ToString()) == null)
                CustomizedMessageBox.Show("null", "", null, Icons.Info);
            else
                CustomizedMessageBox.Show("not null", "", null, Icons.Info);
        }

        private void Button_GridView_Click(object sender, RoutedEventArgs e)
        {
            //GridView gridView = new GridView(new List<string> { "item No.1", "item No.2", "item No.3", "item No.4" });
            //StackPannel_Main.Children.Add(gridView);
        }

        private void Button_DisableTest_Click(object sender, RoutedEventArgs e)
        {
            TextBox_DisableTest.IsEnabled = !TextBox_DisableTest.IsEnabled;
            TextBox_DisableTest.Text = "IsEnabled = " + TextBox_DisableTest.IsEnabled.ToString();
            TextBox_DisableTest.LostFocus += Button_DisableTest_LostFocus;
        }

        private void Button_DisableTest_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Lost Focus");
        }
    }
}
