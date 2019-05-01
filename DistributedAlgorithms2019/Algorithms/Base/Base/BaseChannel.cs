////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\Channel.cs
///
///\brief   Implements the channel class.
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
using DistributedAlgorithms;
using System.Drawing;

namespace DistributedAlgorithms.Algorithms.Base.Base
{
    #region /// \name Enums

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class bc
    ///
    /// \brief A bc.
    ///
    /// \par Description.
    ///      This class holds the enums decelerations of BaseChannel
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 25/06/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class bc
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum eak
        ///
        /// \brief Keys to be used in the ElementAttributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum eak { SourceProcess, DestProcess }

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

        public enum opk { Breakpoints}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ork
        ///
        /// \brief Keys to be used in the OperationResults dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ork { SourcePort, DestPort, MessageReceived, TerminationStatus}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ppk
        ///
        /// \brief Keys to be used in the PresentationParameters dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ppk
        {
            LineColor, HeadColor, FrameColor, FrameWidth, Background, Foreground, PresentationTxt,
            MessagesFrameColor, MessagesFrameWidth, MessagesBackground, MessagesForeground
        }
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BaseChannel
    ///
    /// \brief A base channel.
    /// -   Represent a directed connection between 2 processes
    /// -   The private members includs the sending socket and AsynchronousReader that processes
    ///     the actual communication between the 2 processes
    ///
    /// \par Description.
    ///      when working there are the following phases
    ///      -#  Init    - create a new channel (while creating a new network)
    ///      -#  Design  - Designe the network and fill it's parameters
    ///      -#  Check   - Check if the network design is leagal according to the algorithm specifications
    ///      -#  Create  - Create the network from the software point of view
    ///      -#  Run     - Run the algorithm
    ///      -#  Presentation - update the presentation while running
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilan Hindy
    /// \date 08/03/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class BaseChannel : NetworkElement
    {
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum TerminationStatus
        ///
        /// \brief Values that represent termination status.(For the algorithem termination)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum TerminationStatuses { NotTerminated, Terminated }
        #endregion
        #region /// \name Member Variables

        
        /// \brief The sending socket.
        public Socket sendingSocket = null;
        
        /// \brief The asynchronous reader.
        public AsynchronousReader asynchronousReader = null;

        
        /// \brief Identifier for the temporary source.
        private int tmpSourceId;

        
        /// \brief Identifier for the temporary destination.
        private int tmpDestId;
        #endregion
        #region /// \name constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseChannel() : base(true)
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

        public BaseChannel() : base(true) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseChannel(BaseNetwork network) : base(network)
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

        public BaseChannel(BaseNetwork network) : base(network) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseChannel(bool PermissionsValue) : base(PermissionsValue)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      See explanation about constructors in NetworkElement.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/06/2017
        ///
        /// \param PermissionsValue (bool) - true to permissions value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel(bool PermissionsValue) : base(PermissionsValue) { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseChannel(BaseNetwork network, int id, int sourceProcessId, int destProcessId) : base(network)
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
        /// \param network          (BaseNetwork) - The network.
        /// \param id               (int) - The identifier.
        /// \param sourceProcessId  (int) - Identifier for the source process.
        /// \param destProcessId    (int) - Identifier for the destination process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseChannel(BaseNetwork network, int id, int sourceProcessId, int destProcessId) : base(network)
        {
            tmpSourceId = sourceProcessId;
            tmpDestId = destProcessId;
            Init(id, false);
        }

        #endregion
        #region /// \name Init (methods that are activated while in Init phase)

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
        /// \date 08/03/2017
        ///
        /// \param idx  (int) - The index.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override sealed void InitElementAttributes(int idx)
        {
            base.InitElementAttributes(idx);
            ea.Add(bc.eak.SourceProcess, new Attribute { Value = tmpSourceId, Editable = false, Changed=true });
            ea.Add(bc.eak.DestProcess, new Attribute { Value = tmpDestId, Editable = false, Changed=true });
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
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override sealed void InitPresentationParameters()
        {
            base.InitPresentationParameters();
            pp.Add(bc.ppk.LineColor, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bc.ppk.HeadColor, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bc.ppk.FrameColor, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bc.ppk.FrameWidth, new Attribute { Value = 1, Changed = false });
            pp.Add(bc.ppk.Background, new Attribute { Value = KnownColor.White, Changed = false });
            pp.Add(bc.ppk.Foreground, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bc.ppk.PresentationTxt, new Attribute { Value = ea[ne.eak.Id].ToString(), Changed = false });
            pp.Add(bc.ppk.MessagesFrameColor, new Attribute { Value = KnownColor.Black, Changed = false });
            pp.Add(bc.ppk.MessagesFrameWidth, new Attribute { Value = 1, Changed = false });
            pp.Add(bc.ppk.MessagesBackground, new Attribute { Value = KnownColor.White, Changed = false });
            pp.Add(bc.ppk.MessagesForeground, new Attribute { Value = KnownColor.Black, Changed = false });
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
            op.Add(bc.opk.Breakpoints, new Attribute { Value = new Breakpoint(Breakpoint.HostingElementTypes.Channel), Editable = false, IncludedInShortDescription = false, Changed = false });
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
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void InitOperationResults()
        {
            base.InitOperationResults();
            or.Add(bc.ork.SourcePort, new Attribute {Value = 0, Editable = false, IncludedInShortDescription = false, Changed = false });
            or.Add(bc.ork.DestPort,  new Attribute {Value = 0, Editable = false, IncludedInShortDescription = false, Changed = false });
            or.Add(bc.ork.TerminationStatus, new Attribute { Value = TerminationStatuses.NotTerminated, Editable = false, IncludedInShortDescription = false, Changed = false });
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Connect(int sourceProcess, int destProcess)
        ///
        /// \brief Connects.
        ///
        /// \par Description.
        ///      Set the channel (before the running) to connect between 2 processes.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param sourceProcess  (int) - Source process.
        /// \param destProcess    (int) - Destination process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Connect(int sourceProcess, int destProcess)
        {
            ea[bc.eak.SourceProcess] = sourceProcess;
            ea[bc.eak.DestProcess] = destProcess;
        }

        #endregion
        #region /// \name Check

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///
        /// \par Description.
        ///      Perform the check after build.
        ///      The following are the checks :
        ///      -# if the network is not directed the channel has to have a reverse channel.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            CheckBackwordChannel(buildCorrectionsParameters);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CheckBackwordChannel(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check reverse channel.
        ///
        /// \par Description.
        ///      If the network is not directed and the channel is not a self
        ///      connecting channel of one of the processes The channel has to have a reverse channel.
        ///      To allow 2 directions connections.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param buildCorrectionsParameters  (List&lt;BuildCorrectionParameters&gt;) - Options for controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CheckBackwordChannel(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
            if (network.ea[bn.eak.DirectedNetwork] == true)
            {
                return;
            }
            else
            {
                //Check if the channel has a backwored channel
                //Exclude the self connecting channels
                if (ea[bc.eak.SourceProcess] == ea[bc.eak.DestProcess])
                {
                    return;
                }
                if (CheckIfBackwordChannelNeeded(network))
                {
                    BuildCorrectionParameters newCorrection = new BuildCorrectionParameters();
                    newCorrection.ErrorMessage = "There is no backwored channel for channel : " + ToString();
                    newCorrection.CorrectionMethod = CreateBackworedChannel;
                    newCorrection.CorrectionParameters = network;
                    buildCorrectionsParameters.Add(newCorrection);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool CheckIfBackwordChannelNeeded(BaseNetwork net)
        ///
        /// \brief Determine if reverse channel needed.
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
        /// \param net  (BaseNetwork) - The net.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CheckIfBackwordChannelNeeded(BaseNetwork net)
        {
            //Check if the channel has a backwored channel
            //Exclude the self connecting channels
            if (ea[bc.eak.SourceProcess] == ea[bc.eak.DestProcess])
            {
                return false;
            }
            if (net.Channels.Any(otherChannel => otherChannel.ea[bc.eak.SourceProcess] == ea[bc.eak.DestProcess] &&
                otherChannel.ea[bc.eak.DestProcess] == ea[bc.eak.SourceProcess]) == true)
            {
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CreateBackworedChannel(object parameters)
        ///
        /// \brief Creates backwored channel.
        ///
        /// \par Description.
        ///      - This method is the correction for the check if a backword channel is needed
        ///      - This method is used also when the network changes (In design phase) to not directed
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \param parameters  (object) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateBackworedChannel(object parameters)
        {
            BaseNetwork network = (BaseNetwork)parameters;
            int channelId = network.Channels.Last().ea[ne.eak.Id] + 1;
            BaseChannel channel = ClassFactory.GenerateChannel(network, channelId, ea[bc.eak.DestProcess], ea[bc.eak.SourceProcess]);
            ChannelPresentation presentation = (ChannelPresentation)Presentation;
            channel.Presentation = presentation;
            presentation.AddChannel(channel);
            network.Channels.Add(channel);
        }
        #endregion
        #region /// \name Running Termination of the network

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Terminate()
        ///
        /// \brief Terminates this object.
        ///
        /// \par Description.
        ///      This method is activated by the process to close the sending socket as part of the termination
        ///      of an algorithm
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Terminate()
        {
            if (sendingSocket != null)
            {
                //Release the socket.
                sendingSocket.Shutdown(SocketShutdown.Both);
                sendingSocket.Close();
                sendingSocket = null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool IsSendingSocketConnected()
        ///
        /// \brief Query if this object is sending socket connected.
        ///
        /// \par Description.
        /// Check if the sending socket is still connected this is done
        /// In order to avoid sending to terminated process
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \return True if sending socket connected, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsSendingSocketConnected()
        {
            if (sendingSocket == null)
            {
                return false;
            }
            else
            {
                return AsynchronousSender.IsSocketConnected(sendingSocket);
            }
        }
        #endregion
        #region /// \name Presentation (methods used to update the presentation)

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
        /// \date 08/03/2017
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            return "Id:" + ea[ne.eak.Id].ToString() + " From:" + ea[bc.eak.SourceProcess].ToString() + " To:" + ea[bc.eak.DestProcess].ToString();
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
        /// \brief #### Usage Notes.
        ///
        /// \author Ilan Hindy
        /// \date 08/03/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string PresentationText()
        {
            return ea[ne.eak.Id].ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override sealed void UpdateRunningStatus(object[] parameters)
        ///
        /// \brief Updates the running status described by parameters.
        ///
        /// \par Description.
        ///      - This method is activated after an end of running step 
        ///      - It generate the string of the incomming messages waiting on this channel  
        ///      - It then send a message to the MainWindow to update the presentation of the channel
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/04/2017
        ///
        /// \param parameters  (object[]) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override sealed void UpdateRunningStatus(object[] parameters)
        {
            //string incommingMessageString = "";
            //foreach (BaseMessage message in (List<BaseMessage>)parameters[0])
            //{
            //    incommingMessageString += message.Description() + "\n";
            //}
            //MessageRouter.ReportChangePresentationOfComponent(this, new object[] { incommingMessageString });

            List<string> messagesStrings = new List<string>();
            foreach (BaseMessage message in (List<BaseMessage>)parameters[0])
            {
                messagesStrings.Add(message.Description());
            }
            MessageRouter.ReportChangePresentationOfComponent(this, new object[] { messagesStrings });
        }

        #endregion
    }

}

