////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\AddAlgorithmWindow.cs
///
/// \brief Implements the AddAlgorithmWindow class.
///        This class is a window for creating/Updating the automatic code for an algorithm
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Windows.Media;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class AddAlgorithmWindow
    ///
    /// \brief Dialog for creating or updating an algorithm automatic code data
    ///
    /// \par Description.
    ///      -  This class gets the following parameters:
    ///         -#  Source algorithm subject
    ///         -#  Source algorithm name
    ///         -#  Target algorithm subject
    ///         -#  Target algorithm name
    ///      -  This class produces the following:  
    ///         -#  A code files for each one of the algorithm components:
    ///             -#  Network
    ///             -#  Process
    ///             -#  Channel
    ///             -#  Message
    ///         -#  A file for the parts of the classes that change (hopefully) only by the program
    ///             The file contains:
    ///             -#  All the enums needed
    ///             -#  A message building methods for all the messages that will be used by the algorithm
    ///                 The methods are found in partial class of the process
    ///             -#  A partial class for the algorithm components which contains: the 
    ///                 initializations of the algorithm's data structure
    ///             -#  Constants for accessing attributes by name
    ///             -#  Getters/setters to be used by the class
    ///         
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 27/08/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AddAlgorithmWindow : ElementWindow
    {
        #region /// \name Enums and conversions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ScanningType
        ///
        /// \brief Values that represent scanning types.
        ///        -    There are 3 kinds of scanning of the network elements:
        ///             -#  Scanning for building the window
        ///             -#  Scanning for creating the code file
        ///             -#  Scanning for creating the constants list  
        ///        -    A class can have only one method systems for the scanning so its has to have  
        ///             a member which says which scanning is currently processed
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected enum ScanningType { ElementWindowOperations, Build, Consts }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Class
        ///
        /// \brief Values that represent class.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected enum Class { Message, Network, Process, Channel}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;string&gt;) - List of names of the enum class.
        ///         the names of the classes that contains the enums.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected List<string> enumClassNames = new List<string> { "m", "n", "p", "c" };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;string&gt;) - List of names of the class.
        ///        The suffix of the algorithm classes names.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected List<string> classNames = new List<string> { "Process", "Network", "Process", "Channel"};
        #endregion
        #region /// \name Members
        /// \brief  (string) - Source subject.
        protected string sourceSubject;

        /// \brief  (string) - Source algorithm.
        protected string sourceAlgorithm;

        /// \brief  (string) - Target subject.
        protected string targetSubject;

        /// \brief  (string) - Target algorithm.
        protected string targetAlgorithm;

        /// \brief  (bool) - true to replace user code files.
        protected bool replaceUserCodeFiles;

        /// \brief  (bool) - true to new algorithm.
        protected bool newAlgorithm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to first code generation.
        ///        The code cane be created several times but the backup and the folders creation should be done
        ///        only at the first time the code is generated.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool firstCodeGeneration = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (int) - Zero-based index of the class.
        ///        The index of the class currently generated.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected int classIdx;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string[]) - The enums classes strings.
        ///        This string array contains the generated code for the enum classes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string[] enumsClassesStrings = new string[4];

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (HashSet&lt;string&gt;) - The consts.
        ///        The class is composed from 3 parts. This is the first part
        ///        The set to hold the all names of the enum keys which will used to construct
        ///        the constants that will be used (In running time) to access the attributes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected HashSet<string> consts;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string[]) - The getters setters strings.
        ///        The class is composed from 3 parts. This is the second part contains the
        ///        Getters/Setters used to access variables in the main dictionaries.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string[] gettersSettersStrings = new string[4];

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string[]) - The methods strings.
        ///        The class is composed from 3 parts. This is the third part contains the
        ///        methods of the class.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string[] methodsStrings = new string[4];

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (HashSet&lt;string&gt;) - The syntax highlight.
        ///        Collect class/enums names of the algorithm generated. to be highlighted when
        ///        the code is presented 
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected HashSet<string> syntaxHighlight;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Stack&lt;dynamic&gt;) - Stack of skips.
        ///        This stack is used in order to detect whether the scanning is inside a complex attribute
        ///        which does no have to be scanned. When the stack is empty code generation is allowed.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected Stack<dynamic> skipStack;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (WordDocumentHolder) - The word document holder.
        ///        Used to hold the word documents opened in ShowDocuments command.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected WordDocumentHolder wordDocumentHolder = new WordDocumentHolder();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string) - The current running subject.
        ///                   - The generation of the NetworkElements to be presented is done using the Class factory
        ///                   - In order to generate classes using the class factory the class factory's members has to be set  
        ///                   - This parameter is used to recover the members of the class factory when exiting from the dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string currentRunningSubject;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string) - The current running algorithm.
        ///        - The generation of the NetworkElements to be presented is done using the Class factory
        ///                  - In order to generate classes using the class factory the class factory's members has to be set  
        ///                  - This parameter is used to recover the members of the class factory when exiting from the dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string currentRunningAlgorithm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true if quited.
        ///                 -  The construction of the dialog is by using another dialog for selecting the algorithms
        ///                 -  If the user quited this dialog there is a need to inform the caller
        ///                    not to activate the ShowDialog() method.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool quited = false;


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ScanningType) - Type of the scanning.
        ///        This member holds the code for the scanning current been processed
        ///        (See detailed explanation in ScanningType enum)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ScanningType scanningType = ScanningType.ElementWindowOperations;

        /// \brief  (string) - The EOL.
        protected string eol = Environment.NewLine;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to skip show.
        ///        When generating the code each code part is presented
        ///        This flag is used to signal to finish the code generation without showing the code.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool skipShow;
        #endregion
        #region /// \name Constructors and Init the window

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public AddAlgorithmWindow(bool forBaseAlgorithm = false) : base(null, InputWindows.AddAlgorithmWindow, false, true)
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
        /// \date 27/08/2017
        ///
        /// \param forBaseAlgorithm (Optional)  (bool) - true to for base algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AddAlgorithmWindow(bool forBaseAlgorithm = false) : base(null, InputWindows.AddAlgorithmWindow, false, true)
        {
            if (forBaseAlgorithm)
            {
                return;
            }

            PresentExistingValues = false;
            if (!GetAlgorithmSubjectAndName())
            {
                quited = true;
                return;
            }
            CreateNetworkElements();
            FileUtilities.CreateCodeDir(targetSubject, targetAlgorithm);
            FileUtilities.CreateDirsForNeteork(targetSubject, targetAlgorithm, targetAlgorithm);
            Config.Instance.AddAlgorithmsData(false);
            InitWindow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void FillWindowName()
        ///
        /// \brief Fill window name.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/10/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void FillWindowName()
        {
            string windowName = "Template (source) Algorithm : " + sourceSubject + "." + sourceAlgorithm + eol;
            windowName += "New (target) Algorithm : " + targetSubject + "." + targetAlgorithm;
            Label_Name.Content = windowName;
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
        /// \date 27/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CreateButtons()
        {
            CreateButton("Button_Apply", "Apply", Button_Apply_Click);
            CreateButton("Button_Create", "Create", Button_Create_Click);
            CreateButton("Button_CreateDocuments", "Create Documents", Button_CreateDocuments_Click);
            CreateButton("Button_ShowDocument", "Show Documents", Button_ShowDocuments_Click);
            CreateButton("Button_ShowPseudoCode", "Show Pseudo-Code", Button_ShowPseudoCode_Click);
            CreateButton("Button_Exit", "Exit", Button_Exit_Click);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected bool GetAlgorithmSubjectAndName()
        ///
        /// \brief Gets algorithm subject and name.
        ///
        /// \par Description.
        ///      Activate a dialog for getting the source and target subject and algorithm names
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool GetAlgorithmSubjectAndName()
        {
            SourceTargetAlgorithmsSelect sourceTargetAlgorithmsSelect = new SourceTargetAlgorithmsSelect();
            sourceTargetAlgorithmsSelect.ShowDialog();
            if (sourceTargetAlgorithmsSelect.result == SourceTargetAlgorithmsSelect.SelectResult.Select)
            {
                sourceSubject = sourceTargetAlgorithmsSelect.selectResults[0].selectionText;
                sourceAlgorithm = sourceTargetAlgorithmsSelect.selectResults[1].selectionText;
                targetSubject = FirstLetterToUpper(sourceTargetAlgorithmsSelect.selectResults[2].selectionText);
                targetAlgorithm = FirstLetterToUpper(sourceTargetAlgorithmsSelect.selectResults[3].selectionText);
                replaceUserCodeFiles = sourceTargetAlgorithmsSelect.replaceUserCodeFiles;
                newAlgorithm = sourceTargetAlgorithmsSelect.newAlgorithm;
                return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void CreateNetworkElements()
        ///
        /// \brief Creates network elements.
        ///
        /// \par Description.
        ///      After getting the names of the source and target algorithms and subjects
        ///      the NetworkElements are generated (Using the Class Factory)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void CreateNetworkElements()
        {
            // Save the current running algorithm for recovery of the Class Factory when exiting
            currentRunningSubject = Config.Instance[Config.Keys.SelectedSubject];
            currentRunningAlgorithm = Config.Instance[Config.Keys.SelectedAlgorithm];

            // Set the members of the class factory
            // If the source algorithm does not exist - Use default - The Template algorithm
            if (!TypesUtility.GetAlgorithms().Contains(sourceSubject + "." + sourceAlgorithm))
            {
                Config.Instance[Config.Keys.SelectedSubject] = "Templates";
                Config.Instance[Config.Keys.SelectedAlgorithm] = "Template";
            }
            else
            {
                Config.Instance[Config.Keys.SelectedSubject] = sourceSubject;
                Config.Instance[Config.Keys.SelectedAlgorithm] = sourceAlgorithm;
            }

            // Generate and initialize the NetworkElements
            BaseNetwork network = ClassFactory.GenerateNetwork();
            network.Init(0);
            BaseProcess process = ClassFactory.GenerateProcess(network);
            process.Init(0);
            BaseChannel channel = ClassFactory.GenerateChannel(network);
            channel.Init(0);

            // The message object is created dynamically by the user during executing time
            // Therefor there is no way to generate an object that includes all the enums
            // So we create a demo NetworkElement with all messages in OperationResults dictionary
            NetworkElement messageDemo = CreateMessageDemo(network, process);
            networkElements = new List<NetworkElement> { messageDemo, network, process, channel };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected NetworkElement CreateMessageDemo(BaseNetwork network, BaseProcess process)
        ///
        /// \brief Creates message demo.
        ///
        /// \par Description.
        ///      The message demo is a NetworkElement that holds the messages in the OperatopnResult dictionary
        ///      From the user point of view the building of a message is similar to all the insertions
        ///      of attributes to a dictionary. The generation code produces other methods
        ///      This method loads the messages fro the source file  
        ///
        /// \par Algorithm.
        ///       - The message class does not have a method that produces the messages from the code
        ///       - Therefore the MessageTypes enum is scanned and from it the method name of the method   
        ///         that generates the data structure for the message  is generated and the method is activated
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/10/2017
        ///
        /// \param network  (BaseNetwork) - The network.
        /// \param process  (BaseProcess) - The process.
        ///
        /// \return The new message demo.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected NetworkElement CreateMessageDemo(BaseNetwork network, BaseProcess process)
        {
            // Create the message demo object
            NetworkElement messageDemo = new NetworkElement(network);

            // Get the keys from the message types enum. from it the dictionary generation method 
            // will be produced 
            List<dynamic> messagesKeys = TypesUtility.GetAllEnumValues(ClassFactory.GenerateMessageTypeEnum());

            // For each key in the MessageTypes enum call a method that returns the dictionary 
            // that represent the message and is put in the OperationResults dictionary
            foreach (dynamic messageKey in messagesKeys)
            {
                messageDemo.or[messageKey] = ClassFactory.MessageDictionary(messageKey, messageDemo, process);
            }
            return messageDemo;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override ElementWindowPrms MainElementPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Main element prms.
        ///
        /// \par Description.
        ///      -  The process of creating the fields for the NetworkElements starts by creating
        ///         an Attribute for the NetworkElement
        ///      -  This Attribute does not exist as part of the NetworkElement scanned but rather wraps  
        ///         The NetworkElement
        ///      -  This method allows us to set the ElementWindow prms for presenting in the window  
        ///      -  (The method of the setting the ElementWindowPrms is assigned to the Attribute)  
        ///      -  What this method does is to changed the names of the Header TextBox to a general name of the classes 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEdittable     (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override ElementWindowPrms MainElementPrms(Attribute attribute,
           dynamic key,
           NetworkElement mainNetworkElement,
           NetworkElement.ElementDictionaries mainDictionary,
           NetworkElement.ElementDictionaries dictionary,
           InputWindows inputWindow,
           bool windowEditable)
        {
            ElementWindowPrms elementWindowPrms = new ElementWindowPrms();
            elementWindowPrms.keyText = "Messages";
            if (mainNetworkElement is BaseNetwork)
            {
                elementWindowPrms.keyText = "Network";
            }
            if (mainNetworkElement is BaseProcess)
            {
                elementWindowPrms.keyText = "Process";
            }
            if (mainNetworkElement is BaseChannel)
            {
                elementWindowPrms.keyText = "Channel";
            }
            return elementWindowPrms;
        }
        #endregion
        #region /// \name Buttons handlers

        public void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            UpdateNetworkElement();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Button_Create_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Create for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            if (CreateCodeFiles())
            {
                UpdateConfig();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Button_CreateDocuments_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Create for click events.
        ///
        /// \par Description.
        ///      Activate the DocumentHandler window.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Button_CreateDocuments_Click(object sender, RoutedEventArgs e)
        {
            wordDocumentHolder.CloseAllWordDocuments();
            DocumentHandler documentHandler = new DocumentHandler(targetSubject, targetAlgorithm);
            if (!documentHandler.constructorQuited)
            {
                documentHandler.ShowDialog();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Button_ShowDocuments_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ShowDocuments for click events.
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

        public void Button_ShowDocuments_Click(object sender, RoutedEventArgs e)
        {
            DocumentHandler.LoadDocumentation(wordDocumentHolder, targetSubject, targetAlgorithm);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Button_ShowDocuments_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ShowDocuments for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Button_ShowPseudoCode_Click(object sender, RoutedEventArgs e)
        {
            DocumentHandler.LoadPseudoCode(wordDocumentHolder, targetSubject, targetAlgorithm);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Button_Exit_Click(object sender, RoutedEventArgs e)
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
        /// \date 27/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            // If there where changes that where not inserted to the code file
            // Get permission to regenerate the code file 
            if (controlsAttributeLinks.Any(entry => entry.Value.updateStatus != UpdateStatuses.NotUpdated))
            {
                MessageBoxResult dialogResult =
                    CustomizedMessageBox.Show("There where changes. Are you sure you want to exit ?", "AddAlgorithmDialog Message", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            // If no files where created for the algorithm (The algorithm directories are empty
            // Delete the directories (after getting permission)
            FileUtilities.RemoveCodeAndDataDirs(true, targetSubject, targetAlgorithm);
            Config.Instance.AddAlgorithmsData(false);

            // Restore Class Factory members
            Config.Instance[Config.Keys.SelectedSubject] = currentRunningSubject;
            Config.Instance[Config.Keys.SelectedAlgorithm] = currentRunningAlgorithm;
            wordDocumentHolder.QuitApplication();
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void Button_Remove_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Remove for click events.
        ///
        /// \par Description.
        ///      -  The method here is the method used by the button of the add/remove grid  
        ///      -  This method was rewritten in order to block the deletion of attributes   
        ///         of the base class
        ///      -  An attribute belongs to the base class if it is disabled (done when creating the window)  
        ///      And not removed
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void HandleRemove(ItemsControl item)
        {
            if (controlsAttributeLinks[item].updateStatus != UpdateStatuses.Removed)
            {
                if (controlsAttributeLinks[item].parentAttribute.Value is AttributeDictionary)
                {
                    if (AttributeDictionary.KeyOfThisClass(controlsAttributeLinks[item].key, targetSubject, targetAlgorithm))
                    {
                        if (ItemDisabled((TreeViewItem)item))
                        {
                            CustomizedMessageBox.Show("The entry belongs to the Base Class and cannot be removed",
                                "Add Algorithm Message", null, Icons.Error);
                            return;
                        }
                    }
                }
            }
            base.HandleRemove(item);
        }

        //protected override void HandleRemove(ItemsControl item)
        //{
        //    if (controlsAttributeLinks[item].updateStatus != UpdateStatuses.Removed)
        //    {
        //        if (ItemDisabled((TreeViewItem)item))
        //        {
        //            CustomizedMessageBox.Show("The entry belongs to the Base Class and cannot be removed",
        //                "Add Algorithm Message", null, Icons.Error);
        //            return;
        //        }
        //    }
        //    base.HandleRemove(item);
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected bool ItemDisabled(TreeViewItem item)
        ///
        /// \brief Item disabled.
        ///
        /// \par Description.
        ///      This method is used to check if an item is removable
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/10/2017
        ///
        /// \param item  (TreeViewItem) - The item.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool ItemDisabled(TreeViewItem item)
        {
            if (controlsAttributeLinks[item].newValueControl.IsEnabled)
            {
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void SetCreateAttributeDialogPrms()
        ///
        /// \brief Sets create attribute dialog prms.
        ///
        /// \par Description.
        ///      Set the parameters for window for creating new attribute:
        ///      -  The key has to be string  
        ///      -  If the scanned item is a message the type can be only AttributeDictionary (representing  
        ///         a new message)
        ///      
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void SetCreateAttributeDialogPrms()
        {
            base.SetCreateAttributeDialogPrms();

            // All the keys for this dialog are strings
            attributeCreationPrms.keyCategories = new List<string> { "String" };

            // If the addition is for the messages - disable all the type fields except the key
            // The selection of the attribute type is AttributeDictionary only
            if (currentComplexItem.mainNetworkElement.GetType().Equals(typeof(NetworkElement)))
            {
                string headerText = ((TextBox)((TreeViewItem)currentComplexItem.item).Header).Text;
                if (headerText == "Messages")
                {
                    attributeCreationPrms.enableEditableCheckBox = false;
                    attributeCreationPrms.enableElementWindowPrmsSelect = false;
                    attributeCreationPrms.enableEndInputOperationsSelect = false;
                    attributeCreationPrms.createKey = true;
                    attributeCreationPrms.attributeTypes = new List<string> { "DistributedAlgorithms.AttributeDictionary" };
                }
            }
        }

        protected override List<dynamic> CreateExistingKeysList(ControlsAttributeLink link)
        {
            List<dynamic> existingKeys = new List<dynamic>();
            ItemsControl networkElementItem = (ItemsControl)link.item.Parent;
            foreach (ItemsControl dictionaryItem in networkElementItem.Items)
            {
                foreach (ItemsControl chiledItem in dictionaryItem.Items)
                {
                    ControlsAttributeLink childLink = controlsAttributeLinks[chiledItem];
                    string keyStr = TypesUtility.GetKeyToString(childLink.key);
                    existingKeys.Add(char.ToUpper(keyStr[0]) + keyStr.Substring(1));
                    existingKeys.Add(char.ToLower(keyStr[0]) + keyStr.Substring(1));
                    existingKeys.Add(childLink.key);
                }
            }
            return existingKeys;
        }
        
        #endregion
        #region /// \name Scan and Report 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override bool ScanCondition(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Scans a condition.
        ///
        /// \par Description.
        ///      -  This method decides which attributes will be scanned
        ///      -  There are 2 scanning processes:
        ///         -#  Window presentation scanning (handled by ElementWindow) In this case:  
        ///             -#  For message - only the operation results is scanned
        ///             -#  For others the ElementAttributes, PrivateAttributes, OperationResults are scanned
        ///         -#  Scanning for generation of the constants in this case all the dictionaries which
        ///             are not backup dictionaries are scanned
        ///         -#  Scanning for building the code In this case:
        ///             -#  For message only the OperationResults is scanned
        ///             -#  For others the PrivateAttributes and the OperationResults are scanned          
        ///      
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param attribute       (Attribute) - The attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ScanCondition(dynamic key,
                    Attribute attribute,
                    NetworkElement.ElementDictionaries mainDictionary,
                    NetworkElement.ElementDictionaries dictionary)
        {
            // Scanning for building the window
            if (scanningType == ScanningType.ElementWindowOperations)
            {
                // Scanning Network Process Channel
                if (!networkElementScanned.GetType().Equals(typeof(NetworkElement)))
                {
                    if (mainDictionary == NetworkElement.ElementDictionaries.ElementAttributes ||
                        mainDictionary == NetworkElement.ElementDictionaries.PrivateAttributes ||
                        mainDictionary == NetworkElement.ElementDictionaries.OperationParameters ||

                        mainDictionary == NetworkElement.ElementDictionaries.OperationResults)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                // Scanning Message
                else
                {
                    if (mainDictionary == NetworkElement.ElementDictionaries.OperationResults ||
                        mainDictionary == NetworkElement.ElementDictionaries.OperationParameters)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Scanning for generating the output
            else if (scanningType == ScanningType.Build)
            {
                if (classIdx == (int)Class.Message)
                {
                    if (mainDictionary == NetworkElement.ElementDictionaries.OperationResults)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (mainDictionary == NetworkElement.ElementDictionaries.PrivateAttributes ||
                    mainDictionary == NetworkElement.ElementDictionaries.OperationResults)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Scanning for constants generation
            else
            {
                if (mainDictionary == NetworkElement.ElementDictionaries.ElementAttributes ||
                    mainDictionary == NetworkElement.ElementDictionaries.PrivateAttributes ||
                    mainDictionary == NetworkElement.ElementDictionaries.OperationResults ||
                    mainDictionary == NetworkElement.ElementDictionaries.PresentationParameters)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void AttributeReport(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Attribute report.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/10/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param attribute       (Attribute) - The attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void AttributeReport(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        {
            // If the scanning type is one of the window building and presenting - 
            // activate the ElementWindow (the base class) method
            if (scanningType == ScanningType.ElementWindowOperations)
            {
                base.AttributeReport((object)key, attribute, mainDictionary, dictionary);
                return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void OpenComplexAttribute(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Opens complex attribute.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param attribute       (Attribute) - The attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void OpenComplexAttribute(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        {
            // If the saning type is one of the window building and presenting - 
            // activate the ElementWindow (the base class) method
            if (scanningType == ScanningType.ElementWindowOperations)
            {
                base.OpenComplexAttribute((object)key, attribute, mainDictionary, dictionary);
                return;
            }

            // The following are conditions for skipping the generating a code for a complex attribute
            // Condition No. 1 the attribute is not a mainDictionary and the key is not from this class
            if (!(attribute.Parent.Parent is null) && !AttributeDictionary.KeyOfThisClass(key, targetSubject, targetAlgorithm) ||

                // Condition No. 2  the attribute is a NetworkElement
                attribute.Value is NetworkElement ||

                // Condition No.3 we are inside scanning of an attribute that should be skipped
                skipStack.Count > 0)
            {
                skipStack.Push(key);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void CloseComplexAttribute(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Closes complex attribute.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param attribute       (Attribute) - The attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void CloseComplexAttribute(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        {
            // If the scanning type is one of the window building and presenting - 
            // activate the ElementWindow (the base class) method
            if (scanningType == ScanningType.ElementWindowOperations)
            {
                base.CloseComplexAttribute((object)key, attribute, mainDictionary, dictionary);
                return;
            }

            // If scanning for algorithm building
            // If the complex attribute is not new - skip the generation of methods for it
            if (skipStack.Count > 0)
            {
                skipStack.Pop();
                return;
            }

            // If the scanning type is for collecting the enum keys
            // If the attribute is AttributeDictionary - activate the AddConsts method of the dictionary
            if (scanningType == ScanningType.Consts)
            {
                if (attribute.Value is AttributeDictionary)
                {
                    if (classIdx == (int)Class.Message && attribute.GetEnumName((object)key, true) == "ork")
                    {
                        ((AttributeDictionary)attribute.Value).AddConsts(consts, true);
                    }
                    else
                    {
                        ((AttributeDictionary)attribute.Value).AddConsts(consts, false);
                    }
                }
                return;
            }

            // If the class generated is message class we generate 2 types of methods:
            // For each entry in the MessageTypes enum - a Sending method
            // Foe each other entry a method that builds the data structure
            if (classIdx == (int)Class.Message)
            {
                if (attribute.GetEnumName((object)key, true) == "ork")
                {
                    methodsStrings[classIdx] += ((AttributeDictionary)attribute.Value).SendMessagesStrings();
                }
                else
                {
                    methodsStrings[classIdx] += attribute.Value.MessageMethodString(enumClassNames[classIdx], targetSubject, targetAlgorithm);
                }
            }

            // If the class generated is not a message we generate 2 types of methods
            // For the main dictionary we generate an InitMainDictionary method
            // For the others we generate a regular init method
            else
            {
                if (attribute.Parent.Parent is null)
                {
                    methodsStrings[classIdx] += ((AttributeDictionary)attribute.Value).InitMainDictionaryMethodString(key, enumClassNames[classIdx], ref gettersSettersStrings[classIdx], targetSubject, targetAlgorithm);
                }
                else
                {
                    methodsStrings[classIdx] += attribute.Value.InitMethodString(key, enumClassNames[classIdx], targetSubject, targetAlgorithm);
                }
            }

            // If attribute is dictionary - generate enum
            if (attribute.Value is AttributeDictionary)
            {
                enumsClassesStrings[classIdx] += ((AttributeDictionary)attribute.Value).EnumString(enumClassNames[classIdx], classIdx == (int)Class.Message, targetSubject, targetAlgorithm, syntaxHighlight);
            }
        }

        #endregion
        #region /// \name Utility methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string FirstLetterToUpper(string str)
        ///
        /// \brief First letter to upper.
        ///
        /// \par Description.
        ///      Upper case the first letter
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/10/2017
        ///
        /// \param str  (string) - The string.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
        #endregion
        #region /// \name Create A new Algorithm files

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected bool CreateCodeFiles()
        ///
        /// \brief Creates code files.
        ///        There are 2 kinds of code files generated
        ///        -#   A code file that holds the additional code constructed from:
        ///             -#  The enum classes
        ///             -#  Partial classes for the algorithm classes that holds:
        ///                 -#  Init method for the data structures that will be used in the algorithm
        ///                 -#  Message building methods for sending messages
        ///         -#  Code file that represent each one of the algorithm implementation parts
        ///             These code files are generating by replacing text in the source algorithm 
        ///             code files 
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/10/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool CreateCodeFiles()
        {
            // Save the changes done on the window to the network elements
            scanningType = ScanningType.ElementWindowOperations;
            UpdateNetworkElement();

            // Construct the consts string. This string will be inserted to each class generated
            string constsString = CreateConstsString();

            // Set the type of the scanning to build
            scanningType = ScanningType.Build;

            // This variable will hold the text for the additional code file
            string fileText = AlgorithmFileHeader();

            // This parameter collect class names and enums that are specific to the generated algorithm
            InitSyntaxHighlight();

            // Advance the version of the algorithm
            AdvanceVersion();

            for(classIdx = 0; classIdx < classNames.Count; classIdx ++)
            {
                ScanClass();
            }

            for (classIdx = 0; classIdx < classNames.Count; classIdx++)
            {
                if (!CreateClass(ref fileText, constsString))
                    return false;
            }

            // Close the text (closing of the namespace
            fileText += "}";

            // Create backup and files only at the first time the code is generated
            if (firstCodeGeneration)
            {
                // Backup the previous algorithm implementation
                if (!newAlgorithm)
                {
                    FileUtilities.CreateDirectory(ClassFactory.GenerateAlgorithmBackupPath(targetSubject, targetAlgorithm));
                    CreateBackup();
                }

                // Create the algorithm flies (by replacing the source to target strings in the source files)
                if (replaceUserCodeFiles)
                {
                    CreateAlgorithmFiles();
                }

                firstCodeGeneration = false;
            }

            // Write the auto generated code file
            File.WriteAllText(ClassFactory.GenerateAlgorithmPath(targetSubject, targetAlgorithm) + "\\" + targetAlgorithm + "DefsAndInits" + ".cs", fileText);

            CustomizedMessageBox.Show("Code Build ended", "AddAlgorithmDialog Message", Icons.Success);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateConstsString()
        ///
        /// \brief Creates consts string.
        ///
        /// \par Description.
        ///      Create a string with the constants.
        ///      The is a constant for each key that might be used by the programmer
        ///      Each const declaration is composed with the const name and a string with the const name 
        ///      as value
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ///
        /// \return The new consts string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateConstsString()
        {
            // Initial the dontScanStack which tells whether to generate
            // a code to attribute
            skipStack = new Stack<dynamic>();

            consts = new HashSet<string>();
            scanningType = ScanningType.Consts;
            classIdx = 0;
            foreach (NetworkElement ne in networkElements)
            {
                ne.ScanAndReport(this, NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None);
                classIdx++;
            }

            string s = "";
            foreach (string key in consts)
            {
                string type;
                string value;
                string constant;
                if (key.Substring(key.Length - 2) == "__")
                {
                    type = "m.MessageTypes";
                    value = "m.MessageTypes." + char.ToUpper(key[0]) + key.Substring(1, key.Length - 3);
                    constant = char.ToUpper(key[0]) + key.Substring(1, key.Length - 3);
                }
                else
                {
                    type = "string";
                    value = "\"" + char.ToUpper(key[0]) + key.Substring(1) + "\"";
                    constant = char.ToLower(key[0]) + key.Substring(1);
                }
                s += eol + "\t\tconst " + type +" " + constant + " = " + value + ";";
            }
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected bool CreateClass(ref string fileText, string constsString)
        ///
        /// \brief Creates the class.
        ///
        /// \par Description.
        ///      Create the code for a class there are 2 classes
        ///      -# An enum class. The enum class string is generated while scanning so this
        ///         method has only to present the class
        ///      -# A partial class which is composed from:
        ///         -#  A class header
        ///         -#  The constants string
        ///         -#  The getters and setters
        ///         -#  The methods
        ///         -A  A class end
        ///       Additions and exceptions
        ///       -#    For the message the methods are put in a partial class for the process
        ///             (Because the process is the class that will use the messages generating methods)
        ///       -#    The Network includes a getter / setter to hold the version of the code 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ///
        /// \param [in,out] fileText     (ref string) - The file text.
        /// \param          constsString  (string) - The consts string.
        ///
        /// \return True if it succeeds, false if it fails.
        ///          
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool CreateClass(ref string fileText, string constsString)
        {
                        
            if (!ShowClass(ref fileText, "enum", enumsClassesStrings[classIdx], enumClassNames[classIdx] + " (for class " + targetAlgorithm + classNames[classIdx] + ")"))
                return false;

            string classString = CreateClassStart(classNames[classIdx]);
            if (classIdx != (int)Class.Message)
            {
                classString += constsString;
            }
            classString += gettersSettersStrings[classIdx];
            if (classIdx == (int)Class.Network)
            {
                classString += CreateVersionMethod();
                classString += CreateSetToCentralizedAndDirected();
            }
            classString += methodsStrings[classIdx];
            classString += CreateClassEnd();

            string className;
            if (classIdx != (int)Class.Message)
                className = targetAlgorithm + "Process (Methods for sending messages)";
            else
                className = targetAlgorithm + classNames[classIdx];
            if (!ShowClass(ref fileText, "partial", classString, className))
                return false;

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ScanClass()
        ///
        /// \brief Scans the class.
        ///        Scan a network element to generate:
        ///        -#   The enum class for the network element
        ///        -#   A setters / getters string for the network element
        ///        -#   A methods string 
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ScanClass()
        {
            // Initial the dontScanStack which tells whether to generate
            // a code to attribute
            skipStack = new Stack<dynamic>();

            // Collect the data and build the sub strings
            enumsClassesStrings[classIdx] = CreateEnumClassStart();
            methodsStrings[classIdx] = "";
            gettersSettersStrings[classIdx] = "";
            networkElements[classIdx].ScanAndReport(this, NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None);
            enumsClassesStrings[classIdx] += CreateEnumClassEnd();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string AlgorithmFileHeader()
        ///
        /// \brief Algorithm file header.
        ///
        /// \par Description.
        ///      This method returns the header of the additional file
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string AlgorithmFileHeader()
        {
            string text = "using DistributedAlgorithms.Algorithms.Base.Base;";
            text += eol + "using System.Collections.Generic;" + eol + eol + eol;
            text += @"namespace DistributedAlgorithms.Algorithms." + targetSubject + "." + targetAlgorithm + eol + "{" + eol;
            return text;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void AdvanceVersion()
        ///
        /// \brief Advance version.
        ///
        /// \par Description.
        ///      -  The version is kept in the PrivateAttributes of the Network.  
        ///      -  If there is no version attribute (for example at the first time that the algorithm created)  
        ///         a new attribute is created
        ///      -  Else - the version is advanced 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 31/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void AdvanceVersion()
        {
            if (firstCodeGeneration)
            {
                try
                {
                    Type pakEnum = ClassFactory.GenerateEnum("pak", "n", targetSubject, targetAlgorithm);
                    dynamic versionKey = TypesUtility.GetKeyFromString(pakEnum, "Version");
                    int version = networkElements[(int)Class.Network].pa[versionKey];
                    networkElements[(int)Class.Network].pa.GetAttribute(versionKey).SetFromWindow( ++version, this);
                }
                catch
                {
                    networkElements[1].pa.Add("Version", new Attribute { Value = 0, Changed = false, Editable = false });
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateVersionMethod()
        ///
        /// \brief Creates version method.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ///
        /// \return The new version method.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateVersionMethod()
        {
            string s = eol + eol + eol + eol + "\t\tpublic override int GetVersion()";
            s += eol + "\t\t{";
            s += eol + "\t\t\ttry";
            s += eol + "\t\t\t{";
            s += eol + "\t\t\t\treturn pa[n.pak.Version];";
            s += eol + "\t\t\t}";
            s += eol + "\t\t\tcatch";
            s += eol + "\t\t\t{";
            s += eol + "\t\t\t\treturn 0;";
            s += eol + "\t\t\t}";
            s += eol + "\t\t}";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateSetToCentralizedAndDirected()
        ///
        /// \brief Creates set to centralized and directed.
        ///        The centralized and the DirectedNetwork are ElementAttributes.
        ///        The code generation does not recored the ElementAttributes.
        ///        Except for these NetworkParameters which values has to be set.
        ///        For setting them we use the CodeGenerationAdditionalInit method
        ///        
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/04/2018
        ///
        /// \return The new set to centralized and directed.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateSetToCentralizedAndDirected()
        {

            string s = eol + eol + eol + eol + "\t\tprotected override void CodeGenerationAdditionalInit()";
            s += eol + "\t\t{";
            string value = networkElements[classIdx].ea[bn.eak.Centrilized].ToString();
            value = char.ToLower(value[0]) + value.Substring(1);
            s += eol + "\t\t\tea[bn.eak.Centrilized] = " + value + ";";
            value = networkElements[classIdx].ea[bn.eak.DirectedNetwork].ToString();
            value = char.ToLower(value[0]) + value.Substring(1);
            s += eol + "\t\t\tea[bn.eak.DirectedNetwork] = " + value + ";";
            s += eol + "\t\t}";
            return s;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void InitSyntaxHighlight()
        ///
        /// \brief Init syntax highlight.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void InitSyntaxHighlight()
        {

            // For the syntax highlighting the target classes are added because they might not exist in
            // the current working classes 
            syntaxHighlight = new HashSet<string> { targetAlgorithm + "Network", targetAlgorithm + "Process", targetAlgorithm + "Channel", targetAlgorithm + "Message" };
            HashSet<string> enumClasses = new HashSet<string> { "n" + "\n", "p" + "\n", "c" + "\n", "m" + "\n"
               ,@"n.", @"p.", @"c.", @"m."};

            syntaxHighlight.UnionWith(enumClasses);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateEnumClassStart()
        ///
        /// \brief Creates enum class start.
        ///
        /// \par Description.
        ///      Start string for an enum class
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/10/2017
        ///
        /// \return The new enum class start.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateEnumClassStart()
        {
            string s = eol + "\t#region /// \\name Enums for " + targetAlgorithm + classNames[classIdx];
            s += eol + "\tpublic class " + enumClassNames[classIdx] + eol + "\t{";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateEnumClassEnd()
        ///
        /// \brief Creates enum class end.
        ///        End string for an enum class
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/10/2017
        ///
        /// \return The new enum class end.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateEnumClassEnd()
        {
            string s = eol + "\t}";
            s += eol + "\t#endregion";
            return s + eol;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateClassStart()
        ///
        /// \brief Creates class start.
        ///        Start string for a class
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/10/2017
        ///
        /// \return The new class start.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateClassStart(string className)
        {
            string s = eol + "\t#region /// \\name partial class for " + targetAlgorithm + className;
            s += eol + "\tpublic partial class " + targetAlgorithm + className + @": Base" + className;
            s += eol + "\t{";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string CreateClassEnd()
        ///
        /// \brief Creates class end.
        ///        End string for a class
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/10/2017
        ///
        /// \return The new class end.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string CreateClassEnd()
        {
            string s = eol + "\t}";
            s += eol + "\t#endregion";
            s += eol;
            return s;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool ShowClass(ref string fileText, string stringType, string classString, string className)
        ///
        /// \brief Shows the class.
        ///
        /// \par Description.
        ///      Show the class in a syntax highlighted message box 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/03/2018
        ///
        /// \param [in,out] fileText    (ref string) - The file text.
        /// \param          stringType   (string) - Type of the string.
        /// \param          classString  (string) - The class string.
        /// \param          className    (string) - Name of the class.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool ShowClass(ref string fileText, string stringType, string classString, string className)
        {
            if (!skipShow)
            {
                // The header label
                TextBox headerLabel = CustomizedMessageBox.SetLabel("The code for " + stringType + " class " + className,
                                new Font { fontWeight = FontWeights.Bold, fontSize = 18, alignment = HorizontalAlignment.Stretch });

                SyntaxHighlight syntaxHighlightControl = CustomizedMessageBox.SyntaxHighlightBlock(classString, syntaxHighlight, new Font { fontFamily = new FontFamily("Consolas"), fontSize = 16, alignment = HorizontalAlignment.Left });
                Button OKButton = CustomizedMessageBox.SetButton("OK");
                Button ConfirmAllButton = CustomizedMessageBox.SetButton("Confirm All");
                Button CancelButton = CustomizedMessageBox.SetButton("Cancel");
                string showResult = CustomizedMessageBox.Show(new List<Control> { headerLabel, syntaxHighlightControl },
                    "Add Algorithm Dialog",
                    new List<Button> { OKButton, ConfirmAllButton, CancelButton }, Icons.Info, false);
                switch (showResult)
                {
                    case "OK":
                        fileText += classString;
                        return true;
                    case "Confirm All":
                        fileText += classString;
                        skipShow = true;
                        return true;
                    case "Cancel":
                        return false;
                    default:
                        return false;
                }
            }
            else
            {
                fileText += classString;
                return true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateAlgorithmFiles()
        ///
        /// \brief Creates algorithm file.
        ///
        /// \par Description.
        ///      This method creates the actual algorithm file by replacing the source with the target strings.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateAlgorithmFiles()
        {
            for (classIdx = 0; classIdx < classNames.Count; classIdx++)
            {
                string text = File.ReadAllText(Config.Instance[Config.Keys.AlgorithmsPath] + "\\" + sourceSubject + "\\" + sourceAlgorithm + "\\" + sourceAlgorithm + classNames[classIdx] + ".cs");
                text = text.Replace(sourceSubject + "." + sourceAlgorithm, targetSubject + "." + targetAlgorithm);
                text = text.Replace(sourceAlgorithm, targetAlgorithm);
                File.WriteAllText(Config.Instance[Config.Keys.AlgorithmsPath] + "\\" + targetSubject + "\\" + targetAlgorithm + "\\" + targetAlgorithm + classNames[classIdx] + ".cs", text);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CreateBackup()
        ///
        /// \brief Creates the backup.
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

        private void CreateBackup()
        {
            string algorithmDir = ClassFactory.GenerateAlgorithmPath(targetSubject, targetAlgorithm);
            string backupDir = ClassFactory.GenerateAlgorithmBackupPath(targetSubject, targetAlgorithm);
            FileUtilities.CopyDirFiles(algorithmDir, backupDir);
        }

        #endregion
        #region ///\name Update the config and AlgorithmData folder

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void UpdateConfig()
        ///
        /// \brief Updates the configuration.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void UpdateConfig()
        {
            Config.Instance.CreateFromExisting(null, null, false, false);
            Config.Instance.LoadConfig();
            CustomizedMessageBox.Show("Finished generating " + targetAlgorithm + " Algorithm", "Main Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateDirsForAlgorithm(string algorithmName, string algorithmsDataPath)
        ///
        /// \brief Creates algorithm data directory.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/08/2017
        ///
        /// \param algorithmName       (string) - Name of the algorithm.
        /// \param algorithmsDataPath  (string) - Full pathname of the algorithms data file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateDirsForAlgorithm(string algorithmName, string algorithmsDataPath)
        {
            string subject = algorithmName.Substring(0, algorithmName.IndexOf("."));
            string algorithm = algorithmName.Replace(subject + ".", "");

            // Create directory for the subject if not exists
            if (!Directory.Exists(Path.GetFullPath(algorithmsDataPath + "\\" + subject)))
            {
                FileUtilities.CreateDirectory(Path.GetFullPath(algorithmsDataPath + "\\" + subject));
            }

            // Create the data directory for the algorithm
            string algorithmPath = algorithmsDataPath + subject + "\\" + algorithm;
            FileUtilities.CreateDirectory(Path.GetFullPath(algorithmPath));

            FileUtilities.CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "NetworkData"));
            FileUtilities.CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documentation"));
            FileUtilities.CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documentation\\Source"));
            FileUtilities.CreateDirectory(Path.GetFullPath(algorithmPath + "\\" + "Documentation\\Processed"));
        }
        #endregion

    }
}
