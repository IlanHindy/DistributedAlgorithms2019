////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file infrastructure\networkupdate.cs
///
/// \brief Implements the NetworkUpdate class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;
namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class NetworkUpdate
    ///
    /// \brief A network update.
    ///
    /// \par Description.
    ///      -  The NetworkUpdate algorithm is used in the following case:
    ///         -#  A network was designed
    ///         -#  The automatically generated code of the algorithm was changed
    ///         -#  When reentering the program we want to update the network (To merge between
    ///             the code changes and the existing changes)
    ///      -  The algorithm  
    ///         -#  For each component in the existing network 
    ///             -#  Create a new component (with the new code)
    ///             -#  Deep scan of the element and:
    ///                 -#  If the was changed when building the network - copy the value to the new network
    ///                 -#  If the attribute is a list or dictionary do a clever merge (see the methods for the algorithm of the merge   
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 28/12/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class NetworkUpdate : IScanConsumer
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Stack&lt;Attribute&gt;) - The complex attributes.
        ///        A stack that holds the complex attributes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        Stack<Attribute> complexAttributes = new Stack<Attribute>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Attribute) - The skip attribute.
        ///        The scanning is done on the new algorithm components. If we reached a component in the new that has
        ///        no equivalent in the existing we set the skip and skip all the attributes until closing of that attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        Attribute skipAttribute = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseNetwork Update(BaseNetwork existingNetwork)
        ///
        /// \brief Updates the given existingNetwork.
        ///
        /// \par Description.
        ///      -  Update an existing network after the code was changed.  
        ///      -  The algorithm :  
        ///         -   For each component of the network activate the Update method on it  
        ///         -   The Update method starts ScanAndReport process that will cause the  
        ///             2 instances to be cleverly merged
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param existingNetwork  (BaseNetwork) - The existing network.
        ///
        /// \return A BaseNetwork.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseNetwork Update(BaseNetwork existingNetwork)
        {
            BaseNetwork newNetwork = ClassFactory.GenerateNetwork();
            Update(existingNetwork, newNetwork);

            foreach (BaseProcess existingProcess in existingNetwork.Processes)
            {
                BaseProcess newProcess = ClassFactory.GenerateProcess(newNetwork);
                Update(existingProcess, newProcess);
                newNetwork.Processes.Add(newProcess);
            }

            foreach (BaseChannel existingChannel in existingNetwork.Channels)
            {
                BaseChannel newChannel = ClassFactory.GenerateChannel(newNetwork, 
                    existingChannel.ea[ne.eak.Id],
                    existingChannel.ea[bc.eak.SourceProcess],
                    existingChannel.ea[bc.eak.DestProcess]);
                Update(existingChannel, newChannel);
                newNetwork.Channels.Add(newChannel);
            }
            return newNetwork;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Update(NetworkElement existingNetworkElement, NetworkElement newNetworkElement)
        ///
        /// \brief Updates this object.
        ///
        /// \par Description.
        ///      Starts a ScanAndReport process on the Network component
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param existingNetworkElement  (NetworkElement) - The existing network element.
        /// \param newNetworkElement       (NetworkElement) - The new network element.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Update(NetworkElement existingNetworkElement, NetworkElement newNetworkElement)
        {
            Report("Beginning of update");
            newNetworkElement.Init(existingNetworkElement.ea[ne.eak.Id]);
            complexAttributes.Push(new Attribute() { Value = existingNetworkElement });
            newNetworkElement.ScanAndReport(this, NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool ScanCondition(dynamic key, Attribute newAttribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Scans a condition.
        ///
        /// \par Description.
        ///      Scan all the NetworkElement
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param newAttribute    (Attribute) - The new attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ScanCondition(dynamic key,
            Attribute newAttribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void OpenComplexAttribute(dynamic key, Attribute newAttribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Opens complex attribute.
        ///
        /// \par Description.
        ///      A complex attribute arrived.
        ///      -  If we are in a skip mode (The exists network does not have a corresponding attributes for the new) - return  
        ///      -  If the attribute does not exists in the exists network - enter to skip mode  
        ///      -  Else - Start a merge process  
        ///      -  Push the attribute to the stack 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param newAttribute    (Attribute) - The new attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void OpenComplexAttribute(dynamic key,
            Attribute newAttribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            Report("Start of OpenComplexItem newAttribute is " + 
                newAttribute.Parent.GetChildKey(newAttribute).ToString() +
                " IdInList is " + newAttribute.IdInList.ToString() );
            //Report("Start of OpenComplexItem");
            // If we are in a status of skipping (means that we are scanning an attribute
            // that does not exist in the existing
            if (skipAttribute != null)
            {
                return;
            }

            Attribute existingAttribute = GetExistingChildAttribute(key, newAttribute);
            if (existingAttribute == null)
            {
                skipAttribute = newAttribute;
                return;
            }

            switch (Attribute.GetValueCategory(newAttribute))
            {
                case Attribute.AttributeCategory.ListOfAttributes:
                    MergeLists(existingAttribute, newAttribute);
                    break;
                case Attribute.AttributeCategory.AttributeDictionary:
                    MergeDictionaries(existingAttribute, newAttribute);
                    break;
                default:
                    break;
            }
            complexAttributes.Push(existingAttribute);
            Report("End of OpenComplexItem newAttribute is " + newAttribute.Parent.GetChildKey(newAttribute).ToString());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CloseComplexAttribute(dynamic key, Attribute newAttribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Closes complex attribute.
        ///
        /// \par Description.
        ///      -  If we are in skip mode and the attribute is not the root of the skipping - exit
        ///      -  If we are in skip mode and the attribute is the root of skipping - cancel the skipping mode  
        ///      -  If we are not in skip mode pop the head of the stack
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param newAttribute    (Attribute) - The new attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CloseComplexAttribute(dynamic key,
            Attribute newAttribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            // If we are scanning an attribute that does not exist in the existing we skip the
            // Operation (This process will continue until we get the new attribute from then
            // we continue regularly (pop the item)
            if (skipAttribute != null)
            {
                if (skipAttribute == newAttribute)
                {
                    skipAttribute = null;
                }
            }
            complexAttributes.Pop();
            Report("End of CloseComplexItem");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AttributeReport(dynamic key, Attribute newAttribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Attribute report.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param newAttribute    (Attribute) - The new attribute.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AttributeReport(dynamic key,
           Attribute newAttribute,
           NetworkElement.ElementDictionaries mainDictionary,
           NetworkElement.ElementDictionaries dictionary)
        {
            // If we are in a status of skipping (means that we are scanning an attribute
            // that does not exist in the existing) we exit
            if (skipAttribute != null)
            {
                return;
            }

            Attribute existingAttribute = GetExistingChildAttribute(key, newAttribute);
            if (existingAttribute != null)
            {
                // If the attribute exists in the existing and it's value was changed during network build
                // copy the value from the exists to the new
                if (existingAttribute.Changed)
                {
                    newAttribute.Value = existingAttribute.Value;
                }
                return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private Attribute GetExistingChildAttribute(dynamic key, Attribute newAttribute)
        ///
        /// \brief Gets existing child attribute.
        ///
        /// \par Description.
        ///      There can be the following cases
        ///      -# The attribute is found in both lists/dictionaries (So the attribute will be found)
        ///      -# The attribute is found only in the exists list
        ///         In this case There was a merge between the lists\dictionaries that puts the attributes
        ///         of the existing in the new so the attribute will be found
        ///      -# The attribute exists in the new and the exists - In this case the method returns null
        ///         that will cause the algorithm to skip on all the attribute tree
        ///
        ///
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param key           (dynamic) - The key.
        /// \param newAttribute  (Attribute) - The new attribute.
        ///
        /// \return The existing child attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Attribute GetExistingChildAttribute(dynamic key, Attribute newAttribute)
        {
            Attribute existingComplexAttribute = complexAttributes.Peek();
            string s = "";
            Attribute result = null;

            if (Attribute.GetValueCategory(existingComplexAttribute) == Attribute.AttributeCategory.ListOfAttributes)
            {
                s = "Tring to find IdInList : " + newAttribute.IdInList.ToString();
                result = ((AttributeList)existingComplexAttribute.Value).GetAttribute(newAttribute.IdInList.ToString());
            }

            if (Attribute.GetValueCategory(existingComplexAttribute) == Attribute.AttributeCategory.AttributeDictionary)
            {
                s = "Tring to find attribute with key " + TypesUtility.GetKeyToString(key);
                result = ((AttributeDictionary)existingComplexAttribute.Value).GetAttribute(key);
            }

            if (Attribute.GetValueCategory(existingComplexAttribute) == Attribute.AttributeCategory.NetworkElementAttribute)
            {
                result = ((NetworkElement)existingComplexAttribute.Value).GetDictionaryAttribute(key);
            }

            if (result == null)
            {
                s += " failed : ";
                MessageRouter.ReportMessage("NetworkUpdate - retrieve existing child", "", s);
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void MergeLists(Attribute existAttribute, Attribute newAttribute)
        ///
        /// \brief Merge lists.
        ///
        /// \par Description.
        ///      -  Clever merge between the existing and new lists
        ///         -  If the attribute is found in the existing and the new   
        ///             -   Copy the value from the existing to the new only if the value of the attribute was changed during network build  
        ///         -  If the attribute is found in the existing and not the new  
        ///             -   Add the attribute tree to the new  
        ///         -  If (Default case - automatically happens) The attribute exists in the new and not the old
        ///             -  keep it in the new  
        ///      -  The attributes are identified using the idsInList
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param existAttribute (Attribute) - The exist attribute.
        /// \param newAttribute    (Attribute) - The new attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MergeLists(Attribute existAttribute, Attribute newAttribute)
        {
            foreach (Attribute existAttr in existAttribute.Value)
            {
                Attribute newAttr = ((AttributeList)newAttribute.Value).FirstOrDefault(a => a.IdInList == existAttr.IdInList);
                if (newAttr is null)
                {
                    newAttr = new Attribute();
                    newAttribute.Value.Add(newAttr);
                    newAttr.DeepCopy(existAttr);
                }
                else
                {
                    if (existAttribute.Changed)
                    {
                        if (Attribute.GetValueCategory(existAttr) == Attribute.AttributeCategory.PrimitiveAttribute)
                        {
                            newAttr.Value = existAttr.Value;
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void MergeDictionaries(Attribute existAttribute, Attribute newAttribute)
        ///
        /// \brief Merge dictionaries.
        ///
        /// \par Description.
        ///      -  Clever merge between the existing and new dictionaries
        ///         -  If the attribute is found in the existing and the new
        ///             -   Copy the value from the existing to the new only if the value of the attribute was changed during network build  
        ///         -  If the attribute is found in the existing and not the new  
        ///             -   Add the attribute tree to the new  
        ///         -  If (Default case - automatically happens) The attribute exists in the new and not the old
        ///             -  keep it in the new  
        ///      -  The attributes are identified using the keys
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param existAttribute  (Attribute) - The exist attribute.
        /// \param newAttribute     (Attribute) - The new attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MergeDictionaries(Attribute existAttribute, Attribute newAttribute)
        {
            foreach (var existEntry in existAttribute.Value)
            {

                Attribute newAttr = ((AttributeDictionary)newAttribute.Value).FirstOrDefault(e => TypesUtility.CompareDynamics(e.Key, existEntry.Key)).Value;
                string s = " ** " + TypesUtility.GetKeyToString(existEntry.Key) + " **";
                if (newAttr == null)
                {
                    s += "Attribute null";
                }
                else
                {
                    s += "Attribute value = " + newAttr.Value.ToString();
                }
                MessageRouter.ReportMessage("Attribute Dictionary Merge ", "", s);
                if (newAttr is null)
                {
                    newAttr = new Attribute();
                    ((AttributeDictionary)newAttribute.Value).Add(existEntry.Key, newAttr);
                    newAttr.DeepCopy(existEntry.Value);
                }
                else
                {
                    if (existEntry.Value.Changed)
                    {
                        if (Attribute.GetValueCategory(existEntry.Value) == Attribute.AttributeCategory.PrimitiveAttribute)
                        {
                            newAttr.Value = existEntry.Value.Value;
                        }
                    }
                }
            }
            foreach (var newEntry in newAttribute.Value)
            {
                if (!existAttribute.Value.ContainsKey(newEntry.Key))
                {
                    Attribute attr = new Attribute();
                    attr.DeepCopy(newEntry.Value);
                    existAttribute.Value.Add(newEntry.Key, attr);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Report(string message)
        ///
        /// \brief Reports.
        ///
        /// \par Description.
        ///      Generate a message with the status to the MessagesWindow
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 07/01/2018
        ///
        /// \param message  (string) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Report(string message)
        {
            string s = "\n" + message + "\n";
            Attribute[] stack = complexAttributes.ToArray();
            for (int idx = 0; idx < stack.Length; idx ++)
            {
                if (stack[idx].Parent != null)
                {
                    s += "\t\t\t" + stack[idx].Parent.GetChildKey(stack[idx]).ToString() + "\n";
                }
                else
                {
                    s += "\t\t\t" + stack[idx].Value.GetType().ToString();
                }
            }
            MessageRouter.ReportMessage("NetworkUpdate", "", s);            
        }
    }
}
