////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\Attribute.cs
///
///\brief   Implements the attribute class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using DistributedAlgorithms.Algorithms.Base.Base;


namespace DistributedAlgorithms
{
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class Attribute
    ///
    /// \brief An attribute.
    ///
    /// \par Description.
    ///      This class is the only value holder class in the application.
    ///
    /// \par Usage Notes.
    ///      The attribute has 5 public members
    ///      -#   The value (dynamic)
    ///      -#   Editable : In ElementWindow whether the value of the attribute can be changed
    ///      -#   IncludedInShortDescription : In several usage whether to include the attribute
    ///      -#   EndInputOperation : In ElementWindow actions (like checking) for activating when
    ///           the value is changed
    ///      -#   ElementWindowPrmsMethod : For ElementWindow - the parameters that sets the way
    ///             the controls in the ElementWindow will be created.
    ///
    /// \author Ilanh
    /// \date 25/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Attribute : IValueHolder
    {

        #region /// \name Delegates

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public delegate bool EndInputOperationDelegate(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null);
        ///
        /// \brief Ends input operation.
        ///        This delegate is used to declare and hold a method that will check or perform
        ///        any other operation when a new value is inserted to the attribute in ElementWindow.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param       network         (BaseNetwork) - The network.
        /// \param       networkElement  (NetworkElement) - The network element.
        /// \param       parentAttribute (Attribute) - The parent attribute.
        /// \param       attribute       (Attribute) - The attribute.
        /// \param       newValue        (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        /// \param       inputWindow     (Optional)  (ElementWindow) - The input window.
        ///
        /// \return A bool.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public delegate bool EndInputOperationDelegate(BaseNetwork network,
            NetworkElement networkElement,
            Attribute parentAttribute,
            Attribute attribute,
            string newValue,
            out string errorMessage,
            ElementWindow inputWindow = null);

        #endregion
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum AttributeCategory
        ///
        /// \brief Values that represent attribute categories.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum AttributeCategory { Null, PrimitiveAttribute, NetworkElementAttribute, AttributeDictionary, ListOfAttributes };

        #endregion
        #region /// \name Members

        /// \brief (dynamic) - The value.
        private dynamic value = null;

        /// \brief (bool) - true if editable in element input window.
        public bool Editable = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true if the data is changed.
        ///        This member is used in the NetworkUpdate process. It indicates that
        ///        the attribute was changed during design phase and that the value should
        ///        be copied when the network is been updated.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Changed = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to identifier in list.
        ///        This member is used in the NetworkUpdate process. It is used to identify
        ///        the attribute between the new and existing lists. (In the NetworkUpdate process
        ///        the exists list is scanned. if an attribute was changed the id makes it possible
        ///        to identify the equivalent attribute in the new list)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int IdInList = 0;

        /// \brief (bool) - true to included in short description.
        public bool IncludedInShortDescription = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (bool) - true to include, false to exclude the in equals to.
        ///        Whether to include the Attribute when comparing 2 IValueHolders.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IncludeInEqualsTo = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (EndInputOperationDelegate) - The end input operation in element input window.
        ///        Actions to be done (like checking) after the attribute was changed was made
        ///        in the input field for the attribute in ElementWindow.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EndInputOperationDelegate EndInputOperation = DefaultEndInputOperations;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (InputFieldDelegate) - Determine the presentation and input controls in ElementWindow
        ///        or it's inheritors.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ElementWindowPrmsDelegate ElementWindowPrmsMethod = DefaultElementWindowPrmsMethod;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Permissions) - The permissions.
        ///        Define the permissions for add/remove/clear/set actions according to the program
        ///        activation phase.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Permissions permissions;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (IValueHolder) - The parent. The parent is the AttributeDictionary/AttributeList/NetworkElement
        ///        that the attribute is included in.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private IValueHolder parent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (BaseNetwork) - The network.
        ///         The Network that is handled (presented, running, etc.)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private BaseNetwork network;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (NetworkElement) - The element.
        ///        The NetworkElement that the attribute belongs to (network, process etc.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private NetworkElement element;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (ElementWindow) - The value set sender window. When setting a value from an ElementWindow
        ///        this variable is set to the window that asked for the setting. This variable is then
        ///        used by the CheckPermissions to check if the window actually presented on the screen.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ElementWindow valueSetSenderWindow = null;

        
        #endregion
        #region /// \name IValueHolder Getters

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public BaseNetwork Network
        ///
        /// \brief Gets the network.
        ///
        /// \return The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseNetwork Network { get { return network; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public NetworkElement Element
        ///
        /// \brief Gets the element.
        ///
        /// \return The element.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NetworkElement Element { get { return element; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public Permissions Permissions
        ///
        /// \brief Gets the permissions.
        ///
        /// \return The permissions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Permissions Permissions { get { return permissions; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public IValueHolder Parent
        ///
        /// \brief Gets the parent.
        ///
        /// \return The parent.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IValueHolder Parent { get { return parent; } }


        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///      The attributes are created and assigned in 2 phases:
        /// \par
        ///     -#  First created using the constructor that is convenient for the generating
        ///     -#  Second insert the attribute to AttributeDictionary/AttributeList/NetworkElement
        ///         with a constructor that passes the:
        ///         -#  The Network
        ///         -#  The Permissions
        ///         -#  The Attribute that should be Added
        ///         -#  The Parent (The AttributeDictionary/AttributeList/NetworkElement
        ///             that the attribute is added to)

        /// \par         
        ///      This constructor is one of the constructors in the first phase.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute()
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
        /// \date 04/07/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute()
        {
            permissions = new Permissions(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute(Attribute attribute)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      The attributes are created and assigned in 2 phases:
        ///
        /// \par -#  First created using the constructor that is convenient for the generating
        ///      -#  Second insert the attribute to AttributeDictionary/AttributeList/NetworkElement
        ///          with a constructor that passes the:
        ///          -#  The Network
        ///          -#  The Permissions
        ///          -#  The Attribute that should be Added
        ///          -#  The Parent (The AttributeDictionary/AttributeList/NetworkElement
        ///              that the attribute is added to)
        ///
        /// \par This constructor is one of the constructors in the first phase.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \par Algorithm.
        ///
        /// \param attribute (Attribute) - The attribute.
        ///
        /// \par Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute(Attribute attribute)
        {
            value = attribute.Value;
            Editable = attribute.Editable;
            IncludedInShortDescription = attribute.IncludedInShortDescription;
            EndInputOperation = attribute.EndInputOperation;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute(Permissions permissions, IValueHolder parent)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      The attributes are created and assigned in 2 phases:
        ///
        /// \par -#  First created using the constructor that is convenient for the generating
        ///      -#  Second insert the attribute to AttributeDictionary/AttributeList/NetworkElement
        ///          with a constructor that passes the:
        ///          -#  The Network
        ///          -#  The Permissions
        ///          -#  The Attribute that should be Added
        ///          -#  The Parent (The AttributeDictionary/AttributeList/NetworkElement
        ///              that the attribute is added to)
        ///
        /// \par This constructor is one of the constructors in the first phase.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \par Algorithm.
        ///
        /// \param permissions (Permissions) - The permissions.
        /// \param parent     (IValueHolder) - The parent.
        ///
        /// \par Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute(Permissions permissions, IValueHolder parent)
        {
            this.parent = parent;
            this.permissions = new Permissions(permissions);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute(BaseNetwork network, Permissions permissions, IValueHolder parent)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      The attributes are created and assigned in 2 phases:
        ///
        /// \par -#  First created using the constructor that is convenient for the generating
        ///      -#  Second insert the attribute to AttributeDictionary/AttributeList/NetworkElement
        ///          with a constructor that passes the:
        ///          -#  The Network
        ///          -#  The Permissions
        ///          -#  The Attribute that should be Added
        ///          -#  The Parent (The AttributeDictionary/AttributeList/NetworkElement
        ///              that the attribute is added to)
        ///
        /// \par This constructor is one of the constructors in the first phase.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \par Algorithm.
        ///
        /// \param network     (BaseNetwork) - The network.
        /// \param permissions (Permissions) - The permissions.
        /// \param parent     (IValueHolder) - The parent.
        ///
        /// \par Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute(BaseNetwork network, Permissions permissions, IValueHolder parent)
        {
            this.network = network;
            this.parent = parent;
            this.permissions = new Permissions(permissions);
        }

        public Attribute(BaseNetwork network, Permissions permissions, IValueHolder parent, IValueHolder value, bool recursive = true)
        {
            this.value = value;
            SetMembers(network, element, permissions, parent, recursive);
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute(BaseNetwork network, Permissions permissions, Attribute attribute, IValueHolder parent)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      The attributes are created and assigned in 2 phases:
        ///
        /// \par -#  First created using the constructor that is convenient for the generating
        ///      -#  Second insert the attribute to AttributeDictionary/AttributeList/NetworkElement
        ///          with a constructor that passes the:
        ///          -#  The Network
        ///          -#  The Permissions
        ///          -#  The Attribute that should be Added
        ///          -#  The Parent (The AttributeDictionary/AttributeList/NetworkElement
        ///              that the attribute is added to)
        ///
        /// \par This constructor is one of the constructors in the second phase.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \par Algorithm.
        ///
        /// \param network    (BaseNetwork) - The network.
        /// \param permissions (Permissions) - The permissions.
        /// \param attribute  (Attribute) - The attribute.
        /// \param parent     (IValueHolder) - The parent.
        ///
        /// \par Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute(BaseNetwork network, Permissions permissions, Attribute attribute, IValueHolder parent)
        {
            this.network = network;
            this.parent = parent;
            this.permissions = new Permissions(permissions);
            SetValue(attribute.Value);
            Editable = attribute.Editable;
            IncludedInShortDescription = attribute.IncludedInShortDescription;
            EndInputOperation = attribute.EndInputOperation;
        }

        #endregion
        #region /// \name Value setting

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetFromWindow(dynamic value, ElementWindow sender)
        ///
        /// \brief Sets from window.
        ///
        /// \par Description.
        ///      This method is used to set the value of the attribute from the Gui.
        ///      It bypasses the permissions.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param value  (dynamic) - The value.
        /// \param sender (ElementWindow) - The sender.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetFromWindow(dynamic value, ElementWindow sender)
        {
            valueSetSenderWindow = sender;
            SetValue(value);
            valueSetSenderWindow = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public dynamic Value
        ///
        /// \brief Gets or sets the value.
        ///
        /// \return The value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic Value
        {
            get
            {
                return this.value;
            }
            set
            {
                SetValue(value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetValue(dynamic value)
        ///
        /// \brief Sets a value.
        ///
        /// \par Description.
        ///      Activated by the Setter.
        ///
        /// \par Algorithm.
        ///      -  The following are the check done:
        ///         -# The type is supported type (IvalueHolder or simple type)
        ///         -# The new value can be assigned to the type of the old value
        ///         -# There is a permission to set the value
        ///      -  The following is the setting method  
        ///         -#  If the value is IValueHolder use the constructor for setting
        ///         -#  Else (The value is simple) - set the value.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param value (dynamic) - The value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetValue(dynamic value)
        {
            if (!AttributeTypeIsLeagle(value))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to set value to : " + TypesUtility.GetKeyInParent(parent, this),
                        "Attribute Type " + value.GetType().ToString() + " Not Supported" },
                    "Attribute - Value method", null, Icons.Error);
                return;
            }

            if (this.value != null)
            {
                if (!CanBeAssigned(value))
                {
                    MessageRouter.MessageBox(new List<string> { "Failed to set value to : " + TypesUtility.GetKeyInParent(parent, this),
                            "The current value type is " + this.value.GetType().ToString() + " is not convertible to the new value type which is " + value.GetType().ToString() },
                        "Attribute Set", null, Icons.Error);
                    return;
                }
            }

            if (!CheckPermissions())
            {
                MessageRouter.MessageBox(new List<string> { "Failed to set value to : " + TypesUtility.GetKeyInParent(parent, this) + " because of set permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            " Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.Set),
                            permissions.ToString()},
                        "Attribute Set", null, Icons.Error);
                return;
            }

            this.value = value;

            // The following command will set the members to all the tree under value
            if (!(value is null))
            {
                if (typeof(IValueHolder).IsAssignableFrom(value.GetType()))
                {
                    value.SetMembers(network, element, permissions, this);
                }
            }

            // Handle the Changed attribute
            // The Changed attribute is used in the NetworkUpdate algorithm.
            // It indicates that the attribute was changed during design and should be kept when
            // Creating an updated network
            if (!(valueSetSenderWindow is AddAlgorithmWindow))
            {
                Changed = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CanBeAssigned(dynamic value)
        ///
        /// \brief Determine if the new value can be assigned to the type of the value that already exist
        ///        in the attribute.
        ///
        /// \par Description.
        ///      A new value can be assigned if :
        ///      -# The value of the attribute is null
        ///      -# The existing value is primitive and one of the following is true
        ///         -#  The value of the attribute is number and so the value to be assigned
        ///         -#  The new value can be assigned to the existing value (This is done by taking
        ///             the string of the new value and try to parse it using the type of the existing value)  
        ///      -# If the value is a string and can be parsed to value from the type of the current value
        ///        If the type of the value and the current value is the same.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param value (dynamic) - The value.
        ///
        /// \return True if we can be assigned, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CanBeAssigned(dynamic value)
        {
            Type type = value.GetType();
            switch (GetValueCategory(this))
            {
                case AttributeCategory.PrimitiveAttribute:
                    if (TypesUtility.IsNumeric(value.GetType()) && TypesUtility.IsNumeric(this.value.GetType()))
                    {
                        return true;
                    }
                    if (CheckIfStringCanBeParssed(value.ToString()))
                    {
                        return true;
                    }
                    break;
                case AttributeCategory.Null:
                    return true;
                default:
                    if (((Type)this.value.GetType()).IsAssignableFrom(value.GetType()) ||
                        ((Type)value.GetType()).IsAssignableFrom(this.value.GetType()))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool AttributeTypeIsLeagle(object value)
        ///
        /// \brief Attribute type is legal.
        ///
        /// \par Description.
        ///        The type can be:
        ///        -#   A primitive ( a simple value)
        ///        -#   A string
        ///        -#   An Enum
        ///        -#   A type that implements IValueHolder.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param value (object) - The value.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool AttributeTypeIsLeagle(object value)
        {
            if (value is null)
            {
                return true;
            }
            System.Type type = value.GetType();
            if (type.IsPrimitive)
            {
                return true;
            }
            if (type.Equals(typeof(string)))
            {
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (typeof(IValueHolder).IsAssignableFrom(type))
            {
                return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool CheckPermissions()
        ///
        /// \brief Check the permissions for setting a value.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CheckPermissions()
        {
            if (!permissions.CheckPermition(Permissions.PermitionTypes.Set, valueSetSenderWindow))
            {
                return false;
            }
            return true;
        }
        #endregion
        #region /// \name Value utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GetTypeToString()
        ///
        /// \brief Gets type to string.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \return The type to string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetTypeToString()
        {
            return Value.GetType().ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Type GetValueType()
        ///
        /// \brief Gets value type.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \return The value type.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Type GetValueType()
        {
            return value.GetType();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string GetValueToString()
        ///
        /// \brief Gets value to string.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \return The value to string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetValueToString()
        {
            if (Value.GetType().IsEnum)
            {
                return TypesUtility.GetKeyToString(Value);
            }
            return Value.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetValueFromString(string valueString, ElementWindow sender = null)
        ///
        /// \brief Sets value from string.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param valueString (string) - The value string.
        /// \param sender      (Optional)  (ElementWindow) - The sender.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetValueFromString(string valueString, ElementWindow sender = null)
        {
            valueSetSenderWindow = sender;
            bool converted;
            dynamic tempValue = TypesUtility.ConvertValueFromString(Value.GetType(), valueString, out converted);
            if (converted)
            {
                // If the result is null that means that the element is a complex element
                // (list NetworkElement or AttributeDictionary). In this case because the attribute
                // was already created there is no need to assign a value
                if (tempValue != null)
                {
                    Value = tempValue;
                }
            }
            valueSetSenderWindow = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool CheckIfStringCanBeParssed(string valueString)
        ///
        /// \brief Determine if string can be parsed to the current value type.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param valueString (string) - The value string.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CheckIfStringCanBeParssed(string valueString)
        {
            try
            {
                bool converted;
                var tempValue = TypesUtility.ConvertValueFromString(Value.GetType(), valueString, out converted);
                return converted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetDefaultValue(string typeString)
        ///
        /// \brief Sets default value.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param typeString (string) - The type string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetDefaultValue(string typeString)
        {
            Type type = TypesUtility.GetTypeFromString(typeString);
            if (typeof(IValueHolder).IsAssignableFrom(type))
            {
                Value = TypesUtility.CreateObjectFromTypeString(typeString);
            }
            else
            {
                Value = TypesUtility.GetDefault(type);
            }
            if (typeof(NetworkElement).IsAssignableFrom(type))
            {
                Value.Init(0);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static AttributeCategory GetValueCategory(object attribute)
        ///
        /// \brief Gets value category.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param attribute (object) - The attribute.
        ///
        /// \return The value category.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static AttributeCategory GetValueCategory(object attribute)
        {
            if (attribute == null)
            {
                return AttributeCategory.Null;
            }
            Type attributeType = ((Attribute)attribute).Value.GetType();
            if (typeof(AttributeList).IsAssignableFrom(attributeType))
            {
                return AttributeCategory.ListOfAttributes;
            }
            if (typeof(NetworkElement).IsAssignableFrom(attributeType))
            {
                return AttributeCategory.NetworkElementAttribute;
            }
            if (typeof(AttributeDictionary).IsAssignableFrom(attributeType))
            {
                return AttributeCategory.AttributeDictionary;
            }
            return AttributeCategory.PrimitiveAttribute;
        }
        #endregion
        #region /// \name IValueHolder Load and Save

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Load(XmlNode xmlNode, Type hostType)
        ///
        /// \brief Loads.
        ///        Load the object from XML doc (or file)
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param xmlNode  (XmlNode) - The XML node.
        /// \param hostType (Type) - Type of the host.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Load(XmlNode xmlNode, Type hostType)
        {
            string typeString = xmlNode.Attributes["Type"].Value;
            if (!LoadValue(xmlNode, typeString, hostType))
            {
                return false;
            }
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                switch (xmlAttribute.Name)
                {
                    case "EndInputOperation":
                        TypesUtility.AddEndOperationMethod(this, xmlAttribute.Value);
                        break;
                    case "ElementWindowPrmsMethod":
                        TypesUtility.AddElementWindowPrmsMethod(this, xmlAttribute.Value);
                        break;
                    case "Editable":
                        Editable = bool.Parse(xmlAttribute.Value);
                        break;
                    case "Changed":
                        Changed = bool.Parse(xmlAttribute.Value);
                        break;
                    case "IdInList":
                        IdInList = int.Parse(xmlAttribute.Value);
                        break;
                    case "IncludedInShortDescription":
                        IncludedInShortDescription = bool.Parse(xmlAttribute.Value);
                        break;
                    case "IncludeInEqualsTo":
                        IncludeInEqualsTo = bool.Parse(xmlAttribute.Value);
                        break;
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool LoadValue(XmlNode xmlAttributeNode, string typeString)
        ///
        /// \brief Loads a value.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param xmlAttributeNode (XmlNode) - The XML attribute node.
        /// \param typeString       (string) - The type string.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool LoadValue(XmlNode xmlAttributeNode, string typeString, Type hostType)
        {
            Type type = TypesUtility.GetTypeFromString(typeString);
            XmlNode valueNode = xmlAttributeNode.ChildNodes.Item(0);
            if (typeof(IValueHolder).IsAssignableFrom(Type.GetType(typeString)))
            {
                Value = TypesUtility.CreateObjectFromTypeString(typeString, null);
                if (!((IValueHolder)value).Load(valueNode.ChildNodes.Item(0), hostType))
                {
                    return false;
                }
            }
            else
            {
                bool converted;
                value = TypesUtility.ConvertValueFromString(type, valueNode.ChildNodes.Item(0).InnerText, out converted);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode)
        ///
        /// \brief Saves.
        ///        Saves the object to XML doc (or file)
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param xmlDoc  (XmlDocument) - The XML document.
        /// \param xmlNode (XmlNode) - The XML node.
        ///
        /// \return An XmlNode.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode)
        {
            // The value is in a child node
            xmlNode.AppendChild(SaveValue(xmlDoc));

            //The rest of the fields are attributes
            XmlAttribute xmlTypeAttribute = xmlDoc.CreateAttribute("Type");
            xmlTypeAttribute.Value = Value.GetType().ToString();
            xmlNode.Attributes.Append(xmlTypeAttribute);
            XmlAttribute xmlEditableAttribute = xmlDoc.CreateAttribute("Editable");
            xmlEditableAttribute.Value = Editable.ToString();
            xmlNode.Attributes.Append(xmlEditableAttribute);
            XmlAttribute xmlChangedAttribute = xmlDoc.CreateAttribute("Changed");
            xmlChangedAttribute.Value = Changed.ToString();
            xmlNode.Attributes.Append(xmlChangedAttribute);
            XmlAttribute xmlIdInListAttribute = xmlDoc.CreateAttribute("IdInList");
            xmlIdInListAttribute.Value = IdInList.ToString();
            xmlNode.Attributes.Append(xmlIdInListAttribute);
            XmlAttribute xmlIncludedInShortDescriptionAttribute = xmlDoc.CreateAttribute("IncludedInShortDescription");
            xmlIncludedInShortDescriptionAttribute.Value = IncludedInShortDescription.ToString();
            xmlNode.Attributes.Append(xmlIncludedInShortDescriptionAttribute);
            XmlAttribute xmlIncludeInEqualsToAttribute = xmlDoc.CreateAttribute("IncludeInEqualsTo");
            xmlIncludeInEqualsToAttribute.Value = IncludeInEqualsTo.ToString();
            xmlNode.Attributes.Append(xmlIncludeInEqualsToAttribute);
            XmlAttribute xmlEndInputOperationAttribute = xmlDoc.CreateAttribute("EndInputOperation");
            xmlEndInputOperationAttribute.Value = TypesUtility.GetEndInputOperationName(this);
            xmlNode.Attributes.Append(xmlEndInputOperationAttribute);
            XmlAttribute xmlInputFieldAttribute = xmlDoc.CreateAttribute("ElementWindowPrmsMethod");
            xmlInputFieldAttribute.Value = TypesUtility.GetElementWindowPrmsMethodName(this);
            xmlNode.Attributes.Append(xmlInputFieldAttribute);
            return xmlNode;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected XmlNode SaveValue(XmlDocument xmlDoc)
        ///
        /// \brief Saves a value.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param xmlDoc (XmlDocument) - The XML document.
        ///
        /// \return An XmlNode.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected XmlNode SaveValue(XmlDocument xmlDoc)
        {
            XmlNode xmlNode = xmlDoc.CreateElement("Value");
            XmlNode valueNode;
            object valueAsObject = Value;
            if (typeof(IValueHolder).IsAssignableFrom(value.GetType()))
            {
                valueNode = xmlDoc.CreateElement(value.GetType().ToString().Replace("DistributedAlgorithms.", ""));
                ((IValueHolder)value).Save(xmlDoc, valueNode);
            }
            else
            {
                valueNode = xmlDoc.CreateElement("Value");
                valueNode.InnerText = GetValueToString();
            }
            xmlNode.AppendChild(valueNode);
            return xmlNode;
        }
        #endregion
        #region /// \name IValueHolder Scanning Operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ScanAndReport(IScanConsumer scanConsumer, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, dynamic key = null)
        ///
        /// \brief Scans and report.
        ///
        /// \par Description.
        ///      This method is part of a scanning of a NetworkElement
        ///      The scanning is initiated by the scanConsumer and this method reports to it about
        ///      the attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param scanConsumer   (IScanConsumer) - The scan consumer.
        /// \param mainDictionary (string) - The name.
        /// \param dictionary     (ElementDictionaries) - The dictionary.
        /// \param key            (Optional)  (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ScanAndReport(IScanConsumer scanConsumer,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            dynamic key = null)
        {
            if (scanConsumer.ScanCondition(key, this, mainDictionary, dictionary))
            {
                Type type = value.GetType();
                if (type.IsPrimitive || type.Equals(typeof(string)) || type.IsEnum)
                {
                    scanConsumer.AttributeReport(key, this, mainDictionary, dictionary);
                }
                else
                {
                    scanConsumer.OpenComplexAttribute(key, this, mainDictionary, dictionary);
                    ((IValueHolder)Value).ScanAndReport(scanConsumer, mainDictionary, dictionary);
                    scanConsumer.CloseComplexAttribute(key, this, mainDictionary, dictionary);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool print, bool checkNotSameObject = false)
        ///
        /// \brief Equals to.
        ///
        /// \par Description.
        ///        This method is part of a recursive scanning of an IValueHolder to check
        ///        whether all the values of 2 IValueHolders are equal.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      - If you want to start a compare you should call : `ValueHolder.EqualsTo()`
        ///      - The method does the common checks and call this method to propagate the compare to the children.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param          nestingLevel       (int) - The nesting level.
        /// \param [in,out] error              (ref string) - The error.
        /// \param          other              (IValueHolder) - The attribute.
        /// \param          print               (bool) - true to print.
        /// \param          checkNotSameObject (Optional)  (bool) - true to check not same object.
        ///
        /// \return True if equals to, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool print = false, bool checkNotSameObject = false)
        {
            if (!IncludeInEqualsTo)
            {
                return true;
            }
            dynamic otherValue = ((Attribute)other).Value;
            Type type = value.GetType();
            if (!type.Equals(otherValue.GetType()))
            {
                error = "The type of the value of one is : " + type.ToString() + " And the type of the value of two is : " + otherValue.GetType().ToString();
                return false;
            }

            if (type.IsPrimitive || type.Equals(typeof(string)) || type.IsEnum)
            {
                if (Value != otherValue)
                {
                    error = "The value of one is : " + Value.ToString() + " And the value of two is " + otherValue.ToString();
                    return false;
                }
            }
            else
            {
                // IValueHolder
                if (!((IValueHolder)Value).EqualsTo(nestingLevel + 1, ref error, otherValue, print, checkNotSameObject))
                {
                    return false;
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DeepCopy(IValueHolder source, bool isInitiator = false )
        ///
        /// \brief Deep copy.
        ///
        /// \par Description.
        ///      Copy all the attribute tree under the source.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param source      (IValueHolder) - Source for the.
        /// \param isInitiator (Optional)  (bool) - true if this object is initiator.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DeepCopy(IValueHolder source, bool isInitiator = true)
        {
            Type sourceValueType = ((Attribute)source).Value.GetType();
            CopyPublicMembers((Attribute)source);
            if (typeof(IValueHolder).IsAssignableFrom(sourceValueType))
            {
                value = TypesUtility.CreateObjectFromTypeString(sourceValueType.ToString());
                value.DeepCopy(((Attribute)source).Value, false);
            }
            else
            {
                value = ((Attribute)source).Value;
            }

            if (isInitiator)
            {
                SetMembers(network, element, permissions, parent);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CopyPublicMembers(Attribute source)
        ///
        /// \brief Copies the public members described by source.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/05/2018
        ///
        /// \param source Another instance to copy.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CopyPublicMembers(Attribute source)
        {
            Editable = source.Editable;
            Changed = source.Changed;
            IdInList = source.IdInList;
            IncludedInShortDescription = source.IncludedInShortDescription;
            EndInputOperation = source.EndInputOperation;
            IncludeInEqualsTo = source.IncludeInEqualsTo;
            ElementWindowPrmsMethod = source.ElementWindowPrmsMethod;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool CheckMembers(int nestingLevel, bool checkPermissionsForFirstLevel = true)
        ///
        /// \brief Check the common members to all ValueHolders in the tree.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      - If you want to start a check you should call : `ValueHolder.CheckMembers()`
        ///      - The method does the common checks and call this method to propagate the check to the children.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param nestingLevel                 (int) - The nesting level.
        /// \param checkPermissionsForFirstLevel (Optional)  (bool) - true to check permissions for first level.
        ///                                                         This parameter is for NetworkElements
        ///                                                         that are part of the network. In these NetworkElements the permissions
        ///                                                         of the NetworkElement is all false but the permissions of the dictionaries
        ///                                                         is according to the dictionaries usage.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CheckMembers(int nestingLevel, bool checkPermissionsForFirstLevel = true)
        {
            if (typeof(IValueHolder).IsAssignableFrom(GetValueType()))
            {
                return ValueHolder.CheckMembers(value, network, element, permissions, this, nestingLevel, GetValueType().ToString().Replace("DistributedAlgorithms.", ""));
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true)
        ///
        /// \brief Sets the members common to all value holders. This method is recursive and sets the members
        ///        to all the tree under the IValueHolder.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param network      (BaseNetwork) - The network.
        /// \param element      (NetworkElement) - The element.
        /// \param permissions  (Permissions) - The permissions.
        /// \param parent       (IValueHolder) - The parent.
        /// \param recursive   (Optional)  (bool) - true to process recursively, false to process locally only.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true)
        {
            this.network = network;
            this.permissions = new Permissions(permissions);
            this.parent = parent;
            this.element = element;

            if (Value != null)
            {
                if (typeof(IValueHolder).IsAssignableFrom(GetValueType()))
                {
                    if (recursive)
                    {
                        value.SetMembers(network, element, permissions, this);
                    }
                }
            }
        }

        #endregion
        #region /// \name IValueHolder Presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool ShortDescription(dynamic key, ref string keyString, ref string separator, ref string valueString)
        ///
        /// \brief Short description.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param          key         (dynamic) - The key.
        /// \param [in,out] keyString   (ref string) - The key string.
        /// \param [in,out] separator   (ref string) - The separator.
        /// \param [in,out] valueString (ref string) - The value string.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ShortDescription(dynamic key, ref string keyString, ref string separator, ref string valueString)
        {
            if (!IncludedInShortDescription)
            {
                return false;
            }

            switch (GetValueCategory(this))
            {
                case AttributeCategory.PrimitiveAttribute:
                    keyString = TypesUtility.GetKeyToString(key);
                    separator = "=";
                    valueString = Value.ToString();
                    break;
                case AttributeCategory.Null:
                    keyString = TypesUtility.GetKeyToString(key);
                    separator = "-";
                    valueString = "null";
                    break;
                case AttributeCategory.AttributeDictionary:
                    separator = "-";
                    valueString = "AttributeDictionary";
                    break;
                default:
                    ((IValueHolder)Value).ShortDescription(key, ref keyString, ref separator, ref valueString);
                    break;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private static string GetValueToString(object value)
        ///
        /// \brief Gets value to string.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param value (object) - The value.
        ///
        /// \return The value to string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string GetValueToString(object value)
        {
            if (value.GetType().IsEnum)
            {
                return TypesUtility.GetKeyToString(value);
            }
            if (value is bool)
            {
                if ((bool)value) return "true";
                else return "false";
            }

            // If the value is double and it is a natural number (i.e. can be converted
            // to int - generate a value with . after the number
            if (value is double)
            {
                try
                {
                    return int.Parse(value.ToString()).ToString() + ".";
                }
                catch
                {
                    return value.ToString();
                }
            }
            return value.ToString();
        }
        #endregion
        #region /// \name Add algorithm window support (creating c# code)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string AttributeCreateString(string valueString)
        ///
        /// \brief Attribute create string.
        ///
        /// \par Description.
        ///      Generate a string for creating the attribute.
        ///      
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      
        ///
        /// \author Ilanh
        /// \date 22/08/2017
        ///
        /// \param valueString  (string) - The value string.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string InitString(string valueString, string methodName)
        {
            string s = "new Attribute { " + "Value = ";
            s += InitValueString(valueString, methodName);
            s += ((Editable == true) ? "" : " ,Editable = false");
            s += ((IncludedInShortDescription == true) ? "" : " ,IncludedInShortDescription = false");
            s += ((Changed == true) ? " ,Changed = true" : " ,Changed = false");
            s += ((IdInList == 0) ? "" : " ,IdInList = " + IdInList.ToString());
            s += ((EndInputOperation == DefaultEndInputOperations) ? "" : " ,EndInputOperation = " + TypesUtility.GetEndInputOperationName(this));
            s += ((ElementWindowPrmsMethod == DefaultElementWindowPrmsMethod) ? "" : " ,ElementWindowPrmsMethod = " + TypesUtility.GetElementWindowPrmsMethodName(this));
            s += " } ";
            return s;
        }

        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string InitValueString(string valueString, string methodName = "")
        ///
        /// \brief Init value string.
        ///
        /// \par Description.
        ///      Create a value string represent the value of the attribute to be used when generating
        ///      a declaration of attribute string
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      -  There are 2 options  
        ///      -  The value is the valueString parameter
        ///      -  The value is the value of the attribute or the methodName
        ///
        /// \author Ilanh
        /// \date 12/09/2017
        ///
        /// \param valueString  (string) - The value string.
        /// \param methodName  (Optional)  (string) - Name of the method.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string InitValueString(string valueString, string methodName = "")
        {
            if (valueString != "")
            {
                return valueString;
            }
            else
            {
                if (value is string)
                    return "\"" + value + "\"";
                if (value is AttributeDictionary)
                    return methodName;
                if (value is AttributeList)
                    return methodName;
                if (value is NetworkElement)
                    return "new " + value.GetType().ToString() + "()";
                return GetValueToString(value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string ParameterString(object key)
        ///
        /// \brief Parameter string.
        ///
        /// \par Description.
        ///      This method is used in order to generate a string which contains a declaration of method
        ///      parameter with a default value for example:     
        ///~~~{.cs}
        ///      int intParameterName = 0
        ///~~~
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param key (object) - The key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string ParameterString(object key)
        {
            string s = ValueTypeString();
           
            s += " " + this.LcKeyString(key) + " = ";
            if (value is string)
                return s + "\"" + value + "\"";
            if (value is AttributeDictionary)
                return s + "null";
            if (value is AttributeList)
                return s + "null";
            if (value is NetworkElement)
                return s + "null";
            return s + GetValueToString(value);
        }

        public string ValueTypeString()
        {
            // If the type can be converted to double the type is double
            // This correction is for the following:
            // When reading attributes to the dictionary (In C# code generation) they are 
            // set according to the value. If the value was double 0 it will be converted to int 0
            // and when creating the methods code it will be int so we force it to double 
            // (Even if the programmer declared it as int) 
            string s = "";
            try
            {
                double d;
                d = Value;
                s += d.GetType().ToString();
            }
            catch
            {
                s = Value.GetType().ToString();
            }
            return s;
        }
      
        #endregion
        #region /// \name IValueHolder ElementWindow support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public ElementWindowPrms GetElementWindowPrms(dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Gets ElementWindow prms.
        ///
        /// \par Description.
        ///      -#   The ElementWindow is the base window for presenting/changing attributes of NetworkElements
        ///      -#   The ElementWindow is composed from 3 controls for each attribute
        ///           -#   Header TextBox which usually contains the key
        ///           -#   Existing value TextBox which contains the existing values
        ///           -#   New Value control which contains controls to change the value
        ///      -#   Each attribute has a ElementWindowPrmsMethod in which the input values for the
        ///           controls can be assigned
        ///      -#   In the method the user can set the prms for the ElementWindow controls
        ///      -#   In the method the user can set each value he wants
        ///      -#   This method does the followings:
        ///           -#    Calls the delegate method
        ///           -#    Fills default values to all the values that the delegate method did not fill.  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        /// \param inputWindow     (InputWindows) - The input window.
        /// \param windowEdittable (bool) - true if window editable.
        ///
        /// \return The element window prms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ElementWindowPrms GetElementWindowPrms(dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow, bool windowEdittable)
        {
            // First activate the InputField method of the attribute
            ElementWindowPrms elementWindowPrms = ElementWindowPrmsMethod(this, key,
                mainNetworkElement,
                mainDictionary,
                dictionary,
                inputWindow,
                windowEdittable);

            // Check the results of the method :
            // If the value for the New Value Input Field where set
            if (elementWindowPrms.newValueControlPrms.inputFieldType == InputFieldsType.Null)
                if (GetValueCategory(this) != AttributeCategory.PrimitiveAttribute)
                    Value.DefaultNewValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, windowEdittable, Editable);
                else
                    DefaultNewValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, windowEdittable, Editable);

            // If the string for the key field where set
            if (elementWindowPrms.typeString == "")
                if (GetValueCategory(this) != AttributeCategory.PrimitiveAttribute)
                    Value.DefaultTypeString(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);
                else
                    DefaultTypeString(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);

            // If the string for the key field where set
            if (elementWindowPrms.keyText == "")
                if (GetValueCategory(this) != AttributeCategory.PrimitiveAttribute)
                    Value.DefaultKeyFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, key);
                else
                    DefaultKeyFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, key);

            // If the string for the existing value control field where set
            if (elementWindowPrms.existingValueText == "")
                if (GetValueCategory(this) != AttributeCategory.PrimitiveAttribute)
                    Value.DefaultExistingValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);
                else
                    DefaultExistingValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);

            return elementWindowPrms;


        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms DefaultElementWindowPrmsMethod(Attribute attribute, dynamic key, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Default input field.
        ///
        /// \par Description.
        ///      This is the default method that is assigned This method sets the
        ///      default parameters for the ElementWindowControls.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 03/05/2017
        ///
        /// \param attribute       (Attribute) - The attribute.
        /// \param key             (dynamic) - The key.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary      (ElementDictionaries) - The dictionary.
        /// \param inputWindow     (InputWindows) - The input window.
        /// \param windowEdittable (bool) - true if window editable.
        ///
        /// \return The InputFieldPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms DefaultElementWindowPrmsMethod(Attribute attribute,
            dynamic key,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable)
        {
            ElementWindowPrms elementWindowPrms = new ElementWindowPrms();
            if (typeof(IValueHolder).IsAssignableFrom(attribute.Value.GetType()))
            {
                attribute.Value.DefaultKeyFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, key);
                attribute.Value.DefaultTypeString(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);
                attribute.Value.DefaultNewValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, windowEdittable, attribute.Editable);              
                attribute.Value.DefaultExistingValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);
            }
            else
            {
                attribute.DefaultKeyFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, key);
                attribute.DefaultTypeString(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);
                attribute.DefaultNewValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow, windowEdittable, attribute.Editable);              
                attribute.DefaultExistingValueFieldData(ref elementWindowPrms, mainNetworkElement, mainDictionary, dictionary, inputWindow);
            }
            return elementWindowPrms;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable, bool attributeEdittable)
        ///
        /// \brief Input field type.
        ///
        /// \par Description.
        ///      Set the parameters for new value control for simple values:
        ///      -#   bool - ComboBox
        ///      -#   Enum -ComboBox
        ///      -#   The rest - TextBox.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 03/05/2017
        ///
        /// \return The InputFieldPrms.
        ///
        /// \param [in,out] elementWindowPrms  (Type) - Type of the window.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        /// \param          windowEdittable    (bool) - true if window editable.
        /// \param          attributeEdittable (bool) - true if attribute editable.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable,
            bool attributeEdittable)
        {
            
            bool editable = Editable;
            if (inputWindow == InputWindows.AddAlgorithmWindow)
            {
                editable = this.AddAlgorithmWindowAttributeEditable(mainNetworkElement, mainDictionary);
            }

            // New Value control for boolean - ComboBox
            if (Value.GetType().Equals(typeof(bool)))
            {
                EnumInputField(ref elementWindowPrms, editable, new string[] { "False", "True" });
                return;
            }

            // New Value control for primitive - TextBox
            if (Value.GetType().IsPrimitive || Value.GetType().Equals(typeof(string)))
            {
                elementWindowPrms.newValueControlPrms.enable = editable;
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
                elementWindowPrms.newValueControlPrms.Value = Value.ToString();
                return;
            }

            // NewValue control for Enum - ComboBox
            if (Value.GetType().IsEnum)
            {
                EnumInputField(ref elementWindowPrms, editable, Enum.GetNames(Value.GetType()));
                return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void EnumInputField(ref ElementWindowPrms elementWindowPrms, string[] options)
        ///
        /// \brief Enum input field.
        ///
        /// \par Description.
        ///      Create the specification for enum new value control.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 03/05/2017
        ///
        /// \return The InputFieldPrms.
        ///
        /// \param [in,out] elementWindowPrms (string) - The values.
        /// \param          options           (string[]) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void EnumInputField(ref ElementWindowPrms elementWindowPrms, bool editable, string[] options)
        {
            if (editable)
            {
                elementWindowPrms.newValueControlPrms.options = options;
                elementWindowPrms.newValueControlPrms.Value = TypesUtility.GetKeyToString(Value);
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.ComboBox;
            }
            else
            {
                elementWindowPrms.newValueControlPrms.enable = false;
                elementWindowPrms.newValueControlPrms.Value = TypesUtility.GetKeyToString(Value);
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, dynamic key)
        ///
        /// \brief Key field data.
        ///
        /// \par Description.
        ///      This method is the default method for filling the header TextBox
        ///      key in the ElementWindow for attributes from simple type (not IValueHolder)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms (ref ElementWindowPrms) - The element window prms.
        /// \param          mainDictionary    (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary        (ElementDictionaries) - The dictionary.
        /// \param          inputWindow       (InputWindows) - The input window.
        /// \param          key               (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            dynamic key)
        {
            if (key is null)
            {
                elementWindowPrms.keyText = "";
                return;
            }
            elementWindowPrms.keyText = (key.GetType().IsEnum) ? TypesUtility.GetKeyToString(key) : key.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultTypeString(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow)
        ///
        /// \brief Default type string.
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
        /// \param [in,out] elementWindowPrms  (ref ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
        /// \param          mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary          (ElementDictionaries) - The dictionary.
        /// \param          inputWindow         (InputWindows) - The input window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultTypeString(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow)
        {
            elementWindowPrms.typeString = TypesUtility.GetTypeShortNameToString(Value.GetType()) + " " + MembersStr();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow)
        ///
        /// \brief Existing value field data.
        ///
        /// \par Description.
        ///      This method is the default method for filling the Existing Value
        ///      TextBox in the ElementWindow for attributes from simple type (not IValueHolder)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms (ref ElementWindowPrms) - The element window prms.
        /// \param          mainDictionary    (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary        (ElementDictionaries) - The dictionary.
        /// \param          inputWindow       (InputWindows) - The input window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow)
        {
           
            elementWindowPrms.existingValueText = GetValueToString().Replace("DistributedAlgorithms.", "");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string MembersStr()
        ///
        /// \brief Members string.
        ///
        /// \par Description.
        ///      Write the members to string
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 03/01/2018
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string MembersStr()
        {
            return  " (" + IdInList.ToString() + "," + Changed.ToString()[0] + ")";
        }

        #endregion
        #region /// \name IValueHolder Get Child Key

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public dynamic GetChildKey(IValueHolder child)
        ///
        /// \brief Gets a child key.
        ///
        /// \par Description.
        ///      Get the key of the attribute in the parent.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param child (IValueHolder) - The child.
        ///
        /// \return The child key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic GetChildKey(IValueHolder child)
        {
            return parent.GetChildKey(this);
        }
        #endregion
        #region /// \name End Input Operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool DefaultEndInputOperations(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Default end input operations.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param       network         (BaseNetwork) - The network.
        /// \param       networkElement  (NetworkElement) - The network element.
        /// \param       parentAttribute (Attribute) - The parent attribute.
        /// \param       attribute       (Attribute) - The attribute.
        /// \param       newValue        (string) - The new value.
        /// \param [out] errorMessage    (out string) - Message describing the error.
        /// \param       inputWindow     (Optional)  (ElementWindow) - The input window.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool DefaultEndInputOperations(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";
            return true;
        }
        #endregion
    }
}

