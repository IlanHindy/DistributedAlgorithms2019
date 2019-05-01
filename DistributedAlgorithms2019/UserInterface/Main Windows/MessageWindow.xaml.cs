////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\DebugWindow.xaml.cs
///
///\brief   Implements the debug window.xaml class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * Interaction logic for DebugWindow.xaml. This window Print messages on the running messages
     * window In order to send message to this window : 1. Send log with LogMode :
     *     MainLogAndMessageTrace MainLogAndProcessLogAndMessageTrace
     * 2. Directly to this window using the static MessagsRouter.ReportMessage(string sender, string
     * message)
     * 
     * The featurs of the window : 1. Present all messages received 2. Filter the messages according
     * to the sender/s 3. Filter the messages according to one or multipile strings (and or or
     * action between them)
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/
    
    public partial class MessageWindow : Window
    {
        #region /// \name class declarations and construction
        /** List of the current senders from which messages are presented. */
        private List<string> presentedSenders = new List<string>();
        /** Enum for the filter according to sender status. */
        private enum PresentedSenderStatus { NotFiltered, Filtered}

        /** the status of the filter according to sender. */
        private PresentedSenderStatus presentedSenderStatus = PresentedSenderStatus.NotFiltered;

        /** \brif list of filters according to string. */
        private List<string> filters = new List<string>();

        private MainWindow mainWindow;

        /**********************************************************************************************//**
         * Default constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public MessageWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();

        }
        #endregion

        #region /// \name Insert New Messages

        /**********************************************************************************************//**
         * Print debug message.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender      The sender of the message.
         * \param   vectorClock The vector clock.
         * \param   message     The message to disply.
         *                      
         **************************************************************************************************/

        public void PrintDebugMessage(string sender, string vectorClock, string message)
        {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = sender + " : " + vectorClock + " " + message;
                ListBox_Messages.Items.Add(listBoxItem);

                // Update presented sources
                // If the presentedSenders is in status NotFiltered - show the item 
                // Else show the item only if the sender is in the presented senders
                if (presentedSenderStatus == PresentedSenderStatus.Filtered)
                {
                    if (!presentedSenders.Any(s => s == sender))
                    {
                        listBoxItem.Visibility = Visibility.Collapsed;
                    }
                }

                // If the filter according to sender are in status NotFiltered
                // And the sender is new add the sender to the list of presented senders
                else
                {
                    if (!presentedSenders.Contains(sender))
                    {
                        presentedSenders.Add(sender);
                    }
                }

                // Scroll to the last element
                ListBox_Messages.SelectedIndex = ListBox_Messages.Items.Count - 1;
                ListBox_Messages.ScrollIntoView(ListBox_Messages.SelectedItem);
            }
 
        #endregion
        #region /// \name Reset Presentation from filters

        /**********************************************************************************************//**
         * Event handler. Called by Button_Reset for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
           foreach (ListBoxItem item in ListBox_Messages.Items)
            {
                item.Visibility = Visibility.Visible;
            }
        }

        #endregion
        #region /// \name Filter according to senders

        /**********************************************************************************************//**
         * Event handler. Called by Button_PresentedSources_List for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_PresentedSources_List_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve all senders to list
            List<string> senders = ListBox_Messages.Items.Cast<ListBoxItem>().ToList()
                .Select(item => SenderOfItem(item))
                .ToList().Distinct().ToList();

            // Select the senders to present
            SelectDialog selectDialog = new SelectDialog("Select the sources",
                "DebugWindow Window Select Source", senders, null, null, SelectionMode.Multiple);
            selectDialog.ShowDialog();

            if (selectDialog.Result == SelectDialog.SelectDialogResult.Quit)
            {
                return;
            }

            // Set the presentedSenders according to the selected
            presentedSenders = senders.Select((s, idx) => new { s, idx }).
                Where(p => selectDialog.Selection.IndexOf(p.idx) > -1).
                Select(p => p.s).ToList();

            // presented sender status only messages from the selected senders will be shown
            presentedSenderStatus = PresentedSenderStatus.Filtered;

            // Refresh the presentation
            NewPresentedSendersSelected();
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_CancelPresentedSourcesFilter for click events.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         **************************************************************************************************/

        private void Button_CancelPresentedSourcesFilter_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve all senders to list
            List<string> presentedSenders = ListBox_Messages.Items.Cast<ListBoxItem>().ToList()
                .Select(item => SenderOfItem(item))
                .ToList().Distinct().ToList();

            // Presented sender status - all messages will be shown and new senders will be added to the list 
            presentedSenderStatus = PresentedSenderStatus.NotFiltered;

            // Refresh the presentation
            NewPresentedSendersSelected();
        }

        /**********************************************************************************************//**
         * Creates a new presented senders selected.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         **************************************************************************************************/

        private void NewPresentedSendersSelected()
        {
            // Update the prresentation
            foreach (ListBoxItem item in ListBox_Messages.Items)
            {
                if (presentedSenders.Any(s => ((string)item.Content).Substring(0, ((string)item.Content).IndexOf(" : ")) == s))
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }

        /**********************************************************************************************//**
         * Sender of item.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   item    The item.
         *
         * \return  A string.
         **************************************************************************************************/

        private string SenderOfItem(ListBoxItem item)
        {
            return ((string)item.Content).Substring(0, ((string)item.Content).IndexOf(" : "));
        }

        #endregion

        #region /// \name Filter according to strings in message

        /**********************************************************************************************//**
         * Event handler. Called by Button_Filter_And for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Filter_And_Click(object sender, RoutedEventArgs e)
        {
            ListInput filtersWindow = new ListInput(filters, null);
            filtersWindow.ShowDialog();
            foreach (ListBoxItem item in ListBox_Messages.Items)
            {
                // If the item should be presented according to the presentedSenders
                if (presentedSenders.Any(s => SenderOfItem(item) == s))
                {
                    if (filters.All(f => ((string)item.Content).Contains(f)))
                    {
                        item.Visibility = Visibility.Visible;
                        continue;
                    }
                }
                item.Visibility = Visibility.Collapsed;
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Filter_Or for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Filter_Or_Click(object sender, RoutedEventArgs e)
        {
            ListInput filtersWindow = new ListInput(filters, null);
            filtersWindow.ShowDialog();
            foreach (ListBoxItem item in ListBox_Messages.Items)
            {
                // If the item should be presented according to the presentedSenders
                if (presentedSenders.Any(s => SenderOfItem(item) == s))
                {
                    if (filters.All(f => ((string)item.Content).Contains(f)))
                    {
                        item.Visibility = Visibility.Visible;
                        continue;
                    }
                }
                item.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
        #region /// \name Close activities

        /**********************************************************************************************//**
         * Raises the system. component model. cancel event.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Event information to send to registered event handlers.
         
         **************************************************************************************************/

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //CustomizedMessageBox.Show("The window cannot be closed while running", "Debug Window Message", Icons.Error);
            //e.Cancel = true;
            mainWindow.messageWindow = null;
        }

        /**********************************************************************************************//**
         * Closes the window.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public void CloseWindow()
        {
            this.Closing -= OnClosing;
            Close();
        }
        #endregion
    }
}
