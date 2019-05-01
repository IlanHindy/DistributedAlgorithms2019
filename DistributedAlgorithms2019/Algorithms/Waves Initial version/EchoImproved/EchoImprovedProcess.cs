////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Algorithms\System\EchoImproved\EchoImprovedProcess.cs
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

namespace DistributedAlgorithms.Algorithms.Waves.EchoImproved
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EchoImprovedProcess
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

    class EchoImprovedProcess : BaseProcess
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum PrivateAttributeKeys
        ///
        /// \brief Values that represent private attribute keys. Private attributes are attributes of
        /// the process that do not change during operation Insert.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum PrivateAttributeKeys { }

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

        public new enum OperationResultKeys { Received, ParentId, ProcessingStage }
        private enum ProcessingStage { Init, WaitingForNeighbours, Finished }

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
            OperationResults.Value.Add(OperationResultKeys.Received, new Attribute() { Value = 0 });
            OperationResults.Value.Add(OperationResultKeys.ParentId, new Attribute() { Value = -1 });
            OperationResults.Value.Add(OperationResultKeys.ProcessingStage, new Attribute() { Value = ProcessingStage.Init });
        }

        #endregion
        #region Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EchoImprovedProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
        ///     -# `attribute.EndInputOperationMethod =  EchoImprovedProcessDefaultAttributeCheck`
        ///     -# `PrivateAttributes.Value.Add(PrivateAttributeKeys.SomeKey, new Attribute {Value = SomeValue,
        ///         EndInputOperationMethod =  EchoImprovedProcessDefaultAttributeCheck})`
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

        public static bool EchoImprovedProcessDefaultAttributeCheck(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage)
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
            //base.RunAlgorithm();
            SendToNeighbours(EchoImprovedMessage.MessageTypes.Wave, new AttributeDictionary(), new List<int>(), 0, 0);
            OperationResults.Value[OperationResultKeys.ProcessingStage].Value = ProcessingStage.WaitingForNeighbours;
            SetPresentation();
        }

        protected void SendMessage(int destId)
        {
            BaseChannel channel = OutGoingChannels.First(c => c[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value == destId);
            EchoImprovedMessage message = new EchoImprovedMessage(EchoImprovedMessage.MessageTypes.Wave, channel, 0, 0);
            Send(channel[ElementDictionaries.ElementAttributes].Value[BaseChannel.ElementAttributeKeys.DestProcess].Value, message);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void ReceiveHandling(EchoImprovedMessage message)
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

            //base.ReceiveHandling(message);

            // Get the values from the attributes
            int received = OperationResults.Value[OperationResultKeys.Received].Value;
            int parentId = OperationResults.Value[OperationResultKeys.ParentId].Value;
            bool initiator = ElementAttributes.Value[BaseProcess.ElementAttributeKeys.Initiator].Value;
            ProcessingStage processingStage = OperationResults.Value[OperationResultKeys.ProcessingStage].Value;

            // Get the values from the message
            int senderId = message[ElementDictionaries.ElementAttributes].Value[EchoImprovedMessage.HeaderFieldKeys.SourceProcess].Value;

            // Implement the algorithm
            received++;

            // Handle initializing of none initiator
            if (parentId == -1 && initiator == false)
            {
                parentId = senderId;

                // If the process is a leaf send return message 
                // Else send message to all neighbours
                if (received < OutGoingChannels.Count)
                {
                    SendToNeighbours(message, new List<int>() { parentId });
                    processingStage = ProcessingStage.WaitingForNeighbours;

                }
                else
                {
                    SendMessage(parentId);
                    processingStage = ProcessingStage.Finished;
                }
            }

            // Handle end algorithm detection and processing
            else
            {
                if (received == OutGoingChannels.Count - 1)
                {
                    if (initiator == false)
                    {
                        SendMessage(parentId);
                        processingStage = ProcessingStage.Finished;
                    }
                    else
                    {
                        Terminate();
                        processingStage = ProcessingStage.Finished;
                    }
                }
            }

            // Setting the attributes
            OperationResults.Value[OperationResultKeys.Received].Value = received;
            OperationResults.Value[OperationResultKeys.ParentId].Value = parentId;
            OperationResults.Value[OperationResultKeys.ProcessingStage].Value = processingStage;
            SetPresentation();
        }

        private void SetPresentation()
        {
            int received = OperationResults.Value[OperationResultKeys.Received].Value;
            int parentId = OperationResults.Value[OperationResultKeys.ParentId].Value;
            ProcessingStage processingStage = OperationResults.Value[OperationResultKeys.ProcessingStage].Value;
            switch (processingStage)
            {
                case ProcessingStage.Init:
                    PresentationParameters[PresentationParameterKeys.Foreground].Value = KnownColor.Black;
                    break;
                case ProcessingStage.WaitingForNeighbours:
                    PresentationParameters[PresentationParameterKeys.Foreground].Value = KnownColor.DarkGreen;
                    break;
                case ProcessingStage.Finished:
                    PresentationParameters[PresentationParameterKeys.Foreground].Value = KnownColor.Blue;
                    break;
            }
            string presenationString = ElementAttributes.Value[NetworkElement.ElementAttributeKeys.Id].Value.ToString() + "\n" +
                "Received : " + received.ToString() + "\n" +
                "Parent : " + parentId.ToString();
            PresentationParameters[PresentationParameterKeys.Text].Value = presenationString;
        }

        #endregion
        #region Running utility methods (methods used while in running phase)

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
        #endregion
    }
}
