////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Events\EventTrigger.cs
///
/// \brief Implements the event trigger class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    #region \name Global Enums
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \enum EventTriggerType
    ///
    /// \brief Values that represent event trigger types.
    ///        The trigger types represent the status of the process when the event trigger is checked
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum EventTriggerType
    {
        Initialize, BeforeSendMessage, AfterSendMessage, BeforeReceiveMessage, AfterReceiveMessage, UserEvent
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class EventTrigger
    ///
    /// \brief An event trigger.
    ///        -    The EventTrigger is used by to the BaseAlgorithmHandler and InternalEventsHandler  
    ///        -    The EventTrigger is an AttributeDictionary that holds data for deciding if an event occurs  
    ///        -    Each time the process is about to perform init/send/receive action a call is made  
    ///             to the PerformEvents of the containers and they call the True method of all
    ///             their EventTrigger to see if the trigger hits. If it does they perform their operation
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 09/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class EventTrigger : AttributeDictionary
    {
        #region /// \name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Comps
        ///
        /// \brief Keys that represent the components of the trigger
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum Comps
        {
            /// A key from EventTypes which indicates the status of the processor 
            Trigger,

            /// The round field in the message
            Round,

            /// The type of the message that is sent/received when this trigger is checked
            MessageType,

            /// The source/destination processor in the message that caused the event
            OtherEnd
        }
        #endregion       
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EventTrigger(EventTriggerType triggerType, int round, dynamic messageType, int messageOtherEnd) : base()
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Constructor with values to be used by the code
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param triggerType      (EventTriggerType) - Type of the trigger.
        /// \param round            (int) - The round.
        /// \param messageType      (dynamic) - Type of the message.
        /// \param messageOtherEnd  (int) - The message other end.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EventTrigger(EventTriggerType triggerType,
            int round,
            dynamic messageType,
            int messageOtherEnd) : base()
        {
            Add(Comps.Trigger, new Attribute { Value = triggerType, EndInputOperation = EventTriggerChanged });
            Add(Comps.Round, new Attribute { Value = round, ElementWindowPrmsMethod = DisableIfTriggerInitialize });
            Add(Comps.MessageType, new Attribute { Value = messageType, ElementWindowPrmsMethod = MessageTypePrms });
            Add(Comps.OtherEnd, new Attribute { Value = messageOtherEnd, ElementWindowPrmsMethod = DisableIfTriggerInitialize });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public EventTrigger() : base()
        ///
        /// \brief Default constructor.
        ///        Constructor with default values which is used by the GUI
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EventTrigger() : base()
        {
            Type messageTypeEnum = ClassFactory.GenerateMessageTypeEnum();
            dynamic messageType = Enum.ToObject(messageTypeEnum, 0);
            Add(Comps.Trigger, new Attribute { Value = EventTriggerType.AfterSendMessage, EndInputOperation = EventTriggerChanged });
            Add(Comps.Round, new Attribute { Value = 0, ElementWindowPrmsMethod = DisableIfTriggerInitialize });
            Add(Comps.MessageType, new Attribute { Value = messageType, ElementWindowPrmsMethod = MessageTypePrms });
            Add(Comps.OtherEnd, new Attribute { Value = 0, ElementWindowPrmsMethod = DisableIfTriggerInitialize });
        }
        #endregion
        #region /// \name Event hit checks

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool True(EventTriggerType trigger, BaseMessage message)
        ///
        /// \brief Trues.
        ///
        /// \par Description.
        ///      This method is used by the BaseAlgorithmHandler and the InternalEventsHandler to
        ///      see if the EventTrigger hits
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param trigger  (EventTriggerType) - The trigger.
        /// \param message  (BaseMessage) - The message.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool True(EventTriggerType trigger, BaseMessage message)
        {
            switch (trigger)
            {
                case EventTriggerType.Initialize:
                    return IsInit();
                default:
                    return IsTrue(trigger, message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool IsInit()
        ///
        /// \brief Query if this object is init.
        ///
        /// \par Description.
        ///      Checks if the trigger of the EventTrigger is Initialize
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \return True if init, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool IsInit()
        {
            return this[Comps.Trigger] == EventTriggerType.Initialize;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool IsTrue(EventTriggerType trigger, BaseMessage message)
        ///
        /// \brief Query if 'trigger' is true.
        ///
        /// \par Description.
        ///      -  Check other triggers type (not the Initialize trigger)
        ///
        /// \par Algorithm.
        ///      -  Get the MessageType of the message and the round of the message  
        ///      -  Calculate the other end of the message according to the trigger  
        ///         -   For Send events the other end is the target of the message  
        ///         -   For Receive events the other end is the source of the message  
        ///      -  Check if all the parameters retrieved from the message are equal to the   
        ///         EventTrigger members
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param trigger  (EventTriggerType) - The trigger.
        /// \param message  (BaseMessage) - The message.
        ///
        /// \return True if true, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool IsTrue(EventTriggerType trigger, BaseMessage message)
        {
            // Get the data of the message that caused the event
            int messageRound = message.GetHeaderField(bm.pak.Round);
            dynamic messageType = message.GetHeaderField(bm.pak.MessageType);

            // Get the other end of the message that caused the event:
            // For a sending event this is the destination of the message
            // For a receive event this is the source of the message
            int messageOtherEnd;
            if (trigger == EventTriggerType.AfterSendMessage || trigger == EventTriggerType.BeforeSendMessage)
            {
                messageOtherEnd = message.GetHeaderField(bm.pak.DestProcess);
            }
            else
            {
                messageOtherEnd = message.GetHeaderField(bm.pak.SourceProcess);
            }

            if (this[Comps.Trigger] == trigger &&
                    this[Comps.Round] == messageRound &&
                    TypesUtility.CompareDynamics(this[Comps.MessageType], messageType) &&
                    this[Comps.OtherEnd] == messageOtherEnd)
            {
                return true;
            }
            return false;
        }
        #endregion
        #region /// \name ElementWindow (GUI) support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EventTriggerChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Event trigger changed.
        ///
        /// \par Description.
        ///      -  This method is activated by the GUI when the Trigger was changed  
        ///      -  The purpose of this method is to disable all the other fields if the   
        ///         Event is initialize and enable all the fields otherwise 
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

        public static bool EventTriggerChanged(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            // Get All the childes of the parent attribute
            List<ElementWindow.ControlsAttributeLink> links = inputWindow.controlsAttributeLinks.Values.Where(l => l.parentAttribute == parentAttribute
                && l.attribute != attribute).ToList();
            if (newValue == "Initialize")
            {
                foreach (ElementWindow.ControlsAttributeLink link in links)
                {
                    link.newValueControl.IsEnabled = false;
                    ((Control)link.newValueControl).Background = Brushes.LightGray;
                    string defaultValue = "";
                    inputWindow.ChangeValue(defaultValue, link.attribute);
                }
            }
            else
            {
                string prevValue = inputWindow.controlsAttributeLinks.Values.First(l => l.attribute == attribute).existingNewValue;
                if (prevValue == "Initialize")
                {
                    foreach (ElementWindow.ControlsAttributeLink link in links)
                    {
                        link.newValueControl.IsEnabled = true;
                        ((Control)link.newValueControl).Background = Brushes.White;
                        inputWindow.ChangeValue(link.existingValueTextBox.Text, link.attribute);
                    }
                }
            }
            errorMessage = "";
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms DisableIfTriggerInitialize(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Disables if trigger initialize.
        ///
        /// \par Description.
        ///      -  This method is activated to set for the GUI the display parameters:  
        ///         -   If the trigger is Initialize - disable the field  
        ///         -   Else enable the fields
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
        /// \param windowEdittable     (bool) - true if window edittable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms DisableIfTriggerInitialize(Attribute attribute, dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            attribute.DefaultNewValueFieldData(ref prms,
                mainNetworkElement,
                mainDictionary,
                dictionary,
                inputWindow,
                windowEdittable,
                attribute.Editable);
            if (((AttributeDictionary)attribute.Parent)[Comps.Trigger] == EventTriggerType.Initialize)
            {
                prms.newValueControlPrms.enable = false;
            }
            else
            {
                prms.newValueControlPrms.enable = true;
            }
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms MessageTypePrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Message type prms.
        ///
        /// \par Description.
        ///      -  This method is activated to set for the GUI the display parameters for the MessageType field  
        ///      -  The purpose of the message is to set the values for the ComboBox  
        ///         All the message type of the algorithm + all the message types of the Base Algorithm
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
        /// \param windowEdittable     (bool) - true if window edittable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms MessageTypePrms(Attribute attribute, dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            prms.newValueControlPrms.options = TypesUtility.GetEnumKeysToStrings(attribute.Value.GetType().ToString()).ToArray();
            prms.newValueControlPrms.enable = true;
            // List<string> baseMessagesTypes = TypesUtility.GetEnumKeysToStrings(typeof(bm.MessageTypes).ToString());
            // prms.newValueControlPrms.options = messagesTypes.Concat(baseMessagesTypes).ToArray();
            prms.newValueControlPrms.Value = TypesUtility.GetKeyToString(attribute.Value);
            return prms;
        }
        #endregion
    }
}
