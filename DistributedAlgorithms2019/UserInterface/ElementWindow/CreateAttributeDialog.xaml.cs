////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\CreateAttributeDialog.xaml.cs
///
///\brief   Implements the create attribute dialog.xaml class.
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
using System.Reflection;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class MethodsLists
    ///
    /// \brief The methods lists.
    ///
    /// \par Description.
    ///      -  The reading of the methods list takes long time so we use a singlton in order to read
    ///      them only once
    ///      -  The singlton implementation is :  
    ///      -  There are 2 interface static methods (for each kind of list  
    ///      -  The algorithm of the methods is to create the object (if not exists) using a private constructor  
    ///      -  The private constructor reads and fill the lists  
    ///      -  On the next time the methods will be activated the lists are already in the static member and  
    ///         there is no reason to reread the lists 
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 17/01/2018
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    class MethodsLists
    {
        /// \brief  (List&lt;MethodInfo&gt;) -   The end input operation methods.
        private List<MethodInfo> endInputOperationMethods = null;

        /// \brief  (List&lt;MethodInfo&gt;) - The element window prms method.
        private List<MethodInfo> elementWindowPrmsMethod = null;

        /// \brief  (static MethodsLists) - The method lists.
        private static MethodsLists methodLists = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private MethodsLists()
        ///
        /// \brief Constructor that prevents a default instance of this class from being created.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/01/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private MethodsLists()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            endInputOperationMethods = TypesUtility.GetAllEndInputOperations();
            elementWindowPrmsMethod = TypesUtility.GetAllElementWindowPrmsMethods();
            Mouse.OverrideCursor = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static List<MethodInfo> GetEndInputOperations()
        ///
        /// \brief Gets end input operations.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/01/2018
        ///
        /// \return The end input operations.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<MethodInfo> GetEndInputOperations()
        {
            if (methodLists is null)
            {
                methodLists = new MethodsLists();
            }
            return methodLists.endInputOperationMethods;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static List<MethodInfo> GetElementWindowPrmsMethods()
        ///
        /// \brief Gets element window prms methods.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 17/01/2018
        ///
        /// \return The element window prms methods.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<MethodInfo> GetElementWindowPrmsMethods()
        {
            if (methodLists is null)
            {
                methodLists = new MethodsLists();
            }
            return methodLists.elementWindowPrmsMethod;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class CreateAttributeDialog
    ///
    /// \brief Dialog for creating attributes.
    ///
    /// \brief #### Description.
    ///        The output this dialog is ;
    ///        -#   An attribute with all the fields
    ///        -#   The key for the attribute in the AttributeDictionary that it is ment to be added to
    ///        
    ///        The process of using this dialog is 
    ///        -#   Init : All possible values are inserted to members, The list box are fielded
    ///        -#   Selecting : Selecting from the list boxes
    ///        -#   Finish : Generate Attribute and fill it with the selection results
    ///
    /// \brief #### Usage Notes.
    ///        After the dialod was shoen there are to outputs to retrieve;
    ///        -#   newAttribute - The Attribute object if it is null it meens that the dialog quitted
    ///        -#   key - The key
    ///
    /// \author Ilanh
    /// \date 09/05/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class CreateAttributeDialog : Window
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \class Prms
        ///
        /// \brief A prms.
        ///
        /// \par Description.
        ///      This class holds the parameters for the CreateAttributeDialog
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 30/08/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class Prms
        {
            /// \brief  (List&lt;dynamic&gt;) - The existing keys.
            public List<dynamic> existingKeys = null;

            /// \brief  (bool) - true to enable, false to disable the editable check box.
            public bool enableEditableCheckBox = false;

            /// \brief  (bool) - true to enable, false to disable the element window prms select.
            public bool enableElementWindowPrmsSelect = true;

            /// \brief  (bool) - true to enable, false to disable the end input operations select.
            public bool enableEndInputOperationsSelect = true;

            /// \brief  (List&lt;string&gt;) - List of types of the attributes.
            public List<string> attributeTypes = null;

            /// \brief  (bool) - true to create key.
            public bool createKey = true;

            /// \brief  (string) - Categories the key belongs to.
            public List<string> keyCategories = null;

            /// \brief  (List&lt;string&gt;) - List of types of the keys.
            public List<string> keyTypes = null;

        }
        #region /// \name Members
        
        /// \brief  (List&lt;string&gt;) - List of options for types of the value of the attributes.
        private List<string> attributeTypes;        

        /// \brief  (Attribute) - The new attribute that was created by this dialog (a result variable).
        public Attribute newAttribute = null;


        /// \brief  (dynamic) - The new key that was selected by this dialog (a result variable).
        public dynamic key = null;

        /// \brief  (Prms) - The prms.
        private Prms prms;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ElementWindow) - The calling window.
        ///        This attribute is used to decide the value of the Checked member of the attribute
        ///        when generating the attribute.
        ///        Basically is the calling window is AddAlgorithmWindow the value of Checked will be false
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ElementWindow callingWindow; 
       
        #endregion

        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public CreateAttributeDialog(List<dynamic> existingKeys, bool enableEditableCheckBox, bool createKey, bool stringKeyOnly = true)
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
        /// \date 09/05/2017
        ///
        /// \brief #### Description.
        ///
        /// \param existingKeys           (List&lt;dynamic&gt;) - The existing keys (for avoiding duplicate keys).
        /// \param enableEditableCheckBox (bool) - true to enable, false to disable the editable check box.
        /// \param createKey              (bool) - true to create key for the attribute.
        /// \param stringKeyOnly          (Optional)  (bool) - true to string key only.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public CreateAttributeDialog( ElementWindow callingWindow, Prms prms)
        {
            this.prms = prms;
            this.callingWindow = callingWindow;
            InitializeComponent();
            if (!prms.createKey)
            {
                DockPanel_Key.Visibility = Visibility.Collapsed;
                Label_SelectAttributeKey.Visibility = Visibility.Collapsed;
            }
            else
                InitKeyComboBoxes();
            FillEndInputOperationsListBox();
            FillInputFieldListBox();
            FillTypesListBox();
            CheckBox_Edditable.IsEnabled = prms.enableEditableCheckBox;
        }
        #endregion

        # region /// \name Key Inserting methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void InitKeyComboBoxes()
        ///
        /// \brief Init key combo boxes.
        ///
        /// \brief #### Description.
        ///        
        ///
        /// \brief #### Algorithm.
        ///        -#   Fill the Select Key Type category ComboBox
        ///        -#   Set the Selected index in this ComboBox to 0
        ///        -#   Activate The event handler for selection changed of the select category ComboBox
        ///             to fill the other fields
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitKeyComboBoxes()
        {
            if (prms.keyCategories != null)
            {
                FillComboBox(ComboBox_SelectKeyCategory, prms.keyCategories);
            }
            else
            {
                FillComboBox(ComboBox_SelectKeyCategory, TypesUtility.GetKeyTypesCategories());
            }
            ComboBox_SelectKeyCategory_SelectionChanged(null, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void FillComboBox(ComboBox comboBox, List<string> items)
        ///
        /// \brief Fill ComboBox.
        ///
        /// \brief #### Description.
        ///        This method filles one of the key ComboBoxes
        ///
        /// \brief #### Algorithm.
        ///        -#   Disable the event handlers
        ///        -#   Fill the items of the ComboBox
        ///        -#   Set the selected index to 0
        ///        -#   Enable the event handlers
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param comboBox  (ComboBox) - The combo box.
        /// \param items     (List&lt;string&gt;) - The items.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FillComboBox(ComboBox comboBox, List<string> items)
        {
            ComboBox_SelectKeyCategory.SelectionChanged -= ComboBox_SelectKeyCategory_SelectionChanged;
            ComboBox_SelectKeyType.SelectionChanged -= ComboBox_SelectKeyType_SelectionChanged;
            comboBox.ItemsSource = items;
            if (comboBox.Items.Count != 0)
            {
                comboBox.SelectedIndex = 0;
            }
            ComboBox_SelectKeyType.SelectionChanged += ComboBox_SelectKeyType_SelectionChanged;
            ComboBox_SelectKeyCategory.SelectionChanged += ComboBox_SelectKeyCategory_SelectionChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ComboBox_SelectKeyCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ComboBox_SelectKeyCategory for selection changed events.
        ///
        /// \brief #### Description.
        ///        Change all the key insertion fields after the type category was changed
        ///
        /// \brief #### Algorithm.
        ///        -#   Fill the Select Type ComboBox with the types according to the category
        ///        -#   Set the key insertion ComboBox and text box to the new key type 
        ///             according to the selection of the Type combo box
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ComboBox_SelectKeyCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prms.keyTypes != null)
            {
                FillComboBox(ComboBox_SelectKeyType, prms.keyTypes);
            }
            else
            {
                FillComboBox(ComboBox_SelectKeyType, TypesUtility.GetTypesOfCategory((string)ComboBox_SelectKeyCategory.SelectedItem));
            }
            ComboBox_SelectKeyType_SelectionChanged(null, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void ComboBox_SelectKeyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief Event handler. Called by ComboBox_SelectKeyType for selection changed events.
        ///
        /// \brief #### Description.
        ///        Set the key input fields after the key type was selected
        ///        there are 2 key input fields and 3 options
        ///        -#   If the category is Enum : activate the ComboBox
        ///        -#   If the type is boolean : activate the ComboBox
        ///        -#   Else : activate the TextBox with the default value of the type
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (SelectionChangedEventArgs) - Selection changed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ComboBox_SelectKeyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the type category and the type from the ComboBoxes
            TypeCategories typeCategory = (TypeCategories)TypesUtility.GetKeyFromString(typeof(TypeCategories), (string)ComboBox_SelectKeyCategory.SelectedItem);
            Type type = Type.GetType((string)ComboBox_SelectKeyType.SelectedItem);

            // If the type is an Enum - Present the ComboBox with the enum possible values)
            if (typeCategory == TypeCategories.Enum)
            {
                FillComboBox(ComboBox_SelectKey, TypesUtility.GetPossibleValues((string)ComboBox_SelectKeyType.SelectedItem));
                ComboBox_SelectKey.Visibility = Visibility.Visible;
                TextBox_InsertKey.Visibility = Visibility.Collapsed;
            }

            // If the type is boolean - Present the ComboBox with the bolean values
            else if (type.Equals(typeof(bool)))
            {
                FillComboBox(ComboBox_SelectKey, new List<string> { "Flase", "True" });
                ComboBox_SelectKey.Visibility = Visibility.Visible;
                TextBox_InsertKey.Visibility = Visibility.Collapsed;
            }

            // Else (Other simple types) - Present the TextBox with the default value of the type
            else
            {   
                TextBox_InsertKey.Text = TypesUtility.GetDefault(type).ToString();
                ComboBox_SelectKey.Visibility = Visibility.Collapsed;
                TextBox_InsertKey.Visibility = Visibility.Visible;

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool SetKey()
        ///
        /// \brief Sets the key.
        ///
        /// \brief #### Description.
        ///        This method is activated when exiting the dialog with Create button
        ///        It sets the key member to it's final value so that the calling method
        ///        can get it
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool SetKey()
        { 
            dynamic key;
            Type type = Type.GetType((string)ComboBox_SelectKeyType.SelectedItem);

            // If the type of the key is boolean or enum - get the key from the ComboBox 
            // and convert it from string to key
            if (TextBox_InsertKey.Visibility == Visibility.Collapsed)
            {
                key = TypesUtility.GetKeyFromString(type,(string)ComboBox_SelectKey.SelectedItem);
            }

            else if (type.Equals(typeof(string)))
            {
                key = TextBox_InsertKey.Text;
                Microsoft.CSharp.CSharpCodeProvider csharpProvider = new Microsoft.CSharp.CSharpCodeProvider();
                if (!csharpProvider.IsValidIdentifier(key))
                {
                    CustomizedMessageBox.Show("Error in key insertion :\n" +
                                            "The value : '" + TextBox_InsertKey.Text + " is not a leagal c# variable name",
                                            "Create Attribute Window", Icons.Error);
                    return false;
                }
            }

            // If the type of the key is other Try to convert the string in the TextBox to
            // the selected type - if failed - return false
            else
            {
                bool converted;
                key = TypesUtility.ConvertValueFromString(type, TextBox_InsertKey.Text, out converted);
                if (!converted)
                {
                    CustomizedMessageBox.Show("Error in key insertion :\n" +
                        "The value : '" + TextBox_InsertKey.Text + "' Cannot be converted to type : " + type.ToString(),
                        "Create Attribute Window", Icons.Error);
                    return false;
                }
            }

            // Check if the key already exist the dictionary (If the attribute should be inserted
            // to a list no check is made)
            // If the check succeeded set the member and return true
            if (CheckIfKeyExist(key))
            {
                this.key = key;
                return true;
            }
            else
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CheckIfKeyExist(dynamic key)
        ///
        /// \brief Determine if key exist.
        ///
        /// \brief #### Description.
        ///        If the createKey is true check if the key selected is already in the 
        ///        dictionary that the attribute will belong to
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param key  (dynamic) - The key.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CheckIfKeyExist(dynamic key)
        {
            if (prms.createKey)
            {
                if (prms.existingKeys.Any(k => TypesUtility.CompareDynamics(key, k)))
                {
                    CustomizedMessageBox.Show("Error in key insertion :\n" +
                        "The key already exist in the dictionary that the attribute will be inserted to", 
                        "Create Attribute Dialog", Icons.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region /// \name List Boxes fillings

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void FillEndInputOperationsListBox()
        ///
        /// \brief Fill end input operations list box.
        ///
        /// \brief #### Description.
        ///        Collect all the methods from the type of the delegate to MethodInfo to a member
        ///        Set the ListBox
        ///        
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FillEndInputOperationsListBox()
        {
            List<string> methods = MethodsLists.GetEndInputOperations().Select(m => TypesUtility.GetMethodAndNamespace(m)).ToList();
            ListBox_EndInputOperations.ItemsSource = methods;
            ListBox_EndInputOperations.SelectedIndex = 0;
            ListBox_EndInputOperations.IsEnabled = prms.enableEndInputOperationsSelect;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void FillInputFieldListBox()
        ///
        /// \brief Fill input field list box.
        ///
        /// \brief #### Description.
        ///        Collect all the methods from the type of the delegate to MethodInfo to a member
        ///        Set the ListBox
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FillInputFieldListBox()
        {
            List<string> methods = MethodsLists.GetElementWindowPrmsMethods().Select(m => TypesUtility.GetMethodAndNamespace(m)).ToList();
            ListBox_ElementWindowPrmsMethods.ItemsSource = methods;
            ListBox_ElementWindowPrmsMethods.SelectedIndex = 0;
            ListBox_ElementWindowPrmsMethods.IsEnabled = prms.enableElementWindowPrmsSelect;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void FillTypesListBox()
        ///
        /// \brief Fill types list box.
        ///
        /// \brief #### Description.
        ///        Fill the ListBox with the possible types of the attribute
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FillTypesListBox()
        {
            if (prms.attributeTypes != null)
            {
                attributeTypes = prms.attributeTypes;
            }
            else
            {
                attributeTypes = TypesUtility.GetAllPossibleTypesForAttribute();
            }
            foreach (string attributeType in attributeTypes)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = attributeType;
                ListBox_AttributeTypes.Items.Add(listBoxItem);
            }
            ListBox_AttributeTypes.SelectedIndex = 0;
        }
        #endregion
        #region /// \name Returning from the dialog
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Create_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Create for click events.
        ///
        /// \brief #### Description.
        ///        Generates the key and attribute from the dialog and close the dialog
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///        If the key cannot be produced the closing is canceled
        ///        The key cannot be produced because of 2 reasons;
        ///        -#   The text in the TextBox cannot be parsed to the type 
        ///        -#   The key already exist in the dictionary that the attribute will belong to
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            // Set the key
            if (prms.createKey)
            {
                if (!SetKey())
                {
                    return;
                }
            }
            else
            {
                key = "New";
            }
            // Create the attribute
            newAttribute = new Attribute() {Value = TypesUtility.GetDefault(Type.GetType(attributeTypes[ListBox_AttributeTypes.SelectedIndex])), 
                Editable = (bool)CheckBox_Edditable.IsChecked, 
                IncludedInShortDescription = (bool)CheckBox_IncludedInShortDescription.IsChecked,
            Changed = !(callingWindow is AddAlgorithmWindow)};

            // Add the delegates
            TypesUtility.AddEndOperationMethod(newAttribute, (string)ListBox_EndInputOperations.SelectedItem);
            TypesUtility.AddElementWindowPrmsMethod(newAttribute, (string)ListBox_ElementWindowPrmsMethods.SelectedItem);
            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void Button_Quit_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief Event handler. Called by Button_Quit for click events.
        ///
        /// \brief #### Description.
        ///        Quit the dialog
        ///        The signal for the quit is that the newAttribute is null
        ///
        /// \brief #### Algorithm.
        ///
        /// \brief #### Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param sender  (object) - Source of the event.
        /// \param e       (RoutedEventArgs) - Routed event information.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            newAttribute = null;
            Close();
        }
        #endregion
    }
}
