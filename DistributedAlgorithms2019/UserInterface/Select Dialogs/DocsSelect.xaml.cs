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
    /// <summary>
    /// Interaction logic for DocsSelect.xaml
    /// </summary>
    public partial class DocsSelect : Window
    {
        #region /// \name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectSource
        ///
        /// \brief Values that represent select targets.
        ///        The command that activated the dialog
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectSource { Source, Processed, PseudoCode }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Data
        ///
        /// \brief Values that represent data.
        ///        Enum to be used as index to the data and results from/to the SelectControls
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Data { Subject, Algorithm, Type, File };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectResult
        ///
        /// \brief Values that represent select results.
        ///        enum for the operation needed after finishing the dialog
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectResult { Quit, Cancel, Open, Save };
        #endregion
        #region /// \name Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (SelectResult) - The result.
        ///        The result of the dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SelectResult result = SelectResult.Quit;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;SelectControlData&gt;) - The data.
        ///        The data used to initialize the controls or update the controls.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<SelectControlData> data;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (SelectSource) - The select target.
        ///        The command that called the dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectSource selectSource;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string) - The file extension.
        ///        The extension of the file to select.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string fileExtension = ".docx";

        private Config.AlgorithmKeys pathKey;
        private Config.AlgorithmKeys filesKey;
        private string subject;
        private string algorithm;
        public string fileSelected;
        public DocsSelect(SelectSource selectSource, string message, string subject, string algorithm)
        {
            InitializeComponent();
            this.selectSource = selectSource;
            this.subject = subject;
            this.algorithm = algorithm;
            data = new List<SelectControlData>();
            switch (selectSource)
            {
                case SelectSource.Processed:
                    pathKey = Config.AlgorithmKeys.DocProcessedPath;
                    filesKey = Config.AlgorithmKeys.DocProcessedFiles;
                    break;
            }

            // Header Message
            Label_Message.Content = message;

            // Create the data for the subjects control
            List<string> subjects = ListStr(Config.Instance[Config.Keys.Subjects].Keys);
            List<int> selectedSubjectIdx = new List<int> { subjects.IndexOf(subject) };
            data.Add(new SelectControlData() { options = subjects, initiallySelected = selectedSubjectIdx });

            // Create the data for the algorithm control
            List<string> algorithms = ListStr(Config.Instance[Config.Keys.Subjects][subject].Keys);
            List<int> selectedAlgorithmIdx = new List<int> { algorithms.IndexOf(algorithm) };
            data.Add(new SelectControlData { options = algorithms, initiallySelected = selectedAlgorithmIdx });

            // Create the data for the Type control
            List<string> documentType = new List<string> { TypesUtility.GetKeyToString(selectSource) };
            List<int> selectedTypeIdx = new List<int> { 0 };
            data.Add(new SelectControlData { options = documentType, initiallySelected = selectedTypeIdx });

            // Create the data for the files control
            List<string> files = ListFiles(Config.Instance[Config.Keys.Subjects][subject][algorithm][filesKey]);
            if (files.Count == 0)
            {
                files.Add(algorithm);
            }
            List<int> selectedFileIdx = new List<int> { 0 };
            data.Add(new SelectControlData { options = files, initiallySelected = selectedFileIdx });
            Selection_File.Init(data[(int)Data.File]);
            Selection_Type.Init(data[(int)Data.Type]);
            Selection_Algorithm.Init(data[(int)Data.Algorithm]);
            Selection_Subject.Init(data[(int)Data.Subject]);
        }
        private List<string> ListStr(dynamic keys)
        {
            List<string> result = new List<string>();
            foreach (object key in keys)
            {
                result.Add(key.ToString());
            }
            return result;
        }

        private List<string> ListFiles(AttributeList list)
        {
            List<string> result = new List<string>();
            foreach (Attribute attribute in list)
            {
                result.Add(attribute.Value.Replace(fileExtension, ""));
            }
            return result;
        }

        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            result = SelectResult.Quit;
            Close();
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            fileSelected = Selection_File.GetSelection().selectionText + fileExtension;

            // If the network is new the dictionary of the network is not found in the config
            // so we generate default path 
            string path;

            path = Config.Instance[Config.Keys.Subjects][subject][algorithm][pathKey];


            string absPath = System.IO.Path.GetFullPath(path);

            switch (selectSource)
            {
                case SelectSource.Processed:
                    result = EndSelectionForNew(absPath, fileSelected);
                    break;
            }

            if (result == SelectResult.Cancel)
            {
                return;
            }
            else
            {
                Close();
            }
        }

        private SelectResult EndSelectionForNew(string absPath, string file)
        {
            if (File.Exists(absPath + file))
            {
                string selectedAction = CustomizedMessageBox.Show(new List<string> {"The file already exist",
                    "Select Save to override",
                    "Select Cancel to cancel the selection",
                    "Select Quit to quit the dialog" },
                    "Select Docs Dialog Message",
                    new List<string> { "Save", "Cancel", "Quit" },
                    Icons.Question);
                return (SelectResult)TypesUtility.GetKeyFromString(typeof(SelectResult), selectedAction);
            }
            else
            {
                return SelectResult.Save;
            }
        }
        #endregion
    }
}