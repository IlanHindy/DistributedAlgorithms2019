////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Presentation\ChannelPresentation.cs
///
///\brief   Implements the channel presentation class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Windows.Input;
using System.Diagnostics;

namespace DistributedAlgorithms
{

    /* 
     * Class that present the channels
     * The class can present one channel or 2 channel one oposite to the other
     * The class holds the following controls:
     * 1. A poligon which is the arrow head for each channel
     * 2. The connecting line between the processes
     * 3. A Label that honlds the string that present the channels
     */

    /**********************************************************************************************//**
     * A channel presentation.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class ChannelPresentation : Presentation
    {
        /** Size of the arrow head. */
        private const int arrowHeadSize = 10;
        /** The running update lock. */
        private object runningUpdateLock = new object();
        /** The label relative position. */
        private double[] labelRelativePosition = { 0.9, 0.1 };

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   mainWindow      The main window.
         * \param   canvas          The canvas.
         * \param   forwaredChannel The forwared channel.
         * \param   backwardChannel The backward channel.
         
         **************************************************************************************************/

        public ChannelPresentation(MainWindow mainWindow,
            Canvas canvas,
            BaseChannel forwaredChannel,
            BaseChannel backwardChannel)
            : base(mainWindow, canvas)
        {
            additionalControls.Add(AdditionalControlKeys.Line, CreateConnectionLine());
            Border labelsBorder;
            additionalControls.Add(AdditionalControlKeys.LabelsPanel, CreatePresentationLabelsPanel(out labelsBorder));
            additionalControls.Add(AdditionalControlKeys.LabelsBorder, labelsBorder);
            AddChannel(forwaredChannel, false);
            if (backwardChannel != null)
            {
                AddChannel(backwardChannel, false);
            }
            PositionChannel();
        }

        /* 
         * The ToString is used in the main window channels list
         */

        /**********************************************************************************************//**
         * Convert this object into a string representation.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         *  Returns a string that represents the current object.
         *
         * \return  A string that represents the current object.
         .
         **************************************************************************************************/

        public override string ToString()
        {
            string result = "Present:";
            foreach (NetworkElement networkElement in presentationElements.Keys)
            {
                    result += " sp:" + networkElement.ea[bc.eak.SourceProcess];
                    result += " dp:" + networkElement.ea[bc.eak.DestProcess];
            }
            return result;
        }

        /*
         * Add a channel to a presentation that contains one channel
         */

        /**********************************************************************************************//**
         * Adds a channel to 'positionChannel'.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   channel         The channel.
         * \param   positionChannel (Optional) True to position channel.
         
         **************************************************************************************************/

        public void AddChannel(BaseChannel channel, bool positionChannel = true)
        {
            PresentationElement presentationElement = new PresentationElement();
            presentationElement.status = MainWindow.SelectedStatus.NotSelected;            
            presentationElement.controls.Add(PresentationElement.ControlKeys.ArrowHead, CreatePolygon());
            //presentationElement.controls.Add(PresentationElement.ControlKeys.MessageQ, CreateInvisibleLabel());
            presentationElement.controls.Add(PresentationElement.ControlKeys.MessagesGridView, CreateMessagesGridView(channel.ea[bc.eak.SourceProcess]));
            presentationElement.controls.Add(PresentationElement.ControlKeys.Label, CreatePresentationLabel(channel));
            ((StackPanel)additionalControls[AdditionalControlKeys.LabelsPanel]).Children.Add(presentationElement.controls[PresentationElement.ControlKeys.Label]);
            presentationElements.Add(channel, presentationElement);
            if (positionChannel)
            {
                PositionChannel();
            }
        }

        /*
         * Create a connection line control
         */

        /**********************************************************************************************//**
         * Creates connection line.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  The new connection line.
         .
         **************************************************************************************************/

        private Line CreateConnectionLine()
        {
            Line connectionLine = new Line();
            canvas.Children.Add(connectionLine);
            connectionLine.Visibility = Visibility.Visible;
            connectionLine.StrokeThickness = 1;
            connectionLine.MouseDown += ConnectionLine_MouseDown;
            return connectionLine;
        }



        /*
         * Create a poligon control
         */

        /**********************************************************************************************//**
         * Creates the polygon.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  The new polygon.
         .
         **************************************************************************************************/

        private Polygon CreatePolygon()
        {
            Polygon polygon = new Polygon();
            canvas.Children.Add(polygon);
            polygon.Visibility = Visibility.Visible;
            polygon.StrokeThickness = 1;
            polygon.MouseDown += ArrowHead_MouseDown;
            return polygon;
        }

        /*
         * Create a label control
         */

        /**********************************************************************************************//**
         * Creates presentation label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   channel The channel.
         *
         * \return  The new presentation label.
         .
         **************************************************************************************************/

        private Label CreatePresentationLabel(BaseChannel channel)
        {
            Label label = new Label();
            UpdateLabel(channel, label);
            label.MouseEnter += new MouseEventHandler(labelsPanel_MouseEnter);
            label.MouseLeave += new MouseEventHandler(labelsPanel_MouseLeave);
            return label;
        }

        private GridView CreateMessagesGridView(int sourceProcessId)
        {
            GridView gridView = new GridView(canvas, new List<string>(), sourceProcessId, ChangeMessageOrder.PermutationString);
            canvas.Children.Add(gridView);
            return gridView;
        }

        /**********************************************************************************************//**
         * Updates the label.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   channel The channel.
         * \param   label   The label.
         **************************************************************************************************/

        private void UpdateLabel(BaseChannel channel, Label label)
        {
            label.Background = new SolidColorBrush(CreateColorFromEnum(channel.pp[bc.ppk.Background]));
            label.Foreground = new SolidColorBrush(CreateColorFromEnum(channel.pp[bc.ppk.Foreground]));
            label.BorderThickness = new Thickness(channel.pp[bc.ppk.FrameWidth]);
            label.BorderBrush = new SolidColorBrush(CreateColorFromEnum(channel.pp[bc.ppk.FrameColor]));
            label.Content = channel.pp[bc.ppk.PresentationTxt].ToString();
            SetLabelDimentions(label);
            label.Margin = new Thickness(1);
        }

        /**********************************************************************************************//**
         * Creates presentation labels panel.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param [out] border  The border.
         *
         * \return  The new presentation labels panel.
         **************************************************************************************************/

        private StackPanel CreatePresentationLabelsPanel(out Border border)
        {
            StackPanel labelsPanel = new StackPanel();
            labelsPanel.Orientation = Orientation.Horizontal;
            border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.Child = labelsPanel;
            border.MouseEnter += new MouseEventHandler(labelsPanel_MouseEnter);
            border.MouseLeave += new MouseEventHandler(labelsPanel_MouseLeave);
            canvas.Children.Add(border);
            return labelsPanel;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        /*
         * Set the text of the label :
         * 1. retrieve the text from the channels 
         * 2. If the text retrieved is not empty set the dimention of the Label according 
         *    to the size of the text
         */

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ///\fn  private void SetLabelText(Label label)
        ///
        ///\brief   Sets label text.
        ///
        ///\author  Ilan Hindy
        ///\date    29/09/2016
        ///
        ///\param   label   The label.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void SetLabelText(Label label)
        //{
        //    bool onlyEmptyTextRetrieved = true;
        //    string text = "";
        //    foreach (NetworkElement networkElement in presentationElements.Keys)
        //    {
        //        if (networkElement.PresentationText() != "")
        //        {
        //            onlyEmptyTextRetrieved = false;
        //            text += networkElement.PresentationText() + ";";
        //        }
        //    }
        //    if (!onlyEmptyTextRetrieved)
        //    {
        //        label.Content = text.Remove(text.Length - 1); ;
        //        SetLabelDimentions(label);
        //    }
        //}



        /*
         * Update a label text and location
         */

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ///\fn  public void UpdateLabel()
        ///
        ///\brief   Updates the label.
        ///
        ///\author  Ilan Hindy
        ///\date    29/09/2016
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //public void UpdateLabel()
        //{
        //    if (additionalControls.ContainsKey(AdditionalControlKeys.Label))
        //    {
        //        Label label = (Label)additionalControls[AdditionalControlKeys.Label];
        //        SetLabelText(label);
        //        PositionLabel(label, 0.5);
        //    }
        //}

        /*
         * Position the presentation controls
         * The line starts and end from the processes circle border one towared the other
         * The arrow heads are positioned at the end of the line with direction toward the processes circles
         * The label is placed in the middle of the line
         */

        /**********************************************************************************************//**
         * Position channel.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public void PositionChannel()
        {
            // Get the processes circles controls
            Ellipse sourceProcessPresentationCircle = GetProcessPresentationCircle(bc.eak.SourceProcess);
            Ellipse destProcessPresentationCircle = GetProcessPresentationCircle(bc.eak.DestProcess);

            //Calculate the center of the circles
            double sourceX0 = sourceProcessPresentationCircle.Margin.Left + sourceProcessPresentationCircle.Width / 2;
            double sourceY0 = sourceProcessPresentationCircle.Margin.Top + sourceProcessPresentationCircle.Height / 2;
            double destX0 = destProcessPresentationCircle.Margin.Left + destProcessPresentationCircle.Width / 2;
            double destY0 = destProcessPresentationCircle.Margin.Top + destProcessPresentationCircle.Height / 2;

            //Calculate the direction of the line from the center this is used in order to select from
            //the 2 possible pointes on the circle border that are connected to the line
            double sourceXDirection = (sourceX0 < destX0) ? 1 : -1;
            double destXDirection = (sourceX0 > destX0) ? 1 : -1;
            double sourceYDirection = (sourceY0 < destY0) ? 1 : -1;
            double destYDirection = (sourceY0 > destY0) ? 1 : -1;
            double lineSourceX;
            double lineSourceY;
            double lineDestX;
            double lineDestY;

            //Calculate the circle radioses
            double sourceRadiouse = sourceProcessPresentationCircle.Width / 2;
            double destRadiouse = destProcessPresentationCircle.Width / 2;


            //If the source and destination X coordinate are the same
            //(The source and the destination circles are one above the other and 
            // the line incline will be infinit
            if (sourceX0 == destX0)
            {
                lineSourceX = sourceX0;
                lineDestX = destX0;
                lineSourceY = sourceY0 + sourceYDirection * sourceRadiouse;
                lineDestY = destY0 + destYDirection * destRadiouse;
            }
            else
            {
                double lineIncline = (destY0 - sourceY0) / (destX0 - sourceX0);
                lineSourceX = sourceXDirection * (Math.Sqrt(Math.Pow(sourceRadiouse, 2) / (Math.Pow(lineIncline, 2) + 1))) + sourceX0;
                lineDestX = destXDirection * (Math.Sqrt(Math.Pow(destRadiouse, 2) / (Math.Pow(lineIncline, 2) + 1))) + destX0;
                lineSourceY = lineIncline * (lineSourceX - sourceX0) + sourceY0;
                lineDestY = lineIncline * (lineDestX - destX0) + destY0;
            }

            //Position the poligons (The arrow head)
            ((Line)additionalControls[AdditionalControlKeys.Line]).X1 = lineSourceX;
            ((Line)additionalControls[AdditionalControlKeys.Line]).Y1 = lineSourceY;
            ((Line)additionalControls[AdditionalControlKeys.Line]).X2 = lineDestX;
            ((Line)additionalControls[AdditionalControlKeys.Line]).Y2 = lineDestY;

            //If the line start or end are inside the circles - set the status of the presentation
            //To do not show else the status is NotSelected - call to a function that will select
            //And set the colors of the controls
            if (InOtherCircle(lineSourceX, lineSourceY, destX0, destY0, destRadiouse) ||
                InOtherCircle(lineDestX, lineDestY, sourceX0, sourceY0, sourceRadiouse))
            {
                SetChannelsStatuses(MainWindow.SelectedStatus.DoNotShow);
            }
            else
            {
                SetChannelsStatuses(MainWindow.SelectedStatus.NotSelected);
            }

            //Position the poligons (The arrow head)
            double[] lineX = { lineDestX, lineSourceX };
            double[] lineY = { lineDestY, lineSourceY };
            double angle = GetAngle(lineSourceX, lineSourceY, lineDestX, lineDestY);
            double[] angels = { 180 + angle, angle };
            for (int idx = 0; idx < presentationElements.Count; idx++)
            {
                PositionPolygon((Polygon)presentationElements.Values.ElementAt(idx).controls[PresentationElement.ControlKeys.ArrowHead], lineX[idx], lineY[idx], angels[idx]);
                PositionMessagesGrid((GridView)presentationElements.Values.ElementAt(idx).controls[PresentationElement.ControlKeys.MessagesGridView], labelRelativePosition[idx]);
                //PositionLabel((Label)presentationElements.Values.ElementAt(idx).controls[PresentationElement.ControlKeys.MessageQ], labelRelativePosition[idx]);
            }

            //Position the label
            if (additionalControls.ContainsKey(AdditionalControlKeys.LabelsPanel))
            {
                PositionBorder((Border)additionalControls[AdditionalControlKeys.LabelsBorder], 0.5);
            }
        }

        /*
         * Set network element status 
         * The method determin the satus of the channel
         * If the circles of the processes are one on top of the other
         * The status of all the controls will be DoNotShow
         * If the circles are not one on top of the other the status is determined according
         * The whether the channel is selected or not in the MainWindow
         * After the status was determined the SetSelected method is called to
         * apply the status on the controls
         */

         /**********************************************************************************************//**
          * Sets channels statuses.
          *
          * \author Ilan Hindy
          * \date   29/09/2016
          *
          * \param  desieredStatus  The desiered status.
          *                         
          **************************************************************************************************/

         private void SetChannelsStatuses(MainWindow.SelectedStatus desieredStatus)
        {
             //It is not possible to modify a collection while in foreeach therefor the
             //Dictionary is copied to a temp one

            foreach(var presentationElementEntry in presentationElements)
            {
                if (desieredStatus == MainWindow.SelectedStatus.DoNotShow)
                {
                    presentationElementEntry.Value.status = MainWindow.SelectedStatus.DoNotShow;
                }
                else
                {
                    if (presentationElementEntry.Key == mainWindow.SelectedChannel)
                    {
                        presentationElementEntry.Value.status = MainWindow.SelectedStatus.Selected;
                    }
                    else
                    {
                        presentationElementEntry.Value.status = MainWindow.SelectedStatus.NotSelected;
                    }
                }
                SetSelected(presentationElementEntry.Key, presentationElementEntry.Value.status);
            }
         }

        /*
         * Get the circle control of a process
         */

        /**********************************************************************************************//**
         * Gets process presentation circle.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   processKey  The process key.
         *
         * \return  The process presentation circle.
         .
         **************************************************************************************************/

        private Ellipse GetProcessPresentationCircle(bc.eak processKey)
        {
            int processId = presentationElements.Keys.ElementAt(0).ea[processKey];
            NetworkElement networkElement = mainWindow.net.Processes.First(p => p.ea[ne.eak.Id] == processId);
            return (Ellipse)networkElement.Presentation.presentationElements[networkElement].controls[PresentationElement.ControlKeys.Circle];
        }

        /*
         * Check if a point is inside a circle
         */

        /**********************************************************************************************//**
         * In other circle.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   x           The x coordinate.
         * \param   y           The y coordinate.
         * \param   x0          The x coordinate 0.
         * \param   y0          The y coordinate 0.
         * \param   radiouse    The radiouse.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool InOtherCircle(double x, double y, double x0, double y0, double radiouse)
        {
            return Math.Pow((x - x0), 2) + Math.Pow(y - y0, 2) <= Math.Pow(radiouse, 2);
        }

        /*
         * Get the angle of a line
         */

        /**********************************************************************************************//**
         * Gets an angle.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   x1  The first x value.
         * \param   y1  The first y value.
         * \param   x2  The second x value.
         * \param   y2  The second y value.
         *
         * \return  The angle.
         .
         **************************************************************************************************/

        private double GetAngle(double x1, double y1, double x2, double y2)
        {
            double degrees;

            // Avoid divide by zero run values.
            if (x2 - x1 == 0)
            {
                if (y2 > y1)
                    degrees = 90;
                else
                    degrees = 270;
            }
            else
            {
                // Calculate angle from offset.
                double riseoverrun = (double)(y2 - y1) / (double)(x2 - x1);
                double radians = Math.Atan(riseoverrun);
                degrees = radians * ((double)180 / Math.PI);

                // Handle quadrant specific transformations.       
                if ((x2 - x1) < 0 || (y2 - y1) < 0)
                    degrees += 180;
                if ((x2 - x1) > 0 && (y2 - y1) < 0)
                    degrees -= 180;
                if (degrees < 0)
                    degrees += 360;
            }
            return degrees;
        }

        /*
         * Position and rotate a poligone according to point and angle
         */

        /**********************************************************************************************//**
         * Position polygon.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   polygon The polygon.
         * \param   X1      The first x value.
         * \param   Y1      The first y value.
         * \param   angle   The angle.
         
         **************************************************************************************************/

        private void PositionPolygon(Polygon polygon, double X1, double Y1, double angle)
        {
            polygon.Points.Clear();
            polygon.Points.Add(new Point(X1, Y1));
            polygon.Points.Add(new Point(X1 + arrowHeadSize, Y1));
            polygon.Points.Add(new Point(X1, Y1 + arrowHeadSize));
            RotateTransform rotateTransform = new RotateTransform(angle - 45, X1, Y1);
            polygon.RenderTransform = rotateTransform;
        }

        /*
         * Position of the label in the middle of the line
         */

        /**********************************************************************************************//**
         * Position label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   label           The label.
         * \param   linePosition    The line position.
         
         **************************************************************************************************/

        private void PositionLabel(Label label, double linePosition)
        {
            double width = label.Width; 
            double height = label.Height;
            Line connectingLine = (Line)additionalControls[AdditionalControlKeys.Line];
    
            double lineX = connectingLine.X1 + (connectingLine.X2 - connectingLine.X1) * linePosition;;
            double lineY = connectingLine.Y1 + (connectingLine.Y2 - connectingLine.Y1) * linePosition;
            double labelLeft = lineX - width / 2;
            double labelTop = lineY - height / 2;
            label.Margin = new Thickness(labelLeft, labelTop, 0, 0);
        }

        private void PositionMessagesGrid(GridView grid, double linePosition)
        {
            if (grid.Grid_main.Children.Count == 0)
            {
                return;
            }
            Rect rect = grid.GetDimentions();
            double width = rect.Width;
            double height = rect.Height;
            Line connectingLine = (Line)additionalControls[AdditionalControlKeys.Line];

            double lineX = connectingLine.X1 + (connectingLine.X2 - connectingLine.X1) * linePosition; ;
            double lineY = connectingLine.Y1 + (connectingLine.Y2 - connectingLine.Y1) * linePosition;
            double labelLeft = lineX - width / 2;
            double labelTop = lineY - height / 2;
            grid.Margin = new Thickness(labelLeft, labelTop, 0, 0);
        }

        /**********************************************************************************************//**
         * Position border.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   border          The border.
         * \param   linePosition    The line position.
         **************************************************************************************************/

        private void PositionBorder(Border border, double linePosition)
        {
            double width = 0;
            double height = 0;
            foreach (BaseChannel channel in presentationElements.Keys)
            {
                Label label = ((Label)presentationElements[channel].controls[PresentationElement.ControlKeys.Label]);
                width += label.Width + 1;
                height = label.Height;
            }
            
            //double width = border.Width;
            //double height = border.Height;
            Line connectingLine = (Line)additionalControls[AdditionalControlKeys.Line];

            double lineX = connectingLine.X1 + (connectingLine.X2 - connectingLine.X1) * linePosition; ;
            double lineY = connectingLine.Y1 + (connectingLine.Y2 - connectingLine.Y1) * linePosition;
            double borderLeft = lineX - width / 2;
            double borderTop = lineY - height / 2;
            border.Margin = new Thickness(borderLeft, borderTop, 0, 0);

        }

        /*
         * SetSelected - define the colors of the channels
         * If the status of the channel is DoNotShow the status will not be change by selecting
         * or deselecting of the channel
         * In order that a channel will be released from DoNotShow status one of the processes 
         * has to be repositioned
         * In the PositionChannel there is a condition that if the processes are not detached
         * from one another they will get the status of DoNotShow.
         * If the reposition detach the processes the status will be change to NotSelected or Selected
         * According to whether the channel is selected in the main window
         * After this condition the controls will be colored in the following way:
         * The poligon of the channel will be colored according to the status
         * If one of the channels are in DoNotShow status the line and label will be colored according to
         * DoNotShow status
         * Else if one of the channels are in Selected status the line and the label will be colored
         * according to Selected color
         * Else (Both channels are NotSelected) the line and the label will be colored according to
         * NotSelected color
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
            if (presentationElements[networkElement].status == MainWindow.SelectedStatus.DoNotShow)
            {
                selectedStatus = MainWindow.SelectedStatus.DoNotShow;
            }
            else
            {
                presentationElements[networkElement].status = selectedStatus;
            }

            // Set the Arrow Head
            ((Polygon)presentationElements[networkElement].controls[PresentationElement.ControlKeys.ArrowHead]).Fill = selectedColors[(int)selectedStatus];

            // Set the Labels
            SetLabelColors((BaseChannel)networkElement, selectedStatus);

            if (presentationElements.Values.Any(pe => pe.status == MainWindow.SelectedStatus.DoNotShow))
            {
                ((Line)additionalControls[AdditionalControlKeys.Line]).Stroke = selectedColors[(int)MainWindow.SelectedStatus.DoNotShow];
                SetBorderColors(selectedColors[(int)MainWindow.SelectedStatus.DoNotShow]);
            }
            else if (presentationElements.Values.Any(pe => pe.status == MainWindow.SelectedStatus.Selected))
            {
                ((Line)additionalControls[AdditionalControlKeys.Line]).Stroke = selectedColors[(int)MainWindow.SelectedStatus.Selected];
                SetBorderColors(selectedColors[(int)MainWindow.SelectedStatus.Selected]);
            }
            else //None of the channels is selected
            {
                ((Line)additionalControls[AdditionalControlKeys.Line]).Stroke = selectedColors[(int)MainWindow.SelectedStatus.NotSelected];
                SetBorderColors(selectedColors[(int)MainWindow.SelectedStatus.NotSelected]);
            }
        }

        /*
         * Sete label colors
         * The method coms to solve the following problem:
         * If the label should not be shown the background has to be Transperant
         * Else the background has to be white (Becouse the label is set on the line)
         */

        /**********************************************************************************************//**
         * Sets label colors.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   channel The brush.
         
         * \param   status  The status.
         **************************************************************************************************/

        private void SetLabelColors(BaseChannel channel, MainWindow.SelectedStatus status)
        {
            Label label = (Label)presentationElements[channel].controls[PresentationElement.ControlKeys.Label];
            if (status == MainWindow.SelectedStatus.DoNotShow)
            {
                label.BorderBrush = Brushes.Transparent;
                label.Background = Brushes.Transparent;
                label.Foreground = Brushes.Transparent;
            }
            else
            {
                UpdateLabel(channel, label);
            }
        }

        /**********************************************************************************************//**
         * Sets border colors.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   brush   The brush.
         **************************************************************************************************/

        private void SetBorderColors(Brush brush)
        {
            Border border = (Border)additionalControls[AdditionalControlKeys.LabelsBorder];
            border.BorderBrush = brush;
        }


        /*
         * A method to select one of the channels that are presented by the presentation
         * The selection is done by pressing the arrow head
         */

        /**********************************************************************************************//**
         * Event handler. Called by ConnectionLine for mouse down events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse button event information.
         
         **************************************************************************************************/

        void ConnectionLine_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Get the selected channel from main window
            BaseChannel selectedChannel = mainWindow.SelectedChannel;
 
            //Get the first channel of the presentation
            BaseChannel channel = (BaseChannel)(presentationElements.Keys.ToList()[0]);

            //If the selected channel is not the first channel of the presentation set the selected
            //channel and return
            if (selectedChannel != channel)
            {
                mainWindow.SetSelectedChannel(channel);
                return;
            }

            //If the selected channel was the first channel of the presentation
            //If there is a second channel in the presentation - select it
            if (presentationElements.Keys.Count > 1)
            {
                channel = (BaseChannel)(presentationElements.Keys.ToList()[1]);
                mainWindow.SetSelectedChannel(channel);
                return;
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by ArrowHead for mouse down events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Mouse button event information.
         
         **************************************************************************************************/

        void ArrowHead_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (presentationElements.Values.Any(Pe => Pe.status != MainWindow.SelectedStatus.DoNotShow))
            {
                BaseChannel channel = (BaseChannel)(presentationElements.First(entry => entry.Value.controls[PresentationElement.ControlKeys.ArrowHead] == sender).Key);
                mainWindow.SetSelectedChannel(channel);
            }
        }

        /*
         * Update the presentation after channel attribute input in ElementInput window
         */

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ///\fn  public override void UpdatePresentation()
        ///
        ///\brief   Updates the presentation.
        ///
        ///\author  Ilan Hindy
        ///\date    29/09/2016
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void UpdatePresentation()
        {
            foreach (var entry in presentationElements)
            {
                if (entry.Key.ea[bc.eak.SourceProcess] !=
                    entry.Key.ea[bc.eak.DestProcess])
                {
                    Label channelLabel = ((Label)(presentationElements[entry.Key].controls[PresentationElement.ControlKeys.Label]));
                    UpdateLabel((BaseChannel)entry.Key, channelLabel);
                }
            }
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
            //string msg = "Updating presentation of channel " +
            //    ((BaseChannel)networkElement).ElementAttributes[NetworkElement.ElementAttributeKeys.Id].ToString() +
            //    " The text is :" + ((BaseChannel)networkElement).pp[bc.ppk.PresentationText];
            //CustomizedMessageBox.Show(msg, "", Icons.Info);
            //Update the incomming message
            GetRelativeLinePositionForLabel(networkElement);
            Label channelLabel = ((Label)(presentationElements[networkElement].controls[PresentationElement.ControlKeys.Label]));
            //UpdateMessagesLabel((BaseChannel)networkElement, (string)parameters[0]);
            UpdateMessagesGrid((BaseChannel)networkElement, (List<string>) parameters[0]);
            int sourceProcess = networkElement.ea[bc.eak.SourceProcess];
            int destProcess = networkElement.ea[bc.eak.DestProcess];
            if (sourceProcess != destProcess)
            {
                UpdateLabel((BaseChannel)networkElement, channelLabel);
            }

            CreateFloatingSummary();
        }

        /**********************************************************************************************//**
         * Updates the messages label.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   channel         The channel.
         * \param   messagesString  The messages string.
         **************************************************************************************************/
        private void UpdateMessagesGrid(BaseChannel channel, List<string> messagesStrings)
        {
            if (!mainWindow.showMessages) return;
            GridView gridView = ((GridView)(presentationElements[channel].controls[PresentationElement.ControlKeys.MessagesGridView]));
            gridView.ReplaceContent(messagesStrings);
            PositionMessagesGrid(gridView, GetRelativeLinePositionForLabel(channel));
            gridView.MoveToFront();
        }

        public void SetMessagesVisibility(BaseChannel channel, Visibility visibility)
        {
            GridView gridView = ((GridView)(presentationElements[channel].controls[PresentationElement.ControlKeys.MessagesGridView]));
            gridView.Visibility = visibility;
        }

        /**********************************************************************************************//**
         * Gets relative line position for label.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         *
         * \return  The relative line position for label.
         .
         **************************************************************************************************/

        private double GetRelativeLinePositionForLabel(NetworkElement networkElement)
        {
            double lineX1 = ((Line)additionalControls[AdditionalControlKeys.Line]).X1;
            double lineY1 = ((Line)additionalControls[AdditionalControlKeys.Line]).Y1;
            Polygon arrowHead = (Polygon)presentationElements[networkElement].controls[PresentationElement.ControlKeys.ArrowHead];
            double polygonX1 = arrowHead.Points[0].X;
            double polygonY1 = arrowHead.Points[0].Y;
            if (lineX1 == polygonX1 && lineY1 == polygonY1)
            {
                return labelRelativePosition[1];
            }
            else
            {
                return labelRelativePosition[0];
            }
        }

        #region /// \name Floating summary

        

        public void labelsPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowFloatingSummary();
        }

        public void labelsPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            HideFloatingSummaryIfNeeded(e);
        }

        protected override FrameworkElement ElementObject()
        {
            return (FrameworkElement)additionalControls[AdditionalControlKeys.LabelsBorder];
        }

        #endregion
    }
}
