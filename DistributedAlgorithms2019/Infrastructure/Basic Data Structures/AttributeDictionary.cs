////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file infrastructure\attributedictionary.cs
///
/// \brief Implements the AttributeDictionary class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Xml.Serialization;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class AttributeDictionary
    ///
    /// \brief Dictionary of attributes.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 22/10/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AttributeDictionary : Dictionary<dynamic, Attribute>, IValueHolder
    {
        #region /// \name Members
        
        /// \brief (IValueHolder) - The parent. - The Attribute that this AttributeDictionary is assigned to.
        private IValueHolder parent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Permissions) - The permissions.
        ///        Define the permissions for add/remove/clear/set actions according to the program
        ///        activation phase.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Permissions permissions;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (BaseNetwork) - The network.
        ///         The Network that is handled (presented, running, etc.)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private BaseNetwork network;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (NetworkElement) - The element.
        ///        The NetworkElement that the attributeDictionary belongs to (network, process etc.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private NetworkElement element;

        public string selfEnumName;

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
        #region /// \name Attributes and Values access

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public new dynamic this(dynamic key)
        ///
        /// \brief Indexer to get or set items within this collection using array index syntax.
        ///
        /// \param key  (dynamic) - The key.
        ///
        /// \return The indexed item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new dynamic this[dynamic key]
        {
            get
            {
                key = Key(key);
                return this.First(entry => TypesUtility.CompareDynamics(entry.Key, key)).Value.Value;
            }
            set
            {
                key = Key(key);
                this.First(entry => TypesUtility.CompareDynamics(entry.Key, key)).Value.Value = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private dynamic Key(dynamic key)
        ///
        /// \brief Keys the given key.
        ///
        /// \par Description.
        ///     Get the key for the dictionary
        ///     The key can be one of the following:
        ///     -#  A dynamic key that can be used as is
        ///     -#  A string that holds the name of the key from the enum of the dictionary
        ///
        /// \par Algorithm.
        ///      -# Try to get the key as is
        ///      -# If not found compose a key from the enum of the dictionary + the key
        ///      -# If not found produces an error message and throw exception (program termination)
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 06/03/2018
        ///
        /// \exception Exception Thrown when an exception error condition occurs.
        ///
        /// \param key  (dynamic) - The key.
        ///
        /// \return A dynamic.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private dynamic Key(dynamic key)
        {
            string keyName = key.GetType().ToString() + TypesUtility.GetKeyToString(key);
            dynamic _key = key;

            if (!this.Any(entry => TypesUtility.CompareDynamics(entry.Key, _key)))
            {
                if (_key is string)
                {
                    keyName = selfEnumName + "." + key;
                    _key = TypesUtility.GetKeyFromString(TypesUtility.GetEnumType(selfEnumName), key);
                    if ((_key is null) ||
                        !this.Any(entry => TypesUtility.CompareDynamics(entry.Key, _key)))
                    {
                        string s = TypesUtility.GetKeyToString(Parent.Parent.GetChildKey(Parent));
                        s += "\n key " + keyName + " Not found";
                        MessageRouter.MessageBox(new List<string> { s }, "", null, Icons.Error);
                        throw new Exception();
                    }
                }
            }
            return _key;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute GetAttribute(dynamic key)
        ///
        /// \brief Gets an attribute.
        ///
        /// \par Description.
        ///      The getter and Setter acts directly on the attribute value. This method is for getting
        ///      the attribute itself.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param key (dynamic) - The key.
        ///
        /// \return The attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute GetAttribute(dynamic key)
        {
            return this.First(entry => TypesUtility.CompareDynamics(entry.Key, key)).Value;
        }
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public AttributeDictionary()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///      Generate an empty AttributeDictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary()
        {
            // Temp value for the protection allow changing before it is assigned to attribute
            permissions = new Permissions(true);
        }


        #endregion
        #region /// \name Attribute inserting and removing

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Add(dynamic key, dynamic value, ElementWindow sender = null)
        ///
        /// \brief Add an attribute to the dictionary.
        ///
        /// \par Description.
        ///      -# Checks that the key is legal (simple type or enum)
        ///      -# Check if there is a permission to add new attribute according to the phase or the execution
        ///      -# Add the attribute (using a constructor that gets the network, permissions, parent)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param key     (dynamic) - The key.
        /// \param value   (dynamic) - The value.
        /// \param sender (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Add(dynamic key, dynamic value, ElementWindow sender = null)
        {
            Attribute attribute;
            if (value is Attribute)
            {
                attribute = value;
            }
            else
            {
                attribute = new Attribute { Value = value };
            }

            if (!KeyIsLeagal(key))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to add attribute : " + TypesUtility.GetKeyToString(key) + " to dictionary :" +  TypesUtility.GetKeyInParent(parent, this) + " because the key is not legal\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeDictionary Add", null, Icons.Error);
                return false;
            }
            if (!permissions.CheckPermition(Permissions.PermitionTypes.AddRemove, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to add attribute : " + TypesUtility.GetKeyToString(key) + " to dictionary :" +  TypesUtility.GetKeyInParent(parent, this) + " because of add/remove permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeDictionary Add", null, Icons.Error);
                return false;
            }

            base.Add((object)key, attribute);

            // The following command will set the members to all the tree under the attribute

            attribute.SetMembers(network, element, permissions, this);
            return true;
 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Remove(dynamic key, ElementWindow sender = null)
        ///
        /// \brief Removes according to the given key.
        ///
        /// \par Description.
        ///      -# Check if there is a permission to remove according to the execution phase
        ///      -# Remove the attribute.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param key    The key to remove.
        /// \param sender (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Remove(dynamic key, ElementWindow sender = null)
        {
            if (!permissions.CheckPermition(Permissions.PermitionTypes.AddRemove, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to remove : " + TypesUtility.GetKeyToString(key) + " from dictionary : " + TypesUtility.GetKeyToString(parent.GetChildKey(this)) + " because of add/remove permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeDictionary Remove", null, Icons.Error);
                return false;
            }
            base.Remove((object)key);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Clear(ElementWindow sender = null)
        ///
        /// \brief Clears this object to its blank/initial state.
        ///
        /// \par Description.
        ///      -# Check the permission to activate Clear operation according to the execution phase
        ///      -# Clear the dictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param sender (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Clear(ElementWindow sender = null)
        {
            if (!permissions.CheckPermition(Permissions.PermitionTypes.Clear, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to clear dictionary " + TypesUtility.GetKeyToString(parent.GetChildKey(this)) + " because of clear permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.Clear),
                            permissions.ToString()},
                            "AttributeDictionary Clear", null, Icons.Error);
                return false;
            }
            base.Clear();
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool KeyIsLeagal(dynamic key)
        ///
        /// \brief Key is legal.
        ///
        /// \par Description.
        ///      Key can be:
        ///      -# Enum value
        ///      -# Primitive
        ///      -# String.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param key (dynamic) - The key.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool KeyIsLeagal(dynamic key)
        {
            Type keyType = key.GetType();
            if (keyType.IsEnum) return true;
            if (keyType.IsPrimitive) return true;
            if (keyType.Equals(typeof(string))) return true;
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddOrSet(dynamic key, dynamic value)
        ///
        /// \brief Adds an or set attribute.
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
        /// \param key   (dynamic) - The key.
        /// \param value (dynamic) - The value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddOrSet(dynamic key, dynamic value)
        {
            if (ContainsKey(key))
            {
                this[key].Value = value;
            }
            else
            {
                Add(key, new Attribute { Value = value });
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddAttributeIfNotExist(Attribute attribute, dynamic key)
        ///
        /// \brief Adds an attribute if not exist to 'key'.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param attribute (Attribute) - The attribute.
        /// \param key       (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddAttributeIfNotExist(Attribute attribute, dynamic key)
        {
            if (!ContainsKey(key))
            {
                Add(key, attribute);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RemoveAttributeIfExist(dynamic key)
        ///
        /// \brief Removes the attribute if exist described by key.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/05/2017
        ///
        /// \param key (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveAttributeIfExist(dynamic key)
        {
            if (!ContainsKey(key))
            {
                    Remove(key);
            }
        }

        #endregion
        #region /// \name IValueHolder Load and Save

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Load(XmlNode xmlNode, Type hostType = null)
        ///
        /// \brief Loads.
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
        /// \param hostType (Optional)  (Type) - Type of the host.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Load(XmlNode xmlNode, Type hostType = null)
        {
            Clear();
            bool converted;
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                switch (xmlAttribute.Name)
                {
                    case "selfEnumName":
                        selfEnumName = xmlAttribute.Value;
                        break;
                }
            }

            foreach (XmlNode elementAttributeNode in xmlNode.ChildNodes)
            {
                string keyTypeString = elementAttributeNode.Attributes["KeyType"].Value;
                string keyString = elementAttributeNode.Attributes["Key"].Value;
                dynamic key = TypesUtility.ConvertValueFromString(TypesUtility.GetTypeFromString(keyTypeString), keyString, out converted);
                Logger.WriteOperationToEditLogFile("Key:", new string[] { keyString }, Icons.Info);
                Attribute attribute = new Attribute();
                if (!attribute.Load(elementAttributeNode, hostType))
                {
                    return false;
                }
                Add(key, attribute);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode)
        ///
        /// \brief Saves.
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
            XmlAttribute xmlTypeAttribute = xmlDoc.CreateAttribute("selfEnumName");
            xmlTypeAttribute.Value = selfEnumName;
            xmlNode.Attributes.Append(xmlTypeAttribute);
            foreach (var entry in this)
            {
                XmlNode entryNode = xmlDoc.CreateElement("Attribute");
                SaveKey(xmlDoc, entryNode, entry.Key);
                entry.Value.Save(xmlDoc, entryNode);
                xmlNode.AppendChild(entryNode);
            }
            return xmlNode;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SaveKey(XmlDocument xmlDoc, XmlNode xmlAttributeNode, object key)
        ///
        /// \brief Saves a key.
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
        /// \param xmlDoc           (XmlDocument) - The XML document.
        /// \param xmlAttributeNode (XmlNode) - The XML attribute node.
        /// \param key              (object) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SaveKey(XmlDocument xmlDoc, XmlNode xmlAttributeNode, object key)
        {
            XmlAttribute xmlKeyTypeAttribute = xmlDoc.CreateAttribute("KeyType");
            xmlKeyTypeAttribute.Value = key.GetType().ToString();
            xmlAttributeNode.Attributes.Append(xmlKeyTypeAttribute);

            XmlAttribute xmlKeyAttribute = xmlDoc.CreateAttribute("Key");
            if (key.GetType().Equals(typeof(int)))
            {
                xmlKeyAttribute.Value = key.ToString();
            }
            else
            {
                xmlKeyAttribute.Value = TypesUtility.GetKeyToString(key);
            }
            xmlAttributeNode.Attributes.Append(xmlKeyAttribute);
        }
        #endregion
        #region /// \name IValueHolder Scanning Operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ScanAndReport(IScanConsumer scanConsumer, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, dynamic key = null)
        ///
        /// \brief Scans and report.
        ///
        /// \par Description.
        ///      -  The scanning process is a process that gives to the scanConsumer all the attributes
        ///         that are found under IValueHolder Tree
        ///      -  The scanning process travels on the tree and reports to the scanConsumer on each attribute  
        ///         It reached.
        ///      -  For the AttributeDictionary we have to report all the attributes in the dictionary.
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
            foreach (var entry in this)
            {
                if (scanConsumer.ScanCondition(entry.Key, entry.Value, mainDictionary, dictionary))
                {
                    entry.Value.ScanAndReport(scanConsumer, mainDictionary, dictionary, entry.Key);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool checkNotSameObject = false)
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
        /// \param          checkNotSameObject (Optional)  (bool) - true to check not same object.
        ///
        /// \return True if equals to, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool print = false, bool checkNotSameObject = false)
        {
            AttributeDictionary otherDictionary = (AttributeDictionary)other;

            if (Count != otherDictionary.Count)
            {
                error = "The count of one is : " + Count.ToString() + " The count of two is : " + otherDictionary.Count.ToString();
                return false;
            }

            foreach (var entry in otherDictionary)
            {
                if(!ContainsKey(entry.Key))
                {
                    error = "one does not contain key " + TypesUtility.GetKeyToString(entry.Key);
                    return false;
                }
                Attribute thisAttribute = GetAttribute(entry.Key);
                Attribute otherAttribute = otherDictionary.GetAttribute(entry.Key);
                string keyString = TypesUtility.GetKeyToString(entry.Key);

                if (!thisAttribute.CheckEqual(nestingLevel + 1, keyString, otherAttribute, print, checkNotSameObject))
                {
                    return false;
                }              
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DeepCopy(IValueHolder source, bool isInitiator = true)
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
            Clear();
            foreach (var entry in (AttributeDictionary)source)
            {
                Attribute attribute = new Attribute();
                attribute.DeepCopy(entry.Value, false);
                Add(entry.Key, attribute);
            }

            if (isInitiator)
            {
                SetMembers(network, element, permissions, parent);
            }
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
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param nestingLevel                  (int) - The nesting level.
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
            foreach (var entry in this)
            {
                if (!ValueHolder.CheckMembers(entry.Value, network, element, permissions, this, nestingLevel, TypesUtility.GetKeyToString(entry.Key)))
                    return false;
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

            if (recursive)
            {
                foreach (Attribute attribute in Values)
                {
                    attribute.SetMembers(network, element, permissions, this);
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
            keyString = TypesUtility.GetKeyToString(key);
            separator = "-";
            valueString = "AttributeDictionary";
            return true;
        }
        #endregion
        #region /// \name IValueHolder Get Child Key

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public dynamic GetChildKey(IValueHolder child)
        ///
        /// \brief Gets a child key.
        ///
        /// \par Description.
        ///      Get the key of the dictionary in the parent of the attribute that this AttributeDictionary is assigns to.
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
            return this.First(e => e.Value == child).Key;
        }
        #endregion
        #region /// \name Presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ShortDescription(ref string shortDescription, string separator)
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
        /// \param [in,out] shortDescription (ref string) - Information describing the short.
        /// \param          separator        (string) - The separator.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShortDescription(ref string shortDescription, string separator)
        {
            foreach (var attribute in this)
            {
                if (attribute.Value.IncludedInShortDescription)
                {
                    shortDescription += TypesUtility.GetKeyToString(attribute.Key) + "=" + attribute.Value.GetValueToString() + separator;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string ShortDescription()
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
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string ShortDescription()
        {
            string result = "";
            string keyString = "";
            string valueString = "";
            string separator = "";
            foreach (var entry in this)
            {
                if (entry.Value.ShortDescription(entry.Key, ref keyString, ref separator, ref valueString))
                {
                    result += keyString;
                    result += separator;
                    result += valueString;
                    result += ",";
                }
            }
            return result.Remove(result.Length - 1);
        }
        #endregion
        #region /// \name IValueHolder ElementWindow support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEditable, bool attributeEditable)
        ///
        /// \brief Creates a new value field data.
        ///
        /// \par Description.
        ///      -  This method sets the parameter for presenting/changing controls in ElementWindow.  
        ///      -  The ElementWindow has 3 controls for each field  
        ///         -#  Header which usually contains the key of the attribute
        ///         -#  Existing value TextBox
        ///         -#  New Value control for changing the values
        ///      -  This method sets the default parameters for generating the new value controls for  
        ///         AttributeDictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms  (ref ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement (NetworkElement) - The main network element.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        /// \param          windowEditable     (bool) - true if window editable.
        /// \param          attributeEditable  (bool) - true if attribute editable.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEditable,
            bool attributeEditable)
        {
            bool editable = attributeEditable;


            if (inputWindow == InputWindows.AddAlgorithmWindow)
            {
                if (!this.AddAlgorithmWindowAttributeEditable(mainNetworkElement, mainDictionary))
                {
                    editable = false;
                }
            }

            if (editable)
            {
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.AddRemovePanel;
            }
            else
            {
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
                elementWindowPrms.newValueControlPrms.Value = "AttributeDictionary";
                elementWindowPrms.newValueControlPrms.enable = false;
            }

            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, dynamic key)
        ///
        /// \brief Default key field data.
        ///
        /// \par Description.
        ///      This method is the default method for filling the header TextBox
        ///      key in the ElementWindow for AttributeDictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms  (ref ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement (NetworkElement) - The main network element.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        /// \param          key                (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow,
            dynamic key)
        {
            if (inputWindow == InputWindows.AddAlgorithmWindow)
            {
                if (SetForAddAlgorithmWindow(ref elementWindowPrms, mainNetworkElement, key))
                {
                    return;
                }
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
            elementWindowPrms.typeString = "AttributeDictionary" + " " + MembersStr();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private bool SetForAddAlgorithmWindow(ref ElementWindowPrms elementWindowPrms, NetworkElement networkElement, dynamic key)
        ///
        /// \brief Sets for add algorithm window.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param [in,out] elementWindowPrms (ref ElementWindowPrms) - The element window prms.
        /// \param          networkElement     (NetworkElement) - The network element.
        /// \param          key                (dynamic) - The key.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool SetForAddAlgorithmWindow(ref ElementWindowPrms elementWindowPrms,
            NetworkElement networkElement, dynamic key)
        {
            // The main dictionaries in the message network element in the AddAlgorithmWindow
            // Gets different names (It is just a container for the enums)

            // If this is a main dictionary of the NetworkElement
            if (((Attribute)Parent).Parent == networkElement)
            {
                // If the networkElement is a demoMessage
                if (typeof(NetworkElement).Equals(networkElement.GetType()))
                {
                    if (TypesUtility.CompareDynamics(key, NetworkElement.ElementDictionaries.ElementAttributes))
                    {
                        elementWindowPrms.keyText = "HeaderFields";
                        return true;
                    }
                    if (TypesUtility.CompareDynamics(key, NetworkElement.ElementDictionaries.PrivateAttributes))
                    {
                        elementWindowPrms.keyText = "MessageTypes";
                        return true;
                    }
                    if (TypesUtility.CompareDynamics(key, NetworkElement.ElementDictionaries.OperationResults))
                    {
                        elementWindowPrms.keyText = "Messages";
                        return true;
                    }
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow)
        ///
        /// \brief Existing value field data.
        ///
        /// \par Description.
        ///      This method is the default method for filling the Existing Value
        ///      TextBox in the ElementWindow for attributes from type AttributeDictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/201
        ///
        /// \param [in,out] elementWindowPrms  (ref ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement (NetworkElement) - The main network element.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary         (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            NetworkElement.ElementDictionaries mainDictionary,
            NetworkElement.ElementDictionaries dictionary,
            InputWindows inputWindow)
        {
            string s = 
            elementWindowPrms.existingValueText = "AttributeDictionary";
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
            return "(" + ((Attribute)parent).IdInList.ToString() + "," + ((Attribute)parent).Changed.ToString()[0] + ")";
        }
        #endregion
        #region /// \name Utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Sort()
        ///
        /// \brief Sorts this object.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Sort()
        {
            Dictionary<dynamic, Attribute> orderedDictionary = this.OrderBy(e => (int)e.Key).ToDictionary(x => x.Key, x => x.Value);
            Clear();
            foreach (var e in orderedDictionary)
            {
                Add(e.Key, e.Value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute FindAttribute(string keyString)
        ///
        /// \brief Searches for the first attribute.
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
        /// \param keyString (string) - The key string.
        ///
        /// \return The found attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute FindAttribute(string keyString)
        {
            foreach (var dictionaryEntry in this)
            {
                if (keyString == TypesUtility.GetKeyToString(dictionaryEntry.Key))
                {
                    return dictionaryEntry.Value;
                }
            }

            return null;
        }
        #endregion
        #region /// \name Add algorithm window support (creating c# code)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string EnumString(string enumString, bool isMessage, string targetSubject, string targetAlgorithm, HashSet<string> syntaxHighlight)
        ///
        /// \brief Enum string.
        ///
        /// \par Description.
        ///      Create a code for enum from the keys of this dictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param enumString       (string) - The enum string.
        /// \param isMessage        (bool) - true if this object is message.
        /// \param targetSubject    (string) - Target subject.
        /// \param targetAlgorithm  (string) - Target algorithm.
        /// \param syntaxHighlight  (HashSet&lt;string&gt;) - The syntax highlight.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string EnumString(string enumString, 
            bool isMessage, 
            string targetSubject, 
            string targetAlgorithm, 
            HashSet<string> syntaxHighlight)
        {
            string eol = Environment.NewLine;

            object selfkey = Parent.GetChildKey(this);
            string ucKeyString = this.UcKeyString(selfkey);
            string lcKeyString = this.LcKeyString(selfkey);
            string enumName = this.GetKeyString(false, (isMessage)?true:false);
            syntaxHighlight.Add(enumName);

            string s = eol + " " + eol + "\t\tpublic enum " + enumName;
            s += eol + "\t\t{";
            s+= eol + "\t\t\t";
            foreach (dynamic key in this.Keys)
            {
                if (!KeyOfThisClass(key, targetSubject, targetAlgorithm))
                {
                    continue;
                }

                s += this.UcKeyString((object)key) + ", ";
            }
            s = s.Remove(s.Length - 2);
            s += eol + "\t\t}";
            return s;
        }

        public void AddConsts(HashSet<string> consts, bool isMessageTypeConst)
        {
            foreach (var entry in this)
            {
                consts.Add(TypesUtility.GetKeyToString(entry.Key) + ((isMessageTypeConst)?"__":""));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string MessageMethodString(string enumClass)
        ///
        /// \brief Message method string.
        ///
        /// \par Description.
        ///      -  Create a code for a message component that is this dictionary
        ///      -  The code generated has the following parameters:  
        ///         -#  The way the user wants to fill the dictionary
        ///             -#  bm.PrmSource.MainPrm - Take the second parameter which is a dictionary
        ///             -#  bm.PrmSource.Default - Take the default values of the parameters (from the third parameter)
        ///             -#  bm.PrmSource.Prms - Take the value from the parameters
        ///         -#  An optional main parameter which is a dictionary to be passes
        ///         -#  For each attribute in the dictionary an optional parameter to get the attribute value.  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The following is the ways a call can be made for this method:
        ///      ~~~{.cs}
        ///      // The first option - Getting a value from the dictionary parameter
        ///      public AttributeDictionary MessageDataFor_Ad( bm.PrmSource.MainPrm, dictionary);
        ///      
        ///      // The second option - Default value (sets when the algorithm code was built)
        ///      public AttributeDictionary MessageDataFor_Ad( bm.PrmSource.Default);
        ///      
        ///      // The third option - Value for each parameter
        ///      public AttributeDictionary MessageDataFor_Ad( bm.PrmSource.Prms, null, true, 1);
        ///      ~~~.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param enumClass (string) - The enum class.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string MessageMethodString(string enumClass, string targetSubject, string targetAlgorithm)
        {
            object key = Parent.GetChildKey(this);
          
            string eol = Environment.NewLine;

            // Create the method header
            string s= eol + " " + eol + "\t\tpublic AttributeDictionary " + this.MessageMethodName() + "( bm.PrmSource prmSource = bm.PrmSource.Default," + 
                    eol + "\t\t\tAttributeDictionary dictionary = null,";

            // Parameters in method header
            foreach (var entry in this)
            {
                s += eol + "\t\t\t" + entry.Value.ParameterString(entry.Key) + ",";
            }

            // Remove the last "," and close the method header
            s = s.Remove(s.Length - 1) + ")";

            
            // Method body
            s += eol + "\t\t{";

            // If the source is the main prm
            s+= eol + " " + eol + "\t\t\tif ( " + "prmSource == bm.PrmSource.MainPrm)";
            s+= eol + "\t\t\t{";
            s+= eol + "\t\t\t\treturn dictionary;";
            s+= eol + "\t\t\t}";

            s+= eol + "\t\t\tdictionary = new AttributeDictionary();";
            s += eol + "\t\t\tdictionary.selfEnumName = " + "\"" + this.DictionaryEnum(ClassFactory.GenerateNamespace(targetSubject, targetAlgorithm), enumClass) + "\";";


            // If the source of the parameters is the default values
            s += eol + " " + eol + "\t\t\tif ( " + "prmSource == bm.PrmSource.Default)";
            s += eol + "\t\t\t{";
            foreach (var entry in this)
            {
                string methodName = entry.Value.MessageMethodName() + "(bm.PrmSource.Default)";
                s += eol + "\t\t\t\tdictionary.Add( " + this.AttributeKey((object)entry.Key, enumClass, true) +
                    ", " + entry.Value.InitString("", methodName) + ");";
            }
            s += eol + "\t\t\t\treturn dictionary;";
            s += eol + "\t\t\t}";

            // Else PrmSource.Prms - Create attribute with the values in the parameters
            s += " " + eol;
            foreach (var entry in this)
            {
                s += eol + "\t\t\tdictionary.Add( " + this.AttributeKey((object)entry.Key, enumClass, true) +
                    ", " + entry.Value.InitString(this.LcKeyString((object)entry.Key), "") + ");";
            }
            s += eol + "\t\t\treturn dictionary;";
            s += eol + "\t\t}";
            return s;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string SendMessagesStrings()
        ///
        /// \brief Sends the messages strings.
        ///
        /// \par Description.
        ///      -  This method is called when this dictionary is the MessageType dictionary.
        ///      -  It generates a code for sending a message for each one of the entries in the dictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string SendMessagesStrings()
        {
            string s = "";
            foreach (var entry in this)
            {
                s += SendMessageString(entry.Key);
            }
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string SendMessageString(object key)
        ///
        /// \brief Sends a message string.
        ///
        /// \par Description.
        ///      -  This method generate a code that can be used to send a message  
        ///      -  The method generated has the following parameters:  
        ///         -#  A dictionary that holds all the fields
        ///         -#  The way the list of target ids will be handled:
        ///             -#  SelectingMethod.Include - The list of ids are the ids that the message will be sent to
        ///             -#  SelectingMethod.Exclude - The message will be sent to all the neighbors but the ons in the ids list
        ///             -#  SelectingMethod.All - Send a message to all the neighbors
        ///         -#  ids - a list of ints with the ids to include or exclude.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The following is the ways a call can be made for this method:
        ///      ~~~{.cs}
        ///      //Include option
        ///      public void SendAd(dictionary, SelectingMethod.Include, List&lt;int&gt; ids = new List&lt;int&gt; {1,2,3})
        ///      
        ///      //Exclude option
        ///      public void SendAd(dictionary, SelectingMethod.Exclude, List&lt;int&gt; ids = new List&lt;int&gt; {1,2,3})
        ///      
        ///      //All option
        ///      public void SendAd(dictionary, SelectingMethod.All})
        ///      ~~~.
        ///
        /// \author Ilanh
        /// \date 27/09/2017
        ///
        /// \param key (object) - The key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string SendMessageString(object key)
        {
            string eol = Environment.NewLine;
            string messageName = this.GetEnumName(key, true);
            string s = eol + " " + eol + "\t\tpublic void " + this.SendMethodName(key) + "(";
            s += "AttributeDictionary " + " fields = null, ";
            s += eol + "\t\t\tSelectingMethod selectingMethod = SelectingMethod.All,";
            s += eol + "\t\t\tList<int> ids = null,";
            s += eol + "\t\t\tint round = -1,";
            s += eol + "\t\t\tint clock = -1)";
            s += eol + "\t\t{";
            s += eol + "\t\t\tif(fields is null)";
            s += eol + "\t\t\t{";
            s += eol + "\t\t\t\t" + this.MessageMainDictionaryMethodName(key) + "();";
            s += eol + "\t\t\t}";
            s += eol + "\t\t\tSend(m.MessageTypes." + messageName + ", fields, selectingMethod, ids, round, clock);";
            s += eol + "\t\t}";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string InitMainDictionaryMethodString(object key, string enumClass, ref string gettersSettersString, string targetSubject, string targetAlgorithm)
        ///
        /// \brief Init main dictionary method string.
        ///
        /// \par Description.
        ///      Create an init method for a main dictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The method will be activated automatically when the user initialize the network element.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param          key                   (object) - The key.
        /// \param          enumClass             (string) - The enum class.
        /// \param [in,out] gettersSettersString (ref string) - The getters setters string.
        /// \param          targetSubject         (string) - Target subject.
        /// \param          targetAlgorithm       (string) - Target algorithm.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string InitMainDictionaryMethodString(object key, string enumClass, ref string gettersSettersString, string targetSubject, string targetAlgorithm)
        {
            string eol = Environment.NewLine;
            string s = eol + " " + eol + "\t\tprotected override void " + this.MainDictionaryInitMethodName() + "()";
            s += eol + "\t\t{";
            s += eol + "\t\t\tAttributeDictionary dictionary = " + this.MainDictionaryName() + ";";
            s += eol + "\t\t\tdictionary.selfEnumName = " + "\"" + this.DictionaryEnum(ClassFactory.GenerateNamespace(targetSubject, targetAlgorithm), enumClass) + "\";";
            foreach (var entry in this)
            {
                if (!KeyOfThisClass(entry.Key, targetSubject, targetAlgorithm))
                {
                    continue;
                }
                string attributeKey = this.AttributeKey((object)entry.Key, enumClass);
                gettersSettersString += GetterSetterString(entry.Key, (Attribute)entry.Value, attributeKey);

                string methodName = entry.Value.InitMethodName() + "()";
                s += eol + "\t\t\tdictionary.Add(" + attributeKey +
                             ", " + entry.Value.InitString("", methodName) + ");";
            }
            s += eol + "\t\t\tbase." + this.MainDictionaryInitMethodName() + "();";
            s += eol + "\t\t}";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private string GetterSetterString(string key ,Attribute attribute, string attributeKey)
        ///
        /// \brief Getter setter string.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 06/03/2018
        ///
        /// \param key           (string) - The key.
        /// \param attribute     (Attribute) - The attribute.
        /// \param attributeKey  (string) - The attribute key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private string GetterSetterString(object key ,Attribute attribute, string attributeKey)
        {
            string eol = Environment.NewLine;
            string typeStr;
            if (attribute.Value is null)
            {
                typeStr = "object";
            }
            else
            {
                typeStr = attribute.GetValueType().ToString().Replace("+", ".");
            }
            string s = eol + " " + eol + "\t\tpublic " + typeStr + " " + this.UcKeyString(key);
            s += eol + "\t\t{";
            s += eol + "\t\t\t get { return " + this.MainDictionaryName() + "[" + attributeKey + "]; }";
            s += eol + "\t\t\t set { " + this.MainDictionaryName() + "[" + attributeKey + "] = value; }";
            s += eol + "\t\t}";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string CreateGettersSetters(string enumClass, string targetSubject, string targetAlgorithm, bool isMessage)
        ///
        /// \brief Creates getters setters.
        ///
        /// \par Description.
        ///      Create Getters/Setters strings for the following access to the dictionary
        ///      attributes.
        ///      The getters/Setters are used in order to access NetworkElement dictionary attributes
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/03/2018
        ///
        /// \param enumClass        (string) - The enum class.
        /// \param targetSubject    (string) - Target subject.
        /// \param targetAlgorithm  (string) - Target algorithm.
        /// \param isMessage        (bool) - true if this object is message.
        ///
        /// \return The new getters setters.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string CreateGettersSetters(string enumClass, string targetSubject, string targetAlgorithm, bool isMessage)
        {
            string s = "";
            foreach (var entry  in this)
            {
                if (targetAlgorithm != "")
                {
                    if (!KeyOfThisClass(entry.Key, targetSubject, targetAlgorithm))
                    {
                        continue;
                    }
                }
                string attributeKey = this.AttributeKey((object)entry.Key, enumClass);
                s += GetterSetterString(entry.Key, (Attribute)entry.Value, attributeKey);
            }
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string InitMethodString(object key, string enumClass, ref string constsString, string targetSubject, string targetAlgorithm)
        ///
        /// \brief Init method string.
        ///
        /// \par Description.
        ///      Create an init method for a dictionary which is not a main dictionary.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The method will be activated automatically when the user initialize the network element.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param          key              (object) - The key.
        /// \param          enumClass        (string) - The enum class.
        /// \param [in,out] constsString    (ref string) - The consts string.
        /// \param          targetSubject    (string) - Target subject.
        /// \param          targetAlgorithm  (string) - Target algorithm.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string InitMethodString(object key, string enumClass, string targetSubject, string targetAlgorithm)
        {
            string eol = Environment.NewLine;
            string s = eol + " " + eol + "\t\tprotected AttributeDictionary " + this.InitMethodName() + "()";
            s += eol + "\t\t{";
            s += eol + "\t\t\tAttributeDictionary dictionary = new AttributeDictionary();";
            s += eol + "\t\t\tdictionary.selfEnumName = " + "\"" + this.DictionaryEnum(ClassFactory.GenerateNamespace(targetSubject, targetAlgorithm), enumClass) + "\";";
            foreach (var entry in this)
            {
                if (!KeyOfThisClass(entry.Key, targetSubject, targetAlgorithm))
                {
                    continue;
                }

                string methodName = entry.Value.InitMethodName() + "()";
                s += eol + "\t\t\tdictionary.Add(" + this.AttributeKey((object)entry.Key, enumClass) +
                             ", " + entry.Value.InitString("", methodName) + ");";
            }
            s += eol + "\t\t\treturn dictionary;";
            s += eol + "\t\t}";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool KeyOfThisClass(dynamic key, string targetSubject, string targetAlgorithm)
        ///
        /// \brief Key of this class.
        ///
        /// \par Description.
        ///      -  Checks is the key is a key of this class  
        ///      -  When generating the code the dictionaries can be composed from attributes that their keys  
        ///         are not in the class of the dictionary. In this case the attribute of the key and the key
        ///         should not belong to the dictionary method making.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param key             (dynamic) - The key.
        /// \param targetSubject   (string) - Target subject.
        /// \param targetAlgorithm (string) - Target algorithm.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool KeyOfThisClass(dynamic key, string targetSubject, string targetAlgorithm)
        {
            // If the key is string this is a key that was generated in this operation
            if (key is string)
                return true;

            // If the key belongs to this class (Updating a class)
            string newAlgorithmNameStace = targetSubject + "." + targetAlgorithm;
            string keyTypeString = key.GetType().ToString();
            if (keyTypeString.IndexOf(newAlgorithmNameStace) != -1)
                return true;
            return false;
        }
        #endregion
    }
}
