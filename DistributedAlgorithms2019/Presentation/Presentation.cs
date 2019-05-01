////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Presentation\Presentation.cs
///
///\brief   Implements the presentation class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Windows.Media.Animation;
namespace DistributedAlgorithms
{
    /*
     * The base class for presentations
     * Contains the following members
     * Canvs - the drowing canvas
     * MainWindow - the main window class
     * controls - The controls of the presentation if a control is presenting a network element it will have
     *            the same key in the dictionary as the network element
     * networkElements - the network elements that this presentation presents
     * statuses - the statusces of the network elements the key in the dictionary is the same as the 
     *            key of the network element
     */

    /**********************************************************************************************//**
     * A presentation element.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class PresentationElement
    {
        /**********************************************************************************************//**
         * Values that represent control keys.
        
         **************************************************************************************************/

        public enum ControlKeys { ArrowHead, MessageQ, Circle, TextBlock, DebugDataLabel, DebugDataCanvas, DebugDataButton, BreakpointLabel, Label, MessagesGridView }
        /** The controls. */
        public Dictionary<ControlKeys, UIElement> controls = new Dictionary<ControlKeys, UIElement>();
        /** The status. */
        public MainWindow.SelectedStatus status = MainWindow.SelectedStatus.NotSelected;
        /** The blinking story board. */
        public Storyboard blinkingStoryBoard = null;
        /** The debug window. */
        public ElementDebugWindow debugWindow = null;
        /** The breakpoint window. */
        // public BreakpointWindow breakpointWindow = null;
        public bool breakpointsVisibility = true;
    }

    /**********************************************************************************************//**
     * A presentation.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class Presentation
    {
        /**********************************************************************************************//**
         * Values that represent additional control keys.
        
         **************************************************************************************************/

        protected enum AdditionalControlKeys { Line, LabelsPanel, LabelsBorder, FloatingSummary };
        /** The canvas. */
        protected Canvas canvas;
        /** The main window. */
        protected MainWindow mainWindow;
        /** The presentation elements. */
        public Dictionary<NetworkElement, PresentationElement> presentationElements = new Dictionary<NetworkElement, PresentationElement>();
        /** The additional controls. */
        protected Dictionary<AdditionalControlKeys, UIElement> additionalControls = new Dictionary<AdditionalControlKeys, UIElement>();
        /** The selected colors. */
        protected Brush[] selectedColors = { Brushes.Blue, Brushes.Black, Brushes.Red, Brushes.Transparent, Brushes.Blue, Brushes.Black };
        protected bool showFloatingSummary = true;

        /*
         * Constructor
         */

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   mainWindow  The main window.
         * \param   canvas      The canvas.
         *                      
         **************************************************************************************************/

        public Presentation(MainWindow mainWindow, Canvas canvas)
        {
            this.canvas = canvas;
            this.mainWindow = mainWindow;
        }

        /*
         * Remove one presentation 
         * This method removes from the dictionaries the control, network element and status 
         * of the network element that has to be removed from the presentation
         * If this was the last network element of the presentation it deletes all the rest of the
         * controls from the canvas
         */

        /**********************************************************************************************//**
         * Removes the one presentation described by networkElement.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         
         **************************************************************************************************/

        public void RemoveOnePresentation(NetworkElement networkElement)
        {
            foreach (var entry in presentationElements[networkElement].controls)
            {
                if (entry.Key == PresentationElement.ControlKeys.Label)
                {
                    Label label = (Label)entry.Value;
                    ((StackPanel)additionalControls[AdditionalControlKeys.LabelsPanel]).Children.Remove(label);
                }
                else
                {
                    canvas.Children.Remove((UIElement)entry.Value);
                }
            }
            networkElement.Presentation = null;
            presentationElements.Remove(networkElement);
            if (presentationElements.Count == 0)
            {
                foreach (UIElement control in additionalControls.Values)
                {
                    canvas.Children.Remove(control);
                }
            }
            else
            {
                UpdatePresentation();
            }
        }

        /*
         * Clear the presentation
         */

        /**********************************************************************************************//**
         * Clears this object to its blank/initial state.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public void Clear()
        {
            foreach (NetworkElement networkElement in presentationElements.Keys)
            {
                RemoveOnePresentation(networkElement);
            }
        }

        /**********************************************************************************************//**
         * Creates color from enum.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   knownColor  The known color.
         *
         * \return  The new color from enum.
         **************************************************************************************************/

        public static Color CreateColorFromEnum(System.Drawing.KnownColor knownColor)
        {
            System.Drawing.Color color = System.Drawing.Color.FromKnownColor(knownColor);
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /**********************************************************************************************//**
         * Creates invisible dock panel.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  The new invisible dock panel.
         .
         **************************************************************************************************/

        protected DockPanel CreateInvisibleDockPanel()
        {
            DockPanel dockPanel = new DockPanel();
            dockPanel.Width = 0;
            dockPanel.Height = 0;
            dockPanel.LastChildFill = false;
            dockPanel.Background = Brushes.Green;
            dockPanel.VerticalAlignment = VerticalAlignment.Center;
            dockPanel.HorizontalAlignment = HorizontalAlignment.Center;
            canvas.Children.Add(dockPanel);
            return dockPanel;
        }

        /**********************************************************************************************//**
         * Sets dock panel dimention.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   mouseLocation   The mouse location.
         * \param   dockPanel       The dock panel.
         * \param   label           The label.
         * \param   button          The button control.
         
         **************************************************************************************************/

        protected void SetDockPanelDimention(Point mouseLocation, DockPanel dockPanel, Label label, Button button)
        {
            SetLabelDimentions(label);
            if (label.Width == 0)
            {
                dockPanel.Width = 0;
                dockPanel.Height = 0;
                button.Width = 0;
                button.Height = 0;
            }
            else
            {
                double top = mouseLocation.Y - label.Height - 30;
                double left = mouseLocation.X;
                dockPanel.Margin = new Thickness(left, top, 0, 0);
                label.Margin = new Thickness(0);
                dockPanel.Height = label.Height + 25;
                dockPanel.Width = label.Width;
                button.Height = 20;
                button.Width = 50;
                button.Margin = new Thickness((dockPanel.Width - button.Width) / 2, label.Height, 0, 0);
            }
        }

        /**********************************************************************************************//**
         * Creates invisible label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   dockPanel   (Optional) The dock panel.
         *
         * \return  The new invisible label.
         .
         **************************************************************************************************/

        protected Label CreateInvisibleLabel(DockPanel dockPanel = null)
        {
            Label label = new Label();
            label.Visibility = Visibility.Visible;
            label.BorderThickness = new Thickness(1);
            label.Background = Brushes.White;
            label.Width = 0;
            label.Height = 0;
            label.BorderBrush = Brushes.Black;
            label.Foreground = Brushes.Black;
            label.Background = Brushes.White;
            label.BorderThickness = new Thickness(1);
            label.MouseLeftButtonDown += label_MouseLeftButtonDown;
            if (dockPanel != null)
            {
                dockPanel.Children.Add(label);
                DockPanel.SetDock(label, Dock.Top);
            }
            else
            {
                canvas.Children.Add(label);
            }
            return label;
        }

        /**********************************************************************************************//**
         * Event handler. Called by label for mouse left button down events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse button event information.
         
         **************************************************************************************************/

        void label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var maxZ = canvas.Children.OfType<UIElement>().Where(x => x != sender).Select(x => Panel.GetZIndex(x)).Max();
            Panel.SetZIndex((Label)sender, maxZ + 1);
        }

        /**********************************************************************************************//**
         * Creates invisible button.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   dockPanel   (Optional) The dock panel.
         *
         * \return  The new invisible button.
         .
         **************************************************************************************************/

        protected Button CreateInvisibleButton(DockPanel dockPanel = null)
        {
            Button button = new Button();
            button.Content = "Dock";
            button.Width = 0;
            button.Height = 0;
            if (dockPanel != null)
            {
                dockPanel.Children.Add(button);
            }
            else
            {
                canvas.Children.Add(button);
                DockPanel.SetDock(button, Dock.Bottom);
            }
            return button;
        }

        /**********************************************************************************************//**
         * Sets label dimentions.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   label   The label.
         
         **************************************************************************************************/

        protected void SetLabelDimentions(Label label)
        {
            if (((string)(label.Content)) == "")
            {
                label.Width = 0;
                label.Height = 0;
            }
            else
            {
                FormattedText formattedText = new FormattedText(
                        (string)label.Content,
                        System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface(label.FontFamily.ToString()),
                        label.FontSize,
                        Brushes.Black);
                label.Width = formattedText.Width + 12;
                label.Height = formattedText.Height + 10;
                var maxZ = canvas.Children.OfType<UIElement>().Where(x => x != label).Select(x => Panel.GetZIndex(x)).Max();
                Panel.SetZIndex(label, maxZ + 1);
            }
        }

        /*
         * Set the status of the network element and paint the presentation accordinally
         */

        /**********************************************************************************************//**
         * Sets a selected.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   element The element.
         * \param   status  The status.
         
         **************************************************************************************************/

        public virtual void SetSelected(NetworkElement element, MainWindow.SelectedStatus status)
        { }

        /*
         * Update the presentation after change in the attributes of the network element
         */

        /**********************************************************************************************//**
         * Updates the presentation.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public virtual void UpdatePresentation()
        { }

        /*
         * Update the network element after change in the presentation
         */

        /**********************************************************************************************//**
         * Updates the network element described by networkElement.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         
         **************************************************************************************************/

        protected virtual void UpdateNetworkElement(NetworkElement networkElement)
        {
            mainWindow.networkWasChanged = true;
        }

        #region /// \name Floating Summary
        protected void CreateFloatingSummary()
        {
            Visibility gridVisibility = Visibility.Hidden;
            if (additionalControls.ContainsKey(AdditionalControlKeys.FloatingSummary))
            {
                gridVisibility = additionalControls[AdditionalControlKeys.FloatingSummary].Visibility;
                canvas.Children.Remove(additionalControls[AdditionalControlKeys.FloatingSummary]);
            }

            Grid grid = new Grid();
            for (int colIdx = 0; colIdx < presentationElements.Count * 2; colIdx++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            int rowIdx;
            int elementIdx = -1;
            string keyString = "";
            string valueString = "";
            string separator = "";
            foreach (NetworkElement element in presentationElements.Keys)
            {
                elementIdx++;
                rowIdx = 0;
                AddToFloatingSummary(grid, rowIdx, elementIdx * 2, "Element Name");
                AddToFloatingSummary(grid, rowIdx, elementIdx * 2 + 1, element.ToString());
                List<AttributeDictionary> dictionaries = new List<AttributeDictionary> { element.pa, element.or };
                foreach (AttributeDictionary dictionary in dictionaries)
                {
                    foreach (var entry in dictionary)
                    {
                        if (entry.Value.ShortDescription(entry.Key, ref keyString, ref separator, ref valueString))
                        {
                            rowIdx++;
                            AddToFloatingSummary(grid, rowIdx, elementIdx * 2, keyString);
                            AddToFloatingSummary(grid, rowIdx, elementIdx * 2 + 1, valueString);
                        }
                    }
                }
            }
            canvas.Children.Add(grid);
            additionalControls[AdditionalControlKeys.FloatingSummary] = grid;
            PositionFloatingSummary(grid);
            grid.Visibility = gridVisibility;
            grid.Background = Brushes.White;
        }

        protected void PositionFloatingSummary(Grid grid)
        {
            ScrollViewer scrollViewer = mainWindow.ScrollViewer_canvas;
            double presentedAreaRight = scrollViewer.VerticalOffset + scrollViewer.ActualWidth;
            double presentedAreaTop = scrollViewer.HorizontalOffset;
            grid.UpdateLayout();
            FrameworkElement elementObject = ElementObject();
            double elementCenterX = elementObject.Margin.Left + elementObject.ActualWidth / 2;
            double elementCenterY = elementObject.Margin.Top + elementObject.ActualHeight / 2;
            if (elementCenterY - grid.ActualHeight < presentedAreaTop)
            {
                elementCenterY = presentedAreaTop + grid.ActualHeight + 10;
            }

            if (elementCenterX + grid.ActualWidth > presentedAreaRight)
            {
                elementCenterX = presentedAreaRight - grid.ActualWidth - 10;
            }
            grid.Margin = new Thickness(elementCenterX, elementCenterY - grid.ActualHeight, elementCenterX + grid.ActualWidth, elementCenterY);
        }

        public void FloatingSummary_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
            {
                if (additionalControls.ContainsKey(AdditionalControlKeys.FloatingSummary))
                {
                    Grid grid = (Grid)((TextBlock)sender).Parent;
                    double mouseXPosition = e.GetPosition(canvas).X;
                    double mouseYPosition = e.GetPosition(canvas).Y;


                    additionalControls[AdditionalControlKeys.FloatingSummary].Visibility = Visibility.Hidden;
                }
            }
        }

        public void AddToFloatingSummary(Grid grid, int rowIdx, int colIdx, string text)
        {
            if (grid.RowDefinitions.Count <= rowIdx)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            Grid.SetColumn(border, colIdx);
            Grid.SetRow(border, rowIdx);
            grid.Children.Add(border);
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            if (rowIdx == 0)
            {
                textBlock.FontWeight = FontWeights.Bold;
            }
            textBlock.Margin = new Thickness(3);
            border.Child = textBlock;
            textBlock.MouseLeave += new MouseEventHandler(FloatingSummaryControl_MouseLeave);
            border.MouseLeave += new MouseEventHandler(FloatingSummaryControl_MouseLeave);
        }

        protected void FloatingSummaryControl_MouseLeave(object sender, MouseEventArgs e)
        {
            HideFloatingSummaryIfNeeded(e);
        }

        protected virtual FrameworkElement ElementObject()
        {
            return null;
        }

        protected void ShowFloatingSummary()
        {
            if (mainWindow.showFloatingSummary)
            {
                if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
                {
                    if (additionalControls.ContainsKey(AdditionalControlKeys.FloatingSummary))
                    {
                        Grid grid = (Grid)additionalControls[AdditionalControlKeys.FloatingSummary];
                        grid.Visibility = Visibility.Visible;
                        var maxZ = canvas.Children.OfType<UIElement>()
                            .Where(x => x != grid)
                            .Select(x => Panel.GetZIndex(x))
                            .Max();
                        Panel.SetZIndex(grid, maxZ + 1);
                    }
                }
            }
        }
        protected void HideFloatingSummaryIfNeeded(MouseEventArgs e)
        {
            if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
            {
                if (additionalControls.ContainsKey(AdditionalControlKeys.FloatingSummary))
                {
                    bool mouseInObject = ElementObject().IsMouseOver;
                    Grid grid = (Grid)additionalControls[AdditionalControlKeys.FloatingSummary];
                    bool mouseInGrid = grid.IsMouseOver;
                    if (!mouseInObject && !mouseInGrid)
                    {
                        grid.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        #endregion

        /*
         * Update the running status of the network element
         */

        /**********************************************************************************************//**
         * Updates the running status.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         * \param   parameters      Options for controlling the operation.
         
         **************************************************************************************************/

        public virtual void UpdateRunningStatus(NetworkElement networkElement, object[] parameters)
        { }

        /*
         * EndRunningActions
         */

        /**********************************************************************************************//**
         * Updates the end running described by networkElement.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         
         **************************************************************************************************/

        public virtual void UpdateEndRunning(NetworkElement networkElement)
        {
            foreach (PresentationElement presentationElement in presentationElements.Values)
            {
                if (presentationElement.debugWindow != null)
                {
                    presentationElement.debugWindow.Close();
                    presentationElement.debugWindow = null;
                }
                //if (presentationElement.breakpointWindow != null)
                //{
                //    presentationElement.breakpointWindow.Close();
                //    presentationElement.breakpointWindow = null;
                //}
            }
            if (additionalControls[AdditionalControlKeys.FloatingSummary] != null)
            {
                canvas.Children.Remove(additionalControls[AdditionalControlKeys.FloatingSummary]);
                additionalControls.Remove(AdditionalControlKeys.FloatingSummary);
            }
        }

        public void SetShowFloatingSummary(bool newValue)
        {
            if (newValue == false)
            {
                Grid grid = (Grid)additionalControls[AdditionalControlKeys.FloatingSummary];
                grid.Visibility = Visibility.Hidden;
            }
            showFloatingSummary = newValue;
        }
    }
}
