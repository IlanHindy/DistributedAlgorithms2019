////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\ChandyLamport_NewStyle\ChandyLamport_NewStyleProcess.cs
///
/// \brief Implements the template process class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport_NewStyle
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ChandyLamport_NewStyleProcess
    ///
    /// \brief A template process.
    ///
    /// \par Description.
    ///      when working there are the following phases
    ///      -#  Init    - create a new network
    ///      -#  Design  - Design the network and fill it's parameters
    ///      -#  Check   - Check if the network design is legal according to the algorithm specifications
    ///      -#  Create  - Create the network from the software point of view
    ///      -#  Run     - Run the algorithm
    ///      -#  Presentation - update the presentation while running 
    ///
    /// \par Usage Notes.
    ///      The following is the way the algorithm is run 
    ///      -# For each initiator activate run algorithm
    ///      -# Until termination
    ///         -#  For each message received activate ReceiveHandeling
    /// \par
    ///      So in order to implement an algorithm one should implement:   
    ///      -# RunAlgorithm
    ///      -# ReceiveHandeling
    /// \par
    ///      The following are utility methods that can be used for processing:
    /// ~~~{.cs}
    ///     // Sending messages
    ///     // Send a message to a destination process  
    ///     protected void Send(int destProcessId, Message message)
    ///     
    ///     // Send a message to all neighbors (with except list)
    ///     protected override void SendToNeighbours(BaseMessage message, List<int> exclude)
    ///     
    ///     // Compose and send message
    ///     protected void Send(dynamic messageType,
    ///             AttributeDictionary fields,
    ///             SelectingMethod selectingMethod,
    ///             List<int> ids,
    ///             int round,
    ///             int logicalClock)
    ///     
    ///     
    ///     // Terminate the algorithm (It will cause the whole network to terminate).
    ///     // Can be activated from one process or from several processes at the same time
    ///     public bool Terminate(Message message = null)
    ///     
    ///     // Default process name
    ///     public string GetProcessDefaultName()
    ///     
    ///     
    ///     // Lists of channels and processor ids
    ///     // List of incoming channels without the self connecting channel
    ///     public List<BaseChannel> InChannels
    ///     
    ///     // List of all the processes that has channel to the processor
    ///     public List<int> InProcessesIds
    ///      
    ///     // List of all the outgoing channels without the self connecting channel
    ///     public List<BaseChannel> OutChannels
    ///     
    ///     // List of all the processes that has channel from the processor
    ///     public List<int> OutProcessesIds
    ///     
    ///     
    ///     // Get individual channels
    ///     // The channel that message came from
    ///     public BaseChannel ChannelFrom(BaseMessage message)
    ///     
    ///     // The incoming channel connected this process to process id
    ///     public BaseChannel ChannelFrom(int id)
    ///     
    ///     // The channel that message is sent to
    ///     public BaseChannel ChannelTo(BaseMessage message)
    ///
    ///     // The outgoing channel connected this process to process id
    ///     public BaseChannel ChannelTo(int id)
    ///     
    ///     // Report Text to Message Window
    ///     public ReportText(string text)
    ///          
    /// ~~~
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class ChandyLamport_NewStyleProcess : BaseProcess
    {
        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_NewStyleProcess() : base()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      It is not recommended to change the constructor. All the attributes has to be inserted
        ///      to the pa or dictionaries in the Init... methods     
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_NewStyleProcess() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_NewStyleProcess(ChandyLamport_NewStyleNetwork network) : base(network)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      It is not recommended to change the constructor. All the attributes has to be inserted
        ///      to the pa or dictionaries in the Init... methods
        ///      
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param network  (ChandyLamport_NewStyleNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_NewStyleProcess(ChandyLamport_NewStyleNetwork network) : base(network) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamport_NewStyleProcess(bool permissionsValue) : base(permissionsValue)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      It is not recommended to change the constructor. All the attributes has to be inserted
        ///      to the pa or dictionaries in the Init... methods 
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param permissionsValue  (bool) - true to permissions value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamport_NewStyleProcess(bool permissionsValue) : base(permissionsValue) { }

        #endregion
        #region /// \name Init (methods that are activated while in Init phase)

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
        /// \fn public static bool ChandyLamport_NewStyleProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        ///
        /// \brief A prototype for attribute checking.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -#  Each Attribute can be assigned a value check that will be performed when changing the
        ///          attribute from the process edit window while in design mode
        ///      -#  For each attribute that you want to add a check to,  one method with the prototype of the
        ///          following method have to be written
        ///      -#  In order to add the check to the attribute you have to set the EndInputOperationMethod
        ///          member of the Attribute with the method you generated for example :    
        /// ~~~{.cs}
        ///       // Setting the method to an existing attribute
        ///       attribute.EndInputOperationMethod =  ChandyLamport_NewStyleChannelDefaultAttributeCheck
        ///      
        ///       // Setting the method to an attribute while creation
        ///       pa.Add(p.pak.SomeKey, new Attribute {Value = SomeValue, EndInputOperationMethod =  ChandyLamport_NewStyleChannelDefaultAttributeCheck})
        /// ~~~
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param       network          (BaseNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool ChandyLamport_NewStyleProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        #endregion
        #region /// \name Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///        
        /// \par Description.
        ///      This method performs checks on the channel at the check network
        ///      phase (before running and after designing)
        ///
        /// \par checking network process.
        ///      The check and correct of a network element is composed from :
        ///      -#  A CheckBuild method that does the checks and for each error it finds it fills a
        ///          BuildCorrectionParameters data structure and insert it to a list given as parameter to the
        ///          method
        ///      -#  The BuildCorrectionParameters data structure is composed from :
        ///         -#  A message that will be presented
        ///         -#  A method that corrects the error (a method with the same prototype as the CorrectionMethod of this class)
        ///         -#  An object for passing the parameters from the check method to the correction method.

        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -  In order to access element of the network, Start with the private `network` member of
        ///         this class.
        ///      -  The processes of the network are in `network.Processes`  
        ///      -  The channels of the network are in `network.Channels`  
        ///      
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param buildCorrectionsParameters Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            base.CheckBuild(buildCorrectionsParameters);

            // Insert your checks here
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CorrectionMethod(object correctionParameters)
        ///
        /// \brief Correct an error detected during check build.
        ///
        /// \par Description.
        ///      - This method is an example for the methods that needs to be created for correcting build
        ///        errors.
        ///      - For each error there has to be a method from this prototype that corrects the error.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -#  Create a correction method for each error with a method with this prototype
        ///      -#  In the CheckBuild when encountering an error set the correction method name in the
        ///          BuildCorrectionParameters data structures
        ///      -#  The methods gets its parameters from the object parameter. The structure of the
        ///          parameters has to be the same in the CheckBuild method and the correction method
        ///      -#  In order to access element of the network, Start with the private `network` member of
        ///          this class.
        ///      -#  The processes of the network are in `network.Processes`  
        ///      -#  The channels of the network are in `network.Channels`.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param correctionParameters  (object) - Options for controlling the correction.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CorrectionMethod(object correctionParameters)
        {

        }

        #endregion
        #region /// \name Run (methods that are activated while in running phase)
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void RunAlgorithm()
        ///
        /// \brief Executes the actions that should be done by the initiator to initiate the algorithm
        ///
        /// \par Description.
        ///      -#   This method is used to run the first step of the algorithm
        ///      -#   Usually it is used to run the send of the first message/s from an initiator
        ///      -#   This method is activated only once at the beginning of running of the initiator
        ///           (has ea[bp.eak.Initiator] == true)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void RunAlgorithm()
        {
            Round = 0;
            if (Initiator)
            {
                Weight = 1;
                TurnBlueInternalEventHandler();
                TakeSnapshot();
            }
            //or[bp.ork.Round] = 0;
            //if (ea[bp.eak.Initiator])
            //{
            //    or[p.ork.Weight] = 1;
            //    TurnBlueInternalEventHandler();
            //    TakeSnapshot();
            //}
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void ReceiveHandling(BaseMessage message)
        ///
        /// \brief Receive handling.
        ///
        /// \par Description.
        /// -#  This method is activated when a new message arrived to the process
        /// -#  The method processing is done according to their arrival order
        /// -#  If you want to change the order of processing use the ArrangeMessageQ
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        /// Usually the algorithm of this method is:
        /// -#  if message type is ... perform ...
        /// -#  if message type is ... perform ...
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ///
        /// \param message The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public override void ReceiveHandling(BaseMessage message)
        //{
        //ChandyLamport_NewStyleChannel channel = (ChandyLamport_NewStyleChannel)ChannelFrom(message);
        //ChandyLamport_NewStyleMessage msg = message as ChandyLamport_NewStyleMessage;
        //switch ((msg.MessageType)
        //{
        //    case BaseMessage:

        //        // If the process performed snapshot but still did not get marker from all 
        //        // it's neighbors :
        //        // Save the message - It will be sent as message in the channels
        //        if (Recordered && !channel.Recorderd)
        //        {
        //            channel.State.Add(msg.Name);
        //        }
        //        break;
        //    case (int)Marker:

        //        // If the process received a marker:
        //        // Add the weight in the marker
        //        // Perform TakeSnapshot (If first Marker in round - Send Marker to all the neighbors)
        //        Weight += msg.MarkerWeight;
        //        TakeSnapshot();

        //        // Check if the round ended (Received Marker from all it's neighbors)
        //        // Perform EndSnapshot (Send Report, reset variables) 
        //        channel.or[c.ork.Recorderd] = true;
        //        if (InChannels.All(cnl => cnl.or[c.ork.Recorderd]))
        //        {
        //            EndSnapshot();
        //        }

        //        // Change the text on the process because the weight changed
        //        pp[bp.ppk.Text] = GetProcessDefaultName() + "\n" + or[p.ork.Weight];
        //        break;
        //    case m.MessageTypes.Report:

        //        // If received a Report Message
        //        // If this is not the first report from the source processor in this round
        //        // (or the previouse round because a report can come befor or after 
        //        // the marker throw the message
        //        if (message[bm.pak.Round] < or[bp.ork.Round] - 1 ||
        //            ((AttributeList)or[p.ork.ReceivedMessageFrom]).Any((a => a.Value == (int)message[m.report.Id])))
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            or[p.ork.ReceivedMessageFrom].Add(message[m.report.Id]);
        //        }

        //        // If the process is the initiator
        //        // Add the message to the results
        //        // Add the weight to the process weight
        //        // Check condition for end round (weight == 1)
        //        // Check condition for end running (round = max rounds)
        //        if (ea[bp.eak.Initiator])
        //        {
        //            or[p.ork.Results].Add(message[m.report.Snapshot]);
        //            or[p.ork.Weight] += message[m.report.ReportWeight];
        //            pp[bp.ppk.Text] = GetProcessDefaultName() + "\n" + or[p.ork.Weight];
        //            if (or[p.ork.Weight] == 1)
        //            {
        //                PrintResults();

        //                if (or[bp.ork.Round] < pa[p.pak.MaxRounds])
        //                {
        //                    TakeSnapshot();
        //                }
        //                else
        //                {
        //                    Terminate();
        //                }
        //            }
        //        }

        //        // If the process is not the initiator
        //        // Propagate the message to all the neighbors
        //        else
        //        {
        //            SendToNeighbours(message, SelectingMethod.Exclude, new List<int> { (int)message[m.report.Id] });
        //        }
        //        break;
        //}


        public override void ReceiveHandling(BaseMessage message)
        {
            ChandyLamport_NewStyleChannel channel = (ChandyLamport_NewStyleChannel)ChannelFrom(message);
            ChandyLamport_NewStyleMessage msg = null;
            msg = message as ChandyLamport_NewStyleMessage;

            switch (msg.MessageType)
            {
                case BaseMessage:

                    // If the process performed snapshot but still did not get marker from all 
                    // it's neighbors :
                    // Save the message - It will be sent as message in the channels
                    if (Recorderd && !channel.Recorderd)
                    {
                        channel.State.Add(msg.Name);
                    }
                    break;
                case Marker:

                    // If the process received a marker:
                    // Add the weight in the marker
                    // Perform TakeSnapshot (If first Marker in round - Send Marker to all the neighbors)
                    Weight += msg.MarkerWeight;
                    TakeSnapshot();

                    // Check if the round ended (Received Marker from all it's neighbors)
                    // Perform EndSnapshot (Send Report, reset variables) 
                    channel.or[c.ork.Recorderd] = true;
                    if (InChannels.All(cnl => ((ChandyLamport_NewStyleChannel)cnl).Recorderd))
                    {
                        EndSnapshot();
                    }

                    // Change the text on the process because the weight changed
                    Text = GetProcessDefaultName() + "\n" + Weight;
                    break;
                case Report:

                    // If received a Report Message
                    // If this is not the first report from the source processor in this round
                    // (or the previouse round because a report can come befor or after 
                    // the marker throw the message
                    if (msg.Round < Round - 1 ||
                        (ReceivedMessageFrom.Any(a => a.Value == msg.ReporterId)))
                    {
                        break;
                    }
                    else
                    {
                        ReceivedMessageFrom.Add(msg.ReporterId);
                    }

                    // If the process is the initiator
                    // Add the message to the results
                    // Add the weight to the process weight
                    // Check condition for end round (weight == 1)
                    // Check condition for end running (round = max rounds)
                    if (Initiator)
                    {
                        Results.Add(msg.Snapshot);
                        Weight += msg.ReportWeight;
                        Text = GetProcessDefaultName() + "\n" + Weight;
                        if (Weight == 1)
                        {
                            PrintResults();

                            if (Round < MaxRounds)
                            {
                                TakeSnapshot();
                            }
                            else
                            {
                                Terminate();
                            }
                        }
                    }

                    // If the process is not the initiator
                    // Propagate the message to all the neighbors
                    else
                    {
                        SendReport(MessageDataFor_Report(bm.PrmSource.Prms, null, msg.ReporterId, msg.Snapshot, msg.ReportWeight), SelectingMethod.Exclude, new List<int> { (int)msg.ReporterId });
                        //SendToNeighbours(msg, SelectingMethod.Exclude, new List<int> { (int)msg.ReporterId });
                    }
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void ArrangeMessageQ(MessageQ messageQueue)
        ///
        /// \brief Arrange the order of processing of the messages
        ///
        /// \par Description.
        /// This method is activated before retrieving a message from the message queue
        /// It gives a chance for the processor to determine the order of processing
        /// of the messages
        /// 
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        /// 
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ///
        /// \param destProcessId Identifier for the destination process.
        /// \param messageQueue       The message queue.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void ArrangeMessageQ(MessageQ messageQueue)
        {
            base.ArrangeMessageQ(messageQueue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override bool MessageProcessingCondition(BaseMessage message)
        ///
        /// \brief Decide whether to process the first message in the message queue
        ///
        /// \par Description.
        /// This method is activated before retrieving a message from the message queue
        /// It gives a chance stole the processing of the first message until a condition is fulfilled
        /// 
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        /// 
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ///
        /// \param destProcessId Identifier for the destination process.
        /// \param message       The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool MessageProcessingCondition(BaseMessage message)
        {
            return base.MessageProcessingCondition(message);
        }
        #endregion
        #region /// \name Message handling
        #endregion
        #region /// \name EventHandling
        #region /// \name Internal Events
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitInternalEvents()
        ///
        /// \brief Init internal events.
        ///
        /// \par Description.
        ///      -  InternalEvent are any events that change the data during the processing  
        ///      -  The internalEvent has 2 parts:  
        ///         -#  The Event which holds the condition for activating the InternalEvent
        ///         -#  A method that is activated to implement the InternalEvent
        ///      -  There are 2 ways to add internal events  
        ///         -#  Programatically:
        ///             -#  Create a method with signature like the DefaultInternalEventHandler
        ///             -#  Add insert of the InternalEvent in this method (See the example followed)
        ///         -#  Using the GUI
        ///             -#  Create a method with signature like the DefaultInternalEventHandler
        ///             -#  Add the InternalEvent in the GUI in the design phase
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

        protected override void InitInternalEvents()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultInternalEventHandler(InternalEvent.DummyForInternalEvent dummy = null)
        ///
        /// \brief Default internal event.
        ///
        /// \par Description.
        ///      -  This method is the default InternalEvent handler.  
        ///      -  This method is used as a default value for the InternalEvent handler selection combo box  
        ///      -  When creating an InternalEvent the programmer has to create a handler like this  
        ///         And set it's name to the last parameter of the InsertInternalEvent or set it in the GUI 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/01/2018
        ///
        /// \param dummy (Optional)  (DummyForInternalEvent) - The dummy.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultInternalEventHandler(InternalEvents.DummyForInternalEvent dummy = null)
        { }
        public void TurnBlueInternalEventHandler(InternalEvents.DummyForInternalEvent dummy = null)
        {
            or[p.ork.StatusColor] = "Blue";
            pp[bp.ppk.Background] = System.Drawing.KnownColor.LightBlue;
        }
        public void TurnGreenInternalEventHandler(InternalEvents.DummyForInternalEvent dummy = null)
        {
            or[p.ork.StatusColor] = "Green";
            pp[bp.ppk.Background] = System.Drawing.KnownColor.LightGreen;
        }
        public void TurnGrayInternalEventHandler(InternalEvents.DummyForInternalEvent dummy = null)
        {
            or[p.ork.StatusColor] = "Gray";
            pp[bp.ppk.Background] = System.Drawing.KnownColor.LightGray;
        }
        #endregion
        #region /// \name Base Algorithm Events

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitBaseAlgorithm()
        ///
        /// \brief Init base algorithm data.
        ///
        /// \par Description.
        ///      -  BaseAlgorithmEvent are events representing the base algorithm  
        ///      -  The BaseAlgorithmEvent has 2 parts:  
        ///         -#  The Event which holds the condition for activating the BaseAlgorithmEvent
        ///         -#  Specifications for the message to send
        ///      -  There are 2 ways to add base algorithm events  
        ///         -#  Programatically:
        ///             -#  Add insert of the BaseAlgorithmEvent in this method (See the example followed)
        ///         -#  Using the GUI
        ///             -#  Add the BaseAlgorithmEvent in the GUI in the design phase.
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

        protected override void InitBaseAlgorithm()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override BaseMessage BuildBaseAlgorithmMessage(dynamic messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        ///
        /// \brief Builds base message.
        ///
        /// \par Description.
        ///      -  This method is called in execution time.  
        ///      -  The purpose of this method is to allow the algorithm to add fields to the Base Algorithm message  
        ///      -  The following is the process of sending a base algorithm message  
        ///         -#  During running time before/after a send/receive message the event is checked
        ///         -#  If the conditions are filled up this method is called with the base message data
        ///         -#  This message can add fields to the Base Message according to the status of the algorithm
        ///         -#  The base message is sent  
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

        public override BaseMessage BuildBaseAlgorithmMessage(dynamic messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        {
            BaseMessage message = new BaseMessage(network, messageType, messageFields, new BaseChannel(), messageName, round);
            return message;
        }
        #endregion
        #endregion
        #region /// \name Running utility methods (methods used while in running phase)
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void BeforeSendOperation(BaseMessage message)
        ///
        /// \brief Before send operation.
        ///
        /// \par Description.
        ///      This method is used for algorithm specific actions before a send of a message
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

        protected override void BeforeSendOperation(BaseMessage message)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void AfterSendOperation(BaseMessage message)
        ///
        /// \brief After send operation.
        ///
        /// \par Description.
        ///      This method is used for algorithm specific actions after a send of a message
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

        protected override void AfterSendOperation(BaseMessage message)
        {

        }
        #endregion
        #region /// \name Algorithm Utility methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void TakeSnapshot()
        ///
        /// \brief Take snapshot.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///      -# Advance round
        ///      -# Take the status of the processor (in this simulation take the round)
        ///      -# Set the recorded parameter to true
        ///      -# Clear the list of source processes that sent report
        ///      -3 Send Marker message to all the neighbors with weight
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/01/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void TakeSnapshot()
        {
            if (!Recorderd)
            {
                Round++;
                Snapshot = StatusColor;
                Recorderd = true;
                ReceivedMessageFrom.Clear();
                double weight = Weight;
                int numOfNeighbours = OutChannels.Count;
                Weight = weight / 2;
                SendMarker(MessageDataFor_Marker(bm.PrmSource.Prms, null,
                    weight / (2 * numOfNeighbours)));
                if (Initiator)
                {
                    Results.Add("--- Results of round " + Round + " --");
                }

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void EndSnapshot()
        ///
        /// \brief Ends a snapshot.
        ///
        /// \par Description.
        ///      This message is sent after receiving Marker from all the neighbors
        ///
        /// \par Algorithm.
        ///      
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/01/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EndSnapshot()
        {
            // If the process is the initiator - save the snapshot to the results
            if (Initiator)
            {
                Results.Add(RecordSnapshot());
            }

            // If the process is not the initiator - Send a report and clear the weight
            else
            {
                SendReport(MessageDataFor_Report(bm.PrmSource.Prms, null, Id, RecordSnapshot(), Weight));
                Weight = 0;
            }

            // Set the self Id to the list of source processes (in order to block report message coming
            // from this processor (If it is in a circle) 
            ReceivedMessageFrom.Add(Id);

            // Init the algorithm flags and variables for the next round snapshot
            Recorderd = false;
            Snapshot = "Not Set";
            foreach (ChandyLamport_NewStyleChannel cnl in InChannels)
            {
                cnl.Recorderd = false;
                cnl.State.Clear();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string RecordSnapshot()
        ///
        /// \brief Recored the snapshot.
        ///
        /// \par Description.
        ///      This method is called from EndSnapshot to create the string for the results
        ///      of the snapshot
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/01/2018
        ///
        /// \return The new snapshot.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string RecordSnapshot()
        {
            string s = Name;
            s += ": Status = " + Snapshot;
            string channelsStr = "";
            foreach (ChandyLamport_NewStyleChannel channel in InChannels)
            {
                channelsStr += channel.MessageList();
            }
            if (channelsStr != "")
            {
                s += " Messages : " + channelsStr;
            }
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void PrintResults()
        ///
        /// \brief Print results.
        ///
        /// \par Description.
        ///      This method creates a MessageBox with the results after reports arrived from all the processes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/01/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PrintResults()
        {
            List<string> results = new List<string>();
            results.FromAttributeList(Results);
            MessageRouter.MessageBox(results, "Results of Round " + Round.ToString());
        }
        #endregion
        #region /// \name Presentation (methods used to update the presentation)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string PresentationText()
        ///
        /// \brief Generates the text that will be presented on the screen.
        ///        
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string PresentationText()
        {
            return base.PresentationText();
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
            return base.ToString();
        }
        #endregion
        #region /// \name algorithm utility methods
        #endregion
    }
}
