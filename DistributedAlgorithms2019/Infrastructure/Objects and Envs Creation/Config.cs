////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Config.cs
///
/// \brief Implements the configuration class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class Config
    ///
    /// \brief A configuration.
    ///        The division of responsibility between the ClassFactory and the Config is:
    ///        -    The ClassFactory is responsible to the code  
    ///        -    The Config is responsible to the data 
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 01/11/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Config : NetworkElement
    {
        #region /// \name Default config file name and path

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Filename and path of the configuration file.
        ///        The program will attempt to load the config from this location. If the file
        ///        does not exist it will ask for an alternative file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string configFileName = "DistributedAlgorithmsConfig.config";

        private static string configFilePath = "~\\..\\..\\..\\Program Data\\Config\\";
        #endregion
        #region /// \enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Keys
        ///
        /// \brief The keys of the config
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Keys
        {
            SelectedAlgorithm,
            SelectedSubject,
            SelectedNetwork,
            SelectedDataPath,
            SelectedDataFileName,
            SelectedDebugPath,
            SelectedDebugFileName,
            SelectedLogPath,
            SelectedLogFileName,
            EditLogFileName,
            EditLogFilePath,
            AlgorithmsPath,
            AlgorithmsDataPath,
            ProgramDocsViewerPath,
            ProgramDocsViewer,
            EmptyDocFileName,
            EmptyDocFilePath,
            HtmlDocPath,
            Subjects
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum AlgorithmKeys
        ///
        /// \brief The keys of the dictionary of the algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum AlgorithmKeys
        {
            Name,
            Subject,
            Path,
            DocSourceFiles,
            DocSourcePath,
            DocProcessedFiles,
            DocProcessedPath,
            PseudoCodeFiles,
            PseudoCodePath,
            Networks
        };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum NetworkKeys
        ///
        /// \briefThe keys of the dictionary of the network
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum NetworkKeys
        {
            Name,
            Algorithm,
            Subject,
            Path,
            LastDataFile,
            DataFiles,
            DataFilePath,
            LastDebugFile,
            DebugFiles,
            DebugFilePath,
            LastLogFile,
            LogFiles,
            LogFilePath
        };

        #endregion
        #region /// \name Singleton implementing

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Config) - The instance.
        ///        A Singleton implementation is done at the following way:
        ///        -#  Define a static private member of the class which is the class itself
        ///        -#  The constructor to the class is private
        ///        -#  The access to the class is through a static method that returns the static member.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static Config instance;
        private Config() : base(true)
        {
            InitConfig();
        }

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Config();
                }
                return instance;
            }
        }
        #endregion
        #region /// \name indexers to access the config keys

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public new dynamic this(dynamic key)
        ///
        /// \brief Indexer to get or set items within this collection using array index syntax.
        ///
        /// \param key  (dynamic) - The key.
        ///
        /// \return The indexed item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new dynamic this[dynamic key]
        {
            get { return pa[key]; }
            set { pa[key] = value; }
        }
        #endregion
        #region /// \name Constructor, Init and load config file


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void InitConfig()
        ///
        /// \brief Init configuration.
        ///
        /// \par Description.
        ///      -  The default values of the config are found in the ElementAttribute dictionary.  
        ///      -  This method initialize these default values  
        ///      -  The default values are used to initiate/reset the config
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void InitConfig()
        {
            ea.Clear();
            ea.Add(Keys.SelectedSubject, new Attribute() { Value = "Base", EndInputOperation = SubjectChanged, ElementWindowPrmsMethod = SubjectComboBoxSetting });
            ea.Add(Keys.SelectedAlgorithm, new Attribute() { Value = "Base", EndInputOperation = AlgorithmChanged, ElementWindowPrmsMethod = AlgorithmComboBoxSetting });
            ea.Add(Keys.SelectedNetwork, new Attribute() { Value = "Base", EndInputOperation = NetworkChanged, ElementWindowPrmsMethod = NetworkComboBoxSetting });
            ea.Add(Keys.SelectedDataPath, new Attribute() { Value = "~\\..\\..\\..\\Algorithms Data\\Base\\Base\\Networks\\Base\\Data\\", Editable = false });
            ea.Add(Keys.SelectedDataFileName, new Attribute() { Value = "Base.data", Editable = false });
            ea.Add(Keys.SelectedLogPath, new Attribute() { Value = "~\\..\\..\\..\\Algorithms Data\\Base\\Base\\Networks\\Base\\Logs\\", Editable = false });
            ea.Add(Keys.SelectedLogFileName, new Attribute() { Value = "Base.log", Editable = false });
            ea.Add(Keys.SelectedDebugPath, new Attribute() { Value = "~\\..\\..\\..\\Algorithms Data\\Base\\Base\\Base\\Networks\\Base\\Debug\\", Editable = false });
            ea.Add(Keys.SelectedDebugFileName, new Attribute() { Value = "Base.debug", Editable = false });
            ea.Add(Keys.EditLogFilePath, new Attribute() { Value = "~\\..\\..\\..\\Program Data\\Operation Logs\\", Editable = false });
            ea.Add(Keys.EditLogFileName, new Attribute() { Value = "EditLog.log", Editable = false });
            ea.Add(Keys.AlgorithmsPath, new Attribute() { Value = "~\\..\\..\\..\\Algorithms\\", Editable = false });
            ea.Add(Keys.AlgorithmsDataPath, new Attribute() { Value = "~\\..\\..\\..\\Algorithms Data\\", ElementWindowPrmsMethod = ButtonControlSetting });
            ea.Add(Keys.HtmlDocPath, new Attribute() { Value = "..\\docs\\html", Editable = false });
            ea.Add(Keys.EmptyDocFileName, new Attribute() { Value = "Empty Word Document.docx", Editable = false });
            ea.Add(Keys.EmptyDocFilePath, new Attribute() { Value = "~\\..\\..\\..\\Program Data\\Word Templates\\", Editable = false });
            ea.Add(Keys.ProgramDocsViewer, new Attribute() { Value = "ViewDocuments.bat", Editable = false });
            ea.Add(Keys.ProgramDocsViewerPath, new Attribute() { Value = "~\\..\\..\\..\\External Tools\\Docs\\", Editable = false });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void LoadConfig()
        ///
        /// \brief Loads the configuration.
        ///
        /// \par Description.
        ///      -  This method is used to Load the config when the program starts  
        ///      -  It does :  
        ///         -#  Load the config from the config file
        ///         -#  If the config file do not exist it asks for a config file to load
        ///         -#  if the load of the config file failed it fills the missing data
        ///         -#  The missing data is divided to 2 parts:
        ///             -#  The main (PrivateAttribute) dictionary which is filled from the backup (ElementAttributes)
        ///             -#  The Algorithm Data directory reading
        ///             -#  If the reading of the Algorithm Data directory failed the method generates a new one
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadConfig()
        {

            // This flag is used in order to block the failed to load message if the file is new
            bool newFile = false;
            if (!File.Exists(configFilePath + configFileName))
            {
                CustomizedMessageBox.FileMsg("The config file is not found in default place \n",
                    configFileName, configFilePath, 
                    "Select config file",
                    "Config Message", null, Icons.Error);

                // If it does not exist ask for a config file name
                FileUtilities.SelectInputFile("config", ref configFileName, ref configFilePath, false);
                newFile = true;
            }

            try
            {
                // Try to load the config from the file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilePath + configFileName);
                XmlElement rootNode = xmlDoc.DocumentElement;
                Load(rootNode);
                AddNotExistAttributes();
                CustomizedMessageBox.Show("The config file was successfully loaded", "Config Message", Icons.Info);
            }
            catch (Exception e)
            {
                if (!newFile)
                {
                    CustomizedMessageBox.FileMsgErr("Error while loading the config file from :",
                         configFileName,
                         configFilePath,
                         e.Message,
                         "Config Message");
                }

                // In case that the loading of the file failed
                // There is a need to create the dictionary
                // The creation of the dictionary is made from 2 parts:
                AddNotExistAttributes();

                // Get the Algorithm Data directory
                if (AddAlgorithmsData())
                {
                    //Check the selected value's setting. Correct them if they do not exist
                    CheckAndSetSelected();

                    // Save the file
                    SaveConfig();
                }
                else
                {
                    NewDataFolder(false);
                    SaveConfig();
                }
            }


                // If the Algorithm Data directory exist - Get the files from it
                //if (Directory.Exists(Path.GetFullPath(pa[Keys.AlgorithmsDataPath])))
                //{
                //    AddAlgorithmsData(false);

                //    // Check the selected value's setting. Correct them if they do not exist
                //    CheckAndSetSelected();

                //    // Save the file
                //    SaveConfig();
                //}

                // If the Algorithm Data directory does not exist - Generate a Data directory
            //    else
            //    {
            //        NewDataFolder(false);
            //        SaveConfig();
            //    }
            //}

            // The sort is needed if we added new entry which is not the last attribute
            // Before the sort the order is :
            // The attributes from the file and then the rest of the attributes
            // After the sort the order is according to the enum
            pa.Sort();
        }

        #endregion
        #region /// \name Actions on the config

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ResetToDefault()
        ///
        /// \brief Resets to default.
        ///        This method is resetting the config:
        ///        -#   Copy the config parameters from the default (ElementAttributes)
        ///        -#   In the Algorithm Data folder it:
        ///             -#  Removes the folders of algorithms that do not exist
        ///             -#  Add folders for the new algorithms 
        ///        -#   Read the updated Algorithm Data folder and updates the config file
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

        public void ResetToDefault()
        {
            InitConfig();
            pa.Clear();
            AddNotExistAttributes();
            AddAlgorithmsData(false);
            CheckAndSetSelected();
            UpdateAlgorithmsDataDirectory(null, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void AddNotExistAttributes()
        ///
        /// \brief Adds not exist attributes.
        ///
        /// \par Description.
        ///      Given a partial (or empty) config attributes dictionary (PrivateAttributes), this method
        ///      completes the non existing attributes from the defaults (ElementAttributes)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddNotExistAttributes()
        {
            // Fill all the attributes that are not found in the file
            foreach (var entry in ea)
            {
                if (!pa.ContainsKey(entry.Key))
                {
                    Attribute newAttribute = new Attribute
                    {
                        Value = entry.Value.Value,
                        Editable = entry.Value.Editable,
                        IncludedInShortDescription = entry.Value.IncludedInShortDescription,
                        EndInputOperation = entry.Value.EndInputOperation,
                        ElementWindowPrmsMethod = entry.Value.ElementWindowPrmsMethod
                    };
                    pa.Add(entry.Key, newAttribute);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool UpdateAlgorithmsDataDirectory(bool getAlgorithmsPath = true)
        ///
        /// \brief Updates the Algorithms Data folder 
        ///
        /// \par Description.
        ///      This method does the following:
        ///        -#   If asked prompt to retrieve a Algorithm Data folder
        ///        -#   Scans the Algorithm Data folder and:
        ///             -#  Adds folders for the new algorithms
        ///             -#  Removes folders of not existing algorithms
        ///             -#  Updates the config file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param getAlgorithmsPath (Optional)  (bool) - true to get algorithms path.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool UpdateAlgorithmsDataDirectory(List<string> algorithmsInSelectedDirectory = null, bool getAlgorithmsPath = true)
        {
            //First Select the new algorithm data an get it's data

            if (!AddAlgorithmsData(getAlgorithmsPath))
            {
                return false;
            }

            // Get the algorithms list from the selected directory
            if (algorithmsInSelectedDirectory == null)
            {
                algorithmsInSelectedDirectory = GetAlgorithmsNames();
            }

            // Get the list of the algorithms
            List<string> existingAlgorithms = TypesUtility.GetAlgorithms();

            // Get a list of algorithms that are not found in the Algorithms data directory
            List<string> algorithmsToAdd = existingAlgorithms.Except(algorithmsInSelectedDirectory).ToList();

            // Create directory for all the algorithms that are not found in the directory
            foreach (string algorithmName in algorithmsToAdd)
            {
                FileUtilities.CreateDefaultDirsForNetwork(algorithmName, pa[Keys.AlgorithmsDataPath]);
            }

            // Delete all the none existing algorithms
            List<string> algorithmsToRemove = algorithmsInSelectedDirectory.Except(existingAlgorithms).ToList();
            foreach (string algorithmName in algorithmsToRemove)
            {
                FileUtilities.RemoveAlgorithmDataDirectory(algorithmName, pa[Keys.AlgorithmsDataPath]);
            }

            // Update the config
            if (!AddAlgorithmsData(false))
            {
                return false;
            }
            CheckAndSetSelected();
            return true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool AddAlgorithmsData(bool getAlgorithmsDataPath = true)
        ///
        /// \brief Adds the algorithms data.
        ///
        /// \par Description.
        ///      This method scans the Algorithm Data folder and updates the config
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param getAlgorithmsDataPath (Optional)  (bool) - true to get algorithms data path.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool AddAlgorithmsData(bool getAlgorithmsDataPath = true)
        {
            // Get the algorithms path
            string algorithmsDataPath = pa[Keys.AlgorithmsDataPath];

            // Select algorithms path if needed
            if (getAlgorithmsDataPath)
            {
                if (!FileUtilities.SelectFolder(false,
                    "Select the algorithms base folder",
                    ref algorithmsDataPath))
                {
                    return false;
                }
            }

            try
            {
                // Retrieve the subjects
                List<string> subjectsFolders = FileUtilities.GetSubDirectories(algorithmsDataPath);
                AttributeDictionary subjects = new AttributeDictionary();
                foreach (string subjectFolder in subjectsFolders)
                {
                    AttributeDictionary algorithms = new AttributeDictionary();
                    string subjectName = subjectFolder.Replace(algorithmsDataPath, "");
                    subjects.Add(subjectName, new Attribute { Value = algorithms, Editable = false, ElementWindowPrmsMethod = SubjectSetting });

                    // Get the list of algorithms
                    List<string> algorithmsFolders = FileUtilities.GetSubDirectories(subjectFolder);
                    foreach (string algorithmFolder in algorithmsFolders)
                    {

                        // Algorithm name
                        string algorithmName = algorithmFolder.Replace(subjectFolder + "\\", "");

                        // Add the algorithm to the subject dictionary
                        AttributeDictionary algorithmDictionary = new AttributeDictionary();
                        algorithms.Add(algorithmName, new Attribute { Value = algorithmDictionary, Editable = false, ElementWindowPrmsMethod = AlgorithmSetting });

                        // Algorithm General Data
                        algorithmDictionary.Add(AlgorithmKeys.Name, new Attribute { Value = algorithmName, Editable = false });
                        algorithmDictionary.Add(AlgorithmKeys.Subject, new Attribute { Value = subjectName, Editable = false });
                        algorithmDictionary.Add(AlgorithmKeys.Path, new Attribute { Value = algorithmFolder, Editable = false });

                        // Documentation source file names
                        AttributeList documentationSourceFiles = FilesList(FileUtilities.GetAllFileNames(GenerateSourceDocPath(GenerateDocsPath(subjectName, algorithmName)), "docx"));
                        algorithmDictionary.Add(AlgorithmKeys.DocSourceFiles, new Attribute() { Value = documentationSourceFiles, ElementWindowPrmsMethod = ListItemSetting });
                        algorithmDictionary.Add(AlgorithmKeys.DocSourcePath, new Attribute() { Value = GenerateSourceDocPath(GenerateDocsPath(subjectName, algorithmName)), Editable = false });

                        // Documentation processed file names
                        AttributeList documentationProcessedFiles = FilesList(FileUtilities.GetAllFileNames(GenerateProcessedDocPath(GenerateDocsPath(subjectName, algorithmName)), "docx"));
                        algorithmDictionary.Add(AlgorithmKeys.DocProcessedFiles, new Attribute() { Value = documentationProcessedFiles, ElementWindowPrmsMethod = ListItemSetting });
                        algorithmDictionary.Add(AlgorithmKeys.DocProcessedPath, new Attribute() { Value = GenerateProcessedDocPath(GenerateDocsPath(subjectName, algorithmName)), Editable = false });

                        // Pseudo-code file
                        AttributeList pseudoCodeFiles = FilesList(FileUtilities.GetAllFileNames(GeneratePseudoCodePath(GenerateDocsPath(subjectName, algorithmName)), "docx"));
                        algorithmDictionary.Add(AlgorithmKeys.PseudoCodeFiles, new Attribute() { Value = pseudoCodeFiles, ElementWindowPrmsMethod = ListItemSetting });
                        algorithmDictionary.Add(AlgorithmKeys.PseudoCodePath, new Attribute() { Value = GeneratePseudoCodePath(GenerateDocsPath(subjectName, algorithmName)), Editable = false });

                        // Networks Dictionary
                        AttributeDictionary networks = new AttributeDictionary();
                        algorithmDictionary.Add(AlgorithmKeys.Networks, new Attribute() { Value = networks, ElementWindowPrmsMethod = NetworksSetting });
                        // Get the list of algorithms
                        List<string> networksFolders = FileUtilities.GetSubDirectories(algorithmFolder + "\\Networks");
                        foreach (string networkFolder in networksFolders)
                        {

                            AttributeDictionary networkDictionary = new AttributeDictionary();

                            // Network name
                            string networkName = networkFolder.Replace(algorithmFolder + "\\Networks\\", "");

                            // Network General Data
                            networkDictionary.Add(NetworkKeys.Name, new Attribute { Value = networkName, Editable = false });
                            networkDictionary.Add(NetworkKeys.Algorithm, new Attribute { Value = algorithmName, Editable = false });
                            networkDictionary.Add(NetworkKeys.Subject, new Attribute { Value = subjectName, Editable = false });
                            networkDictionary.Add(NetworkKeys.Path, new Attribute { Value = networkFolder, Editable = false });

                            // data files
                            AttributeList dataFiles = FilesList(GetDataFiles(networkFolder, networkName));

                            // last data file used by this algorithm
                            networkDictionary.Add(NetworkKeys.LastDataFile, new Attribute() { Value = dataFiles[0], Editable = false });

                            // data files path
                            networkDictionary.Add(NetworkKeys.DataFilePath, new Attribute() { Value = GenerateDataFilePath(networkFolder), Editable = false });

                            // data files list
                            networkDictionary.Add(NetworkKeys.DataFiles, new Attribute() { Value = dataFiles, ElementWindowPrmsMethod = ListItemSetting });


                            // debug files
                            AttributeList debugFiles = FilesList(GetDebugFiles(networkFolder, networkName));

                            // last debug file used by this algorithm
                            networkDictionary.Add(NetworkKeys.LastDebugFile, new Attribute() { Value = debugFiles[0], Editable = false });

                            // debug files path
                            networkDictionary.Add(NetworkKeys.DebugFilePath, new Attribute() { Value = GenerateDebugFilePath(networkFolder), Editable = false });

                            // debug files list
                            networkDictionary.Add(NetworkKeys.DebugFiles, new Attribute() { Value = debugFiles, ElementWindowPrmsMethod = ListItemSetting });

                            // log files
                            AttributeList logFiles = FilesList(GetLogFiles(networkFolder, networkName));

                            // last debug file used by this algorithm
                            networkDictionary.Add(NetworkKeys.LastLogFile, new Attribute() { Value = logFiles[0], Editable = false });

                            // debug files path
                            networkDictionary.Add(NetworkKeys.LogFilePath, new Attribute() { Value = GenerateLogFilePath(networkFolder), Editable = false });

                            // debug files list
                            networkDictionary.Add(NetworkKeys.LogFiles, new Attribute() { Value = logFiles, ElementWindowPrmsMethod = ListItemSetting });

                            // Add the network to the algorithm dictionary                    
                            networks.Add(networkName, new Attribute { Value = networkDictionary, ElementWindowPrmsMethod = NetworkSetting });
                        }

                    }
                }

                // If the algorithms attribute exists - remove it
                if (pa.ContainsKey(Keys.Subjects))
                {
                    pa.Remove(Keys.Subjects);
                }

                // Add the Algorithms attribute
                pa.Add(Keys.Subjects, new Attribute() { Value = subjects, Editable = false, ElementWindowPrmsMethod = ListItemSetting });
                pa[Keys.AlgorithmsDataPath] = algorithmsDataPath;
                return true;
            }
            catch (Exception e)
            {
                CustomizedMessageBox.Show(new List<Control> {
                    CustomizedMessageBox.SetLabel("Failed to load algorithms probably the directory structure is wrong"),
                    CustomizedMessageBox.SetLabel("The error is :", new Font{fontWeight = FontWeights.Bold }),
                    CustomizedMessageBox.SetLabel(e.Message) },
                    "Config Message", new List<string> { "OK" }, Icons.Error);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void NewDataFolder()
        ///
        /// \brief Creates a new data folder.
        ///
        /// \par Description.
        ///      Create a totally new and empty data file folder
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void NewDataFolder(bool save = true)
        {
            // Select algorithms path
            // The selection is done for the parent directory in order that the name of the directory
            // will be fixed ("Algorithm Data").
            string path = pa[Keys.AlgorithmsDataPath];
            path = Path.GetFullPath(path);
            path = Directory.GetParent(path).FullName;
            path = Directory.GetParent(path).FullName;

            if (FileUtilities.SelectFolder(false, "Select the Algorithms Data PARENT folder", ref path) == false)
                return;

            // If the folder exists - produce a warning
            path += "\\Algorithms Data\\";
            if (Directory.Exists(path))
            {
                if (CustomizedMessageBox.Show(new List<string> { "The directory exist", "Do you want to remove it ?" },
                    "ConfigWindow Message", MessageBoxButton.YesNo, Icons.Question) == MessageBoxResult.No)
                    return;
            }

            Logger.CloseAllLogFiles();

            // Create the algorithm data directory
            if (Directory.Exists(Path.GetFullPath(path)))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception e)
                {
                    CustomizedMessageBox.FileMsgErr("Error while deleting directory",
                        "",
                        path,
                        "",
                        e.Message);
                    return;
                }
            }
            Directory.CreateDirectory(path);

            // Create directory for all the algorithms that are not found in the directory
            List<string> algorithms = TypesUtility.GetAlgorithms();

            foreach (string algorithm in algorithms)
            {
                FileUtilities.CreateDefaultDirsForNetwork(algorithm, path);
            }

            // Get the data from the directory to the config
            pa[Keys.AlgorithmsDataPath] = path;
            AddAlgorithmsData(false);

            // Set the selected network files
            CheckAndSetSelected();

            // Save the config
            if (save)
            {
                SaveConfig();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SaveConfig()
        ///
        /// \brief Saves the configuration.
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

        public void SaveConfig()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(Save(xmlDoc, xmlDoc.CreateElement("Configuration")));
            xmlDoc.Save(configFilePath + configFileName);
            CustomizedMessageBox.FileMsg("The config file was saved to :", configFileName, configFilePath ,"", "Config Message", null, Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateFromExisting(string loadFileName, string loadFilePath, bool getAlgorithmsPath = true, bool getConfigFileName = true)
        ///
        /// \brief Creates from existing.
        ///
        /// \par Description.
        ///      if the Algorithm Data was changed this method updates the config with the changes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param loadFileName       (string) - Filename of the load file.
        /// \param loadFilePath       (string) - Full pathname of the load file.
        /// \param getAlgorithmsPath (Optional)  (bool) - true to get algorithms path.
        /// \param getConfigFileName (Optional)  (bool) - true to get configuration file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateFromExisting(string loadFileName, string loadFilePath, bool getAlgorithmsPath = true, bool getConfigFileName = true)
        {

            // Select the target config file
            if (getConfigFileName)
            {
                if (!FileUtilities.SelectOutputFile(".config", ref configFileName, ref configFilePath))
                {
                    return;
                }
            }

            // Add Algorithms data
            AddAlgorithmsData(getAlgorithmsPath);

            // Save the file
            SaveConfig();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void UpdateSelectedDataFileChanged(string subject, string algorithm, string network, string dataFile, string dataPath)
        ///
        /// \brief Updates the selected data file changed.
        ///
        /// \par Description.
        ///      Update the Config after DataFile selection was changed in MainWindow
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        /// \param network    (string) - The network.
        /// \param dataFile   (string) - The data file.
        /// \param dataPath   (string) - Full pathname of the data file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateSelectedDataFileChanged(string subject,
            string algorithm,
            string network,
            string dataFile,
            string dataPath)
        {
            UpdateSelectionChanged(subject, algorithm, network);
            pa[Keys.SelectedDataFileName] = dataFile;
            pa[Keys.SelectedDataPath] = dataPath;
            pa[Keys.SelectedDebugFileName] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LastDebugFile];
            pa[Keys.SelectedDebugPath] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.DebugFilePath];
            SetSelectedFile(subject,
                algorithm,
                network,
                dataFile,
                dataPath,
            NetworkKeys.DataFiles,
            NetworkKeys.DataFilePath,
            NetworkKeys.LastDataFile);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void UpdateSelectedDebugFileChanged(string subject, string algorithm, string network, string debugFile, string debugPath)
        ///
        /// \brief Updates the selected debug file changed.
        ///
        /// \par Description.
        ///      Update Config after debug file was selected in the MainWindow
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        /// \param network    (string) - The network.
        /// \param debugFile  (string) - The debug file.
        /// \param debugPath  (string) - Full pathname of the debug file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateSelectedDebugFileChanged(string subject,
            string algorithm,
            string network,
            string debugFile,
            string debugPath)
        {
            UpdateSelectionChanged(subject, algorithm, network);
            pa[Keys.SelectedDebugFileName] = debugFile;
            pa[Keys.SelectedDebugPath] = debugPath;
            pa[Keys.SelectedDataFileName] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LastDataFile];
            pa[Keys.SelectedDataPath] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.DataFilePath];
            SetSelectedFile(subject,
                algorithm,
                network,
                debugFile,
                debugPath,
            NetworkKeys.DebugFiles,
            NetworkKeys.DebugFilePath,
            NetworkKeys.LastDebugFile);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetSelectedFile(string subject, string algorithm, string network, string file, string path, NetworkKeys filesKey, NetworkKeys pathKey, NetworkKeys lastFileKey)
        ///
        /// \brief Sets selected file.
        ///
        /// \par Description.
        ///      Update all the fields of the selected file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject      (string) - The subject.
        /// \param algorithm    (string) - The algorithm.
        /// \param network      (string) - The network.
        /// \param file         (string) - The file.
        /// \param path         (string) - Full pathname of the file.
        /// \param filesKey     (NetworkKeys) - The files key.
        /// \param pathKey      (NetworkKeys) - The path key.
        /// \param lastFileKey  (NetworkKeys) - The last file key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetSelectedFile(string subject,
            string algorithm,
            string network,
            string file,
            string path,
            NetworkKeys filesKey,
            NetworkKeys pathKey,
            NetworkKeys lastFileKey)
        {
            // If the selected file is new add it to the list of data files
            AttributeList files = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][filesKey];
            Attribute fileAttribute = files.FirstOrDefault(a => a.Value == file);
            if (!(fileAttribute is null))
            {
                files.Remove(fileAttribute);
            }
            files.Insert(0, new Attribute { Value = file, Editable = false });

            // Update the last file to be the one selected
            pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][lastFileKey] = file;
            pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][pathKey] = path;

            // Update the selected file

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateSelectionChanged(string subject, string algorithm, string network)
        ///
        /// \brief Updates the selection changed.
        ///
        /// \par Description.
        ///      Update all the selected field after a file was selected in the MainWindow
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        /// \param network    (string) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateSelectionChanged(string subject,
            string algorithm,
            string network)
        {
            // Create a directories if needed
            FileUtilities.CreateDirsForNeteork(subject, algorithm, network, pa[Keys.AlgorithmsDataPath]);
            AddAlgorithmsData(false);

            // Update the selected algorithm fields
            pa[Keys.SelectedAlgorithm] = algorithm;
            pa[Keys.SelectedSubject] = subject;
            pa[Keys.SelectedNetwork] = network;

            // Log file
            pa[Keys.SelectedLogFileName] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LastLogFile];
            pa[Keys.SelectedLogPath] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LogFilePath];
        }

        #endregion
        #region /// \name Utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private AttributeList FilesList(AttributeList filesList)
        ///
        /// \brief Files list.
        ///        
        /// \par Description.
        ///      Given a files list set the attribute's members to fit a file list
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param filesList  (AttributeList) - List of files.
        ///
        /// \return An AttributeList.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private AttributeList FilesList(AttributeList filesList)
        {
            foreach (Attribute attribute in filesList)
            {
                attribute.Editable = false;
            }
            return filesList;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn AttributeList GetDataFiles(string networkFolder, string networkName)
        ///
        /// \brief Gets data files.
        ///
        /// \par Description.
        ///      This utility method is used to collect all the data files of an algorithm
        ///      and generate a list that can be entered to the config.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \param networkFolder  (string) - Pathname of the network folder.
        /// \param networkName    (string) - Name of the network.
        ///
        /// \return The data files.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        AttributeList GetDataFiles(string networkFolder, string networkName)
        {
            AttributeList dataFiles = FileUtilities.GetAllFileNames(GenerateDataFilePath(networkFolder), "data");
            if (dataFiles.Count == 0)
            {
                dataFiles.Add(new Attribute { Value = GenerateDefaultDataFileName(networkName) });
            }
            return dataFiles;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn AttributeList GetDebugFiles(string networkFolder, string networkName)
        ///
        /// \brief Gets debug files.
        ///
        /// \par Description.
        ///        This utility method is used to collect all the debug files of an algorithm
        ///        and generate a list that can be entered to the config.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param networkFolder  (string) - Pathname of the network folder.
        /// \param networkName    (string) - Name of the network.
        ///
        /// \return The debug files.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        AttributeList GetDebugFiles(string networkFolder, string networkName)
        {
            AttributeList debugFiles = FileUtilities.GetAllFileNames(GenerateDebugFilePath(networkFolder), "debug");
            if (debugFiles.Count == 0)
            {
                debugFiles.Add(new Attribute { Value = GenerateDefaultDebugFileName(networkName) });
            }
            return debugFiles;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn AttributeList GetLogFiles(string networkFolder, string networkName)
        ///
        /// \brief Gets log files.
        ///
        /// \par Description.
        ///      Get a list of the log files in a network
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param networkFolder  (string) - Pathname of the network folder.
        /// \param networkName    (string) - Name of the network.
        ///
        /// \return The log files.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        AttributeList GetLogFiles(string networkFolder, string networkName)
        {
            AttributeList logFiles = FileUtilities.GetAllFileNames(GenerateLogFilePath(networkFolder), "log");

            // In each running several logs might be generated.
            // The log file names are generated in the following way:
            // <main log name>.<private log name>.log
            // The following code generates a list of the main log name + ".log"
            // Step 1 generate a list of all the main log name (might contain duplications
            logFiles = logFiles.Select(a => ((string)a.Value).Remove(((string)a.Value).IndexOf(".")))

                // Step 2 perform distinct add the ".log" and convert to AttributeList
                .Distinct().Select(s => s + ".log").ToList().ToAttributeList();

            if (logFiles.Count == 0)
            {
                logFiles.Add(new Attribute { Value = GenerateLogFileName(networkName) });
            }
            return logFiles;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<string> GetAlgorithmsNames()
        ///
        /// \brief Gets algorithms names.
        ///
        /// \par Description.
        ///      This method returns a list of subject.algorith for all the algorithms in the config file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \return The algorithms names.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<string> GetAlgorithmsNames()
        {
            List<string> algorithmsNames = new List<string>();

            // List of attributes each one represents a subject
            AttributeDictionary subjects = pa[Keys.Subjects];

            foreach (var subject in subjects)
            {
                // List of algorithms for the subject
                //AttributeDictionary algorithms = subject.Value[SubjectDataKeys.Algorithms];
                string subjectName = subject.Key;
                AttributeDictionary algorithms = subject.Value.Value;

                foreach (string algorithmName in algorithms.Keys)
                {
                    // Each algorithm attribute holds a dictionary with the attributes
                    algorithmsNames.Add(subjectName + "." + algorithmName);
                }
            }
            return algorithmsNames;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<string> GetAlgorithmsOfSubject(string subject)
        ///
        /// \brief Gets algorithms of subject.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 24/12/2017
        ///
        /// \param subject  (string) - The subject.
        ///
        /// \return The algorithms of subject.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<string> GetAlgorithmsOfSubject(string subject)
        {
            try
            {
                AttributeDictionary algorithms = pa[Keys.Subjects][subject];
                return algorithms.Keys.Select(k => (string)k).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<string> GetSubjects()
        ///
        /// \brief Gets the subjects.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 24/12/2017
        ///
        /// \return The subjects.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<string> GetSubjects()
        {
            List<string> result = new List<string>();
            foreach (var key in pa[Keys.Subjects].Keys)
            {
                result.Add((string)key);
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CheckAndSetSelected()
        ///
        /// \brief Sets selected algorithm file names.
        ///
        /// \par Description.
        ///      -  This method is activated after the Algorithm Data directory was scanned  
        ///      -  It sets all the selected parameters after checking that they exist in the Algorithm Data dictionary  
        ///      -  For the subject, algorithm, network : if the selected is not found in the lists scanned from the   
        ///         Algorithm Data directory - take the value from the default 
        ///      -  For files and paths : if they do not exist in the Algorithm Data directory - take the values from  
        ///         the Algorithm Data directory scanning result  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/11/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CheckAndSetSelected()
        {

            string subject = pa[Keys.SelectedSubject];
            if (TypesUtility.IndexInKeys(pa[Keys.Subjects].Keys, subject) == -1)
            {
                subject = ea[Keys.SelectedSubject];
                pa[Keys.SelectedSubject] = subject;
            }

            string algorithm = pa[Keys.SelectedAlgorithm];
            if (TypesUtility.IndexInKeys(pa[Keys.Subjects][subject].Keys, algorithm) == -1)
            {
                algorithm = ea[Keys.SelectedAlgorithm];
                pa[Keys.SelectedAlgorithm] = algorithm;
            }

            string network = pa[Keys.SelectedNetwork];
            if (TypesUtility.IndexInKeys(pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks].Keys, network) == -1)
            {
                network = ea[Keys.SelectedNetwork];
                pa[Keys.SelectedNetwork] = network;
            }

            SetSelectedFile(Keys.SelectedDataPath,
                NetworkKeys.DataFilePath,
                Keys.SelectedDataFileName,
                NetworkKeys.DataFiles,
                NetworkKeys.LastDataFile);

            SetSelectedFile(Keys.SelectedDebugPath,
                NetworkKeys.DebugFilePath,
                Keys.SelectedDebugFileName,
                NetworkKeys.DebugFiles,
                NetworkKeys.LastDebugFile);

            SetSelectedFile(Keys.SelectedLogPath,
                NetworkKeys.LogFilePath,
                Keys.SelectedLogFileName,
                NetworkKeys.LogFiles,
                NetworkKeys.LastLogFile);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetSelectedFile(dynamic selectedPathKey, dynamic pathKey, dynamic selectedFileKey, dynamic filesKey, dynamic lastFileKey)
        ///
        /// \brief Sets selected file.
        ///
        /// \par Description.
        ///      Set the selected file and path
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param selectedPathKey  (dynamic) - The selected path key.
        /// \param pathKey          (dynamic) - The path key.
        /// \param selectedFileKey  (dynamic) - The selected file key.
        /// \param filesKey         (dynamic) - The files key.
        /// \param lastFileKey      (dynamic) - The last file key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetSelectedFile(dynamic selectedPathKey,
            dynamic pathKey,
            dynamic selectedFileKey,
            dynamic filesKey,
            dynamic lastFileKey)
        {
            string subject = pa[Keys.SelectedSubject];
            string algorithm = pa[Keys.SelectedAlgorithm];
            string network = pa[Keys.SelectedNetwork];

            // The path of the selected path must be the same as the path in the network's dictionary 
            string dataPath = pa[selectedPathKey];
            if (dataPath != pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][pathKey])
            {
                pa[selectedPathKey] = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][pathKey];
            }


            // The data file has to be in the data files list 
            // The selected data file has to be the same as the last data file in the network's dictionary
            // If the data file is not found in the data files list - Set the SelectedDataFile to the Last data file
            // of the algorithm
            // if the data file is found in the data file list - it is the LastDataFile of the algorithm to the Selected data file
            string dataFile = pa[selectedFileKey];
            AttributeList dataFiles = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][filesKey];
            if (dataFiles.FirstOrDefault(a => (string)a.Value == dataFile) == null)
            {
                dataFile = pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][lastFileKey];
                pa[selectedFileKey] = dataFile;
            }
            else
            {
                pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][lastFileKey] = dataFile;
            }
        }



        #endregion
        #region /// \name Files names and paths

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateNetworkPath(string subject = null, string algorithm = null, string network = null)
        ///
        /// \brief Generates a default network path.
        ///
        /// \par Description.
        ///        Generate the network path of the current selected network or network given in the parameters.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        /// \param network   (Optional)  (string) - The network.
        ///
        /// \return The default network path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateNetworkPath(string subject = null, string algorithm = null, string network = null)
        {
            if (network is null)
            {
                network = pa[Keys.SelectedNetwork];
            }

            if (algorithm is null)
            {
                algorithm = pa[Keys.SelectedAlgorithm];
            }

            if (subject is null)
            {
                subject = pa[Keys.SelectedSubject];
            }
            return pa[Keys.AlgorithmsDataPath] + subject + "\\" + algorithm + "\\Networks\\" + network;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateAlgorithmPath(string subject = null, string algorithm = null)
        ///
        /// \brief Gets default algorithm path.
        ///
        /// \par Description.
        ///      This methods generates the path of the selected algorithm or algorithm given in the parameters.
        ///      
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The default algorithm path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateAlgorithmPath(string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = pa[Keys.SelectedAlgorithm];
            }

            if (subject is null)
            {
                subject = pa[Keys.SelectedSubject];
            }
            return pa[Keys.AlgorithmsDataPath] + subject + "\\" + algorithm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSubjectPath(string subject = null)
        ///
        /// \brief Generates a subject path.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/12/2017
        ///
        /// \param subject (Optional)  (string) - The subject.
        ///
        /// \return The subject path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSubjectPath(string subject = null)
        {
            if (subject is null)
            {
                subject = pa[Keys.SelectedSubject];
            }
            return pa[Keys.AlgorithmsDataPath] + subject;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateDocsPath(string subject = null, string algorithm = null)
        ///
        /// \brief Generates the documents path.
        ///
        /// \par Description.
        ///      This method generates the path to the documents of an algorithm
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The documents path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateDocsPath(string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = pa[Keys.SelectedAlgorithm];
            }

            if (subject is null)
            {
                subject = pa[Keys.SelectedSubject];
            }
            return pa[Keys.AlgorithmsDataPath] + subject + "\\" + algorithm + "\\Documents";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSelectedDataFileName()
        ///
        /// \brief Generates default data file name.
        ///
        /// \par Description.
        ///      Generate the default data file name for the algorithm currently running.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \return The default data file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSelectedDataFileName()
        {
            return GenerateDefaultDataFileName(pa[Keys.SelectedNetwork]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateDefaultDataFileName(string networkName)
        ///
        /// \brief Generates default data file name.
        ///
        /// \par Description.
        ///      Generate the default data file name for a given algorithm.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \param networkName (string) - Name of the network.
        ///
        /// \return The default data file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateDefaultDataFileName(string networkName)
        {
            return networkName + ".data";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSelectedDataFilePath()
        ///
        /// \brief Generates default data file path.
        ///
        /// \par Description.
        ///      Generate the default algorithm data path of the selected algorithm.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \return The default data file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSelectedDataFilePath()
        {
            return GenerateDataFilePath(GenerateNetworkPath());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateDataFilePath(string subject, string algorithm, string network)
        ///
        /// \brief Generates a data file path.
        ///
        /// \par Description.
        ///      Generate data file path for subject, algorithm, network.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/11/2017
        ///
        /// \param subject   (string) - The subject.
        /// \param algorithm (string) - The algorithm.
        /// \param network   (string) - The network.
        ///
        /// \return The data file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateDataFilePath(string subject, string algorithm, string network)
        {
            return GenerateDataFilePath(GenerateNetworkPath(subject, algorithm, network));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GenerateDataFilePath(string networkFolder)
        ///
        /// \brief Generates default data file path.
        ///
        /// \par Description.
        ///      Generate the default algorithm data path given the algorithm path.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \param networkFolder (string) - Pathname of the network folder.
        ///
        /// \return The default data file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GenerateDataFilePath(string networkFolder)
        {
            return networkFolder + "\\Data\\";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSelectedDebugFileName()
        ///
        /// \brief Generates selected debug file name.
        ///
        /// \par Description.
        ///      Generate the default debug file name for the selected network.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \return The default debug file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSelectedDebugFileName()
        {
            return GenerateDefaultDebugFileName(pa[Keys.SelectedNetwork]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateDefaultDebugFileName(string networkName)
        ///
        /// \brief Generates default debug file name.
        ///
        /// \par Description.
        ///      Generate the default debug file name for a given network.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param networkName (string) - Name of the network.
        ///
        /// \return The default debug file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateDefaultDebugFileName(string networkName)
        {
            return networkName + ".debug";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSelectedDebugFilePath()
        ///
        /// \brief Generates selected debug file path.
        ///
        /// \par Description.
        ///      Generate the selected debug file path for the network currently running.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \return The default debug file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSelectedDebugFilePath()
        {
            return GenerateDebugFilePath(GenerateNetworkPath());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateDebugFilePath(string subject, string algorithm, string network)
        ///
        /// \brief Generates a debug file path.
        ///
        /// \par Description.
        ///      Generate a debug file path for a given subject, algorithm, network
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        /// \param network    (string) - The network.
        ///
        /// \return The debug file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateDebugFilePath(string subject, string algorithm, string network)
        {
            return GenerateDebugFilePath(GenerateNetworkPath(subject, algorithm, network));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GenerateDebugFilePath(string networkFolder)
        ///
        /// \brief Generates debug file path.
        ///
        /// \par Description.
        ///      Generate the debug file path for a giving algorithm path.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param networkFolder (string) - Pathname of the network folder.
        ///
        /// \return The default debug file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GenerateDebugFilePath(string networkFolder)
        {
            return networkFolder + "\\Debug\\";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSelectedtLogFileName()
        ///
        /// \brief Generates default log file name.
        ///
        /// \par Description.
        ///      Generate the default log file name.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \return The default log file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSelectedLogFileName()
        {
            return GenerateLogFileName(pa[Keys.SelectedNetwork]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateLogFileName(string networkName)
        ///
        /// \brief Generates a log file name.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param networkName  (string) - Name of the network.
        ///
        /// \return The log file name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GenerateLogFileName(string networkName)
        {
            return networkName + ".log";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSelectedLogFilePath()
        ///
        /// \brief Generates selected log file path.
        ///
        /// \par Description.
        ///      Generate the default log file path given the selected network.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \return The default log file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSelectedLogFilePath()
        {
            return GenerateLogFilePath(GenerateNetworkPath());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateLogFilePath(string subject, string algorithm, string network)
        ///
        /// \brief Generates a log file path.
        ///
        /// \par Description.
        ///      Generate the log file path for a given subject, algorithm, network
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        /// \param network    (string) - The network.
        ///
        /// \return The log file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateLogFilePath(string subject, string algorithm, string network)
        {
            return GenerateLogFilePath(GenerateNetworkPath(subject, algorithm, network));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateLogFilePath(string algorithmFolder)
        ///
        /// \brief Generates a log file path.
        ///
        /// \par Description.
        ///      Generate the log file path given the network path
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param algorithmFolder  (string) - Pathname of the algorithm folder.
        ///
        /// \return The log file path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateLogFilePath(string networkFolder)
        {
            return networkFolder + "\\Logs\\";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateSourceDocPath(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a source document path.
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
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The source document path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateSourceDocPath(string subject = null, string algorithm = null)
        {
            return GenerateSourceDocPath(GenerateDocsPath(subject, algorithm));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GenerateSourceDocPath(string docsFolder)
        ///
        /// \brief Generates source document path.
        ///
        /// \par Description.
        ///      Generate the source documentation path given the algorithm path.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \param docsFolder (string) - Pathname of the algorithm folder.
        ///
        /// \return The source document path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GenerateSourceDocPath(string docsFolder)
        {
            return docsFolder + "\\Source\\";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateProcessedDocPath(string subject = null, string algorithm = null)
        ///
        /// \brief Generates the processed document path.
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
        /// \param subject   (Optional) (string) - Name of the algorithm.
        /// \param algorithm (Optional) (string) - Name of the subject.
        ///
        /// \return The source document path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateProcessedDocPath(string subject = null, string algorithm = null)
        {
            return GenerateProcessedDocPath(GenerateDocsPath(subject, algorithm));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GenerateProcessedDocPath(string docsFolder)
        ///
        /// \brief Generates processed document path.
        ///
        /// \par Description.
        ///      Generate the processed documentation default path given the documents path.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \param docsFolder  (string) - Pathname of the documents folder.
        ///
        /// \return The processed document path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenerateProcessedDocPath(string docsFolder)
        {
            return docsFolder + "\\Processed\\";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GeneratePeudoCodePath(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a pseudo code document path.
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
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The source document path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GeneratePeudoCodePath(string subject = null, string algorithm = null)
        {
            return GeneratePseudoCodePath(GenerateDocsPath(subject, algorithm));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GeneratePseudoCodePath(string docsFolder)
        ///
        /// \brief Generates pseudo code path.
        ///
        /// \par Description.
        ///      Generate the pseudo-code documentation default path given the documents path.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 12/11/2017
        ///
        /// \param docsFolder  (string) - Pathname of the documents folder.
        ///
        /// \return The pseudo code path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GeneratePseudoCodePath(string docsFolder)
        {
            return docsFolder + "\\PseudoCode\\";
        }


        #endregion
        #region /// \name ConfigWindow support - EndInputOperation methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool SubjectChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Check subject changed.
        ///        A method activated after a subject is changed in the config window dialog.
        ///
        /// \par Description.
        ///      Changing the following controls:
        ///      -# The selected algorithm ComboBox
        ///      -# The selected network ComboBox
        ///      -# All the TextBoxes of the selected files and paths
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/11/2017
        ///
        /// \param       network          (BaseNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        /// \param       inputWindow     (Optional)  (ElementWindow) - The input window.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool SubjectChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";

            // The method has 2 modes :
            // When the inputWindow is not null the combo box of the algorithms is filled
            // When the inputWindow is null (Select not in input window) only check if the algorithm is
            // in the subject (if not - return false and the correction method should correct it)
            List<string> algorithms = TypesUtility.GetAlgorithmsOfSubject(newValue);
            string algorithm = networkElement.pa[Keys.SelectedAlgorithm];

            if (inputWindow == null)
            {
                if (!algorithms.Contains(algorithm))
                {
                    SelectDialog selectDialog = new SelectDialog("Select Algorithm", "Select Algorithm Dialog", algorithms);
                    selectDialog.ShowDialog();
                    if (selectDialog.Result == SelectDialog.SelectDialogResult.Select)
                    {
                        attribute.Value = algorithms[selectDialog.Selection[0]];
                        return true;
                    }
                    else
                    {
                        errorMessage = "The algorithm not found in the subject";
                        return false;
                    }
                }
                return true;
            }
            else
            {
                // If the call is from the Config window change the controls
                string selectedAlgorithm = UpdateSelectedAlgorithmControl(newValue, (ConfigWindow)inputWindow);
                string selectedNetwork = UpdateSelectedNetworkControl(newValue, selectedAlgorithm, (ConfigWindow)inputWindow);
                UpdateSelectedDataFile(newValue, selectedAlgorithm, selectedNetwork, (ConfigWindow)inputWindow);
                return true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool AlgorithmChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Algorithm changed.
        ///        This method is activated after the algorithms ComboBox selection changed
        ///
        /// \par Description.
        ///      Changing the following controls:
        ///      -# The selected network ComboBox
        ///      -# All the TextBoxes of the selected files and paths
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param       network          (BaseNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        /// \param       inputWindow     (Optional)  (ElementWindow) - The input window.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool AlgorithmChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";

            // The method has 2 modes :
            // When the inputWindow is not null the combo box of the algorithms is filled
            // When the inputWindow is null (Select not in input window) only check if the algorithm is
            // in the subject (if not - return false and the correction method should correct it)

            string algorithm = networkElement.pa[Keys.SelectedAlgorithm];

            if (inputWindow == null)
            {
                return true;
            }
            else
            {
                // If the call is from the Config window change the controls
                Attribute subjectAttribute = networkElement.pa.GetAttribute(Keys.SelectedSubject);
                string subject = inputWindow.GetNewValue(subjectAttribute);
                string selectedNetwork = UpdateSelectedNetworkControl(subject, newValue, (ConfigWindow)inputWindow);
                UpdateSelectedDataFile(subject, newValue, selectedNetwork, (ConfigWindow)inputWindow);
                return true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool NetworkChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Network changed.
        ///        This method is activated after changing the network in the selected network combo box
        ///
        /// \par Description.
        ///      Changing the following controls:
        ///      -# The selected algorithm ComboBox
        ///      -# The selected network ComboBox
        ///      -# All the TextBoxes of the selected files and paths 
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param       network          (BaseNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        /// \param       inputWindow     (Optional)  (ElementWindow) - The input window.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool NetworkChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";

            // The method has 2 modes :
            // When the inputWindow is not null the combo box of the algorithms is filled
            // When the inputWindow is null (Select not in input window) only check if the algorithm is
            // in the subject (if not - return false and the correction method should correct it)

            if (inputWindow == null)
            {
                return true;
            }
            else
            {
                Attribute subjectAttribute = networkElement.pa.GetAttribute(Keys.SelectedSubject);
                string subject = inputWindow.GetNewValue(subjectAttribute);
                Attribute algorithmAttribute = networkElement.pa.GetAttribute(Keys.SelectedAlgorithm);
                string algorithm = inputWindow.GetNewValue(subjectAttribute);
                UpdateSelectedDataFile(subject, algorithm, newValue, (ConfigWindow)inputWindow);
                return true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static string UpdateSelectedAlgorithmControl(string subject, ConfigWindow inputWindow)
        ///
        /// \brief Updates the selected algorithm control.
        ///
        /// \par Description.
        ///      -  This method changes the selected algorithm ComboBox in the ConfigWindow  
        ///      -  The method collects the algorithms of the new subject and then recreate the ComboBox
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject      (string) - The subject.
        /// \param inputWindow  (ConfigWindow) - The input window.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string UpdateSelectedAlgorithmControl(string subject, ConfigWindow inputWindow)
        {
            List<string> algorithms = TypesUtility.GetAlgorithmsOfSubject(subject);
            Config config = (Config)inputWindow.networkElements[0];
            Attribute algorithmAttribute = config.pa.GetAttribute(Keys.SelectedAlgorithm);
            NewValueControlPrms controlPrms = new NewValueControlPrms();
            controlPrms.inputFieldType = InputFieldsType.ComboBox;
            controlPrms.options = algorithms.ToArray();
            controlPrms.Value = controlPrms.options[0];
            inputWindow.ReplaceNewValueControl(controlPrms, algorithmAttribute);
            return controlPrms.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static string UpdateSelectedNetworkControl(string subject, string algorithm, ConfigWindow inputWindow)
        ///
        /// \brief Updates the selected network control.
        ///
        /// \par Description.
        ///      -  This method changes the selected network ComboBox in the ConfigWindow  
        ///      -  The method collects the networks of the new subject and algorithm and then recreate the ComboBox
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject      (string) - The subject.
        /// \param algorithm    (string) - The algorithm.
        /// \param inputWindow  (ConfigWindow) - The input window.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string UpdateSelectedNetworkControl(string subject, string algorithm, ConfigWindow inputWindow)
        {
            Config config = (Config)inputWindow.networkElements[0];
            ICollection<dynamic> networks = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks].Keys;

            Attribute networkAttribute = config.pa.GetAttribute(Keys.SelectedNetwork);
            NewValueControlPrms controlPrms = new NewValueControlPrms();
            controlPrms.inputFieldType = InputFieldsType.ComboBox;
            controlPrms.options = networks.Cast<string>().ToArray();
            controlPrms.Value = controlPrms.options[0];
            inputWindow.ReplaceNewValueControl(controlPrms, networkAttribute);
            return controlPrms.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void UpdateSelectedDataFile(string subject, string algorithm, string network, ConfigWindow configWindow)
        ///
        /// \brief Updates the selected data file.
        ///
        /// \par Description.
        ///      -  This method updates the selected files and paths when a network was changed  
        ///      -  The selected subject, algorithm, network, are retrieved from the ComboBoxes  
        ///      -  The Last data, debug, log files of the network are copied to the selected files  
        ///      -  The paths of data, debug, log files of the network are copied to the selected paths
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param subject       (string) - The subject.
        /// \param algorithm     (string) - The algorithm.
        /// \param network       (string) - The network.
        /// \param configWindow  (ConfigWindow) - The configuration window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void UpdateSelectedDataFile(string subject, string algorithm, string network, ConfigWindow configWindow)
        {
            Config config = (Config)configWindow.networkElements[0];

            Attribute attribute = config.pa.GetAttribute(Keys.SelectedDataFileName);
            string value = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LastDataFile];
            configWindow.ChangeValue(value, attribute);

            attribute = config.pa.GetAttribute(Keys.SelectedDataPath);
            value = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.DataFilePath];
            configWindow.ChangeValue(value, attribute);

            attribute = config.pa.GetAttribute(Keys.SelectedDebugFileName);
            value = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LastDebugFile];
            configWindow.ChangeValue(value, attribute);

            attribute = config.pa.GetAttribute(Keys.SelectedDebugPath);
            value = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.DebugFilePath];
            configWindow.ChangeValue(value, attribute);

            attribute = config.pa.GetAttribute(Keys.SelectedLogFileName);
            value = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LastLogFile];
            configWindow.ChangeValue(value, attribute);

            attribute = config.pa.GetAttribute(Keys.SelectedLogPath);
            value = config[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks][network][NetworkKeys.LogFilePath];
            configWindow.ChangeValue(value, attribute);
        }


        #endregion
        #region /// \name Methods for setting attributes presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (static Dictionary&lt;dynamic,RoutedEventHandler&gt;) - The buttons click events.
        ///        The Following is the mechanism of connecting buttons event handler to the buttons
        ///        -   The event handlers are found in the ConfigWindow (because they are activated by the dialog
        ///        -   The attributes that use buttons have a ElementWindowPrmsMethod ButtonControlSetting
        ///        -   So when the button is defined (means the dialog tree is generated the ButtonControlSetting is called
        ///        -   The ButtonControlSetting connects the button event handler to the button according to the buttonsClickEvents dictionary.  
        ///        -   So when the button is pressed the event handler from the ConfigWindow is activated
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Dictionary<dynamic, RoutedEventHandler> buttonsClickEvents =
            new Dictionary<dynamic, RoutedEventHandler> {
                { Keys.SelectedDataFileName, ConfigWindow.Button_SelectSelectedAlgorithmDataFile_Click },
                { Keys.SelectedDataPath, ConfigWindow.Button_SelectSelectedAlgorithmDataFile_Click },
                { Keys.SelectedDebugFileName, ConfigWindow.Button_SelectSelectedAlgorithmDebugFile_Click },
                { Keys.SelectedDebugPath, ConfigWindow.Button_SelectSelectedAlgorithmDebugFile_Click },
                { Keys.AlgorithmsDataPath, ConfigWindow.Button_SelectAlgorithmsPath_Click } };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms ButtonControlSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Button control setting.
        ///
        /// \par Description.
        ///      Creates the parameters for a button presentation in the ConfigWindow of a button.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms ButtonControlSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.Button;
            prms.newValueControlPrms.Value = attribute.Value;
            if (buttonsClickEvents.Keys.Any(k => TypesUtility.CompareDynamics(k, key)))
            {
                prms.newValueControlPrms.button_click = buttonsClickEvents[key];
            }
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms SubjectComboBoxSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Subject combo box setting.
        ///
        /// \par Description.
        ///      Set the presentation parameters for the combo box to select a subject.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms SubjectComboBoxSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            prms.newValueControlPrms.options = TypesUtility.GetAlgorithmsSubjects().ToArray();
            string subject = mainNetworkElement.pa[Keys.SelectedSubject];
            prms.newValueControlPrms.Value = attribute.Value;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms AlgorithmComboBoxSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Algorithm combo box setting.
        ///
        /// \par Description.
        ///      Set the presentation parameters for the algorithms ComboBox.
        ///
        /// \par Algorithm.
        ///      -  Take the selected subject  
        ///      -  Insert all the algorithms of the selected subject to the ComboBox  
        ///      -  Set the selection to the selected algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms AlgorithmComboBoxSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            string subject = mainNetworkElement.pa[Keys.SelectedSubject];
            prms.newValueControlPrms.options = TypesUtility.GetAlgorithmsOfSubject(subject).ToArray();
            prms.newValueControlPrms.Value = attribute.Value;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms NetworkComboBoxSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Network combo box setting.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///      -  Take the selected subject and algorithm  
        ///      -  Insert all the network of the selected subject to the ComboBox  
        ///      -  Set the selection to the selected network.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms NetworkComboBoxSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            string algorithm = mainNetworkElement.pa[Keys.SelectedAlgorithm];
            string subject = mainNetworkElement.pa[Keys.SelectedSubject];
            ICollection<dynamic> networks = mainNetworkElement.pa[Keys.Subjects][subject][algorithm][AlgorithmKeys.Networks].Keys;
            prms.newValueControlPrms.options = networks.Cast<string>().ToArray();
            prms.newValueControlPrms.Value = attribute.Value;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms ListItemSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief List item setting.
        ///
        /// \par Description.
        ///      All the list and the dictionaries that are disabled gets this method.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms ListItemSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            prms.newValueControlPrms.Value = TypesUtility.GetKeyToString(key) + " list";
            prms.newValueControlPrms.enable = false;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms SubjectSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Subject setting.
        ///
        /// \par Description.
        ///      Setting the presentation parameters for a subject in the subject list
        ///      (disabled TextBlock)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms SubjectSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            prms.newValueControlPrms.Value = "Algorithms of subject " + TypesUtility.GetKeyToString(key);
            prms.newValueControlPrms.enable = false;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms AlgorithmSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Algorithm setting.
        ///
        /// \par Description.
        ///      Setting the presentation parameters for an algorithm in the algorithm list
        ///      (disabled TextBlock)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms AlgorithmSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            prms.newValueControlPrms.Value = "Attributes of algorithm " + TypesUtility.GetKeyToString(key);
            prms.newValueControlPrms.enable = false;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms NetworkSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Network setting.
        ///
        /// \par Description.
        ///      Setting the presentation parameters for a network in the network list
        ///      (disabled TextBlock)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms NetworkSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            prms.newValueControlPrms.Value = "Attributes of network " + TypesUtility.GetKeyToString(key);
            prms.newValueControlPrms.enable = false;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms NetworksSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Networks setting.
        ///
        /// \par Description.
        ///        Setting the presentation parameters for a networks list
        ///        (disabled TextBlock)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/11/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms NetworksSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            prms.newValueControlPrms.Value = "Networks of the algorithm ";
            prms.newValueControlPrms.enable = false;
            return prms;
        }

        #endregion
    }
}
