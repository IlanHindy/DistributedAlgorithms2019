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
using DistributedAlgorithms.Algorithms.System.Base;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Snapshots.ChandyLamport
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ChandyLamportProcess
    ///
    /// \brief A template process.
    ///
    /// \brief #### Description.
    /// when working there are the following phases
    /// -#  Init    - create a new networ
    /// -#  Design  - Designe the network and fill it's parameters
    /// -#  Check   - Check if the network design is leagal according to the algorithm specifications
    /// -#  Create  - Create the network fro the software point of view
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
    /// ~~~
    ///
    /// \author Ilan Hindy
    /// \date 26/01/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class ChandyLamportProcess : BaseProcess
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrivateAttributeKeys
        ///
        /// \brief Values that represent private attribute keys. Private attributes are attributes of
        /// the process that do not change during operation Insert.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrivateAttributeKeys { NumberOfRounds}

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

        public new enum OperationResultKeys { Recorded, Snapshot, RoundNum, Weight, ReceivedReportFrom, MessagesReceived}
        

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
            InitGenerateBaseMessageData();
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
            PrivateAttributes.Value.Add(PrivateAttributeKeys.NumberOfRounds, new Attribute { Value = 3, IncludedInShortDescription = true }); 
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
            OperationResults.Value.Add(OperationResultKeys.Recorded, new Attribute { Value = false, IncludedInShortDescription = true });
            OperationResults.Value.Add(OperationResultKeys.Snapshot, new Attribute { Value = -1, IncludedInShortDescription = true });
            OperationResults.Value.Add(OperationResultKeys.RoundNum, new Attribute { Value = 0, IncludedInShortDescription = true });
            OperationResults.Value.Add(OperationResultKeys.Weight, new Attribute { Value = 0, IncludedInShortDescription = true });
            OperationResults.Value.Add(OperationResultKeys.ReceivedReportFrom, new Attribute { Value = new AttributeDictionary(), IncludedInShortDescription = true });
            OperationResults.Value.Add(OperationResultKeys.MessagesReceived, new Attribute { Value = new AttributeDictionary(), IncludedInShortDescription = true });
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
        /// * You can chane this parameter to a message type of your algorithm  
        ///
        /// \author Ilan Hindy
        /// \date 28/02/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void InitGenerateBaseMessageData()
        {
            int processId = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            switch (processId)
            {
                case 0:
                    InsertBaseMessageData(BaseAlgorithmEvent.AfterSendMessage, 0, ChandyLamportMessage.MessageTypes.BaseMessage, "m1", new AttributeDictionary(), new AttributeList { new Attribute { Value = 1 } }, ChandyLamportMessage.MessageTypes.Marker, 1);
                    break;
                case 1:
                    InsertBaseMessageData(BaseAlgorithmEvent.BeforeSendMessage, 0, ChandyLamportMessage.MessageTypes.BaseMessage, "m2", new AttributeDictionary(), new AttributeList { new Attribute { Value = 2 } }, ChandyLamportMessage.MessageTypes.Marker, 2);
                    break;
            }
        }

        #endregion
        #region Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool ChandyLamportProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///     -# `attribute.EndInputOperationMethod =  ChandyLamportProcessDefaultAttributeCheck`
        ///     -# `PrivateAttributes.Value.Add(PrivateAttributeKeys.SomeKey, new Attribute {Value = SomeValue,
        ///         EndInputOperationMethod =  ChandyLamportProcessDefaultAttributeCheck})`
        ///
        /// \author Ilan Hindy
        /// \date 24/01/2017
        ///
        /// \param       network         The network.
        /// \param       networkElement  The network element.
        /// \param       parentAttribute The parent attribute.
        /// \param       attribute       The attribute.
        /// \param       newValue        The new value.
        /// \param [out] errorMessage    Message describing the error.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool ChandyLamportProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
            // base.RunAlgorithm();

            OperationResults.Value[OperationResultKeys.Weight].Value = 1.0;
            TakeSnapshot(CreateFirstMarker(0));
            UpdatePresentation();
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
            // base.ReceiveHandling(message);
            
            ChandyLamportMessage.MessageTypes messageType =
                (ChandyLamportMessage.MessageTypes)message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.MessageType].Value;
            int sourceProcessId = message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.SourceProcess].Value;
            ChandyLamportChannel channel = (ChandyLamportChannel)InChannels().First(
                c => c[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.SourceProcess].Value == sourceProcessId);
            switch (messageType)
            {
                case ChandyLamportMessage.MessageTypes.BaseMessage:
                    BaseMessageArrived(message, channel);
                    break;
                case ChandyLamportMessage.MessageTypes.Marker:
                    MarkerArrived(message, channel);
                    break;
                case ChandyLamportMessage.MessageTypes.Report:
                    ReportArrived(message);
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
            //base.ArrangeMessageQueue(ref messageQueue);
            // messageQueue = (AttributeList)messageQueue.OrderBy(attribute => attribute.Value.GetHeaderField(BaseMessage.HeaderFieldKeys.Round).Value);
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

        private void BaseMessageArrived(BaseMessage message, ChandyLamportChannel channel)
        {
            bool markerArrivedFromChannel = channel[ElementDictionaries.OperationResults].Value[ChandyLamportChannel.OperationResultKeys.MarkerArrived].Value;
            bool recorded = OperationResults.Value[OperationResultKeys.Recorded].Value;
            if (!markerArrivedFromChannel && recorded)
            {
                channel.AddMessage(message);
            }
        }

        private void MarkerArrived(BaseMessage marker, ChandyLamportChannel channel)
        {
            // Add the weight of the message to the weight of the process
            double messageWeight = marker.GetField(ChandyLamportMessage.FieldKeys.Weight).Value;
            OperationResults.Value[OperationResultKeys.Weight].Value += messageWeight;

            TakeSnapshot(marker);

            // Termination condition
            channel[ElementDictionaries.OperationResults].Value[ChandyLamportChannel.OperationResultKeys.MarkerArrived].Value = true;

            if (InChannels().All(c => c[ElementDictionaries.OperationResults].Value[ChandyLamportChannel.OperationResultKeys.MarkerArrived].Value))
            {
                bool initiator = ElementAttributes.Value[BaseProcess.ElementAttributeKeys.Initiator].Value;
                if (!initiator)
                {
                    Report();
                }
            }
        }

        public void Report()
        {
            BaseMessage reportMessage = CreateReport();
            SendToNeighbours(reportMessage, null);
            OperationResults.Value[OperationResultKeys.Recorded].Value = false;
            InChannels().ForEach(c => c[ElementDictionaries.OperationResults].Value[ChandyLamportChannel.OperationResultKeys.MarkerArrived].Value = false);
        }

        private void ReportArrived(BaseMessage report)
        {
            // Get attributes of the process
            AttributeDictionary receivedReportFrom = OperationResults.Value[OperationResultKeys.ReceivedReportFrom].Value;
            bool initiator = ElementAttributes.Value[BaseProcess.ElementAttributeKeys.Initiator].Value;
            int maxRound = PrivateAttributes.Value[PrivateAttributeKeys.NumberOfRounds].Value;
            int processId = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            int processSnapshot = OperationResults.Value[OperationResultKeys.Snapshot].Value;
            int processRound = OperationResults.Value[OperationResultKeys.RoundNum].Value;
            double processWeight = OperationResults.Value[OperationResultKeys.Weight].Value;
            AttributeDictionary processMessagesArived = OperationResults.Value[OperationResultKeys.MessagesReceived].Value;

            // Get attributes of the message
            double messageWeight = report.GetField(ChandyLamportMessage.FieldKeys.Weight).Value;            
            int messageId = report.GetField(ChandyLamportMessage.FieldKeys.Id).Value;            
            int messageSnapshot = report.GetField(ChandyLamportMessage.FieldKeys.Snapshots).Value;
            int messageRound = report.GetHeaderField(BaseMessage.HeaderFieldKeys.Round).Value;
            AttributeList listOfMessagesArrived = report.GetField(ChandyLamportMessage.FieldKeys.MessageSnapshot).Value;

            // The algorithm
            // If a message from the process arrived (in this round) 
            // or the round advanced (which mees that the initiator got report from al the processor
            // throw the message
            if (!receivedReportFrom.Keys.Any(key => key == messageId) && processRound == messageRound)
            {
                receivedReportFrom.Add(messageId, new Attribute { Value = messageSnapshot });
                processMessagesArived.Add(messageId, new Attribute { Value = listOfMessagesArrived });
                if (initiator)
                {
                    processWeight += messageWeight;
                    if (processWeight == 1)
                    {
                        // Add the initiator's snapshot to the snapshot list
                        receivedReportFrom.Add(processId, new Attribute { Value = processSnapshot });
                        // Collect the messages from the channels
                        AttributeList messages = new AttributeList();
                        foreach (ChandyLamportChannel channel in InChannels())
                        {
                            messages.Concat(channel.ReportChannelMessages());
                        }
                        processMessagesArived.Add(processId, new Attribute { Value = messages });

                        // Report the snapshots
                        PrintSnapshot(processRound, receivedReportFrom, processMessagesArived);

                        // Start a new round 
                        processRound++;
                        if (processRound < maxRound)
                        {
                            InitNewRound(processRound);
                        }
                        else
                        {
                            Terminate();
                        }
                    }
                    else
                    {
                        // Set the attributes for the initiator
                        OperationResults.Value[OperationResultKeys.Weight].Value = processWeight;
                        OperationResults.Value[OperationResultKeys.ReceivedReportFrom].Value = receivedReportFrom;
                    }
                }
                else
                {
                    SendToNeighbours(report, null);
                    OperationResults.Value[OperationResultKeys.ReceivedReportFrom].Value = receivedReportFrom;
                }
            }
        }

        private void PrintSnapshot(int round, AttributeDictionary snapshots, AttributeDictionary messages)
        {
            List<int> keys = snapshots.Select(entry => (int)(entry.Key)).OrderBy(key => key).ToList();
            AttributeDictionary orderedSnapshots = new AttributeDictionary();
            AttributeDictionary orderedMessages = new AttributeDictionary();
            string s = "Results of round " + round.ToString();
            foreach (int key in keys)
            {
                s += "\n Processor " + key.ToString() + ":" + snapshots[key].Value.ToString();
                s += "  ";
                foreach (Attribute attribute in (AttributeList)messages[key].Value)
                {
                    s += attribute.Value + ", ";
                }
            }
            MessageRouter.MessageBox(new List<string> { s }, "Report of end snapshot", null, Icons.Info);
        }

        public void InitNewRound(int round)
        {
            OperationResults.Value[OperationResultKeys.Recorded].Value = false;
            InChannels().ForEach(c => c[ElementDictionaries.OperationResults].Value[ChandyLamportChannel.OperationResultKeys.MarkerArrived].Value = false);
            BaseMessage marker = CreateFirstMarker(round);
            TakeSnapshot(marker);
        }

        private void TakeSnapshot(BaseMessage message)
        {
            bool recorded = OperationResults.Value[OperationResultKeys.Recorded].Value;
            double weight = message.GetField(ChandyLamportMessage.FieldKeys.Weight).Value;
            int round = message.GetHeaderField(BaseMessage.HeaderFieldKeys.Round).Value;
            if (!recorded)
            {
                OperationResults.Value[OperationResultKeys.Recorded].Value = true;
                double messageWeight = weight / (2 *(OutGoingChannels.Count - 1));
                message.GetField(ChandyLamportMessage.FieldKeys.Weight).Value = messageWeight;
                OperationResults.Value[OperationResultKeys.RoundNum].Value = round;                
                SendToNeighbours(message, null);
                OperationResults.Value[OperationResultKeys.Snapshot].Value = round;
                OperationResults.Value[OperationResultKeys.Weight].Value = weight / 2;
                OperationResults.Value[OperationResultKeys.ReceivedReportFrom].Value = new AttributeDictionary();
                OperationResults.Value[OperationResultKeys.MessagesReceived].Value = new AttributeDictionary();
                InChannels().ForEach(c => ((ChandyLamportChannel)c).ClearMessages());
            }
        }


        #endregion
 
        #region Presentation (methods used to update the presentation)

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

        #endregion
        #region algorithm utility methods

        private BaseMessage CreateFirstMarker(int round)
        {
            BaseMessage message = new BaseMessage(ChandyLamportMessage.MessageTypes.Marker, 0, 0, 0, 0, round, 0);
            message.AddField(ChandyLamportMessage.FieldKeys.Weight, new Attribute { Value = 1 });
            return message;
        }

        private BaseMessage CreateReport()
        {
            // Get the parameters of the processor
            int round = OperationResults.Value[OperationResultKeys.RoundNum].Value;
            int id = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            int snapshot = OperationResults.Value[OperationResultKeys.Snapshot].Value;
            double weight = OperationResults.Value[OperationResultKeys.Weight].Value;

            // Collect the messages from the channels
            AttributeList messages = new AttributeList();
            foreach(ChandyLamportChannel channel in InChannels())
            {
                messages = messages.Concat(channel.ReportChannelMessages()).ToList().ToAttributeList();
            }

            // Build the message
            BaseMessage message = new BaseMessage(ChandyLamportMessage.MessageTypes.Report, 0, 0, 0, 0, round, 0);
            message.AddField(ChandyLamportMessage.FieldKeys.Id, new Attribute { Value = id });
            message.AddField(ChandyLamportMessage.FieldKeys.Snapshots, new Attribute { Value = snapshot });
            message.AddField(ChandyLamportMessage.FieldKeys.Weight, new Attribute { Value = weight });
            message.AddField(ChandyLamportMessage.FieldKeys.MessageSnapshot, new Attribute { Value = messages, IncludedInShortDescription = false });
            return message;
        }

        private void UpdatePresentation()
        {
            foreach (ChandyLamportChannel channel in network.Channels)
            {
                channel.UpdatePresentation();
            }
            string presentationText = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value.ToString();
            presentationText += "\n Round = " + OperationResults.Value[OperationResultKeys.RoundNum].Value.ToString();
            int snapshot = OperationResults.Value[OperationResultKeys.Snapshot].Value;
            double weight = OperationResults.Value[OperationResultKeys.Weight].Value;
            AttributeDictionary receivedReportFrom = OperationResults.Value[OperationResultKeys.ReceivedReportFrom].Value;
            presentationText += "\n" + snapshot.ToString() + " " + weight.ToString() + "\n" + receivedReportFrom.ToString();            
            PresentationParameters[PresentationParameterKeys.Text].Value = presentationText;
            MessageRouter.ReportMessage(GetProcessDefaultName(), Logger.VectorClockString(), "Snapshot - " + snapshot.ToString());
        }
        #endregion
    }
}
