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
using DistributedAlgorithms.Algorithms.System.Base;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Waves.Echo
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoProcess
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

    class EchoProcess : BaseProcess
    {
        #region Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrivateAttributeKeys
        ///
        /// \brief Values that represent private attribute keys. Private attributes are attributes of
        /// the process that do not change during operation Insert.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrivateAttributeKeys { NumberOfRounds }

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

        public new enum OperationResultKeys { Status, Round }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Status
        ///
        /// \brief Values that represent status.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Status { SentForwaredMessages }
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
            PrivateAttributes.Value.Add(PrivateAttributeKeys.NumberOfRounds, new Attribute { Value = 1 });
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
            OperationResults.Value.Add(OperationResultKeys.Round, new Attribute { Value = 0, Editable = false });
            OperationResults.Value.Add(OperationResultKeys.Status, new Attribute { Value = Status.SentForwaredMessages, Editable = false });
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
        ///                                 BaseMessage.MessageTypes.BaseMessage,  // *The message that was sent/received
        ///                                 0,                                     // The round on the message
        ///                                 1,                                     // The source/destination of the message
        ///                                 BaseMessage.MessageTypes.BaseMessage,  // *The type of the message that will be sent
        ///                                 "m1",                                  // The name of the message that will be sent
        ///                                 new AttributeList { new Attribute { Value = 1 } }); // A list of destinations for the message
        /// }
        /// ~~~
        /// * You can chane this parameter to a message type of your algorithm  
        ///
        /// \author Ilan Hindy
        /// \date 28/02/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitBaseAlgorithmData()
        {
            int processId = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value;
            switch (processId)
            {
            }
        }

        #endregion
        #region Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EchoProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///     -# `attribute.EndInputOperationMethod =  EchoProcessDefaultAttributeCheck`
        ///     -# `PrivateAttributes.Value.Add(PrivateAttributeKeys.SomeKey, new Attribute {Value = SomeValue,
        ///         EndInputOperationMethod =  EchoProcessDefaultAttributeCheck})`
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

        public static bool EchoProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
            if (ElementAttributes.Value[BaseProcess.ElementAttributeKeys.Initiator].Value == true)
            {
                //Change the status to SentForwaredMessages
                OperationResults.Value[OperationResultKeys.Status].Value = Status.SentForwaredMessages;
                OperationResults.Value[OperationResultKeys.Round].Value = 1;
                SendToAllNeighbours(EchoMessage.MessageTypes.Forewored, false);
            }
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
            //Logger.Log(Logger.LogMode.MainLogAndProcessLogAndMessageTrace, ToString(), "ForeworedReceived()", "DEBUG1", message);
            if (message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.MessageType].Value == EchoMessage.MessageTypes.Forewored)
            {
                ForeworedReceived(message);
                return;
            }
            if (message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.MessageType].Value == EchoMessage.MessageTypes.Backwored)
            {
                BackworedReceved(message);
                return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ForeworedReceived(BaseMessage message)
        ///
        /// \brief Forewored received.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param message The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ForeworedReceived(BaseMessage message)
        {
            Logger.Log(Logger.LogMode.MainLogAndProcessLogAndMessageTrace, GetProcessDefaultName(), "ForeworedReceived()", "Message round = " + message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.Round].Value + " Process round = " + OperationResults.Value[OperationResultKeys.Round].Value.ToString(), message);
            if (message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.Round].Value > OperationResults.Value[OperationResultKeys.Round].Value)
            {
                //Change the round of the process
                OperationResults.Value[OperationResultKeys.Round].Value = message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.Round].Value;

                //Mark the father's channel
                BaseChannel fatherOutChannel = OutGoingChannels.First(channel => channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value == message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.SourceProcess].Value);
                fatherOutChannel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value = EchoChannel.Status.Father;

                //Send forwared messages to all the rest of the channels
                SendToAllNeighbours(EchoMessage.MessageTypes.Forewored, false);

                //If the process has no suns - esend a backword message
                int count = OutGoingChannels.Count(channel => channel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value == EchoChannel.Status.SonSent);
                if (count == 0)
                {
                    //Send backwored message to processor that sent the message
                    SendBackworedMessage(message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.SourceProcess].Value);
                }
            }
            else
            {
                //Send backwored message to processor that sent the message
                SendBackworedMessage(message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.SourceProcess].Value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void BackworedReceved(BaseMessage message)
        ///
        /// \brief Backwored receved.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param message The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void BackworedReceved(BaseMessage message)
        {
            //Mark the outgoing channel 
            BaseChannel receiveOutGoingChannel = OutGoingChannels.First(channel => channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value == message[ElementDictionaries.ElementAttributes].Value[BaseMessage.HeaderFieldKeys.SourceProcess].Value);
            receiveOutGoingChannel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value = EchoChannel.Status.Null;

            //Check if all the Outgoing channels that represent a son got answer
            int count = OutGoingChannels.Count(channel => channel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value == EchoChannel.Status.SonSent);
            if (count == 0)
            {
                if (ElementAttributes.Value[BaseProcess.ElementAttributeKeys.Initiator].Value == true)
                {
                    if (OperationResults.Value[OperationResultKeys.Round].Value == PrivateAttributes.Value[PrivateAttributeKeys.NumberOfRounds].Value)
                    {
                        //SendToAllNeighbours("Finish", true);
                        Terminate();
                    }
                    else
                    {
                        int round = OperationResults.Value[OperationResultKeys.Round].Value;
                        OperationResults.Value[OperationResultKeys.Round].Value = round + 1;
                        SendToAllNeighbours(EchoMessage.MessageTypes.Forewored, true);
                    }
                }
                else
                {
                    //Send backwored message through the father's channel
                    BaseChannel fatherChannel = OutGoingChannels.First(channel => channel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value == EchoChannel.Status.Father);
                    SendBackworedMessage(fatherChannel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value);
                }
            }
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

        protected override BaseMessage BuildBaseMessage(object messageType, string messageName, AttributeDictionary messageFields, int round, AttributeList targets)
        {
            return base.BuildBaseMessage(messageType, messageName, messageFields, round, targets);
        }
        #endregion
        #region Running utility methods (methods used while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SendToAllNeighbours(EchoMessage.MessageTypes messageType, bool ckeckForSocketConnection)
        ///
        /// \brief Sends to all neighbours.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param messageType              Type of the message.
        /// \param ckeckForSocketConnection True to ckeck for socket connection.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SendToAllNeighbours(EchoMessage.MessageTypes messageType, bool ckeckForSocketConnection)
        {
            foreach (BaseChannel channel in OutGoingChannels)
            {
                if (channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value != ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value)
                {
                    if (channel.IsSendingSocketConnected() == true || !ckeckForSocketConnection)
                    {
                        if (channel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value != EchoChannel.Status.Father)
                        {
                            BaseMessage message = new BaseMessage(messageType, channel, OperationResults.Value[OperationResultKeys.Round].Value, 0);
                            Send(channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value, message);
                            channel[ElementDictionaries.OperationResults].Value[EchoChannel.OperationResultKeys.Status].Value = EchoChannel.Status.SonSent;
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SendBackworedMessage(int destProcess)
        ///
        /// \brief Sends a backwored message.
        ///
        /// \brief #### Description.
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 12/03/2017
        ///
        /// \param destProcess Destination process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SendBackworedMessage(int destProcess)
        {
            BaseChannel senderOutgoingChannel = OutGoingChannels.First(channel => channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value == destProcess);
            BaseMessage backworedMessage = new BaseMessage(EchoMessage.MessageTypes.Backwored, senderOutgoingChannel, OperationResults.Value[OperationResultKeys.Round].Value, 0);
            Send(destProcess, backworedMessage);
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
