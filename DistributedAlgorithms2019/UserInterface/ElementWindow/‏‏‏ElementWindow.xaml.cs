////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\‏‏‏ElementWindow.xaml.cs
///
///\brief   Implements the element window.xaml class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    #region /// \name ElementWindowPrms class
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \enum InputFieldsType
    ///
    /// \brief Values that represent input fields types.
    ///        This enum is used to pass to the ElementWindow and it's inheritors the type
    ///        of input control that is needed to be generated for the changing the values of the attribute.
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum InputFieldsType { Null, ComboBox, TextBox, AddRemovePanel, Button }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \enum InputWindows
    ///
    /// \brief Values that represent input windows.
    ///        This enum is used in order to pass to the ElementWindowPrms delegate the type of the window
    ///        that requested the specifications for the attribute presentations.
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum InputWindows { InputWindow, DebugWindow, AddAlgorithmWindow, ConfigWindow }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \fn public delegate ElementWindowPrms ElementWindowPrmsDelegate(Attribute attribute, dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable);
    ///
    /// \brief Input field delegate. This delegate is used to declare and hold a method that decides
    ///        The control and values of the controls in the ElementWindow.
    ///
    /// \author Ilanh
    /// \date 03/05/2017
    ///
    /// \param attribute       (Attribute) - The attribute.
    /// \param key             (dynamic) - The key.
    /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
    /// \param dictionary      (ElementDictionaries) - The dictionary.
    /// \param inputWindow     (InputWindows) - The type of the input window.
    /// \param windowEdittable (bool) - true if the window editable.
    ///
    /// \return The InputFieldPrms.
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public delegate ElementWindowPrms ElementWindowPrmsDelegate(Attribute attribute, dynamic key,
        NetworkElement mainNetworkElement,
        NetworkElement.ElementDictionaries mainDictionary,
        NetworkElement.ElementDictionaries dictionary,
        InputWindows inputWindow,
        bool windowEdittable);

    public delegate Attribute GridAddDelegate();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class NewValueControlPrms
    ///
    /// \brief A new value control prms.
    ///        This class is used by the network element to define the way the new value control
    ///        will be presented
    ///
    /// \par Description.
    ///      Parameters for setting the new value control
    ///
    /// \par Usage Notes.
    ///      Used in the following cases;
    ///      -# When building the window
    ///      -# When updating a new value control during window operations
    ///
    /// \author Ilanh
    /// \date 28/11/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class NewValueControlPrms
    {
        /// \brief (InputFieldsType) - Type of the input field (For the New Value Control).
        public InputFieldsType inputFieldType = InputFieldsType.Null;

        /// \brief (bool) - true to enable the new value control, false to disable.
        public bool enable = true;

        /// \brief (string[]) - Options for a combo box new value control.
        public string[] options = { };


        /// \brief (string) - The value in the new value control.
        private string value;

        /// \brief (RoutedEventHandler) - The button click event handler.
        public RoutedEventHandler button_click = null;

        /// \brief  (RoutedEventHandler) - The grid add button click.
        public GridAddDelegate addAttributeMethod = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public string Value
        ///
        /// \brief Gets or sets the value.
        ///
        /// \return The value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Value { get => value; set => this.value = value; }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ElementWindowPrms
    ///
    /// \brief An input field prms.
    ///
    /// \par Description.
    ///      This class holds the specifications of the fields in ElementWindow that
    ///      will be used for input, view and change the attribute in element input windows.
    ///
    /// \par Usage Notes.
    ///      There are 3 fields for each attribute in the ElementWindow:
    ///      -#   The Header text box (which usually present the key of the attribute in the dictionary)
    ///      -#   The existing value text box
    ///      -#   The input control (can be TextBox, AddRemoveGrid, ComboBox) for changing the attribute
    ///           value.
    ///
    /// \author Ilanh
    /// \date 03/05/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ElementWindowPrms
    {
        /// \brief  (NewValueControlPrms) - The new value control prms.
        public NewValueControlPrms newValueControlPrms = new NewValueControlPrms();

        /// \brief (string) - The existing value text box text.
        public string existingValueText = "";

        /// \brief (string) - The key text (to be put in the header of the item).
        public string keyText = "";

        /// \brief  (string) - The type string.
        public string typeString = "";        
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ElementWindow
    ///
    /// \brief Form for viewing the element.
    ///
    /// \par Description.
    ///      this class is the base class for all the input windows on network elements
    ///      It allows showing, changing adding and removing attributes.
    ///
    /// \par Usage Notes.
    ///      The following are the usage notes.
    ///
    /// \author Ilanh
    /// \date 03/05/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class ElementWindow : Window, IScanConsumer
    {
        #region /// \name ControlsAttributeLink

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \class ControlsAttributeLink
        ///
        /// \brief The controls attribute link.
        ///
        /// \par Description
        ///      This class holds all the data about an attribute presented in the window.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 11/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class ControlsAttributeLink
        {
            
            /// \brief (ItemsControl) - The tree view item.
            public ItemsControl item;

            public TextBox typeTextBox;
            
            /// \brief (TextBox) - The existing value control.
            public TextBox existingValueTextBox;

            
            /// \brief (FrameworkElement) - The new value control.
            public FrameworkElement newValueControl;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (Attribute) - The parent attribute (for example if the attribute is in a list this
            ///        parameter is the list attribute).
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public Attribute parentAttribute;

            /// \brief (int) - The index of the attribute in perant attribute.
            public int indexInPerant;

            /// \brief (UpdateStatuses) - The update status (If this item was added remove updated).
            public UpdateStatuses updateStatus;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief The prev update status is used to restore the update status in De-Remove operation
            ///        (The remove operation is reversable this variable is used to set the previouse status in case
            ///        there was a remove that was reversed)
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public UpdateStatuses prevUpdateStatus;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (bool) - true if existing value was updated. When in running mode and the running was
            ///        advanced the existing value should changed according to the new values (after the advance)
            ///        This variable indicates that the existing value was changed for the method
            ///        UpdateExistingValueUpdated which set the font of the changed attributes to bold.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public bool existingValueWasUpdated;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (bool) - true to was scanned in the last scan. When in running mode some fields
            ///        might be deleted (because of the running). In the process of updating the
            ///        window these fields has to be deleted. The deletion is done when the complex
            ///        item is closed. At the beginning of the updating process this field is set to false
            ///        for all the attributes. If the attribute still exist it is set to true while
            ///        scanned so that all the attributes that were not scanned can be deleted.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public bool wasScannedInTheLastScan;    
            
            /// \brief (Attribute) - The attribute that the link is connected to.
            public Attribute attribute;

            /// \brief (ElementDictionaries) - Dictionary of attributes.
            public NetworkElement.ElementDictionaries attributeDictionary;

            /// \brief (ElementDictionaries) - Dictionary of mains.
            public NetworkElement.ElementDictionaries mainDictionary;

            /// \brief  (NetworkElement) - The main network element.
            public NetworkElement mainNetworkElement;

            /// \brief (dynamic) - The key of the attribute in the dictionary or list.
            public dynamic key;

            
            /// \brief (AttributeCategory) - Category the attribute belongs to.
            public Attribute.AttributeCategory attributeCategory;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (string) - The existing new value. When changing the value of a field, after the
            ///        user left the field, the is checked. If this check failes there is a need
            ///        to recover the previouse value the recovery is done from this member.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public string existingNewValue;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (NetworkElement) - The hosting network element. (Passed to the EndInputOperation to
            ///        enable checks on the network element when changing an attribute)
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public NetworkElement hostingNetworkElement;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (ControlsAttributeLink) - The selected child link. The selected Attribute of an
            ///        AttributeDictionary and AttributeList is marked by a border width of 2 when the combo box of
            ///        which is used to select an attribute is changed the previouse selected item has to be chosen
            ///        this parameter keeps the previous index in order to do so.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public ControlsAttributeLink selectedChildLink;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// \brief (GridAddDelegate) - The grid add delegate.
            ///        This delegate hold a method that will be used to add attribute when the Grid Add button
            ///        is pressed.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public GridAddDelegate gridAddMethod = null;
        }
        #endregion
        #region /// \name Enums and Selecting arrays

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ScanProcess
        ///
        /// \brief Values that represent scan process.
        ///        The scan process fills the window (or adds to the window) a complex item
        ///        for example on init fills the window with the NetworkElements that are represented by the
        ///        window. This enum keeps the type of scan process been processed.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected enum ScanProcess { InitBuild, UpdateFromNetworkElement, AdditionOfNetworkElement, AdditionOfFields }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum UpdateStatuses
        ///
        /// \brief Values that represent update statuses.
        ///        The update status is the status of an attribute in the changing process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum UpdateStatuses { NotUpdated, Updated, Added, Removed }

        /// \brief (Brush[]) - List of colors of the update status.
        protected Brush[] UpdateStatusColors = { Brushes.Black, Brushes.Blue, Brushes.Green, Brushes.Red };
        #endregion
        #region /// \name Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (InputWindows) - The input window. This member is used when asking the attribute for
        ///        it's input control specifications to support the case that the attributes wants to be
        ///        presented in different ways in different windows. (for example if an attribute should be
        ///        presented when in design mode differently when the algorithm is running)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected InputWindows inputWindow;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Dictionary&lt;ItemsControl,ControlsAttributeLink&gt;) - The controls attribute links.
        ///        This dictionary is holding al the data neede to present and manipulate the presentation.
        ///        This dictionary is regenarated each time the presentation is refreshed (and actually rebuilded)
        ///        The keys to the dictionary is the item in the TreeView which is placed in the left column.
        ///        The values of the dictionary are objects from the class ControlAttributeLink.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Dictionary<ItemsControl, ControlsAttributeLink> controlsAttributeLinks = new Dictionary<ItemsControl, ControlsAttributeLink>();

        /// \brief (List&lt;NetworkElement&gt;) - The network elements that this window presents.
        public List<NetworkElement> networkElements;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to activate end input operation. End input operation is a static method
        ///        assigned to each attribute for operations needed after a value of an attribute was changed in
        ///        the input field (for example checking if the entered value is leagle).
        ///        This attribute determins if end imput operation will be activated or not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool activateEndInputOperation;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ControlsAttributeLink) - The current complex item. When inserting an attribute to
        ///        the presentation this member holds the complex item (for example the item of the attribute
        ///        list that holds the attribute that it's presentation currently been generated).
        ///        This member is used to fill the parentAttribute of the ControlAttributeLink of the attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink currentComplexItem;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (NetworkElement) - The current network element. This variable is used when scanning
        ///        the network element to fill in a ControllAttributeLink the hostingNetworkElement. This
        ///        variable is needed when activating the EndInputOperation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected NetworkElement currentNetworkElement;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ScanProcess) - The scan process. This variable is used to indicate the type of the
        ///        current scanning process (See the ScanProcess Enum for detailes)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ScanProcess scanProcess;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (int) - Zero-based index of the previous selected.
        ///        When updating a network element all the input fields and links are deleted.
        ///        in order to restore the index of the como boxes that where before the update,
        ///        they are alll saved in this member variable.
        ///        The key is the attribute that is because an existing attribute is not replaces
        ///        only updated.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected Dictionary<Attribute, int> prevSelectedIndexes = new Dictionary<Attribute, int>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to dictionary key string only.
        ///        This parameter is used to force the CreateAttribute dialog to accept keys from
        ///        string type only. This parameter is true for the AddAlgorithmWindow.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool dictionaryKeyStringOnly;

        /// \brief  (NetworkElement) - The network element scanned.
        protected NetworkElement networkElementScanned = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Prms) - The attribute window prms.
        ///        Variable for changing the parameters of create attribute dialog.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public CreateAttributeDialog.Prms attributeCreationPrms;
        #endregion
        #region /// \name border width (changes when an item in a list selected)

        /// \brief (int) - Width of the border when not selected.
        protected int borderWidth = 1;

        /// \brief (int) - The selected border width.
        protected int selectedBorderWidth = 2;
        #endregion
        #region /// \name Graphic constants

        /// \brief (double) - Height of the main dock panel maximum.
        protected double dockPanelMaxHeight = 700;
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ElementWindow(List<NetworkElement> networkElements, InputWindows inputWindow, bool activateEndInputOperation = false, bool presentExistingValues = true, bool presentNewValues = true, bool dictionaryKeyStringOnly = false)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      In order to create the class Activate the constructor and then set to false
        ///      the column you don't want to be presented
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param networkElements           (List&lt;NetworkElement&gt;) - The network elements.
        /// \param inputWindow               (InputWindows) - The input window type.
        /// \param activateEndInputOperation (Optional)  (bool) - true to activate end input operation.
        /// \param presentExistingValues     (Optional)  (bool) - true to present existing values.
        /// \param presentNewValues          (Optional)  (bool) - true to present new values.
        /// \param dictionaryKeyStringOnly   (Optional)  (bool) - true to dictionary key string only.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ElementWindow(List<NetworkElement> networkElements,
            InputWindows inputWindow,
            bool activateEndInputOperation = true,
            bool dictionaryKeyStringOnly = false)
        {
            InitializeComponent();
            this.networkElements = networkElements;
            this.activateEndInputOperation = activateEndInputOperation;
            this.inputWindow = inputWindow;
            this.dictionaryKeyStringOnly = dictionaryKeyStringOnly;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public bool PresentExistingValues
        ///
        /// \brief Sets a value indicating whether to present existing values.
        ///
        /// \return True if present existing values, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool PresentExistingValues
        {
            set
            {
                if (value)
                {
                    StackPanel_AttributeExistingValues.Visibility = Visibility.Visible;
                    GridSplitter_AfterExistingValueColumnSeparator.Visibility = Visibility.Visible;
                }
                else
                {
                    StackPanel_AttributeExistingValues.Visibility = Visibility.Collapsed;
                    GridSplitter_AfterExistingValueColumnSeparator.Visibility = Visibility.Collapsed;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public bool PresentNewValues
        ///
        /// \brief Sets a value indicating whether to present new values.
        ///
        /// \return True if present new values, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool PresentNewValues
        {
            set
            {
                if (value)
                {
                    StackPanel_AttributeNewValues.Visibility = Visibility.Visible;
                    GridSplitter_AfterNewValueColumnSeparator.Visibility = Visibility.Visible;
                }
                else
                {
                    StackPanel_AttributeNewValues.Visibility = Visibility.Collapsed;
                    GridSplitter_AfterNewValueColumnSeparator.Visibility = Visibility.Collapsed;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public bool PresentTypes
        ///
        /// \brief Sets a value indicating whether to present types.
        ///
        /// \return True if present types, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool PresentTypes
        {
            set
            {
                if (value)
                {
                    StackPanel_AttributeTypes.Visibility = Visibility.Visible;
                    GridSplitter_AfterTypeColumnSeparator.Visibility = Visibility.Visible;
                }
                else
                {
                    StackPanel_AttributeTypes.Visibility = Visibility.Collapsed;
                    GridSplitter_AfterTypeColumnSeparator.Visibility = Visibility.Collapsed;
                }
            }
        }


        #endregion
        #region /// \name Window building

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void InitWindow()
        ///
        /// \brief Init window.
        ///
        /// \par Description
        ///      This method should be activated by there constructor.
        ///      The separation between the constructor and this method allows the inherited
        ///      classes to update members before drawing the window.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void InitWindow()
        {
            FillTreeView(TreeView);
            FillWindowName();
            CreateButtons();
            MouseDown += Window_MouseDown;
            Loaded += Window_Loaded;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void CreateButtons()
        ///
        /// \brief Creates the buttons.
        ///
        /// \par Description
        ///      This method should be filled by the inheritted class.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes
        ///      For each button the inherritted class wants to generate call CreateButton method.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void CreateButtons()
        {           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void CreateButton(string name, string content, RoutedEventHandler clickFunc)
        ///
        /// \brief Creates a button.
        ///
        /// \par Description
        ///      This method is should be called by CreateButtons (overload) to create one button.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param name      (string) - The name.
        /// \param content   (string) - The content.
        /// \param clickFunc (RoutedEventHandler) - The click function.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void CreateButton(string name, string content, RoutedEventHandler clickFunc)
        {
            Button button = new Button();
            button.Name = name;
            button.Content = content;
            button.Click += clickFunc;
            StackPanel_Buttons.Children.Add(button);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void FillWindowName()
        ///
        /// \brief Fill window name.
        ///
        /// \par Description
        ///      This method is sed to fill the window name
        ///      This method is virtual so that the inherritted window can change the window header.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void FillWindowName()
        {
            string windowName = "";
            for (int idx = 0; idx < networkElements.Count; idx++)
            {
                windowName += ToString().Replace("DistributedAlgorithms.", "") + " : " +
                  networkElements[idx].GetType().ToString().Replace("DistributedAlgorithms.", "") + " : " +
                  networkElements[idx].ToString();
                if (idx < networkElements.Count - 1)
                {
                    windowName += "\n";
                }
            }
            Label_Name.Content = windowName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Window_MouseDown(object sender, MouseButtonEventArgs e)
        ///
        /// \brief Event handler. Called by Window for mouse down events.
        ///
        /// \par Description.
        ///      Move the window (needed because the window has no title bar.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (MouseButtonEventArgs) - Mouse button event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {
                ;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void Window_Loaded(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Window for loaded events.
        ///
        /// \par Description.
        ///      Set the DockPanel_Main max height (that will set the window's max height
        ///      because it is FitToContents)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DocPanel_Main.ActualHeight > dockPanelMaxHeight)
            {
                DocPanel_Main.MaxHeight = dockPanelMaxHeight;
            }
            else
            {
                DocPanel_Main.MaxHeight = dockPanelMaxHeight;
            }

        }
        #endregion
        #region /// \name NetworkElements scanning

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void FillTreeView(ItemsControl item)
        ///
        /// \brief Fill tree view.
        ///
        /// \par Description
        ///      This method generates the presentation when the window is initiated.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void FillTreeView(ItemsControl item)
        {
            // The scanning type is when the window is initiated
            scanProcess = ScanProcess.InitBuild;

            // Create the link for the tree
            ControlsAttributeLink treeLink = CreateNewLink(null, "", 
                NetworkElement.ElementDictionaries.None, 
                NetworkElement.ElementDictionaries.None, 
                TreeView, null, null, null, null);

            // Create the tree for each network element
            foreach (NetworkElement networkElement in networkElements)
            {
                // Set the NetworkElementScanned for inserting to the link
                networkElementScanned = networkElement;

                // Create the item for the NetworkElement
                currentNetworkElement = networkElement;

                // The tree link is inserted to the currentComplexItem in order to make the NetworkElement item it's chiled
                currentComplexItem = treeLink;

                // Create an attribute with the same permition as the networkElement (Otherwise the permissions
                // of the attribute will be setted to the networkElement)
                Attribute attribute = networkElement.CreateWrappingAttribute();
                attribute.Editable = false;

                // The following method decides on the way that the NetworkElement will be presented
                // (Because the attribute of the NetworkElement is created in this method - there is a 
                // possibility to change the presentation prms by inheritting from MainElementPrms)
                attribute.ElementWindowPrmsMethod = MainElementPrms;

                currentComplexItem = CreateNewItem(attribute , networkElement.ToString(), NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None);
                networkElement.ScanAndReport(this, 
                    NetworkElement.ElementDictionaries.None, 
                    NetworkElement.ElementDictionaries.None);
            }
            RearrangeFields(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual ElementWindowPrms MainElementPrms(Attribute attribute, dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Main element prms.
        ///
        /// \par Description.
        ///      When generating the tree the main network elements are put in an attribute.
        ///      This method is assigned to the attribute to determin the presentation parameters
        ///      for these network elements.
        ///      The method is virtual so each window can define the behavioure.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param attribute       (Attribute) - The attribute.
        /// \param key             (dynamic) - The key.
        /// \param mainDictionary   (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        /// \param inputWindow     (InputWindows) - The input window.
        /// \param windowEdittable (bool) - true if window edittable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual ElementWindowPrms MainElementPrms(Attribute attribute, dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable)
        {
            return new ElementWindowPrms();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void UpdateExistingValues()
        ///
        /// \brief Updates the existing values.
        ///
        /// \par Description
        ///      -  This method is ment for updating the presentation in case that the attributes
        ///         were changed outside of the window
        ///      -  In this case the existing value field of the attributes that were changed   
        ///         should change and it's font should be bold
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateExistingValues()
        {
            // Set all the indicators for existing value was changed to false so that only
            // fields that were changed in the last step will be shown in bold
            foreach (var entry in controlsAttributeLinks)
            {
                entry.Value.existingValueWasUpdated = false;
                entry.Value.wasScannedInTheLastScan = false;
            }

            // Scan all the NetworkElements
            scanProcess = ScanProcess.UpdateFromNetworkElement;

            foreach (TreeViewItem networkElementItem in TreeView.Items)
            {
                currentComplexItem = controlsAttributeLinks[networkElementItem];

                // Set the NetworkElementScanned for inserting to the link
                networkElementScanned = currentComplexItem.attribute.Value;

                currentComplexItem.attribute.Value.ScanAndReport(this, NetworkElement.ElementDictionaries.None, NetworkElement.ElementDictionaries.None);
            }
            RearrangeFields(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual bool ScanCondition(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Scans a condition.
        ///
        /// \par Description
        ///      This method specify the condition for presenting an attribute in the tree
        ///      This is a virtual method the inheritted windows can change the conditions.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - The dictionary key.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual bool ScanCondition(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void OpenComplexAttribute(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Opens complex attribute.
        ///
        /// \par Description
        ///      Handeling the beginning of a complex item.
        ///      The handeling is the same as for new regular attribute + setting the currentComplexItem.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param name (string) - The name.
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void OpenComplexAttribute(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            //MessageRouter.ReportMessage("Open ; ", "", TypesUtility.GetKeyToString(key));
            ControlsAttributeLink link = AttributeHandling(attribute, key, mainDictionary, dictionary);
            if (link != null)
            {
                currentComplexItem = link;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void CloseComplexAttribute(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Closes complex attribute.
        ///
        /// \par Description
        ///      The end of scanning of a complex attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param name (string) - The name.
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void CloseComplexAttribute(dynamic key, 
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            //MessageRouter.ReportMessage("Close : ", "", TypesUtility.GetKeyToString(key));
            if (currentComplexItem.item.GetType().Equals(typeof(TreeViewItem)))
            {
                //MessageRouter.ReportMessage("Parent ; ", "", TypesUtility.GetKeyToString(((TextBox)((TreeViewItem)currentComplexItem.item.Parent).Header).Text));
            }
            ArrangeItemsList(currentComplexItem.item);

            // If the current complex item is the tree - do nothing as it has no parent or presentation
            if (currentComplexItem.item != TreeView)
            {
                HandleExpandedWhenClosingComplexItem();

                // If the scanning is a scanning while the program is in running phase and the 
                // current complex attribute was changed (or one of it's chiled attribute was changed)
                // Set the parent attribute to changed
                if (currentComplexItem.existingValueWasUpdated)
                {
                    controlsAttributeLinks[(ItemsControl)currentComplexItem.item.Parent].existingValueWasUpdated = true;
                }

                // Set the parent the current complex item
                currentComplexItem = controlsAttributeLinks[(ItemsControl)currentComplexItem.item.Parent];
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ArrangeItemsList(ItemsControl item)
        ///
        /// \brief Arrange items list.
        ///
        /// \par Description
        ///      This method is operated when a complex item is closed during scanning process.
        ///      When the complex item is closed the controlsAttributeLinks dictionary is updated
        ///      but in the tree view the items collection of item of the complex attribute are not
        ///      updated.
        ///      The order of the items in the complex item will determin the order of the attribute
        ///      in the complex attribute
        ///      In addition we have to remove from the controlAttributeLinks all the items that
        ///      are not in the network element scanned and there update status is NotUpdated.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ArrangeItemsList(ItemsControl item)
        {
            /*
             * Initialize status:
             * The items list of the item is not updated
             * The controlsAttributeLinks is updated with the action that has to be done:
             * - Controled that where scanned (That meens that they are in the NetworkElement) will be inserted in the init list
             * - Controls that where not scanned but has update status != NotUpdated will be inserted according to theire location
             * - Controls that where not scanned and not updated that will be removed
             */


            /*
             * The Creation of the new items list is done in the following way
             * Create a list of all the items that where scanned
             * Create a list of all the items that where not scanned but cannot be removed because of update status
             * Order the list of items that where scanned or added.
             * insert the items of this list as chiled items of the main item
             * insert the updated items in the location that they have in the link 
             */

            /*
             * Create a list of links that represent scanned items to insert and order it
             */
            List<ControlsAttributeLink> itemsToInsert = controlsAttributeLinks.Where(
                entry => entry.Value.wasScannedInTheLastScan == true && item.Items.IndexOf(entry.Key) != -1)
                .Select(entry => entry.Value).ToList();
            itemsToInsert = itemsToInsert.OrderBy(entry => entry.indexInPerant).ToList();

            /*
             * Create a list of all the updated but not scanned items
             */
            List<ControlsAttributeLink> updatedButNotScannedItems = (List<ControlsAttributeLink>)(controlsAttributeLinks.Where(
                entry => entry.Value.wasScannedInTheLastScan == false &&
                    entry.Value.updateStatus != UpdateStatuses.NotUpdated &&
                    item.Items.IndexOf(entry.Value.item) != -1)
                    .Select(entry => entry.Value)
                    .OrderBy(link => link.indexInPerant)
                    .ToList());

            /*
             * Remove from the link dictionary the items that where not updated and not scanned
             */
            controlsAttributeLinks = controlsAttributeLinks.Where(
                entry => ConditionToStayInTheLinks(entry.Value, item))
                                 .ToDictionary(entry => entry.Key, entry => entry.Value);

            /*
             * Insert the scanned or added items as chiled items to the item
             */
            item.Items.Clear();
            itemsToInsert.ForEach(link => item.Items.Add(link.item));

            /*
             * insert the updated but not scaned items in the place that they have in the record
             * (before the last scan)
             */
            updatedButNotScannedItems.ForEach(link => item.Items.Insert(link.indexInPerant, link.item));

            /*
             * Update the links with the index from the item
             */
            UpdateIndexInParent(item);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected bool ConditionToStayInTheLinks(ControlsAttributeLink link, ItemsControl item)
        ///
        /// \brief Condition to stay in the links.
        ///
        /// \par Description
        ///      The condition to stay in the controlsAttributeLink
        ///      (Used in order to remove items from the dictionary by ArrangeItemsList.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param link (ControlsAttributeLink) - The link.
        /// \param item (ItemsControl) - The item.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool ConditionToStayInTheLinks(ControlsAttributeLink link, ItemsControl item)
        {
            /*
             * The condition to be removed from the list is:
             * The item in the link is a chiled of the item
             * The link item was not scanned in the last scan
             * The link item update status is NotUpdated
             */
            bool result = true;
            if (item.Items.IndexOf(link.item) != -1)
                if (link.wasScannedInTheLastScan == false)
                    if (link.updateStatus == UpdateStatuses.NotUpdated)
                        result = false;
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateIndexInParent(ItemsControl item)
        ///
        /// \brief Updates the index in parent described by item.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateIndexInParent(ItemsControl item)
        {
            for (int idx = 0; idx < item.Items.Count; idx++)
            {
                controlsAttributeLinks[(TreeViewItem)item.Items[idx]].indexInPerant = idx;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void AttributeReport(dynamic key, Attribute attribute, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Attribute report.
        ///
        /// \par Description
        ///      Action to be done when a simple attribute is reported.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param name (string) - The name.
        ///
        /// \param key            (dynamic) - The key.
        /// \param attribute      (Attribute) - The attribute.
        /// \param mainDictionary (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void AttributeReport(dynamic key,
            Attribute attribute,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            AttributeHandling(attribute, key, mainDictionary, dictionary);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected ControlsAttributeLink AttributeHandling(Attribute attribute, dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary)
        ///
        /// \brief Attribute handling.
        ///
        /// \par Description
        ///      Handles a attribute that was scanned (either complex item or a simple item)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param attribute      (Attribute) - The attribute.
        /// \param key            (dynamic) - The key.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        ///
        /// \return A ControlsAttributeLink.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink AttributeHandling(Attribute attribute, 
            dynamic key, 
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary)
        {
            ControlsAttributeLink controlsAttributeLink;

            // Try to find the link associated with the attribute
            controlsAttributeLink = controlsAttributeLinks.FirstOrDefault(entry => entry.Value.attribute == attribute).Value;

            // If the link does not exist create a new one (+ the tree view item and the controls)
            if (controlsAttributeLink == null)
            {
                controlsAttributeLink = CreateNewItem(attribute, key, mainDictionary, dictionary);
            }

            // If the link exist Update the existing controls
            else
            {
                UpdateAttribute(attribute.GetValueToString(), controlsAttributeLink);
            }

            controlsAttributeLink.wasScannedInTheLastScan = true;

            // If the existing value control of the attribute was changed - set the flag to the current complex item
            // so that it will show in bold indicating that one of it's childs was changed
            if (controlsAttributeLink.existingValueWasUpdated)
            {
                currentComplexItem.existingValueWasUpdated = true;
            }
            return controlsAttributeLink;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateAttribute(string newValue, ControlsAttributeLink controlsAttributeLink)
        ///
        /// \brief Updates the attribute.
        ///
        /// \par Description
        ///      Update an existing attribute while in scanning.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param newValue              (string) - The new value.
        /// \param controlsAttributeLink (ControlsAttributeLink) - The controls attribute link.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateAttribute(string newValue, ControlsAttributeLink controlsAttributeLink)
        {

            // If the existing value (The current value of the attribute) was changed:
            if (controlsAttributeLink.existingValueTextBox.Text != newValue)
            {
                // Change the value and reise the existingValueWasUpdated flag
                controlsAttributeLink.existingValueTextBox.Text = newValue;
                controlsAttributeLink.existingValueWasUpdated = true;

                // If the update status of the attribute is NotUpdated change the newValueControl
                // If the value was update the new value control stays the same 
                if (controlsAttributeLink.updateStatus == UpdateStatuses.NotUpdated)
                {
                    if (controlsAttributeLink.newValueControl.GetType().Equals(typeof(ComboBox)))
                    {
                        ((ComboBox)controlsAttributeLink.newValueControl).SelectedItem = newValue;
                    }
                    else if (controlsAttributeLink.newValueControl.GetType().Equals(typeof(TextBox)))
                    {
                        ((TextBox)controlsAttributeLink.newValueControl).Text = newValue;
                    }
                }
            }
        }

        #endregion
        #region /// \name New Attribute presentation creation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected ControlsAttributeLink CreateNewItem(Attribute attribute, dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, int newItemLocation = -1)
        ///
        /// \brief Creates new item.
        ///
        /// \par Description
        ///      This is the main method for creating the presentation and data for a new attribute.
        ///      The following are the objects created:
        ///      -#   TreeViewItem
        ///      -#   TextBox - Tree view item header
        ///      -#   TextBox - Existing value
        ///      -#   Control (Grid, TextBox, ComboBox) - The new value control
        ///      -#   ControlAttributeLink.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param attribute       (Attribute) - The attribute.
        /// \param key             (dynamic) - The key.
        /// \param mainDictionary   (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        /// \param newItemLocation (Optional)  (int) - The new item location in the parent item.
        ///                        (Used when adding new attributes by the user)
        ///
        /// \return The new item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink CreateNewItem(Attribute attribute,
            dynamic key,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            int newItemLocation = -1)
        {
            // tree view item
            TreeViewItem item = new TreeViewItem();
            item.IsExpanded = true;
            
            // Controls for presenting the attribute and key
            ElementWindowPrms controlsPrms = attribute.GetElementWindowPrms(key,
                networkElementScanned,
                mainDictionary,
                dictionary,
                inputWindow,
                true);
            TextBox headerTextBox = (TextBox)CreateHeaderTextBox(controlsPrms);
            TextBox typeTextBox = (TextBox)CreateTypeTextBox(controlsPrms);
            TextBox existingTextBox = (TextBox)CreateExistingValueTextBox(controlsPrms);
            FrameworkElement newValueControl = CreateNewValueControl(controlsPrms.newValueControlPrms);

            // Assign the header to the item
            item.Header = headerTextBox;

            // In case we are not in a refreshing the window operation but it an addition of an attribute
            // by the user the insertion can be in the middle and not in the end
            if (newItemLocation == -1)
            {
                currentComplexItem.item.Items.Add(item);
            }
            else
            {
                currentComplexItem.item.Items.Insert(newItemLocation, item);
            }

            // Create the ControlAttributeLink
            return CreateNewLink(attribute,
                key,
                mainDictionary,
                dictionary,
                item,
                typeTextBox,
                existingTextBox,
                newValueControl,
                 controlsPrms.newValueControlPrms.addAttributeMethod);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected ControlsAttributeLink CreateNewLink(Attribute attribute, dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, ItemsControl item, TextBox typeTextBox, TextBox existingValueTextBox, FrameworkElement newValueControl, GridAddDelegate gridAddMethod)
        ///
        /// \brief Creates new link.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 20/12/2017
        ///
        /// \param attribute             (Attribute) - The attribute.
        /// \param key                   (dynamic) - The key.
        /// \param mainDictionary        (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary            (ElementDictionaries) - The dictionary.
        /// \param item                  (ItemsControl) - The item.
        /// \param typeTextBox           (TextBox) - The type control.
        /// \param existingValueTextBox  (TextBox) - The existing value control.
        /// \param newValueControl       (FrameworkElement) - The new value control.
        /// \param gridAddMethod         (GridAddDelegate) - The grid add method.
        ///
        /// \return The new new link.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink CreateNewLink(Attribute attribute,
            dynamic key,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            ItemsControl item,
            TextBox typeTextBox,
            TextBox existingValueTextBox,
            FrameworkElement newValueControl,
            GridAddDelegate gridAddMethod)
        {
            ControlsAttributeLink newLink = new ControlsAttributeLink();
            newLink.attribute = attribute;
            newLink.key = key;
            newLink.attributeDictionary = dictionary;
            newLink.mainDictionary = mainDictionary;
            newLink.mainNetworkElement = networkElementScanned;
            newLink.typeTextBox = typeTextBox;
            newLink.existingValueTextBox = existingValueTextBox;
            newLink.newValueControl = newValueControl;
            newLink.updateStatus = UpdateStatuses.NotUpdated;
            newLink.existingValueWasUpdated = true;
            newLink.item = item;
            newLink.attributeCategory = Attribute.GetValueCategory(attribute);
            if (currentComplexItem != null)
            {
                newLink.parentAttribute = (Attribute)currentComplexItem.attribute;
                newLink.hostingNetworkElement = currentNetworkElement;
            }
            if (newLink.existingValueTextBox != null)
            {
                newLink.existingNewValue = existingValueTextBox.Text;
            }
            if (gridAddMethod != null)
            {
                newLink.gridAddMethod = gridAddMethod;
            }
            controlsAttributeLinks.Add(item, newLink);
            return newLink;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateHeaderTextBox(ElementWindowPrms controlsPrms)
        ///
        /// \brief Creates header text box.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param dictionary (ElementDictionaries) - The dictionary.
        ///
        /// \param controlsPrms (dynamic) - The key.
        ///
        /// \return The new header text box.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateHeaderTextBox(ElementWindowPrms controlsPrms)
        {
            // create the name field            
            return CreateTextBox(controlsPrms.keyText, false, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateTypeTextBox(ElementWindowPrms controlsPrms)
        ///
        /// \brief Creates type text box.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 31/10/2017
        ///
        /// \param controlsPrms  (ElementWindowPrms) - The controls prms.
        ///
        /// \return The new type text box.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateTypeTextBox(ElementWindowPrms controlsPrms)
        {
            // create the name field            
            return CreateTextBox(controlsPrms.typeString, false, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateExistingValueTextBox(ElementWindowPrms controlsPrms)
        ///
        /// \brief Creates existing value text box.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param controlsPrms (Attribute) - The attribute.
        ///
        /// \return The new existing value text box.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateExistingValueTextBox(ElementWindowPrms controlsPrms)
        {
            return CreateTextBox(controlsPrms.existingValueText, false, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateNewValueControl(ElementWindowPrms controlsPrms)
        ///
        /// \brief Creates new value control.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param key (dynamic) - The key.
        ///
        /// \param controlsPrms (Attribute) - The attribute.
        ///
        /// \return The new new value control.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateNewValueControl(NewValueControlPrms controlsPrms)
        {
            switch (controlsPrms.inputFieldType)
            {
                case InputFieldsType.AddRemovePanel:
                    return CreateAddRemoveGrid(controlsPrms);
                case InputFieldsType.Button:
                    return CreateButton(controlsPrms);
                case InputFieldsType.ComboBox:
                    return CreateComboBox(controlsPrms);
                case InputFieldsType.TextBox:
                    return CreateTextBox(controlsPrms.Value, controlsPrms.enable, true);
                default:
                    return null;
            }
        }

        #endregion
        #region /// \name Create the new value controls

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetControlPresentation(Control control)
        ///
        /// \brief Sets control presentation.
        ///
        /// \par Description.
        ///      Presentation parameters common to all the controls
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/04/2018
        ///
        /// \param control  (Control) - The control.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetControlPresentation(Control control)
        {
            control.FontSize = 11;
            control.Height = 22;
            control.Margin = new Thickness(1);
            control.Padding = new Thickness(2);
            control.BorderThickness = new Thickness(1);
            control.BorderBrush = UpdateStatusColors[(int)UpdateStatuses.NotUpdated];
            control.Foreground = UpdateStatusColors[(int)UpdateStatuses.NotUpdated];
            control.Background = Brushes.White;
            control.Effect = null;
            if (!control.IsEnabled)
            {
                control.Background = Brushes.LightGray;
            }
            else
            {
                control.Background = Brushes.White;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateTextBox(string content, bool enable, bool setEventHandler)
        ///
        /// \brief Creates text box.
        ///
        /// \par Description.
        ///      Create a Text box for all the text boxes in the window.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param content         (string) - The content.
        /// \param enable          (bool) - true to enable, false to disable.
        /// \param setEventHandler (bool) - true to set event handler.
        ///
        /// \return The new text box.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateTextBox(string content, bool enable, bool setEventHandler)
        {
            TextBox textBox = new TextBox();
            //textBox.Style = (Style)Resources["ElementWindowTextBox"];
            textBox.Text = content;
            textBox.IsEnabled = enable;
            textBox.TextAlignment = TextAlignment.Center;
            SetControlPresentation(textBox);
            if (setEventHandler)
            {
                textBox.LostFocus += TextBox_LostFocus;
            }
            return textBox;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateComboBox(ElementWindowPrms controlPrms)
        ///
        /// \brief Creates combo box.
        ///
        /// \par Description.
        ///      Create combo box for the new value.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param controlPrms (ElementWindowPrms) - The control prms.
        ///
        /// \return The new combo box.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateComboBox(NewValueControlPrms controlPrms)
        {
            ComboBox comboBox = new ComboBox();
            for (int idx = 0; idx < controlPrms.options.Length; idx++)
            {
                comboBox.Items.Add(controlPrms.options[idx]);
            }
            comboBox.Style = (Style)Resources["ElementWindowComboBox"];
            comboBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            comboBox.IsEnabled = controlPrms.enable;
            comboBox.SelectedItem = controlPrms.Value;
            SetControlPresentation(comboBox);
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            return comboBox;
        }

        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateButton(ElementWindowPrms controlsPrms)
        ///
        /// \brief Creates a button.
        ///
        /// \par Description.
        ///      Create a button for handling attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param controlsPrms (ElementWindowPrms) - The controls prms.
        ///
        /// \return The new button.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateButton(NewValueControlPrms controlsPrms)
        {
            Button button = new Button();
            button.Style = (Style)Resources["ElementWindowGridButton"];
            button.Content = controlsPrms.Value;
            button.IsEnabled = controlsPrms.enable;
            SetControlPresentation(button);           
            if (controlsPrms.button_click != null)
            {
                button.Click += controlsPrms.button_click;
            }
            return button;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected FrameworkElement CreateAddRemoveGrid(ElementWindowPrms controlPrms)
        ///
        /// \brief Creates add remove grid.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param controlPrms (ElementWindowPrms) - The control prms.
        ///
        /// \return The new add remove grid.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected FrameworkElement CreateAddRemoveGrid(NewValueControlPrms controlPrms)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Children.Add(CreateGridButton(0, "Add", controlPrms, Button_Add_Click));
            grid.Children.Add(CreateGridButton(1, "Remove", controlPrms, Button_Remove_Click));
            grid.Children.Add(CreateGridComboBox(2, "0", controlPrms, ComboBox_Index_SelectionChanged));
            grid.Margin = new Thickness(0);
            return grid;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private ComboBox CreateGridComboBox(int column, string content, ElementWindowPrms controlPrms, SelectionChangedEventHandler eventHandler)
        ///
        /// \brief Creates grid combo box.
        ///
        /// \par Description.
        ///      Create a combo box for the add remove grid.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param column       (int) - The column.
        /// \param content      (string) - The content.
        /// \param controlPrms   (ElementWindowPrms) - The control prms.
        /// \param eventHandler (SelectionChangedEventHandler) - The event handler.
        ///
        /// \return The new grid combo box.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ComboBox CreateGridComboBox(int column, 
            string content,
            NewValueControlPrms controlPrms,
            SelectionChangedEventHandler eventHandler)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add(content);
            comboBox.Style = (Style)Resources["ElementWindowComboBox"];
            comboBox.SelectedIndex = 0;
            SetGridControl(comboBox, column, controlPrms);
            comboBox.SelectionChanged += eventHandler;
            comboBox.IsEditable = false;
            return comboBox;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private Button CreateGridButton(int column, string content, ElementWindowPrms controlPrms, RoutedEventHandler eventHandler)
        ///
        /// \brief Creates grid button.
        ///
        /// \par Description.
        ///      Create button for the add remove grid.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param column       (int) - The column.
        /// \param content      (string) - The content.
        /// \param controlPrms   (ElementWindowPrms) - The control prms.
        /// \param eventHandler (RoutedEventHandler) - The event handler.
        ///
        /// \return The new grid button.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Button CreateGridButton(int column, 
            string content,
            NewValueControlPrms controlPrms,
            RoutedEventHandler eventHandler)
        {
            Button button = new Button();
            button.Style = (Style)Resources["ElementWindowGridButton"];
            button.Content = content;
            button.Click += eventHandler;
            SetGridControl(button, column, controlPrms);
            return button;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetGridControl(Control control, int column, ElementWindowPrms controlPrms)
        ///
        /// \brief Sets grid control.
        ///
        /// \par Description.
        ///      Set all the common attributes to all the controls in the add remove grid.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param control     (Control) - The control.
        /// \param column      (int) - The column.
        /// \param controlPrms  (ElementWindowPrms) - The control prms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetGridControl(Control control, int column, NewValueControlPrms controlPrms)
        {
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, column);
            control.IsEnabled = controlPrms.enable;
            SetControlPresentation(control);
        }
        #endregion
        #region /// \name Support for Value getting and setting by external objects

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ReplaceNewValueControl(NewValueControlPrms controlPrms, Attribute attribute)
        ///
        /// \brief Replace new value control.
        ///
        /// \par Description.
        ///      Replace a new value control assigned to an attribute with a new one 
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/11/2017
        ///
        /// \param controlPrms  (NewValueControlPrms) - The control prms.
        /// \param attribute    (Attribute) - The attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReplaceNewValueControl(NewValueControlPrms controlPrms, Attribute attribute)
        {
            FrameworkElement newValueControl = CreateNewValueControl(controlPrms);
            ControlsAttributeLink link = controlsAttributeLinks.First(entry => entry.Value.attribute == attribute).Value;
            link.newValueControl = newValueControl;
            UpdateNewValueChanged((Control)link.newValueControl, controlPrms.Value);
            RearrangeFields(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ChangeValue(string value, Attribute attribute)
        ///
        /// \brief Change value.
        ///
        /// \par Description.
        ///      Chane the value of a new value control
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/11/2017
        ///
        /// \param value      (string) - The value.
        /// \param attribute  (Attribute) - The attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ChangeValue(string value, Attribute attribute)
        {
            ControlsAttributeLink link = controlsAttributeLinks.First(entry => entry.Value.attribute == attribute).Value;
            Control newValueControl = (Control)link.newValueControl;
            if (newValueControl is TextBox)
            {
                ((TextBox)newValueControl).Text = value;
            }
            else if (newValueControl is Button)
            {
                ((Button)newValueControl).Content = value;
            }
            else if (newValueControl is ComboBox)
            {
                ((ComboBox)newValueControl).SelectedItem = value;
            }
            UpdateNewValueChanged(newValueControl, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GetNewValue(Attribute attribute)
        ///
        /// \brief Gets new value.
        ///        Get a value of a new value control
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/11/2017
        ///
        /// \param attribute  (Attribute) - The attribute.
        ///
        /// \return The new value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetNewValue(Attribute attribute)
        {
            ControlsAttributeLink link = controlsAttributeLinks.First(entry => entry.Value.attribute == attribute).Value;
            Control newValueControl = (Control)link.newValueControl;
            if (newValueControl is TextBox)
            {
                return ((TextBox)newValueControl).Text;
            }
            else if (newValueControl is Button)
            {
                return (string)((Button)newValueControl).Content;
            }
            else if (newValueControl is ComboBox)
            {
                return (string)((ComboBox)newValueControl).SelectedItem;
            }
            return "";
        }
        #endregion
        #region /// \name Rearrange Fields

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void RearangeFields(bool saveSelectedIndexes)
        ///
        /// \brief Rearrange fields.
        ///
        /// \par Description.
        ///      After the network elements were scanned this method is putting the controls in the pannels
        ///      and set their colors.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param saveSelectedIndexes (bool) - true to save selected indexes. This parameter is true is
        ///                            the scanning is not the first scanning of the window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void RearrangeFields(bool saveSelectedIndexes)
        {
            if (saveSelectedIndexes)
                SaveSelectedIndexs();
            StackPanel_AttributeTypes.Children.Clear();
            StackPanel_AttributeExistingValues.Children.Clear();
            StackPanel_AttributeNewValues.Children.Clear();
            StackPanel_AttributeTypes.Children.Add(Label_Types);
            StackPanel_AttributeExistingValues.Children.Add(Label_ExistingValues);
            StackPanel_AttributeNewValues.Children.Add(Label_NewValue);
            foreach (TreeViewItem chiledItem in TreeView.Items)
            {
                InsertControls(chiledItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void SaveSelectedIndexs()
        ///
        /// \brief Saves the selected indexs.
        ///
        /// \par Description.
        ///      when inserting the ComboBoxes of the AddRemoveGrid the options of the combo box are reinserted
        ///      In order to keep the index of the selected items this method is called to save the selected index
        ///      before the changing.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void SaveSelectedIndexs()
        {
            prevSelectedIndexes.Clear();
            foreach (var entry in controlsAttributeLinks)
            {
                if (entry.Value.newValueControl != null)
                {
                    // Get the combo box
                    if (entry.Value.newValueControl.GetType().Equals(typeof(Grid)))
                    {
                        Grid grid = (Grid)entry.Value.newValueControl;
                        ComboBox comboBox = grid.Children.OfType<ComboBox>().First();

                        // Save the selected index
                        prevSelectedIndexes.Add(entry.Value.attribute, comboBox.SelectedIndex);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void InsertControls(ItemsControl item)
        ///
        /// \brief Inserts the controls described by item.
        ///
        /// \par Description.
        ///      This method recursivelly insert the controls of an attribute to the pannels
        ///      and color them according to the update status on the link.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void InsertControls(ItemsControl item)
        {
            StackPanel_AttributeTypes.Children.Add(controlsAttributeLinks[item].typeTextBox);
            StackPanel_AttributeExistingValues.Children.Add(controlsAttributeLinks[item].existingValueTextBox);
            StackPanel_AttributeNewValues.Children.Add(controlsAttributeLinks[item].newValueControl);
            FillGridComboBox(item);
            if (scanProcess == ScanProcess.UpdateFromNetworkElement)
            {
                UpdateExistingValueUpdated(item);
            }
            SetColors(item);
            SetEnabled(item);
            foreach (ItemsControl chiledItem in item.Items)
            {
                InsertControls(chiledItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateExistingValueUpdated(ItemsControl item)
        ///
        /// \brief Updates the existing value updated described by item.
        ///
        /// \par Description
        ///      After updating from the NetworkElement while in running phase if the existing
        ///      value was changed outside of this window (by performng single step in the running
        ///      for example) the text in the existing value field will be in bold.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateExistingValueUpdated(ItemsControl item)
        {
            ControlsAttributeLink link = controlsAttributeLinks[item];
            if (link.existingValueTextBox != null)
            {
                if (link.existingValueWasUpdated)
                {
                    link.existingValueTextBox.FontWeight = System.Windows.FontWeights.Bold;
                }
                else
                {
                    link.existingValueTextBox.FontWeight = System.Windows.FontWeights.Normal;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void FillGridComboBox(ItemsControl item)
        ///
        /// \brief Fill grid combo box.
        ///
        /// \par Description
        ///      Fill the grid combo box.
        ///      The grid is composed from 2 buttons and a ComboBox which allows selecting
        ///      The attribute to be deleted or the place of a new item to be inserted
        ///      The combo box options are the keys of the attributes included in the complex
        ///      attribute that the grid represent
        ///      The keys are found in each items header.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void FillGridComboBox(ItemsControl item)
        {
            ControlsAttributeLink link = controlsAttributeLinks[item];

            if (link.newValueControl.GetType().Equals(typeof(Grid)))
            {
                ComboBox comboBox = ((Grid)link.newValueControl).Children.OfType<ComboBox>().First();
                comboBox.Items.Clear();

                foreach (TreeViewItem chiledItem in item.Items)
                {
                    comboBox.Items.Add(((TextBox)chiledItem.Header).Text);
                }
                comboBox.Items.Add("Last");

                Attribute attribute = link.attribute;
                if (attribute != null)
                {
                    if (prevSelectedIndexes.ContainsKey(attribute))
                    {
                        int prevSelectedIndex = prevSelectedIndexes[attribute];
                        if (prevSelectedIndex >= comboBox.Items.Count)
                        {
                            comboBox.SelectedIndex = comboBox.Items.Count - 1;
                        }
                        else
                        {
                            comboBox.SelectedIndex = prevSelectedIndex;
                        }
                    }
                    else
                    {
                        comboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        
        #endregion
        #region /// \name Visibility Handeling

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by TreeViewItem for collapsed events.
        ///
        /// \par Description.
        ///      If the tree view item was collapsed all the controll of all the item under this
        ///      item have to be set the visibility to collapsed so that the pannels will
        ///      be in sync with the tree view.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            ChangeVisibility(item, System.Windows.Visibility.Visible);
            SetVisibilityToTree(item, System.Windows.Visibility.Collapsed);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by TreeViewItem for expanded events.
        ///
        /// \par Description.
        ///      If the tree view item was expanded all the controls under it should be set visible
        ///      so that the pannels will be in sync with the tree view.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            ChangeVisibility(item, System.Windows.Visibility.Visible);
            SetVisibilityToTree(item, System.Windows.Visibility.Visible);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void HandleExpandedWhenClosingComplexItem()
        ///
        /// \brief Handles the expanded when closing complex item.
        ///
        /// \par Description.
        ///      -#  This method is called when a closing of a complex attribute occures
        ///      -#  The following is the policy of the visibility
        ///             -#  If the process of scanning is UpdateFromNetworkElement - all the controls will
        ///                 be collapsed
        ///             -#  Else If one of the fields under the complex attribute was updated - expand
        ///                 the attribute
        ///             -#  Else expand the attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void HandleExpandedWhenClosingComplexItem()
        {

            if (scanProcess != ScanProcess.UpdateFromNetworkElement)
            {
                if (((TreeViewItem)currentComplexItem.item).IsExpanded == false)
                {
                    ChangeVisibility(((TreeViewItem)currentComplexItem.item), System.Windows.Visibility.Visible);
                    SetVisibilityToTree(((TreeViewItem)currentComplexItem.item), System.Windows.Visibility.Collapsed);
                }
                ((TreeViewItem)currentComplexItem.item).IsExpanded = false;
            }
            else
            {
                if (currentComplexItem.existingValueWasUpdated)
                {
                    if (((TreeViewItem)currentComplexItem.item).IsExpanded == true)
                    {
                        ChangeVisibility(((TreeViewItem)currentComplexItem.item), System.Windows.Visibility.Visible);
                        SetVisibilityToTree(((TreeViewItem)currentComplexItem.item), System.Windows.Visibility.Visible);
                    }
                    else
                    {
                        ((TreeViewItem)currentComplexItem.item).IsExpanded = true;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void SetVisibilityToTree(TreeViewItem item, System.Windows.Visibility newVisibility)
        ///
        /// \brief Set visibility to an attribute and all the attributes under it.
        ///
        /// \par Description.
        ///      -  The visibility of the item and it's child items are identicall to the parrent's visibility
        ///         except from one case when the visibility is visible and the collaped is false. In this case
        ///         only the item will be set visible and children will be collaped.
        ///      -  This policy causes the following behavioure  
        ///         -   If the visibility is collapsed - All the subtree will be collaped  
        ///         -   Else only the chiled items that are expanded will be expanded (keep the status as it  
        ///             was before the collaps.  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param item          (TreeViewItem) - The item.
        /// \param newVisibility (Visibility) - The new visibility.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void SetVisibilityToTree(TreeViewItem item, System.Windows.Visibility newVisibility)
        {
            System.Windows.Visibility chiledVisibility = newVisibility;
            if (newVisibility == System.Windows.Visibility.Visible)
                if (item.IsExpanded == false)
                    chiledVisibility = System.Windows.Visibility.Collapsed;

            foreach (TreeViewItem chiledItem in item.Items)
            {
                ChangeVisibility(chiledItem, chiledVisibility);
                SetVisibilityToTree(chiledItem, newVisibility);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ChangeVisibility(TreeViewItem item, System.Windows.Visibility newVisibility)
        ///
        /// \brief Change visibility.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param item          (TreeViewItem) - The item.
        /// \param newVisibility (Visibility) - The new visibility.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ChangeVisibility(TreeViewItem item, System.Windows.Visibility newVisibility)
        {
            ControlsAttributeLink link = controlsAttributeLinks[item];
            link.typeTextBox.Visibility = newVisibility;
            link.newValueControl.Visibility = newVisibility;
            link.existingValueTextBox.Visibility = newVisibility;
        }
        #endregion
        #region /// \name New Value Controls event handling

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void Button_Remove_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Remove for click events.
        ///
        /// \par Description.
        ///      Handle the remove of an item from AttributeList and AttributeDictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            //Get the control of the removed attribute
            //get the Grid panel of the button
            Grid grid = (Grid)((Button)sender).Parent;

            //Find the link of the grid
            var listLinkEntry = controlsAttributeLinks.First(entry => entry.Value.newValueControl == grid);

            //Get the index from the combo box
            ComboBox comboBox = grid.Children.OfType<ComboBox>().First();
            int index = comboBox.SelectedIndex;
            if (index >= listLinkEntry.Value.item.Items.Count)
            {
                index = listLinkEntry.Value.item.Items.Count - 1;
            }
            if (comboBox.Items.Count == 0 || index < 0)
            {
                CustomizedMessageBox.Show("There are no items to remove", "Element Window Message", Icons.Error);
                return;
            }

            //Recursively update the newValueControls of the item in the index place of the list item
            HandleRemove((ItemsControl)listLinkEntry.Key.Items[index]);
            RearrangeFields(true);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void HandleRemove(ItemsControl item)
        ///
        /// \brief Handles the remove described by item.
        ///
        /// \par Description.
        ///      This method implements the toggle policy of remove event handler:
        ///      -# If the attribute was removed it return to NotUpdated
        ///      -# Else the attribute will have a remove state.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void HandleRemove(ItemsControl item)
        {
            if (controlsAttributeLinks[item].updateStatus == UpdateStatuses.Removed)
            {
                UpdateStatus(item, controlsAttributeLinks[item].prevUpdateStatus);
            }
            else
            {
                UpdateStatus(item, UpdateStatuses.Removed);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void Button_Add_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Add for click events.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            // Get the grid of the panel where the Add button is found
            Grid grid = (Grid)((Button)sender).Parent;

            //Get the index for the addition
            ComboBox comboBox = grid.Children.OfType<ComboBox>().First();
            int index = comboBox.SelectedIndex;

            // Find the link of the grid
            var complexAttributeLinkEntry = controlsAttributeLinks.First(entry => entry.Value.newValueControl == grid);

            // Set the current complex item to the item of the panel of the add button
            // (The methods that adds the item use it)
            currentComplexItem = complexAttributeLinkEntry.Value;
            ControlsAttributeLink newLink;
            SetCreateAttributeDialogPrms();
            newLink = CreateNewAttribute(index);

            if (newLink != null)
            {
                UpdateStatus(newLink.item, UpdateStatuses.Added);
                UpdateIndexInParent(complexAttributeLinkEntry.Key);
                scanProcess = ScanProcess.AdditionOfFields;
                RearrangeFields(true);
                ((TreeViewItem)complexAttributeLinkEntry.Value.item).IsExpanded = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void SetCreateAttributeDialogPrms()
        ///
        /// \brief Sets create attribute dialog prms.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void SetCreateAttributeDialogPrms()
        {
            attributeCreationPrms = new CreateAttributeDialog.Prms();
            if (currentComplexItem.attributeCategory == Attribute.AttributeCategory.ListOfAttributes)
            {
                    attributeCreationPrms.createKey = false;
            }
            else
            {
                attributeCreationPrms.existingKeys = CreateExistingKeysList(currentComplexItem);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ComboBox_Index_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ComboBox_Index for selection changed events.
        ///
        /// \par Description.
        ///      The issues handled in this method (when the grid combo box selection changes0
        ///      -# If the selected item is removed - write De - remove on the remove button of the pannel
        ///      -# Set the border of the selected item - Thick.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ComboBox_Index_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the control of the removed attribute
            //get the Grid panel of the button
            Grid grid = (Grid)((ComboBox)sender).Parent;

            //Find the link of the grid
            var listLinkEntry = controlsAttributeLinks.First(entry => entry.Value.newValueControl == grid);

            //Get the index from the combo box
            int index = ((ComboBox)sender).SelectedIndex;

            if (index >= listLinkEntry.Value.item.Items.Count)
            {
                return;
            }
            if (index < 0)
            {
                return;
            }

            // Get the link of the element the combo box is pointed at
            ControlsAttributeLink link = controlsAttributeLinks[(ItemsControl)listLinkEntry.Key.Items[index]];

            // Change the text on the remove button
            // The changing of the text is done only if the list is not in removed state
            // That means that if there is a list inside list and it is been removed
            // Only the top list will change it's button text 

            if (listLinkEntry.Value.updateStatus != UpdateStatuses.Removed)
            {
                // Get the remove button of the panel
                Button removeButton = grid.Children.OfType<Button>().First(b => (string)b.Content != "Add");

                // Set the content of the button
                if (link.updateStatus == UpdateStatuses.Removed)
                {
                    removeButton.Content = "De-Remove";
                }
                else
                {
                    removeButton.Content = "Remove";
                }
            }

            // Set the width of the border of the prev selected
            ControlsAttributeLink prevSelectedLink = listLinkEntry.Value.selectedChildLink;
            SetBorderWidth(prevSelectedLink, borderWidth);
            SetBorderWidth(link, selectedBorderWidth);
            listLinkEntry.Value.selectedChildLink = link;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected ControlsAttributeLink CreateNewAttribute(int index)
        ///
        /// \brief Creates new attribute.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param index (int) - Zero-based index of the.
        ///
        /// \return The new new attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink CreateNewAttribute(int index)
        {
            // Show the dialog for creating new attribute and creating the link for the new attribute
            ControlsAttributeLink newLink = CreateAttribute(index);

            // If the dialog was not quited
            if (newLink != null)
            {

                // If the new attribute is a complex attribute scan it (to add all the child attributes)
                Attribute newAttribute = newLink.attribute;
                if (Attribute.GetValueCategory(newAttribute) == Attribute.AttributeCategory.NetworkElementAttribute ||
                    Attribute.GetValueCategory(newAttribute) == Attribute.AttributeCategory.AttributeDictionary ||
                    Attribute.GetValueCategory(newAttribute) == Attribute.AttributeCategory.ListOfAttributes)
                {
                    currentComplexItem = newLink;
                    newAttribute.Value.ScanAndReport(this, newLink.mainDictionary, newLink.attributeDictionary);
                    ((TreeViewItem)newLink.item).IsExpanded = false;
                }
            }
            return newLink;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected ControlsAttributeLink CreateAttribute(int index, bool createKey)
        ///
        /// \brief Creates an attribute.
        ///
        /// \par Description.
        ///      Opens a dialog for getting new attribute and create the link and controls for the attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param index     (int) - Zero-based index of the.
        /// \param createKey (bool) - true to create key.
        ///
        /// \return The new attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ControlsAttributeLink CreateAttribute(int index)
        {
            Attribute newAttribute;
            dynamic key;
            if (currentComplexItem.gridAddMethod == null)
            {
                // Show the dialog
                CreateAttributeDialog createAttributeDialog = new CreateAttributeDialog(this, attributeCreationPrms);
                createAttributeDialog.ShowDialog();

                // Get the attribute from the dialog. If it is null it means that the dialog quitted
                newAttribute = createAttributeDialog.newAttribute;                
                key = createAttributeDialog.key;
            }
            else
            {
                newAttribute = currentComplexItem.gridAddMethod();
                key = "[" + index.ToString() + "]";
            }

            if (newAttribute == null) return null;

            // The following command will set the members to all the tree under the attribute
            IValueHolder parent = (IValueHolder)currentComplexItem.attribute.Value;
            newAttribute.SetMembers(parent.Network, parent.Element, parent.Permissions, parent);


            // Create a new item, controls and link
            ControlsAttributeLink newLink = CreateNewItem(newAttribute,
                key,
                currentComplexItem.mainDictionary,
                currentComplexItem.attributeDictionary,
                index);
            return newLink;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ListIdsHandling(Attribute newAttribute)
        ///
        /// \brief List identifiers handling.
        ///
        /// \par Description.
        ///      -  The list ids are used in the UpdateNetwork algorithm  
        ///      -  The Update network is responsible for updating a network after it's code was changed  
        ///         In the AddAlgorithmWindow
        ///      -  See the NetworkUpdate class for more details of the use of these counters  
        ///      -  Each list has 2 counters:  
        ///         -# A positive counter that is responsible to the ids of the attributes that are in the code
        ///         -# A negative counter that is responsible to the ids of the attributes that where created in design time
        ///
        /// \par Algorithm.
        ///      -# If the list is a new list - create the new attributes
        ///      -# If the current complex item is a list :
        ///         -#  Advance the appropriate counter and set the id to the new attribute
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 28/12/2017
        ///
        /// \param newAttribute (Attribute) - The new attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void ListIdsHandling()
        //{            
        //    // If the addition is to list - Update the IdInList
        //    // The value is automatically assigned to the attribute and the counter when
        //    // the attribute was inserted to the list.
        //    // Here we have to take care about the presentation - the counter attribute was changed

        //    Attribute currentComplexAttribute = currentComplexItem.attribute;
        //    if (Attribute.GetValueCategory(currentComplexAttribute) == Attribute.AttributeCategory.ListOfAttributes)
        //    {
        //        // If added while changing the code
        //        int stepAndId;
        //        if (inputWindow == InputWindows.AddAlgorithmWindow)
        //        {
        //            stepAndId = 1;
        //        }
        //        else
        //        {
        //            stepAndId = -1;
        //        }
        //        Attribute counterAttribute = ((AttributeList)currentComplexAttribute.Value)[stepAndId.ToString()];
        //        TextBox textBox = (TextBox)controlsAttributeLinks.First(l => l.Value.attribute == counterAttribute).Value.newValueControl;
        //        UpdateNewValueChanged(textBox, counterAttribute.Value.ToString());
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected List<dynamic> CreateExistingKeysList(ControlsAttributeLink link)
        ///
        /// \brief Creates existing keys list.
        ///
        /// \par Description.
        ///      Create a list of keys of the item (Called only for AttributeDictionary)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param link (ControlsAttributeLink) - The link.
        ///
        /// \return The new existing keys list.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual List<dynamic> CreateExistingKeysList(ControlsAttributeLink link)
        {
            List<dynamic> existingKeys = new List<dynamic>();
            foreach (ItemsControl chiledItem in link.item.Items)
            {
                ControlsAttributeLink childLink = controlsAttributeLinks[chiledItem];
                existingKeys.Add(childLink.key);
            }
            return existingKeys;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateStatus(ItemsControl item, UpdateStatuses newStatus)
        ///
        /// \brief Updates the status of an attribute.
        ///
        /// \par Description.
        ///      When adding or removing a field all the fields in the attribute tree has to be updated with
        ///      the same status (If an item is removed or added the items under it should get the same status)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param item      (ItemsControl) - The item.
        /// \param newStatus (UpdateStatuses) - The new status.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateStatus(ItemsControl item, UpdateStatuses newStatus)
        {
            ControlsAttributeLink link = controlsAttributeLinks[item];
            link.prevUpdateStatus = link.updateStatus;
            link.updateStatus = newStatus;
            foreach (ItemsControl chiledItem in item.Items)
            {
                UpdateStatus(chiledItem, newStatus);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by TextBox for lost focus events.
        ///
        /// \par Description.
        ///      Actions when a new value was inserted to a new value text box;
        ///      -# Check if the new value can be assigned to the attribute according to the attribute type -
        ///         If failed generate error message
        ///      -# Activate EndInputOperation for the attribue -
        ///         If failed - restore the previouse value.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlsAttributeLink link = controlsAttributeLinks.First(entry => entry.Value.newValueControl == sender).Value;
            if (CheckIfNewValueCanBeAssigned(((TextBox)sender).Text, link) == false) return;
            if (activateEndInputOperation)
            {
                if (EndInputOperation(((TextBox)sender).Text, link, this) == false)
                {
                    ((TextBox)sender).Text = link.existingNewValue;
                    return;
                }
            }
            UpdateNewValueChanged((Control)sender, ((TextBox)sender).Text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by ComboBox for selection changed events.
        ///
        /// \par Description.
        ///      Action when a new value was selected in a new value combo box
        ///      -# Activate end input operation method of the attribute
        ///         If failed restore the previouse value.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param sender (object) - Source of the event.
        /// \param e      (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ControlsAttributeLink link = controlsAttributeLinks.First(entry => entry.Value.newValueControl == sender).Value;
            //string selectedItemText = ((TextBlock)((ComboBox)sender).SelectedItem).Text;
            string selectedItemText = (string)((ComboBox)sender).SelectedItem;

            if (activateEndInputOperation)
            {
                if (EndInputOperation(selectedItemText, link, this) == false)
                {
                    ((ComboBox)sender).SelectedItem = link.existingNewValue;
                    return;
                }
            }
            UpdateNewValueChanged((Control)sender, selectedItemText);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CheckIfNewValueCanBeAssigned(string newValue, ControlsAttributeLink link)
        ///
        /// \brief Determine if new value can be assigned.
        ///
        /// \par Description.
        ///      Check if the new value can be assigned to the attribute according to the attribute type.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param newValue (string) - The new value.
        /// \param link     (ControlsAttributeLink) - The link.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CheckIfNewValueCanBeAssigned(string newValue, ControlsAttributeLink link)
        {
            bool converted = true;
            string typeString;

            typeString = link.attribute.GetTypeToString();
            if (!link.attribute.CheckIfStringCanBeParssed(newValue))
            {
                converted = false;
            }

            if (!converted)
            {
                CustomizedMessageBox.Show("The value inserted cannot be parsed to type " + typeString, "Element Input Message", Icons.Error);
                ((TextBox)link.newValueControl).Text = link.existingNewValue;
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool EndInputOperation(string newValue, ControlsAttributeLink link, ElementWindow elementWindow)
        ///
        /// \brief Ends input operation.
        ///
        /// \par Description.
        ///      Activate the end input operation of an attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param newValue      (string) - The new value.
        /// \param link          (ControlsAttributeLink) - The link.
        /// \param elementWindow (ElementWindow) - The element window.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool EndInputOperation(string newValue, ControlsAttributeLink link, ElementWindow elementWindow)
        {

            //Activate the end input operation method that check if insertion is leagle
            //If the insertion is not leagle show a message box an return the text in 
            //the input control to the last one saved in the attribute
            string errorMessage;
            Attribute.EndInputOperationDelegate endInputOperation = ((Attribute)link.attribute).EndInputOperation;
            if (!(endInputOperation(networkElements[0].Network, link.hostingNetworkElement, link.parentAttribute, (Attribute)link.attribute, newValue, out errorMessage, elementWindow)))
            {
                CustomizedMessageBox.Show(errorMessage, "Element Window Message", Icons.Error);
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateNewValueChanged(Control control, string newValue)
        ///
        /// \brief Updates the new value changed.
        ///
        /// \par Description.
        ///      This method is called by the event handlers of the new value controls after checking
        ///      the new value to update the link with the new value.
        ///
        /// \par Algorithm.
        ///      -# if the new value is identical to the existing (In the network element
        ///         set the status as not updated exept when the attribute is new
        ///      -3 Else set the status to updated exept when the attribute is new.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param control  (Control) - The control.
        /// \param newValue (string) - The new value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateNewValueChanged(Control control, string newValue)
        {
            ControlsAttributeLink link = controlsAttributeLinks.First(entry => entry.Value.newValueControl == control).Value;
            if (link.existingValueTextBox.Text == newValue)
            {
                if (link.updateStatus != UpdateStatuses.Added)
                {
                    link.updateStatus = UpdateStatuses.NotUpdated;
                }
                link.existingNewValue = newValue;
            }
            else
            {
                if (link.updateStatus != UpdateStatuses.Added)
                {
                    link.updateStatus = UpdateStatuses.Updated;
                }
                link.existingNewValue = newValue;
            }
            SetColors(link.item);
        }
        #endregion
        #region /// \name controls graphical parameters setting

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private List<Control> controlsOfLink(ControlsAttributeLink link)
        ///
        /// \brief Controls of link.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 31/10/2017
        ///
        /// \param link  (ControlsAttributeLink) - The link.
        ///
        /// \return A List&lt;Control&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<Control> LinkControls(ControlsAttributeLink link)
        {
            List<Control> controls = new List<Control>();
            // The new value controls
            // If this is AddRemoveGrid - add the 3 controls to the list
            if (typeof(Grid).Equals(link.newValueControl.GetType()))
            {
                foreach (Control chiledControl in ((Grid)link.newValueControl).Children)
                {
                    controls.Add(chiledControl);
                }
            }

            // Add the new value control to the other control types
            else
            {
                controls.Add((Control)link.newValueControl);
            }

            // Add the controls of the header and the existing value
            controls.Add(link.existingValueTextBox);
            controls.Add((TextBox)((TreeViewItem)link.item).Header);
            controls.Add(link.typeTextBox);
            return controls;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetBorderWidth(ControlsAttributeLink link, int borderWidth)
        ///
        /// \brief Sets border width.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param link        (ControlsAttributeLink) - The link.
        /// \param borderWidth (int) - Width of the border.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetBorderWidth(ControlsAttributeLink link, int borderWidth)
        {
            if (link == null)
            {
                return;
            }

            List<Control> controls = LinkControls(link);
            foreach(Control control in controls)
            {
                control.BorderThickness = new Thickness(borderWidth);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void SetColors(ItemsControl item)
        ///
        /// \brief Sets the colors of the new value control.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \param item (ItemsControl) - The item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void SetColors(ItemsControl item)
        {
            ControlsAttributeLink link = controlsAttributeLinks[item];

            // Collect the controls to change the colors to
            List<Control> controls = LinkControls(link);

            foreach (Control control in controls)
            {
                control.Foreground = UpdateStatusColors[(int)link.updateStatus];
                control.BorderBrush = UpdateStatusColors[(int)link.updateStatus];
            }
        }

        protected void SetEnabled(ItemsControl item)
        {
            if (controlsAttributeLinks[item].updateStatus == UpdateStatuses.Removed)
            {
                controlsAttributeLinks[item].newValueControl.IsEnabled = false;
            }
        }
        #endregion
        #region /// \name Update the network elements

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateNetworkElement()
        ///
        /// \brief Updates the network element.
        ///        Update network element with the changes done in the window.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateNetworkElement()
        {
            // Save the indexes of the Add Remove grid for recovery when the window will be rebuilded
            SaveSelectedIndexs();

            // Update the network elements with the changes 
            UpdateAllLists();
            UpdateAllDictionaries();
            SetAllNewValuesToAttributes();

            // Clear the all the windows controls and data
            controlsAttributeLinks.Clear();
            TreeView.Items.Clear();
            StackPanel_AttributeNewValues.Children.Clear();
            StackPanel_AttributeExistingValues.Children.Clear();

            // Rebuild the window
            FillTreeView(TreeView);

            // Update the network's element presentation
            foreach (NetworkElement networkElement in networkElements)
            {
                if (networkElement.Presentation != null)
                {
                    networkElement.Presentation.UpdatePresentation();
                }
                if (MainWindow.ActivationPhase == MainWindow.ActivationPhases.Running)
                {
                    networkElement.UpdateRunningStatus(new object[] { true });
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void SetAllNewValuesToAttributes()
        ///
        /// \brief Sets all new values to attributes.
        ///
        /// \par Description.
        ///      Update all the attributes that where changed with the value inserted.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void SetAllNewValuesToAttributes()
        {
            foreach (ControlsAttributeLink link in controlsAttributeLinks.Values)
            {
                if (link.updateStatus == UpdateStatuses.Updated || link.updateStatus == UpdateStatuses.Added)
                {
                    link.attribute.SetValueFromString(link.existingNewValue, this);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateAllLists()
        ///
        /// \brief Updates all lists.
        ///
        /// \par Description.
        ///      Update an attribute which is list with all the child attributes.
        ///
        /// \par Algorithm.
        ///      The process is :
        ///      -# Remove from the link list all the items that were removed
        ///      -# Clear the list in the network element
        ///      -# Fill the network's element list with the attributes that where left in the link.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateAllLists()
        {
            foreach (ControlsAttributeLink link in controlsAttributeLinks.Values)
            {
                if (link.attributeCategory == Attribute.AttributeCategory.ListOfAttributes)
                {
                    for (int idx = link.item.Items.Count - 1; idx >= 0; idx--)
                    {
                        if (controlsAttributeLinks[(ItemsControl)(link.item.Items[idx])].updateStatus == UpdateStatuses.Removed)
                        {
                            link.item.Items.RemoveAt(idx);
                        }
                    }
                    AttributeList list = link.attribute.Value;
                    list.Clear(false, this);
                    foreach (ItemsControl chiledItem in link.item.Items)
                    {
                        Attribute attribute = controlsAttributeLinks[chiledItem].attribute;

                        // IdInList member is updated only when the first addition to the list occurs
                        // So if the IdInList not 0 the IdInList should not be updated 
                        if (attribute.IdInList == 0)
                        {
                            list.Add(attribute, true, this);
                        }
                        else
                        {
                            list.Add(attribute, false, this);
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void UpdateAllDictionaries()
        ///
        /// \brief Updates all dictionaries.
        ///
        /// \par Description.
        ///      Update an attribute which is a dictionary with all the child attributes.
        ///
        /// \par Algorithm.
        ///      The process is :
        ///      -# Remove from the link dictionary all the items that were removed
        ///      -# Clear the dictionary in the network element
        ///      -# Fill the network's element dictionary with the attributes that where left in the link.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void UpdateAllDictionaries()
        {
            foreach (ControlsAttributeLink link in controlsAttributeLinks.Values)
            {
                if (link.attributeCategory == Attribute.AttributeCategory.AttributeDictionary)
                {
                    for (int idx = link.item.Items.Count - 1; idx >= 0; idx--)
                    {
                        if (controlsAttributeLinks[(ItemsControl)(link.item.Items[idx])].updateStatus == UpdateStatuses.Removed)
                        {
                            link.item.Items.RemoveAt(idx);
                        }
                    }
                    AttributeDictionary attributeDictionary = link.attribute.Value;
                    attributeDictionary.Clear(this);
                    foreach (ItemsControl chiledItem in link.item.Items)
                    {
                        attributeDictionary.Add(controlsAttributeLinks[chiledItem].key,
                            controlsAttributeLinks[chiledItem].attribute, this);
                    }
                }
            }
        }

        #endregion
        #region /// \name Utility methods 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ResetToExisting()
        ///
        /// \brief Resets to existing.
        ///
        /// \par Description.
        ///      Reset the window to the values in the network element undoing all the changes.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ResetToExisting()
        {
            controlsAttributeLinks.Clear();
            TreeView.Items.Clear();
            StackPanel_AttributeNewValues.Children.Clear();
            StackPanel_AttributeExistingValues.Children.Clear();
            FillTreeView(TreeView);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected void ResetToInit()
        ///
        /// \brief Resets to init.
        ///
        /// \par Description.
        ///      Init the network element (Using the Init method of the network element)
        ///      and update the window.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ResetToInit()
        {
            networkElements[0].Init(networkElements[0].ea[ne.eak.Id], false);
            ResetToExisting();
            networkElements[0].ea[ne.eak.Edited] = false;
        }

        protected void CheckNetworkElements(string checkName = "")
        {
            foreach (NetworkElement networkElement in networkElements)
            {
                networkElement.CheckMembers("Check : " + checkName + "from ElementWindow");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected bool Exit()
        ///
        /// \brief Exits this object.
        ///
        /// \par Description.
        ///      Method for exiting from the window (Save the changes if the user confirm)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/06/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected bool Exit(bool forceSave = false)
        {
            bool elementWasSaved = false;
            if (forceSave || controlsAttributeLinks.Any(entry => entry.Value.updateStatus != UpdateStatuses.NotUpdated))
            {
                MessageBoxResult dialogResult =
                    CustomizedMessageBox.Show("There are updated or removed or added attributes. Do you want to update the network element ? ?", "Changed attributes dialoge", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    SetAllNewValuesToAttributes();
                    UpdateAllLists();
                    UpdateAllDictionaries();
                    controlsAttributeLinks.Clear();
                    TreeView.Items.Clear();
                    elementWasSaved = true;
                }
            }
            foreach (NetworkElement networkElement in networkElements)
            {
                if (networkElement.Presentation != null)
                {
                    networkElement.Presentation.presentationElements[networkElement].debugWindow = null;
                }
            }
            Close();
            return elementWasSaved;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Window GetParentWindow(DependencyObject child)
        ///
        /// \brief Gets the parent window.
        ///
        /// \par Description.
        ///      A static method to be used by static methods (Set by ElementWindowControlPrms)
        ///      to get the parent window
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 20/12/2017
        ///
        /// \param child  (DependencyObject) - The child.
        ///
        /// \return The parent window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            Window parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }
        #endregion
    }
}
