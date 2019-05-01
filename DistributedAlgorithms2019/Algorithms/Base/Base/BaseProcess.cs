                                                                                                                                                                                                                                                                                                                                                                        ////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\Process.cs
///
///\brief   Implements the process class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Drawing;
using System.Windows;
using DistributedAlgorithms;


namespace DistributedAlgorithms.Algorithms.Base.Base
{
    #region /// \name Enums

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class bp
    ///
    /// \brief A bp.
    ///
    /// \par Description.
    ///       This class holds the enums declaration of BaseProcess
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 27/06/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class bp
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum eak
        ///
        /// \brief Keys to be used in the ElementAttributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum eak
        {
            Initiator, Name, Background, TextColor, TestListOfAttributes, Breakpoints
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum eak
        ///
        /// \brief Keys to be used in the PrivateAttributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum pak { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum opk
        ///
        /// \brief Keys to be used in the Operation Parameters .
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum opk { Breakpoints, InternalEvents, BaseAlgorithm, ChangeOrderEvents }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ork
        ///
        /// \brief Keys to be used in the OperationResults dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ork { ReceivePort, TerminationStatus, MessageQ, MessageToBeSentInInit, Round }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ppk
        ///
        /// \brief Keys to be used in the PresentationParameters dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ppk
        {
            FrameColor, FrameHeight, FrameWidth, FrameLeft, FrameTop, FrameLineWidth, Background, Foreground, Text,
            ///< An enum constant representing the breakpoints frame color option
            BreakpointsFrameColor, BreakpointsFrameWidth, BreakpointsBackground, BreakpointsForeground
        }
    }
    #endregion


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BaseProcess
    ///
    /// \brief A base process.
    ///
    /// \par Description.
    ///      When working there are the following phases
    ///      -#  Init    - create a new network
    ///      -#  Design  - Design the network and fill it's parameters
    ///      -#  Check   - Check if the network design is leagal according to the algorithm specifications
    ///      -#  Create  - Create the network from the software point of view
    ///      -#  Run     - Run the algorithm
    ///      -#  Presentation - update the presentation while running 
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilan Hindy
    /// \date 02/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class BaseProcess : NetworkElement
    {
        #region  /// \name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum TerminationStatus
        ///
        /// \brief Values that represent termination status.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum TerminationStatuses { NotTerminated, WaitingForNeighbours, WaitingForSelf }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectingMethod
        ///
        /// \brief Values that represent selecting methods.
        ///        This enum is used to inform the Send message how to select the targets for the message
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectingMethod { All, Include, Exclude }
        #endregion
        #region /// \name Member Variables

        /// \brief The process thread.
        public Thread ProcessThread;

        /// \brief The listener thread - the thread that listen for new TCP connections.
        public Thread ListenerThread;

        /// \brief The incomming channels.
        public List<BaseChannel> IncommingChannels = new List<BaseChannel>();

        /// \brief The out going channels.
        public List<BaseChannel> OutGoingChannels = new List<BaseChannel>();

        /// \brief True to stop flag. This flag is used to signal the listen thread to terminate
        public bool StopFlag = false;

        /// \brief The breakpoint event. This event is set when a breakpoint should stop the processing
        public AutoResetEvent BreakpointEvent = new AutoResetEvent(false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief The messages for init in debug mode. This variable is used in initiating debug mode
        ///        It includes all the messages that where in the queue when the last operation was stopped.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<BaseMessage> MessagesForInitInDebugeMode = new List<BaseMessage>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief True to waiting for breakpoint event. This method is used to signal to the GUI that the
        ///        Process is waiting for breakpoints to be released.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool WaitingForBreakpointEvent;

        /// \brief True if breakpoints where updated.
        public bool BreakpointsWhereUpdated;

        /// \brief The message in process. The current message that is processed
        public BaseMessage MessageInProcess = new BaseMessage();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief The terminate method lock. A lock for the Terminate method because many (All the reading threads)
        ///        may access it concurrently.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private object TerminateMethodLock = new object();

        /// \brief True to enable in debug mode, false to disable it.
        private bool InDebugMode;

        /// \brief The true breakpoints.The breakpoints that where found true and stopped the processing
        private List<Breakpoint> trueBreakpoints = new List<Breakpoint>();
        
        #endregion
        #region /// \name Attributes

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public List<BaseChannel> InChannels
        ///
        /// \brief Gets the in channels.
        ///
        /// \return The in channels.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<BaseChannel> InChannels
        {
            get
            {
                return IncommingChannels.Where(c => c.ea[bc.eak.SourceProcess] != c.ea[bc.eak.DestProcess]).ToList();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public List<BaseChannel> OutChannels
        ///
        /// \brief Gets the out channels.
        ///
        /// \return The out channels.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<BaseChannel> OutChannels
        {
            get
            {
                return OutGoingChannels.Where(c => c.ea[bc.eak.SourceProcess] != c.ea[bc.eak.DestProcess]).ToList();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public List<int> InProcessesIds
        ///
        /// \brief Gets a list of identifiers of the in processes.
        ///
        /// \return A list of identifiers of the in processes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<int> InProcessesIds
        {
            get
            {
                return InChannels.Select(c => (int)c.ea[bc.eak.SourceProcess]).ToList();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public List<int> OutProcessesIds
        ///
        /// \brief Gets a list of identifiers of the out processes.
        ///
        /// \return A list of identifiers of the out processes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<int> OutProcessesIds
        {
            get
            {
                return OutChannels.Select(c => (int)c.ea[bc.eak.DestProcess]).ToList();
            }
        }
        #endregion
        #region  /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseProcess() : base(true)
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

        public BaseProcess() : base(true)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseProcess(BaseNetwork network) : base(network)
        ///
        /// \brief Constructor.
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
        ///
        /// \param network  (BaseNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseProcess(BaseNetwork network) : base(network)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseProcess(bool permissionsValue) : base(permissionsValue)
        ///
        /// \brief Constructor.
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
        ///
        /// \param permissionsValue  (bool) - true to permissions value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseProcess(bool permissionsValue) : base(permissionsValue)
        {
            
        }

        #endregion
        #region  /// \name Init (methods that are activated while in Init phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private Attribute CreateTestListOfAttributes()
        ///
        /// \brief Creates test list of attributes.
        ///
        /// \par Description.
        ///      This method is used in order to generate a list attribute that will be found
        ///      In the BaseProcess only (Not in the inherited classes) for testing purposes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \return The new test list of attributes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Attribute CreateTestListOfAttributes()
        {
            AttributeList listOfAttributes = new AttributeList();
            listOfAttributes.Add(new Attribute { Value = 1 });
            listOfAttributes.Add(new Attribute { Value = "A string" });
            BaseMessage message = new BaseMessage(network, bm.MessageTypes.Forewared, 1, 2, 3, 4, "TestMessage", 5, 6);
            listOfAttributes.Add(new Attribute { Value = message });
            listOfAttributes.Add(new Attribute { Value = message });
            return new Attribute { Value = listOfAttributes };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override sealed void InitElementAttributes(int idx)
        ///
        /// \brief Init element attributes.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param idx  (int) - The index.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override sealed void InitElementAttributes(int idx)
        {
            base.InitElementAttributes(idx);
            ea.Add(bp.eak.Name, new Attribute { Value = GetProcessDefaultName(), Changed = false });
            ea.Add(bp.eak.Initiator, new Attribute { Value = false, Editable = true, Changed = false, EndInputOperation = CheckIfThereIsAnotherInitiator });
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
            op.Add(bp.opk.Breakpoints, new Attribute { Value = new Breakpoint(Breakpoint.HostingElementTypes.Process), Editable = false, Changed = false, IncludedInShortDescription = false });
            op.Add(bp.opk.BaseAlgorithm, new Attribute { Value = new BaseAlgorithmHandler(), Changed = false, ElementWindowPrmsMethod = BaseAlgorithmHandler.BaseAlgorithmPrms });
            op.Add(bp.opk.InternalEvents, new Attribute { Value = new InternalEventsHandler(), Changed = false, ElementWindowPrmsMethod = InternalEventsHandler.InternalEventPrms });
            op.Add(bp.opk.ChangeOrderEvents, new Attribute { Value = new ChangeMessageOrder(), Editable = false, Changed = false});
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override sealed void InitPresentationParameters()
        ///
        /// \brief Init presentation parameters.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override sealed void InitPresentationParameters()
        {
            base.InitPresentationParameters();
            pp.Add(bp.ppk.FrameColor, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bp.ppk.FrameWidth, new Attribute { Value = 100, Changed = false });
            pp.Add(bp.ppk.FrameHeight, new Attribute { Value = 100, Changed = false });
            pp.Add(bp.ppk.FrameLeft, new Attribute { Value = 100, Changed = false });
            pp.Add(bp.ppk.FrameTop, new Attribute { Value = 100, Changed = false });
            pp.Add(bp.ppk.FrameLineWidth, new Attribute { Value = 2, Changed = false });
            pp.Add(bp.ppk.Background, new Attribute { Value = KnownColor.White, Changed = false });
            pp.Add(bp.ppk.Foreground, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bp.ppk.Text, new Attribute { Value = ToString(), Changed = false });
            pp.Add(bp.ppk.BreakpointsFrameColor, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bp.ppk.BreakpointsFrameWidth, new Attribute { Value = 1, Changed = false });
            pp.Add(bp.ppk.BreakpointsBackground, new Attribute { Value = KnownColor.White, Changed = false });
            pp.Add(bp.ppk.BreakpointsForeground, new Attribute { Value = KnownColor.Black });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitOperationResults()
        ///
        /// \brief Init operation results.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitOperationResults()
        {
            base.InitOperationResults();
            or.Add(bp.ork.Round, new Attribute { Value = 0, Editable = false, Changed = false});
            or.Add(bp.ork.TerminationStatus, new Attribute { Value = TerminationStatuses.NotTerminated, Editable = false, Changed = false, IncludedInShortDescription = false });
            or.Add(bp.ork.MessageQ, new Attribute { Value = new MessageQ(), Editable = false, Changed = false, IncludedInShortDescription = false });
            or.Add(bp.ork.ReceivePort, new Attribute { Value = 0, Editable = false, Changed = false, IncludedInShortDescription = false });
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
            InitBaseAlgorithm();
            InitInternalEvents();
        }
        #endregion
        #region  /// \name Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckIfThereIsAnotherInitiator(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Determine if there is another initiator.
        ///
        /// \par Description.
        ///      When (on edit process window) the initializer parameter is set to true,
        ///      this method is activated to check if the network is centralized and there is another initiator
        ///      (In centralized network there can be only one initiator)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
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

        public static bool CheckIfThereIsAnotherInitiator(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";
            if (newValue == "False")
            {
                return true;
            }
            if (network.ea[bn.eak.Centrilized] == true)
            {
                List<BaseProcess> initiators = network.Processes.Where(process => process.ea[bp.eak.Initiator] == true).ToList();
                if (initiators.Count > 0)
                {
                    errorMessage = "The network allows only one initiator (see the network Centalized attribute) \n" +
                        "the folowing processes are already marked as initiators :";
                    foreach (BaseProcess process in initiators)
                    {
                        errorMessage += process.ea[ne.eak.Id].ToString() + " ";
                    }
                    return false;
                }
            }
            return true;
        }
        #endregion
        #region  /// \name Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///
        /// \par Description.
        ///      -  This method is activated when the Check operation is pressed from the MainWindow
        ///      -  The following are the checks done:
        ///      -# The process has to have a channel to itself.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            if (!network.Channels.Any(c => c.ea[bc.eak.SourceProcess] == ea[ne.eak.Id] &&
                c.ea[bc.eak.DestProcess] == ea[ne.eak.Id]))
            {
                BuildCorrectionParameters newCorrection = new BuildCorrectionParameters();
                newCorrection.ErrorMessage = "The Process does not have a channel to itself";
                newCorrection.CorrectionMethod = AddSelfConnectingChannel;
                newCorrection.CorrectionParameters = network;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void AddSelfConnectingChannel(object parameters)
        ///
        /// \brief Adds a self connecting channel.
        ///
        /// \par Description.
        ///      This method is activated when the Check is pressed from MainWindow and
        ///      an error that there is no self connecting channel was detected
        ///
        /// \par Algorithm.
        ///      Create a self connecting channel.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param parameters  (object) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void AddSelfConnectingChannel(object parameters)
        {
            int channelId = network.Channels.Last().ea[ne.eak.Id] + 1;
            network.Channels.Add(new BaseChannel(network, channelId, ea[ne.eak.Id], ea[ne.eak.Id]));
        }
        #endregion
        #region  /// \name Run (methods that are activated while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Activate(bool inDebugMode)
        ///
        /// \brief Activates the process thread.
        ///
        /// \par Description.
        ///      The method that genarate the process running thread.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param inDebugMode  (bool) - true to enable in debug mode, false to disable it.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Activate(bool inDebugMode)
        {
            //Initialize the main thread of the object from Network.Activate()
            InDebugMode = inDebugMode;
            ProcessThread = new Thread(RunAlgorithmEnvelope);

            ProcessThread.Name = ea[bp.eak.Name];

            // Start the worker thread.
            ProcessThread.Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void RunAlgorithmEnvelope()
        ///
        /// \brief Executes the algorithm envelope operation.
        ///
        /// \par Description.
        ///      This method is activated at the beginning of the running
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RunAlgorithmEnvelope()
        {
            // Init the message queue (the message parameter is for comaptibility and not realy inserted
            // to the queue)
            BaseMessage message = new BaseMessage(network);
            MessageQHandling(ref message, MessageQOperation.Init);

            // Create the listen thread to listen for new connections
            CreateListenThread();

            if (InDebugMode)
            {
                // Sends the messages that where sent from this processor in the previouse
                // activation and where waiting in the dest's Q when the running was stopped
                SendInitDebugMessages();
            }
            else
            {
                // If the process is initiator activate the RunAlgorithm method
                // to initiate the algorithm
                if (ea[bp.eak.Initiator] == true)
                {
                    // There are 3 ways to start the running:
                    // RunToEnd - Run the algorithm untill it ends or reached a breakpoint
                    // SingleStepAll - Activate the initiators
                    // InitializeRunning - Wait for the mouse press to activate the initiators
                    // In order to pause the network a breakpoint is used
                    // The value of the breakpoint is true for the first 2 initiation methods
                    // The value of the breakpoint is false for the thired initiation method
                    // So only in the thired initiation method the running will halt before activating the initiator
                    WaitingForBreakpointEvent = true;
                    BreakpointEvent.WaitOne();
                    WaitingForBreakpointEvent = false;
                    ProcessEvents(EventTriggerType.Initialize, null);
                    RunAlgorithm();
                }
            }

            // All the processes (The initiators after the initialization and the rest of the processes
            // at the beginning) enter the message receive loop
            ReceiveHandlingEnvelope();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SendInitDebugMessages()
        ///
        /// \brief When starting debug mode send the messages from this processor
        ///        that where waiting in the Q of the target processor but where
        ///        not processed yet
        ///
        /// \par Description.
        ///      -  In the beginning of the debug, the messages that where in the Q are taken
        ///         and given to the source processor to send them.
        ///      -  The taking of the messages from the dest's Q to the source processor 
        ///         is done in BaseNetwork.CreateDebug
        ///      -  This method sends all the messages in order to recover the dest's message Q.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SendInitDebugMessages()
        {
            Logger.Log(Logger.LogMode.MainLogAndProcessLogAndMessageTrace, GetProcessDefaultName(), "Process.RunDebug()", "Entered RunDebug", "", "RunningWindow");
            if (MessagesForInitInDebugeMode.Count > 0)
            {
                foreach (BaseMessage message in MessagesForInitInDebugeMode)
                {
                    if (!message.IsEmpty())
                    {
                        Send(message.GetHeaderField(bm.pak.DestProcess), message);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ReceiveHandlingEnvelope()
        ///
        /// \brief Receive handling envelope.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ReceiveHandlingEnvelope()
        {
            while (true)
            {
                // Retrieve the first message from the Queue
                // This method will return only when there is a message in the queue 
                MessageQHandling(ref MessageInProcess, MessageQOperation.RetrieveFirstMessage);

                // Breakpoint handeling
                // The following parameter is set when the breakpoints are addedqupdated from the gui 
                BreakpointsWhereUpdated = true;
                while (BreakpointsWhereUpdated)
                {
                    BreakpointsWhereUpdated = false;

                    // Evaluate the breakpoints assigned to the :
                    // Network
                    // This process
                    // The process channels
                    // The message
                    trueBreakpoints = EvaluateBreakpoints(MessageInProcess);
                    WaitingForBreakpointEvent = trueBreakpoints.Count > 0;

                    // The following method will cause the RunningHandler to check if a processing step 
                    // was finished. The method is sent from here because this is the place that
                    // the process thread finishes a single step
                    MessageRouter.CheckFinishProcessingStep(this, new object[] { });

                    if (WaitingForBreakpointEvent)
                    {
                        // hold processing untill the event will be set from the RunningHandler
                        BreakpointEvent.Reset();
                    }
                    else
                    {
                        // continue processing
                        BreakpointEvent.Set();
                    }
                    BreakpointEvent.WaitOne();
                    WaitingForBreakpointEvent = false;
                }

                // Reset the list of breakpoints that returned true
                trueBreakpoints = new List<Breakpoint>();

                // An empty message is used when forcing a termination 
                // and the message in processes needs to be thrown
                // An empty message is used also when the message processing condition is false
                if (MessageInProcess.IsEmpty())
                {
                    continue;
                }
                else
                {
                    MessageQHandling(ref MessageInProcess, MessageQOperation.RemoveFirstMessage);
                }

                //A "Terminate" message is used to close the thared and the scket
                if (TypesUtility.CompareDynamics(MessageInProcess.GetHeaderField(bm.pak.MessageType), bm.MessageTypes.Terminate))
                {

                    // The method Terminate returns true if the process finished the termination algorithm and should stop
                    if (Terminate(MessageInProcess))
                    {
                        MessageRouter.CheckFinishProcessingStep(this, new object[] { });
                        return;
                    }
                }
                else
                {
                    // Activate the ReceiveHandling method of the process (Activate the process algorithm)
                    // See if there is a need to send Base algorithm message 
                    ProcessEvents(EventTriggerType.BeforeReceiveMessage, MessageInProcess);

                    // This is the method that the user's algorithm implement
                    ReportText("Processing : " + MessageInProcess.ShortDescription());
                    ReceiveHandling(MessageInProcess);

                    // See if there is a need to send Base Algorithm message
                    ProcessEvents(EventTriggerType.AfterReceiveMessage, MessageInProcess);

                    // Check if a processing step finished
                    MessageRouter.CheckFinishProcessingStep(this, new object[] { });
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool IsAlive()
        ///
        /// \brief Query if this object is alive.
        ///
        /// \par Description.
        ///      -  A process is alive when it's main listen thread is alive and at least one of
        ///         his reader threads is alive
        ///      -  This method is used by the Network to check if the process is alived if none
        ///         of the processes is alive the Network knows that it terminated
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \return True if alive, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsAlive()
        {
            foreach (BaseChannel channel in OutGoingChannels)
            {
                if (channel.asynchronousReader != null)
                {
                    if (channel.asynchronousReader.ReadThread.IsAlive)
                    {
                        return true;
                    }
                }
            }

            if (ListenerThread.IsAlive)
            {
                return true;
            }
            return ProcessThread.IsAlive;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<Breakpoint> EvaluateBreakpoints(BaseMessage message)
        ///
        /// \brief Evaluate breakpoints.
        ///
        /// \par Description.
        ///      This method is activated at the beginning of each iteration to check if there are
        ///      true breakpoints
        ///      The following NetworkElements's breakpoints are checked:
        ///      -#   The network
        ///      -#   The process
        ///      -#   The channel that the message came from
        ///      -#   The message.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param message  (BaseMessage) - The message.
        ///
        /// \return A List&lt;Breakpoint&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<Breakpoint> EvaluateBreakpoints(BaseMessage message)
        {
            BaseChannel channel = IncommingChannels.Find(c => c.ea[bc.eak.SourceProcess] == message.GetHeaderField(bm.pak.SourceProcess));
            List<Breakpoint> breakpoints = new List<Breakpoint>();
            if (network.op[bn.opk.Breakpoints].Evaluate(Breakpoint.EvaluationMode.Running, network, this, channel, message))
            {
                breakpoints.Add(network.op[bn.opk.Breakpoints]);
            }

            if (op[bp.opk.Breakpoints].Evaluate(Breakpoint.EvaluationMode.Running, network, this, channel, message))
            {
                breakpoints.Add(ea[bp.eak.Breakpoints]);
            }

            if (channel.op[bc.opk.Breakpoints].Evaluate(Breakpoint.EvaluationMode.Running, network, this, channel, message))
            {
                breakpoints.Add(channel.op[bc.opk.Breakpoints]);
            }

            if (!MessageInProcess.IsEmpty())
            {
                if (message.op[bm.opk.Breakpoints].Evaluate(Breakpoint.EvaluationMode.Running, network, this, channel, message))
                {
                    breakpoints.Add(message.op[bm.opk.Breakpoints]);
                }
            }
            return breakpoints;
        }


        #endregion
        #region  /// \name The implementation of the algorithm of the base namespace

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void RunAlgorithm()
        ///
        /// \brief The activation of the initializer in the Base algorithm
        ///
        /// \par Description.
        ///      -  This method is a virtual method  
        ///      -  It is activated when the initiator has to start the algorithm  
        ///      -  Each algorithm will have to overload this method with it's algorithm initiator's initiation  
        ///      -  This method implements the initiaton of the base algorithm
        ///
        /// \par Algorithm.
        ///      If you are the process with minProcessId (Used to detect initiator)
        ///      Send a message to the process with the maxProcessId
        ///
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void RunAlgorithm()
        {
            int minProcessId = network.Processes.Min(p => p.ea[ne.eak.Id]);
            int maxProcessId = network.Processes.Max(p => p.ea[ne.eak.Id]);
            if (ea[ne.eak.Id] == minProcessId)
            {
                BaseChannel channel = OutGoingChannels.First(c => c.ea[bc.eak.DestProcess] == maxProcessId);
                BaseMessage message = new BaseMessage(network, bm.MessageTypes.Forewared, channel, "", 0, 0);
                Send(maxProcessId, message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn virtual public void ReceiveHandling(BaseMessage message)
        ///
        /// \brief Receive handling.
        ///
        /// \par Description.
        ///      -  This method is a virtual method  
        ///      -  It is activated when a message arrives to the process  
        ///      -  Each algorithm will have to overload this method with it's message handeling
        ///      -  This method implements the message handeling of the base algorithm.  
        ///
        /// \par Algorithm.
        ///      The algorithm implemented is :
        ///      -#   Untill round 2 ended:
        ///           -#  Send a message from the initiator (minProcessId) to the process with maxProcessId
        ///           -#  Send a message from the maxProcessId to the minProcessId
        ///      -# Terminate the network.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        virtual public void ReceiveHandling(BaseMessage message)
        {
            int minProcessId = network.Processes.Min(process => process.ea[ne.eak.Id]);
            int maxProcessId = network.Processes.Max(process => process.ea[ne.eak.Id]);

            // If the message is not Base message throw it
            dynamic messageType = message.GetHeaderField(bm.pak.MessageType);
            if (!(TypesUtility.CompareDynamics(messageType, bm.MessageTypes.Forewared) ||
                TypesUtility.CompareDynamics(messageType, bm.MessageTypes.LastMessage)))
            {
                return;
            }

            // If the process is the process with the maxProcessId (used to indicate the non-initiator)
            if (ea[ne.eak.Id] == maxProcessId)
            {
                // Get the round of the message
                int messageCounter = message.GetHeaderField(bm.pak.Round);

                // Get the outgoing channel toword the process with of the minProcessId
                BaseChannel channel = OutGoingChannels.First(c => c.ea[bc.eak.DestProcess] == minProcessId);

                // If this is round 2 - Terminate
                if (messageCounter == 5)
                {
                    BaseMessage newMessage = new BaseMessage(network,
                        bm.MessageTypes.LastMessage,
                        channel, "",
                        messageCounter,
                        minProcessId);
                    Send(0, newMessage);
                    Terminate();
                }

                // If this is not the end of round 2 - start a new round
                else
                {
                    messageCounter++;
                    BaseMessage newMessage = new BaseMessage(network, bm.MessageTypes.Forewared, channel, "", messageCounter, minProcessId);
                    Send(minProcessId, newMessage);
                }
            }

            // Process with minProcessId
            else
            {

                // If a Terminate message arrived - Terminate()
                if (TypesUtility.CompareDynamics(message.GetHeaderField(bm.pak.MessageType),bm.MessageTypes.LastMessage))
                {
                    // The termination will be recieved by all the processes in the network
                    Terminate();
                }

                // If an algorithm message arrived - send it back
                else
                {
                    BaseChannel channel = OutGoingChannels.First(c => c.ea[bc.eak.DestProcess] == maxProcessId);
                    BaseMessage newMessage = new BaseMessage(network,
                        bm.MessageTypes.Forewared,
                        channel,
                        message.GetField(bm.ork.Name),
                        message.GetHeaderField(bm.pak.Round), maxProcessId);
                    Send(maxProcessId, newMessage);
                }
            }
        }

        #endregion
        #region /// \name MessageQ handling

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void MessageQHandling(ref BaseMessage message, MessageQOperation operation)
        ///
        /// \brief Message queue handling.
        ///
        /// \par Description.
        ///      This method is an envelope for all the message queue operations
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ///
        /// \param [in,out] message   (ref BaseMessage) - The message.
        /// \param          operation  (MessageQOperation) - The operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MessageQHandling(ref BaseMessage message, MessageQOperation operation)
        {
            or[bp.ork.MessageQ].MessageQHandling(ref message, operation);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void ArrangeMessageQ(MessageQ messageQueue)
        ///
        /// \brief Arrange message queue.
        ///
        /// \par Description.
        ///      -  This method is used by the algorithm programmer to change the order of messages
        ///         grammatically.
        ///      -  The method is called before each retrieve of a message from the queue.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ///
        /// \param messageQueue  (MessageQ) - Queue of messages.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void ArrangeMessageQ(MessageQ messageQueue)
        {
            return;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual bool MessageProcessingCondition(BaseMessage message)
        ///
        /// \brief Message processing condition.
        ///
        /// \par Description.
        ///      -  This method is used by the algorithm programmer to block/unblock processing 
        ///         of the next message in the MessageQ
        ///      -  The method is called before each retrieve of a message from the queue
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ///
        /// \param message  (BaseMessage) - The message.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual bool MessageProcessingCondition(BaseMessage message)
        {
            return true;
        }
        #endregion
        #region /// \name Event Handling
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ProcessEvents(EventTriggerType trigger, BaseMessage message)
        ///
        /// \brief Process the events.
        ///
        /// \par Description.
        ///      Process all the events for this event trigger:
        ///      -  There are 2 kinds of events  
        ///         -#  Base algorithm events which are event for simulating the base algorithms
        ///             (The results of these events are sending of messages)
        ///         -#  Internal Events which can be any action which is implemented by methods
        ///     -   This method is activated before and after each send or receive of a message  
        ///     -   It checks the events and if there is an event to process it process it
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/01/2018
        ///
        /// \param trigger  (EventTrigger) - The trigger.
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ProcessEvents(EventTriggerType trigger, BaseMessage message)
        {
            op[bp.opk.BaseAlgorithm].ProcessEvents(trigger, message);
            op[bp.opk.InternalEvents].ProcessEvents(trigger, message);
        }
        #region /// \name Internal Events

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitInternalEvents()
        ///
        /// \brief Init internal events.
        ///
        /// \par Description.
        ///      -  if the programmer will want to add internal events programmatically he will do it in this method  
        ///      -  This method is activated in the init phase
        ///    
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The construct of this method body is:
        ///      
        ///~~~{.cs}
        /// //Example for adding internal events in this method
        /// protected virtual void InitInternalEvents()
        /// {
        ///     // For each event create a call like :
        ///    InsertInternalEvent(
        ///        0,                                    // The id of the process that will activate the event
        ///        EventTriggerType.AfterSendMessage,    // Trigger (The action the process is doing)
        ///        0,                                    // Round (the round of the message)
        ///        bm.MessageTypes.Forewared,            // Message type (can be any message type)
        ///        1,                                    // The process which is the other end of the send/receive of the trigger
        ///        new List<InternalEvents.InternalEventDelegate> { SomeMethod });    // The name of the method that processes the event
        /// }
        ///~~~
        ///      
        ///
        /// \author Ilanh
        /// \date 14/01/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitInternalEvents()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void InsertInternalEvent( int ActivatorProcessId, EventTriggerType eventTrigger, int eventRound, dynamic eventMessageType, int eventMessageOtherEnd, List<InternalEvents.InternalEventDelegate> methods)
        ///
        /// \brief Inserts an internal event.
        ///
        /// \par Description.
        ///      This method is used by the InitInternalEvents to insert events.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/01/2018
        ///
        /// \param ActivatorProcessId    (int) - Identifier for the activator process.
        /// \param eventTrigger          (EventTriggerType) - The event trigger.
        /// \param eventRound            (int) - The event round.
        /// \param eventMessageType      (dynamic) - Type of the event message.
        /// \param eventMessageOtherEnd  (int) - The event message other end.
        /// \param methods               (InternalEventDelegate>) - The methods.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void InsertInternalEvent(
            int ActivatorProcessId,
            EventTriggerType eventTrigger,
            int eventRound,
            dynamic eventMessageType,
            int eventMessageOtherEnd,
            List<InternalEvents.InternalEventDelegate> methods)
        {
            if (ActivatorProcessId == ea[ne.eak.Id])
            {
                ((InternalEventsHandler)op[bp.opk.InternalEvents]).CreateEvent(eventTrigger,
                    eventRound,
                    eventMessageType,
                    eventMessageOtherEnd,
                    methods);
            }
        }
        #endregion
        #region /// \name Base Algorithm Events

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitBaseAlgorithm()
        ///
        /// \brief Init base algorithm data.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The implementation of this method is :
        ///      
        /// ~~~{.cs}
        /// // Example for adding base algorithm message in this method
        /// protected virtual void InitBaseAlgorithmData()
        /// {
        ///      // For each event you want to create do steps 1-3
        ///      //1. Create BaseAlgorithmMessages object
        ///      BaseAlgorithmMessages messages = new BaseAlgorithmMessages();
        ///      
        ///      //2. Foreach message you want to be sent in the event call AddMessage of the object
        ///      messages.AddMessage(bm.MessageTypes.NullMessageType,  // The type of the message to be sent
        ///                "BaseMessages",                             // The name of the message to be sent
        ///                new AttributeDictionary(),                  // The fields of the message to be sent
        ///                new AttributeList { 1 });                   // The id of the target processor of the message
        ///                
        ///      //3.Call InsertBaseAlgorithmEvent 
        ///      InsertBaseAlgorithmEvent(
        ///             // The id of the process that will activate the event
        ///             0,                                      
        ///             
        ///             // Define the trigger which describe condition for sending the messages
        ///             EventTriggerType.AfterSendMessage,    // Trigger (The action the process is doing)
        ///             0,                                    // Round (the round of the message)
        ///             bm.MessageTypes.Forewared,            // Message type (can be any message type)
        ///             1,                                    // The process which is the other end of the send/receive of the trigger
        ///             
        ///             // The messages that will be sent when the trigger hits
        ///             messages)     
        /// ~~~
        ///
        /// \author Ilanh
        /// \date 20/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitBaseAlgorithm()
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void InsertBaseAlgorithmEvent(int ActivatorProcessId, EventTriggerType eventTrigger, int eventRound, dynamic eventMessageType, int eventMessageOtherEnd, BaseAlgorithmMessages baseAlgorithmMessages)
        ///
        /// \brief Inserts a base message data.
        ///
        /// \par Description.
        ///      -  The base algorithm simulates the algorithm that the network is
        ///          working on.
        ///      -  The implementation of the base algorithm is composed on events that happen in the
        ///         process
        ///      -  The data of the events and the message that they send is found in :
        ///~~~{.cs}
        ///op[bp.opk.BaseAlgorithm]
        ///~~~
        ///
        /// \par 
        ///      Each time there is a send or receive event in the processor the list is checked and if the
        ///      event conditions are fulfilled the message in the data is sent.
        ///
        /// \par Algorithm.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \par Usage Notes.
        ///      This method is for use from the Process or it's inherited classes.
        ///
        /// \param ActivatorProcessId     (int) - Identifier for the activator process.
        /// \param eventTrigger           (EventTriggerType) - The event trigger.
        /// \param eventRound             (int) - The event round.
        /// \param eventMessageType       (dynamic) - Type of the event message.
        /// \param eventMessageOtherEnd   (int) - The event message other end.
        /// \param baseAlgorithmMessages  (BaseAlgorithmMessages) - The base algorithm messages.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void InsertBaseAlgorithmEvent(int ActivatorProcessId,
            EventTriggerType eventTrigger,
            int eventRound,
            dynamic eventMessageType,
            int eventMessageOtherEnd,
            BaseAlgorithmMessages baseAlgorithmMessages)
            
        {
            if (ActivatorProcessId == ea[ne.eak.Id])
            {
                ((BaseAlgorithmHandler)op[bp.opk.BaseAlgorithm]).CreateEvent(eventTrigger,
                    eventRound,
                    eventMessageType,
                    eventMessageOtherEnd,
                   baseAlgorithmMessages);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual BaseMessage BuildBaseAlgorithmMessage(dynamic messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        ///
        /// \brief Builds base message.
        ///
        /// \par Description.
        ///      -  This method is called in execution time.  
        ///      -  The purpose of this method is to allow the algorithm to add fields to the Base Algorithm message  
        ///      -  The following is the process of sending a base algorithm message  
        ///         -#  During running time before/after a send/receive message the event is checked
        ///         -#  If the conditions are filled up this method is called with the base message data
        ///         -#  This method can add fields to the Base Message according to the status of the algorithm
        ///         -#  The base algorithm message is sent  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param messageType   (dynamic) - Type of the message.
        /// \param messageName   (string) - Name of the message.
        /// \param messageFields (AttributeDictionary) - The message fields.
        /// \param round         (int) - The round.
        /// \param targets       (List&lt;Attribute&gt;) - The targets.
        ///
        /// \return A BaseMessage.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual BaseMessage BuildBaseAlgorithmMessage(dynamic messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        {
            BaseMessage message = new BaseMessage(network, messageType, messageFields, new BaseChannel(), messageName, round);
            return message;
        }
        #endregion
        #endregion       
        #region  /// \name Running Termination of the network

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Terminate(BaseMessage message = null)
        ///
        /// \brief Terminates the running of the algorithm.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///      The following is the termination process:
        ///      -# Step 1 (TerminationStatus = NotTerminated )
        ///         -# Triggers to move to this step : Received Terminate message, or an activation of the Terminate method
        ///         -# At this stage the following is performed:
        ///             -#   Send terminate to all the outgoing channels except for self connecting channel
        ///                  (When the receive thread of the destination will receive the message it will terminate)
        ///      -# Step 2 (TerminationStatus = WaitingForNeighbours)
        ///         -#  Trigger to move to this step : The previous step actions were performed
        ///         -#  At this step : Receiving Terminate from all the incoming channels except the self connecting channel
        ///             When this occur the reading thread from the sources terminates
        ///      -# Step 3 (TerminationStatus = WaitingForSelf)
        ///         -#  Triggers to move to this step : Revived Terminate from all the incoming channel
        ///         -#  At this step  Sending Terminate to self and set the StopFlag in order that the listener will terminate
        ///      -# Step 4 (End)
        ///         -# Trigger to move to this step : A message arrived from self
        ///         -# Return true to signal that the process terminated.
        ///
        /// \par Usage Notes.
        ///      Note that this method can be activated from one process or several processes
        ///      the result is the same - termination of all the processes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param message (Optional)  (BaseMessage) - The message.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Terminate(BaseMessage message = null)
        {
            bool result;
            TerminationStatuses terminationStatus = or[bp.ork.TerminationStatus];
            string outgoingChannelsDestinations = "";
            OutGoingChannels.ForEach(delegate (BaseChannel channel) { outgoingChannelsDestinations += channel.ea[bc.eak.DestProcess].ToString() + ","; });
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, ToString(), "", "OutGoingChannelsDestinations = " + outgoingChannelsDestinations + "TerminationStatus = " + TypesUtility.GetKeyToString(terminationStatus), message, "RunningWindow");
            switch (terminationStatus)
            {
                // If this is the first activation of the Terminate method (by direct calling 
                // or by Terminate message received)
                case TerminationStatuses.NotTerminated:

                    // Send Terminate to all neighbors
                    foreach (BaseChannel channel in OutChannels)
                    {
                        if (channel.ea[bc.eak.DestProcess] !=
                            ea[ne.eak.Id])
                        {
                            SendTerminatMessage(channel);
                        }
                    }

                    // If there are no incoming channel that where not terminated
                    //  Terminate the listening thread (by setting the StopFlag)
                    //  and the self connecting thread (by sending Terminate through the channel)
                    //  Transfer to stage waiting to self
                    if (!NonTerminateChannelsExist(message))
                    {
                        StopFlag = true;
                        BaseChannel selfConnectingChannel = OutGoingChannels.First(channel => channel.ea[bc.eak.DestProcess] ==
                                ea[ne.eak.Id]);
                        SendTerminatMessage(selfConnectingChannel);
                        or[bp.ork.TerminationStatus] = TerminationStatuses.WaitingForSelf;
                    }

                    // If there are incoming channels that where not terminated - wait for them
                    else
                    {
                        or[bp.ork.TerminationStatus] = TerminationStatuses.WaitingForNeighbours;
                    }
                    result = false;
                    break;

                // If waiting for Terminate messages from neighbors
                case TerminationStatuses.WaitingForNeighbours:

                    // If there are no incoming channel that where not terminated
                    //  Terminate the listening thread (by setting the StopFlag)
                    //  and the self connecting thread (by sending Terminate through the channel)
                    //  Transfer to stage waiting to self
                    if (!NonTerminateChannelsExist(message))
                    {
                        StopFlag = true;
                        BaseChannel selfConnectingChannel = OutGoingChannels.First(channel => channel.ea[bc.eak.DestProcess] ==
                                ea[ne.eak.Id]);
                        {
                            SendTerminatMessage(selfConnectingChannel);
                        }
                        or[bp.ork.TerminationStatus] = TerminationStatuses.WaitingForSelf;
                    }
                    result = false;
                    break;

                // If the status is waiting to self and a Terminate message was arrived that means
                // that this message arrived from self and the termination process ended
                case TerminationStatuses.WaitingForSelf:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, ToString(), "Terminate()", "At the end of the method", "TerminationStatus = " + TypesUtility.GetKeyToString(or[bp.ork.TerminationStatus]), "RunningWindow");
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SendTerminatMessage(BaseChannel channel)
        ///
        /// \brief Sends a terminate message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param channel The channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SendTerminatMessage(BaseChannel channel)
        {
            BaseMessage message = new BaseMessage(network, bm.MessageTypes.Terminate, channel, "", 0, 0);
            Send(channel.ea[bc.eak.DestProcess], message);
            channel.Terminate();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool NonTerminateChannelsExist(BaseMessage message)
        ///
        /// \brief Non terminate incoming channels exist.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param message : the last message arrived
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool NonTerminateChannelsExist(BaseMessage message)
        {
            //Mark the incoming channel of the message as terminated
            if (message != null)
            {
                BaseChannel sourceChannel = IncommingChannels.First(channel => channel.ea[bc.eak.SourceProcess] ==
                    message.GetHeaderField(bm.pak.SourceProcess));
                sourceChannel.or[bc.ork.TerminationStatus] = BaseChannel.TerminationStatuses.Terminated;
            }

            // Compare the number of terminated channel to the number of incoming channel (less the self connecting channel)
            // If it is the same that means that all the incoming channels where terminated
            int numberOfTerminatedChannels = IncommingChannels.Count(channel => channel.or[bc.ork.TerminationStatus] == BaseChannel.TerminationStatuses.Terminated);
            if (numberOfTerminatedChannels == IncommingChannels.Count - 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
        #region  /// \name Running utility methods (methods used while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn BaseChannel ChannelFrom(BaseMessage message)
        ///
        /// \brief Channel from.
        ///
        /// \par Description.
        ///      The channel of an incoming message
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 07/01/2018
        ///
        /// \param message  (BaseMessage) - The message.
        ///
        /// \return A BaseChannel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel ChannelFrom(BaseMessage message)
        {
            return ChannelFrom(message[bm.pak.SourceProcess]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn BaseChannel ChannelFrom(int id)
        ///
        /// \brief Channel from.
        ///
        /// \par Description.
        ///      The channel from a processor id
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 07/01/2018
        ///
        /// \param id  (int) - The identifier.
        ///
        /// \return A BaseChannel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel ChannelFrom(int id)
        {
            return InChannels.FirstOrDefault(c => c.ea[bc.eak.SourceProcess] == id);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn BaseChannel ChannelTo(BaseMessage message)
        ///
        /// \brief Channel to.
        ///
        /// \par Description.
        ///      A channel that is the destination of a message
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 07/01/2018
        ///
        /// \param message  (BaseMessage) - The message.
        ///
        /// \return A BaseChannel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel ChannelTo(BaseMessage message)
        {
            return ChannelTo(message[bm.pak.DestProcess]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn BaseChannel ChannelTo(int id)
        ///
        /// \brief Channel to.
        ///
        /// \par Description.
        ///      A channel towards a processor
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 07/01/2018
        ///
        /// \param id  (int) - The identifier.
        ///
        /// \return A BaseChannel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel ChannelTo(int id)
        {
            return OutChannels.FirstOrDefault(c => c.ea[bc.eak.DestProcess] == id);
        }

        #endregion
        #region  /// \name Sending methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Send(int destProcessId, BaseMessage message)
        ///
        /// \brief Send this message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param destProcessId  (int) - Identifier for the destination process.
        /// \param message        (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Send(int destProcessId, BaseMessage message)
        {
            // If a message will be sent from a process to the same process it will create a channel.sendingSocket.
            // After creating this socket all the messages will be received to the socket without passing in the
            // listener. When we will want to terminate the process the listener will not be stopped
            if (destProcessId == ea[ne.eak.Id])
            {
                if (message.FindAttribute(TypesUtility.GetKeyToString(bm.MessageTypes.Terminate)) != null)
                {
                    Logger.Log(Logger.LogMode.MainLogProcessLogAndError,
                        GetProcessDefaultName(), "Process.Send()",
                        "A process can send only Terminate message to itself",
                        "Otherwise it will not be able to terminate", "Error");
                }
            }

            foreach (BaseChannel channel in OutGoingChannels)
            {
                if (channel.ea[bc.eak.DestProcess] == destProcessId)
                {
                    Logger.Log(Logger.LogMode.MainLogAndProcessLogAndMessageTrace,
                        GetProcessDefaultName(),
                        "Process.RunAlgorithm()",
                        "Sending message : " + "to port " + channel.or[bc.ork.DestPort],
                        message,
                        "RunningWindow");

                    ProcessEvents(EventTriggerType.BeforeSendMessage, message);
                    BeforeSend(message);
                    AsynchronousSender.StartSending(message, this, channel);
                    AfterSend(message);
                    ProcessEvents(EventTriggerType.AfterSendMessage, message);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void SendToNeighbours(BaseMessage message, List<int> exclude)
        ///
        /// \brief Sends to neighbors.
        ///
        /// \par Description.
        ///      Send a message to all the neighbors
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param message  (BaseMessage) - The message.
        /// \param exclude  (List&lt;int&gt;) - The exclude.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SendToNeighbours(BaseMessage message, SelectingMethod selectingMethod, List<int> ids)
        {
            List<int> messageTargets = MessageTargets(selectingMethod, ids);

            foreach (BaseChannel channel in OutGoingChannels)
            {
                if (messageTargets.Exists(id => id == channel.ea[bc.eak.DestProcess]))
                {
                    BaseMessage newMessage = new BaseMessage(network, message, channel);
                    ProcessEvents(EventTriggerType.BeforeSendMessage, newMessage);
                    BeforeSend(newMessage);
                    AsynchronousSender.StartSending(newMessage, this, channel);
                    AfterSend(newMessage);
                    ProcessEvents(EventTriggerType.AfterSendMessage, newMessage);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SendWithNoEvents(BaseMessage message, SelectingMethod selectingMethod, List<int> ids)
        ///
        /// \brief Sends a with no events.
        ///
        /// \par Description.
        ///      -  Send the message without processing events  
        ///      -  This method is meant to be used by the event handlers to send their messages  
        ///         (If an event was processed when a message generated by event is sent it 
        ///         might cause an infinite loop)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 03/06/2018
        ///
        /// \param message          (BaseMessage) - The message.
        /// \param selectingMethod  (SelectingMethod) - The selecting method.
        /// \param ids              (List&lt;int&gt;) - The identifiers.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SendWithNoEventsProcessing(BaseMessage message, SelectingMethod selectingMethod, List<int> ids)
        {
            List<int> messageTargets = MessageTargets(selectingMethod, ids);

            foreach (BaseChannel channel in OutGoingChannels)
            {
                if (messageTargets.Exists(id => id == channel.ea[bc.eak.DestProcess]))
                {
                    BaseMessage newMessage = new BaseMessage(network, message, channel);
                    BeforeSend(newMessage);
                    AsynchronousSender.StartSending(newMessage, this, channel);
                    AfterSend(newMessage);
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Send(dynamic messageType, AttributeDictionary fields, SelectingMethod selectingMethod, List<int> ids, int round, int logicalClock)
        ///
        /// \brief Sends to neighbors.
        ///
        /// \par Description.
        ///      Build a message and send it to all the neighbors.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param messageType     (dynamic) - Type of the message.
        /// \param fields          (AttributeDictionary) - The fields.
        /// \param selectingMethod (List&lt;int&gt;) - The exclude.
        /// \param ids              (List&lt;int&gt;) - The identifiers.
        /// \param round           (int) - The round.
        /// \param logicalClock    (int) - The logical clock.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Send(dynamic messageType,
            AttributeDictionary fields,
            SelectingMethod selectingMethod,
            List<int> ids,
            int round,
            int logicalClock)
        {
            if (round == -1)
            {
                round = or[bp.ork.Round];
            }
            List<int> messageTargets = MessageTargets(selectingMethod, ids);
            // Send the messages
            foreach (BaseChannel channel in OutGoingChannels)
            {
                if (messageTargets.Any(id => channel.ea[bc.eak.DestProcess] == id))
                {
                    BaseMessage newMessage = ClassFactory.GenerateMessage(network, messageType, fields, channel, "", null, null, round, logicalClock);
                    ProcessEvents(EventTriggerType.BeforeSendMessage, newMessage);
                    BeforeSend(newMessage);
                    AsynchronousSender.StartSending(newMessage, this, channel);
                    AfterSend(newMessage);
                    ProcessEvents(EventTriggerType.AfterSendMessage, newMessage);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<int> MessageTargets(SelectingMethod selectingMethod, List<int> ids)
        ///
        /// \brief Message targets.
        ///
        /// \par Description.
        ///      Build a list of Targets according to specifications
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/04/2018
        ///
        /// \param selectingMethod  (SelectingMethod) - The selecting method.
        /// \param ids              (List&lt;int&gt;) - The identifiers.
        ///
        /// \return A List&lt;int&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<int> MessageTargets(SelectingMethod selectingMethod, List<int> ids)
        {
            // Build the list of targets
            if (ids == null)
            {
                ids = new List<int>();
            }
            List<int> messageTargets = new List<int>();
            switch (selectingMethod)
            {
                // All - All the out processes
                case SelectingMethod.All:
                    messageTargets = OutProcessesIds;
                    break;

                // Include - All the process in the parameters
                case SelectingMethod.Include:
                    messageTargets = ids;
                    break;

                // Exclude - All the processes but the ones in the parameter
                case SelectingMethod.Exclude:
                    messageTargets = OutProcessesIds.Except(ids).ToList();
                    break;
            }

            // At all cases- do not send a message to self
            messageTargets.Remove(ea[ne.eak.Id]);
            return messageTargets;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void BeforeSend(BaseMessage message)
        ///
        /// \brief Before send.
        ///
        /// \par Description.
        ///      -  This method allows the programmer to insert actions before a message is sent 
        ///      -  This is an envelope method meant to filter when the programmer will have an
        ///         ability to activate the BeforeSendOperation
        ///      -  The conditions for not activating BeforeSendOperations :
        ///         -# The message is not a Terminate message (Which is a message handled only by the system) 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 16/04/2018
        ///
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void BeforeSend(BaseMessage message)
        {
            if (!TypesUtility.CompareDynamics(message.GetHeaderField(bm.pak.MessageType), bm.MessageTypes.Terminate))
            {
                BeforeSendOperation(message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void AfterSend(BaseMessage message)
        ///
        /// \brief After send.
        ///         -   This method allows the programmer to insert actions before a message is sent
        ///         -   This is an envelope method meant to filter when the programmer will have an
        ///             ability to activate the BeforeSendOperation
        ///         -   The conditions for not activating BeforeSendOperations :
        ///             -# The message is not a Terminate message (Which is a message handled only by the system)
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 16/04/2018
        ///
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AfterSend(BaseMessage message)
        {
            if (!TypesUtility.CompareDynamics(message.GetHeaderField(bm.pak.MessageType), bm.MessageTypes.Terminate))
            {
                AfterSendOperation(message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void BeforeSendOperation(BaseMessage message)
        ///
        /// \brief Before send operation.
        ///        This method is used for algorithm specific actions before a send of a message
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 05/04/2017
        ///
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void BeforeSendOperation(BaseMessage message)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void AfterSendOperation(BaseMessage message)
        ///
        /// \brief After send operation.
        ///        This method is used for algorithm specific actions after a send of a message
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 05/04/2017
        ///
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void AfterSendOperation(BaseMessage message)
        {

        }
        #endregion
        #region  /// \name Receive communication handling


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void CreateListenThread()
        ///
        /// \brief Creates listen thread.
        ///
        /// \par Description.
        ///      -  The listen thread holds a listening socket that listen for new connections
        ///      -  When a request for a new connection is received from the TCP port a reading thread is made
        ///         to read the messages from the source
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void CreateListenThread()
        {

            //When the following flag is false the listening loop is activating 
            StopFlag = false;

            //Activating the listening for incoming connections and the read threads
            ListenerThread = new Thread(Listen);

            ListenerThread.Name = ea[bp.eak.Name] + " Listener";

            // Start the worker thread.
            ListenerThread.Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Listen()
        ///
        /// \brief Listens this object.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Listen()
        {
            AsynchronousListener.StartListening(or[bp.ork.ReceivePort], this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddAsynchronousReader(SocketStateObject state)
        ///
        /// \brief Adds the asynchronous reader.
        ///
        /// \par Description.
        ///      When the listen thread receives from the TCP a message from a source that is not yet connected
        ///      this method is activated to generate a thread that will process the reading from the source
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 02/03/2017
        ///
        /// \param state The state.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddAsynchronousReader(SocketStateObject state)
        {
            //Get the sending port
            int sourcePort = ((IPEndPoint)state.workSocket.RemoteEndPoint).Port;

            //Check which one from the incoming channel has this port
            foreach (BaseChannel channel in IncommingChannels)
            {
                if (channel.or[bc.ork.SourcePort] == sourcePort)
                {
                    channel.asynchronousReader = new AsynchronousReader(state);
                }
            }
        }



        #endregion
        #region  /// \name Presentation (methods used to update the presentation)
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override sealed void UpdateRunningStatus(object[] parameters)
        ///
        /// \brief Updates the running status described by parameters.
        ///
        /// \par Description.
        ///       - The update running status is called from the network's update running status
        ///         when the network has finished processing step, to update the presentation
        ///       - The update running status sends to the presentation the following:
        ///         -#  For each channel a list of messages in the channel
        ///         -#  For the process presentation it sends an array of parameters:
        ///             -#  Whether the process can be activated to run(If it has messages)
        ///             -#  A list of breakpoints resolved to the status of true in the last iteration
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \param parameters Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override sealed void UpdateRunningStatus(object[] parameters)
        {
            // Get the waiting message queue
            AttributeList attributeQueue = or[bp.ork.MessageQ];

            // A dictionary that will hold for each incoming channel the messages from the channel waiting for processing
            Dictionary<int, List<BaseMessage>> messagesSortedByIncommingChannels = new Dictionary<int, List<BaseMessage>>();

            // Divide the messages to the incoming channels
            foreach (Attribute attribute in attributeQueue)
            {
                BaseMessage message = attribute.Value;
                int sourceProcess = message.GetHeaderField(bm.pak.SourceProcess);
                if (!messagesSortedByIncommingChannels.ContainsKey(sourceProcess))
                {
                    messagesSortedByIncommingChannels.Add(sourceProcess, new List<BaseMessage>());
                }
                messagesSortedByIncommingChannels[sourceProcess].Add(message);
            }

            // Call Update running status of the channels with the messages lists
            foreach (BaseChannel channel in IncommingChannels)
            {
                int channelSourceProcess = channel.ea[bc.eak.SourceProcess];
                if (messagesSortedByIncommingChannels.ContainsKey(channelSourceProcess))
                {
                    channel.UpdateRunningStatus(new object[] { messagesSortedByIncommingChannels[channelSourceProcess] });
                }
                else
                {
                    channel.UpdateRunningStatus(new object[] { new List<BaseMessage>() });
                }
            }

            // Change the presentation of the process
            // This effect of this method is a set of all the presentation parameters (also the parameters
            // changed by the inherited process)
            if (or[bp.ork.MessageQ].Count > 0)
            {
                // When there are messages in the queue it is possible to activate single step on it
                MessageRouter.ReportChangePresentationOfComponent(this, new object[] { MainWindow.SelectedStatus.RunningNextProcess, trueBreakpoints });
            }
            else
            {
                MessageRouter.ReportChangePresentationOfComponent(this, new object[] { MainWindow.SelectedStatus.RunningNotNextProcess, trueBreakpoints });
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
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            return "Id:" + ea[ne.eak.Id].ToString() + " Name:" + ea[bp.eak.Name];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string PresentationText()
        ///
        /// \brief Presentation text.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string PresentationText()
        {
            return "Id:" + ea[ne.eak.Id].ToString() + " Name:" + ea[bp.eak.Name];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GetProcessDefaultName()
        ///
        /// \brief Gets process default name.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 05/03/2017
        ///
        /// \return The process default name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetProcessDefaultName()
        {
            return "Processor No." + ea[ne.eak.Id].ToString();
        }
        #endregion
        
    }
}

