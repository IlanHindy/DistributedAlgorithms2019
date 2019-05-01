////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Events\InternalEventsHandler.cs
///
/// \brief Implements the internal events handler class.
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
    /// \class InternalEventsHandler
    ///
    /// \brief An internal events handler.
    ///
    /// \par Description.
    ///      -  This class is used in order to implement activating of methods when the process hits conditions.  
    ///      -  The following is the method that the implementation works:  
    ///         -   An object from this class is found the OperationalParameters dictionary of the process  
    ///         -   The class is composed from a list of dictionaries that contains 2 items  
    ///             -#  The trigger
    ///             -#  A list of method to be activated if the trigger is hit
    ///         -   Each time the process is doing something, a call to the process call the ProcessEvents method of this class  
    ///         -   The ProcessEvents method checks all the EventTriggers and if one hit all the methods  
    ///             of the event are activated  
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 09/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class InternalEventsHandler:AttributeList
    {
        #region /// \name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Comps
        ///
        /// \brief Keys for components of one event
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum Comps
        {
            /// A EventTrigger (Conditions that has to be fulfilled in order to send the messages) 
            Trigger,

            /// An InternalEvents object (methods to be activated when the trigger hits)
            Events
        }
        #endregion
        #region /// \name Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public InternalEventsHandler():base()
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
        /// \date 09/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public InternalEventsHandler():base()
        { }
        #endregion
        #region \\\ /name Add Events
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Attribute CreateDefaultEvent()
        ///
        /// \brief Creates default event.
        ///
        /// \par Description.
        ///      Create a default event (This method is used by the GUI to add new event)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \return The new default event.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Attribute CreateDefaultEvent()
        {
            AttributeDictionary dict = new AttributeDictionary();
            dict.Add(Comps.Trigger, new Attribute { Value = new EventTrigger(), Editable = false });
            dict.Add(Comps.Events, new Attribute { Value = new InternalEvents(), ElementWindowPrmsMethod = InternalEvents.MethodsListPrms });
            return new Attribute { Value = dict };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute CreateEvent(EventTriggerType trigger, int round, dynamic eventMessageType, int eventMessageOtherEnd, List<InternalEvents.InternalEventDelegate> methods)
        ///
        /// \brief Creates an event.
        ///
        /// \par Description.
        ///      Create event with data. This method is used by the code to add events
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param trigger               (EventTriggerType) - The trigger.
        /// \param round                 (int) - The round.
        /// \param eventMessageType      (dynamic) - Type of the event message.
        /// \param eventMessageOtherEnd  (int) - The event message other end.
        /// \param methods               (InternalEventDelegate>) - The methods.
        ///
        /// \return The new event.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateEvent(EventTriggerType trigger,
            int round,
            dynamic eventMessageType,
            int eventMessageOtherEnd,
            List<InternalEvents.InternalEventDelegate> methods)
        {
            AttributeDictionary dict = new AttributeDictionary();
            EventTrigger eventTrigger = new EventTrigger(trigger, round, eventMessageType, eventMessageOtherEnd);
            dict.Add(Comps.Trigger, new Attribute { Value = eventTrigger, Editable = false });
            dict.Add(Comps.Events, new Attribute { Value = new InternalEvents(methods), ElementWindowPrmsMethod = InternalEvents.MethodsListPrms });
            Add(dict);
        }
        #endregion
        #region \\\ /name Process Events

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ProcessEvents(EventTriggerType triggerType, BaseMessage message)
        ///
        /// \brief Process the events.
        ///
        /// \par Description.
        ///      This method precess the events
        ///
        /// \par Algorithm.
        ///      -# For each event in the list
        ///         -#  If the event hits activate all the methods
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param triggerType  (EventTriggerType) - Type of the trigger.
        /// \param message      (BaseMessage) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ProcessEvents(EventTriggerType triggerType, BaseMessage message)
        {
            foreach (Attribute attribute in this)
            {
                if (attribute.Value[Comps.Trigger].True(triggerType, message))
                {
                    attribute.Value[Comps.Events].Activate();
                }
            }
        }
        #endregion
        #region /// \name ElementWindow (GUI) support
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms InternalEventPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Internal event prms.
        ///
        /// \par Description.
        ///      -  This method sets the parameters of this class representation in the GUI
        ///      -  It meant mainly to set the CreateDefaultEvent as the method to be activated  
        ///         When the Add button is pressed (When the Add button is pressed a new event
        ///         with default values will be inserted)
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

        public static ElementWindowPrms InternalEventPrms(Attribute attribute,
            dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEditable)
        {

            ElementWindowPrms elementWindowPrms = new ElementWindowPrms();
            elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.AddRemovePanel;
            elementWindowPrms.newValueControlPrms.addAttributeMethod = CreateDefaultEvent;
            return elementWindowPrms;
        }
        #endregion
    }
}
