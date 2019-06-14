////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Events\BaseAlgorithmMessages.cs
///
/// \brief Implements the base algorithm messages class.
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
    /// \class BaseAlgorithmMessages
    ///
    /// \brief A base algorithm messages.
    ///
    /// \par Description.
    ///      -  This class is a list of entries contains data for sending a base algorithm message  
    ///      -  The list is part of a base algorithm event  
    ///      -  The base algorithm event is an entry in the BaseAlgorithmHandler  
    ///      -  The event is composed from a trigger and list of messages (This class)  
    ///      -  If the event hits the messages in this object are sent
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 09/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class BaseAlgorithmMessages : AttributeList
    {
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Comps
        ///
        /// \brief Keys for the components of a message to be sent
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Comps
        {
            /// The message type (A key from an enum)
            Type,

            /// The message name (A string)
            Name,

            /// The message fields (An AttributeDictionaries)
            Fields,

            /// The ids of the targets of the message (An AttributeList of int)
            Targets
        }
        #endregion
        #region /// \name Add a message

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddMessage(dynamic type, string name, AttributeDictionary fields, AttributeList targets)
        ///
        /// \brief Adds a message.
        ///
        /// \par Description.
        ///      -  Add a new message containing message data  
        ///      -  This method is used to add message from the code 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param type     (dynamic) - The type.
        /// \param name     (string) - The name.
        /// \param fields   (AttributeDictionary) - The fields.
        /// \param targets  (AttributeList) - The targets.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddMessage(dynamic type,
            string name,
            AttributeDictionary fields,
            AttributeList targets)
        {
            AttributeDictionary messageData = new AttributeDictionary();
            messageData.Add(Comps.Type, new Attribute { Value = type });
            messageData.Add(Comps.Name, new Attribute { Value = name });
            messageData.Add(Comps.Fields, new Attribute { Value = fields });
            messageData.Add(Comps.Targets, new Attribute { Value = targets, ElementWindowPrmsMethod = IntListPrms });
            Add(new Attribute { Value = messageData, Editable = false });
        }       
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Attribute EmptyMessageData()
        ///
        /// \brief Empty message data.
        ///
        /// \par Description.
        ///      -  This method is used to add an empty message to the list (with default values)  
        ///      -  This method is used by the GUI to create an empty entry to the list
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \return An Attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Attribute EmptyMessageData()
        {
            AttributeDictionary messageData = new AttributeDictionary();
            Type messageTypeEnum = ClassFactory.GenerateMessageTypeEnum();
            dynamic type = Enum.ToObject(messageTypeEnum, 0);
            messageData.Add(Comps.Type, new Attribute { Value = type });
            messageData.Add(Comps.Name, new Attribute { Value = "BaseMessage" });
            messageData.Add(Comps.Fields, new Attribute { Value = new AttributeDictionary() });
            messageData.Add(Comps.Targets, new Attribute { Value = new AttributeList(), ElementWindowPrmsMethod = IntListPrms });
            return new Attribute { Value = messageData, Editable = false };
        }
        #endregion
        #region /// \name Send all messages

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Send(int round)
        ///
        /// \brief Send this message.
        ///
        /// \par Description.
        ///      -  Send all the messages that are in the list  
        ///
        /// \par Algorithm.
        ///      -  For each message in the list  
        ///      -  Call BuildBaseAlgorithmMessage of the process. This method allows the process  
        ///         to change the message according to process status
        ///      -  Compose a list of targets and send the message/s
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param round  (int) - The round.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Send(int round)
        {
            BaseProcess process = (BaseProcess)Element;
            foreach (AttributeDictionary data in AsList())
            {
                // Duplicate the messageData
                // This is done in order to avoid the change to the messageData in
                // the method BuildBaseAlgorithmMessage
                // (The data in this object is not recovered after running so we have
                // to prevent changes to it while running)
                AttributeDictionary messageData = new AttributeDictionary();
                messageData.DeepCopy(data);
                //BaseMessage message = process.BuildBaseAlgorithmMessage(
                //    messageData[Comps.Type],
                //    messageData[Comps.Name],
                //    messageData[Comps.Fields],
                //    round,
                //    messageData[Comps.Targets]);
                process.SendWithNoEventsProcessing(messageData[Comps.Type], 
                    messageData[Comps.Fields], 
                    BaseProcess.SelectingMethod.Include, 
                    messageData[Comps.Name], 
                    messageData[Comps.Targets].AsList(), 
                    -1, -1);
                //process.SendWithNoEventsProcessing(message, BaseProcess.SelectingMethod.Include, messageData[Comps.Targets].AsList());
            }
        }
        #endregion
        #region /// \name ElementWindow (GUI) support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms MessageListPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Message list prms.
        ///
        /// \par Description.
        ///      -  The BaseAlgorithmMessages is part of an event in the BaseAlgorithmHandler list
        ///      -  This method is used by the BaseAlgorithmHandler to set the GUI presentation  
        ///         parameters
        ///      -  The purpose of this method is to set the EmptyMessagesData as the method that will  
        ///         Create a new entry in the list (When pressing the Add button of the messages list (this object) the EmptyMessageData
        ///         will be activated in order to create a new empty message)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms MessageListPrms(Attribute attribute,
        dynamic key,
        NetworkElement mainNetworkElement,
        NetworkElement.ElementDictionaries mainDictionary,
        NetworkElement.ElementDictionaries dictionary,
        InputWindows inputWindow,
        bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            attribute.DefaultNewValueFieldData(ref prms, mainNetworkElement, mainDictionary, dictionary, inputWindow, true, true);
            prms.newValueControlPrms.addAttributeMethod = EmptyMessageData;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms IntListPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Int list prms.
        ///
        /// \par Description.
        ///      -  This method is meant to set the GUI parameters for the field Targets of the messages  
        ///      -  Each message contains a list of ints which are target for the message  
        ///      -  This method is added to each possible target of the message  
        ///      -  The purpose of this message is to assign a check that the target entered (a process id)  
        ///         exists in the network been designed 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEditable      (bool) - true if window editable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms IntListPrms(Attribute attribute,
        dynamic key,
               NetworkElement mainNetworkElement,
               NetworkElement.ElementDictionaries mainDictionary,
               NetworkElement.ElementDictionaries dictionary,
               InputWindows inputWindow,
               bool windowEditable)
        {

            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.AddRemovePanel;
            prms.newValueControlPrms.addAttributeMethod = CreateProcessIdAttribute;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Attribute CreateProcessIdAttribute()
        ///
        /// \brief Creates process identifier attribute.
        ///
        /// \par Description.
        ///      -  This method assign to the process id attribute in the targets list a check that
        ///         will be performed after the value in the GUI is changed
        ///      -  The check is that the process exists
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \return The new process identifier attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Attribute CreateProcessIdAttribute()
        {
            return new Attribute { Value = 0, EndInputOperation = CheckIfProcessExists };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckIfProcessExists(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Determine if process exists.
        ///
        /// \par Description.
        ///      -  This method is activated by the GUI after a value in one of the target processors  
        ///         for the message was changed.
        ///      -  The method checks whether there is a process with id equals to the new value in the network
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param       network          (BaseNetwork) - The network.
        /// \param       networkElement   (NetworkElement) - The network element.
        /// \param       parentAttribute  (Attribute) - The parent attribute.
        /// \param       attribute        (Attribute) - The attribute.
        /// \param       newValue         (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        /// \param       inputWindow     (Optional)  (ElementWindow) - The input window.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool CheckIfProcessExists(BaseNetwork network,
            NetworkElement networkElement,
            Attribute parentAttribute,
            Attribute attribute,
            string newValue,
            out string errorMessage,
            ElementWindow inputWindow = null)
        {
            errorMessage = "";
            try
            {
                errorMessage = "The value has to be int";
                int id = int.Parse(newValue);

                errorMessage = "There is no processor with id :" + newValue;
                network.Processes.First(p => p.ea[ne.eak.Id] == id);

                errorMessage = "";
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
       