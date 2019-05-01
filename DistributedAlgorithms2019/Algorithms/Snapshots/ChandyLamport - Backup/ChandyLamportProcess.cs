////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\ChandyLamport\ChandyLamportProcess.cs
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

namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ChandyLamportProcess
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
    /// ~~~
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class ChandyLamportProcess : BaseProcess
    {
        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamportProcess() : base()
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

        public ChandyLamportProcess() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamportProcess(ChandyLamportNetwork network) : base(network)
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
        /// \param network  (ChandyLamportNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChandyLamportProcess(ChandyLamportNetwork network) : base(network) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ChandyLamportProcess(bool permissionsValue) : base(permissionsValue)
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

        public ChandyLamportProcess(bool permissionsValue) : base(permissionsValue) { }

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
        /// \fn public static bool ChandyLamportProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///       attribute.EndInputOperationMethod =  ChandyLamportChannelDefaultAttributeCheck
        ///      
        ///       // Setting the method to an attribute while creation
        ///       pa.Add(p.pak.SomeKey, new Attribute {Value = SomeValue, EndInputOperationMethod =  ChandyLamportChannelDefaultAttributeCheck})
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

        public static bool ChandyLamportProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
            if (ea[bp.eak.Initiator])
            {
                or[p.ork.Weight] = 1;
                TakeSnapshot();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void ReceiveHandling(BaseMessage message)
        ///
        /// \brief Receive handling.
        ///
        /// \par Description.
        /// -#  This method is activated when a new message arrived to the process
        /// -#  The method processing is done according to their arrival order
        /// -#  If you want to change the order of processing use the ArrangeMessageQueue
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

        public override void ReceiveHandling(BaseMessage message)
        {

            switch (message[bm.headerFieldKeys.MessageType])
            {
                case m.MessageTypes.BaseMessage:
                    if (or[p.ork.Recordered] && InChannels.All(channel => !channel.or[c.ork.Marker]))
                    {
                        ChannelFrom(message).or[c.ork.State].Add(message[bm.fieldKeys.MessageName]);
                    }
                    break;
                case m.MessageTypes.Marker:
                    or[p.ork.Weight] += message[m.marker.Weight];
                    TakeSnapshot();
                    ChannelFrom(message).or[c.ork.Marker] = true;
                    if (InChannels.All(channel => channel.or[c.ork.Marker]))
                    {
                        EndRound();
                    }
                    break;
                case m.MessageTypes.Report:
                    if (!((AttributeList)or[p.ork.ReceivedMessageFrom]).Any((a => a.Value == (int)message[m.report.Id])))
                    {
                        or[p.ork.ReceivedMessageFrom].Add(message[m.report.Id]);
                    }
                    if (ea[bp.eak.Initiator])
                    {
                        or[p.ork.Weight] += message[m.report.Weight];
                        or[p.ork.Results].Add(message[m.report.Snapshot]);
                        pp[bp.ppk.Text] = GetProcessDefaultName() + "\n" + or[p.ork.Weight];
                        if (or[p.ork.Weight] == 1)
                        {
                            PrintResults();
                            if (or[p.ork.Round] < pa[p.pak.MaxRounds])
                            {
                                TakeSnapshot();
                            }
                            else
                            {
                                Terminate();
                            }
                        }
                    }
                    else
                    {
                        SendToNeighbours(message, new List<int> { (int)message[m.report.Id] });
                    }
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void ArrangeMessageQueue(ref AttributeList messageQueue)
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

        protected override void ArrangeMessageQueue(ref AttributeList messageQueue)
        {
            base.ArrangeMessageQueue(ref messageQueue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override bool MessageProcessingCondition(BaseMessage message)
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

        protected override bool MessageProcessingCondition(BaseMessage message)
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
        /// protected override void InitInternalEvents()
        /// {
        ///     // For each event create a call like :
        ///    InsertInternalEvent(
        ///        Event.EventTrigger.AfterSendMessage,    // Trigger
        ///        0,                                      // Round
        ///        bm.messageTypes.Forewared,              // Message type (can be any message type)
        ///        1,                                      // The process which is the other end of the send/receive of the trigger
        ///        new AttributeList { "SomeMethod" });    // The name of the method that processes the event
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

        public void DefaultInternalEventHandler(InternalEvent.DummyForInternalEvent dummy = null)
        { }
        #endregion
        #region /// \name Base Algorithm Events

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitBaseAlgorithmEvents()
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
        ///~~~{.cs}
        ///protected virtual void InitBaseAlgorithmData()
        ///{
        ///      
        ///      // For each Base Algorithm message to be send make the following call
        ///      InsertBaseAlgorithmEvent(
        ///      
        ///         // The trigger fields. (fields that are used to decide whether to send the Base Algorithm message
        ///         Event.EventTrigger.AfterSendMessage,    // Trigger
        ///         0,                                      // Round
        ///         bm.messageTypes.Forewared,              // Message type (can be any message type)
        ///         1,                                      // The process which is the other end of the send/receive of the trigger
        ///      
        ///         // The base algorithm message (The message to be sent if the trigger is true
        ///         bm.messageTypes.Forewared,              // The type of the message to send. Can also be any type that the algorithm declares
        ///         "Message Name",                         // The name of the message to send
        ///         new AttributeDictionary {               // An AttributeDictionary which contains all the fields in the message to be sent
        ///             { bm.fieldKeys.MessageName, "somestring" } // Can also use keys that are declared by the algorithm
        ///         },
        ///         new AttributeList { 1 });                   // List of the target processes of the base algorithm message
        ///}
        ///~~~.
        ///
        /// \author Ilanh
        /// \date 20/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitBaseAlgorithmEvents()
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
        private void TakeSnapshot()
        {
            if (!or[p.ork.Recordered])
            {
                or[p.ork.Round]++;
                or[p.ork.Snapshot] = or[p.ork.Round];
                or[p.ork.Recordered] = true;
                or[p.ork.ReceivedMessageFrom].Clear();
                double weight = or[p.ork.Weight];
                int numOfNeighbours = OutChannels.Count;
                or[p.ork.Weight] = weight / 2;
                SendMarker(MessageDataFor_Marker(bm.PrmSource.Prms, null, 
                    weight / (2 * numOfNeighbours)));
                pp[bp.ppk.Text] = GetProcessDefaultName() + "\n" + or[p.ork.Weight];

            }
        }

        private void EndRound()
        {
            or[p.ork.ReceivedMessageFrom].Add(ea[ne.eak.Id]);
            SendReport(MessageDataFor_Report(bm.PrmSource.Prms, null, ea[ne.eak.Id], CreateSnapshot(), or[p.ork.Weight]));           
            or[p.ork.Recordered] = false;
            for(int idx =0; idx < InChannels.Count; idx ++)
            {
                InChannels[idx].or[c.ork.Marker] = false;
                InChannels[idx].or[c.ork.State].Clear();
            }       
        }

        private string CreateSnapshot()
        {
            string s = ea[bp.eak.Name] + "\n";
            s += "Status = " + or[p.ork.Snapshot].ToString() + " Messages : ";
            foreach (ChandyLamportChannel channel in InChannels)
            {
                s +=  channel.MessageList() + "; ";
            }
            return s;
        }

        private void PrintResults()
        {
            //string s = "";
            //for (int idx = 0; idx < or[p.ork.Results].count; idx ++)
            //{
            //    s += or[p.ork.Results][idx] + "\n";
            //}
            List<string> results = new List<string>();
            results.FromAttributeList((AttributeList)or[p.ork.Results]);
            MessageRouter.MessageBox(results, "Results of Round " + or[p.ork.Round].ToString());
        }
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
