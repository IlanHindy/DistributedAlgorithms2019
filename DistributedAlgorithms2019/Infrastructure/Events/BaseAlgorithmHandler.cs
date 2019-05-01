////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Events\BaseAlgorithmHandler.cs
///
/// \brief Implements the base algorithm handler class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Windows;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BaseAlgorithmHandler
    ///
    /// \brief A base algorithm handler.
    ///
    /// \par Description.
    ///      -  This class is used in order to implement sending of a base algorithm.  
    ///      -  The following is the method that the implementation works:  
    ///         -   An object from this class is found the OperationalParameters dictionary of the process  
    ///         -   The class is composed from a list of dictionaries that contains 2 items  
    ///             -#  The trigger
    ///             -#  A list of messages to send if the trigger is hit
    ///         -   Each time the process is doing something a call to the process call the ProcessEvents method of this class  
    ///         -   The ProcessEvents method checks all the EventTriggers and if one hit all the messages  
    ///             of the event are sent  
    ///             
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 09/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class BaseAlgorithmHandler:AttributeList
    {
        #region \\\ /name Enums
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum Comps
        ///
        /// \brief Keys for components of one event
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum Comps
        {
            /// A EventTrigger (Conditions that has to be fulfilled in order to send the messages) 
            Trigger,

            /// A BaseAlgorithmMessages (Data for messages to send when the trigger hits)
            Messages
        }
        #endregion
        #region \\\ /name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseAlgorithmHandler():base()
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

        public BaseAlgorithmHandler():base()
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
            dict.Add(Comps.Messages, new Attribute { Value = new BaseAlgorithmMessages(), ElementWindowPrmsMethod = BaseAlgorithmMessages.MessageListPrms });
            return new Attribute { Value = dict, Editable = false };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute CreateEvent(EventTriggerType trigger, int round, dynamic eventMessageType, int eventMessageOtherEnd, dynamic baseMessageType, string baseMessageName, AttributeDictionary fields, AttributeList targets)
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
        /// \param baseMessageType       (dynamic) - Type of the base message.
        /// \param baseMessageName       (string) - Name of the base message.
        /// \param fields                (AttributeDictionary) - The fields.
        /// \param targets               (AttributeList) - The targets.
        ///
        /// \return The new event.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateEvent(EventTriggerType trigger,
            int round,
            dynamic eventMessageType,
            int eventMessageOtherEnd,
            BaseAlgorithmMessages baseAlgorithmMessages)
        {
            AttributeDictionary dict = new AttributeDictionary();
            EventTrigger eventTrigger = new EventTrigger(trigger, round, eventMessageType, eventMessageOtherEnd);
            dict.Add(Comps.Trigger, new Attribute { Value = eventTrigger, Editable = false });
            dict.Add(Comps.Messages, new Attribute { Value = baseAlgorithmMessages, ElementWindowPrmsMethod = BaseAlgorithmMessages.MessageListPrms });
            Add(new Attribute { Value = dict, Editable = false });
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
        ///         -#  If the event hits send all the messages 
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
            // Calculating the round
            // If the message is null we are in the Initialize trigger so the round is 0. 
            int round = 0;
            if (message is null)
            {
                round = 0;
            }
            if (Count == 0)
            {
                return;
            }
            foreach (AttributeDictionary evnt in AsList())
            {
                if (evnt[Comps.Trigger].True(triggerType, message))
                {

                    evnt[Comps.Messages].Send(((BaseProcess)Element).or[bp.ork.Round]);
                }
            }
        }
        #endregion
        #region /// \name ElementWindow (GUI) support


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms BaseAlgorithmPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Base algorithm prms.
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

        public static ElementWindowPrms BaseAlgorithmPrms(Attribute attribute,
            dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEditable)
        {
            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.AddRemovePanel;
            prms.newValueControlPrms.addAttributeMethod = CreateDefaultEvent;
            return prms;
        }
        #endregion
    }
}
