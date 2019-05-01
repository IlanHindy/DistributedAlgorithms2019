////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\Test\TestProcess.cs
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

namespace DistributedAlgorithms.Algorithms.Tests.Test
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class TestProcess
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

    public partial class TestProcess : BaseProcess
    {
        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public TestProcess() : base()
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

        public TestProcess() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public TestProcess(TestNetwork network) : base(network)
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
        /// \param network  (TestNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public TestProcess(TestNetwork network) : base(network) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public TestProcess(bool permissionsValue) : base(permissionsValue)
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

        public TestProcess(bool permissionsValue) : base(permissionsValue) { }

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
        /// \fn public static bool TestProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///       attribute.EndInputOperationMethod =  TestChannelDefaultAttributeCheck
        ///      
        ///       // Setting the method to an attribute while creation
        ///       pa.Add(p.pak.SomeKey, new Attribute {Value = SomeValue, EndInputOperationMethod =  TestChannelDefaultAttributeCheck})
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

        public static bool TestProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        delegate dynamic GetMethod();
        delegate void SetMethod();
        void Check(string checkName, GetMethod getMethod, SetMethod setMethod)
        {
            string s = "Check name ; " + checkName + "\n";
            try
            {
                setMethod();
                s += "Set succeeded\n";
            }
            catch (Exception e)
            {
                s += "Set through exception :\n\t" + e.Message + "\n";
            }

            try
            {
                s += "Value set is : " + getMethod().ToString() +"\n";
                s += "Get succeeded\n";
            }
            catch (Exception e)
            {
                s += "Get through exception : \n\t" + e.Message;
            }
            MessageRouter.MessageBox(new List<string> { s });
        }

        protected override void RunAlgorithm()
        {
            // For this test fully functioning
            // 1. Declare S1 in the process (Operating results)
            // 2. Declare Type in the channel (Operating results) (must be inserted manually  to the defs because 
            //      The program does not allow duplications
            // 3. Declare Cs1 in the channel (Private Attributes)
            // 4. Declare Cs2 in the channel (Private Attributes)
            // 5. Declare Cs3 in the channel (Operation results)
            if (pa[p.pak.RunGetSetTest])
            {
                TestChannel channel = (TestChannel)InChannels[0];
                TestMessage message = new TestMessage(network, m.MessageTypes.Message1, MessageDataFor_Message1(bm.PrmSource.Prms, null, false), channel, "MessageName");
                Check("No string key exist", () => { return channel[s1]; }, () => { channel[s1] = "Test"; });
                Check("Duplicate string key exist", () => { return channel[type]; }, () => { channel[type] = "Test"; });
                Check("Single string key exist", () => { return channel[cs3]; }, () => { channel[cs3] = "Test"; });
                Check("No key exist", () => { return channel[p.ork.S1]; }, () => { channel[p.ork.S1] = "Test"; });
                Check("Duplicate key exist", () => { return channel[c.pak.Cs2]; }, () => { channel[c.pak.Cs2] = "Test"; });
                Check("Single key exist", () => { return channel[c.ork.Cs3]; }, () => { channel[c.ork.Cs3] = "Test"; });
                Check("Access to by index", () => { return channel.Cs3; }, () => { channel.Cs3 = "Test"; });
                Check("Base message attribute access", () => { return message.SourcePort; }, () => { message.SourcePort = 123; });
                Check("Base message index access", () => { return message[sourcePort]; }, () => { message[sourcePort] = 456; });
                Check("Base message index access", () => { return message[prevAttr]; }, () => { message[prevAttr] = true; });
            }
            SendAlgorithmMessage(null, SelectingMethod.All,null, 0);
        }


        //protected override void RunAlgorithm()
        //{

        //    // For this test fully functioning
        //    // 1. Declare S1 in the process (Operating results)
        //    // 2. Declare Type in the channel (Operating results) (must be inserted manually  to the defs because 
        //    //      The program does not allow duplications
        //    // 3. Declare Cs1 in the channel (Private Attributes)
        //    // 4. Declare Cs2 in the channel (Private Attributes)
        //    // 5. Declare Cs3 in the channel (Operation results) 

        //    TestChannel channel = (TestChannel)InChannels[0];

        //    // No string key exist
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "No string key exist" });
        //        MessageRouter.MessageBox(new List<string> { channel[s1].ToString() }, "No string key exist");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "No string key exist - generated exception" });
        //    }

        //    // Duplicate string key exist
        //    try
        //    {
        //        channel.or.Add(c.ork.Type, "Read from Operation results");
        //    }
        //    catch
        //    { }
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Duplicate string key exist" });
        //        MessageRouter.MessageBox(new List<string> { channel[type].ToString() }, "Duplicate string key exist");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Duplicate string key exist - generated exception" });
        //    }

        //    // Single string key exist
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Single string key exist" });
        //        channel[cs3] = "String taken from channel[cs3]";
        //        MessageRouter.MessageBox(new List<string> { channel[cs3].ToString() }, "Single string key exist");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Single string key exist - generated exception" });
        //    }

        //    // No key exist
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "No key exist" });
        //        MessageRouter.MessageBox(new List<string> { channel[p.ork.S1].ToString() }, "No key exist");
        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "No key exist - generated exception" });
        //    }

        //    // Duplicate key exist
        //    try
        //    {
        //        channel.or.Add(c.pak.Cs2, "Value taken from or");
        //    }
        //    catch { }
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Duplicate key exist" });
        //        MessageRouter.MessageBox(new List<string> { channel[c.pak.Cs2].ToString() }, "Duplicate string key exist");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Duplicate key exist - generated exception" });
        //    }

        //    // Single key exist
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Single key exist" });
        //        channel[c.ork.Cs3] = "Value taken from channel[c.pak.Cs3]";
        //        MessageRouter.MessageBox(new List<string> { channel[c.ork.Cs3].ToString() }, "Single string key exist");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Single key exist - generated exception" });
        //    }

        //    // Acess by index
        //    try
        //    {
        //        channel.Cs3 = "Access to channel.Cs3 by index ok";
        //        MessageRouter.MessageBox(new List<string> { channel.Cs3 });
        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Access to channel.Cs3 by index - generated exception" });
        //    }

        //    // BaseMessageKey
        //    TestMessage message = new TestMessage(network, m.MessageTypes.Message1, MessageDataFor_Message1(bm.PrmSource.Prms, null, false), channel, "MessageName");
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Base message attribute access message.SourcePort" });
        //        message.SourcePort = 123;
        //        MessageRouter.MessageBox(new List<string> { message.SourcePort.ToString() }, "Base message attribute access");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Base message attribute access message.SourcePort - generated exception" });
        //    }

        //    //BaseMessage index access
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Base message index access message[_SourcePort ]" });
        //        message[sourcePort] = 456;
        //        MessageRouter.MessageBox(new List<string> { message[sourcePort].ToString() }, "Base message index access");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Base message index access message[_SourcePort ] - generated exception" });
        //    }

        //    //Message index access
        //    try
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Base message index access message[_PrevAttr ]" });
        //        message[prevAttr ] = true;
        //        MessageRouter.MessageBox(new List<string> { message[prevAttr].ToString() }, "Base message index access");

        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "Base message index access message[_PrevAttr ] - generated exception" });
        //    }

        //    //Check MessageType
        //    try
        //    {
        //        if (TypesUtility.CompareDynamics(message.MessageType, Message1))
        //        {
        //            MessageRouter.MessageBox(new List<string> { "TypesUtility.CompareDynamics(message.MessageType, Message1) returned True" });
        //        }
        //        else
        //        {
        //            MessageRouter.MessageBox(new List<string> { "TypesUtility.CompareDynamics(message.MessageType, Message1) returned False" });
        //        }
        //    }
        //    catch
        //    {
        //        MessageRouter.MessageBox(new List<string> { "TypesUtility.CompareDynamics(message.MessageType, Message1) - generated exception" });
        //    }           
        //}

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

        public override void ReceiveHandling(BaseMessage message)
        {
            if (ea[ne.eak.Id] == 1)
            {
                int x = 1;
            }
            int round = message.GetHeaderField(bm.pak.Round);
            if (message.GetHeaderField(bm.pak.MessageType) == m.MessageTypes.AlgorithmMessage)
            {
                if (ea[bp.eak.Initiator])
                {
                    round++;
                    if (round == pa[p.pak.MaxRound])
                    {
                        Terminate();
                    }
                    else
                    {
                        or[bp.ork.Round]++;
                        SendAlgorithmMessage(null, SelectingMethod.Exclude, null, round, 0);
                    }
                }
                else
                {
                    SendAlgorithmMessage(null, SelectingMethod.Exclude, null, round, 0);
                }
                
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
        ///         -#  Methods that are activated to implement the InternalEvent
        ///      -  There are 2 ways to add internal events  
        ///         -#  Programmatic:
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
            InsertInternalEvent(
                0,
                EventTriggerType.BeforeSendMessage,
                2,
                bm.MessageTypes.Forewared,
                1,
                new List<InternalEvents.InternalEventDelegate> { InCodeInternalEventHandler });
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
        {
            MessageRouter.MessageBox(new List<string> { "In DefaultInternalEventHandler" }, "TestProcess");
        }
        public void OtherInternalEventHandler(InternalEvents.DummyForInternalEvent dummy = null)
        {
            MessageRouter.MessageBox(new List<string> { "In OtherInternalEventHandler" }, "TestProcess");
        }
        public void InCodeInternalEventHandler(InternalEvents.DummyForInternalEvent dummy = null)
        {
            MessageRouter.MessageBox(new List<string> { "In InCodeInternalEventHandler" }, "TestProcess");
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
            BaseAlgorithmMessages messages = new BaseAlgorithmMessages();
            messages.AddMessage(bm.MessageTypes.NullMessageType,
                "BaseMessages",
                new AttributeDictionary(),
                new AttributeList { 1 });
            InsertBaseAlgorithmEvent(0,
                EventTriggerType.BeforeSendMessage,
                3,
                bm.MessageTypes.Forewared,
                1,
                messages);
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
