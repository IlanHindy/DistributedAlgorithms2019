////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\SourceTargetAlgorithmsSelect.xaml.cs
///
/// \brief Implements the source target algorithms select.xaml class.
///        
////////////////////////////////////////////////////////////////////////////////////////////////////

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
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class FileSelect
    ///
    /// \brief A file select.
    ///
    /// \par Description.
    ///      This class is used to select subject, algorithm, network, file
    ///
    /// \par Usage Notes.
    ///      Used by the MainWindow in New, Open, Debug, SaveAs, SaveDebug commands 
    ///
    /// \author Ilanh
    /// \date 29/11/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class FileSelect : Window
    {
        #region /// \name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectSource
        ///
        /// \brief Values that represent select targets.
        ///        The command that activated the dialog
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectSource { New, Open, Save, SaveAs, Debug, Log, SaveDebug}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Data
        ///
        /// \brief Values that represent data.
        ///        Enum to be used as index to the data and results from/to the SelectControls
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Data { Subject, Algorithm, Network, File};

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectResult
        ///
        /// \brief Values that represent select results.
        ///        enum for the operation needed after finishing the dialog
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectResult { Quit, Cancel, Open, New, Save, Debug, Log };
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
        /// \brief (List&lt;SelectResults&gt;) - The select results.
        ///        A list of the selected result from all the controls.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<SelectResults> selectResults;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to in init.
        ///        This parameter is used in order to block the selection changed events when initializing.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool inInit;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (SelectSource) - The select target.
        ///        The command that called the dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectSource selectSource;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string) - The file extension.
        ///        The extension of the file to select.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string fileExtension;

        /// \brief  (NetworkKeys) - The path key.
        private Config.NetworkKeys pathKey;

        /// \brief  (NetworkKeys) - The files key.
        private Config.NetworkKeys filesKey;

        /// \brief  (Keys) - The selected file key.
        private Config.Keys selectedFileKey;
        #endregion
        #region /// \name Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public SourceTargetAlgorithmsSelect()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////



        public FileSelect(SelectSource selectSource, string message)
        {
            inInit = true;
            InitializeComponent();
            this.selectSource = selectSource;
            data = new List<SelectControlData>();
            switch (selectSource)
            {

                // Setting the locals if the call is for start debug
                case SelectSource.Debug:
                    fileExtension = ".debug";
                    pathKey = Config.NetworkKeys.DebugFilePath;
                    filesKey = Config.NetworkKeys.DebugFiles;
                    selectedFileKey = Config.Keys.SelectedDebugFileName;
                    break;

                // Setting the locals if the call is for save during debug
                // (only the file name can be selected)
                case SelectSource.SaveDebug:
                    fileExtension = ".debug";
                    pathKey = Config.NetworkKeys.DebugFilePath;
                    filesKey = Config.NetworkKeys.DebugFiles;
                    Selection_Algorithm.DisableSelection = true;
                    Selection_Subject.DisableSelection = true;
                    Selection_Network.DisableSelection = true;
                    selectedFileKey = Config.Keys.SelectedDebugFileName;
                    break;

                // Setting the locals if the call is for selecting log file
                // (only the file name can be selected)
                case SelectSource.Log:
                    fileExtension = ".log";
                    pathKey = Config.NetworkKeys.LogFilePath;
                    filesKey = Config.NetworkKeys.LogFiles;
                    Selection_Algorithm.DisableSelection = true;
                    Selection_Subject.DisableSelection = true;
                    Selection_Network.DisableSelection = true;
                    selectedFileKey = Config.Keys.SelectedLogFileName;
                    break;

                // Setting the locals if the call is for selecting file to save
                // (The network and file name can be selected)
                case SelectSource.SaveAs:
                    fileExtension = ".data";
                    pathKey = Config.NetworkKeys.DataFilePath;
                    filesKey = Config.NetworkKeys.DataFiles;
                    Selection_Algorithm.DisableSelection = true;
                    Selection_Subject.DisableSelection = true;
                    selectedFileKey = Config.Keys.SelectedDataFileName;
                    break;

                // Setting the parameters for Open and New operations
                default:
                    fileExtension = ".data";
                    pathKey = Config.NetworkKeys.DataFilePath;
                    filesKey = Config.NetworkKeys.DataFiles;
                    selectedFileKey = Config.Keys.SelectedDataFileName;
                    break;
            }            

            // Header Message
            Label_Message.Content = message;
            
            // Create the data for the subjects control
            List<string> subjects = ListStr(Config.Instance[Config.Keys.Subjects].Keys);           
            string subject = Config.Instance[Config.Keys.SelectedSubject];
            List<int> selectedSubjectIdx = new List<int> { subjects.IndexOf(subject) };
            data.Add(new SelectControlData() { options = subjects, initiallySelected = selectedSubjectIdx });

            // Create the data for the algorithm control
            List<string> algorithms = ListStr(Config.Instance[Config.Keys.Subjects][subject].Keys);
            string algorithm = Config.Instance[Config.Keys.SelectedAlgorithm]; 
            List<int> selectedAlgorithmIdx = new List<int> { algorithms.IndexOf(algorithm) };
            data.Add(new SelectControlData { options = algorithms, initiallySelected = selectedAlgorithmIdx });

            // Create the data for the algorithm control
            List<string> networks = ListStr(Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.Networks].Keys);
            string network = Config.Instance[Config.Keys.SelectedNetwork];
            List<int> selectedNetworkIdx = new List<int> { networks.IndexOf(network) };
            data.Add(new SelectControlData { options = networks, initiallySelected = selectedNetworkIdx });

            // Create the data for the algorithm control
            List<string> files = ListFiles(Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.Networks][network][filesKey]);
            string file = Config.Instance[selectedFileKey].Replace(fileExtension, "");
            List<int> selectedFileIdx = new List<int> { files.IndexOf(file) };
            data.Add(new SelectControlData { options = files, initiallySelected = selectedFileIdx });

            Selection_File.Init(data[(int)Data.File]);
            Selection_Network.Init(data[(int)Data.Network]);
            Selection_Algorithm.Init(data[(int)Data.Algorithm]);
            Selection_Subject.Init(data[(int)Data.Subject]);
            inInit = false;
        }
        #endregion
        #region /// \name Utility methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<string> ListStr(dynamic keys)
        ///
        /// \brief List string.
        ///
        /// \par Description.
        ///      List of strings from a list of keys
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param keys  (dynamic) - The keys.
        ///
        /// \return A List&lt;string&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<string> ListStr(dynamic keys)
        {
            List<string> result = new List<string>();
            foreach (object key in keys)
            {
                result.Add(key.ToString());
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<string> ListFiles(AttributeList list)
        ///
        /// \brief List files.
        ///
        /// \par Description.
        ///      List of file names without the extension
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param list  (AttributeList) - The list.
        ///
        /// \return A List&lt;string&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<string> ListFiles(AttributeList list)
        {
            List<string> result = new List<string>();
            foreach (Attribute attribute in list)
            {
                result.Add(attribute.Value.Replace(fileExtension, ""));
            }
            return result;
        }
        #endregion
        #region /// \name Buttons event handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Quit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Quit for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            result = SelectResult.Quit;
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Select_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Select for click events.
        ///
        /// \par Description.
        ///      -  This method gets the results from the controls and  
        ///         -   According to the SelectSource call a method to determine on the action to perform  
        ///         -   According to the action to perform determine:  
        ///             -   If to update the config  
        ///             -   If to exit or continue with the dialog
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            string subject = Selection_Subject.GetSelection().selectionText;
            string algorithm = Selection_Algorithm.GetSelection().selectionText;
            string network = Selection_Network.GetSelection().selectionText;
            string file = Selection_File.GetSelection().selectionText + fileExtension;

            // If the network is new the dictionary of the network is not found in the config
            // so we generate default path 
            string path;
            try
            {
                path = Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.Networks][network][pathKey];
            }
            catch
            {
                path = Config.Instance.GenerateDataFilePath(subject, algorithm, network);
            }

            string absPath = System.IO.Path.GetFullPath(path);

            switch (selectSource)
            {
                case SelectSource.New:
                    result = EndSelectionForNew(absPath, file);
                    break;
                case SelectSource.Open:
                    result = EndSelectionForOpen(absPath, file);
                    break;
                case SelectSource.SaveAs:
                    result = EndSelectionForSaveAs(absPath, file);
                    break;
                case SelectSource.SaveDebug:
                    result = EndSelectionForSaveAs(absPath, file);
                    break;
                case SelectSource.Debug:
                    result = EndSelectionForDebug(absPath, file);
                    break;
                case SelectSource.Log:
                    result = SelectResult.Log;
                    break;
                default:
                    result = SelectResult.Quit;
                    break;
            }

            switch (result)
            {                
                case SelectResult.Quit:
                    Close();
                    break;
                case SelectResult.Cancel:
                    break;
                case SelectResult.Debug:
                    Config.Instance.UpdateSelectedDebugFileChanged(subject, algorithm, network, file, path);
                    Close();
                    break;
                case SelectResult.Log:
                    Config.Instance[Config.Keys.SelectedLogFileName] = file;
                    Close();
                    break;
                default:
                    if (selectSource == SelectSource.SaveDebug)
                    {
                        Config.Instance.UpdateSelectedDebugFileChanged(subject, algorithm, network, file, path);
                    }
                    else
                    {
                        Config.Instance.UpdateSelectedDataFileChanged(subject, algorithm, network, file, path);
                    }
                    Close();
                    break;
            }            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private SelectResult EndSelectionForNew(string absPath, string file)
        ///
        /// \brief Ends selection for new.
        ///
        /// \par Description.
        ///      Determine what will be the end operation when the calling command was New
        ///      -  If the file exist let the user decide whether to override (open new), load, quit or stay in the dialog(Cancel)   
        ///      -  Else return that the New operation is possible
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param absPath  (string) - Full pathname of the abs file.
        /// \param file     (string) - The file.
        ///
        /// \return A SelectResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectResult EndSelectionForNew(string absPath, string file)
        {
            if (File.Exists(absPath + file))
            {
                string selectedAction = CustomizedMessageBox.Show(new List<string> {"The file already exist",
                    "Select Open to open",
                    "Select New to override",
                    "Select Cancel to cancel the selection",
                    "Select Quit to quit the dialog" },
                    "Select File Dialog Message",
                    new List<string> { "Open", "New", "Cancel", "Quit" },
                    Icons.Question);
                return (SelectResult)TypesUtility.GetKeyFromString(typeof(SelectResult), selectedAction);                
            }
            else
            {
                return SelectResult.New;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private SelectResult EndSelectionForOpen(string absPath, string file)
        ///
        /// \brief Ends selection for open.
        ///
        /// \par Description.
        ///      Determine what will be the end operation when the calling command was Open
        ///      -  If the file does not exist let the user decide whether to open new, quit or stay in the dialog(Cancel)
        ///      -  Else return that the Open operation is possible
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param absPath  (string) - Full pathname of the abs file.
        /// \param file     (string) - The file.
        ///
        /// \return A SelectResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectResult EndSelectionForOpen(string absPath, string file)
        {
            if (!File.Exists(absPath + file))
            {
                string selectedAction = CustomizedMessageBox.Show(new List<string> {"The file does not exist",
                    "Select New to open new file",
                    "Select Cancel to cancel the selection",
                    "Select Quit to quit the dialog" },
                    "Select File Dialog Message",
                    new List<string> { "New", "Cancel", "Quit" },
                    Icons.Question);
                return (SelectResult)TypesUtility.GetKeyFromString(typeof(SelectResult), selectedAction);
            }
            else
            {
                return SelectResult.Open;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private SelectResult EndSelectionForSaveAs(string absPath, string file)
        ///
        /// \brief Ends selection for save as.
        ///
        /// \par Description.
        ///      Determine what will be the end operation when the calling command was SaveAs
        ///      -  If the file exist let the user decide whether to override, load, quit or stay in the dialog(Cancel)
        ///      -  Else return that the Save as operation is possible
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param absPath  (string) - Full pathname of the abs file.
        /// \param file     (string) - The file.
        ///
        /// \return A SelectResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectResult EndSelectionForSaveAs(string absPath, string file)
        {
            if (File.Exists(absPath + file))
            {
                string selectedAction = CustomizedMessageBox.Show(new List<string> {"The file already exist",
                    "Select Save to override",
                    "Select Cancel to cancel the selection",
                    "Select Quit to quit the dialog" },
                    "Select File Dialog Message",
                    new List<string> { "Save", "Cancel", "Quit" },
                    Icons.Question);
                return (SelectResult)TypesUtility.GetKeyFromString(typeof(SelectResult), selectedAction);
            }
            else
            {
                return SelectResult.Save;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private SelectResult EndSelectionForDebug(string absPath, string file)
        ///
        /// \brief Ends selection for debug.
        ///
        /// \par Description.
        ///      Check for select debug file - the file must exist
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param absPath  (string) - Full pathname of the abs file.
        /// \param file     (string) - The file.
        ///
        /// \return A SelectResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectResult EndSelectionForDebug(string absPath, string file)
        {
            if (!File.Exists(absPath + file))
            {
                string selectedAction = CustomizedMessageBox.Show(new List<string> {"The file dose not exist",
                    "Select Cancel to cancel the selection",
                    "Select Quit to quit the dialog" },
                    "Select File Dialog Message",
                    new List<string> { "Cancel", "Quit" },
                    Icons.Question);
                return (SelectResult)TypesUtility.GetKeyFromString(typeof(SelectResult), selectedAction);
            }
            else
            {
                return SelectResult.Debug;
            }
        }
        #endregion
        #region /// \name Event handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Selection_Subject_SelectionChanged(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_Subject for selection changed events.
        ///
        /// \par Description.
        ///      Change the Selection_Algorithm control to hold the algorithms of the selected subject
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Selection_Subject_SelectionChanged(object sender, EventArgs e)
        {
            if (!inInit)
            {
                string subject = Selection_Subject.TextBox_Selected.Text;
                data[(int)Data.Algorithm].options = ListStr(Config.Instance[Config.Keys.Subjects][subject].Keys);
                data[(int)Data.Algorithm].enableItems = null;
                data[(int)Data.Algorithm].initiallySelected = new List<int> { 0 };
                Selection_Algorithm.Init(data[(int)Data.Algorithm]);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Selection_Algorithm_SelectionChanged(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_Algorithm for selection changed events.
        ///
        /// \par Description.
        ///      Change the Selection_Network control to hold the networks of the selected algorithm
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Selection_Algorithm_SelectionChanged(object sender, EventArgs e)
        {
            if (!inInit)
            {
                string subject = Selection_Subject.TextBox_Selected.Text;
                string algorithm = Selection_Algorithm.TextBox_Selected.Text;
                data[(int)Data.Network].options = ListStr(Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.Networks].Keys);
                data[(int)Data.Network].enableItems = null;
                data[(int)Data.Network].initiallySelected = new List<int> { 0 };
                Selection_Network.Init(data[(int)Data.Network]);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Selection_Network_SelectionChanged(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_Network for selection changed events.
        ///
        /// \par Description.
        ///      Change the Selection_Files control to hold the files of the network or a default file if
        ///      the network is new
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Selection_Network_SelectionChanged(object sender, EventArgs e)
        {
            if (!inInit)
            {
                string subject = Selection_Subject.TextBox_Selected.Text;
                string algorithm = Selection_Algorithm.TextBox_Selected.Text;
                string network = Selection_Network.TextBox_Selected.Text;
                List<string> files;
                
                // If the network is new it is not found in the config so we have to create a new file
                // for it. The name of the file is the same as the network
                try
                {
                   files = ListFiles(Config.Instance[Config.Keys.Subjects][subject][algorithm][Config.AlgorithmKeys.Networks][network][filesKey]);
                }
                catch
                {
                    files = new List<string> { network};
                }

                data[(int)Data.File].options = files;
                data[(int)Data.File].enableItems = null;
                data[(int)Data.File].initiallySelected = new List<int> { 0 };
                Selection_File.Init(data[(int)Data.File]);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Selection_Network_NewItemSelected(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_Network for new item selected events.
        ///
        /// \par Description.
        ///      When a new item was created in the network control (a network that does not exist)
        ///      Create a file name the same as the network and insert it to the Selection_File control
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Selection_Network_NewItemSelected(object sender, EventArgs e)
        {
            string text = Selection_Network.TextBox_Selected.Text;
            List<string> files = new List<string> { text };
            data[(int)Data.File].options = files;
            data[(int)Data.File].enableItems = null;
            data[(int)Data.File].initiallySelected = new List<int> { 0 };
            Selection_File.Init(data[(int)Data.File]);
        }
#endregion
    }
}
