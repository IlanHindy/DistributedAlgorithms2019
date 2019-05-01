////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\CustomizedMessageBox.cs
///
///\brief   Implements the customized message box class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Documents;
using System.IO;
using System.Text.RegularExpressions;


namespace DistributedAlgorithms
{
    #region /// \name Enums    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \enum Icons
    ///
    /// \brief Values that represent icons.
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum Icons { Info, Warning, Success, Question, Error };
#endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class Font
    ///
    /// \brief A font.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 10/12/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Font
    {
        #region /// \name Font Attributes
        /// \brief  The font family.
        public FontFamily fontFamily = new FontFamily("Calibbri");
        /// \brief  Size of the font.
        public double fontSize = 16;
        /// \brief  The font style.
        public FontStyle fontStyle = FontStyles.Normal;
        /// \brief  The font weight.
        public FontWeight fontWeight = FontWeights.Normal;
        /// \brief  (Thickness) - The margin.
        public Thickness margin = new Thickness(0);
        /// \brief  (HorizontalAlignment) - The alignment.
        public HorizontalAlignment alignment = HorizontalAlignment.Left;
        /// \brief  (Brush) - for color.
        public Brush forColor = Brushes.Black;
        /// \brief  (Brush) - The back color.
        public Brush backColor = Brushes.White;
        /// \brief  (TextWrapping) - The wrap.
        public TextWrapping wrap = TextWrapping.Wrap;
        #endregion
        #region /// \name Font Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Font()
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

        public Font()
        { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Font(string fontFamily, double fontSize, FontStyle fontStyle, FontWeight fontWeight, Thickness margin, HorizontalAlignment alignment, Brush forColor, Brush backColor, TextWrapping wrap)
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
        /// \date 10/12/2017
        ///
        /// \param fontFamily (string) - The font family.
        /// \param fontSize   (double) - Size of the font.
        /// \param fontStyle  (FontStyle) - The font style.
        /// \param fontWeight (FontWeight) - The font weight.
        /// \param margin     (Thickness) - The margin.
        /// \param alignment  (HorizontalAlignment) - The alignment.
        /// \param forColor    (Brush) - for color.
        /// \param backColor   (Brush) - The back color.
        /// \param wrap        (TextWrapping) - The wrap.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Font(string fontFamily, 
            double fontSize, 
            FontStyle fontStyle, 
            FontWeight fontWeight, 
            Thickness margin, 
            HorizontalAlignment alignment,
            Brush forColor,
            Brush backColor,
            TextWrapping wrap)
        {
            this.fontFamily = new FontFamily(fontFamily);
            this.fontSize = fontSize;
            this.fontStyle = fontStyle;
            this.fontWeight = fontWeight;
            this.margin = margin;
            this.alignment = alignment;
            this.forColor = forColor;
            this.backColor = backColor;
            this.wrap = wrap;
        }
        #endregion
        #region /// \name Font Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetFont(Control control)
        ///
        /// \brief Sets a font.
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
        /// \param control  (Control) - The control.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetFont(Control control)
        {
            control.FontFamily = fontFamily;
            control.FontSize = fontSize;
            control.FontStyle = fontStyle;
            control.FontWeight = fontWeight;
            control.Margin = margin;
            control.HorizontalAlignment = alignment;
            control.Foreground = forColor;
            control.Background = backColor;
            if (control is TextBox)
            {
                ((TextBox)control).TextWrapping = wrap;
            }
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetFontAndDimentions(Control control, string text)
        ///
        /// \brief Sets font and dimensions.
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
        /// \param control  (Control) - The control.
        /// \param text     (string) - The text.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetFontAndDimensions(Control control, string text)
        {
            SetFont(control);
            FormattedText formattedText = new FormattedText(
                        text,
                        System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface(fontFamily, fontStyle, fontWeight, control.FontStretch),
                        fontSize,
                        Brushes.Black);
            control.Width = formattedText.Width + 12;
            control.Height = formattedText.Height + 10;
            if (control is TextBox)
            {
                ((TextBox)control).TextWrapping = wrap;
            }
        }
    }
    #endregion
    #region /// \name MessageBoxElementData

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class MessageBoxElementData
    ///
    /// \brief A message box element data.
    ///        This data structure is used for creating a customized message box with
    ///        customized labels and buttons from the NetworkElements (In running phase)
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 16/04/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class MessageBoxElementData
    {
        public Font font = new Font();
        public string label;
        public MessageBoxElementData(string label, Font font = null)
        {
            this.label = label;
            if (font == null)
            {
                this.font = new Font();
            }
            else
            {
                this.font = font;
            }
        }
    }
    #endregion


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class CustomizedMessageBox
    ///
    /// \brief A customized message box.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 10/12/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class CustomizedMessageBox : Window
    {
        #region /// \name Members
        /// \brief  The pan main.
        DockPanel panMain;
        /// \brief  The pan text and image.
        DockPanel panTextAndImage;
        /// \brief  The pan image.
        StackPanel panImage;
        /// \brief  The pan text.
        StackPanel panText;
        /// \brief  The pan buttons.
        Grid panButtons;
        /// \brief  The result.
        private string Result = "";
        /// \brief  The gwl style.
        private const int GWL_STYLE = -16;
        /// \brief  The ws sysmenu.
        private const int WS_SYSMENU = 0x80000;
#endregion

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        ///
        /// \brief Gets window long.
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
        /// \param hWnd    (IntPtr) - The window.
        /// \param nIndex  (int) - The index.
        ///
        /// \return The window long.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        ///
        /// \brief Sets window long.
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
        /// \param hWnd       (IntPtr) - The window.
        /// \param nIndex     (int) - The index.
        /// \param dwNewLong  (int) - The new long.
        ///
        /// \return An int.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #region /// \name Methods that actually build the MessageBox
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private CustomizedMessageBox()
        ///
        /// \brief Constructor that prevents a default instance of this class from being created.
        ///
        /// \par Description.
        ///      The static method Show builds a CustomizedMessageBox object using this constructor
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/12/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private CustomizedMessageBox()
        {
            // panImage
            panImage = new StackPanel();
            DockPanel.SetDock(panImage, Dock.Left);
            panImage.VerticalAlignment = VerticalAlignment.Center;
            panImage.Margin = new Thickness(10, 10, 0, 0);

            // panText
            panText = new StackPanel();
            panText.HorizontalAlignment = HorizontalAlignment.Stretch;
            panText.VerticalAlignment = VerticalAlignment.Center;
            panText.Margin = new Thickness(10);

            // panButtons 
            panButtons = new Grid();
            panButtons.Margin = new Thickness(10);

            //PanTextAndImage
            panTextAndImage = new DockPanel();
            panTextAndImage.Children.Add(panImage);
            panTextAndImage.Children.Add(panText);
            panButtons.VerticalAlignment = VerticalAlignment.Center;
            DockPanel.SetDock(panTextAndImage, Dock.Top);

            //panMain
            panMain = new DockPanel();
            panMain.Children.Add(panTextAndImage);
            panMain.Children.Add(panButtons);

            //Window
            SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Loaded += DisableTitleButtons;
            AddChild(panMain);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show(List<Control> controls = null, string title = "", List<Button> buttonsList = null, Icons imageIcon = Icons.Info, bool sizeToContent)
        ///
        /// \brief Shows.
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
        /// \param controls      (Optional)  (List&lt;Control&gt;) - The controls.
        /// \param title         (Optional)  (string) - The title.
        /// \param buttonsList   (Optional)  (List&lt;Button&gt;) - List of buttons.
        /// \param imageIcon     (Optional)  (Icons) - The image icon.
        /// \param sizeToContent  (bool) - true to size to content.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show(List<Control> controls, string title, List<Button> buttonsList, Icons imageIcon, bool sizeToContent)
        {
            if (controls == null) controls = new List<Control>();
            if (controls.Count == 0) controls.Add(SetLabel(""));
            if (buttonsList == null)
            {
                buttonsList = new List<Button>();
                buttonsList.Add(SetButton("OK"));
            }

            CustomizedMessageBox messageBox = new CustomizedMessageBox();

            messageBox.Title = title;

            //Image
            Image image = SetImage(imageIcon);
            messageBox.panImage.Children.Add(image);

            //Controls
            string fullMessage = "";
            foreach (Control control in controls)
            {
                messageBox.panText.Children.Add(control);
                if (control is TextBox)
                {
                    fullMessage += ((TextBox)control).Text + "\n";
                }
                else if (control is SyntaxHighlight)
                {
                    messageBox.MaxWidth = 800;
                    fullMessage += ((SyntaxHighlight)control).Text;                  
                }
            }

            //Buttons
            int insertIdx = 0;
            foreach (Button button in buttonsList)
            {
                messageBox.panButtons.ColumnDefinitions.Add(new ColumnDefinition());
                messageBox.panButtons.Children.Add(button);
                Grid.SetColumn(button, insertIdx);
                Grid.SetRow(button, 0);
                button.Click += new RoutedEventHandler(messageBox.Button_Click);
                insertIdx++;
            }
            if (!sizeToContent)
            {
                messageBox.MaxWidth = 500;
            }
            MessageRouter.AddEditOperation(title, fullMessage, imageIcon, new Font("Calibbri", 12, FontStyles.Normal, FontWeights.Normal, new Thickness(0), HorizontalAlignment.Left, Brushes.Black, null, TextWrapping.NoWrap));
            messageBox.ShowDialog();
            MessageRouter.AddEditOperationResult(messageBox.Result, new Font("Calibbri", 12, FontStyles.Normal, FontWeights.Normal, new Thickness(0), HorizontalAlignment.Left, Brushes.Black, null, TextWrapping.NoWrap));
            return messageBox.Result;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void DisableTitleButtons(object sender, RoutedEventArgs e)
        ///
        /// \brief Disables the title buttons.
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

        private void DisableTitleButtons(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        #endregion
        #region /// \name Show interfaces

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show(Control control, string title = "", List<Button> buttons = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        ///
        /// \brief Shows.
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
        /// \param control       (Control) - The control.
        /// \param title         (Optional)  (string) - The title.
        /// \param buttons       (Optional)  (List&lt;Button&gt;) - The buttons.
        /// \param imageIcon     (Optional)  (Icons) - The image icon.
        /// \param sizeToContent (Optional)  (bool) - true to size to content.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show(Control control, string title = "", List<Button> buttons = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            controls.Add(control);
            return Show(controls, title, buttons, imageIcon, sizeToContent);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show(string labelString, string title = "", List<Button> buttons = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        ///
        /// \brief Shows.
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
        /// \param labelString   (string) - The label string.
        /// \param title         (Optional)  (string) - The title.
        /// \param buttons       (Optional)  (List&lt;Button&gt;) - The buttons.
        /// \param imageIcon     (Optional)  (Icons) - The image icon.
        /// \param sizeToContent (Optional)  (bool) - true to size to content.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show(string labelString, string title = "", List<Button> buttons = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            controls.Add(SetLabel(labelString));
            return Show(controls, title, buttons, imageIcon, sizeToContent);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show(List<string> labelsStrings, string title = "", List<string> buttonsStrings = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        ///
        /// \brief Shows.
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
        /// \param labelsStrings  (List&lt;string&gt;) - The labels strings.
        /// \param title          (Optional)  (string) - The title.
        /// \param buttonsStrings (Optional)  (List&lt;string&gt;) - The buttons strings.
        /// \param imageIcon      (Optional)  (Icons) - The image icon.
        /// \param sizeToContent  (Optional)  (bool) - true to size to content.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show(List<string> labelsStrings, string title = "", List<string> buttonsStrings = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            foreach (string labelString in labelsStrings)
            {
                controls.Add(SetLabel(labelString));
            }
            if (buttonsStrings == null)
            {
                buttonsStrings = new List<string> { "OK" };
            }
            List<Button> buttons = new List<Button>();
            foreach (string buttonString in buttonsStrings)
            {
                buttons.Add(SetButton(buttonString));
            }
            return Show(controls, title, buttons, imageIcon, sizeToContent);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static MessageBoxResult Show(string labelString, string title, MessageBoxButton messageBoxButtons, Icons imageIcon = Icons.Question, bool sizeToContent = false)
        ///
        /// \brief Shows.
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
        /// \param labelString       (string) - The label string.
        /// \param title             (string) - The title.
        /// \param messageBoxButtons (MessageBoxButton) - The message box buttons.
        /// \param imageIcon         (Optional)  (Icons) - The image icon.
        /// \param sizeToContent     (Optional)  (bool) - true to size to content.
        ///
        /// \return A MessageBoxResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static MessageBoxResult Show(string labelString, string title, MessageBoxButton messageBoxButtons, Icons imageIcon = Icons.Question, bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            controls.Add(SetLabel(labelString));
            List<Button> buttons = GenerateButtonsAccordingToEnum(messageBoxButtons);
            return TranslateResultToEnum(Show(controls, title, buttons, imageIcon, sizeToContent));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static MessageBoxResult Show(List<string> labelsStrings, string title, MessageBoxButton messageBoxButtons, Icons imageIcon = Icons.Question, bool sizeToContent = false)
        ///
        /// \brief Shows.
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
        /// \param labelsStrings     (List&lt;string&gt;) - The labels strings.
        /// \param title             (string) - The title.
        /// \param messageBoxButtons (MessageBoxButton) - The message box buttons.
        /// \param imageIcon         (Optional)  (Icons) - The image icon.
        /// \param sizeToContent     (Optional)  (bool) - true to size to content.
        ///
        /// \return A MessageBoxResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static MessageBoxResult Show(List<string> labelsStrings, string title, MessageBoxButton messageBoxButtons, Icons imageIcon = Icons.Question, bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            foreach (string labelString in labelsStrings)
            {
                controls.Add(SetLabel(labelString));
            }
            List<Button> buttons = GenerateButtonsAccordingToEnum(messageBoxButtons);
            return TranslateResultToEnum(Show(controls, title, buttons, imageIcon, sizeToContent));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show(string labelString, string title = "", Icons imageIcon = Icons.Info, List<Button> buttons = null, bool sizeToContent = false)
        ///
        /// \brief Shows.
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
        /// \param labelString   (string) - The label string.
        /// \param title         (Optional)  (string) - The title.
        /// \param imageIcon     (Optional)  (Icons) - The image icon.
        /// \param buttons       (Optional)  (List&lt;Button&gt;) - The buttons.
        /// \param sizeToContent (Optional)  (bool) - true to size to content.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show(string labelString, string title = "", Icons imageIcon = Icons.Info, List<Button> buttons = null, bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            controls.Add(SetLabel(labelString));
            return Show(controls, title, buttons, imageIcon, sizeToContent);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show (List<Control> controls, string title, List<string> buttonsStrs, Icons imageIcon)
        ///
        /// \brief Shows.
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
        /// \param controls     (List&lt;Control&gt;) - The controls.
        /// \param title        (string) - The title.
        /// \param buttonsStrs  (List&lt;string&gt;) - The buttons strs.
        /// \param imageIcon    (Icons) - The image icon.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show (List<Control> controls, string title, List<string> buttonsStrs, Icons imageIcon, bool sizeToContent = false)
        {
            List<Button> buttons = new List<Button>();
            foreach (string buttonStr in buttonsStrs)
            {
                buttons.Add(SetButton(buttonStr));
            }
            return Show(controls, title, buttons, imageIcon, sizeToContent);
        }
        #endregion
        #region /// \name FileMsg interfaces

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string FileMsg (string beforeString, string fileName, string path, string afterString, string title, List<string> buttons = null, Icons imageIcon = Icons.Info)
        ///
        /// \brief File message.
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
        /// \param beforeString  (string) - The before string.
        /// \param fileName      (string) - Filename of the file.
        /// \param path          (string) - Full pathname of the file.
        /// \param afterString   (string) - The after string.
        /// \param title         (string) - The title.
        /// \param buttons      (Optional)  (List&lt;string&gt;) - The buttons.
        /// \param imageIcon    (Optional)  (Icons) - The image icon.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string FileMsg (string beforeString, 
            string fileName, 
            string path, 
            string afterString,
            string title,
            List<string> buttons = null,
            Icons imageIcon = Icons.Info)
        {
            List<Control> controls = new List<Control>();
            controls.Add(SetLabel(beforeString));
            controls.Add(SetLabel("File:", new Font { fontWeight = FontWeights.Bold }));
            controls.Add(SetLabel(fileName));
            controls.Add(SetLabel("Path:", new Font { fontWeight = FontWeights.Bold }));
            controls.Add(SetLabel(Path.GetFullPath(path)));
            if (afterString is null)
                controls.Add(SetLabel(afterString));
            else
            {
                if (afterString == "")
                    controls.Add(SetLabel(afterString));
                else controls.Add(SetLabel(afterString));
            }


            if (buttons is null)
            {
                buttons = new List<string> { "OK" };
            }
            return Show(controls, title, buttons, imageIcon);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string FileMsgErr(string beforeString, string fileName, string path, string error, string title, List<string> buttonsStrings = null)
        ///
        /// \brief File message error.
        ///        Generate a CustomizedMessageBox for an error in file opening/closing
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 16/04/2018
        ///
        /// \param beforeString    (string) - The before string.
        /// \param fileName        (string) - Filename of the file.
        /// \param path            (string) - Full pathname of the file.
        /// \param error           (string) - The error.
        /// \param title           (string) - The title.
        /// \param buttonsStrings (Optional)  (List&lt;string&gt;) - The buttons strings.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string FileMsgErr(string beforeString,
            string fileName,
            string path,
            string error,
            string title,
            List<string> buttonsStrings = null,
            bool sizeToContent = false)
        {
            List<Control> controls = new List<Control>();
            controls.Add(SetLabel(beforeString));
            controls.Add(SetLabel("File:", new Font { fontWeight = FontWeights.Bold }));
            controls.Add(SetLabel(fileName));
            controls.Add(SetLabel("Path:", new Font { fontWeight = FontWeights.Bold }));
            controls.Add(SetLabel(Path.GetFullPath(path)));
            controls.Add(SetLabel("Error:", new Font { fontWeight = FontWeights.Bold }));
            controls.Add(SetLabel(error));
            List<Button> buttons = new List<Button>();
            if (buttonsStrings is null)
            {
                buttons.Add(SetButton("OK"));
            }
            else
            {
                foreach (string buttonString in buttonsStrings)
                {
                    buttons.Add(SetButton(buttonString));
                }
            }
            return Show(controls, title, buttons, Icons.Error, sizeToContent);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string FileMsg(string beforeString, string fileName, string path, List<Control> controls, string title, List<string> buttons = null, Icons imageIcon = Icons.Info)
        ///
        /// \brief File message.
        ///        Customized message box for file handling with TextBox list
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 16/04/2018
        ///
        /// \param beforeString  (string) - The before string.
        /// \param fileName      (string) - Filename of the file.
        /// \param path          (string) - Full pathname of the file.
        /// \param controls      (List&lt;Control&gt;) - The controls.
        /// \param title         (string) - The title.
        /// \param buttons      (Optional)  (List&lt;string&gt;) - The buttons.
        /// \param imageIcon    (Optional)  (Icons) - The image icon.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string FileMsg(string beforeString,
            string fileName,
            string path,
            List<Control> controls,
            string title,
            List<string> buttons = null,
            Icons imageIcon = Icons.Info)
        {
            int insertionIdx = 0;
            controls.Insert(insertionIdx++, SetLabel(beforeString));
            controls.Insert(insertionIdx++, SetLabel("File:", new Font { fontWeight = FontWeights.Bold }));
            controls.Insert(insertionIdx++, SetLabel(fileName));
            controls.Insert(insertionIdx++, SetLabel("Path:", new Font { fontWeight = FontWeights.Bold }));
            controls.Insert(insertionIdx++, SetLabel(Path.GetFullPath(path)));
            if (buttons is null)
            {
                buttons = new List<string> { "OK" };
            }
            return Show(controls, title, buttons, imageIcon);

        }
        #endregion
        #region /// \name Building using MessageBoxElementData

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string Show(List<MessageBoxElementData> labels, string title = "", List<MessageBoxElementData> buttons = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        ///
        /// \brief Shows.
        ///        A customizedMessageBox for using in the running phase (The algorithm implementation)
        ///        Allows formating of the labels and buttons.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 16/04/2018
        ///
        /// \param labels        (List&lt;MessageBoxElementData&gt;) - The labels.
        /// \param title         (Optional)  (string) - The title.
        /// \param buttons       (Optional)  (List&lt;MessageBoxElementData&gt;) - The buttons.
        /// \param imageIcon     (Optional)  (Icons) - The image icon.
        /// \param sizeToContent (Optional)  (bool) - true to size to content.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Show(List<MessageBoxElementData> labels, 
            string title = "", List<MessageBoxElementData> buttons = null,
            Icons imageIcon = Icons.Info,
            bool sizeToContent = false)
        {
            List<Control> messageBoxLabels = new List<Control>();
            List<Button> messageBoxButtons = new List<Button>();

            foreach (MessageBoxElementData data in labels)
            {
                messageBoxLabels.Add(SetLabel(data.label, data.font));
            }


            if (buttons == null)
            {
                messageBoxButtons.Add(SetButton("OK"));
            }
            else
            {
                foreach (MessageBoxElementData data in buttons)
                {
                    messageBoxButtons.Add(SetButton(data.label, data.font));
                }
            }
            return Show(messageBoxLabels, title, messageBoxButtons, imageIcon, sizeToContent);
        }
        #endregion
        #region /// \name Utility methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static List<Button> GenerateButtonsAccordingToEnum(MessageBoxButton messageBoxButton)
        ///
        /// \brief Generates the buttons according to enum.
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
        /// \param messageBoxButton  (MessageBoxButton) - The message box button.
        ///
        /// \return The buttons according to enum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static List<Button> GenerateButtonsAccordingToEnum(MessageBoxButton messageBoxButton)
        {
            List<Button> buttons = new List<Button>();
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:
                    buttons.Add(SetButton("OK"));
                    return buttons;
                case MessageBoxButton.OKCancel:
                    buttons.Add(SetButton("OK"));
                    buttons.Add(SetButton("Cancel"));
                    return buttons;
                case MessageBoxButton.YesNo:
                    buttons.Add(SetButton("Yes"));
                    buttons.Add(SetButton("No"));
                    return buttons;
                case MessageBoxButton.YesNoCancel:
                    buttons.Add(SetButton("Yes"));
                    buttons.Add(SetButton("No"));
                    buttons.Add(SetButton("Cancel"));
                    return buttons;
                default:
                    buttons.Add(SetButton("OK"));
                    return buttons;

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static MessageBoxResult TranslateResutlToEnum(string result)
        ///
        /// \brief Translate result to enum.
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
        /// \param result  (string) - The result.
        ///
        /// \return A MessageBoxResult.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static MessageBoxResult TranslateResultToEnum(string result)
        {
            switch (result)
            {
                case "OK": return MessageBoxResult.OK;
                case "Cancel": return MessageBoxResult.Cancel;
                case "Yes": return MessageBoxResult.Yes;
                case "No": return MessageBoxResult.No;
                default: return MessageBoxResult.None;
            }
        }
#endregion
        #region /// \name Components creating
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static TextBox SetLabel(string text = "", Font font = null, Brush foreColor = null, Brush backColor = null)
        ///
        /// \brief Sets a label.
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
        /// \param text      (Optional)  (string) - The text.
        /// \param font      (Optional)  (Font) - The font.
        /// \param foreColor (Optional)  (Brush) - The foreground color.
        /// \param backColor (Optional)  (Brush) - The back color.
        ///
        /// \return A TextBox.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static TextBox SetLabel(string text = "", Font font = null)
        {
            TextBox textBox = new TextBox();
            textBox.Text = text;
            textBox.Margin = new Thickness(1);
            textBox.Padding = new Thickness(2);
            textBox.BorderThickness = new Thickness(1);
            textBox.BorderBrush = Brushes.White;
            textBox.Effect = null;
            if (font == null)
            {
                font = new Font("Calibbri", 16, FontStyles.Normal, FontWeights.Normal, new Thickness(0), HorizontalAlignment.Left, Brushes.Black, Brushes.White, TextWrapping.Wrap);
            }

            textBox.Effect = null;
            font.SetFont(textBox);
            //font.SetFontAndDimentions(textBlock, (string)textBlock.Text);
            return textBox;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Button SetButton(string text = "", int width = 0, int height = 30, Font font = null, Brush foreColor = null, Brush backColor = null)
        ///
        /// \brief Sets a button.
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
        /// \param text      (Optional)  (string) - The text.
        /// \param width     (Optional)  (int) - The width.
        /// \param height    (Optional)  (int) - The height.
        /// \param font      (Optional)  (Font) - The font.
        /// \param foreColor (Optional)  (Brush) - The foreground color.
        /// \param backColor (Optional)  (Brush) - The back color.
        ///
        /// \return A Button.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Button SetButton(string text = "", Font font = null, int width = 0, int height = 30)
        {
            Button button = new Button();
            button.Content = text;
            if (font == null)
            {
                font = new Font("Calibbri", 16, FontStyles.Normal, FontWeights.Normal, new Thickness(0), HorizontalAlignment.Left, Brushes.Black, Brushes.White, TextWrapping.NoWrap);
            }
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            if (width != 0) button.Width = width;
            if (height != 0) button.Height = height;
            button.Margin = new Thickness(5);
            return button;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Image SetImage(Icons imageIcon, int width = 45, int height = 45)
        ///
        /// \brief Sets an image.
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
        /// \param imageIcon  (Icons) - The image icon.
        /// \param width     (Optional)  (int) - The width.
        /// \param height    (Optional)  (int) - The height.
        ///
        /// \return An Image.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Image SetImage(Icons imageIcon, int width = 45, int height = 45)
        {
            Image image = new Image();
            //image.Source = new BitmapImage(new Uri(imagePath), );
            image.Width = width;
            image.Height = height;
            string imagePath = GetIconImagePath(imageIcon); //ClassFactory.IconsPath() + "info.png"; // imageIcon;
            if (imagePath != null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath);
                bitmapImage.DecodePixelWidth = width;
                bitmapImage.DecodePixelHeight = height;
                bitmapImage.EndInit();
                image.Source = bitmapImage;
            }
            else
            {
                image.Height = 0;
                image.Width = 0;
            }
            return image;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static string GetIconImagePath(Icons imageIcon)
        ///
        /// \brief Gets icon image path.
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
        /// \exception Exception Thrown when an exception error condition occurs.
        ///
        /// \param imageIcon  (Icons) - The image icon.
        ///
        /// \return The icon image path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string GetIconImagePath(Icons imageIcon)
        {
            string iconName = TypesUtility.GetKeyToString(imageIcon);
            string iconsPath = ClassFactory.IconsPath();
            foreach (string fileExtention in new List<string>() { ".png" })
            {
                string fullFileName = iconsPath + iconName + fileExtention;
                if (File.Exists(fullFileName)) return fullFileName;
            }
            throw new Exception("The icon " + iconName + " does not exist in path " + iconsPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Click(object sender, EventArgs e)
        ///
        /// \brief Event handler. Called by Button for click events.
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
        /// \param e       (EventArgs) - Event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Click(object sender, EventArgs e)
        {
            Result = (string)((Button)sender).Content;
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static SyntaxHighlight SyntaxHighlightBlock(string text, HashSet<string> classes, Font font = null)
        ///
        /// \brief Syntax highlight block.
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
        /// \param text     (string) - The text.
        /// \param classes  (HashSet&lt;string&gt;) - The classes.
        /// \param font    (Optional)  (Font) - The font.
        ///
        /// \return A SyntaxHighlight.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static SyntaxHighlight SyntaxHighlightBlock(string text, HashSet<string> classes, Font font = null)
        {
            SyntaxHighlight syntaxHighlight = new SyntaxHighlight(text, classes);
            font.SetFont(syntaxHighlight);
            return syntaxHighlight;
        }
        public static DataGrid SetDataGrid(List<List<string>> data)
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.ItemsSource = data;
            return dataGrid;
        }
#endregion
    }
}

        