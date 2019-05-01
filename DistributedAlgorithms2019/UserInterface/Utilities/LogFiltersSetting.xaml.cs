////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\LogFiltersSetting.xaml.cs
///
/// \brief Implements the log filters setting.xaml class.
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
    /// \class LogFiltersSetting
    ///
    /// \brief Window for setting the log filters
    ///        -#   The log filters decide which log messages will be written and displayed in the MessageWindow
    ///        -#   The log filter setting dialog contains a Select Control and buttons 
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 10/12/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class LogFiltersSetting : Window
    {
        /// \brief  (SelectControlData) - The data.
        private SelectControlData data;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public LogFiltersSetting()
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
        /// \date 10/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public LogFiltersSetting()
        {
            InitializeComponent();
            data = new SelectControlData { options = Logger.Filters };
            SelectControl.Init(data);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Save_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Save for click events.
        ///
        /// \par Description.
        ///      Set the filters in the Logger
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Logger.Filters.Clear();
            foreach (ListBoxItem item in SelectControl.ListBox_Options.Items)
            {
                Logger.Filters.Add((string)item.Content);
            }
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Remove_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Remove for click events.
        ///
        /// \par Description.
        ///      Remove a filter
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            // First set the index in order not to create an exception 
            int selectedIndex = SelectControl.ListBox_Options.SelectedIndex;
            if (selectedIndex == 0)
            {
                SelectControl.ListBox_Options.SelectedIndex = selectedIndex + 1;
            }
            else
            {
                SelectControl.ListBox_Options.SelectedIndex = selectedIndex - 1;
            }

            // Remove the item
            SelectControl.ListBox_Options.Items.RemoveAt(selectedIndex);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Reset_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Reset for click events.
        ///
        /// \par Description.
        ///      Reset the filters
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            SelectControl.Init(data);
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
        /// \date 10/12/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
