////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\configwindow.cs
///
/// \brief Implements the ConfigWindow class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;


namespace DistributedAlgorithms
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ConfigWindow
    ///
    /// \brief Form for viewing the configuration.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 01/11/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ConfigWindow : ElementWindow
    {
        #region /// \name Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true if selection changed.
        ///        is the selected subject, algorithm, network, data file where changed.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool selectionChanged = false; 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to force save.
        ///        Force the save to the config file when exiting.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool forceSave = false;

#endregion
        #region /// \name Constructor And Init window

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ConfigWindow() : base(new List<NetworkElement>()
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
        /// \date 01/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ConfigWindow()
            : base(new List<NetworkElement>() { Config.Instance }, InputWindows.ConfigWindow)
        {
            InitWindow();
            InitialExpand();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CreateButtons()
        ///
        /// \brief Creates the buttons.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CreateButtons()
        {
            CreateButton("Button_NewDataFolder", "New Data Folder", Button_NewDataFolder_Click);
            CreateButton("Button_Scan", "Scan Data Folder", Button_Scan_Click);
            CreateButton("Button_Corralate", "Correlate with existing algorithms", Button_Correlate_Click);
            //CreateButton("Button_UpdateSelected", "Set the selected algorithm files", Button_SetSelectedFiles_Click);
            CreateButton("Button_ResetToDefault", "Reset To Default", Button_ResetToDefault_Click);
            CreateButton("Button_ResetToSaved", "Reset To Saved", Button_ResetToSaved_Click);
            CreateButton("Button_FillAlgorithms", "Apply", Button_Apply_Click);
            CreateButton("Button_Save", "Save", Button_Save_Click);
            CreateButton("Button_Exit", "Exit", Button_Exit_Click);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override bool ScanCondition(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Scans a condition.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ScanCondition(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            return dictionary == NetworkElement.ElementDictionaries.PrivateAttributes;
        }

        #endregion
        #region /// \name Buttons Event handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_NewDataFolder_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_NewDataFolder for click events.
        ///
        /// \par Description.
        ///      Creates a new Algorithm Data folder
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_NewDataFolder_Click(object sender, RoutedEventArgs e)
        {
            if (CustomizedMessageBox.Show(new List<string> {"The creation of an algorithms pass will cause save to the config",
                "Do you want to continue ?" },
                "Select Algorithms Path Message",
                MessageBoxButton.YesNo,
                Icons.Question) == MessageBoxResult.No)
            {
                return;
            }
                
            ((Config)networkElements[0]).NewDataFolder();
            ResetToExisting();
            InitialExpand();
            forceSave = false;
            selectionChanged = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Scan_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Scan for click events.
        ///
        /// \par Description.
        ///      Scan the current Algorithm Data directory
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Scan_Click(object sender, RoutedEventArgs e)
        {
            if (CustomizedMessageBox.Show(new List<string> {"The current Algorithms Data will be scanned and the config file will be saved",
                "Do you want to continue ?" },
                "Select Algorithms Path Message",
                MessageBoxButton.YesNo,
                Icons.Question) == MessageBoxResult.Yes)
            {
                ((Config)networkElements[0]).AddAlgorithmsData(false);
                ResetToExisting();
                InitialExpand();
                forceSave = false;
                selectionChanged = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Correlate_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Correlate for click events.
        ///
        /// \par Description.
        ///      Correlate between the algorithm programed and the Algorithm Data directory:
        ///      -  Remove folders of algorithms that do not exist  
        ///      -  Add folders to new algorithms
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Correlate_Click(object sender, RoutedEventArgs e)
        {
            if (CustomizedMessageBox.Show(new List < string > { "The Algorithms Data directory structure will correlate with the existing algorithms",
                "Do you want to continue ?"},
                "Select Algorithms Path Message",
                MessageBoxButton.YesNo,
                Icons.Question) == MessageBoxResult.Yes)
            {
                ((Config)networkElements[0]).UpdateAlgorithmsDataDirectory(null, false);
                ResetToExisting();
                InitialExpand();
                forceSave = false;
                selectionChanged = true;
            }
        }        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_ResetToDefault_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ResetToDefault for click events.
        ///
        /// \par Description.
        ///      Reset the Config data and directory to the default:
        ///      -  Reset the config parameters to the defaults  
        ///      -  Correlate the Algorithm Data directory 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_ResetToDefault_Click(object sender, RoutedEventArgs e)
        {
            ((Config)networkElements[0]).ResetToDefault();
            selectionChanged = true;
            ResetToExisting();
            InitialExpand();
            forceSave = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_ResetToSaved_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ResetToSaved for click events.
        ///
        /// \par Description.
        ///      Reset the Config to the last saved config file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_ResetToSaved_Click(object sender, RoutedEventArgs e)
        {
            ((Config)networkElements[0]).LoadConfig();
            ResetToExisting();
            InitialExpand();
            forceSave = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Apply_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Apply for click events.
        ///
        /// \par Description.
        ///      If changes were made to the controls values, this method update the Config from the controls 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            UpdateNetworkElement();
            InitialExpand();
            forceSave = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Save_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Save for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            UpdateNetworkElement();
            InitialExpand();
            ((Config)networkElements[0]).SaveConfig();
            forceSave = false;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Exit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Exit for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            bool configUpdated = Exit(forceSave);
            if (configUpdated)
                ((Config)networkElements[0]).SaveConfig();
        }
        #endregion
        #region /// \name Attribute buttons click methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_SelectAlgorithmsPath_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_SelectAlgorithmsPath for click events.
        ///
        /// \par Description.
        ///      -  Change the selected Algorithm Data path.  
        ///      -  This method is activated when the Algorithm Data Path attribute's button is clicked
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Button_SelectAlgorithmsPath_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configWindow = (ConfigWindow)GetWindow((Button)sender);
            Config config = (Config)configWindow.networkElements[0];
            if (CustomizedMessageBox.Show("The selecting of an algorithms pass will cause save to the config \n Do you want to continue ?",
                "Select Algorithms Path Message",
                MessageBoxButton.YesNo,
                Icons.Question) == MessageBoxResult.Yes)
            {
                if (config.AddAlgorithmsData())
                {
                    config.CheckAndSetSelected();
                    configWindow.selectionChanged = true;
                    configWindow.ResetToExisting();
                    configWindow.InitialExpand();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_SelectSelectedAlgorithmDataFile_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_SelectSelectedAlgorithmDataFile for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Button_SelectSelectedAlgorithmDataFile_Click(object sender, RoutedEventArgs e)
        {
            // Get the config and config window
            ConfigWindow configWindow = (ConfigWindow)GetWindow((Button)sender);
            Config config = (Config)configWindow.networkElements[0];

            // Get the data file name and path from the buttons
            Button fileNameButton = (Button)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedDataFileName)).newValueControl;
            string selectedAlgorithmDataFileName = (string)fileNameButton.Content;
            Button filePathButton = (Button)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedDataPath)).newValueControl;
            string selectedAlgorithmDataPath = (string)filePathButton.Content;

            // Get the subject and the algorithm names
            ComboBox subjectComboBox = (ComboBox)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedSubject)).newValueControl;
            string subjectName = (string)subjectComboBox.SelectedItem;
            ComboBox algorithmComboBox = (ComboBox)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedAlgorithm)).newValueControl;
            string algorithmName = (string)algorithmComboBox.SelectedItem;

            // Select the file
            if (FileUtilities.SelectInputFile("data", ref selectedAlgorithmDataFileName, ref selectedAlgorithmDataPath))
            {
                // Update the buttons
                configWindow.UpdateNewValueChanged(fileNameButton, selectedAlgorithmDataFileName);
                fileNameButton.Content = selectedAlgorithmDataFileName;
                configWindow.UpdateNewValueChanged(filePathButton, selectedAlgorithmDataPath);
                filePathButton.Content = selectedAlgorithmDataPath;
                configWindow.selectionChanged = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void Button_SelectSelectedAlgorithmDebugFile_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_SelectSelectedAlgorithmDebugFile for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Button_SelectSelectedAlgorithmDebugFile_Click(object sender, RoutedEventArgs e)
        {
            // Get the config and config window
            ConfigWindow configWindow = (ConfigWindow)GetWindow((Button)sender);
            Config config = (Config)configWindow.networkElements[0];

            // Get the data file name and path from the buttons
            Button debugfileNameButton = (Button)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedDebugFileName)).newValueControl;
            string selectedAlgorithmDebugFileName = (string)debugfileNameButton.Content;
            Button debugfilePathButton = (Button)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedDebugPath)).newValueControl;
            string selectedAlgorithmDebugPath = (string)debugfilePathButton.Content;

            // Get the subject and the algorithm names
            ComboBox subjectComboBox = (ComboBox)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedSubject)).newValueControl;
            string subjectName = (string)subjectComboBox.SelectedItem;
            ComboBox algorithmComboBox = (ComboBox)configWindow.controlsAttributeLinks.Values.First(link => TypesUtility.CompareDynamics(link.key, Config.Keys.SelectedAlgorithm)).newValueControl;
            string algorithmName = (string)algorithmComboBox.SelectedItem;

            // Select the file
            if (FileUtilities.SelectInputFile("debug", ref selectedAlgorithmDebugFileName, ref selectedAlgorithmDebugPath))
            {
                // Update the buttons
                configWindow.UpdateNewValueChanged(debugfileNameButton, selectedAlgorithmDebugFileName);
                debugfileNameButton.Content = selectedAlgorithmDebugFileName;
                configWindow.UpdateNewValueChanged(debugfilePathButton, selectedAlgorithmDebugPath);
                debugfilePathButton.Content = selectedAlgorithmDebugPath;
                configWindow.selectionChanged = true;
            }
        }
        #endregion
        #region /// \name Utilities
        private void InitialExpand()
        {
            ItemsControl item = controlsAttributeLinks.Values.First(l => TypesUtility.CompareDynamics(l.key, NetworkElement.ElementDictionaries.PrivateAttributes)).item;
            ((TreeViewItem)item).IsExpanded = true;
        }
        #endregion
    }
}

