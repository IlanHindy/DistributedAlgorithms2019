////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\Events\InternalEvents.cs
///
/// \brief Implements the internal events class.
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
    /// \class InternalEvents
    ///
    /// \brief An internal events.
    ///
    /// \par Description.
    ///      -  This class is a list of entries contains names of methods in the process class   
    ///      -  The list is part of InternalEventsHandler  
    ///      -  The InternalEventHandler contains a list of events
    ///      -  The event is composed from a trigger and list of methods (This class)
    ///      -  If the event hits the methods in this class are activated
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 09/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class InternalEvents:AttributeList
    {
        #region /// \name Delegate for InternalEvent
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \class DummyForInternalEvent
        ///
        /// \brief A dummy for internal event.
        ///
        /// \par Description.
        ///      -  This class is used to identify the methods that are used as event handlers
        ///      -  The methods has this class as their first parameter so that the GUI when
        ///         searching for all the methods will be able to identify them.
        ///      -  The searching for the methods is done by searching in the process class for  
        ///         methods that fit a delegate. If the delegate will have no parameters all
        ///         the methods in the process will be collected.
        ///      -  Using this class as the first parameter to the delegate and the methods  
        ///         make it possible to filter the methods only for methods that has this class
        ///         as their only parameter  
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class DummyForInternalEvent { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public delegate void InternalEventDelegate(DummyForInternalEvent dummy);
        ///
        /// \brief Internal event delegate.
        ///        This delegate is the prototype for the methods that will be activated
        ///        when internal event occures
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param dummy  (DummyForInternalEvent) - The dummy.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public delegate void InternalEventDelegate(DummyForInternalEvent dummy);
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public InternalEvents() : base()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///      Empty constructor for use by adding InternalEvents in the GUI
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public InternalEvents() : base()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public InternalEvents(List<InternalEventDelegate> methods) : base()
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Constructor that holds methods to be activated from the code
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ///
        /// \param methods  (List&lt;InternalEventDelegate&gt;) - The methods.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public InternalEvents(List<InternalEventDelegate> methods) : base()
        {
            foreach (InternalEventDelegate method in methods)
            {
                Add(new Attribute { Value = method.Method.Name, Editable = false });
            }            
        }
        #endregion
        #region /// \name Activate the methods listed in this object

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Activate()
        ///
        /// \brief Performs this object.
        ///
        /// \par Description.
        ///      Activate all the methods in the list (By the processor)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Activate()
        {
            List<string> methods = (List<string>)AsList();
            for (int idx = 0; idx < methods.Count; idx++)
            {
                TypesUtility.InvokeMethod(Element, methods[idx], new List<object>(), 1, false);
            }
        }
        #endregion
        #region /// \name ElementWindow (GUI) support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms MethodsListPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Methods list prms.
        ///
        /// \par Description.
        ///      -  This method is used to set the presentation parameters for this class  
        ///      -  The purpose of this method is to set AddMethod method to be activating  
        ///         when pressing Add button of this list.
        ///      -  The AddMethod creates an attribute with a ComboBox containing all the possible methods 
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

        public static ElementWindowPrms MethodsListPrms(Attribute attribute,
            dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEditable)
        {

            ElementWindowPrms prms = new ElementWindowPrms();
            prms.newValueControlPrms.inputFieldType = InputFieldsType.AddRemovePanel;
            prms.newValueControlPrms.addAttributeMethod = AddMethod;
            return prms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Attribute AddMethod()
        ///
        /// \brief Adds method.
        ///
        /// \par Description.
        ///      -  This method is used to create an attribute that will be added to the list in the GUI
        ///      -  The purpose of this method is to set the MethodComBoBoxPrms for the attribute
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

        public static Attribute AddMethod()
        {
            return new Attribute { Value = "DefaultInternalEvent", ElementWindowPrmsMethod = MethodComboBoxPrms };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms MethodComboBoxPrms(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable)
        ///
        /// \brief Method combo box prms.
        ///
        /// \par Description.
        ///      -  This method is used to set the parameters for a method field in the GUI  
        ///      -  The purpose of this method is to set the options for the ComboBox as all the   
        ///         messages which has the delegate signature
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

        public static ElementWindowPrms MethodComboBoxPrms(Attribute attribute,
            dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEditable)
        {
            ElementWindowPrms elementWindowPrms = new ElementWindowPrms();
            elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            string[] options = TypesUtility.GetInternalEventMethods().ToArray();
            elementWindowPrms.newValueControlPrms.options = options;
            elementWindowPrms.newValueControlPrms.Value = options[0];
            return elementWindowPrms;
        }
        #endregion
    }
}
        