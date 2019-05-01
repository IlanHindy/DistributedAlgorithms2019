////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Network Elements Components\ChangeMessageOrder.cs
///
/// \brief Implements the change message order class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ChangeMessageOrder
    ///
    /// \brief A change message order.
    ///
    /// \par Description.
    ///      -  The ChangeMessageOrder is a class that is responsible to record change message  
    ///         order and operate them in the next running of the algorithm
    ///      -  The ChangeMessageOrder is found in the OperationParameters dictionary of  
    ///         the process. That means that it record changes and the changes are not deleted
    ///         after the running 
    ///      -  The following is the operation method:  
    ///         -#  During running the messages are presented in a GridView
    ///         -#  The GridView allows a change in the order of the messages
    ///         -#  If the change is confirmed by the user a method from this class is called to record
    ///             The change
    ///         -#  This class is an AttributeList of AttributeDictionaries
    ///         -#  Each AttributeDictionary holds the following data:
    ///             -#  The source process
    ///             -#  The permutation - A list stating the new order of the messages
    ///             -#  The original messages 
    ///         -#  During running the messages are checked, and if there is a situation in which
    ///             the messages in the MessageQ from the source process are identical
    ///             to the recorded a messages,  the user is ask the user if he wants to 
    ///             reorder the messages
    ///         -#  If the user gives positive answer the process's message queue is reordered
    ///             Using method of the MessageQ  object
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 27/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ChangeMessageOrder:AttributeList
    {
        #region /// \name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Comps
        ///
        /// \brief Values that represent comps.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum Comps { SourceProcess, Permutations, MessageQ}
        #endregion
        #region /// \name Record an order change

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RecordOrderChange(List<int> permutations, int sourceProcess)
        ///
        /// \brief Record order change.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param permutations   (List&lt;int&gt;) - The permutations.
        /// \param sourceProcess  (int) - Source process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RecordOrderChange(List<int> permutations, int sourceProcess)
        {
            // Extract messages of the channel from the messageQ
            MessageQ messageQ = Element.or[bp.ork.MessageQ];
            AttributeList channelMessages;
            messageQ.ExstractChannelMessages(sourceProcess, out channelMessages);
            AttributeDictionary changeRecord = new AttributeDictionary();
            changeRecord.Add(Comps.SourceProcess, new Attribute { Value = sourceProcess, Editable = false });
            changeRecord.Add(Comps.MessageQ, new Attribute { Value = channelMessages, Editable = false });
            changeRecord.Add(Comps.Permutations, new Attribute { Value = permutations.ToAttributeList(), Editable = false });
            Add(new Attribute { Value = changeRecord, Editable = false });
        }
        #endregion
        #region /// \name Check and Change message Q and 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CheckAndChange()
        ///
        /// \brief Check and change.
        ///
        /// \par Description.
        ///      -  For each entry in the list:    
        ///         -#  Extract the methods from the MessageQ of the sourceProcess (attribute in the entry)
        ///         -#  Compare this list with the list in the entry 
        ///         -#  if they are identical
        ///             -#  Ask the user if to perform the change  
        ///         -#  If the answer is yes perform the order change using a method of the MessageQ class
        ///
        /// \par Algorithm.
        ///      
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CheckAndChange()
        {
            MessageQ messageQ = Element.or[bp.ork.MessageQ];

            if (Count > 0)
            {
                for (int idx = 0; idx < Count; idx ++) 
                {
                    AttributeList channelMessages;                   
                    int sourceProcess = this[idx][Comps.SourceProcess];
                    List<int> messageIdxs = messageQ.ExstractChannelMessages(sourceProcess, out channelMessages);
                    if (channelMessages.CheckEqual(0, "", (AttributeList)this[idx][Comps.MessageQ]))
                    {
                        List<int> permutations = this[idx][Comps.Permutations].AsList();
                        if (PerformQuestion(this[idx], messageIdxs))
                        {
                            messageQ.ChangeOrder(permutations, sourceProcess);
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool PerformQuestion(AttributeDictionary entry, List<int> messageIdxs)
        ///
        /// \brief Performs the question action.
        ///
        /// \par Description.
        ///      -  Ask the user if to perform an order change  
        ///      -  There can be 3 answers  
        ///         -#  Yes
        ///         -#  No
        ///         -#  Remove from the list
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sourceProcess (int) - Source process.
        ///
        /// \param entry       (List&lt;int&gt;) - The permutations.
        /// \param messageIdxs (List&lt;int&gt;) - The message idxs.
        ///
        /// \return True if it succeeds, false if it fails.
        ///
        /// \param MessageQ (AttributeList) - The message q.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool PerformQuestion(AttributeDictionary entry, List<int> messageIdxs)
        {
            MessageBoxElementData l1 = new MessageBoxElementData("Message from process : " + Element.ToString(), new Font { alignment = HorizontalAlignment.Center, fontWeight = FontWeights.Bold });
            MessageBoxElementData l2 = new MessageBoxElementData("In Previous running of the algorithm the order of the messages was change");
            MessageBoxElementData l3 = new MessageBoxElementData("The source process is :" + entry[Comps.SourceProcess].ToString());
            MessageBoxElementData l4 = new MessageBoxElementData("The source list is :", new Font { fontWeight = FontWeights.Bold });
            List<string> originalContent = new List<string>();
            AttributeList messagesQ = entry[Comps.MessageQ];
            messagesQ.ForEach(a => originalContent.Add(((BaseMessage)a.Value).Description()));
            string originalString;
            string transforedString;
            PermutationString(entry[Comps.Permutations].AsList(), originalContent, out originalString, out transforedString);
            MessageBoxElementData l5 = new MessageBoxElementData(originalString);
            MessageBoxElementData l6 = new MessageBoxElementData("The transformed list is :", new Font { fontWeight = FontWeights.Bold });
            MessageBoxElementData l7 = new MessageBoxElementData(transforedString);
            MessageBoxElementData l8 = new MessageBoxElementData("Do you want to apply this change ?", new Font { fontWeight = FontWeights.Bold });
            MessageBoxElementData b1 = new MessageBoxElementData("Yes");
            MessageBoxElementData b2 = new MessageBoxElementData("No");
            MessageBoxElementData b3 = new MessageBoxElementData("Remove");
            string result = MessageRouter.CustomizedMessageBox(new List<MessageBoxElementData> { l1, l2, l3, l4, l5, l6, l7, l8 },
                "ChangeMessageOrder Message",
                new List<MessageBoxElementData> { b1, b2, b3 },
                Icons.Question, true);
            string s = "";
            switch(result)
            {
                case "Yes":
                    return true;
                case "Remove":
                    Remove((Attribute)entry.Parent);
                    return false;
                default:
                    return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void PermutationString(List<int> permutations, List<string> originalContent, out string originalString, out string transformedString, List<int> messageIdxs = null)
        ///
        /// \brief Permutation string.
        ///
        /// \par Description.
        ///      This goal of this method is to create 2 strings:
        ///      -# The original message list
        ///      -# The message list after the transformation
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param       permutations       (List&lt;int&gt;) - The permutations.
        /// \param       originalContent    (List&lt;string&gt;) - The original content.
        /// \param [out] originalString    (out string) - The original string.
        /// \param [out] transformedString (out string) - The transformed string.
        /// \param       messageIdxs       (Optional)  (List&lt;int&gt;) - The message idxs.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void PermutationString(List<int> permutations, List<string> originalContent, out string originalString, out string transformedString, List<int> messageIdxs = null)
        {
            originalString = "";
            for (int contentIdx = 0; contentIdx < originalContent.Count; contentIdx ++)
            {
                if (messageIdxs != null)
                {
                    originalString += "[" + messageIdxs[contentIdx].ToString() +"]";
                }
                originalString += originalContent[contentIdx] + "\n";
            }
            transformedString = "";
            for (int contentIdx = 0; contentIdx < originalContent.Count; contentIdx++)
            {
                if (messageIdxs != null)
                {
                    transformedString += "[" + messageIdxs[permutations[contentIdx]].ToString() + "]";
                }
                transformedString += originalContent[permutations[contentIdx]] + "\n";
            }
        }
        #endregion
    }
}
