////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\Echo\EchoProcess.cs
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

namespace DistributedAlgorithms.Algorithms.Waves.Echo
{
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoProcess
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
    ///     // Send a message to a destination process  
    ///     protected void Send(int destProcessId, Message message)
    ///     
    ///     // Send a message to all neighbors (with except list)
    ///     protected override void SendToNeighbours(BaseMessage message, List<int> exclude)
    ///     
    ///     // Compose a message and send to all neighbors
    ///     protected override void SendToNeighbours(dynamic messageType, AttributeDictionary fields, List<int> exclude = null, int round = 0, int logicalClock = 0)
    /// 
    ///     // Terminate the algorithm (It will cause the whole network to terminate).
    ///     // Can be activated from one process or from several processes at the same time
    ///     public bool Terminate(Message message = null)
    ///     
    ///     // Default process name
    ///     public string GetProcessDefaultName()
    ///     
    ///     // List of incoming channels without the self connecting channel
    ///     protected List<EchoChannel> InChannels()
    ///      
    ///     // List of all the outgoing channels without the self connecting channel
    ///     protected List<EchoChannel> OutChannels()
    ///     
    ///     // Send base algorithm messages connected to user events
    ///     protected void BaseAlgorithmUserEvent(int round)
    ///     
    /// ~~~
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class EchoProcess : BaseProcess
    {
        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoProcess() : base()
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

        public EchoProcess() : base() { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoProcess(EchoNetwork network) : base(network)
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
        /// \param network  (EchoNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EchoProcess(EchoNetwork network) : base(network) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EchoProcess(bool permissionsValue) : base(permissionsValue)
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

        public EchoProcess(bool permissionsValue) : base(permissionsValue) { }

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitBaseAlgorithmData()
        ///
        /// \brief Init base algorithm data.
        ///
        /// \par Description.
        ///      -#  The base algorithm data is found in the attribute or[bp.ork.GenerateBaseMessageData]
        ///      -#  It is a list of events in which a sending of the base message will occur
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///     The algorithm of the method should look like this:
        /// ~~~{.cs}
        /// switch(ea[ne.eak.Id]):
        /// {
        ///     case 0:
        ///         InsertBaseMessageData(  BaseAlgorithmEvent.AfterSendMessage,   // The event
        ///                                 0,                                     // The round on the message
        ///                                 BaseMessage.MessageTypes.BaseMessage,  // *The type of the message that will be sent
        ///                                 "m1",                                  // The name of the message that will be sent
        ///                                 new AttributeList { new Attribute { Value = 1 } }  // A list of destinations for the message
        ///                                 BaseMessage.MessageTypes.BaseMessage,  // *The message that was sent/received - omit if the event is user event
        ///                                 1);                                    // The source/destination of the message - omit if the event is user event
        /// }
        /// ~~~
        /// * You can change this parameter to a message type of your algorithm  
        ///
        /// \author Ilan Hindy
        /// \date 28/02/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitBaseAlgorithmData()
        {
            int processId = ea[ne.eak.Id];
            switch (processId)
            {
            }
        }

        #endregion
        #region /// \name Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EchoProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///       attribute.EndInputOperationMethod =  EchoChannelDefaultAttributeCheck
        ///      
        ///       // Setting the method to an attribute while creation
        ///       pa.Add(p.pak.SomeKey, new Attribute {Value = SomeValue, EndInputOperationMethod =  EchoChannelDefaultAttributeCheck})
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

        public static bool EchoProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
            SendWave(MessageDataFor_Wave(bm.PrmSource.Prms, null, (short)ea[ne.eak.Id]));
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
            or[p.ork.Received].Add(message[m.wave.Parent]);
            if (or[p.ork.Parent] == -1 && !ea[bp.eak.Initiator])
            {
                or[p.ork.Parent] = message[m.wave.Parent];
                if (OutChannels.Count > 1)
                {
                    SendWave(MessageDataFor_Wave(bm.PrmSource.Prms, null, ea[ne.eak.Id]), SelectingMethod.Exclude, new List<int> { or[p.ork.Parent] });
                }
                else
                {
                    SendWave(MessageDataFor_Wave(bm.PrmSource.Prms, null, ea[ne.eak.Id]), SelectingMethod.Include, new List<int> { or[p.ork.Parent] });
                }
            }
            else
            {
                if (or[p.ork.Received].Count == OutChannels.Count)
                {
                    if (or[p.ork.Parent] != -1)
                    {
                        SendWave(MessageDataFor_Wave(bm.PrmSource.Prms, null, ea[ne.eak.Id]), SelectingMethod.Include, new List<int> { or[p.ork.Parent] });
                    }
                    else
                    {
                        Terminate();
                    }
                }
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override BaseMessage BuildBaseAlgorithmMessage(object messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        ///
        /// \brief Builds base message.
        ///
        /// \par Description.
        ///      This method is called when there is a decision to send a base
        ///      message, in order to build the base message.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes. 
        ///      - Change this method if you want to add fields to the base algorithm message  
        ///      - Add the fields as attributes to the messageFields AttributeDictionary
        ///
        /// \author Ilan Hindy
        /// \date 28/02/2017
        ///
        /// \param messageType   (object) - Type of the message.
        /// \param messageName   (string) - Name of the message.
        /// \param messageFields (AttributeDictionary) - The message fields.
        /// \param round         (int) - The round.
        /// \param targets       (List&lt;Attribute&gt;) - The targets.
        ///
        /// \return A BaseMessage.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override BaseMessage BuildBaseAlgorithmMessage(object messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        {
            return base.BuildBaseAlgorithmMessage(messageType, messageName, messageFields, round, targets);
        }
        #endregion
        #region /// \name Message handling
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
