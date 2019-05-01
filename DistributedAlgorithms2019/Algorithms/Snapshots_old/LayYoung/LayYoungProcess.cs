////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\LayYoung\LayYoungProcess.cs
///
/// \brief Implements the template process class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.System.Base;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Snapshots.LayYoung
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class LayYoungProcess
    ///
    /// \brief A template process.
    ///
    /// \brief #### Description.
    /// when working there are the following phases
    /// -#  Init    - create a new process (while creating a new network)
    /// -#  Design  - Designe the network and fill it's parameters
    /// -#  Check   - Check if the network design is leagal according to the algorithm specifications
    /// -#  Create  - Create the network from the software point of view
    /// -#  Run     - Run the algorithm
    /// -#  Presentation - update the presentation while running 
    ///
    /// \brief #### Usage Notes.
    ///        
    /// \brief The order of operation for an algorithm is:
    /// -#  Activate the RunAlgorithm once to initiate the running (usually send first message from initiator)
    /// -#  For each message arrived run ReceiveHandeling
    ///
    /// \brief The following are utility methods that can be used for processing:
    ///
    /// ~~~{.cs}
    ///     // Send a message to a destination process  
    ///     protected void Send(int destProcessId, Message message)
    ///     
    ///     // Send a message to all neighbours (with except list)
    ///     protected override void SendToNeighbours(BaseMessage message, List<int> exclude)
    ///     
    ///     // Compose a message and send to all neighbours
    ///     protected override void SendToNeighbours(dynamic messageType, AttributeDictionary fields, List<int> exclude = null, int round = 0, int logicalClock = 0)
    /// 
    ///     // Terminate the algorithm (It will cause the whole network to terminate.
    ///     // Can be activated from one process or from several processes at the same time
    ///     public bool Terminate(Message message = null)
    ///     
    ///     // Default process name
    ///     public string GetProcessDefaultName()
    ///     
    ///     // List of incomming channels without the self connecting channel
    ///     protected List<BaseChannel> InChannels()
    ///      
    ///     // List of all the outgoing channels without the self connecting channel
    ///     protected List<BaseChannel> OutChannels()
    /// ~~~
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class LayYoungProcess : BaseProcess
    {
        #region Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrivateAttributeKeys
        ///
        /// \brief Values that represent private attribute keys. Private attributes are attributes of
        /// the process that do not change during operation Insert.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrivateAttributeKeys { MaxRounds }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum OperationResultKeys
        ///
        /// \brief Values that represent operation result keys. Operation results are all the variables
        /// used by the program to implement the algorithm
        /// 
        /// \remark Even though it is possible to use regular variables the regular variables cannot be
        /// reconstructed when saving and loading debug statuses and cannot be changed by the user during
        /// running of the algorithm.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new enum OperationResultKeys { Round, PrevRoundReportSources, CurrentRoundReportSources, PrevRoundWeight, CurrentRoundWeight, Parent, Snapshot, NetworkSnapshots, NetworkSnapshotsMessages }
        #endregion
        #region constructors
        #endregion
        #region Init (methods that are activated while in Init phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void Init(int idx, bool clearPresentation = true)
        ///
        /// \brief This method is activated when the init button is pressed to init the process
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -   Do not deleate the call to bese.Init
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Init(int idx, bool clearPresentation = true)
        {
            base.Init(idx, clearPresentation);

            // Insert your modifications here
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitPrivateAttributes.Value()
        ///
        /// \brief Init private attributes.
        ///        
        /// \brief #### Description.
        /// -#  This method is activated when a new process is initiated This method is used in order to
        /// add new private attributes to the process
        /// -#  Private attributes are attributes describing the process that do not change during
        /// execution.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Insert all the new attributes after the statement : `base.InitPrivateAttributes.Value;`
        /// -#  The following is the format of adding to a dictionary `PrivateAttributes.Value.Add(key, new
        /// Attribute() { Value = ...})`.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitPrivateAttributes.Value()
        {
            base.InitPrivateAttributes.Value();

            // Add initialization of the private attributes here
            PrivateAttributes.Value.Add(PrivateAttributeKeys.MaxRounds, new Attribute { Value = 3 });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitOperationResults.Value()
        ///
        /// \brief Init operation results.
        ///        
        ///  \brief #### Description.
        /// -#  This method is activated when a new process is initiated This method is used in order to
        /// add new operation results to the process
        /// -#  Operation results are attributes tha holds the execution parameters.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Insert all the new attributes after the statement : `base.InitOperationResults.Value();`
        /// -#  The following is the format of adding to a dictionary `OperationResults.Value.Add(key, new
        /// Attribute() { Value = ...})`.
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitOperationResults.Value()
        {
            base.InitOperationResults.Value();

            // Add initialization of OperationResults.Value here
            OperationResults.Value.Add(OperationResultKeys.Round, new Attribute { Value = 0 });
            OperationResults.Value.Add(OperationResultKeys.Parent, new Attribute { Value = -1 });
            OperationResults.Value.Add(OperationResultKeys.PrevRoundReportSources, new Attribute { Value = new AttributeList() });
            OperationResults.Value.Add(OperationResultKeys.CurrentRoundReportSources, new Attribute { Value = new AttributeList() });
            OperationResults.Value.Add(OperationResultKeys.PrevRoundWeight, new Attribute { Value = 0 });
            OperationResults.Value.Add(OperationResultKeys.CurrentRoundWeight, new Attribute { Value = 0 });
            OperationResults.Value.Add(OperationResultKeys.Snapshot, new Attribute { Value = -1 });
            OperationResults.Value.Add(OperationResultKeys.NetworkSnapshots, new Attribute { Value = new AttributeDictionary() });
            OperationResults.Value.Add(OperationResultKeys.NetworkSnapshotsMessages, new Attribute { Value = new AttributeDictionary() });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void InitBaseAlgorithmData()
        ///
        /// \brief Init base algorithm data.
        ///
        /// \brief #### Description.
        /// -#  The base algorithm data is found in the attribute ElementAttributes.Value[ElementAttributeKeys.GenerateBaseMessageData]
        /// -#  It is a list of events in which a sending of the base message will occure
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// The algorithm of the method should look like this:
        /// ~~~{.cs}
        /// switch(ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value):
        /// {
        ///     case 0:
        ///         InsertBaseMessageData(  BaseAlgorithmEvent.AfterSendMessage,   // The event
        ///                                 0,                                     // The round on the message
        ///                                 BaseMessage.MessageTypes.BaseMessage,  // *The type of the message that will be sent
        ///                                 "m1",                                  // The name of the message that will be sent
        ///                                 new AttributeList { new Attribute { Value = 1 } }  // A list of destinations for the message
        ///                                 BaseMessage.MessageTypes.BaseMessage,  // *The message that was sent/received - ommit if the event is user event
        ///                                 1);                                    // The source/destination of the message - ommit if the event is user event
        /// }
        /// ~~~
        /// * You can change this parameter to a message type of your algorithm  
        ///
        /// \author Ilan Hindy
        /// \date 28/02/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitBaseAlgorithmData()
        {
            int processId = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            AttributeDictionary round_0 = new AttributeDictionary { { LayYoungMessage.FieldKeys.BaseMessageRound, new Attribute { Value = 0 } } };
            AttributeDictionary round_1 = new AttributeDictionary { { LayYoungMessage.FieldKeys.BaseMessageRound, new Attribute { Value = 1 } } };
            AttributeDictionary round_2 = new AttributeDictionary { { LayYoungMessage.FieldKeys.BaseMessageRound, new Attribute { Value = 2 } } };
            switch (processId)
            {
                case 0:
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        0, LayYoungMessage.MessageTypes.Base, "m1", 
                        round_0, new AttributeList { new Attribute { Value = 1 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        0, LayYoungMessage.MessageTypes.Base, "m2",
                        round_0, new AttributeList { new Attribute { Value = 1 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        0, LayYoungMessage.MessageTypes.Base, "m3",
                        round_1, new AttributeList { new Attribute { Value = 1 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        0, LayYoungMessage.MessageTypes.Base, "m4",
                        round_0, new AttributeList { new Attribute { Value = 1 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        1, LayYoungMessage.MessageTypes.Base, "m5",
                        round_1, new AttributeList { new Attribute { Value = 2 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        1, LayYoungMessage.MessageTypes.Base, "m6",
                        round_1, new AttributeList { new Attribute { Value = 2 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        1, LayYoungMessage.MessageTypes.Base, "m7",
                        round_2, new AttributeList { new Attribute { Value = 2 } });
                    InsertBaseMessageData(BaseAlgorithmEvent.UserEvent,
                        1, LayYoungMessage.MessageTypes.Base, "m8",
                        round_1, new AttributeList { new Attribute { Value = 2 } });
                    break;
            }
        }

        #endregion
        #region Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool LayYoungProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        ///
        /// \brief A prototype for attribute checking 
        ///        
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Each Attribute can be assigned a value check that will be performed when changing the
        /// attribute from the process edit window while in design mode
        /// -#  for each attribute that you want to add a check to one method with the prototype of the
        /// following method have to be designed
        /// -#  In order to add the check to the attribute you have to set the EndInputOperationMethod
        /// variable of the Attribute with the method you generated for example :
        ///     -# `attribute.EndInputOperationMethod =  LayYoungProcessDefaultAttributeCheck`
        ///     -# `PrivateAttributes.Value.Add(PrivateAttributeKeys.SomeKey, new Attribute {Value = SomeValue,
        ///         EndInputOperationMethod =  LayYoungProcessDefaultAttributeCheck})`
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param       network         The network.
        /// \param       networkElement  The network element.   (in this case the process that is been editted)
        /// \param       parentAttribute The parent attribute.  (The parent attribute in the editting window tree)
        /// \param       attribute       The attribute.         (The attribute that is been changed
        /// \param       newValue        The new value.         (The new value
        /// \param [out] errorMessage    Message describing the error.  (A string for a message that will be presented if an error was detected by this method)
        ///
        /// \return True if it's OK to change the attribute's value
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool LayYoungProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        #endregion
        #region Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///        
        /// \brief #### Description.
        /// This method performs checks on the process at the check network phase 
        /// (before running and after designing)
        ///
        /// \brief #### checking network process.
        /// The check and correct of a network element is composed from :
        /// -#  A CheckBuild method that does the checks and for each error it finds it fills a
        ///     BuildCorrectionParameters data structure and insert it to a list given as 
        ///     parameter to the method
        /// -#  The BuildCorrectionParameters data structure is composed from :
        ///     -#  A message that will be presented
        ///     -#  A method that correctes the error
        ///     -#  An object for passing the parameters from the check method to the correction method
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  In order to access tne element of the network start with the private `network` member of this class
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
        /// \brief Correct an error detected during check build
        ///        
        /// \brief #### Description.
        /// - This method is an example for the methods that needs to be created for correcting build errors.
        /// - For each error that has to be handled a method from this prototype has to be created
        /// 
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// -#  Create a correction method for the error with this prototype
        /// -#  In the CheckBuild when encountering an error set the correction method name in the 
        ///     BuildCorrectionParameters data structurs
        /// -#  The methods gets its parameters from the object parameter. The structure of the 
        ///     parameters has to be the same in the CheckBuild method and the correction method
        /// -#  In order to access tne element of the network start with the private `network` member of this class
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param buildCorrectionsParameters Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CorrectionMethod(object correctionParameters)
        {

        }

        #endregion
        #region Run (methods that are activated while in running phase)
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void RunAlgorithm()
        ///
        /// \brief Executes the algorithm operation.
        ///
        /// \brief #### Description.
        /// -#   This method is used to run the first step of the algorithm
        /// -#   Usually it is used to run the send the first message/s from an initiator
        /// -#   This method is activated only onece at the beginning of running an algorithm by the process
        /// -#   This method is activated only if the process has the attribute Initiator = true
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 26/01/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void RunAlgorithm()
        {
            // Note - Remove the following lise because your algorithm does not need to operate
            //        The base process algorithm
            //base.RunAlgorithm();

            TakeSnapshot(0.5, ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value, null);
            OperationResults.Value[OperationResultKeys.PrevRoundWeight].Value = 0.5;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void ReceiveHandling(BaseMessage message)
        ///
        /// \brief Receive handling.
        ///
        /// \brief #### Description.
        /// -#  This method is activated when a new message arrived to the process
        /// -#  The method processing is done according to theire arrival order
        /// -#  If you want to change the order of processing save the messages In an OperationResult attribute
        ///     and then when you want to activate it process it befor procesing the message that is given 
        ///     to this method as a parameter
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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

            // Note - Remove the following lise because your algorithm does not need to operate
            //        The base process algorithm
            //base.ReceiveHandling(message);
            LayYoungMessage.MessageTypes messageType = message.GetHeaderField(BaseMessage.HeaderFieldKeys.MessageType).Value;
            switch (messageType)
            {
                case LayYoungMessage.MessageTypes.Base:
                    HandleBaseMessage(message);
                    break;
                case LayYoungMessage.MessageTypes.Presnp:
                    HandlePresnpMessage(message);
                    break;
                case LayYoungMessage.MessageTypes.Weight:
                    HandleWeightMessage(message);
                    break;
                case LayYoungMessage.MessageTypes.Report:
                    HandleReportMessage(message);
                    break;
            }
            UpdatePresentation();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void ArrangeMessageQueue(ref AttributeList messageQueue)
        ///
        /// \brief Arrange the order of processing of the messages
        ///
        /// \brief #### Description.
        /// This method is activated before retrieving a message from the message queue
        /// It gives a chance for the processor to determin the order of processing
        /// of the messages
        /// 
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        /// \brief #### Description.
        /// This method is activated before retrieving a message from the message queue
        /// It gives a chance stoll the processing of the first message untill a condition is fullfilled
        /// 
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        /// \fn protected override BaseMessage BuildBaseMessage(object messageType, string messageName, int round)
        ///
        /// \brief Builds base message.
        ///
        /// \brief #### Description.
        /// This method is called when there is a decision to send a base message
        /// in order to build the base message
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        /// The base class method generate a message with the type and one field for the message name
        ///
        /// \author Ilan Hindy
        /// \date 28/02/2017
        ///
        /// \param messageType Type of the message.
        /// \param messageName Name of the message.
        /// \param round       The round.
        ///
        /// \return A BaseMessage.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override BaseMessage BuildBaseMessage(object messageType, string messageName,  AttributeDictionary messageFields, int round, AttributeList targets)
        {
            int messageRound = messageFields[LayYoungMessage.FieldKeys.BaseMessageRound].Value;
            int processRound = OperationResults.Value[OperationResultKeys.Round].Value;
            if (processRound == messageRound)
            {
                LayYoungChannel channel = (LayYoungChannel)OutChannels().First(c => c[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value == targets[0].Value);
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Sent].Value++;
            }
            return base.BuildBaseMessage(messageType, messageName, messageFields, round, targets);
        }
        #endregion
        #region Message handeling
        private void HandleBaseMessage(BaseMessage message)
        {
            int messageRound = message.GetField(LayYoungMessage.FieldKeys.BaseMessageRound).Value;
            int processRound = OperationResults.Value[OperationResultKeys.Round].Value;
            int messageSource = message.GetHeaderField(BaseMessage.HeaderFieldKeys.SourceProcess).Value;
            LayYoungChannel channel = (LayYoungChannel)InChannels().First(c => c[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.SourceProcess].Value == messageSource);

            // if this is the first message from the next round - perform snapshot
            if (messageRound > processRound)
            {
                TakeSnapshot(0, messageSource, channel);
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value++;
                return;
            }

            // If this is not the first message from the next round:
            // If the message is from this round - advance the count
            // else add the message to state - a list of messages that where in the channel
            // at the time of the snapshot that ended the round
            if (messageRound == processRound)
            {
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value++;
            }
            else
            {
                channel.AddToState(message);
                SendReportIfNeeded();
            }
        }

        private void HandlePresnpMessage(BaseMessage message)
        {
            int messageSource = message.GetHeaderField(BaseMessage.HeaderFieldKeys.SourceProcess).Value;
            int messageRound = message.GetHeaderField(BaseMessage.HeaderFieldKeys.Round).Value;
            int messageExpected = message.GetField(LayYoungMessage.FieldKeys.Expected).Value;
            double messageWeight = message.GetField(LayYoungMessage.FieldKeys.Weight).Value;
            int processRound = OperationResults.Value[OperationResultKeys.Round].Value;
            int processParent = OperationResults.Value[OperationResultKeys.Parent].Value;
            LayYoungChannel channel = (LayYoungChannel)InChannels().First(c => c[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.SourceProcess].Value == messageSource);
            int channelCount = channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value;
            int channelPrevRoundCount = channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.PrevRoundCount].Value;


            channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value = 0;

            // Presnp that caused the snapshot
            if (messageRound > processRound)
            {
                TakeSnapshot(messageWeight / 2, messageSource, channel);
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value = messageWeight / 2;
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Expected].Value = messageExpected - channelPrevRoundCount;
            }

            // Presnp that came after a snapshot was done (by base message)
            else
            {
          
                // If the message does not have weight - nothing has to be done - waiting for weight message
                if (messageWeight > 0)
                {
                    // If the presnap is from the parent - send the weight to all neighbours
                    if (processParent == messageSource)
                    {
                        SendWeight(messageWeight / 2);
                        channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value = messageWeight / 2;
                        channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Expected].Value = messageExpected - channelPrevRoundCount;
                    }

                    // If the message is from processor that did not couse the stapshot:
                    // update expected and weight of the channel
                    else
                    {
                        channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value = messageWeight;
                        channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Expected].Value = messageExpected - channelCount;
                    }
                    channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value = 0;
                }
            }
            SendReportIfNeeded();
        }

        private void HandleWeightMessage(BaseMessage message)
        {
            int messageSource = message.GetHeaderField(BaseMessage.HeaderFieldKeys.SourceProcess).Value;
            double messageWeight = message.GetField(LayYoungMessage.FieldKeys.Weight).Value;
            LayYoungChannel channel = (LayYoungChannel)InChannels().First(c => c[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.SourceProcess].Value == messageSource);
            int processParent = OperationResults.Value[OperationResultKeys.Parent].Value;

            // if the process parent is the message source - send weight to all neighbours
            if (processParent == messageSource)
            {
                SendWeight(messageWeight / 2);
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value = messageWeight / 2;
            }

            // if the process parent is not the message source - assign the wight to the channel
            else
            {
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value = messageWeight;
            }
            SendReportIfNeeded();
        }

        private void HandleReportMessage(BaseMessage message)
        {
            int messageRound = message.GetHeaderField(BaseMessage.HeaderFieldKeys.Round).Value;
            int messageOriginator = message.GetField(LayYoungMessage.FieldKeys.Id).Value;
            int messageSource = message.GetHeaderField(BaseMessage.HeaderFieldKeys.SourceProcess).Value;
            double messageWeight = message.GetField(LayYoungMessage.FieldKeys.Weight).Value;
            int processRound = OperationResults.Value[OperationResultKeys.Round].Value;
            int maxRounds = PrivateAttributes.Value[PrivateAttributeKeys.MaxRounds].Value;
            bool isOriginator = ElementAttributes.Value[ElementAttributeKeys.Initiator].Value;
            Attribute originatorSnapshot = message.GetField(LayYoungMessage.FieldKeys.Snapshot);
            Attribute originatorSnapshotMessages = message.GetField(LayYoungMessage.FieldKeys.SnapshotMessages);

            // If the message is from a finished round - exit
            if (processRound > messageRound + 1)
            {
                return;
            }

            // Se parameters according to whether the message is from this round or prev round
            AttributeList reportSources;
            Attribute weightAttribute;
            bool messageFromPrevRound;
            if (processRound == messageRound)
            {
                reportSources = OperationResults.Value[OperationResultKeys.CurrentRoundReportSources].Value;
                weightAttribute = OperationResults.Value[OperationResultKeys.CurrentRoundWeight];
                messageFromPrevRound = false;
            }
            else
            {
                reportSources = OperationResults.Value[OperationResultKeys.PrevRoundReportSources].Value;
                weightAttribute = OperationResults.Value[OperationResultKeys.PrevRoundWeight];
                messageFromPrevRound = true;
            }
            
            // If this is not the first message from the origins process - exit
            if (reportSources.Any(a => a.Value == messageOriginator))
            {
                return;
            }

            // If this is the first message from the origin process - add the originator to the already arrived originators
            else
            {
                reportSources.Add(new Attribute { Value = messageOriginator });
            }


            // If the process is an originator
            if (isOriginator)
            {
                // Update the dictionaries that holds the result
                AttributeDictionary networkSnapshots = OperationResults.Value[OperationResultKeys.NetworkSnapshots].Value;
                networkSnapshots.Add(messageOriginator, new Attribute { Value = originatorSnapshot.Value });
                AttributeDictionary networkSnapshotsMessages = OperationResults.Value[OperationResultKeys.NetworkSnapshotsMessages].Value;
                networkSnapshotsMessages.Add(messageOriginator, new Attribute { Value = originatorSnapshotMessages.Value });
                weightAttribute.Value += messageWeight;

                // If this is a message from the prev round
                // Check if the round finished and decide whether to generate a message or
                // start a termination process
                if (messageFromPrevRound)
                {
                    // Decide if this is the end of the round
                    // The end of the round is if the process weight + all network processes weights + channels wheights is 1
                    double totalWeight = weightAttribute.Value;
                    foreach (LayYoungChannel channel in IncommingChannels)
                    {
                        totalWeight += channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value;
                    }
                    if (totalWeight == 1)
                    {
                        ShowResults(messageRound);

                        // If reached the last round - terminate
                        if (maxRounds == processRound)
                        {
                            Terminate();
                        }

                        // If did not reach the last round - perform take snapshot - start a new round
                        else
                        {
                            OperationResults.Value[OperationResultKeys.NetworkSnapshots].Value = new AttributeDictionary();
                            OperationResults.Value[OperationResultKeys.NetworkSnapshotsMessages].Value = new AttributeDictionary();
                            TakeSnapshot(0.5, ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value, null);
                        }
                    }
                }
            }

            // If the process is not an initiators - propagate the message to all neighbours except
            // The process that this message was sent from and the originator of the message
            else
            {
                SendToNeighbours(message, new List<int> { messageOriginator, messageSource });
            }
        }
        #endregion
        #region Running utility methods (methods used while in running phase)
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void BeforeSendOperation(BaseMessage message)
        ///
        /// \brief Before send operation.
        ///        This method is used for algorithm specific actions before a send of a message
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        ///        This method is used for algorithm specific actions after a send of a message
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 05/04/2017
        ///
        /// \param message  (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void AfterSendOperation(BaseMessage message)
        {

        }
        private void TakeSnapshot(double weight, int messageSource, LayYoungChannel sourceChannel)
        {
   
            // Keep the count (of the round that is about to finish) in PrevRoundCount
            // for the case that the snapshot is done not by presnp
            // This operation is not done in case the method is activated by the initiator
            // (The sourceChannel == null)
            if (sourceChannel != null)
            {
                sourceChannel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.PrevRoundCount].Value =
                    sourceChannel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value;
                sourceChannel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Count].Value = 0;
            }

            // advance the round
            Attribute roundAttribute = OperationResults.Value[OperationResultKeys.Round];
            BaseAlgorithmUserEvent(roundAttribute.Value);
            roundAttribute.Value++;

            // Take the snapshot
            OperationResults.Value[OperationResultKeys.Snapshot].Value = roundAttribute.Value;

            // set the parent
            OperationResults.Value[OperationResultKeys.Parent].Value = messageSource;

            // Send presnp to all the neighbours
            SendPresnp(weight, roundAttribute.Value);


            // Clear the weight of the prev round
            foreach (LayYoungChannel channel in InChannels())
            {
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value = 0;
                channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.State].Value = new AttributeList();
            }

            // Handle Report Data
            ReportHandelingDataStartRound();
        }

        private void SendPresnp(double weight, int round)
        {
            double messageWeight = weight / OutChannels().Count();
            foreach (LayYoungChannel channel in OutChannels())
            {
                AttributeDictionary fields = new AttributeDictionary();
                Attribute sentAttribute = channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Sent];
                fields.Add(LayYoungMessage.FieldKeys.Expected, new Attribute { Value = sentAttribute.Value });
                fields.Add(LayYoungMessage.FieldKeys.Weight, new Attribute { Value = messageWeight });
                BaseMessage message = new BaseMessage(LayYoungMessage.MessageTypes.Presnp,
                    fields, channel, round, 0);
                Send(channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value, message);
                sentAttribute.Value = 0;
            }
        }

        private void ReportHandelingDataStartRound()
        {
            // Reset the report sources and the report weight attributes
            Attribute currentRoundReportSourcesAttribute = OperationResults.Value[OperationResultKeys.CurrentRoundReportSources];
            Attribute prevRoundReportSourcesAttribute = OperationResults.Value[OperationResultKeys.PrevRoundReportSources];
            prevRoundReportSourcesAttribute.Value = currentRoundReportSourcesAttribute.Value;
            currentRoundReportSourcesAttribute.Value = new AttributeList();

            Attribute currentRoundWeightAttribute = OperationResults.Value[OperationResultKeys.CurrentRoundWeight];
            Attribute prevRoundWeightAttribute = OperationResults.Value[OperationResultKeys.PrevRoundWeight];
            prevRoundWeightAttribute.Value = currentRoundWeightAttribute.Value;
            currentRoundWeightAttribute.Value = 0.5;
        }

        private void SendWeight(double weight)
        {
            double messageWeight = weight / OutChannels().Count();
            int round = OperationResults.Value[OperationResultKeys.Round].Value;
            AttributeDictionary fields = new AttributeDictionary();
            fields.Add(LayYoungMessage.FieldKeys.Weight, new Attribute { Value = messageWeight });
            SendToNeighbours(LayYoungMessage.MessageTypes.Weight, fields, null, round, 0);
        }

        private void SendReportIfNeeded()
        {
            if (ElementAttributes.Value[ElementAttributeKeys.Initiator].Value)
            {
                return;
            }

            if (InChannels().Any(channel => !((LayYoungChannel)channel).FinishedRound()))
            {
                return;
            }

            // Build the report message
            int round = OperationResults.Value[OperationResultKeys.Round].Value - 1;
            int id = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            double weight = InChannels().Select(c => (double)c[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.Weight].Value).Sum();
            Attribute snapshotAttribute = OperationResults.Value[OperationResultKeys.Snapshot];
            AttributeList snapshotMessages = new AttributeList();
            foreach (LayYoungChannel channel in InChannels())
            {
                AttributeList channelState = (AttributeList)(channel[ElementDictionaries.OperationResults].Value[LayYoungChannel.OperationResultKeys.State].Value);
                snapshotMessages = snapshotMessages.Concat(channelState).ToList().ToAttributeList();

            }
            
            AttributeDictionary fields = new AttributeDictionary();
            fields.Add(LayYoungMessage.FieldKeys.SnapshotMessages, new Attribute { Value = snapshotMessages, IncludedInShortDescription = true });
            fields.Add(LayYoungMessage.FieldKeys.Snapshot, snapshotAttribute);
            fields.Add(LayYoungMessage.FieldKeys.Id, new Attribute { Value = id });
            fields.Add(LayYoungMessage.FieldKeys.Weight, new Attribute { Value = weight });

            SendToNeighbours(LayYoungMessage.MessageTypes.Report, fields, null, round, 0);
        }

        #endregion
        #region Presentation (methods used to update the presentation)
        private void ShowResults(int round)
        {
            AttributeDictionary networkSnapshots = OperationResults.Value[OperationResultKeys.NetworkSnapshots].Value;
            AttributeDictionary networkSnapshotsMessages = OperationResults.Value[OperationResultKeys.NetworkSnapshotsMessages].Value;
            int initiatorId = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            int initiatorSnapshot = OperationResults.Value[OperationResultKeys.Snapshot].Value;
            string s = "Results of round " + round.ToString();
            s += "\n Initiator " + initiatorId.ToString() + ":" + initiatorSnapshot.ToString();
            foreach (var entry in networkSnapshots)
            {
                s += "\n Processor " + entry.Key.ToString() + ":" + entry.Value.Value.ToString();
                s += " [";
                foreach(Attribute messageAttribute in networkSnapshotsMessages[entry.Key].Value)
                {
                    s += TypesUtility.GetKeyToString(messageAttribute.Value.GetHeaderField(BaseMessage.HeaderFieldKeys.MessageType).Value) +
                        " " + messageAttribute.Value.GetField(BaseMessage.FieldKeys.MessageName).Value +
                        " " + messageAttribute.Value.GetHeaderField(BaseMessage.HeaderFieldKeys.SourceProcess).Value.ToString() +
                        "->" + messageAttribute.Value.GetHeaderField(BaseMessage.HeaderFieldKeys.DestProcess).Value.ToString();
                }
                s += "]";
            }
            MessageRouter.MessageBox(new List<string> { s }, "Report of end snapshot", null, Icons.Info);
        }

        private void UpdatePresentation()
        {
            string text = "Process : " + ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value.ToString();
            text += "\n" + "Round : " + OperationResults.Value[OperationResultKeys.Round].Value.ToString();
            text += "\n" + "Parent : " + OperationResults.Value[OperationResultKeys.Parent].Value.ToString();
            if (ElementAttributes.Value[BaseProcess.ElementAttributeKeys.Initiator].Value)
            {
                text += "\n" + "Weight :" + OperationResults.Value[OperationResultKeys.PrevRoundWeight].Value.ToString() +
                    "/" + OperationResults.Value[OperationResultKeys.CurrentRoundWeight].Value.ToString();
            }
            PresentationParameters[PresentationParameterKeys.Text].Value = text;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string PresentationText()
        ///
        /// \brief Genarates the text that will be presented on the screen.
        ///        
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
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
        #region algorithm utility methods
        #endregion
    }
}
