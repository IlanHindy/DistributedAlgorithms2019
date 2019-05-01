////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\ElementDebugWindow.cs
///
///\brief   Implements the element debug Windows Form.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DistributedAlgorithms.Algorithms.Base.Base;
//using System.Diagnostics;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// \namespace DistributedAlgorithms
///
/// \brief .
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ElementDebugWindow
    ///
    /// \brief Form for viewing and changing the element attributes while running.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 04/07/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ElementDebugWindow : ElementWindow
    {
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ElementDebugWindow(List<NetworkElement> networkElements) :base(networkElements, InputWindows.DebugWindow)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param networkElements (List&lt;NetworkElement&gt;) - The network elements.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ElementDebugWindow(List<NetworkElement> networkElements)
            :base(networkElements, InputWindows.DebugWindow)
        {
            InitWindow();
        }
        #endregion
        #region /// \name Window building

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override bool ScanCondition(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Scans a condition.
        ///
        /// \par Description.
        ///      The dictionaries presented;
        ///      -# ElementAttributes
        ///      -# PrivateAttribute
        ///      -# OperationResults.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ScanCondition(dynamic key, 
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            if (mainDictionary == NetworkElement.ElementDictionaries.PresentationParametersBackup)
            {
                return false;
            }
            if (mainDictionary == NetworkElement.ElementDictionaries.OperationResultsBackup)
            {
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected override void CreateButtons()
        ///
        /// \brief Creates the buttons.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void CreateButtons()
        {
            CreateButton("Button_ResetToExisting", "Reset To Existing Values", Button_ResetToExisting_Click);
            CreateButton("Button_UpdateNetworkElement", "Update Network Element", Button_UpdateNetworkElement_Click);
            if (typeof(BaseProcess).IsAssignableFrom(networkElements[0].GetType()))
            {
                CreateButton("Button_SendMessage", "Send Message", Button_SendMessage_Click);
            }
            CreateButton("Button_Breakpoints", "Breakpoints", Button_Breakpoints_Click);
            CreateButton("Button_Exit", "Exit", Button_Exit_Click);
        }
        #endregion
        #region /// \name Buttons handelers 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_UpdateNetworkElement_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_UpdateNetworkElement for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_UpdateNetworkElement_Click(object sender, RoutedEventArgs e)
        {
            UpdateNetworkElement();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_ResetToExisting_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ResetToExisting for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_ResetToExisting_Click(object sender, RoutedEventArgs e)
        {
            ResetToExisting();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_SendMessage for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            ///*
            // * Find the link of the message queue
            // */
            //Attribute messageQueueAttribute = networkElements[0].or[bp.ork.MessageQ];
            //ControlsAttributeLink messageQueueLink = controlsAttributeLinks.First(entry => entry.Value.attribute == messageQueueAttribute).Value;

            ///*
            // * Activate the message editing window
            // */
            //BaseMessage newMessage = CreateDefaultMessage();
            //SendMessageWindow sendMessageWindow = new SendMessageWindow(new List<NetworkElement>() {newMessage}, messageQueueAttribute.Value);
            //sendMessageWindow.ShowDialog();
            //switch (sendMessageWindow.OperationNeeded)
            //{
            //    case SendMessageWindow.OperationsNeeded.Send:
            //        ((BaseProcess)networkElements[0]).MessageQHandling(ref newMessage, BaseProcess.MessageQOperation.AddMessage, sendMessageWindow.InsertionIndex);
            //        break;
            //    case SendMessageWindow.OperationsNeeded.SendWhenApply:
            //        AddMessageToNewValues(newMessage, messageQueueLink, sendMessageWindow.InsertionIndex);
            //        break;
            //}
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private BaseMessage CreateDefaultMessage()
        ///
        /// \brief Creates default message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \return The new default message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private BaseMessage CreateDefaultMessage()
        {
            /*
             * Select the source process
             */
            List<BaseProcess> sourceProcesses = new List<BaseProcess>() ;
            networkElements[0].Network.Processes.ForEach(process => { if (networkElements[0] != process) sourceProcesses.Add(process); });
            List<string> sourceProcessesNames = new List<string>();
            sourceProcesses.ForEach(process => sourceProcessesNames.Add(process.ToString()));
            SelectDialog sourceProcessSelectDialog = new SelectDialog("Select the source process of the message ", "Source process select dialog", sourceProcessesNames);
            sourceProcessSelectDialog.ShowDialog();
            BaseProcess sourceProcess = sourceProcesses[sourceProcessSelectDialog.Selection[0]];
            BaseChannel sendingChannel = sourceProcess.OutGoingChannels.First(channel => channel.ea[bc.eak.DestProcess] == networkElements[0].ea[ne.eak.Id]);
            BaseMessage message = new BaseMessage(networkElements[0].Network, bm.MessageTypes.EmptyMessage,
                sendingChannel,
                "", 
                0,
                0);
            return message;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void AddMessageToNewValues(BaseMessage message, ControlsAttributeLink messageQueueLink, int insertionIndex)
        ///
        /// \brief Adds a message to new values.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param message          (BaseMessage) - The message.
        /// \param messageQueueLink (ControlsAttributeLink) - The message queue link.
        /// \param insertionIndex   (int) - Zero-based index of the insertion.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddMessageToNewValues(BaseMessage message, ControlsAttributeLink messageQueueLink, int insertionIndex)
        {
            //Create item for the message
            currentComplexItem = messageQueueLink;
            ControlsAttributeLink newLink = CreateNewItem(new Attribute() {Value = message}, "New", NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None, insertionIndex);

            //Scan the message to create the tree of the message 
            scanProcess = ScanProcess.AdditionOfNetworkElement;
            currentComplexItem = newLink;
            message.ScanAndReport(this, newLink.attributeDictionary, newLink.attributeDictionary);
            ((TreeViewItem)newLink.item).IsExpanded = false;

            //Update the window
            UpdateStatus((ItemsControl)newLink.item, UpdateStatuses.Added);
            UpdateIndexInParent(messageQueueLink.item);
            RearrangeFields(true);
            ((TreeViewItem)messageQueueLink.item).IsExpanded = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Breakpoints_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Breakpoints for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Breakpoints_Click(object sender, RoutedEventArgs e)
        {
            BaseProcess process = (BaseProcess)networkElements[0];
            BaseNetwork network = process.Network;
            BaseChannel channel = null;
            BaseMessage message = null;
            AttributeList messageQueue = process.or[bp.ork.MessageQ];
            List<NetworkElement> breakpoints = new List<NetworkElement>();
            breakpoints.Add(network.op[bn.opk.Breakpoints]);
            breakpoints.Add(process.ea[bp.eak.Breakpoints]);
            if (messageQueue.Count > 0)
            {
                message = messageQueue[0];
                channel = process.IncommingChannels.First(c => c.ea[bc.eak.SourceProcess]
                == message.GetHeaderField(bm.pak.SourceProcess));                
                breakpoints.Add(channel.op[bn.opk.Breakpoints]);
                breakpoints.Add(message.ea[bp.eak.Breakpoints]);
            }
            //BreakpointWindow breakpointWindow = new BreakpointWindow(BreakpointWindow.ProgramStatus.Running, breakpoints, network, process, channel, message);
            //breakpointWindow.Show();
        }                    

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Exit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Exit for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }
        #endregion
    }
}
