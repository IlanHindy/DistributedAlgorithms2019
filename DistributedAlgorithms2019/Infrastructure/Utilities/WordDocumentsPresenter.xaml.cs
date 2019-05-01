/**********************************************************************************************//**
 * \file    Infrastructure\WordDocumentsPresenter.xaml.cs
 *
 * Implements the word documents presenter.xaml class.
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
using System.IO;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * Interaction logic for WordDocumentsPresenter.xaml.
     *
     * \author  Ilan Hindy
     * \date    16/01/2017
     **************************************************************************************************/

    public partial class WordDocumentsPresenter : Window
    {
        /**********************************************************************************************//**
         * Default constructor.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         **************************************************************************************************/

        public WordDocumentsPresenter()
        {
            InitializeComponent();
            string fileName = @"C:\Users\ilanh\Documents\Distributed Algorithms\Distributed Algorithms Chapter 3 - Snapshots\Distributed Algorithms Chapter 3 - Snapshots.rtf";
            TextRange range = new TextRange(RichTextBox_document.Document.ContentStart, RichTextBox_document.Document.ContentEnd);
            FileStream fStream = new FileStream(fileName, FileMode.OpenOrCreate);
            range.Load(fStream, DataFormats.Rtf);
            fStream.Close();
        }
    }
}
