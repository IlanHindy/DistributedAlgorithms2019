////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Network Elements Components\MessageQ.cs
///
/// \brief Implements the message q class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    #region /// \name Enums
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \enum MessageQOperation
    ///
    /// \brief Values that represent message q operations.
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum MessageQOperation { AddMessage, RetrieveFirstMessage, RemoveFirstMessage, EmptyQueue, Init, BlockMessageProcessing, ReleaseMessageProcessing }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class MessageQ
    ///
    /// \brief A message q.
    ///
    /// \par Description.
    ///      -  The MessageQ holds a list of messages arrived to a process  
    ///      -  A message is received by an AsynchronousReader  
    ///      -  There is an AsynchronousReader for each incoming channel  
    ///      -  When a message is received by an AsynchronousReader it locks the MessageQ   
    ///         (To avoid concurrent insert of messages) and insert the message to the Q
    ///      -  The getting of the messages by the process is done in 2 steps  
    ///         -#  Retrieve the message
    ///         -#  Remove the message from the Q when the process is sure that it wants to process it
    ///      -  The MessageQ contains ManualResetEvent that is set when the Q is not empty and  
    ///         reset when the Q is empty. The process processing is done in loop. The loop waits
    ///         when the Q is empty
    ///      
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 30/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class MessageQ:AttributeList
    {
        #region /// \name Members
        /// \brief  (BaseProcess) - The process.
        BaseProcess process;

        /// \brief  (ManualResetEvent) - The message q event.
        ///         -   This event is set when the MessageQ is not empty.
        ///         -   The event is used to make the process thread receive loop wait when there is no messages in the Q
        private ManualResetEvent MessageQEvent = new ManualResetEvent(false);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief The message queue lock. This lock is used to lock the message queue
        ///        The lock is needed because many (all the reading threads) are accessing the same queue.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private object MessageQLock = new object();
        #endregion
        #region /// \name Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public MessageQ():base()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MessageQ():base()
        {

        }
        #endregion
        #region /// \name Set members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true)
        ///
        /// \brief Sets the members.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ///
        /// \param network      (BaseNetwork) - The network.
        /// \param element      (NetworkElement) - The element.
        /// \param permissions  (Permissions) - The permissions.
        /// \param parent       (IValueHolder) - The parent.
        /// \param recursive   (Optional)  (bool) - true to process recursively, false to process locally only.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true)
        {
            base.SetMembers(network, element, permissions, parent);
            process = (BaseProcess)Element;
        }
        #endregion
        #region /// \name MessageQ operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void MessageQHandling(ref BaseMessage message, MessageQOperation operation, int additionIndex = -1)
        ///
        /// \brief Message q handling.
        ///
        /// \par Description.
        ///      -  This is the entry point to the MessageQ operations.  
        ///      -  It activates all the operations on the MessageQ
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ///
        /// \param [in,out] message       (ref BaseMessage) - The message.
        /// \param          operation      (MessageQOperation) - The operation.
        /// \param          additionIndex (Optional)  (int) - Zero-based index of the addition.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MessageQHandling(ref BaseMessage message, MessageQOperation operation, int additionIndex = -1)
        {
            ChangeMessageOrder changeMessageOrder = Element.op[bp.opk.ChangeOrderEvents];
            switch (operation)
            {
                case MessageQOperation.Init: InitMessageQ(); return;
                case MessageQOperation.AddMessage:
                    AddMessageToQueue(message, additionIndex);
                    changeMessageOrder.CheckAndChange();
                    return;
                case MessageQOperation.RetrieveFirstMessage: RetrieveFirstMessageFromQueue(ref message); return;
                case MessageQOperation.RemoveFirstMessage:
                    RemoveFirstMessageFromQueue();
                    changeMessageOrder.CheckAndChange();
                    return;
                case MessageQOperation.EmptyQueue: EmptyMessageQ(); return;
                case MessageQOperation.BlockMessageProcessing: BlockMessageProcessing(); return;
                case MessageQOperation.ReleaseMessageProcessing: ReleaseMessageProcessing(); return;
                default: return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void InitMessageQ()
        ///
        /// \brief Init message q.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitMessageQ()
        {
            MessageQEvent.Reset();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetMessagesPositionInQ()
        ///
        /// \brief Sets messages position in q.
        ///
        /// \par Description.
        ///      After each change in the queue the attribute __PositionInProcessQ__ of the message is updated
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetMessagesPositionInQ()
        {
            for (int idx = 0; idx < Count; idx++)
            {
                this[idx].SetField(bm.ork.PositionInProcessQ, idx);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void AddMessageToQueue(BaseMessage message, int additionIndex)
        ///
        /// \brief Adds a message to queue to 'additionIndex'.
        ///
        /// \par Description.
        ///      Add a message to the MessageQ by the receive threads
        ///      -  Because there are many receive threads each thread locks the MessageQ when  
        ///         Performing the message additions
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ///
        /// \param message        (BaseMessage) - The message.
        /// \param additionIndex  (int) - Zero-based index of the addition.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AddMessageToQueue(BaseMessage message, int additionIndex)
        {
            // This method is used by all the receive threads so it has to be protected by a lock
            lock (MessageQLock)
            {
                if (additionIndex == -1)
                {
                    Add(new Attribute { Value = message });
                }
                else
                {
                    Insert(additionIndex, new Attribute { Value = message });
                }
                SetMessagesPositionInQ();
                MessageQEvent.Set();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void RetrieveFirstMessageFromQueue(ref BaseMessage message)
        ///
        /// \brief Retrieves first message from queue.
        ///
        /// \par Description.
        ///      Retrieve the first message from the queue
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ///
        /// \param [in,out] message (ref BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RetrieveFirstMessageFromQueue(ref BaseMessage message)
        {
            // If the Q is empty the main thread process will wait on this event until a message
            // will arrive
            MessageQEvent.WaitOne();

            // This is a virtual method that make it possible to arrange the messages in the
            // message Q not according to the order they arrived
            process.ArrangeMessageQ(this);

            // This is a message that make it possible to hold a processing of the first message in the Q
            // If a hold is done an empty message will return that will cause the ReceiveLoopEnvelope
            // To skip the call to ReceiveHandeling (start a new iteration)
            if (process.MessageProcessingCondition(this[0]))
            {
                message = this[0];
            }
            else
            {
                message = new BaseMessage(network);
            }
            SetMessagesPositionInQ();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void RemoveFirstMessageFromQueue()
        ///
        /// \brief Removes the first message from queue.
        ///
        /// \par Description.
        ///      -  Remove the first message from the Queue  
        ///      -  If the size of the Q is 0 reset the event (so that the process thread will  
        ///         not continue
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RemoveFirstMessageFromQueue()
        {
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, process.GetProcessDefaultName(), "", "In RemoveFirstMessageFromQueue  ", "", "RunningWindow");
            if (Count > 0)
            {
                RemoveAt(0);
                SetMessagesPositionInQ();
                if (Count == 0)
                {
                    MessageQEvent.Reset();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void EmptyMessageQ()
        ///
        /// \brief Empty message q.
        ///
        /// \par Description.
        ///      -  Empty the messageQ  
        ///      -  Reset the event
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EmptyMessageQ()
        {
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, process.GetProcessDefaultName(), "", "In EmptyMessageQ  ", "", "RunningWindow");
            while (Count > 0)
            {
                RemoveAt(0);
            }
            MessageQEvent.Reset();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void BlockMessageProcessing()
        ///
        /// \brief Block message processing.
        ///
        /// \par Description.
        ///      Reset the event
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void BlockMessageProcessing()
        {
            MessageQEvent.Reset();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ReleaseMessageProcessing()
        ///
        /// \brief Releases the message processing.
        ///
        /// \par Description.
        ///      Set the event if the Q is not empty
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ReleaseMessageProcessing()
        {
            if (Count == 0)
            {
                MessageQEvent.Reset();
            }
            else
            {
                MessageQEvent.Set();
            }
        }
        #endregion
        #region /// \name Change message order during running

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<int> ExstractChannelMessages(int sourceProcess, out AttributeList channelMessages)
        ///
        /// \brief Creates channel messages idxs.
        ///
        /// \par Description.
        ///      -  This method returns a list of indexes of the messages from a channel in the MessageQ  
        ///      -  This method also creates a list of messages from the channel  
        ///      -  Example
        ///         -   for a MessageQ which looks like:  
        ///             -#  [0] from process 1 message1
        ///             -#  [1] from process 2 message2
        ///             -#  [2] from process 1 message3
        ///         - The following list for process 1 will be generated for the indexes  
        ///             -#  [0] 0
        ///             -#  [1] 2
        ///         - The following list will be generated for the messages  
        ///             -#  [0] message1
        ///             -#  [1] message3.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2018
        ///
        /// \param       sourceProcess   (int) - Source process.
        /// \param [out] channelMessages (MessageQ) - The message q.
        ///
        /// \return The new channel messages idxs.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<int> ExstractChannelMessages(int sourceProcess, out AttributeList channelMessages)
        {
            List<int> channelMessagesIdxs = new List<int>();
            channelMessages = new AttributeList();
            for (int idx = 0; idx < Count; idx++)
            {
                if (((BaseMessage)this[idx]).GetHeaderField(bm.pak.SourceProcess) == sourceProcess)
                {
                    channelMessagesIdxs.Add(idx);
                    BaseMessage newMessage = new BaseMessage(network, this[idx]);
                    channelMessages.Add(newMessage);
                }
            }
            return channelMessagesIdxs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ChangeOrder(List<int> permutations, int sourceProcessId)
        ///
        /// \brief Change order.
        ///
        /// \par Description.
        ///      This method change the order of messages coming from a process.
        ///
        /// \par Algorithm.
        ///      -# The first step is to create the __channelMessagesIdxs__ which is conversion list from the Q (which contains  
        ///         messages from all processes) to a list that contains messages from the source process
        ///         -#  Example:
        ///             -#  Suppose we have the following Message Q
        ///                 -#  [0] Message0 from process 0
        ///                 -#  [1] Message1 from process 1
        ///                 -#  [2] Message2 from process 0
        ///                 -#  [3] Message3 from process 2
        ///             -#  The conversion table for process 0 will be
        ///                 -#  [0] 0
        ///                 -#  [1] 2
        ///      -# The second step is to create __messagesFromProcess__ which is a list of messages from the source process
        ///         -#  For this example the list will be
        ///                 -#  [0] message0
        ///                 -#  [1] message2
        ///      -# The third step is to apply the permutations:
        ///         -#  Example if we switched between message0 and message1
        ///             -# The permutation list will be
        ///                -#  [0] 1
        ///                -#  [1] 0
        ///                -#  The source index of each change is the value of the entry in __permutations__
        ///                -#  The target index of each change is the index of the entry in __permutations__
        ///                -#  The changes will be:
        ///                    -#  For index 0 in the __permutation__ list
        ///                        -#  Take the message in index 1 in the __messagesFromProcess__ list(because 1 is the value)
        ///                        -#  Put this message in the MessageQ in index __channelMessagesIdxs[0]__ which is 0
        ///                    -#  For index 1 in the __permutation__ list
        ///                        -#  Take the message in index 1 in the __messagesFromProcess__ list(because 0 is the value)
        ///                        -#  Put this message in the MessageQ in index __channelMessagesIdxs[1]__ which is 2.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2018
        ///
        /// \param permutations    (List&lt;int&gt;) - The permutations.
        /// \param sourceProcessId (int) - Identifier for the source process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ChangeOrder(List<int> permutations, int sourceProcessId)
        {
            // Create a list of indexes of the messages coming from the source process
            // This list is used to convert indexes from the permutation list to the
            // MessageQues : 
            // If we have a value in the permutations the message that corresponds to this value
            // Is channelMessagesIdx[value] 
            List<int> channelMessagesIdxs = new List<int>();

            // Create a list of messagesFromProcess
            // The purpose of this list is to be the source of the messages that changed
            // If we have a permutation we copy from this list to the MessageQ and
            // don't have to worry about overriding messages in the messageQ
            AttributeList messagesFromProcess = new AttributeList();
            channelMessagesIdxs = ExstractChannelMessages(sourceProcessId, out messagesFromProcess);
            
            // Do the permutations
            for (int idx = 0; idx < permutations.Count; idx ++)
            {
                if (permutations[idx] != idx)
                {
                    int originalIndex = permutations[idx];
                    int finalIndex = idx; 
                    this[channelMessagesIdxs[finalIndex]] = messagesFromProcess[originalIndex];
                }
            }

            // Change the message in process of the processor
            RetrieveFirstMessageFromQueue(ref ((BaseProcess)Element).MessageInProcess);
        }
        #endregion
    }
}
