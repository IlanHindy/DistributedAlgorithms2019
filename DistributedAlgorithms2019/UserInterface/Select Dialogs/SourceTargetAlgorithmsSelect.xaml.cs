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

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class SourceTargetAlgorithmsSelect
    ///
    /// \brief Interaction logic for SourceTargetAlgorithmsSelect.xaml.
    ///
    /// \par Description.
    ///      -  This class is used to get the source and target subjects and algorithms
    ///         for the AddAlgorithmWindow
    ///      -  It holds 4 SelectControl 
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 29/08/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class SourceTargetAlgorithmsSelect : Window
    {
        public enum SelectResult { Quit, Select };
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (SelectResult) - The result.
        ///        The result of the dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SelectResult result = SelectResult.Quit;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;SelectControlData&gt;) - The data.
        ///        The data used to initialize the control or update the controls after selection.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected List<SelectControlData> data;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;SelectResults&gt;) - The select results.
        ///        A list of the selected result from all the controls.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<SelectResults> selectResults;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to replace user code files.
        ///        true if to replace the user code files (not the auto generated file)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool replaceUserCodeFiles;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to new algorithm.
        ///        When selecting button is pressed this variable is updated for the main dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool newAlgorithm;
 
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

        public SourceTargetAlgorithmsSelect()
        {
            InitializeComponent();
            List<SelectControlData> data = new List<SelectControlData>();

            // Create a list of the subjects and an enable list for them
            List<string> subjects = TypesUtility.GetAlgorithmsSubjects();
            var enableSubjectsSource = Enumerable.Repeat(true, subjects.Count).ToList();
            

            // Create the data for the source subject control
            // For the source subject the System subject is disabled (It cannot be used as source)
            enableSubjectsSource[subjects.IndexOf("Base")] = false;
            List<int> initialySelectedSubject = new List<int> { subjects.IndexOf("Templates") };
            data.Add(new SelectControlData() { options = subjects, enableItems = enableSubjectsSource, initiallySelected = initialySelectedSubject  });

            // Create the data for the control of the source algorithm
            data.Add(new SelectControlData { options = new List<string>() });

            // Create the data for the control of the target subject
            // For the target subject the System and Templates subjects are diasabled
            List<string> allSubjects = subjects.Concat(Config.Instance.GetSubjects().ToList()).ToList();
            allSubjects = allSubjects.Distinct().ToList();
            var enableSubjectsTragets = Enumerable.Repeat(true, allSubjects.Count).ToList();
            enableSubjectsTragets[allSubjects.IndexOf("Base")] = false;
            enableSubjectsTragets[allSubjects.IndexOf("Templates")] = false;            
            data.Add(new SelectControlData { options = allSubjects, enableItems = enableSubjectsTragets });

            // Create the data for the control of the target algorithm
            data.Add(new SelectControlData { options = new List<string>() });
            this.data = data;

            // Order of changing the presentation according to selection change is as follows
            // Source subject change : 
            //      The Source algorithms control change (To present the algorithms of the subject)
            //      The Target subject control change (To set the Target subject is set to the source subject
            // Target subject change :
            //      The Target algorithms control change (To present the algorithms of the subject)
            //
            // There for the order of initialize of the controls is in reverse order
            // (So the initiation will not override the data that was set by a control that is before
            // in the change chain)  
            Selection_TargetAlgorithm.Init(data[3]);
            Selection_TargetSubject.Init(data[2]);
            Selection_SourceAlgorithm.Init(data[1]);
            Selection_SourceSubject.Init(data[0]);
        }

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
            // If the source and the target are not equal generate a message that the user code
            // will be overridden
            List<string> algorithms = TypesUtility.GetAlgorithms();
            string targetAlgorithm = Selection_TargetSubject.GetSelection().selectionText + "." + Selection_TargetAlgorithm.GetSelection().selectionText;
            string sourceAlgorithm = Selection_SourceSubject.GetSelection().selectionText + "." + Selection_SourceAlgorithm.GetSelection().selectionText;
            if (algorithms.Any(a => a == targetAlgorithm))
            {
                newAlgorithm = false;
                if (sourceAlgorithm == targetAlgorithm)
                {
                    string messageResult = CustomizedMessageBox.Show(new List<string> {"The algorithm already exist",
                        "Your code files Should not be re-generated",
                        "Select Replace to regenerate the user code files (and delete your implementation)",
                        "Select OK to replace only the automatically generated code file",
                        "Select Cancel to replace the selection" },
                        "SourceTargetAlgorithmsSelect Message",
                        new List<string> { "Replace", "OK", "Cancel" },
                        Icons.Question);
                    switch (messageResult)
                    {
                        case "Replace":
                            replaceUserCodeFiles = true;
                            break;
                        case "OK":
                            replaceUserCodeFiles = false;
                            break;
                        case "Cancel":
                            return;
                    }
                }
                else
                {
                    string messageResult = CustomizedMessageBox.Show(new List<string> {"The algorithm already exist",
                        "You selected another algorithm as your source algorithm",
                        "Select Replace to replace the code files of the existing algorithm (Backup will be produced)",
                        "Select OK to replace only the auto generated code file",
                        "Select Cancel to replace the selection" },
                         "SourceTargetAlgorithmsSelect Message",
                         new List<string> { "Replace", "OK", "Cancel" },
                         Icons.Question);
                    switch (messageResult)
                    {
                        case "Replace":
                            replaceUserCodeFiles = true;
                            break;
                        case "OK":
                            replaceUserCodeFiles = false;
                            break;
                        case "Cancel":
                            return;
                    }
                }
            }
            else
            {
                newAlgorithm = true;
                replaceUserCodeFiles = true;
            }

            selectResults = new List<SelectResults>();
            selectResults.Add(Selection_SourceSubject.GetSelection());
            selectResults.Add(Selection_SourceAlgorithm.GetSelection());
            selectResults.Add(Selection_TargetSubject.GetSelection());
            selectResults.Add(Selection_TargetAlgorithm.GetSelection());
            result = SelectResult.Select;
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Selection_SourceSubject_SelectionChanged(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_SourceSubject for selection changed events.
        ///
        /// \par Description.
        ///      -  The Source algorithms control change (To present the algorithms of the subject)
        ///      -  The Target subject control change (To set the Target subject is set to the source subject
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Selection_SourceSubject_SelectionChanged(object sender, EventArgs e)
        {
            data[1].options = TypesUtility.GetAlgorithmsOfSubject(Selection_SourceSubject.TextBox_Selected.Text);
            data[1].enableItems = null;
            Selection_SourceAlgorithm.Init(data[1]);
            if (((ListBoxItem)Selection_TargetSubject.ListBox_Options.Items[Selection_SourceSubject.ListBox_Options.SelectedIndex]).IsEnabled)
            {
                Selection_TargetSubject.ListBox_Options.SelectedIndex = Selection_SourceSubject.ListBox_Options.SelectedIndex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Selection_SourceAlgorithm_SelectionChanged(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_SourceAlgorithm for selection changed events.
        ///
        /// \par Description.
        ///      Change the target algorithm if the source algorithm changed
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/01/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Selection_SourceAlgorithm_SelectionChanged(object sender, EventArgs e)
        {
            if (((ListBoxItem)Selection_TargetAlgorithm.ListBox_Options.Items[Selection_SourceAlgorithm.ListBox_Options.SelectedIndex]).IsEnabled)
            {
                Selection_TargetAlgorithm.ListBox_Options.SelectedIndex = Selection_SourceAlgorithm.ListBox_Options.SelectedIndex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Selection_TargetSubject_SelectionChanged(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_TargetSubject for selection changed events.
        ///
        /// \par Description.
        ///      The Target algorithms control change (To present the algorithms of the subject)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Selection_TargetSubject_SelectionChanged(object sender, EventArgs e)
        {
            string subject = Selection_TargetSubject.TextBox_Selected.Text;
            if ( subject != "")
            {                
                List<string> options = TypesUtility.GetAlgorithmsOfSubject(subject);
                data[3].options = options.Concat(Config.Instance.GetAlgorithmsOfSubject(subject)).Distinct().ToList();
            }
            else
            {
                data[3].options = new List<string>();
            }
            data[3].enableItems = null;
            Selection_TargetAlgorithm.Init(data[3]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Selection_TargetSubject_NewItemCreated(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Selection_TargetSubject for new item created events.
        ///
        /// \par Description.
        ///      If there is a new target subject empty the target algorithm
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 13/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Selection_TargetSubject_NewItemCreated(object sender, EventArgs e)
        {
            string text = Selection_TargetSubject.TextBox_Selected.Text;
            List<string>  options = new List<string> { text };
            data[3].options = options;
            data[3].enableItems = null;
            data[3].initiallySelected = new List<int> { 0 };
            Selection_TargetAlgorithm.Init(data[3]);
        }
    }
}
