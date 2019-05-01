////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\SelectControl.xaml.cs
///
/// \brief Implements the selectcontrol.xaml class.
///        A control for selecting from list of strings
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class SelectControlData
    ///
    /// \brief Interaction logic for SelectControl.xaml.
    ///
    /// \par Description.
    ///      This class holds the data needed to create the control 
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 29/08/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    #region /// \name SelectControlData
    public class SelectControlData
    {
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;string&gt;) - Options for controlling the operation.
        ///        The list to select from.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<string> options;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;int&gt;) - The initially selected.
        ///        List of entries of options that should be selected when the control is created.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<int> initiallySelected = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;bool&gt;) - The enable items.
        ///        In this list each option that has to be enabled get's true in it's entry (else gets false)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<bool> enableItems = null;     
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class SelectResults
    ///
    /// \brief A select results.
    ///
    /// \par Description.
    ///      This class holds the results of the selected and it filled when the dialog terminates
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 29/08/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////
     #region /// \name SelectResults class
    public class SelectResults
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;int&gt;) - The selection.
        ///        A list of selected indexes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<int> selection = new List<int>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (string) - The selection text.
        ///        In case the selection is single - the selectionText.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string selectionText = "";
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class SelectControl
    ///
    /// \brief A select control.
    ///
    /// \par Description.
    ///      This control is used for selection from a list of strings
    ///
    /// \par Usage Notes.
    ///      -  The following are the attributes of the control  
    ///         -#  EnableTextBox  
    ///         -#  SelectionMode
    ///         -#  Message
    ///      -  The following are the events of the control  
    ///         -#  SelectionChanged in the list box
    ///         -#  NewItemCreated (When inserting value in the TextBox if the text is part of 
    ///             an existing item that item is selected. else a new item is opened and this
    ///             event is fired)
    ///
    /// \author Ilanh
    /// \date 29/08/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class SelectControl : UserControl
    {
#region /// \name Events fired by this control
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (event EventHandler) - Event queue for all listeners interested in SelectionChanged events.
        ///        This EventHandler is used to specify the behavior when the selection changed in the ListBox
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler SelectionChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (event EventHandler) - Event queue for all listeners interested in NewItemCreated events.
        ///        This event is fired when a new item was added to the ListBox.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler NewItemCreated;
        #endregion
        #region /// \name Attributes of the control

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public bool EnableTextBox
        ///
        /// \brief Gets or sets a value indicating whether the text box is enabled.
        ///        This is an attribute of the control indicating whether the TextBox will be enabled
        ///
        /// \return True if enable text box, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool EnableTextBox
        {
            get { return (bool)GetValue(EnableTextBoxProperty); }
            set { SetValue(EnableTextBoxProperty, value); }
        }

        
        public static readonly DependencyProperty EnableTextBoxProperty =
            DependencyProperty.Register(
                "EnableTextBox", 
                typeof(bool), 
                typeof(SelectControl), 
                new PropertyMetadata(false));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public bool DisableSelection
        ///
        /// \brief Gets or sets a value indicating whether the selection is disabled.
        ///
        /// \return True if disable selection, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool DisableSelection
        {
            get { return (bool)GetValue(DisableSelectionProperty); }
            set { SetValue(DisableSelectionProperty, value); }
        }

        public static readonly DependencyProperty DisableSelectionProperty =
            DependencyProperty.Register(
                "DisableSelection",
                typeof(bool),
                typeof(SelectControl),
                new PropertyMetadata(false));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public SelectionMode SelectionMode
        ///
        /// \brief Gets or sets the selection mode.
        ///        This is an attribute of the control for inserting the selection mode (Single or Multiple)
        ///
        /// \return The selection mode.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(SelectionMode),
                typeof(SelectControl),
                new PropertyMetadata(SelectionMode.Single));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public string Message
        ///
        /// \brief Gets or sets the message.
        ///        This is an attribute of the control to insert the header text of the control
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(SelectControl),
                new PropertyMetadata(""));

        
        #endregion
        #region /// \name Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (SelectControlData) - The data.
        ///        The data used to build the window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectControlData data;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (SelectResults) - The select results.
        ///        A member used to hold the results of the selection.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SelectResults selectResults = new SelectResults();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ListBoxItem) - The new item.
        ///        -   When inserting text to the text box the following algorithm takes place:
        ///            -   If the text is a part of one of the items that item is selected
        ///            -   Else a new item is opened with the text  
        ///        -   Therefore there might be a need for deleting the new item (for example if  
        ///            backspace was pressed).
        ///        -   This member holds that item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ListBoxItem newItem = new ListBoxItem();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to disable, false to enable the selected item event.
        ///        -   When inserting text to the TextBox the ListBox is changed.
        ///        -   This parameter is used to disable the SelectedChanged event while the list changes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool disableSelectedItemEvent = false;
        #endregion
        #region /// \name Constructor and Init

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public SelectControl()
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
        /// \date 29/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SelectControl()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Init(SelectControlData data)
        ///
        /// \brief Initializes the given data.
        ///
        /// \par Description.
        ///      Build the control and fill it with data
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param data  (SelectControlData) - The data.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Init(SelectControlData data)
        {
            this.data = data;
            Label_Text.Content = Message;
            TextBox_Selected.Text = "";

            // if enabledItem is null - set all items enabled
            if (DisableSelection)
            {
                data.enableItems = Enumerable.Range(0, data.options.Count).Select(i => false).ToList();
            }
            else
            {
                if (data.enableItems == null)
                {
                    data.enableItems = Enumerable.Range(0, data.options.Count).Select(i => true).ToList(); ;
                }
            }

            // disable the event (in order not to trigger while initiating)
            ListBox_Options.SelectionChanged -= ListBox_Options_SelectionChanged;

            // the clear is needed because the method might be called for update
            ListBox_Options.Items.Clear();

            // fill the list box
            for (int idx = 0; idx < data.options.Count; idx++)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = data.options[idx];
                listBoxItem.IsEnabled = data.enableItems[idx];
                ListBox_Options.Items.Add(listBoxItem);                
            }
            ListBox_Options.SelectionMode = SelectionMode;

            // enable the trigger in order that it will be relevant when init of the selection 
            ListBox_Options.SelectionChanged += ListBox_Options_SelectionChanged;

            // if the initially selected is not null select all the initially selected
            if (data.initiallySelected != null)
            {
                // If the mode is single - set the selection index to the first option in the list
                if (SelectionMode == SelectionMode.Single)
                {
                    ListBox_Options.SelectedIndex = data.initiallySelected[0];
                    
                }

                // if the selection mode is multiple - Set all the selected indexes
                else
                {
                    foreach (int selectedItemIndex in data.initiallySelected)
                    {
                        ListBox_Options.SelectedItems.Add(ListBox_Options.Items[selectedItemIndex]);
                    }
                }
            }

            // if the initially selected is null - select the first enabled item which
            else
            {
                for (int idx = 0; idx < ListBox_Options.Items.Count; idx ++)
                {
                    if (((ListBoxItem)ListBox_Options.Items[idx]).IsEnabled)
                    {
                        ListBox_Options.SelectedIndex = idx;
                        break;
                    }
                }                
            }

            // Disable the TextBlock if needed
            TextBox_Selected.IsEnabled =  EnableTextBox;

            // if the selection mode is multiply - remove the TextBox
            if (SelectionMode == SelectionMode.Multiple)
            {
                TextBox_Selected.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
        #region /// \name Event handlers

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ListBox_Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ListBox_Options for selection changed events.
        ///
        /// \par Description.
        ///      -  This event handler is activated when the selection in the ListBox was changed  
        ///      -  It does :  
        ///         -#  Set the value in the TextBox
        ///         -#  Activate the SelectionChanged EventHandler
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ListBox_Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (disableSelectedItemEvent)
                return;

            if (ListBox_Options.SelectionMode == SelectionMode.Single)
            {
                TextBox_Selected.Text = ((string)((ListBoxItem)ListBox_Options.SelectedItem).Content).ToString().Replace(ListBox_Options.SelectedItem.GetType().ToString() + ": ", "");
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void TextBox_Selected_KeyUp(object sender, KeyEventArgs e)
        ///
        /// \brief Event handler. Called by TextBox_Selected for key up events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///      -  If CR was pressed - start a new item insertion
        ///      -  If the text is part of an existing item - select that item  
        ///      -  Else put the text in a new item
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/11/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (KeyEventArgs) - Key event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void TextBox_Selected_KeyUp(object sender, KeyEventArgs e)
        {
            string text = TextBox_Selected.Text;
            if (e.Key == Key.Enter)
            {
                // If the text does not exist in the list - add it to the list
                bool textInListBox = false;
                foreach (ListBoxItem item in ListBox_Options.Items)
                {                
                    if ((string)item.Content == text)
                    {
                        textInListBox = true;
                        break;
                    }
                }
                if (!textInListBox)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = text;
                    ListBox_Options.Items.Add(item);
                    disableSelectedItemEvent = true;
                    ListBox_Options.SelectedItem = item;
                    disableSelectedItemEvent = false;
                }


                // Create a new empty item
                TextBox_Selected.Text = "";
                newItem = new ListBoxItem();
                newItem.Content = "";
                return;
            }
            
            // For all other inputs
            // Remove the new item from the list
            // If the new item string is part of an existing item - mark this item
            // Else Add the new item to the list
            disableSelectedItemEvent = true;
            ListBox_Options.Items.Remove(newItem);

            foreach (ListBoxItem item in ListBox_Options.Items)
            {
                if (((string)item.Content).IndexOf(text) == 0)
                {
                    ListBox_Options.SelectedItem = item;
                    return;
                }
            }
            newItem.Content = text;
            ListBox_Options.Items.Add(newItem);
            ListBox_Options.SelectedItem = newItem;
            NewItemCreated?.Invoke(null, null);
            disableSelectedItemEvent = false;
        }
        #endregion
        #region /// \name Get the results of the control

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public SelectResults GetSelection()
        ///
        /// \brief Gets the selection.
        ///
        /// \par Description.
        ///      This method fills the results 
        ///      
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      This method should be activated by containing window when it's OK button is pressed
        ///
        /// \author Ilanh
        /// \date 29/08/2017
        ///
        /// \return The selection.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SelectResults GetSelection()
        {
            foreach (ListBoxItem item in ListBox_Options.SelectedItems)
            {
                selectResults.selection.Add(ListBox_Options.Items.IndexOf(item));
            }
            selectResults.selectionText = TextBox_Selected.Text;
            return selectResults;
        }
        #endregion
    }
}
