////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\ElementInputWindow.cs
///
///\brief   Implements the element input Windows Form.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DistributedAlgorithms.Algorithms.Base.Base;


namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ElementInputWindow
    ///
    /// \brief Form for viewing the element input.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 04/07/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ElementInputWindow : ElementWindow
    {
        #region /// \name Members

        /// \brief /** true if element was changed.
        public bool elementWasChanged = false;
        #endregion

        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ElementInputWindow(List<NetworkElement> networkElements) : base(networkElements, InputWindows.InputWindow, true, true, true)
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

        public ElementInputWindow(List<NetworkElement> networkElements)
            : base(networkElements, InputWindows.InputWindow)
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
            CreateButton("Button_ResetToInit", "Reset To initialize Values", Button_ResetToInit_Click);
            CreateButton("Button_ResetToSaved", "Reset To Saved Values", Button_ResetToSaved_Click);
            CreateButton("Button_UpdateNetworkElement", "Update Network Element", Button_UpdateNetworkElement_Click);
            CreateButton("Button_Check", "Check", Button_Check_Click);
            CreateButton("Button_Exit", "Exit", Button_Exit_Click);
            elementWasChanged = false;
        }
        #endregion
        #region /// \name Buttons handelers 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_ResetToInit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ResetToInit for click events.
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

        private void Button_ResetToInit_Click(object sender, RoutedEventArgs e)
        {
            ResetToInit();
            CustomizedMessageBox.Show("Reset To Init Ended", "Element Input Window Message", Icons.Success);
            elementWasChanged = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_ResetToSaved_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_ResetToSaved for click events.
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

        private void Button_ResetToSaved_Click(object sender, RoutedEventArgs e)
        {
            ResetToExisting();
            CustomizedMessageBox.Show("Reset To Saved Ended", "Element Input Window Message", Icons.Success);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_UpdateNetworkElement_Click(object sender, RoutedEventArgs e)
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

        private void Button_UpdateNetworkElement_Click(object sender, RoutedEventArgs e)
        {
            UpdateNetworkElement();
            CustomizedMessageBox.Show("Update Network Element Ended", "Element Input Window Message", Icons.Success);
            elementWasChanged = true;
        }

        private void Button_Check_Click(object sender, RoutedEventArgs e)
        {
            CheckNetworkElements();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Exit_Click(object sender, RoutedEventArgs e)
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

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            bool elementWasSaved = Exit();
            elementWasChanged = elementWasChanged || elementWasSaved;
        }

        //protected override ControlsAttributeLink CreateAttribute(int index, bool createKey)
        //{
        //    ControlsAttributeLink link;
        //    if (TypesUtility.CompareDynamics(currentComplexItem.key, bp.ork.GenerateBaseMessageData))
        //    {
        //        link = CreateBaseMessageData(index);
        //    }
        //    else
        //    {
        //        link = base.CreateAttribute(index, createKey);
        //    }
        //    return link;
        //}

        //protected ControlsAttributeLink CreateBaseMessageData(List<List<dynamic>> existingKeyList, int index)
        //{
        //    allowAddRemoveAttributes = false;
        //    string messageTypesEnumName = "DistributedAlgorithms." +
        //        "Algorithms." +
        //        Config.Instance[Config.Keys.SelectedSubject] + "." +
        //        Config.Instance[Config.Keys.SelectedAlgorithm] + "." +
        //        Config.Instance[Config.Keys.SelectedAlgorithm] + "Message" +
        //        "+MessageTypes";
        //    string x = (typeof(ChandyLamportMessage.MessageTypes)).ToString();
        //    Type messageTypesEnum = TypesUtility.GetTypeFromString(messageTypesEnumName);
        //    dynamic defaultMessageType = TypesUtility.GetAllEnumValues(messageTypesEnum)[0];
        //    AttributeDictionary data = BaseProcess.CreateBaseMessageEvent(BaseProcess.BaseAlgorithmEvent.UserEvent,
        //         0, defaultMessageType, "BaseMessage", new AttributeDictionary(), new AttributeList(), defaultMessageType, 0);
        //    ControlsAttributeLink newLink = CreateNewItem(new Attribute { Value = data },
        //        index.ToString(),
        //        currentComplexItem.attributeDictionary,
        //        index);
        //    return newLink;

        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected ControlsAttributeLink CreateBaseMessageData(int index)
        ///
        /// \brief Creates base message data.
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
        /// \param index (int) - Zero-based index of the.
        ///
        /// \return The new base message data.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink CreateBaseMessageData(int index)
        {
            return null;
        }
        #endregion
    }
}
