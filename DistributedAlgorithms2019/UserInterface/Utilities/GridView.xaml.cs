////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file UserInterface\Utilities\GridView.xaml.cs
///
/// \brief Implements the grid view.xaml class.
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
using System.Windows.Controls.Primitives;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class GridView
    ///
    /// \brief Interaction logic for GridView.xaml.
    ///
    /// \par Description.
    ///      -  The GridView control is a control that presents a list of strings and allows  
    ///         Changing the order of the strings in the list
    ///
    /// \par Usage Notes.
    ///      To use the control:
    ///      -# In the object that the GridView present create a method with the signature of the ChangeFinished() delegate. 
    ///         This method should implement the order change. This method is called when the user finished the 
    ///         changes.
    ///      -# Assign the method to the ChangeFinishedEvent() of this class 
    ///      -# Create a method with the signature of PresentPermutation() delegate. This method
    ///         should create 2 strings one for the original list and one for the list after the
    ///         transformation. This method is used to present the results of the order change
    ///         for the end change dialog
    ///      -# Pass this method is as a parameter to the constructor
    ///      -# The GridView is created once and each time a modification is needed call ReplaceContent()
    ///
    /// \author Ilanh
    /// \date 27/05/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class GridView : UserControl
    {
        #region /// \name Delegates and events

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public delegate void ChangeFinished(List<int> permutations, int presentedItemId);
        ///
        /// \brief Change finished.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param permutations     (List&lt;int&gt;) - The permutations.
        /// \param presentedItemId  (int) - Identifier for the presented item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public delegate void ChangeFinished(List<int> permutations, int presentedItemId);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ChangeFinished) - The change finished event.
        ///        -   This event is activated if the user decides to apply the change
        ///        -   In the methods assigned to this event there is an implementation
        ///            of the order change in the object that this GridView presents.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ChangeFinished ChangeFinishedEvent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public delegate void PresentPermutation(List<int> permutations, List<string> originalContentout, out string originalString, out string transforedString, List<int> idxs = null);
        ///
        /// \brief Present permutation.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param       permutations       (List&lt;int&gt;) - The permutations.
        /// \param       originalContentout (List&lt;string&gt;) - The original contentout.
        /// \param [out] originalString     (out string) - The original string.
        /// \param [out] transforedString   (out string) - The transfored string.
        /// \param       idxs               (Optional)  (List&lt;int&gt;) - The idxs.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public delegate void PresentPermutation(List<int> permutations, List<string> originalContentout, out string originalString, out string transforedString, List<int> idxs = null);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (PresentPermutation) - The present permutation event.
        ///        -   This event is activated in order to present to the user the results
        ///            of the order change
        ///        -   The calling class should create a method that generates 2 strings:
        ///            -#  The original list
        ///            -#  The list after the change
        ///        -   This event is assigned in the constructor.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private PresentPermutation PresentPermutationEvent;
        #endregion
        #region /// \name Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;int&gt;) - The permutations.
        ///        This list holds the permutations. If x is in index y that means
        ///        that the string that was in index y in the original list was moved to index x.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<int> permutations;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Label) - Source label.
        ///        -   This member holds the source label (The label that is moved.
        ///        -   This member is needed because the events thrown are from the
        ///            label that the cursor is on (the target labels)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Label sourceLabel;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Canvas) - The canvas.
        ///        The canvas that the grid is presented on.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Canvas canvas;

        /// \brief  (int) - Identifier for the presented item.
        private int presentedItemId;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (List&lt;string&gt;) - The original content.
        ///        Holds the list before the transformation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<string> originalContent;
        #endregion
        #region /// \name Constructor and change content

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public GridView(Canvas canvas,List<string> content, int presentedItemId, PresentPermutation presentPermutationFunc)
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
        /// \date 27/05/2018
        ///
        /// \param canvas                  (Canvas) - The canvas.
        /// \param content                 (List&lt;string&gt;) - The content.
        /// \param presentedItemId         (int) - Identifier for the presented item.
        /// \param presentPermutationFunc  (PresentPermutation) - The present permutation function.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public GridView(Canvas canvas,List<string> content, int presentedItemId, PresentPermutation presentPermutationFunc)
        {
            InitializeComponent();
            this.canvas = canvas;
            this.presentedItemId = presentedItemId;
            originalContent = content;
            PresentPermutationEvent += presentPermutationFunc;
            ReplaceContent(content);            
        }
       

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ReplaceContent(List<string> content)
        ///
        /// \brief Replace content.
        ///
        /// \par Description.
        ///      This method should be used when the content is changed outside the GridView 
        ///      (in the object that this GridView presents)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param content  (List&lt;string&gt;) - The content.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReplaceContent(List<string> content)
        {
            Grid_main.Children.Clear();
            for (int idx = 0; idx < content.Count; idx++)
            {
                Grid_main.RowDefinitions.Add(new RowDefinition());
                Label label = new Label();
                label.Content = content[idx];
                label.Height = 25;
                label.Background = Brushes.White;
                label.MouseLeftButtonUp += Label_MouseLeftButtonUp;
                label.MouseMove += Label_MouseMove;
                //label.GiveFeedback += Label_GiveFeedback;
                label.AllowDrop = true;
                label.DragEnter += Label_DragEnter;
                label.DragLeave += Label_DragLeave;
                label.DragOver += Label_DragOver;
                label.Drop += Label_Drop;
                Grid_main.Children.Add(label);
                Grid.SetRow(label, idx);
                Grid.SetColumn(label, 0);
                permutations = new List<int>();
                int i = 0;
                content.ForEach(s => permutations.Add(i++));
                originalContent = new List<string>(content);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Rect GetDimentions()
        ///
        /// \brief Gets the dimensions.
        ///
        /// \par Description.
        ///      Calculate the dimensions of the GridView
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \return The dimentions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Rect GetDimentions()
        {
            double width = 0;
            double height = 0;
            foreach (Label label in Grid_main.Children)
            {
                FormattedText formattedText = new FormattedText(
                                (string)label.Content,
                                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                                FlowDirection.LeftToRight,
                                new Typeface(label.FontFamily.ToString()),
                                label.FontSize,
                                Brushes.Black);
                width = (formattedText.Width + 12 > width) ? formattedText.Width + 12 : width;
                height += formattedText.Height + 10;
            }
            return new Rect(0,0,width,height);
        }
        #endregion
        #region /// \name Move the Grid to front

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Label_MouseLeftButtonUp(object sender, MouseEventArgs e)
        ///
        /// \brief Event handler. Called by Label for mouse left button up events.
        ///
        /// \par Description.
        ///      This method is used to bring the GridView in front of other controls in the canvas
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (MouseEventArgs) - Mouse event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Label_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            MoveToFront();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void MoveToFront()
        ///
        /// \brief Move to front.
        ///
        /// \par Description.
        ///      This method is used to bring the GridVive in front of other controls in the canvas
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MoveToFront()
        {
            canvas.Children.Remove(this);
            canvas.Children.Add(this);
        }
        #endregion
        #region /// \name Drag and Drop process
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Label_MouseMove(object sender, MouseEventArgs e)
        ///
        /// \brief Enabling the object to be drag source.
        ///
        /// \par Description.
        ///      This method starts the dragging operation
        ///
        /// \par Algorithm.
        ///      -#  Set the source label  
        ///      -#  Change the color of the source label  
        ///      -#  Start DragDrop operation
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (MouseEventArgs) - Mouse event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            sourceLabel = label;

            if (label != null && e.LeftButton == MouseButtonState.Pressed)
            {
                MoveToFront();
                label.Foreground = Brushes.White;
                label.Background = Brushes.Blue;
                DragDrop.DoDragDrop(label,
                                 label.Content,
                                 DragDropEffects.Move);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Label_DragEnter(object sender, DragEventArgs e)
        ///
        /// \brief Enabling the object to be a drag target
        ///
        /// \par Description.
        ///      -  The DragEnter event specifies how the target object will behave
        ///         when the source object will pass on it.
        ///
        /// \par Algorithm.
        ///      -  The method of the "drag" is to change the texts in the labels and the colors
        ///      -  The labels themselves are not replacing places
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (DragEventArgs) - Drag event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Label_DragEnter(object sender, DragEventArgs e)
        {
            Label label = sender as Label;
            MessageRouter.ReportMessage("GridView", "In DragEnter source = " + sourceLabel.Content + " target = " + label.Content, "");
            if (label != null)
            {
                sourceLabel.Foreground = Brushes.Black;
                sourceLabel.Background = Brushes.White;

                label.Foreground = Brushes.White;
                label.Background = Brushes.Blue;
                ReOrder(label);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Label_DragOver(object sender, DragEventArgs e)
        ///
        /// \brief
        ///
        /// \par Description.
        ///      The DragOver event occurs in the target object continuously while the
        ///      source object is passes on it.
        ///
        /// \par Algorithm.
        ///      Does nothing
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (DragEventArgs) - Drag event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Label_DragOver(object sender, DragEventArgs e)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ReOrder(Label targetLabel)
        ///
        /// \brief Re order.
        ///
        /// \par Description.
        ///      -  This method change the order of the list  
        ///      -  The labels are not changed only their text
        ///
        /// \par Algorithm.
        ///      
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param targetLabel  (Label) - Target label.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ReOrder(Label targetLabel)
        {
            // Get the rows of the source and target labels
            int sourceLabelRow = Grid.GetRow(sourceLabel);
            int targetLabelRow = Grid.GetRow(targetLabel);

            // If the source and target rows are the same - exit
            if (sourceLabelRow == targetLabelRow)
            {
                return;
            }

            int sourcePermutation = permutations[sourceLabelRow];
            
            // If the sourceLabelRow is smaller than the targetLabelRow that means that the
            // drag is moving downwards The following is an example of what is done:
            // Original list:
            // [0] text0
            // [1] text1
            // [2] text2
            // [3] text3
            // Drag results:
            // source row : 0
            // target row : 2
            // Transformation step 1
            // [0] is set to 1 (row = 1)
            // [1] is set to 2 (row = 2)
            // The result of this step is [1,2,2,3]
            // Transformation step 2
            // Move the source row to the target index
            // The result of this step is [1,2,0,3]
            
            if (sourceLabelRow  < targetLabelRow)
            {
                for (int row = sourceLabelRow + 1; row <= targetLabelRow; row ++)
                {                  
                    permutations[row - 1] = permutations[row];                    
                }
            }

            // If the sourceLabelRow is smaller than the targetLabelRow that means that the
            // drag is moving downwards The following is an example of what is done:
            // Original list:
            // [0] text0
            // [1] text1
            // [2] text2
            // [3] text3
            // Drag results:
            // source row : 2
            // target row : 0
            // Transformation step 1
            // [2] is set to 1 (row = 1)
            // [1] is set to 0 (row = 0)
            // The result of this step is [0,0,1,3]
            // Transformation step 2
            // Move the source row to the target index
            // The result of this step is [2,0,1,3]
            else if (sourceLabelRow > targetLabelRow)
            {
                for(int row = sourceLabelRow - 1; row >= targetLabelRow; row --)
                {
                    permutations[row + 1] = permutations[row];
                }
            }
            permutations[targetLabelRow] = sourcePermutation;

            // Move the texts according to the permutation
            SetContents();
            sourceLabel = targetLabel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetContents()
        ///
        /// \brief Sets the contents.
        ///
        /// \par Description.
        ///      -  Change the content of the labels according to the permutation 
        ///      -  If index x holds value y that means that the string that was in label y is now in label x
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetContents()
        {
            for (int idx = 0; idx < permutations.Count; idx++)
            {
                Label label = Grid_main.Children.Cast<Label>()
                                    .First(e => Grid.GetRow(e) == idx && Grid.GetColumn(e) == 0);
                label.Content = originalContent[permutations[idx]];
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Label_DragLeave(object sender, DragEventArgs e)
        ///
        /// \brief 
        ///
        /// \par Description.
        ///      - The DragLeave event occurs in the target object when the source object leaves it.  
        ///      - The method changes the colors of the leaved label to not selected colors
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (DragEventArgs) - Drag event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Label_DragLeave(object sender, DragEventArgs e)
        {
            Label label = sender as Label;
            if (label != null)
            {
                if (sourceLabel != label)
                {
                    label.Foreground = Brushes.Black;
                    label.Background = Brushes.White;
                }
            }
        }
        #endregion
        #region /// \name Drag operation finished

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Label_Drop(object sender, DragEventArgs e)
        ///
        /// \brief 
        ///
        /// \par Description.
        ///      -  The Drop event occurs in the target when the mouse is released.  
        ///      -  There are 3 options :  
        ///         -#  __Continue__ the change (In this case the permutation and the labels are not changed)
        ///         -#  __Apply__ (In this case the methods of the event are called)
        ///         -#  __Quit__ Change (In this case the permutations list and the label strings are restored)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (DragEventArgs) - Drag event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Label_Drop(object sender, DragEventArgs e)
        {
            Label label = (Label)sender;
            MessageRouter.ReportMessage("GridView", "In DragDrop source = " + sourceLabel.Content + " target = " + label.Content, "");
            if (label != null)
            {
                label.Foreground = Brushes.Black;
                label.Background = Brushes.White;
                string result = EndDragQuestion();
                switch (result)
                {
                    case "Continue Change":
                        break;
                    case "Apply":
                        ChangeFinishedEvent(permutations, presentedItemId);
                        permutations = null;
                        break;
                    case "Quit Change":
                        for (int idx = 0; idx < permutations.Count; idx ++)
                        {
                            permutations[idx] = idx;
                        }
                        SetContents();
                        break;
                }               
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string EndDragQuestion()
        ///
        /// \brief Ends drag question.
        ///
        /// \par Description.
        ///      Ask the user what he wants to do with the change
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string EndDragQuestion()
        {
            
            MessageBoxElementData l1 = new MessageBoxElementData("The permutation list is :", new Font { fontWeight = FontWeights.Bold, wrap = TextWrapping.NoWrap });
            MessageBoxElementData l2 = new MessageBoxElementData("The source list is :", new Font { fontWeight = FontWeights.Bold, wrap = TextWrapping.NoWrap });
            string originalString;
            string transforedString;
            PresentPermutationEvent(permutations, originalContent, out originalString, out transforedString);
            MessageBoxElementData l3 = new MessageBoxElementData(originalString, new Font { wrap = TextWrapping.NoWrap });
            MessageBoxElementData l4 = new MessageBoxElementData("The transformed list is :", new Font { fontWeight = FontWeights.Bold, wrap = TextWrapping.NoWrap });
            MessageBoxElementData l5 = new MessageBoxElementData(transforedString, new Font { wrap = TextWrapping.NoWrap });
            MessageBoxElementData l6 = new MessageBoxElementData("What do you want to do?", new Font { fontWeight = FontWeights.Bold, wrap = TextWrapping.NoWrap });
            MessageBoxElementData b1 = new MessageBoxElementData("Continue Change", new Font { fontWeight = FontWeights.Bold, wrap = TextWrapping.NoWrap });
            MessageBoxElementData b2 = new MessageBoxElementData("Apply");
            MessageBoxElementData b3 = new MessageBoxElementData("Quit Change", new Font { fontWeight = FontWeights.Bold, wrap = TextWrapping.NoWrap });
            return  MessageRouter.CustomizedMessageBox(new List<MessageBoxElementData> { l1, l2, l3, l4, l5, l6 }, "GridView Message", new List<MessageBoxElementData> { b1, b2, b3 }, Icons.Question, true);

        }
        #endregion
    }
}
