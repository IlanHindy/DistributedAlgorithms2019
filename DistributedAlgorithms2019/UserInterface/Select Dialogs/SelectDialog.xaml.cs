////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file userinterface\SelectDialog.xaml.cs
///
/// \brief Implements the SelectDialog.xaml class.
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
using System.Windows.Shapes;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class SelectDialog
    ///
    /// \brief Dialog for setting the select.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 20/02/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class SelectDialog : Window
    {
        #region

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum SelectDialogResult
        ///
        /// \brief Values that represent select dialog results.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum SelectDialogResult {Quit, Select};
        #endregion
        #region

        /// \brief The selection result.
        public List<int> Selection = new List<int>();

        /// \brief The selection text result.
        public string SelectionText = "";

        /// \brief The dialog exit action.
        public SelectDialogResult Result = SelectDialogResult.Quit;
        
        /// \brief The selection mode.
        private SelectionMode selectionMode;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public SelectDialog(string message, string title, List<string> options, List<int> initialySelected = null, List<bool> enableItems = null, SelectionMode selectionMode = SelectionMode.Single)
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
        /// \date 20/02/2018
        ///
        /// \param message           (string) - The message.
        /// \param title             (string) - The title.
        /// \param options           (List&lt;string&gt;) - Options for controlling the operation.
        /// \param initialySelected (Optional)  (List&lt;int&gt;) - The initially selected.
        /// \param enableItems      (Optional)  (List&lt;bool&gt;) - The enable items.
        /// \param selectionMode    (Optional)  (SelectionMode) - The selection mode.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SelectDialog(string message, string title, List<string> options, List<int> initiallySelected = null, List<bool> enableItems = null, SelectionMode selectionMode = SelectionMode.Single)
        {
            InitializeComponent();
            this.selectionMode = selectionMode;
            Title = title;
            SelectCtrl.Message = message;
            SelectCtrl.SelectionMode = selectionMode;
            SelectControlData data = new SelectControlData();
            data.enableItems = enableItems;
            data.initiallySelected = initiallySelected;
            data.options = options;            
            SelectCtrl.Init(data);            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Quit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Quit for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 20/02/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            Selection.Add(0);
            Result = SelectDialogResult.Quit;
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Select_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Select for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 20/02/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBoxItem item in SelectCtrl.ListBox_Options.SelectedItems)
            {
                Selection.Add(SelectCtrl.ListBox_Options.Items.IndexOf(item));
            }
            SelectionText = SelectCtrl.TextBox_Selected.Text;
            Result = SelectDialogResult.Select;
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ListBox_Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ListBox_Options for selection changed events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 20/02/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ListBox_Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectCtrl.ListBox_Options.SelectionMode == SelectionMode.Single)
            {
                SelectCtrl.TextBox_Selected.Text = SelectCtrl.ListBox_Options.SelectedItem.ToString().Replace(SelectCtrl.ListBox_Options.SelectedItem.GetType().ToString() + ": ", "");
            }
        }
    }
}
