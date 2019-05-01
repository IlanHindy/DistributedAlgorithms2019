////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Presentation\ProcessPresentation.cs
///
///\brief   Implements the process presentation class.
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
using System.Runtime.Remoting;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Windows.Media.Animation;
//using System.Drawing;

namespace DistributedAlgorithms
{

    /*
     * Presentation of a process
     * The presenation contains 2 controls:
     * A circle
     * A textBlock
     */

    /**********************************************************************************************//**
     * The process presentation.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class ProcessPresentation : Presentation, IScanConsumer
    {
        /**********************************************************************************************//**
         * Values that represent presentation update actions.
        
         **************************************************************************************************/

        public enum PresentationUpdateAction { RunningUpdate, BreakpointsEvaluationUpdate}
        /** true if this object is in placing process. */
        private bool isInPlacingProcess = false;
        /** true to in resize process. */
        private bool inResizeProcess = false;
        /** The distance from center. */
        private double distanceFromCenter = 0;
        /** The breakpoint label string. */
        private string breakpointLabelString;
        /** The breakpoints label string indent. */
        private string breakpointsLabelStringIndent;

        /*
         * Constructor
         */

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   canvas      The canvas.
         * \param   process     The process.
         * \param   mainWindow  The main window.
         *                      
         **************************************************************************************************/

        public ProcessPresentation(Canvas canvas, BaseProcess process, MainWindow mainWindow) : base(mainWindow, canvas)
        {
            Ellipse circle;
            PresentationElement presentationElement = new PresentationElement();
            presentationElement.controls.Add(PresentationElement.ControlKeys.Circle, circle = CreateCircle(process));
            presentationElement.controls.Add(PresentationElement.ControlKeys.TextBlock,CreateTextBlock(process));
            presentationElement.controls.Add(PresentationElement.ControlKeys.BreakpointLabel, CreateInvisibleLabel());
            presentationElement.status = MainWindow.SelectedStatus.Selected;
            presentationElement.blinkingStoryBoard = CreateBlinkingStoryBoard(circle);
            SetTextBlockDimentions(presentationElement);
            MoveTextBlockInCircle(presentationElement);
            presentationElements.Add(process, presentationElement);
            SetBlinkingForAllProcesses();
        }

        /*
         * Create a circle control from the following parameters (found in the network element:
         * top, Left, Width, Height
         */

        /**********************************************************************************************//**
         * Creates a circle.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process The process.
         *
         * \return  The new circle.
         .
         **************************************************************************************************/

        private Ellipse CreateCircle(BaseProcess process)
        {
            Ellipse circle = new Ellipse();
            canvas.Children.Add(circle);
            circle.Width = process.pp[bp.ppk.FrameWidth];
            circle.Height = process.pp[bp.ppk.FrameHeight];
            double circleLeft = process.pp[bp.ppk.FrameLeft];
            double circleTop = process.pp[bp.ppk.FrameTop];
            circle.Fill = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.Background]));
            circle.Margin = new Thickness(circleLeft, circleTop, 0, 0);          
            circle.Visibility = Visibility.Visible;
            circle.StrokeThickness = 2;
            circle.MouseDown += new MouseButtonEventHandler(Circle_MouseDown);
            circle.MouseMove += new System.Windows.Input.MouseEventHandler(Circle_MouseMove);
            circle.MouseUp += new MouseButtonEventHandler(Circle_MouseUp);
            circle.MouseEnter += new MouseEventHandler(Circle_MouseEnter);
            circle.MouseLeave += new MouseEventHandler(Circle_MouseLeave);
            return circle;
        }

        /**********************************************************************************************//**
         * Creates blinking story board.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   circle  The circle.
         *
         * \return  The new blinking story board.
         .
         **************************************************************************************************/

        private Storyboard CreateBlinkingStoryBoard(Ellipse circle)
        {
            var blinkAnimation = new DoubleAnimationUsingKeyFrames();
            blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250))));

            var blinkStoryboard = new Storyboard()
            {
                Duration = TimeSpan.FromMilliseconds(1000),
                RepeatBehavior = RepeatBehavior.Forever,
            };

            Storyboard.SetTarget(blinkAnimation, circle);
            Storyboard.SetTargetProperty(blinkAnimation, new PropertyPath(Ellipse.OpacityProperty));

            blinkStoryboard.Children.Add(blinkAnimation);
            return blinkStoryboard;
        }


        /*
         * Create a textBlock control
         * The textBlock control resids inside the circle and have the same events as the circle
         */

        /**********************************************************************************************//**
         * Creates text block.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process The process.
         *
         * \return  The new text block.
         .
         **************************************************************************************************/

        private TextBlock CreateTextBlock(BaseProcess process)
        {
            TextBlock textBlock = new TextBlock();
            canvas.Children.Add(textBlock);
            textBlock.Background = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.Background]));
            textBlock.Foreground = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.Foreground]));
            textBlock.Text = process.PresentationText();
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.MouseDown += new MouseButtonEventHandler(Circle_MouseDown);
            textBlock.MouseMove += new System.Windows.Input.MouseEventHandler(Circle_MouseMove);
            textBlock.MouseUp += new MouseButtonEventHandler(Circle_MouseUp);
            textBlock.MouseEnter += new MouseEventHandler(Circle_MouseEnter);
            textBlock.MouseLeave += new MouseEventHandler(Circle_MouseLeave);
            return textBlock;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ///\fn  private Color CreateColorFromEnum(System.Drawing.KnownColor knownColor)
        ///
        ///\brief   Creates color from enum.
        ///
        ///\author  Ilan Hindy
        ///\date    29/09/2016
        ///
        ///\param   knownColor  The known color.
        ///
        ///\return  The new color from enum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////



        /*
         * SetBackgroundColorsOfAllProcesses
         * This method changes the text color and the background 
         * It does that by calling to SetBackgroundColors of each process
         * The method has to support also the case when the network is building and in this
         * case there are the following limitations
         * 1. not all the processes has presentation
         * 2. the current process presentation is not yet attached to the process
         */

        /**********************************************************************************************//**
         * Sets blinking for all processes.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void SetBlinkingForAllProcesses()
        {
            BaseProcess process = (BaseProcess)presentationElements.Keys.ToList()[0];
            foreach (BaseProcess p in mainWindow.net.Processes)
            {
                //Set the colors to all the processes that has presentation
                if (p.Presentation != null && p != process)
                {
                    ((ProcessPresentation)p.Presentation).SetBlinking();
                }
            }
            // Set the colors to this process
            SetBlinking();
        }

        /**********************************************************************************************//**
         * Sets the blinking.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public void SetBlinking()
        {
            BaseProcess process = (BaseProcess)presentationElements.Keys.ToList()[0];
            Ellipse theProcessCircle = (Ellipse)presentationElements[process].controls[PresentationElement.ControlKeys.Circle];
            Storyboard theProcessStoryboard = presentationElements[process].blinkingStoryBoard;
            //Collect all the circles
            foreach (BaseProcess p in mainWindow.net.Processes)
            {
                if (p != process)
                {
                    if (p.Presentation != null)
                    {
                        Ellipse otherProcessCircle = (Ellipse)p.Presentation.presentationElements[p].controls[PresentationElement.ControlKeys.Circle];
                        if (theProcessCircle.Margin.Top == otherProcessCircle.Margin.Top &&
                            theProcessCircle.Margin.Left == otherProcessCircle.Margin.Left &&
                            theProcessCircle.Margin.Bottom == otherProcessCircle.Margin.Bottom &&
                            theProcessCircle.Margin.Right == otherProcessCircle.Margin.Right)
                        {
                            //set the circle blinking
                            theProcessStoryboard.Begin();
                            return;
                        }
                    }
                }
            }
            //There is no another circle in the same place set the regular colors
            theProcessStoryboard.Stop();
        }
      
        /*
         * Set the textBlock dimentions
         */

        /**********************************************************************************************//**
         * Sets text block dimentions.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   presentationElement The presentation element.
         *                              
         **************************************************************************************************/

        private void SetTextBlockDimentions(PresentationElement presentationElement)
        {
            Ellipse circle = (Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle];
            TextBlock textBlock = (TextBlock)presentationElement.controls[PresentationElement.ControlKeys.TextBlock];
            Double a = circle.Width / 2;
            Double b = circle.Height / 2;
            Double PI = 3.1415926;
            textBlock.Width = 2 * a * Math.Cos(45 * PI / 180) - 4;
            textBlock.Height = 2 * b * Math.Sin(45 * PI / 180) - 4;
        }

        /*
         * Move the textBlock after the moving of the circle
         */

        /**********************************************************************************************//**
         * Move text block in circle.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   presentationElement The presentation element.
         *                              
         **************************************************************************************************/

        private void MoveTextBlockInCircle(PresentationElement presentationElement)
        {
            Ellipse circle = (Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle];
            TextBlock textBlock = (TextBlock)presentationElement.controls[PresentationElement.ControlKeys.TextBlock];
            double top = circle.Margin.Top;
            double left = circle.Margin.Left;
            double textBlockTop = top + circle.Height / 2 - textBlock.Height / 2 + 2;
            double textBlockLeft = left + circle.Width / 2 - textBlock.Width / 2 + 2;
            textBlock.Margin = new Thickness(textBlockLeft, textBlockTop, 0, 0);
        }

        /*
         * Events 
         * The events serve 3 processs :
         * 1. ciircle movement
         * 2. circle resize
         * 3. selecting processes in add channel process
         * 4. Selectng a process
         * The following are the algorithms of the processes
         * 1. Circle move
         *    Mouse doun - If the mouse button is the left button set a flag that we are in moving process
         *    Mouse move - If the flag is set move the circle to thenew location and update the textBlock and
         *                 channels connected to the process
         *    Mouse Up - Update the network element with the new location  and set the process as selectted
         * 2. Circle resize
         *    Mouse down - if the button is the right button set a flag,get the distance of 
         *                 the mouse from the center of the circle
         *   Mouse move - each time :
         *      calculate the distance of the mouse from the center
         *      Add the difference of the distance from the distance calculated in the previouse step to the
         *          width and the height of the circle
         *      Redrow the circle, the textBlock and the channel connectted to the process
         *   Mouse Up - update the network elemet data     
         *3. Sellecting processes for adding a channel
         *   Mouse down - if in add chnnel process do nothing
         *   Mouse down - call main window add channel method with the process as a parameter
         *4. Selecting a process - Is part of the moving circle process
         */

        /*
         * Event - mouuse down - starts the following processes:
         * If the mouse button is the left button - start a moving of the circle process
         * If the mouse button is the right button - start circle resize process
         */

        /**********************************************************************************************//**
         * Event handler. Called by Circle for mouse down events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse button event information.
         
         **************************************************************************************************/

        public void Circle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var presentationElementEntry = presentationElements.First(pe => pe.Value.controls.Values.Any(control => control == sender));
            if (e.ChangedButton == MouseButton.Left)
            {
                if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        mainWindow.runningHandler.RunSingleStep((BaseProcess)presentationElementEntry.Key);
                    }
                }
                else
                {
                    if (mainWindow.channelAddState == MainWindow.ChannelAddState.NotInProcess)
                    {
                        isInPlacingProcess = true;
                    }
                }
            }
            else // right mouse button
            {
                if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
                {
                    if (presentationElementEntry.Value.debugWindow == null)
                    {
                        UpdateDebugDataDockPannel(e.GetPosition(canvas), (BaseProcess)presentationElementEntry.Key, presentationElementEntry.Value, false);
                    }
                }
                else
                {
                    inResizeProcess = true;
                    Ellipse circle = (Ellipse)presentationElementEntry.Value.controls[PresentationElement.ControlKeys.Circle];
                    distanceFromCenter = DistanceFromCircleCenter(e.GetPosition(circle), circle);
                }
            }
        }

        /*
         * Mouse move event
         */

        /**********************************************************************************************//**
         * Event handler. Called by Circle for mouse move events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse event information.
         
         **************************************************************************************************/

        public void Circle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var presentationElementEntry = presentationElements.First(pe => pe.Value.controls.Values.Any(control => control == sender));
            if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
            {
                return;
            }

            Ellipse circle = (Ellipse)presentationElementEntry.Value.controls[PresentationElement.ControlKeys.Circle];
            if (isInPlacingProcess== true && e.LeftButton == MouseButtonState.Pressed)
            {
                double left = e.GetPosition(canvas).X - circle.Width / 2;
                double top = e.GetPosition(canvas).Y - circle.Height /2;
                circle.Margin = new Thickness(left, top, 0, 0);
                MoveTextBlockInCircle(presentationElementEntry.Value);
                MoveChannels((BaseProcess)presentationElementEntry.Key);
                SetBlinkingForAllProcesses();
            }
            if (inResizeProcess && e.RightButton == MouseButtonState.Pressed)
            {
                double distanceOfNewPointFromCircleCenter = DistanceFromCircleCenter(e.GetPosition(circle), circle);
                double diffDistance = (distanceFromCenter - distanceOfNewPointFromCircleCenter) * 2;

                //If the movement of the cursor was too fast - stop the resize process
                if (circle.Width + diffDistance < 0)
                {
                    inResizeProcess = false;
                    UpdateNetworkElement(presentationElementEntry.Key);
                    mainWindow.UpdatePresentation();
                    mainWindow.SetSelectedProcess((BaseProcess)presentationElementEntry.Key);
                    return;
                }
                else
                {
                    circle.Width = circle.Height = circle.Width + diffDistance;
                }
                //PrintCircleData();
                SetTextBlockDimentions(presentationElementEntry.Value);
                MoveTextBlockInCircle(presentationElementEntry.Value);
                MoveChannels((BaseProcess)presentationElementEntry.Key);
                distanceFromCenter = DistanceFromCircleCenter(e.GetPosition(circle), circle);
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Circle for mouse up events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse button event information.
         
         **************************************************************************************************/

        public void Circle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var presentationElementEntry = presentationElements.First(pe => pe.Value.controls.Values.Any(control => control == sender));
            if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
            {
                return;
            }
            if (mainWindow.channelAddState != MainWindow.ChannelAddState.NotInProcess)
            {
                mainWindow.Command_AddChannel(presentationElementEntry.Key, null);
            }
            else
            {
                if (inResizeProcess)
                {
                    inResizeProcess = false;
                    UpdateNetworkElement(presentationElementEntry.Key);
                    mainWindow.UpdatePresentation();
                    mainWindow.SetSelectedProcess((BaseProcess)presentationElementEntry.Key);
                }
                else
                {
                    isInPlacingProcess = false;
                    UpdateNetworkElement(presentationElementEntry.Key);
                    mainWindow.UpdatePresentation();
                    mainWindow.SetSelectedProcess((BaseProcess)presentationElementEntry.Key);
                }
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Circle for mouse enter events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse event information.
         
         **************************************************************************************************/

 

        /**********************************************************************************************//**
         * Updates the debug data dock pannel.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   mouseLocation       The mouse location.
         * \param   process             The process.
         * \param   presentationElement The presentation element.
         * \param   remove              true to remove.
         *                              
         **************************************************************************************************/

        private void UpdateDebugDataDockPannel(Point mouseLocation, BaseProcess process, PresentationElement presentationElement, bool remove)
        {
            Ellipse circle = (Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle];
            double circleCenretX = circle.Margin.Left + circle.Width/2;
            double circleCenterY = circle.Margin.Top + circle.Height / 2;
            presentationElement.debugWindow = new ElementDebugWindow(new List<NetworkElement>() {process});
            presentationElement.debugWindow.Show();
        }


        /*
         * Calculate the distance of the mouse from circle center.
         */

        /**********************************************************************************************//**
         * Distance from circle center.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   point   The point.
         * \param   circle  The circle.
         *
         * \return  A double.
         .
         **************************************************************************************************/

        private double DistanceFromCircleCenter(Point point, Ellipse circle)
        {
            Point circleCenter = new Point(circle.Margin.Left + circle.Width / 2, circle.Margin.Top + circle.Height / 2);
            return Math.Sqrt(Math.Pow(circleCenter.X - point.X, 2) + Math.Pow(circleCenter.Y - point.Y, 2));
        }

        /*
         * Send the circle data to the debug window
         */

        /**********************************************************************************************//**
         * Print circle data.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void PrintCircleData()
        {
            Ellipse circle = (Ellipse)presentationElements.ElementAt(0).Value.controls[PresentationElement.ControlKeys.Circle];
            //mainWindow.PrintDubugMessage("Top:" + circle.Margin.Top.ToString() + " Left:" + circle.Margin.Left.ToString() + " width:" + circle.Width + " height:" + circle.Height);
        }

        /*
         * Move all the channels connected to the process 
         */

        /**********************************************************************************************//**
         * Move channels.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process The process.
         
         **************************************************************************************************/

        private void MoveChannels(BaseProcess process)
        {
            int processId = process.ea[ne.eak.Id];
            List<BaseChannel> channelsToUpdate = mainWindow.net.CollectChannelsConnectedToProcess(processId);
            foreach (BaseChannel channel in channelsToUpdate)
            {
                if (channel.Presentation != null)
                {
                    ((ChannelPresentation)channel.Presentation).PositionChannel();
                }
            }
        }


        /*
         * Update the network element data after move or resize processes
         */

        /**********************************************************************************************//**
         * Updates the network element described by networkElement.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         
         **************************************************************************************************/

        protected override void UpdateNetworkElement(NetworkElement networkElement)
        {
            base.UpdateNetworkElement(networkElement);
            networkElement.pp[bp.ppk.FrameHeight] = ((Ellipse)presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle]).Height;
            networkElement.pp[bp.ppk.FrameWidth] = ((Ellipse)presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle]).Width;
            networkElement.pp[bp.ppk.FrameLeft] = ((Ellipse)presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle]).Margin.Left;
            networkElement.pp[bp.ppk.FrameTop] = ((Ellipse)presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle]).Margin.Top;
        }

        /*
         * Update the presentation after network element data update
         */

        /**********************************************************************************************//**
         * Updates the presentation.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public override void UpdatePresentation()
        {
            foreach (BaseProcess process in presentationElements.Keys)
            {
                UpdatePresentationShapeParameters(process);
            }
        }

        /**********************************************************************************************//**
         * Updates the presentation shape parameters described by process.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   process The process.
         **************************************************************************************************/

        private void UpdatePresentationShapeParameters(BaseProcess process, bool moveChannels = true)
        {
            PresentationElement presentationElement = presentationElements[process];
            ((Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle]).Width = process.pp[bp.ppk.FrameWidth];
            ((Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle]).Height = process.pp[bp.ppk.FrameHeight];
            ((Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle]).Margin = new Thickness(process.pp[bp.ppk.FrameLeft], process.pp[bp.ppk.FrameTop], 0, 0);
            ((Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle]).Fill = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.Background]));
            ((Ellipse)presentationElement.controls[PresentationElement.ControlKeys.Circle]).StrokeThickness = process.pp[bp.ppk.FrameLineWidth];
            ((TextBlock)presentationElement.controls[PresentationElement.ControlKeys.TextBlock]).Text = process.pp[bp.ppk.Text];
            ((TextBlock)presentationElement.controls[PresentationElement.ControlKeys.TextBlock]).Background = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.Background]));
            ((TextBlock)presentationElement.controls[PresentationElement.ControlKeys.TextBlock]).Foreground = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.Foreground]));
            SetTextBlockDimentions(presentationElement);
            MoveTextBlockInCircle(presentationElement);
            if (moveChannels)
            {
                MoveChannels(process);
            }
            SetBlinkingForAllProcesses();
        }

        /*
         * Set the sircle status and colors according to if it was selectted or not
         */

        /**********************************************************************************************//**
         * Sets a selected.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         * \param   selectedStatus  The selected status.
         
         **************************************************************************************************/

        public override void SetSelected(NetworkElement networkElement, MainWindow.SelectedStatus selectedStatus)
        {
            presentationElements[networkElement].status = selectedStatus;
            ((Ellipse)presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle]).Stroke = selectedColors[(int)selectedStatus];
        }

        /**********************************************************************************************//**
         * Updates the running status.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         * \param   parameters      Options for controlling the operation.
         
         **************************************************************************************************/

        public override void UpdateRunningStatus(NetworkElement networkElement, object[] parameters)
        {
            // Update the shape parameters
            UpdatePresentationShapeParameters((BaseProcess)networkElement, false);

            /*
             * The parameters are:
             * [0] Whether the process is the process to be activated
             * [1] list of main breakpoints of the participated in last breakpoint evaluation
             */
            UpdateBreakpointLabel((BaseProcess)networkElement, (List<Breakpoint>)parameters[1]);
            base.UpdateRunningStatus(networkElement, parameters);
            CreateFloatingSummary();
            ((Ellipse)presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle]).Stroke = selectedColors[(int)(parameters[0])];

            // There can be only one from DebugWindow and Floating summary
            if (presentationElements[networkElement].debugWindow != null)
            {
                presentationElements[networkElement].debugWindow.UpdateExistingValues();
            }
            //if (presentationElements[networkElement].breakpointWindow != null)
            //{
            //    presentationElements[networkElement].breakpointWindow.UpdateExistingValues();
            //}
        }

        #region /// \name Floating summary

        public void Circle_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowFloatingSummary();
        }

        public void Circle_MouseLeave(object sender, MouseEventArgs e)
        {
            HideFloatingSummaryIfNeeded(e);
        }

        protected override FrameworkElement ElementObject()
        {
            foreach (PresentationElement presentationElement in presentationElements.Values)
            {
                return (FrameworkElement)presentationElement.controls[PresentationElement.ControlKeys.Circle];
            }
            return null;
        }
        #endregion


        /**********************************************************************************************//**
         * Updates the end running described by process.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process The process.
         
         **************************************************************************************************/

        public override void UpdateEndRunning(NetworkElement process)
        {
            base.UpdateEndRunning(process);
            UpdateBreakpointLabel((BaseProcess)process, new List<Breakpoint>());
        }

        /**********************************************************************************************//**
         * Updates the breakpoint label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process     The process.
         * \param   breakpoints The breakpoints.
         *                      
         **************************************************************************************************/

        public void UpdateBreakpointLabel(BaseProcess process, List<Breakpoint> breakpoints)
        {
            if (!mainWindow.showBreakpoints) return;

            breakpointLabelString = "";            
            foreach (Breakpoint breakpoint in breakpoints)
            {
                breakpointsLabelStringIndent = "";
                AddBreakpointToLabel(breakpoint);
                breakpointsLabelStringIndent = "\t";
                breakpoint.ScanAndReport(this, NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None);
            }
            Label label = (Label)presentationElements[process].controls[PresentationElement.ControlKeys.BreakpointLabel];
            if (breakpointLabelString.Length > 0)
            {
                label.Content = breakpointLabelString.Substring(0, breakpointLabelString.Length - 1);
                label.Visibility = Visibility.Visible;
            }
            else
            {
                label.Content = "";
                label.Visibility = Visibility.Hidden;
            }
            SetLabelDimentions(label);
            PositionBreakpointLabel(process, label);

            label.Background = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.BreakpointsBackground]));
            label.Foreground = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.BreakpointsForeground]));
            label.BorderThickness = new Thickness(process.pp[bp.ppk.BreakpointsFrameWidth]);
            label.BorderBrush = new SolidColorBrush(CreateColorFromEnum(process.pp[bp.ppk.BreakpointsFrameColor]));
        }

        public void SetBreakpoinsVisibility(BaseProcess process, Visibility visibility)
        {
            Label label = (Label)presentationElements[process].controls[PresentationElement.ControlKeys.BreakpointLabel];
            label.Visibility = visibility;
        }

        /**********************************************************************************************//**
         * Position breakpoint label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process The process.
         * \param   label   The label.
         
         **************************************************************************************************/

        private void PositionBreakpointLabel(BaseProcess process, Label label)
        {
            double width = process.pp[bp.ppk.FrameWidth];
            double top = process.pp[bp.ppk.FrameTop] - label.Height;
            double left = process.pp[bp.ppk.FrameLeft] + (width -label.Width) / 2;
            label.Margin = new Thickness(left, top, left + label.Width, top + label.Height);
        }

        /**********************************************************************************************//**
         * Adds a breakpoint to label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   breakpoint  The breakpoint.
         *                      
         **************************************************************************************************/

        private void AddBreakpointToLabel(Breakpoint breakpoint)
        {
            List<String> resultsStrings = ((AttributeList)breakpoint.or[brp.ork.LastRunningResult])
                .Select( a => (string)a.Value).ToList();
            BaseProcess process = (BaseProcess)presentationElements.Keys.ElementAt(0);
            int processId = process.ea[ne.eak.Id];
            if (resultsStrings[processId] == "True")
            {
                breakpointLabelString += breakpointsLabelStringIndent + breakpoint.ToString();
                breakpointLabelString += "\n";                
            }            
        }

        /**********************************************************************************************//**
         * Opens complex attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   attribute   The attribute.
         * \param   key         The key.
         * \param   name        The name.
         * \param   dictionary  The dictionary.
         *                      
         **************************************************************************************************/

        public void OpenComplexAttribute(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            if (((Attribute)attribute).Value is Breakpoint)
            {
                string s = ((Breakpoint)((Attribute)attribute).Value).ea[brp.ork.Name];
                AddBreakpointToLabel((Breakpoint)((Attribute)attribute).Value);
                breakpointsLabelStringIndent += "\t";
            }
        }

        /**********************************************************************************************//**
         * Closes complex attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   attribute   The attribute.
         * \param   key         The key.
         * \param   name        The name.
         * \param   dictionary  The dictionary.
         *                      
         **************************************************************************************************/

        public void CloseComplexAttribute(dynamic key, 
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            if (((Attribute)attribute).Value is Breakpoint)
            {
                string s = ((Breakpoint)((Attribute)attribute).Value).or[brp.ork.Name];
                breakpointsLabelStringIndent =  breakpointsLabelStringIndent.Substring(0, breakpointsLabelStringIndent.Length - 1);
            }
        }

        /**********************************************************************************************//**
         * Attribute report.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   attribute   The attribute.
         * \param   key         The key.
         * \param   name        The name.
         * \param   dictionary  The dictionary.
         *                      
         **************************************************************************************************/

        public void AttributeReport(dynamic key, 
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
        }

        /**********************************************************************************************//**
         * Scans a condition.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   key             The key.
         * \param   attribute       The attribute.
         * \param   dictionaryKey   The dictionary key.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        public bool ScanCondition(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            if (attribute.Value is Breakpoint || 
                TypesUtility.GetKeyToString(key) == TypesUtility.GetKeyToString(brp.ork.Parameters))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetShowBreakpoints(bool showBreakpoints)
        {
            Visibility newVisibility;
            if (showBreakpoints)
            {
                newVisibility = Visibility.Visible;
            }
            else
            {
                newVisibility = Visibility.Collapsed;
            }
            foreach (var entry in presentationElements)
            {
                presentationElements[entry.Key].breakpointsVisibility = showBreakpoints;
                presentationElements[entry.Key].controls[PresentationElement.ControlKeys.BreakpointLabel].Visibility = newVisibility;
            }
        }
    }
}
