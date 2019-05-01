////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    UserInterface\ListInput.xaml.cs
///
///\brief   Implements the list input.xaml class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
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
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * Interaction logic for LogFiltersSetting.xaml.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ListInput
    ///
    /// \brief A list input.
    ///
    /// \par Description.  
    ///      a description.  
    ///      a description.  
    ///      a description. 
    ///
    /// \par Usage Notes.  
    ///      a user note.
    ///
    /// \author Ilanh
    /// \date 21/05/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class ListInput : Window
    {
        /** true if the data is updated. */
        public bool Updated = false;
        /** The list. */
        private object list;
        /** true if editable. */
        private bool editable;
        /** The list for reset to init. */
        private object listForResetToInit;
        /** The list for reset to saved. */
        private object listForResetToSaved;
        /** true to list of attributes. */
        private bool listOfAttributes;
        /** The network. */
        private BaseNetwork network;

        /**********************************************************************************************//**
         * Values that represent index creating operations.
        
         **************************************************************************************************/

        private enum IndexCreatingOperation { Start, Previouse, Current, End}

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list        The list.
         * \param   network     The network.
         * \param   Editable    (Optional) True if editable.
         *                      
         **************************************************************************************************/

        public ListInput(object list, BaseNetwork network, bool Editable = true)
        {
            this.network = network;
            listOfAttributes = list.GetType().GenericTypeArguments[0].Equals(typeof(Attribute));
            this.list = list;
            listForResetToInit = TypesUtility.CreateListFromString(list.GetType().ToString());
            TypesUtility.CopyList(list, listForResetToInit);
            listForResetToSaved = TypesUtility.CreateListFromString(list.GetType().ToString());
            TypesUtility.CopyList(list, listForResetToSaved);
            InitializeComponent();
            if (!listOfAttributes)
            {
                Button_Edit.Visibility = System.Windows.Visibility.Collapsed;
                ComboBox_Replace_SelectValue.Visibility = System.Windows.Visibility.Collapsed;
                ComboBox_SelectValue.Visibility = System.Windows.Visibility.Collapsed;
                ComboBox_SelectCategory.Visibility = System.Windows.Visibility.Collapsed;
                ComboBox_SelectType.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                InitComboBoxes();
            }
            this.Title = "Editting list of type " + list.GetType().GenericTypeArguments[0].ToString();
            FillListBox(list, IndexCreatingOperation.Start);
            editable = Editable;
        }

        /**********************************************************************************************//**
         * Init combo boxes.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void InitComboBoxes()
        {
            FillComboBox(ComboBox_SelectCategory, TypesUtility.GetTypesCategories());
            ComboBox_SelectCategory.SelectedIndex = 0;
            FillComboBox(ComboBox_SelectType, TypesUtility.GetTypesOfCategory((string)ComboBox_SelectCategory.SelectedItem));
            ComboBox_SelectType.SelectedIndex = 0;
        }

        /**********************************************************************************************//**
         * Removes all events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void RemoveAllEvents()
        {
            ComboBox_SelectCategory.SelectionChanged -= ComboBox_SelectCategory_SelectionChanged;
            ComboBox_SelectType.SelectionChanged -= ComboBox_SelectType_SelectionChanged;
            ListBox_Elements.SelectionChanged -= ListBox_Elements_SelectionChanged;
        }

        /**********************************************************************************************//**
         * Adds all events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void AddAllEvents()
        {
            ComboBox_SelectCategory.SelectionChanged += ComboBox_SelectCategory_SelectionChanged;
            ComboBox_SelectType.SelectionChanged += ComboBox_SelectType_SelectionChanged;
            ListBox_Elements.SelectionChanged += ListBox_Elements_SelectionChanged;
        }

        /**********************************************************************************************//**
         * Fill combo box.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   comboBox    The combo box.
         * \param   items       The items.
         *                      
         **************************************************************************************************/

        private void FillComboBox(ComboBox comboBox, List<string> items)
        {
            RemoveAllEvents();
            comboBox.ItemsSource = items;
            AddAllEvents();
        }

        /**********************************************************************************************//**
         * Determines if we can editable check.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool EditableCheck()
        {
            if (!editable)
            {
                CustomizedMessageBox.Show("The list cannot be updated because it is not editable", "List Input Message", Icons.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        /**********************************************************************************************//**
         * Fill list box.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list                    The list.
         * \param   indexChangingOperation  The index changing operation.
         *                                  
         **************************************************************************************************/

        private void FillListBox(object list, IndexCreatingOperation indexChangingOperation)
        {
            int nextIndex;
            switch (indexChangingOperation)
            {
                case IndexCreatingOperation.Previouse: 
                    nextIndex = ListBox_Elements.SelectedIndex - 1; 
                    if (nextIndex < 0)
                    {
                        nextIndex = 0;
                    }
                    break;
                case IndexCreatingOperation.Current: nextIndex = ListBox_Elements.SelectedIndex;break;
                case IndexCreatingOperation.Start: nextIndex = 0;break;
                case IndexCreatingOperation.End: nextIndex = ListBox_Elements.Items.Count; break;
                default: nextIndex = 0; break;
            }
            if (indexChangingOperation == IndexCreatingOperation.Current)
            {

            }
            RemoveAllEvents();
            object itemsArray = TypesUtility.InvokeMethodOfList(list, "ToArray", null, true);
            ListBox_Elements.Items.Clear();

                foreach (var item in itemsArray as Array)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = item.ToString();
                    ListBox_Elements.Items.Add(listBoxItem);
                }
            
            //Force change of index so that the replace fields will update
            ListBox_Elements.SelectedIndex = -1;
            AddAllEvents();
            ListBox_Elements.SelectedIndex = nextIndex;
            if (ListBox_Elements.SelectedIndex == -1)
            {
                ListBox_Elements_SelectionChanged(null, null);
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_ResetToSaved for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_ResetToSaved_Click(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            TypesUtility.CopyList(listForResetToSaved, list);
            FillListBox(list, IndexCreatingOperation.Start);
            ListBox_Elements.UpdateLayout();
            CustomizedMessageBox.Show("Finished Reset to Saved", "List Input Message", Icons.Success);
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_ResetToInit for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_ResetToInit_Click(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            TypesUtility.CopyList(listForResetToInit, list);
            FillListBox(list, IndexCreatingOperation.Start);
            ListBox_Elements.UpdateLayout();
            Updated = false;
            CustomizedMessageBox.Show("Finished Reset to init", "List Input Message", Icons.Success);
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Remove for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            if (((IList)list).Count > 0)
            {
                TypesUtility.InvokeMethodOfList(list, "RemoveAt", new object[] { ListBox_Elements.SelectedIndex }, false);
                FillListBox(list, IndexCreatingOperation.Previouse);
                Updated = true;
            }
            else
            {
                CustomizedMessageBox.Show("The list is already empty", "List Input Message", Icons.Error);
                return;
            }  
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Add for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            bool additionalSuccess;
            object newValue;
            if (listOfAttributes)
            {
                additionalSuccess = CreateAttribute(out newValue);
            }
            else
            {
                additionalSuccess = CreateValue(out newValue);
            }
            if (additionalSuccess)
            {
                TypesUtility.InvokeMethodOfList(list, "Add", new object[] { newValue }, false);
                FillListBox(list, IndexCreatingOperation.End);
                ListBox_Elements.SelectedIndex = ListBox_Elements.Items.Count - 1;
                if (Updated == false)
                {
                    Updated = true;
                }
            }
        }

        /**********************************************************************************************//**
         * Creates a value.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param [out] newValue    The new value.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool CreateValue(out object newValue)
        {
            bool converted;
            newValue = TypesUtility.Parse(list.GetType().GenericTypeArguments[0], TextBox_AddValue.Text, out converted);
            if (converted)
            {
                return true;
            }
            else
            {
                CustomizedMessageBox.Show("Cannot convert value " + TextBox_AddValue.Text + " to type " + list.GetType().GenericTypeArguments[0].ToString(), "List Input Message", Icons.Error);
                return false;
            }            
        }

        /**********************************************************************************************//**
         * Creates an attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param [out] newValue    The new value.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        private bool CreateAttribute(out object newValue)
        {
            string valueString;
            Attribute attribute;
            if (!ComboBox_SelectValue.IsVisible)
            {
                valueString = TextBox_AddValue.Text;
            }
            else
            {
                valueString = (string)ComboBox_SelectValue.SelectedItem;
            }
            if (TypesUtility.CreateAttribute((string)ComboBox_SelectCategory.SelectedItem,
                (string)ComboBox_SelectType.SelectedItem,
                valueString, out attribute))
            {
                newValue = attribute;
                return true;
            }
            else
            {
                CustomizedMessageBox.Show("Cannot convert value " + valueString + " to type " + (string)ComboBox_SelectType.SelectedItem, "List Input Message", Icons.Error);
                newValue = null;
                return false;
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Save for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            TypesUtility.CopyList(list, listForResetToSaved);
            ListBox_Elements.UpdateLayout();
            CustomizedMessageBox.Show("Finished Saving", "List Input Message", Icons.Success);
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Exit for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /**********************************************************************************************//**
         * Exit operations.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void ExitOperations()
        {
            if (Updated)
            {
                MessageBoxResult dialogResult = CustomizedMessageBox.Show("There is unsaved data do you want to exit or to save first?", "Data not saved dialog", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.No)
                {
                    Button_ResetToInit_Click(null, null);
                    Updated = false;
                }
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_Edit for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if(!EditableCheck()) return;
            if(listOfAttributes)
            {
                Attribute attribute = ((List<Attribute>)list)[ListBox_Elements.SelectedIndex];
                Type type = attribute.Value.GetType();
                if (typeof(IList).IsAssignableFrom(type))
                {
                    ListInput listInput = new ListInput(attribute.Value, network, editable);
                    listInput.ShowDialog();
                    return;
                }
                if (typeof(NetworkElement).IsAssignableFrom(type))
                {
                    ElementInputWindow elementInput = new ElementInputWindow(attribute.Value);
                    elementInput.ShowDialog();
                    return;
                }
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by Button_ReplaceValue for click events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Routed event information.
         
         **************************************************************************************************/

        private void Button_ReplaceValue_Click(object sender, RoutedEventArgs e)
        {
            if (!EditableCheck()) return;
            IndexCreatingOperation indexCreatingOperation;
            if (listOfAttributes)
            {
                ReplaceValueInAttribute();
                indexCreatingOperation = IndexCreatingOperation.Current;
            }
            else
            {
                indexCreatingOperation = replaceValue();
            }
            FillListBox(list, indexCreatingOperation);
            Updated = true;
        }

        /**********************************************************************************************//**
         * Replace value.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  An IndexCreatingOperation.
         .
         **************************************************************************************************/

        private IndexCreatingOperation replaceValue()
        {
            bool converted;
            object value = TypesUtility.Parse(list.GetType().GenericTypeArguments[0], TextBox_Replace_AddValue.Text, out converted);
            if (converted)
            {
                if (ListBox_Elements.SelectedIndex >= 0)
                {
                    TypesUtility.InvokeMethodOfList(list, "RemoveAt", new object[] { ListBox_Elements.SelectedIndex }, false);
                    TypesUtility.InvokeMethodOfList(list, "Insert", new object[] { ListBox_Elements.SelectedIndex, value }, false);
                    return IndexCreatingOperation.Current;
                }
                else
                {
                    TypesUtility.InvokeMethodOfList(list, "Add", new object[] { value }, false);
                    return IndexCreatingOperation.Start;
                }
            }
            else
            {
                CustomizedMessageBox.Show("Cannot convert value " + TextBox_AddValue.Text + " to type " + list.GetType().GenericTypeArguments[0].ToString(), "List Input Message", Icons.Error);
                return IndexCreatingOperation.Start;
            }
        }

        /**********************************************************************************************//**
         * Replace value in attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private void ReplaceValueInAttribute()
        {
            if (ListBox_Elements.SelectedIndex == -1)
            {
                CustomizedMessageBox.Show("No selected item", "List Input Message", Icons.Error);
                return;
            }
            string newValueString;
            Attribute attribute = ((List<Attribute>)list)[ListBox_Elements.SelectedIndex];
            if (!ComboBox_Replace_SelectValue.IsVisible)
            {
                newValueString = TextBox_Replace_AddValue.Text;
            }
            else
            {
                newValueString = (string)ComboBox_Replace_SelectValue.SelectedItem;
            }
            Type attributeType = attribute.Value.GetType();
            if (attributeType.IsPrimitive || attributeType.Equals(typeof(string)) || attributeType.IsEnum)
            {
                bool converted;
                dynamic newValue = TypesUtility.ConvertValueFromString(attributeType, newValueString, out converted);
                if (!converted)
                {
                    CustomizedMessageBox.Show("The value of new value string: " + newValueString + " cannot be assigned to the attribute type: ", attributeType.ToString(), Icons.Error);
                }
                else
                {
                    attribute.Value = newValue;
                }
            }
            else
            {
                CustomizedMessageBox.Show("Attributes from types NetworkElement and List cannot be replaced they can only be removed and added ", "List Input Message", Icons.Error);
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by ComboBox_SelectCategory for selection changed events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Selection changed event information.
         
         **************************************************************************************************/

        private void ComboBox_SelectCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_SelectType.SelectionChanged -= ComboBox_SelectType_SelectionChanged;
            FillComboBox(ComboBox_SelectType, TypesUtility.GetTypesOfCategory((string)ComboBox_SelectCategory.SelectedItem));
            ComboBox_SelectType.SelectedIndex = 0;
            ComboBox_SelectType.SelectionChanged += ComboBox_SelectType_SelectionChanged;
            FillComboBox(ComboBox_SelectValue, TypesUtility.GetPossibleValues((string)ComboBox_SelectType.SelectedItem));
            if (ComboBox_SelectValue.Items.Count != 0)
            {
                ComboBox_SelectValue.SelectedIndex = 0;
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by ComboBox_SelectType for selection changed events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Selection changed event information.
         
         **************************************************************************************************/

        private void ComboBox_SelectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillComboBox(ComboBox_SelectValue, TypesUtility.GetPossibleValues((string)ComboBox_SelectType.SelectedItem));
            if (ComboBox_SelectValue.Items.Count != 0)
            {
                ComboBox_SelectValue.SelectedIndex = 0;
            }
        }

        /**********************************************************************************************//**
         * Event handler. Called by ListBox_Elements for selection changed events.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sender  Source of the event.
         * \param   e       Selection changed event information.
         
         **************************************************************************************************/

        private void ListBox_Elements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfAttributes)
            {
                Type type;
                string valueString;
                Attribute attribute;
                if (ListBox_Elements.SelectedItem == null)
                {
                    type = typeof(string);
                    valueString = "";
                }
                else
                {
                    attribute = ((List<Attribute>)list)[ListBox_Elements.SelectedIndex];
                    type = attribute.Value.GetType();
                    valueString = attribute.GetValueToString();
                }
                if (type.IsEnum || type.Equals(typeof(bool)))
                {
                    FillComboBox(ComboBox_Replace_SelectValue, TypesUtility.GetPossibleValues(type.ToString()));
                    ComboBox_Replace_SelectValue.SelectedItem = valueString;
                    ComboBox_Replace_SelectValue.Visibility = System.Windows.Visibility.Visible;
                    TextBox_Replace_AddValue.Visibility = System.Windows.Visibility.Collapsed;
                    Button_Edit.Visibility = System.Windows.Visibility.Collapsed;
                    Button_ReplaceValue.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
                if (type.IsPrimitive || type.Equals(typeof(string)))
                {
                    TextBox_Replace_AddValue.Text = valueString;
                    ComboBox_Replace_SelectValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBox_Replace_AddValue.Visibility = System.Windows.Visibility.Visible;
                    Button_Edit.Visibility = System.Windows.Visibility.Collapsed;
                    Button_ReplaceValue.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
                ComboBox_Replace_SelectValue.Visibility = System.Windows.Visibility.Collapsed;
                TextBox_Replace_AddValue.Visibility = System.Windows.Visibility.Collapsed;
                Button_Edit.Visibility = System.Windows.Visibility.Visible;
                Button_ReplaceValue.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (ListBox_Elements.SelectedItem != null)
                {
                    TextBox_Replace_AddValue.Text = ((ListBoxItem)(ListBox_Elements.SelectedValue)).Content.ToString();
                }
                else
                {
                    TextBox_Replace_AddValue.Text = TypesUtility.GetDefault(list.GetType().GenericTypeArguments[0]).ToString();
                    TextBox_AddValue.Text = TypesUtility.GetDefault(list.GetType().GenericTypeArguments[0]).ToString();
                }
            }

        }

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
            ExitOperations();
        }
    }
}
