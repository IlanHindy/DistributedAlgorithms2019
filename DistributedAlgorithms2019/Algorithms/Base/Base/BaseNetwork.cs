////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\Network.cs
///
///\brief   Implements the network class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Windows;
using System.Net.Sockets;
using System.Net;
using System.Windows.Controls;
using DistributedAlgorithms;


namespace DistributedAlgorithms.Algorithms.Base.Base
{
    #region /// \name Enums

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class bn
    ///
    /// \brief A button.
    ///
    /// \par Description.
    ///      This class holds the enums decelerations of BaseNetwork
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 27/06/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class bn
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum eak
        ///
        /// \brief Keys to be used in the ElementAttributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum eak { Algorithm, Centrilized, DirectedNetwork}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum opk
        ///
        /// \brief Keys to be used in the Operation Parameters .
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum opk { Breakpoints }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum eak
        ///
        /// \brief Keys to be used in the PrivateAttributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum pak { Version }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ork
        ///
        /// \brief Keys to be used in the OperationResults dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ork { SingleStepStatus}
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BaseNetwork
    ///
    /// \brief A base network.
    ///
    /// \par Description.
    ///        -    This class represents a network.
    ///        -    The network is composed from Processes and Channels that connects them
    ///        -    when working there are the following phases
    ///        -#  Init    - create a new network
    ///        -#  Design  - Design the network and fill it's parameters
    ///        -#  Check   - Check if the network design is leagal according to the algorithm specifications
    ///        -#  Create  - Create the network from the software point of view
    ///        -#  Run     - Run the algorithm
    ///        -#  Presentation - update the presentation while running
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilan Hindy
    /// \date 08/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class BaseNetwork : NetworkElement
    {
        #region /// \name Member Variables

        /// \brief The processes.
        public List<BaseProcess> Processes = new List<BaseProcess>();

        /// \brief The channels.
        public List<BaseChannel> Channels = new List<BaseChannel>();
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseNetwork():base(true)
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///      See explanation about constructors in NetworkElement
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseNetwork():base(null)
        {
            network = this;
        }

        #endregion
        #region /// \name Init (methods that are activated while in Init phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void InitNetwork(int processMaxLeft, int processMaxHeight)
        ///
        /// \brief Init network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ///
        /// \param processMaxLeft    (int) - The process maximum left.
        /// \param processMaxHeight  (int) - Height of the process maximum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void InitNetwork(int processMaxLeft, int processMaxHeight)
        {
            ClearNetwork();
            
            //Creat an initial network
            CreateInitNetwork(processMaxLeft, processMaxHeight);
            
            //Init single step breakpoints
            Breakpoint breakpoint = op[bn.opk.Breakpoints];
            breakpoint.SetSingleStep();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void CreateInitNetwork(int processMaxLeft, int processMaxHeight)
        ///
        /// \brief Creates init network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ///
        /// \param processMaxLeft    (int) - The process maximum left.
        /// \param processMaxHeight  (int) - Height of the process maximum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void CreateInitNetwork(int processMaxLeft, int processMaxHeight)
        {
            Random random = new Random();

            //Init the network
            Init(0);

            //Init the processes
            int numberOfProcesses = 2;
            int channelIdx = 0;
            for (int idx = 0; idx < numberOfProcesses; idx++)
            {
                //Create and init a process
                BaseProcess process = ClassFactory.GenerateProcess(this);
                Processes.Add(process);
                process.Init(idx);
                process.pp[bp.ppk.FrameLeft] = random.Next(0, processMaxLeft - (int)process.pp[bp.ppk.FrameWidth]);
                process.pp[bp.ppk.FrameTop] = random.Next(0, processMaxHeight - (int)process.pp[bp.ppk.FrameHeight]);
                

                //Create a Channel from the process to itself - used to terminate the sockets
                //Used by the process
                BaseChannel channel = ClassFactory.GenerateChannel(this, channelIdx, idx, idx);
                Channels.Add(channel);
                channelIdx++;
            }

            //Init the other channels
            int numberOfChannels = 4;
            for (; channelIdx < numberOfChannels; channelIdx++)
            {
                BaseChannel channel = ClassFactory.GenerateChannel(this, channelIdx, channelIdx % 2, (channelIdx + 1) % 2);
                Channels.Add(channel);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override sealed void InitElementAttributes(int idx)
        ///
        /// \brief Init element attributes.
        ///
        /// \par Description.
        ///      Add all the ElementAttributes to the ea dictionary
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param idx The index.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitElementAttributes(int idx)
        {
            base.InitElementAttributes(idx);

            //Init the attributes of the network
            ea.Add(bn.eak.Algorithm, new Attribute { Value = Config.Instance[Config.Keys.SelectedAlgorithm], Changed = false });
            ea.Add(bn.eak.Centrilized, new Attribute { Value = true, Changed = false, ElementWindowPrmsMethod= NetAttrComboBoxSetting, EndInputOperation = CheckAndCorrectCentralizedNetwork });
            ea.Add(bn.eak.DirectedNetwork, new Attribute { Value = true, Changed = false, ElementWindowPrmsMethod = NetAttrComboBoxSetting, EndInputOperation = CheckAndCorrectDirectedNetwork });            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override sealed void InitPrivateAttributes()
        ///
        /// \brief Init private attributes.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitPrivateAttributes()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override sealed void InitOperationParameters()
        ///
        /// \brief Init operation parameters.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override sealed void InitOperationParameters()
        {
            op.Add(bn.opk.Breakpoints, new Attribute { Value = new Breakpoint(Breakpoint.HostingElementTypes.Network), Editable = false, Changed = false, IncludedInShortDescription = false });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitOperationResults()
        ///
        /// \brief Init operation results.
        ///
        /// \par Description.
        ///      Add all the OperationResults to the or dictionary
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitOperationResults()
        {
            base.InitOperationResults();
            or.Add(bn.ork.SingleStepStatus, new Attribute { Value = false, Editable = false, Changed = false });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected sealed override void InitPresentationParameters()
        ///
        /// \brief Init presentation parameters.
        ///
        /// \par Description.
        ///      Add all the PresentationParameters to the pp dictionary
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected sealed override void InitPresentationParameters()
        {
            base.InitPresentationParameters();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void AdditionalInitiations()
        ///
        /// \brief Additional initiations.
        ///
        /// \par Description.
        ///      This method is activated at the end of the init phase to handle all the actions needed
        ///      that are not addition of attributes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void AdditionalInitiations()
        {
            base.AdditionalInitiations();
        }
        #endregion
        #region /// \name Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ClearNetwork()
        ///
        /// \brief Clears the network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearNetwork()
        {
            Clear();
            foreach (BaseProcess process in Processes)
            {
                process.Clear();
            }
            Processes.Clear();

            foreach (BaseChannel channel in Channels)
            {
                channel.Clear();
            }
            Channels.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void LoadNetwork(string dataFileName, string dataFilePath)
        ///
        /// \brief Loads a network.
        ///
        /// \par Description.
        ///        Loads a network from file.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param dataFileName  (string) - Filename of the data file.
        /// \param dataFilePath  (string) - Full pathname of the data file.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadNetwork(string dataFileName = null, string dataFilePath = null)
        {
            if (dataFileName is null)
            {
                dataFileName = Config.Instance[Config.Keys.SelectedDataFileName];
            }

            if (dataFilePath is null)
            {
                dataFilePath = Config.Instance[Config.Keys.SelectedDataPath];
            }


            ClearNetwork();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataFilePath + dataFileName);

            // Load the network attributes that ae xml attributes in "GeneralInformation" node
            Load(xmlDoc.GetElementsByTagName("GeneralInformation")[0]);

            // Log Filters 
            Logger.LoadFilters(xmlDoc.GetElementsByTagName("LogFilters")[0]);

            //Load the processes using the NetworkElement's Load method
            Processes.Clear();
            foreach (XmlNode processNode in xmlDoc.GetElementsByTagName("Processes")[0].ChildNodes)
            {
                BaseProcess process = ClassFactory.GenerateProcess(this);
                process.Load(processNode);
                Processes.Add(process);
            }

            //Load the Channels using the NeyworkElement's Load method
            Channels.Clear();
            foreach (XmlNode channelNode in xmlDoc.GetElementsByTagName("Channels")[0].ChildNodes)
            {
                BaseChannel channel = ClassFactory.GenerateChannel(this);
                channel.Load(channelNode);
                Channels.Add(channel);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SaveNetwork(bool isDebug)
        ///
        /// \brief Saves a network.
        ///        Saves network to file.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param isDebug  (bool) - true if this object is debug.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SaveNetwork(bool isDebug, string fileName, string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("DistributedAlgorithmNetwork");
            xmlDoc.AppendChild(rootNode);

            //Save the networ's attributes using the Save method of NetworkElement
            rootNode.AppendChild(Save(xmlDoc, xmlDoc.CreateElement("GeneralInformation")));

            //Save the log filters
            XmlNode logFiltersNode = xmlDoc.CreateElement("LogFilters");
            Logger.SaveFilters(xmlDoc, logFiltersNode);
            rootNode.AppendChild(logFiltersNode);

            //Create a root node for the processes and save the processes in it
            XmlNode processesNode = xmlDoc.CreateElement("Processes");
            rootNode.AppendChild(processesNode);
            
            foreach (BaseProcess process in Processes)
            {
                processesNode.AppendChild(process.Save(xmlDoc, xmlDoc.CreateElement("Process")));
            }

            //Create the root node for the channels and save the channels in it
            XmlNode channelsNode = xmlDoc.CreateElement("Channels");
            rootNode.AppendChild(channelsNode);
            foreach (BaseChannel channel in Channels)
            {
                channelsNode.AppendChild(channel.Save(xmlDoc, xmlDoc.CreateElement("Channel")));
            }

            //Save the XmlDocument in a file
            if (isDebug)
            {
                xmlDoc.Save(filePath + fileName);
            }
            else
            {
                xmlDoc.Save(filePath + fileName);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<BaseChannel> CollectChannelsConnectedToProcess(int processId)
        ///
        /// \brief Collect channels connected to process.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param processId  (int) - Identifier for the process.
        ///
        /// \return A List&lt;BaseChannel&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<BaseChannel> CollectChannelsConnectedToProcess(int processId)
        {
            return Channels.Where(channel => channel.ea[bc.eak.SourceProcess] == processId
                || channel.ea[bc.eak.DestProcess] == processId).ToList();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ReindexProcesses(bool changeProcessNames)
        ///
        /// \brief Reindex processes.
        ///
        /// \par Description.
        ///      This method is activated by the gui to reindex the processors (so that the ids
        ///      will be continues)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param changeProcessNames  (bool) - true to change process names.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReindexProcesses(bool changeProcessNames)
        {
            for (int idx = 0; idx <= Processes.Last().ea[ne.eak.Id]; idx++)
            {
                if (!Processes.Any(process => process.ea[ne.eak.Id] == idx))
                {
                    ReplaceProcessId(Processes.Last().ea[ne.eak.Id], idx, changeProcessNames);
                    Processes = Processes.OrderBy(p => p.ea[ne.eak.Id]).ToList();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ReplaceProcessId(int fromIdx, int toIdx, bool changeProcessName)
        ///
        /// \brief Replace process index.
        ///
        /// \par Description.
        ///      Replace the indexes between 2 processes (and the source process id and dest process id
        ///      in the channels)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param fromIdx           (int) - Zero-based index of from.
        /// \param toIdx             (int) - Zero-based index of to.
        /// \param changeProcessName (bool) - true to change process name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ReplaceProcessId(int fromIdx, int toIdx, bool changeProcessName)
        {
            BaseProcess process = Processes.First(p => p.ea[ne.eak.Id] == fromIdx);
            bool processHasDefaultName = false;
            if (process.ea[bp.eak.Name] == process.GetProcessDefaultName())
            {
                processHasDefaultName = true;
            }
            process.ea[ne.eak.Id] = toIdx;
            if (changeProcessName || processHasDefaultName)
            {
                process.ea[bp.eak.Name] = process.GetProcessDefaultName();
            }
            ReplaceChannelsId(fromIdx, toIdx);
            process.Presentation.UpdatePresentation();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ReindexChannels()
        ///
        /// \brief Reindex channels.
        ///
        /// \par Description.
        ///      This method is activated by the gui to reindex the channels (so that the ids
        ///      will be continues)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReindexChannels()
        {
            for (int idx = 0; idx <= Channels.Last().ea[ne.eak.Id]; idx++)
            {
                if (!Channels.Any(c => c.ea[ne.eak.Id] == idx))
                {
                    Channels.Last().ea[ne.eak.Id] = idx;
                    Channels = Channels.OrderBy(c => c.ea[ne.eak.Id]).ToList();
                }
            }
            foreach (BaseChannel channel in Channels)
            {
                channel.pp[bc.ppk.PresentationTxt] =
                    channel.ea[ne.eak.Id].ToString();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ReplaceChannelsId(int fromIdx, int toIdx)
        ///
        /// \brief Replace channels source and destination.
        ///
        /// \par Description.
        ///      This method replaces the channel id of a channel.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param fromIdx  (int) - Zero-based index of from.
        /// \param toIdx    (int) - Zero-based index of to.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ReplaceChannelsId(int fromIdx, int toIdx)
        {
            foreach (BaseChannel channel in Channels)
            {
                if (channel.ea[bc.eak.SourceProcess] == fromIdx)
                {
                    channel.ea[bc.eak.SourceProcess] = toIdx;
                }
                if (channel.ea[bc.eak.DestProcess] == fromIdx)
                {
                    channel.ea[bc.eak.DestProcess] = toIdx;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckAndCorrectDirectedNetwork(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Check and correct directed network.
        ///
        /// \par Description.
        ///      -  After the directed network attribute was changed in the edit network window
        ///         this method is activated.
        ///      -  If the directed network is false all the channels has to have a backword channel.
        ///
        /// \par Algorithm
        ///      -  If the value of the parameter is false (means not directed network)
        ///         -   If there are channels that do not have a channel in the oposit direction (backword channel)
        ///             -   Ask the user if he wants to add backword channels
        ///                 -   If he does create backword channel to all the channels that has no backword channel.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
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

        public static bool CheckAndCorrectDirectedNetwork(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";
            List<BaseChannel> channelsThatNeedsBackwords = network.CollectChannelsThatNeedsBeckword(ref errorMessage, bool.Parse(newValue));
            if (channelsThatNeedsBackwords.Count > 0)
            {
                errorMessage += "Do you want to create reverse channels ? ";
                MessageBoxResult dialogResult = CustomizedMessageBox.Show(errorMessage, "Backword Channel Dialog", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    network.CreateBackwordChannels(channelsThatNeedsBackwords);
                    return true;
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    return false;
                }
            }
            errorMessage = "";
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckAndCorrectCentralizedNetwork(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Check and correct centalized network.
        ///
        /// \par Description.
        ///      -  After the centralized attribute was changed to true in the edit network window
        ///         This method make it possble for the user to select the initiator
        ///      -  In centalized networks there can be only one initiator      
        ///
        /// \par Algorithm.
        ///      -  If the parameter was changed to true (centralized network)
        ///         -   If the number of initiators is larger than 1
        ///             -   Ask the user if he wants to select the initiator
        ///                 -   If he does, show a select dialog with the names of all the initiators
        ///                 -   After exiting the dialog set the initiator attribute of all the not selected processes to false  
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
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

        public static bool CheckAndCorrectCentralizedNetwork(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            if (attribute.Value == false && newValue == "True")
            {
                List<BaseProcess> initiators = network.Processes.Where(process => process.ea[bp.eak.Initiator] == true).ToList();
                if (initiators.Count > 1)
                {
                    errorMessage = "There is more than one initiators Do you select one of them ? ";
                    List<string> processNames = new List<string>();
                    foreach (BaseProcess process in initiators)
                    {
                        processNames.Add(process.ToString());
                    }
                    SelectDialog selectDialog = new SelectDialog(errorMessage, "Select Process Dialog", processNames);
                    selectDialog.ShowDialog();
                    if (selectDialog.Result == SelectDialog.SelectDialogResult.Select)
                    {
                        for (int idx = 0; idx < initiators.Count; idx++)
                        {
                            if (idx != selectDialog.Selection[0])
                            {
                                initiators[idx].ea[bp.eak.Initiator] = false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            errorMessage = "";
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<BaseChannel> CollectChannelsThatNeedsBeckword(ref string errorMessage, bool directedAttributeValue)
        ///
        /// \brief Collect channels that needs beckword.
        ///
        /// \par Description.
        ///      If the network is not a directed network
        ///      collect all the channels that don't have a channel in the oposit direction.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param [in,out] errorMessage           (ref string) - Message describing the error.
        /// \param          directedAttributeValue  (bool) - true to directed attribute value.
        ///
        /// \return A List&lt;BaseChannel&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<BaseChannel> CollectChannelsThatNeedsBeckword(ref string errorMessage, bool directedAttributeValue)
        {
            List<BaseChannel> channelsThatNeedsBackword = new List<BaseChannel>();
            errorMessage = "";
            if (directedAttributeValue == false)
            {
                channelsThatNeedsBackword = Channels.Where(channel => channel.CheckIfBackwordChannelNeeded(this) == true).ToList();
                if (channelsThatNeedsBackword.Count > 0)
                {
                    errorMessage = "The following channels does not have a reverse channel : \n ";
                    foreach (BaseChannel channel in channelsThatNeedsBackword)
                    {
                        errorMessage += channel.ToString() + "\n";
                    }
                }
            }
            return channelsThatNeedsBackword;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateBackwordChannels(object channelsThatNeedsBackword)
        ///
        /// \brief Creates backword channels.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///      For each channel that has no backword channel activate the CreateBackwordChannel method
        ///      of the channel.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param channelsThatNeedsBackword  (object) - The channels that needs backword.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateBackwordChannels(object channelsThatNeedsBackword)
        {
            ((List<BaseChannel>)channelsThatNeedsBackword).ForEach((BaseChannel channel) => channel.CreateBackworedChannel(this));
        }
        #endregion
        #region /// \name Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CheckNetworkkBuild()
        ///
        /// \brief Check networkk build.
        ///
        /// \par Description.
        ///      This method is the main method of the Check phase. It is activated from MainWindow
        ///
        /// \par Algorithm.
        ///      -# Check and corrected all the channels:
        ///         -#  Open Edit window if it was not editted and the user confirms to edit
        ///         -#  Activate the CheckAndCorrectBuild method of the channel
        ///      -# Check and correct all the processes:
        ///         -#  Open Edit window if it was not editted and the user confirms to edit
        ///         -#  Activate the CheckAndCorrectBuild method of the process      
        ///      -# Check and correct the network:
        ///         -#  Open Edit window if it was not editted and the user confirms to edit
        ///         -#  Activate the CheckAndCorrectBuild metho of the networkd      

        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CheckNetworkkBuild()
        {
            List<BaseChannel> channelsThatWhereChecked = new List<BaseChannel>();
            List<BaseChannel> channelsThatWhereNotChecked = new List<BaseChannel>(Channels);
            bool noToAll = false;
            while (channelsThatWhereNotChecked.Count > 0)
            {
                BaseChannel channel = channelsThatWhereNotChecked[0];
                if (noToAll)
                {
                    channel.ea[ne.eak.Edited] = true;
                }
                else
                { 
                    noToAll = channel.EditIfNeeded();
                }
                channel.CheckAndCorrectBuild();
                channelsThatWhereChecked.Add(channel);
                channelsThatWhereNotChecked = Channels.Except(channelsThatWhereChecked).ToList();
            }

            List<BaseProcess> processesThatWhereChecked = new List<BaseProcess>();
            List<BaseProcess> processesThatWhereNotChecked = new List<BaseProcess>(Processes);
            noToAll = false;
            while (processesThatWhereNotChecked.Count > 0)
            {
                BaseProcess process = processesThatWhereNotChecked[0];
                if (noToAll)
                {
                    process.ea[ne.eak.Edited] = true;
                }
                else
                {
                    noToAll = process.EditIfNeeded();
                }
                process.CheckAndCorrectBuild();
                processesThatWhereChecked.Add(process);
                processesThatWhereNotChecked = Processes.Except(processesThatWhereChecked).ToList();
            }
            CheckAndCorrectBuild();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///
        /// \par Description.
        ///      This is the main method that holds all the checks that are done to a network
        ///      Currently there are 2 checks;
        ///      -# If the network is centralized it has to have only one initiator process
        ///      -# If this is the base network activate the CheckBaseNetwork method (which include several checks)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            CheckCentrilizedParameters(buildCorrectionsParameters);
            if (Config.Instance[Config.Keys.SelectedAlgorithm] == "Base")
            {
                CheckBaseNetwork(buildCorrectionsParameters);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CheckCentrilizedParameters(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check centrilized parameters.
        ///
        /// \par Description.
        ///      If the network is centralized it has to have exactlly one initiator process.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CheckCentrilizedParameters(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            int numberOfInitiators = Processes.Count(process => process.ea[bp.eak.Initiator] == true);
            if (numberOfInitiators == 0 || (ea[bn.eak.Centrilized] == true && numberOfInitiators > 1))
            {
                BuildCorrectionParameters buildCorrectionParameter = new BuildCorrectionParameters();
                buildCorrectionParameter.CorrectionMethod = CorrectCentrilizedParameters;
                buildCorrectionParameter.ErrorMessage = "The Centralized structure of the network is wrong ";

                buildCorrectionsParameters.Add(buildCorrectionParameter);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CorrectCentrilizedParameters(object parameters)
        ///
        /// \brief Correct centralized parameters.
        ///
        /// \par Description.
        ///        Correct the centralized parameter using dialogs.
        ///
        /// \par Algorithm.
        ///        -#   If the number of the initializers is 0
        ///             -#  If the network is centralized
        ///                 -# Show a select dialog with all the processes and with single select
        ///             -#  If the network is not centralized
        ///                 -# Show a select dialog with all the processes and with multy select
        ///             -# set all the selected processes initiators
        ///        -#   If the network is centralized
        ///             -#  If the number of initiators is larger than 1
        ///                 -#  Show a select dialog with single select from all the initiators
        ///                 -#  Set the initializer attribute of all the not selected processes to false.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param parameters  (object) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CorrectCentrilizedParameters(object parameters)
        {
            List<BaseProcess> initiators = Processes.Where(process => process.ea[bp.eak.Initiator] == true).ToList();
            bool centrilizedNetwork = ea[bn.eak.Centrilized] == true;
            if (initiators.Count == 0)
            {
                // The selection will be done from all the processes
                List<string> processNames = new List<string>();
                Processes.ForEach((process) => { processNames.Add(process.ToString()); });
                string message;
                SelectionMode selectionMode;
                if (centrilizedNetwork)
                {
                    // If the network is centalized only one process can be selected
                    message = "The number of initializers is 0 Select one initializer";
                    selectionMode = SelectionMode.Single;
                }
                else
                {
                    // If the network is not centarlized many processes can be selected
                    message = "The number of initializers is 0 Select one or more initializers ";
                    selectionMode = SelectionMode.Multiple;
                }
                SelectDialog selectDialog = new SelectDialog(message, "Select Process Dialog", processNames, null, null, selectionMode);
                selectDialog.ShowDialog();

                // Set all the selected processes initiators
                for (int idx = 0; idx < selectDialog.Selection.Count; idx++)
                {
                    Processes[selectDialog.Selection[idx]].ea[bp.eak.Initiator] = true;
                }
            }
            else if (centrilizedNetwork && initiators.Count > 1)
            {

                // The selection will be done from all the initiators and will be a single select
                List<string> initiatorNames = new List<string>();
                Processes.ForEach((process) => { initiatorNames.Add(process.ToString()); });
                SelectDialog selectDialog = new SelectDialog("The network is centrilized and there is more than one initializer Select one initiator", "Select Process Dialog", initiatorNames);
                selectDialog.ShowDialog();

                // Set the initializer attribute of all the not selected processes - false
                for (int idx = 0; idx < initiators.Count; idx++)
                {
                    if (idx == selectDialog.Selection[0])
                    {

                        initiators[idx].ea[bp.eak.Initiator] = true;
                    }
                    else
                    {
                        initiators[idx].ea[bp.eak.Initiator] = false;
                    }
                }

            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CheckBaseNetwork(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check base network.
        ///
        /// \par Description.
        ///      -  This method checks the Base algorithm network
        ///      -  The base algorithm is a ping pong between the initiator and the none initiator (2 processes only)
        ///      -  The method does 3 checks and for each one that is true (means an error detected)
        ///         A value in the switches boolean list is set to true
        ///      -  The switches boolean list is given to the correct method as a parameter.
        ///
        /// \par Algorithm.
        ///      The following are the checks:
        ///      -#   Check that a process with id = 0 exist
        ///      -#   Check that a process with id = 1 exists
        ///      -#   Check that there are only 2 processes.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CheckBaseNetwork(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            BuildCorrectionParameters buildCorrectionParameters = new BuildCorrectionParameters();
            List<bool> errorSwitches = new List<bool> { false, false, false };
            buildCorrectionParameters.CorrectionParameters = new List<bool>();

            //Check that process with id = 0 exist
            if (!Processes.Any(process => process.ea[ne.eak.Id] == 0))
            {
                buildCorrectionParameters.ErrorMessage += "\n There Shuold be a process with Id 0";
                errorSwitches[0] = true;
            }

            //Check that process with id 1 exist
            if (!Processes.Any(process => process.ea[ne.eak.Id] == 1))
            {
                buildCorrectionParameters.ErrorMessage += "\n There Shuold be a process with Id 1";
                errorSwitches[1] = true;
            }

            //Check that there are only 2 processes
            if (Processes.Count > 2)
            {
                buildCorrectionParameters.ErrorMessage += "\n There Shuold be only 2 processes";
                errorSwitches[2] = true;
            }

            //Add the errors to the list if any was found
            if (errorSwitches.Any(errorSwitch => errorSwitch == true))
            {
                buildCorrectionParameters.CorrectionMethod = CorrectBaseNetwork;
                buildCorrectionParameters.CorrectionParameters = errorSwitches;
                buildCorrectionsParameters.Add(buildCorrectionParameters);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CorrectBaseNetwork(object parameters)
        ///
        /// \brief Correct base network.
        ///
        /// \par Description.
        ///      Make the following corrections:
        ///      -#   Select a process for the initiator (process with id 0)
        ///      -#   Select a process for the none initiator (process with id 1)
        ///      -#   Remove all the rest of the processes.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param parameters  (object) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CorrectBaseNetwork(object parameters)
        {
            //Correct process with Id 0 and a process with id 1 does not exist          
            for (int idx = 0; idx < 2; idx++)
            {
                if (((List<bool>)parameters)[idx] == true)
                {
                    List<string> processNames = Processes.Select((BaseProcess process) => process.ToString()).ToList();
                    List<bool> canBeSelected = Processes.Select((BaseProcess process) => (process.ea[ne.eak.Id] == (idx + 1) % 2) ? false : true).ToList();
                    string message = "Process with Id " + idx.ToString() + "does not exist - select one \n If you Quit the first one will be selected";
                    SelectDialog selectDialog = new SelectDialog(message, "Select a process dialog", processNames, null, canBeSelected);
                    selectDialog.ShowDialog();
                    int selectionIdx;
                    if (selectDialog.Result == SelectDialog.SelectDialogResult.Quit)
                    {
                        selectionIdx = 0;
                    }
                    else
                    {
                        selectionIdx = selectDialog.Selection[0];
                    }
                    ReplaceProcessId(Processes[selectionIdx].ea[ne.eak.Id], idx, false);
                }
            }

            //Remove all the rest of the processes
            if (((List<bool>)parameters)[2] == true)
            {
                Processes = Processes.OrderBy(process => process.ea[ne.eak.Id]).ToList();
                while (Processes.Count > 2)
                {
                    BaseProcess lastProcess = Processes.Last();
                    List<BaseChannel> channelsToDelete = CollectChannelsConnectedToProcess(lastProcess.ea[ne.eak.Id]);
                    foreach (BaseChannel channel in channelsToDelete)
                    {
                        channel.Clear();
                        Channels.Remove(channel);
                    }
                    lastProcess.Clear();
                    Processes.Remove(lastProcess);
                }
            }
        }


        #endregion
        #region /// \name Create (methods that are activated when the network is created

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Create()
        ///
        /// \brief Creates the network.
        ///
        /// \par Description.
        ///      Implements the create action from the gui
        ///
        /// \par Algorithm.
        ///      The method does the following
        ///      -# Create a TCP socket for the listening ports of all the processes
        ///      -# Create a sending TCP socket for sending ports of all the channels
        ///      -# Fill the List of incomming channels in each process
        ///      -# Fill the list of outgoing channels in each process 
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Create()
        {
            int numberOfProcesses = Processes.Count;
            int numberOfChannels = Channels.Count;

            // Find and set the receive (listenning) port for each process
            for (int idx = 0; idx < numberOfProcesses; idx++)
            {
                Processes[idx].AddOrReplaceOperationResult(bp.ork.ReceivePort, FindPort(), false);
                Processes[idx].OutGoingChannels.Clear();
                Processes[idx].IncommingChannels.Clear();
            }

            // Fill the ports in the channels
            for (int idx = 0; idx < numberOfChannels; idx++)
            {
                // The sending port number is saved only in the channel
                int sourceProcessId = Channels[idx].ea[bc.eak.SourceProcess];
                Channels[idx].AddOrReplaceOperationResult(bc.ork.SourcePort, FindPort(), false);
                BaseProcess sourceProcess = Processes.First(p => p.ea[ne.eak.Id] == sourceProcessId);
                sourceProcess.OutGoingChannels.Add(Channels[idx]);

                //The receive (listening port is taken from the process
                int destProcessId = Channels[idx].ea[bc.eak.DestProcess];
                BaseProcess destProcess = Processes.First(p => p.ea[ne.eak.Id] == destProcessId);
                Channels[idx].AddOrReplaceOperationResult(bc.ork.DestPort, destProcess.or[bp.ork.ReceivePort], false);
                destProcess.IncommingChannels.Add(Channels[idx]);

                // Handle MessageQ events
                GridView channelGridView = (GridView)Channels[idx].Presentation.presentationElements[Channels[idx]].controls[PresentationElement.ControlKeys.MessagesGridView];
                channelGridView.ChangeFinishedEvent = null;
                channelGridView.ChangeFinishedEvent += ((ChangeMessageOrder)destProcess.op[bp.opk.ChangeOrderEvents]).RecordOrderChange;
                channelGridView.ChangeFinishedEvent += ((MessageQ)destProcess.or[bp.ork.MessageQ]).ChangeOrder;

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public int FindPort()
        ///
        /// \brief Searches for the first free tcp port.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \return The found port.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int FindPort()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateDebug()
        ///
        /// \brief Creates the debug.
        ///
        /// \par Description.
        ///      Create the network when a debug button was pressed 
        ///
        /// \par Algorithm.
        ///      This method takes all the messages that were in the Q when the previouse running
        ///      was saved and gives them to the source processors to be sent (in the process activate method)
        ///      So that the processes will recive the messages that where in their Q before the end of
        ///      the previouse running
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateDebug()
        {
            foreach (BaseProcess process in Processes)
            {
                AttributeList messageQueue = process.or[bp.ork.MessageQ];
                while (messageQueue.Count > 0)
                {
                    BaseMessage message = messageQueue.ElementAt(0).Value;
                    if (!message.IsEmpty())
                    {
                        int sourceProcessId = message.GetHeaderField(bm.pak.SourceProcess);
                        BaseProcess sourceProcess = Processes.First(p => p.ea[ne.eak.Id] == sourceProcessId);
                        BaseMessage messageToBeSent = new BaseMessage(network, message);
                        sourceProcess.MessagesForInitInDebugeMode.Add(messageToBeSent);
                        messageQueue.RemoveAt(0);
                    }
                }
            }
        }

        #endregion
        #region /// \name Run (methods that are activated while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Activate(bool inDebugMode, bool activateFirstStep)
        ///
        /// \brief Activates this object.
        ///
        /// \par Description.
        ///      This is the method that is used to run the algorithm.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param inDebugMode        (bool) - true to enable in debug mode, false to disable it.
        /// \param activateFirstStep  (bool) - true to activate first step.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Activate(bool inDebugMode, bool activateFirstStep)
        {
            // Report to the Gui the beginning of the running
            MessageRouter.ReportStartRunning();

            //Start the processors
            foreach (BaseProcess process in Processes)
            {
                Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "Network", "Network.Activate()", "Before activating " + process.ea[bp.eak.Name], "", "ActivateTrace");
                if (!inDebugMode && !activateFirstStep)
                {
                    // Because the processes do not run there is no indicaion
                    // Which process can be activated to start the algorithm
                    // If the algorithm is not running the processes that can
                    // activate the algorithm are the initiators
                    if (process.ea[bp.eak.Initiator])
                    {
                        process.Presentation.SetSelected(process, MainWindow.SelectedStatus.Selected);
                    }
                    process.BreakpointEvent.Reset();
                }
                else
                {
                    Logger.Log(Logger.LogMode.MainLogAndMessageTrace, "Network", "In activate", "", "process " + process.ea[ne.eak.Id].ToString() + "Breakpoint Set", "RunningWindow");
                    process.BreakpointEvent.Set();
                }
                process.Activate(inDebugMode);
            }

            // Wait for the processes threads ends in a monitor thread
            Thread monitorThread = new Thread(MonitorProcessThreads);
            monitorThread.Name = "Processes end running monitoring";

            // Start the worker thread.
            monitorThread.Start();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void BackupPresentation()
        ///
        /// \brief Backup presentation.
        ///
        /// \par Description.
        ///      Backup the presentation parameter for recovery after running
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void BackupPresentation()
        {
            BackupPresentationParameters();
            foreach (BaseProcess process in Processes)
            {
                process.BackupPresentationParameters();
            }
            foreach (BaseChannel channel in Channels)
            {
                channel.BackupPresentationParameters();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void BackupInitOperationResults()
        ///
        /// \brief Backup init operation results.
        ///
        /// \par Description.
        ///      Backup the operation results for recovery after running
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void BackupInitOperationResults()
        {
            BackupOperationResults();
            foreach (BaseProcess process in Processes)
            {
                process.BackupOperationResults();
            }
            foreach (BaseChannel channel in Channels)
            {
                channel.BackupOperationResults();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void MonitorProcessThreads()
        ///
        /// \brief Monitor process threads.
        ///
        /// \par Description.
        ///      Detect when the processes finished (and the algorithm terminated)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MonitorProcessThreads()
        {
            while (true)
            {
                Thread.Sleep(1000);
                bool alive = false;
               
                // Usually the algorithm processing is finished by one
                // or more processes activate the Process's Terminate
                // method which starts a termination algorithm for the 
                // network. In case that the Processes does not know
                // if the termination conditions were fullfiled by 
                // All the other processes this method is activated
                if (!Terminate())
                {
                    alive = true;
                }
                foreach (BaseProcess process in Processes)
                {
                    if (process.IsAlive() == true)
                    {
                        alive = true;
                    }
                }
                if (alive == false)
                {
                    MessageRouter.ReportFinishRunning();
                    return;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual bool Terminate()
        ///
        /// \brief Monitor process threads.
        ///
        /// \par Description.
        ///      Usually the algorithm processing is finished by one
        ///      or more processes activate the Process's Terminate
        ///      method which starts a termination algorithm for the 
        ///      network. In case that the Processes does not know
        ///      if the termination conditions were fullfiled by 
        ///      All the other processes this method is activated
        ///
        /// \par Algorithm.
        ///      The method returns true - means
        ///      that it does not know about processes and the processes should
        ///      activate the Terminate themselvs.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 29/04/2019
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool Terminate()
        {
            return true;
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RestoreNetworkPresentationParameters()
        ///
        /// \brief Restore network presentation parameters.
        ///
        /// \par Description.
        ///      When running ends Restore the presenatation parameters to the values that 
        ///      were before the running
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RestoreNetworkPresentationParameters()
        {
            RestorePresentationParameters();
            foreach (BaseProcess process in Processes)
            {
                process.RestorePresentationParameters();
            }
            foreach (BaseChannel channel in Channels)
            {
                channel.RestorePresentationParameters();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RestoreInitOperationResults()
        ///
        /// \brief Restore init operation results.
        ///
        /// \par Description.
        ///      After running ends restore the operation results to the values that
        ///      were before the running (to be used in the next running)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RestoreInitOperationResults()
        {
            RestoreOperationResults();
            foreach (BaseProcess process in Processes)
            {
                process.RestoreOperationResults();
            }
            foreach (BaseChannel channel in Channels)
            {
                channel.RestoreOperationResults();
            }
        }
        #endregion
        #region /// \name Presentation (methods used to update the presentation)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override sealed void UpdateRunningStatus(object[] parameters)
        ///
        /// \brief Updates the running status described by parameters.
        ///
        /// \par Description.
        ///      This method is activated from the gui when a running step ended.
        ///      It causes the presentation to be updated.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param parameters  (object[]) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override sealed void UpdateRunningStatus(object[] parameters)
        {
            //The first parameter is the selected process
            List<BaseProcess> selectedProcesses = (List<BaseProcess>)parameters[0];
            foreach (BaseProcess process in Processes)
            {
                if (selectedProcesses.Any(selectedProcess => process == selectedProcess))
                {
                    process.UpdateRunningStatus(new object[] { true });
                }
                else
                {
                    process.UpdateRunningStatus(new object[] { false });
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string ToString()
        ///
        /// \brief Convert this object into a string representation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/01/2018
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            return TypesUtility.GetTypeNameOnlyToString(GetType());
        }
        #endregion
        #region /// \name Utility Methods
        public void CheckNetwork(string checkName)
        {
            List<string> errorElements = new List<string>() { "Check : " + checkName, "Error elements" };
            if (!CheckMembers(checkName + " Network"))
            {
                errorElements.Add(ToString());
            }

            foreach(BaseProcess process in Processes)
            {
                if (!process.CheckMembers(checkName + " Process Id = " + process.ea[ne.eak.Id].ToString()))
                { 
                    errorElements.Add(process.ToString());
                }
            }

            foreach (BaseChannel channel in Channels)
            {
                if (!channel.CheckMembers(checkName + " Channel Id = " + channel.ea[ne.eak.Id].ToString()))
                {
                    errorElements.Add(channel.ToString());
                }
            }

            Icons icon;
            if (errorElements.Count > 2)
            {
                errorElements.Insert(0, "Error in check");
                icon = Icons.Error;
            }
            else
            {
                errorElements.Insert(0, "Check succeeded");
                icon = Icons.Success;
            }
            CustomizedMessageBox.Show(errorElements, "Network Members Check", null, icon);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual int GetVersion()
        ///
        /// \brief Gets the version.
        ///
        /// \par Description.
        ///      -  This method is used for deciding if a network should be updated after changing the algorithm definitions
        ///      -  Virtual method for getting the version.
        ///      -  Versing is relevant only for the inherited classes  
        ///      -  This method returns a value that will not cause the network to be updated in case the algorithm is the base algorithm
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 31/12/2017
        ///
        /// \return The version.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual int GetVersion()
        {
            return 0;
        }
        #endregion
        #region /// \name Methods for setting attributes presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms NetAttrComboBoxSetting(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Net attribute combo box setting.
        ///
        /// \par Description.
        ///      Setting the presentation for Centralized and Directed attributes
        ///      The method was defined here in order to force the ability to change
        ///      the attributes in the algorithm creating window
        ///      (The attributes belongs to the ElementAttributes dictionary which
        ///      is not editable in the window)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/04/2018
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

        public static ElementWindowPrms NetAttrComboBoxSetting(Attribute attribute, dynamic key,
                   NetworkElement mainNetworkElement,
                   ElementDictionaries mainDictionary,
                   ElementDictionaries dictionary,
                   InputWindows inputWindow,
                   bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            prms.newValueControlPrms.options = new string[] { "False", "True" };
            prms.newValueControlPrms.Value = attribute.Value.ToString();
            prms.newValueControlPrms.enable = true;
            return prms;
        }
        #endregion
    }
}       

 


