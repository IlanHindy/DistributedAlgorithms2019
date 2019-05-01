////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file infrastructure\attributelist.cs
///
/// \brief Implements the AttributeList class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    #region /// \name List Extensions

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ListExtensions
    ///
    /// \brief A list extensions.
    ///        This class is used in order to provide a casting possibility
    ///        to cast between the List&lt;Attribute&gt; which is the result of the
    ///        Linq operation to AttributeList.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///        Whenever you have a List&lt;Attribute&gt; call the method ToAttributeList()
    ///
    /// \author Ilanh
    /// \date 26/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class ListExtensions
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static AttributeList ToAttributeList(this List<Attribute> list)
        ///
        /// \brief A List&lt;Attribute&gt; extension method that converts a list to an attribute
        ///        list.
        ///
        /// \par Description.
        ///      Generate Attribute list from List
        ///      -# If the list is a list of attributes - Generate a list wit the attributes
        ///      -# Else - Generate Attributes with the values and insert them to the AttributeList  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param list The list to act on.
        ///
        /// \return List as an AttributeList.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static AttributeList ToAttributeList(this IList list)
        {
            Type genericType = list.GetType().GenericTypeArguments[0];

            AttributeList attributeList = new AttributeList();
            foreach (var value in list)
            {
                // The Add method of the AttributeList has the following behavior
                // If the value is Attribute add it to the list
                // Else generate Attribute with the value and add it to the list
                attributeList.Add(value);
            }
            return attributeList;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void FromAttributeList(this IList list, AttributeList attributeList)
        ///
        /// \brief An IList extension method that from attribute list.
        ///
        /// \par Description.
        ///      -  Convert from AttributeList to list  
        ///      -  Conversion rules  
        ///         -#  If the type of the list generic type is Attribute copy the attributes
        ///         -#  Else copy the values of the Attribute list only if they has the same type
        ///             to the list (A message informs for a refusal)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/01/2018
        ///
        /// \param list          The list to act on.
        /// \param attributeList  (AttributeList) - List of attributes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void FromAttributeList(this IList list, AttributeList attributeList)
        {
            Type genericType = list.GetType().GenericTypeArguments[0];
            if (genericType.Equals(typeof(Attribute)))
            {
                foreach (Attribute attribute in attributeList)
                {
                    list.Add(attribute);
                }
            }
            else
            {
                for(int idx = 0; idx < attributeList.Count; idx ++)
                {
                    dynamic value = attributeList[idx];
                    if (value.GetType().Equals(genericType))
                    {
                        list.Add(value);
                    }
                    else
                    {
                        MessageRouter.MessageBox("Cannot convert attribute indexed : " + idx.ToString() + "\n"+
                            "from Attribute value type " + value.GetType().ToString() + "\n" +
                            "To list type : " + genericType.ToString(), 
                            "Convert AttributeList to List", null, Icons.Error);
                    }
                }
            }
        }
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class AttributeList
    ///
    /// \brief List of attributes.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 25/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AttributeList: List<Attribute>, IValueHolder
    {
        #region /// \name Members

        /// \brief (IValueHolder) - The parent. - The Attribute that this AttributeList is assigned to.
        protected IValueHolder parent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Permissions) - The permissions.
        ///        Define the permissions for add/remove/clear/set actions according to the program
        ///        activation phase.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected Permissions permissions;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (BaseNetwork) - The network.
        ///         The Network that is handled (presented, running, etc.)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected BaseNetwork network;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public NetworkElement Element
        ///
        /// \brief Gets the element.
        ///
        /// \return The element.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NetworkElement Element { get { return element; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (NetworkElement) - The element.
        ///        The NetworkElement that the AttributeList belongs to (network, process etc.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private NetworkElement element;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (int) - The code attribute counter.
        ///        This counter is used for setting a unique id to attributes generated when adding new
        ///        algorithm in AddNewAlgorithm window.
        ///        The unique id is used in the NetworkUpdate process
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int codeAttributeCounter = 0;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (int) - The design attribute counter.
        ///        This counter is used for setting a unique id to attributes generated in design mode
        ///        algorithm in ElementInputWindow.
        ///        The unique id is used in the NetworkUpdate process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int designAttributeCounter = 0;
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
        /// \fn public new dynamic this(int idx)
        ///
        /// \brief Indexer to get or set items within this collection using array index syntax.
        ///
        /// \param idx (int) - The index.
        ///
        /// \return The indexed item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new dynamic this[int idx]
        {
            get { return this.ElementAt(idx).Value; }
            set { this.ElementAt(idx).Value = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public dynamic this(string idInList)
        ///
        /// \brief Indexer to get or set items within by the idInList.
        ///
        /// \param idInList  (string) - List of identifier INS.
        ///
        /// \return The indexed item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic this[string idInList]
        {
            get { return this.First(a => a.IdInList == int.Parse(idInList)).Value; }
            set { this.First(a => a.IdInList == int.Parse(idInList)).Value = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute GetAttribute(int idx)
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
        /// \param idx (dynamic) - The key.
        ///
        /// \return The attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute GetAttribute(int idx)
        {
            return this.ElementAt(idx);
        }

        public Attribute GetAttribute(string idInList)
        {
            return this.FirstOrDefault(a => a.IdInList == int.Parse(idInList));
        }
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public AttributeList()
        ///
        /// \brief Default constructor.
        ///
        /// \par Description.
        ///      Generate an empty AttributeList.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeList()
        {
            permissions = new Permissions(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public AttributeList(List<Attribute> list):base(list.ToList())
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Generate AttributeList from List&lt;Attribute&gt;
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param list (List&lt;Attribute&gt;) - The list.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeList(List<Attribute> list)
        {
            permissions = new Permissions(true);

            foreach (Attribute attribute in list)
            {
                Add(attribute);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public AttributeList(AttributeList attributeList):base(attributeList.ToList())
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      Generate AttributeList from AttributeList.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2017
        ///
        /// \param attributeList (AttributeList) - List of attributes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeList(AttributeList attributeList)
        {
            permissions = new Permissions(true);
            foreach (Attribute attribute in attributeList)
            {
                Add(attribute);
            }
        }
        #endregion
        #region /// \name Adding and Removing

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Add(dynamic value, ElementWindow sender = null)
        ///
        /// \brief Adds attribute.
        ///
        /// \par Description.
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
        /// \param value   (dynamic) - The value.
        /// \param sender (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Add(dynamic value, bool updateIdInList = true, ElementWindow sender = null)
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

            if (!permissions.CheckPermition(Permissions.PermitionTypes.AddRemove, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to add attribute to List :" +  TypesUtility.GetKeyToString(parent.GetChildKey(this)) + " because of add/remove permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeList Add", null, Icons.Error);
                return false;
            }

            // Handle the IdInList:
            // If we are generating code we give the IdInList the value of the code counter
            // Else we give the IdInList the value of the design counter
            if (updateIdInList)
            {
                if (sender is AddAlgorithmWindow)
                {
                    attribute.IdInList = ++codeAttributeCounter;
                }
                else
                {
                    attribute.IdInList = --designAttributeCounter;
                }
            }

            base.Add(attribute);

            // The following command will set the members to all the tree under the attribute
            attribute.SetMembers(network, element, permissions, this);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Remove(Attribute attribute, ElementWindow sender = null)
        ///
        /// \brief Removes the given attribute.
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
        /// \param attribute The attribute to remove.
        /// \param sender    (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Remove(Attribute attribute, ElementWindow sender = null)
        {

            if (!permissions.CheckPermition(Permissions.PermitionTypes.AddRemove, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to remove attribute (index : " + TypesUtility.GetKeyToString(GetChildKey(attribute)) + ") from List :" +  TypesUtility.GetKeyToString(parent.GetChildKey(this)) + " because of add/remove permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeList Remove", null, Icons.Error);
                return false;
            }
            base.Remove(attribute);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Insert(int index, Attribute attribute, ElementWindow sender = null)
        ///
        /// \brief Inserts.
        ///
        /// \par Description.
        ///       -# Check if there is a permission to add new attribute according to the phase or the execution
        ///      -# Insert the attribute (using a constructor that gets the network, permissions, parent)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/07/2017
        ///
        /// \param index     (int) - Zero-based index of the.
        /// \param attribute (Attribute) - The attribute.
        /// \param sender    (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Insert(int index, Attribute attribute, ElementWindow sender = null)
        {
            if (!permissions.CheckPermition(Permissions.PermitionTypes.AddRemove, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to insert attribute (index : " + TypesUtility.GetKeyInParent(parent, this) + "from List :" +  TypesUtility.GetKeyToString(parent.GetChildKey(this)) + " because of add/remove permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeList Insert", null, Icons.Error);
                return false;
            }

            base.Insert(index, attribute);

            // Handle the IdInList:
            // If we are generating code we give the IdInList the value of the code counter
            // Else we give the IdInList the value of the design counter
            if (sender is AddAlgorithmWindow)
            {
                attribute.IdInList = codeAttributeCounter++;
            }
            else
            {
                attribute.IdInList = designAttributeCounter--;
            }

            // The following command will set the members to all the tree under the attribute

            attribute.SetMembers(network, element, permissions, this);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool RemoveAt(int index, ElementWindow sender = null)
        ///
        /// \brief Removes at described by index.
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
        /// \param index  (int) - Zero-based index of the.
        /// \param sender (Optional)  (ElementWindow) - The sender.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool RemoveAt(int index, ElementWindow sender = null)
        {
            if (!permissions.CheckPermition(Permissions.PermitionTypes.AddRemove, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to remove attribute (index : " + index.ToString() + ") from List :" +  TypesUtility.GetKeyInParent(parent, this) + " because of add/remove permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.AddRemove) },
                            "AttributeList RemoveAt", null, Icons.Error);
                return false;
            }
            base.RemoveAt(index);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Clear(ElementWindow sender = null)
        ///
        /// \brief Clears this object to its blank/initial state.
        ///
        /// \par Description.
        ///      -# Check the permission to activate Clear operation according to the execution phase
        ///      -# Clear the list.
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

        public bool Clear(bool clearCounters = false, ElementWindow sender = null)
        {
            if (!permissions.CheckPermition(Permissions.PermitionTypes.Clear, sender))
            {
                MessageRouter.MessageBox(new List<string> { "Failed to clear list " + TypesUtility.GetKeyToString(parent.GetChildKey(this)) + " because of clear permission\n",
                            "Program phase is :" + TypesUtility.GetKeyToString(MainWindow.ActivationPhase),
                            "Permission checked : " + TypesUtility.GetKeyToString(Permissions.PermitionTypes.Clear) },
                            "AttributeList Clear", null, Icons.Error);
                return false;
            }
            base.Clear();
            if (clearCounters)
            {
                codeAttributeCounter = 0;
                designAttributeCounter = 0;
            }
            return true;
        }
        #endregion
        #region /// \name IValueHolder - Load and Save

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Load(XmlNode xmlNode, Type hostType = null)
        ///
        /// \brief Loads. Load the object from XML doc (or file)
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
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                switch (xmlAttribute.Name)
                {
                    case "CodeAttributeCounter":
                        codeAttributeCounter = int.Parse(xmlAttribute.Value);
                        break;
                    case "DesignAttributeCounter":
                        designAttributeCounter = int.Parse(xmlAttribute.Value);
                        break;
                }
            }
            foreach (XmlNode xmlEntryNode in xmlNode.ChildNodes)
            {
                Attribute attribute = new Attribute();
                if (!attribute.Load(xmlEntryNode, hostType))
                {
                    return false;
                }
                Add(attribute, false);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode)
        ///
        /// \brief Saves. Saves the object to XML doc (or file)
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
            //The code attribute counter is inserted as an attribute of the list
            XmlAttribute xmlCodeCounterAttribute = xmlDoc.CreateAttribute("CodeAttributeCounter");
            xmlCodeCounterAttribute.Value = codeAttributeCounter.ToString();
            xmlNode.Attributes.Append(xmlCodeCounterAttribute);

            //The design attribute counter is inserted as an attribute of the list
            XmlAttribute xmlDesignCounterAttribute = xmlDoc.CreateAttribute("DesignAttributeCounter");
            xmlDesignCounterAttribute.Value = designAttributeCounter.ToString();
            xmlNode.Attributes.Append(xmlDesignCounterAttribute);

            //The attributes of the list
            for (int idx = 0; idx < Count; idx++)
            {
                XmlNode entryNode = xmlDoc.CreateElement("idx_" + idx.ToString());
                GetAttribute(idx).Save(xmlDoc, entryNode);
                xmlNode.AppendChild(entryNode);
            }
            return xmlNode;
        }
        #endregion
        #region /// \name IValueHolder Scanning operations

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ScanAndReport(IScanConsumer scanConsumer, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, dynamic key = null)
        ///
        /// \brief Scans and activate.
        ///        This method implements a deep scanning of all the attributes in the list.  
        ///
        /// \par Description.
        ///      -  The scanning process is a process that gives to the scanConsumer all the attributes
        ///         that are found under IValueHolder Tree
        ///      -  The scanning process travels on the tree and reports to the scanConsumer on each attribute  
        ///         It reached.
        ///      -  For the AttributeList we have to report all the attributes in the list.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
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
            for (int idx = 0; idx < Count; idx++)
            {
                string keyString = "[" + idx.ToString() + "]";
                GetAttribute(idx).ScanAndReport(scanConsumer, mainDictionary, dictionary, keyString);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool print = false, bool checkNotSameObject = false)
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
        /// \param          print              (Optional)  (bool) - true to print.
        /// \param          checkNotSameObject (Optional)  (bool) - true to check not same object.
        ///
        /// \return True if equals to, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool EqualsTo(int nestingLevel, ref string error, IValueHolder other, bool print = false, bool checkNotSameObject = false)
        {
            AttributeList otherList = (AttributeList)other;

            if (Count != otherList.Count)
            {
                error = "The count of one is : " + Count.ToString() + " The count of two is : " + otherList.Count.ToString();
                return false;
            }

            for (int idx = 0; idx < otherList.Count; idx++)
            {
                if (!GetAttribute(idx).CheckEqual(nestingLevel + 1, idx.ToString(), otherList.GetAttribute(idx), print, checkNotSameObject))
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

            // The method is keeping the variables related to the attribute counters
            // because the value of these variables should be set only once
            codeAttributeCounter = ((AttributeList)source).codeAttributeCounter;
            designAttributeCounter = ((AttributeList)source).designAttributeCounter;
            Clear(false);
            foreach(Attribute entry in (AttributeList)source)
            {
                Attribute attribute = new Attribute();
                attribute.DeepCopy(entry, false);
                Add(attribute, false);
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
            for (int idx = 0; idx < Count; idx++)
            {
                if (!ValueHolder.CheckMembers(GetAttribute(idx), network, element, permissions, this, nestingLevel + 1, "[" + idx.ToString() + "]"))
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
                foreach (Attribute attribute in this)
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
            valueString = "";
            separator = "-";
            if (Count > 0)
            {
                valueString = "List : ";
                foreach (Attribute attribute in this)
                {
                    valueString += attribute.Value.ToString() + ";";
                }
            }
            else
            {
                valueString = "Empty";
            }
            return true;
        }
        #endregion
        #region /// \name IValueHolder ElementWindow support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable, bool attributeEdittable)
        ///
        /// \brief Default new value field data.
        ///
        /// \par Description.
        ///      -  This method sets the parameter for presenting/changing controls in ElementWindow.  
        ///      -  The ElementWindow has 3 controls for each field  
        ///         -#  Header which usually contains the key of the attribute
        ///         -#  Existing value TextBox
        ///         -#  New Value control for changing the values
        ///      -  This method sets the default parameters for generating the new value controls for  
        ///         AttributeList.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms  (ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
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
            bool editable = attributeEdittable;
                       
            if (inputWindow == InputWindows.AddAlgorithmWindow)
            {
                if (inputWindow == InputWindows.AddAlgorithmWindow)
                {
                    editable = this.AddAlgorithmWindowAttributeEditable(mainNetworkElement, mainDictionary);
                }
            }

            if (editable)
            {
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.AddRemovePanel;
            }
            else
            {
                elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
                elementWindowPrms.newValueControlPrms.Value = "AttributeList";
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
        /// \param [in,out] elementWindowPrms  (ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
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
            elementWindowPrms.typeString = "AttributeList" + " " + MembersStr();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow)
        ///
        /// \brief Default existing value field data.
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
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms  (ElementWindowPrms) - The element window prms.
        /// \param          mainNetworkElement  (NetworkElement) - The main network element.
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
            
            elementWindowPrms.existingValueText = "AttributeList";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string MembersStr()
        ///
        /// \brief Members string.
        ///
        /// \par Description.
        ///      Writing the members to string
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
            string s = "(" + ((Attribute)parent).IdInList.ToString() + "," + ((Attribute)parent).Changed.ToString()[0];
            s += ", " + codeAttributeCounter.ToString() + ", " + designAttributeCounter.ToString() + ")";
            return s;
        }
        #endregion
        #region /// \name IValueHolder Get Child Key

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public dynamic GetChildKey(IValueHolder child)
        ///
        /// \brief Gets a child key.
        ///
        /// \par Description.
        ///      Get the key of the dictionary in the parent of the attribute that this AttributeList is assigns to.
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
            return IndexOf((Attribute)child);
        }
        #endregion
        #region \name Conversions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public IList AsList()
        ///
        /// \brief Converts this object to a list.
        ///
        /// \par Description.
        ///      The conversion will succeed if the AttributeList is not empty and
        ///      the types of all the values is the same
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 02/05/2018
        ///
        /// \return An IList.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IList AsList()
        {
            if (Count == 0)
            {
                MessageRouter.MessageBox(new List<string> { "There is no way to convert empty AttributeList to List when the AttributeList is empty" }, "AttributeList", null, Icons.Error);
                return this;
            }
            Type genericType = this[0].GetType();
            foreach (Attribute attribute in this)
            {
                if (!attribute.Value.GetType().Equals(genericType))
                {
                    MessageRouter.MessageBox(new List<string>
                    {
                        "In Order to convert from AttributeList to List the attribute value types has to be the same",
                        "Returning the AttributeList"
                    }, "AttributeList", null, Icons.Error);
                    return this;
                }
            }
            IList list = (IList)TypesUtility.CreateListFromArgumentTypeString(genericType.ToString());
            foreach (Attribute attribute in this)
            {
                list.Add(attribute.Value);
            }
            return list;
        }
            
        #endregion
        #region /// \name Add algorithm window support (creating c# code)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string MessageMethodString(string enumClass)
        ///
        /// \brief Message method string.
        ///
        /// \par Description.
        ///      -  Create a code for a message component that is this list
        ///      -  The code generated has the following parameters:  
        ///         -#  The way the user wants to fill the dictionary
        ///             -#  bm.PrmSource.MainPrm - Take the second parameter which is a list
        ///             -#  bm.PrmSource.Default - Take the default values of the parameters
        ///             -#  bm.PrmSource.Prms - Take the value from the parameters
        ///         -#  An optional main parameter which is a list to be passes
        ///         -#  A variable number of parameters to be inserted to the list.  
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///      The following is the ways a call can be made for this method:
        ///~~~{.cs}
        ///      // The first option - Getting a value from the list parameter
        ///      public AttributeList MessageDataFor_Ad_Al( bm.PrmSource.MainPrm, list);
        ///      
        ///      // The second option - Default value (sets when the algorithm code was built)
        ///      public AttributeList MessageDataFor_Ad_Al( bm.PrmSource.Default);
        ///      
        ///      // The third option - Value for each parameter
        ///      public AttributeList MessageDataFor_Ad_Al( bm.PrmSource.Prms, null, true, 1, "string");
        ///~~~
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
            string s = eol + " " + eol + "\t\tpublic AttributeList " + this.MessageMethodName() + "( bm.PrmSource prmSource," +
                    eol + "\t\t\tAttributeList list = null," +
                    eol + "\t\t\tparams dynamic[] prms)";          

            // Method body
            s += eol + "\t\t{";

            // If the source is the main prm
            s += eol + "\t\t\tif (" + "prmSource == bm.PrmSource.MainPrm)";
            s += eol + "\t\t\t{";
            s += eol + "\t\t\t\treturn list;";
            s += eol + "\t\t\t}";

            // If the source is Default
            s += eol + " " + eol + "\t\t\tlist = new AttributeList();";

            s += eol + "\t\t\tif (" + "prmSource == bm.PrmSource.Default)";
            s += eol + "\t\t\t{";
            foreach (Attribute attribute in this)
            {
                string methodName = attribute.MessageMethodName() + "(bm.PrmSource.Default)";
                s += eol + "\t\t\t\tlist.Add("  + attribute.InitString("", methodName) + ", false);";
            }
            s += eol + "\t\t\t\treturn list;";
            s += eol + "\t\t\t}";

            // Else PrmSource.Prms - Create attribute with the values in the parameters
            s += eol + " " + eol + "\t\t\tfor (int i = 0; i < prms.Length; i++)";
            s += eol + "\t\t\t{";
            s += eol + "\t\t\t\tlist.Add(new Attribute { Value = prms[i] });";
            s += eol + "\t\t\t}";         
            s += eol + "\t\t\treturn list;";
            s += eol + "\t\t}";
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string InitMethodString(object key, string enumClass, ref string constsString, string targetSubject, string targetAlgorithm)
        ///
        /// \brief Init method string.
        ///
        /// \par Description.
        ///      Create code for an init method for a list.
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
            string s = eol + "\t\tprotected AttributeList " + this.InitMethodName() + "()";
            s += eol + "\t\t{";
            s += eol + "\t\t\tAttributeList list = new AttributeList();";
            s += eol + "\t\t\tlist.codeAttributeCounter = " + codeAttributeCounter.ToString() + ";";
            s += eol + "\t\t\tlist.designAttributeCounter = 0;";
            foreach (Attribute attribute in this)
            {
                string methodName = attribute.InitMethodName() + "()";
                s += eol + "\t\t\tlist.Add(" + attribute.InitString("", methodName) + ", false);";
            }
            s += eol + "\t\t\treturn list;";
            s += eol + "\t\t}";
            return s;
        }
#endregion
    }
}
