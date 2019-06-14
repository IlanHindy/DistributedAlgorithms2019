////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\NetworkElement.cs
///\brief #### Usage Noteser
///\brief   Implements the network element class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using DistributedAlgorithms;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    #region /// \name BuildCorrection

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class BuildCorrectionParameters
    ///
    /// \brief A build correction parameters.
    ///
    /// \par Description
    ///        -    The check build phase is composed from 2 majour stages:
    ///        -    Perform checks on the NetworkElement according to the specifications of the algorithm
    ///             -   If an error was detected, an object from this class is created in order to
    ///                 set the way the error is presented and corrected
    ///        -    Present the errors and correct them according to the object filled by the checking method.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 26/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class BuildCorrectionParameters
    {
        /// \brief Message describing the error.
        public string ErrorMessage = "";
        /// \brief The correction method.
        public BuildErrorCorrectionMethod CorrectionMethod = null;
        /// \brief Options for controlling the correction.
        public object CorrectionParameters = null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \fn public delegate void BuildErrorCorrectionMethod(object correctionParameters);
    ///
    /// \brief Builds error correction method.
    ///
    /// \author Ilanh
    /// \date 26/04/2017
    ///
    /// \param correctionParameters (object) - Options for controlling the correction.
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public delegate void BuildErrorCorrectionMethod(object correctionParameters);
    #endregion
    #region /// \name enums

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ne
    ///
    /// \brief A ne.
    ///
    /// \par Description.
    ///      Values to be used in the OperationResults dictionary.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 25/06/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ne
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum eak
        ///
        /// \brief Keys to be used in the ElementAttributes dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        public enum eak { Edited, Type, Id }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum pak
        ///
        /// \brief Keys used for the Private Attributes dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum pak { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum opk
        ///
        /// \brief Keys used for the OperationParameters dictionary
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum opk { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ork
        ///
        /// \brief Keys to be used in the OperationResults dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ork { }
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class NetworkElement
    ///
    /// \brief A network element.
    ///
    /// \par Description.
    ///
    /// \par The class implements 2 major subjects
    ///      -#   The implementation for the class as the base class for the network components
    ///           This is done by having virtual methods for the versiose phases of the application running
    ///      -#   The implementation for the class for using as an attribute value
    ///           This is done by implementing the IValueHolder methods.
    /// \par The class has 4 dictionaries that holds the attributes and results of the operation :
    ///      -#  ElementAttributes.Value -  Holds the attributes common to all the class inherited(For
    ///          Example all the classes that inherits from Process)
    ///      -#  PrivateAttributes.Value - Holds the attributes private to each class
    ///      -#  OperationParameters - Holds attributes used for the operation (like breakpoints, events etc..)
    ///      -#  OperationResults.Value - Holds the results of the operation for end running reporting.
    /// \par The class has 1 dictionary that holds the presentation parameters for changing the GUI:
    ///      -#  PresentationParameters.
    /// \par The class has 2 dictionaries that are used to backup the parameter before running and
    ///      restoring them after running:
    ///      -#  OperationResultsBackup.Value
    ///      -#  PresentationParametersBackup.Value.
    ///      
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 26/04/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class NetworkElement : IValueHolder
    {
        #region /// \name Enums

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \enum ElementDictionaries
        ///
        /// \brief Values that represent element dictionaries.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public enum ElementDictionaries { ElementAttributes, PrivateAttributes, OperationParameters, OperationResults, OperationResultsBackup, PresentationParameters, PresentationParametersBackup, None }

        
        /// \brief  (static List&lt;string&gt;) - List of names of the dictionaries.
        public static List<string> DictionariesNames = new List<string>() { "ElementAttributes", "PrivateAttributes", "OperationParameters", "OperationResults", "OperationResultsBackup", "PresentationParameters", "PresentationParametersBackup" };

        /// \brief  (static List&lt;string&gt;) - List of names of the short dictionaries.
        public static List<string> ShortDictionariesNames = new List<string>() { "ea", "pa", "op", "or", "orb", "pp", "ppb" };

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (static List&lt;bool&gt;) - The operational dictionaries.
        ///        whether a dictionary is a backup ictionary or not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<bool> OperationalDictionaries = new List<bool>() { true, true, true, true, false, true, false };
        #endregion
        #region /// \name Members
        /// \brief (List&lt;Attribute&gt;) - The attributes that holds the AttributeDictionaries.
        private List<Attribute> attributes = new List<Attribute>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (Permissions) - The permissions.
        ///        Define the permissions for add/remove/clear/set actions according to the program
        ///        activation phase.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected Permissions permissions = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (IValueHolder) - The parent. The parent is the AttributeDictionary/AttributeList/NetworkElement
        ///        that the attribute is included in.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected IValueHolder parent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (BaseNetwork) - The network.
        ///         The Network that is handled (presented, running, etc.)
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected BaseNetwork network;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief (NetworkElement) - The element.
        ///        The NetworkElement that the attribute belongs to (network, process etc.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected NetworkElement element;

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

        /// \brief  (Presentation) - The presentation.
        public Presentation Presentation = null;
        #endregion
        #region /// \name Dictionaries access

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public AttributeDictionary ea
        ///
        /// \brief Gets the ElementAttributes.
        ///
        /// \return The ElementAttributes dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary ea { get { return attributes[(int)ElementDictionaries.ElementAttributes].Value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public AttributeDictionary pa
        ///
        /// \brief Gets the PrivateAttributes.
        ///
        /// \return The PrivateAttributes dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary pa { get { return attributes[(int)ElementDictionaries.PrivateAttributes].Value; } }

        public AttributeDictionary op { get { return attributes[(int)ElementDictionaries.OperationParameters].Value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public AttributeDictionary or
        ///
        /// \brief Gets the OperationResults.
        ///
        /// \return The OperationResults dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary or { get { return attributes[(int)ElementDictionaries.OperationResults].Value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public AttributeDictionary orb
        ///
        /// \brief Gets the OperationResultsBackup.
        ///
        /// \return The OperationResultsBackup dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary orb { get { return attributes[(int)ElementDictionaries.OperationResultsBackup].Value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public AttributeDictionary pp
        ///
        /// \brief Gets the PresentationParameters.
        ///
        /// \return The PresentationParameters dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary pp { get { return attributes[(int)ElementDictionaries.PresentationParameters].Value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \property public AttributeDictionary ppb
        ///
        /// \brief Gets the PresentationParametersBackup.
        ///
        /// \return The PresentationParametersBackup dictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AttributeDictionary ppb { get { return attributes[(int)ElementDictionaries.PresentationParametersBackup].Value; } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<AttributeDictionary> Dictionaries()
        ///
        /// \brief Gets the dictionaries.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/06/2017
        ///
        /// \return A List&lt;AttributeDictionary&gt;
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<AttributeDictionary> Dictionaries()
        {
            return new List<AttributeDictionary> { ea, pa, op, or, orb, pp, ppb };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public dynamic this(ElementDictionaries dictionary)
        ///
        /// \brief Indexer to get dictionary using dictionary key
        ///
        /// \param dictionary  (ElementDictionaries) - The dictionary.
        ///
        /// \return The indexed item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic this[ElementDictionaries dictionary]
        {
            get { return Dictionaries()[(int)dictionary]; }
        }
        #endregion
        #region ///\name Indexers access

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public dynamic this(dynamic key)
        ///
        /// \brief Indexers access to the NetworkElement
        ///        -    There are 3 types of indexers access to the NetworkElement:  
        ///             -#  When the index is ElementAttribute - returns a dictionary
        ///             -#  When the index is string :
        ///                 -   Find in all the dictionaries a key with the name equals to the index  
        ///                 -   Together with the definitions in BaseDefsAndInits.cs support the following   
        ///                     access:  
        ///~~~{.cs}
        /// // If, for example, the OperationResults includes a key p.ork.SomeKey
        /// // The regular call is:
        /// ObjectName.or[p.ork.SomeKey]
        ///
        /// // The string index call is 
        /// ObjectName[SomeKey]
        ///~~~
        ///                     
        ///             -#  Else:
        ///                 -   Find in all the dictionaries the key as the index  
        ///                 -   This supports the following access method:  
        ///~~~{.cs}
        /// // If, for example, the OperationResults includes a key p.ork.SomeKey
        /// // The regular call is:
        /// ObjectName.or[p.ork.SomeKey]
        ///
        /// // The key index call is
        /// ObjectName[p.ork.SomeKey]
        ///~~~  
        ///                 -  
        ///
        /// \param key  (dynamic) - The key.
        ///
        /// \return The indexed item.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic this[dynamic key]
        {
            get
            {
                if (key is ElementDictionaries)
                {
                    return Dictionaries()[(int)key];
                }
                else
                {
                    int dictIdx;
                    key = Key(key, out dictIdx);
                    return Dictionaries()[dictIdx][key];
                }
            }
            set
            {
                if (key is ElementDictionaries)
                {
                    string s = "Setting a dictionary of NetworkElement is not allowed";
                    MessageRouter.MessageBox(new List<string> { s }, "NetworkElement", null, Icons.Error);
                    throw new Exception(s);
                }
                int dictIdx;
                key = Key(key, out dictIdx);
                Dictionaries()[dictIdx][key] = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private dynamic Key(object key, out int dictIdx)
        ///
        /// \brief Keys.
        ///
        /// \par Description.
        ///      -  This method handles the finding of the key in the dictionaries in the  
        ///         following cases:
        ///         -#  The key is a string
        ///         -#  The key is dynamic key
        ///      
        ///
        /// \par Algorithm.
        ///      -# If the key is a string
        ///         -#  The method collects from all the dictionaries all keys with the same name
        ///      -# Else
        ///         -#  The method collects from all the dictionaries all the keys equals to the parameter
        ///      -# If No key where found
        ///         -#  Generate a message and throw an exception
        ///      -# If more than one key exists
        ///         -#  Generate a message and throw an exception
        ///      -# If only one key exists
        ///         -#  Return the key and dictionary      
        ///      
        ///
        /// \par Usage Notes.
        ///      The backup dictionaries are not searched because they has duplication with the operational dictionaries
        ///
        /// \author Ilanh
        /// \date 21/03/2018
        ///
        /// \exception In case there is 0 or more than 1 keys
        ///            
        ///
        /// \param       key      (object) - The key.
        /// \param [out] dictIdx (out int) - Zero-based index of the dictionary.
        ///
        /// \return A dynamic.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private dynamic Key(object key, out int dictIdx)
        {
            List<int> dictionaries = new List<int>();
            List<dynamic> keys = new List<dynamic>();
            if (key is string)
            {

                for (int idx = 0; idx < DictionariesNames.Count; idx++)
                {
                    if (OperationalDictionaries[idx])
                    {
                        foreach (dynamic k in Dictionaries()[idx].Keys)
                        {
                            if ((string)TypesUtility.GetKeyToString(k) == (string)key)
                            {
                                dictionaries.Add(idx);
                                keys.Add(k);
                            }

                        }
                    }
                }
            }
            else
            {
                for (int idx = 0; idx < DictionariesNames.Count; idx++)
                {
                    if (OperationalDictionaries[idx])
                    {
                        foreach (dynamic k in Dictionaries()[idx].Keys)
                        {
                            if (TypesUtility.CompareDynamics(k, key))
                            {
                                dictionaries.Add(idx);
                                keys.Add(k);
                            }
                        }
                    }
                }
            }
            if (dictionaries.Count == 0)
            {
                string s = "No index with name " + TypesUtility.GetKeyToString(key);
                MessageRouter.MessageBox(new List<string> { s }, "NetworkElement");
                throw new Exception(s);
            }
            else if (dictionaries.Count > 1)
            {
                string s = "Duplicate key between :";
                for (int idx = 0; idx < dictionaries.Count; idx ++)
                {
                    string keyString = TypesUtility.GetKeyAndTypeToString(keys[idx]).Replace("+", ".");
                    keyString = keyString.Replace(ClassFactory.GenerateNamespace() + ".", "");
                    keyString = keyString.Replace("DistributedAlgorithms.", "");
                    s += "\n" + DictionariesNames[dictionaries[idx]] + " : " + TypesUtility.GetKeyAndTypeToString(key);
                    s += "\n " + "For selection use : <object name>." + ShortDictionariesNames[dictionaries[idx]] + "[" + keyString + "]";
                }
                MessageRouter.MessageBox(new List<string> { s }, "NetworkElement");
                throw new Exception(s);
            }
            dictIdx = dictionaries[0];
            return keys[0];            
        }
        #endregion
        #region /// \name Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public NetworkElement(BaseNetwork network)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes
        ///      This constructor is for network initiation phase
        ///      This constructor is for creating NetworkElements in which the permissions have to be set
        ///      (The NetworkElements which are part of the network like BaseNetwork, BaseProcess etc..)
        ///      This constructor sets the permissions for using the dictionaries rather than copy it from
        ///      the parent because it creates the elements that are in the network.
        ///
        /// \author Ilanh
        /// \date 20/06/2017
        ///
        /// \param network (BaseNetwork) - The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NetworkElement(BaseNetwork network)
        {
            this.network = network;
            BaseNetwork networkToPass = network;
            if (network is null)
            {
                networkToPass = (BaseNetwork)this;
            }
            element = this;

            // The NetworkElement itself cannot be updated only it's dictionaries
            permissions = new Permissions(false);

            for (int idx = 0; idx < TypesUtility.GetAllEnumValues(typeof(ElementDictionaries)).Count - 1; idx++)
            {
                ElementDictionaries dictionaryKey = (ElementDictionaries)idx;
                Attribute attribute = new Attribute() { Value = new AttributeDictionary(), Changed = false };
                attribute.SetMembers(networkToPass, this, SetDirectoryPermissions(dictionaryKey), this);
                attributes.Add(attribute);
            }
            SetInitEditable();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public NetworkElement(bool permissionsValue)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      This constructor is used when a network element is created not in initiation phase
        ///      (When the NetworkElement is going to be an attribute of another NetworkElement)
        ///      This kind of objects are generated in 2 phases:
        ///
        /// \par -# First created using the constructor that is convenient for the generating
        ///      -# Second setted to an attribute with the constructor that passes to it the
        ///          -#  The Network
        ///          -#  The Permissions
        ///          -#  The NetworkElement that should be setted
        ///          -#  The Parent (The attribute that the NetworkElement is setted to)
        ///
        /// \par This constructor is one of the constructors in the first phase.
        ///
        /// \author Ilanh
        /// \date 25/06/2017
        ///
        /// \par Algorithm.
        ///
        /// \param permissionsValue (bool) - true to permissions value.
        ///
        /// \par Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NetworkElement(bool permissionsValue)
        {
            permissions = new Permissions(permissionsValue);
            for (int idx = 0; idx < TypesUtility.GetAllEnumValues(typeof(ElementDictionaries)).Count - 1; idx++)
            {
                ElementDictionaries dictionaryKey = (ElementDictionaries)idx;
                Attribute attribute = new Attribute() { Value = new AttributeDictionary(), Changed = false };
                attribute.SetMembers(network, element, new Permissions(permissionsValue), this);
                attributes.Add(attribute);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public NetworkElement()
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      This constructor is used when a network element is created not in initiation phase
        ///      (When the NetworkElement is going to be an attribute of another NetworkElement)
        ///      This kind of objects are generated in 2 phases:
        ///
        /// \par -# First created using the constructor that is convenient for the generating
        ///      -# Second setted to an attribute with the constructor that passes to it the
        ///         -#  The Network
        ///         -#  The Permissions
        ///         -#  The NetworkElement that should be setted
        ///         -#  The Parent (The attribute that the NetworkElement is setted to)
        ///
        /// \par This constructor is one of the constructors in the first phase.
        ///
        /// \author Ilanh
        /// \date 25/06/2017
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \param permissionsValue (bool) - true to permissions value.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NetworkElement()
        {
            permissions = new Permissions(true);
            for (int idx = 0; idx < TypesUtility.GetAllEnumValues(typeof(ElementDictionaries)).Count - 1; idx++)
            {
                ElementDictionaries dictionaryKey = (ElementDictionaries)idx;
                Attribute attribute = new Attribute() { Value = new AttributeDictionary(), Changed = false };
                attribute.SetMembers(network, element, permissions, this);
                attributes.Add(attribute);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public NetworkElement(NetworkElement source)
        ///
        /// \brief Constructor.
        ///
        /// \par Description.
        ///      This constructor is used when a network element is created not in initiation phase
        ///      (When the NetworkElement is going to be an attribute of another NetworkElement)
        ///      This kind of objects are generated in 2 phases:
        ///
        /// \par -# First created using the constructor that is convenient for the generating
        ///      -# Second setted to an attribute with the constructor that passes to it the
        ///         -#  The Network
        ///         -#  The Permissions
        ///         -#  The NetworkElement that should be setted
        ///         -#  The Parent (The attribute that the NetworkElement is setted to)
        ///
        /// \par This constructor is one of the constructors in the first group.
        ///
        /// \author Ilanh
        /// \date 25/06/2017
        ///
        /// \par Algorithm.
        ///
        /// \param source (NetworkElement) - Source for the.
        ///
        /// \par Usage Notes.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public NetworkElement(NetworkElement source)
        {
            permissions = new Permissions(true);
            parent = null;
            network = null;
            for (int idx = 0; idx < TypesUtility.GetAllEnumValues(typeof(ElementDictionaries)).Count - 1; idx++)
            {
                ElementDictionaries dictionaryKey = (ElementDictionaries)idx;
                AttributeDictionary dictionary = new AttributeDictionary();
                dictionary.DeepCopy(source[dictionaryKey]);
                Attribute attribute = new Attribute() { Value = dictionary, Changed = false };
                attribute.SetMembers(network, element, permissions, this);
                attributes.Add(attribute);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute CreateWrappingAttribute()
        ///
        /// \brief Creates wrapping attribute.
        ///
        /// \par Description.
        ///      -  This constructor is used in the ElementWindow.  
        ///      -  The ElementWindow gets NetworkElements bu in the Tree only Attributes can be presented
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/07/2017
        ///
        /// \return The new wrapping attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute CreateWrappingAttribute()
        {
            return new Attribute(network, permissions, null, this, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void SetInitEdittable()
        ///
        /// \brief Sets init editable.
        ///        Set the Editable member for the attributes that hold the dictionaries
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/04/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetInitEditable()
        {
            attributes[(int)ElementDictionaries.ElementAttributes].Editable = false;
            attributes[(int)ElementDictionaries.OperationParameters].Editable = false;
            attributes[(int)ElementDictionaries.OperationResultsBackup].Editable = false;
            attributes[(int)ElementDictionaries.PresentationParametersBackup].Editable = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private Permissions SetDirectoryPermissions(ElementDictionaries dictionary)
        ///
        /// \brief Sets directory permissions.
        ///
        /// \par Description.
        ///      -  This method sets the permissions for a NetworkElement which is a part of the network
        ///      -  Each dictionary gets it's own permissions according to the phase of the program and the
        ///         action.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/06/2017
        ///
        /// \param dictionary (ElementDictionaries) - The dictionary.
        ///
        /// \return The Permissions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Permissions SetDirectoryPermissions(ElementDictionaries dictionary)
        {
            Permissions dictionaryPermissions = new Permissions();
            switch (dictionary)
            {
                // ElementAttributes protection
                // This dictionary is created once and it's values can be changed only when not running
                // No backups made for it
                case ElementDictionaries.ElementAttributes:
                    dictionaryPermissions.dictionary = ElementDictionaries.ElementAttributes;
                    dictionaryPermissions.Set(false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Init, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Initiated, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Loaded, true);
                    break;

                // PrivateAttributes protection
                // This dictionary can be changed when not running 
                // No backups made for it
                case ElementDictionaries.PrivateAttributes:
                    dictionaryPermissions.dictionary = ElementDictionaries.PrivateAttributes;
                    dictionaryPermissions.Set(false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Init, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Initiated, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Loaded, true);
                    dictionaryPermissions.Set(Permissions.PermitionTypes.Set, true);
                    break;

                // OperationParameters protection
                // This dictionary can be changed in design mode 
                // This dictionary can be changed in running mode 
                case ElementDictionaries.OperationParameters:
                    dictionaryPermissions.dictionary = ElementDictionaries.OperationParameters;
                    dictionaryPermissions.Set(true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.BeforeRunning, false);
                    break;

                // OperationResults protection
                // This dictionary can be changed in design mode 
                // The dictionary is backup before the running and restored after running 
                case ElementDictionaries.OperationResults:
                    dictionaryPermissions.dictionary = ElementDictionaries.OperationResults;
                    dictionaryPermissions.Set(true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.BeforeRunning, false);
                    break;

                // OperationResultsBackup protection
                // This dictionary is used for backup only so it can be changed only before running
                case ElementDictionaries.OperationResultsBackup:
                    dictionaryPermissions.dictionary = ElementDictionaries.OperationResultsBackup;
                    dictionaryPermissions.Set(false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Init, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.BeforeRunning, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Initiated, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Loaded, true);
                    break;

                // PresentationParameters protection
                // After initiating no Add or Remove is allowed for this dictionary
                // The attributes of this dictionary can be changed in design and running phases
                // The dictionary is backup before the running and restored after running 
                case ElementDictionaries.PresentationParameters:
                    dictionaryPermissions.dictionary = ElementDictionaries.PresentationParameters;
                    dictionaryPermissions.Set(true);
                    dictionaryPermissions.Set(Permissions.PermitionTypes.Clear, false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Initiated, Permissions.PermitionTypes.Clear, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Loaded, Permissions.PermitionTypes.Clear, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Running, Permissions.PermitionTypes.AddRemove, false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Running, Permissions.PermitionTypes.Clear, false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.BeforeRunning, false);
                    break;

                // PresentationParametersBackup protection
                // This dictionary is used for backup only so it can be changed only before running
                case ElementDictionaries.PresentationParametersBackup:
                    dictionaryPermissions.dictionary = ElementDictionaries.PresentationParametersBackup;
                    dictionaryPermissions.Set(false);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Init, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.BeforeRunning, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Initiated, true);
                    dictionaryPermissions.Set(MainWindow.ActivationPhases.Loaded, true);
                    break;
            }
            return dictionaryPermissions;
        }


        #endregion
        #region /// \name Init (methods that are activated while in Init phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Init(int idx, bool clearPresentation = true)
        ///
        /// \brief S.
        ///
        /// \par Description
        ///        The init methods are method operated when generating a network from scrach
        ///        (each component in the network is generated and then the Init method is activated)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param idx               (int) - The index.
        /// \param clearPresentation (Optional)  (bool) - true to clear presentation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Init(int idx, bool clearPresentation = true)
        {
            Clear(clearPresentation);
            InitElementAttributes(idx);
            InitPrivateAttributes();
            InitOperationParameters();
            InitOperationResults();
            InitPresentationParameters();
            CodeGenerationAdditionalInit();
            AdditionalInitiations();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitElementAttributes(int idx)
        ///
        /// \brief Init element attributes.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param idx (int) - The index.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitElementAttributes(int idx)
        {
            ea.Add(ne.eak.Type, new Attribute() { Value = GetType().ToString(), Editable = false, Changed = false, IncludedInShortDescription = false, ElementWindowPrmsMethod = TypeElementWindowPrms});
            ea.Add(ne.eak.Edited, new Attribute() { Value = false, Editable = false, Changed = false, IncludedInShortDescription = false });
            ea.Add(ne.eak.Id, new Attribute() { Value = idx, Editable = false, Changed = false, IncludedInShortDescription = true });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitPrivateAttributes()
        ///
        /// \brief Init private attributes.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitPrivateAttributes()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitOperationParameters()
        ///
        /// \brief Init operation parameters.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/04/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitOperationParameters()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitOperationResults()
        ///
        /// \brief Init operation results.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitOperationResults()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void InitPresentationParameters()
        ///
        /// \brief Init presentation parameters.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitPresentationParameters()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void CodeGenerationAdditionalInit()
        ///
        /// \brief Code generation additional init.
        ///        This method is used for additional initiations done by the code generation
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 15/04/2018
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void CodeGenerationAdditionalInit()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void AdditionalInitiations()
        ///
        /// \brief Additional initiations.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 05/07/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void AdditionalInitiations()
        {
            orb.DeepCopy(or);
            ppb.DeepCopy(pp);
        }
        #endregion
        #region /// \name Design (methods that are activated while in Design phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool ShouldBeEditted()
        ///
        /// \brief Determine if we should be editted.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ShouldBeEditted()
        {
            if (ea.Any(attribute => attribute.Value.Editable != false) ||
                pa.Any(attribute => attribute.Value.Editable != false) ||
                or.Any(attribute => attribute.Value.Editable != false) ||
                pp.Any(attribute => attribute.Value.Editable != false))
            {
                if (ea[ne.eak.Edited] == false)
                {
                    return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckIfIdOfIdExist(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        ///
        /// \brief Determine if identifier of identifier exist.
        ///
        /// \par Description
        ///        This method is used when updating attribute while in design mode
        ///        It checks if there is another NetworkElement from the same type
        ///        with the same id.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
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

        public static bool CheckIfIdOfIdExist(BaseNetwork network, NetworkElement networkElement, Attribute parentAttribute, Attribute attribute, string newValue, out string errorMessage, ElementWindow inputWindow = null)
        {
            errorMessage = "";
            if (typeof(BaseProcess).IsAssignableFrom(networkElement.GetType()))
            {
                if (network.Processes.Exists(p => networkElement.ea[ne.eak.Id].Value.ToString() == newValue) == true)
                {
                    errorMessage = "The id of the process already exist";
                    return false;
                }
            }
            else
            {
                if (network.Channels.Exists(c => c.ea[ne.eak.Id].Value.ToString() == newValue) == true)
                {
                    errorMessage = "The id of the channel already exist";
                    return false;
                }
            }
            return true;
        }
        #endregion
        #region /// \name Check (methods that are activated while in check phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void CheckAndCorrectBuild()
        ///
        /// \brief Check and correct build.
        ///        This is the main method for the check and correct build operation
        ///        -#   Activate the virtual method for the checking (specific to each network element)
        ///        -#   The CheckBuild method fills the buildCorrectionParameters list (one entry for each error)
        ///        -#   For each error in the list show the message of the error and if the user chooses
        ///             To correct the error - activate the correction method.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CheckAndCorrectBuild()
        {
            List<BuildCorrectionParameters> buildCorrectionsParameters = new List<BuildCorrectionParameters>();
            CheckBuild(buildCorrectionsParameters);
            for (int idx = 0; idx < buildCorrectionsParameters.Count; idx++)
            {
                CorrectBuildError(buildCorrectionsParameters[idx]);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected virtual void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        ///
        /// \brief Check build.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param buildCorrectionsParameters (List&lt;BuildCorrectionParameters&gt;) - Options for
        ///                                   controlling the build corrections.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void CheckBuild(List<BuildCorrectionParameters> buildCorrectionsParameters)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private void CorrectBuildError(BuildCorrectionParameters correctionParameters)
        ///
        /// \brief Correct build error.
        ///
        /// \par Description
        ///        Handles one error in the check build phase.
        ///
        /// \par Algorithm
        ///       -# Show a dialog with the error message
        ///       -# If the user chooses to correct the error - activate the correction method.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param correctionParameters (BuildCorrectionParameters) - Options for controlling the
        ///                             correction.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CorrectBuildError(BuildCorrectionParameters correctionParameters)
        {
            MessageBoxResult dialogResult = CustomizedMessageBox.Show(correctionParameters.ErrorMessage + "\nDo you want to automatically correct this error ?", this.GetType().ToString() + ":" + this.ToString() + " Error in network build", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                if (correctionParameters.CorrectionMethod != null)
                {
                    correctionParameters.CorrectionMethod(correctionParameters.CorrectionParameters);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool EditIfNeeded()
        ///
        /// \brief Edit if needed.
        ///
        /// \par Description
        ///      While in the check build phase the network element is tested if it
        ///      was eddited If it wasn't a dialog is shows to ask the user if he wants to edit the network
        ///      element If he selects Yes the edit window for the network element is opened.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return True if no to all was selected.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool EditIfNeeded()
        {
            if (ShouldBeEditted())
            {
                string dialogResult = CustomizedMessageBox.Show(new List<string> { ToString() + "\n Should be edited Do you want to edit it? " },
                    GetType().ToString() + " " + ToString(), new List<string> { "Yes", "No", "No to all" }, Icons.Question);
                switch (dialogResult)
                {
                    case "Yes":
                        ElementInputWindow elementInput = new ElementInputWindow(new List<NetworkElement>() { network });
                        elementInput.ShowDialog();
                        Presentation.UpdatePresentation();
                        return false;
                    case "No":
                        ea[ne.eak.Edited] = false;
                        return false;
                    case "No to all":
                        ea[ne.eak.Edited] = true;
                        return true;

                }
            }
            return false;
        }

        #endregion
        #region /// \name Run (methods that are activated while in running phase)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void BackupPresentationParameters()
        ///
        /// \brief Backup presentation parameters.
        ///
        /// \par Description
        ///        Buckup the presentation parameters before running in order to restore them after running.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void BackupPresentationParameters()
        {
            ppb.DeepCopy(pp);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void BackupOperationResults()
        ///
        /// \brief Backup operation results.
        ///
        /// \par Description
        ///        Buckup the operation results before running in order to restore them after running.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void BackupOperationResults()
        {
            orb.DeepCopy(or);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RestoreOperationResults()
        ///
        /// \brief Restore operation results.
        ///
        /// \par Description
        ///        Restore the operation results from the operation results backup (used after running)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RestoreOperationResults()
        {
            or.DeepCopy(orb);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RestorePresentationParameters()
        ///
        /// \brief Restore presentation parameters.
        ///
        /// \par Description
        ///        Restore the presentation parameters from the presentation parameters backup (used after running)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RestorePresentationParameters()
        {
            pp.DeepCopy(ppb);
        }

        #endregion
        #region /// \name Presentation (methods used to update the presentation)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual string PresentationText()
        ///
        /// \brief Presentation text.
        ///
        /// \par Description
        ///        This method returns the text to be presented while running.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual string PresentationText()
        {
            return ea[ne.eak.Id].ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public virtual void UpdateRunningStatus(object[] parameters)
        ///
        /// \brief Updates the running status described by parameters.
        ///
        /// \par Description
        ///        This method is used to update the running status in the presentation of the network element
        ///        The running status is composed from the network element's presentation parameters (passed automatically)
        ///        and additional parameters passed in the object array.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param parameters (object[]) - Options for controlling the operation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void UpdateRunningStatus(object[] parameters)
        { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public override string ToString()
        ///
        /// \brief Convert this object into a string representation.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return A string that represents this object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public override string ToString()
        {
            return GetType().ToString().Replace("DistributedAlgorithms.", "").Replace(".", " ");
        }
        #endregion
        #region /// \name IValueHolder Load and Save

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool Load(XmlNode xmlNode, Type hostType = null)
        ///
        /// \brief Loads.
        ///
        /// \par Description
        ///        Load the object from xml doc (or file)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \exception Exception Thrown when an exception error condition occurs.
        ///
        /// \param xmlNode  (XmlNode) - The XML node.
        /// \param hostType (Optional)  (Type) - Type of the host.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Load(XmlNode xmlNode, Type hostType = null)
        {
            Clear();
            bool result;
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                int dictIdx = DictionariesNames.IndexOf(childNode.Name);
                result = Dictionaries()[dictIdx].Load(childNode, GetType());

                if (result == false)
                {
                    return false;
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode)
        ///
        /// \brief Saves.
        ///
        /// \par Description
        ///        Saves the object to xml doc (or file)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param xmlDoc  (XmlDocument) - The XML document.
        /// \param xmlNode (XmlNode) - The XML node.
        ///
        /// \return An XmlNode.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public XmlNode Save(XmlDocument xmlDoc, XmlNode xmlNode)
        {
            XmlAttribute detailsAttribute = xmlDoc.CreateAttribute("Details");
            detailsAttribute.Value = ToString();
            xmlNode.Attributes.Append(detailsAttribute);
            List<AttributeDictionary> dictionariesList = Dictionaries();
            for (int idx = 0; idx < dictionariesList.Count; idx++)
            {
                XmlNode xmlDictionaryNode = xmlDoc.CreateElement(DictionariesNames[idx]);
                dictionariesList[idx].Save(xmlDoc, xmlDictionaryNode);
                xmlNode.AppendChild(xmlDictionaryNode);
            }
            return xmlNode;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn protected string XmlToString(XmlDocument xmlDoc)
        ///
        /// \brief XML to string.
        ///
        /// \par Description
        ///        Genarates a string holding the xml of the network element.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param xmlDoc (XmlDocument) - The XML document.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string XmlToString(XmlDocument xmlDoc)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlDoc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public XmlDocument ElementXml()
        ///
        /// \brief Element XML.
        ///
        /// \par Description
        ///        Generate a XmlDocument holding the xml presentation of the network element.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return An XmlDocument.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public XmlDocument ElementXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(Save(xmlDoc, xmlDoc.CreateElement(GetType().ToString())));
            return xmlDoc;
        }
        #endregion
        #region /// \name IValueHolder  Scanning Operation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ScanAndReport(IScanConsumer scanConsumer, ElementDictionaries mainDictionary, ElementDictionaries dictionary, dynamic key = null)
        ///
        /// \brief Scans and report.
        ///
        /// \par Description
        ///        This method implements a deep scanning of all the objects in the
        ///        object and all it's children.  
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
            ElementDictionaries mainDictionary,
            ElementDictionaries dictionary, dynamic key = null)
        {
            ElementDictionaries newMainDictionary = mainDictionary;

            for (int idx = 0; idx < attributes.Count; idx++)
            {
                // If the mainDictionary is None that means that the NetworkElement is the
                // root of the scan (scanning the NetworkElement) in this case the mainDictionary 
                // parameter has to be set
                if (mainDictionary == ElementDictionaries.None)
                {
                    newMainDictionary = (ElementDictionaries)idx;
                }

                if (scanConsumer.ScanCondition((ElementDictionaries)idx, attributes[idx], newMainDictionary, (ElementDictionaries)idx))
                {
                    attributes[idx].ScanAndReport(scanConsumer, newMainDictionary, (ElementDictionaries)idx, (ElementDictionaries)idx);
                }
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
        ///      - The method does the common checks and call this method to propagate the compare to the childrens.
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

            NetworkElement otherElement = (NetworkElement)other;
            List<AttributeDictionary> otherDictionaries = ((NetworkElement)other).Dictionaries();
            for (int idx = 0; idx < attributes.Count; idx++)
            {
                if (!attributes[idx].CheckEqual(nestingLevel + 1, (ElementDictionaries)idx, otherDictionaries[idx].Parent, print, checkNotSameObject))
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
            List<AttributeDictionary> dictionaries = Dictionaries();
            List<AttributeDictionary> sourceDictionaries = ((NetworkElement)source).Dictionaries();
            for (int idx = 0; idx < dictionaries.Count; idx++)
            {
                dictionaries[idx].DeepCopy(sourceDictionaries[idx], isInitiator);
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
        ///      - If you want to start a check you should call : `CheckMembers(string checkName)`
        ///      - The method does the common checks and call this method to propagate the check to the childrens.
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
        public bool CheckMembers(int nestingLevel, bool checkFirstLevel = true)
        {

            // The first case is the case where the the NetworkElement is a part of a network
            // In this case the permissions are different from the permissions of the 
            // NetworkElement
            if (nestingLevel == 0 && !checkFirstLevel)
            {
                for (int idx = 0; idx < attributes.Count; idx++)
                {
                    if (!ValueHolder.CheckMembers(attributes[idx], 
                        (BaseNetwork)((network is null)?this:network), element,
                        SetDirectoryPermissions((ElementDictionaries)idx),
                        this, nestingLevel + 1,
                        TypesUtility.GetKeyToString((ElementDictionaries)idx)))
                        return false;
                }
            }
            else
            {
                for (int idx = 0; idx < attributes.Count; idx++)
                {
                    if (!ValueHolder.CheckMembers(attributes[idx], network, element, permissions, this,
                        nestingLevel + 1, TypesUtility.GetKeyToString((ElementDictionaries)idx)))
                        return false;
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public bool CheckMembers(string checkName)
        ///
        /// \brief Check members.
        ///        This method is a starting method for Checking members of a network element
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
        /// \param checkName  (string) - Name of the check.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CheckMembers(string checkName)
        {
            string toString;
            try
            {
                toString = ToString();
            }
            catch
            {
                toString = GetType().ToString();
            }
            this.GenerateCheckMessage(0, toString, " : Start");
            if (!CheckMembers(0, false))
            {
                MessageRouter.MessageBox(new List<string> { "Check : " + checkName, "The Check for : " + toString + " - Failed" },
                "Check NetworkElement result", null, Icons.Error);
                return false;
            }
            this.GenerateCheckMessage(0, toString, " : End - True");
            MessageRouter.MessageBox(new List<string> { "Check : " + checkName, "The Check for : " + toString + " - Succeeded" },
                "Check NetworkElement result", null, Icons.Success);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true)
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

        public void SetMembers(BaseNetwork network, NetworkElement element, Permissions permissions, IValueHolder parent, bool recursive = true)
        {
            this.network = network;
            this.element = element;
            this.permissions = new Permissions(permissions);
            this.parent = parent;
            this.element = element;

            if (recursive)
            {
                foreach (Attribute attribute in attributes)
                {
                    attribute.SetMembers(network, element, permissions, this);
                }
            }
        }
        #endregion
        #region /// \name IValueHolder Presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string ShortDescription(string separator = " ; ")
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
        /// \date 26/04/2017
        ///
        /// \param separator (Optional)  (string) - The separator.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string ShortDescription(string separator = " ; ")
        {
            List<AttributeDictionary> dictionariesList = Dictionaries();
            string shortDescription = "";
            foreach (AttributeDictionary dictionary in dictionariesList)
            {
                dictionary.ShortDescription(ref shortDescription, separator);
            }
            return shortDescription;
        }

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
        /// \date 26/04/2017
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
            valueString = GetType().ToString();
            return true;
        }
        #endregion
        #region /// \name IValueHolder ElementWindow support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable, bool attributeEdittable)
        ///
        /// \brief Default new value field data.
        ///
        /// \par Description
        ///        This method sets the parameter for generating the New Value control
        ///        for this class.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms  (ElementWindowPrms) - The element window prms.
        /// \param          mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary          (ElementDictionaries) - The dictionary.
        /// \param          inputWindow        (InputWindows) - The input window.
        /// \param          windowEdittable    (bool) - true if window edittable.
        /// \param          attributeEdittable (bool) - true if attribute edittable.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultNewValueFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            ElementDictionaries mainDictionary,
            ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable,
            bool attributeEdittable)
        {
            elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            elementWindowPrms.newValueControlPrms.Value = "NetworkElement";
            elementWindowPrms.newValueControlPrms.enable = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static ElementWindowPrms ElementWindowPrmsDelegate(Attribute attribute, dynamic key, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow, bool windowEdittable)
        ///
        /// \brief Element window prms delegate.
        ///
        /// \par Description.
        ///      Element window params for Type field (leave only the type with no namespaces)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 09/01/2018
        ///
        /// \param attribute           (Attribute) - The attribute.
        /// \param key                 (dynamic) - The key.
        /// \param mainNetworkElement  (NetworkElement) - The main network element.
        /// \param mainDictionary      (ElementDictionaries) - Dictionary of mains.
        /// \param dictionary          (ElementDictionaries) - The dictionary.
        /// \param inputWindow         (InputWindows) - The input window.
        /// \param windowEdittable     (bool) - true if window edittable.
        ///
        /// \return The ElementWindowPrms.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ElementWindowPrms TypeElementWindowPrms(Attribute attribute, 
            dynamic key,
            NetworkElement mainNetworkElement,
            ElementDictionaries mainDictionary,
            ElementDictionaries dictionary,
            InputWindows inputWindow,
            bool windowEdittable)
        {
            ElementWindowPrms elementWindowPrms = new ElementWindowPrms();
            string newValue = (string)attribute.Value;
            newValue = newValue.Substring(newValue.LastIndexOf('.') + 1);
            elementWindowPrms.existingValueText = newValue;
            elementWindowPrms.newValueControlPrms.inputFieldType = InputFieldsType.TextBox;
            elementWindowPrms.newValueControlPrms.enable = false;
            elementWindowPrms.newValueControlPrms.Value = newValue;
            return elementWindowPrms;

        }
      
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement.ElementDictionaries mainDictionary, NetworkElement.ElementDictionaries dictionary, InputWindows inputWindow, dynamic key)
        ///
        /// \brief Default key field data.
        ///
        /// \par Description
        ///        This method is the default method for filling the header TextBox
        ///        key in the ElementWindow for NetworkElement.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/05/2017
        ///
        /// \param [in,out] elementWindowPrms (ElementWindowPrms) - The element window prms.
        /// \param          mainDictionary     (ElementDictionaries) - Dictionary of mains.
        /// \param          dictionary        (ElementDictionaries) - The dictionary.
        /// \param          inputWindow       (InputWindows) - The input window.
        /// \param          key               (dynamic) - The key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DefaultKeyFieldData(ref ElementWindowPrms elementWindowPrms,
            NetworkElement mainNetworkElement,
            ElementDictionaries mainDictionary,
            ElementDictionaries dictionary,
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
        /// \date 09/01/2018
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
            elementWindowPrms.typeString = TypesUtility.GetTypeNameOnlyToString(GetType());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string DefaultExistingValueFieldData(ref ElementWindowPrms elementWindowPrms, NetworkElement mainNetworkElement, ElementDictionaries mainDictionary, ElementDictionaries dictionary, InputWindows inputWindow)
        ///
        /// \brief Default existing value field data.
        ///
        /// \par Description
        ///        This method is the default method for filling the Existing Value
        ///        TextBox in the ElementWindow for attributes from type AttributeList.
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
            ElementDictionaries mainDictionary,
            ElementDictionaries dictionary,
            InputWindows inputWindow)
        {
            elementWindowPrms.existingValueText = TypesUtility.GetTypeNameOnlyToString(GetType()) + " " + MembersStr();
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
            string s = "";
            if (parent != null)
            {
                s += " (" + ((Attribute)parent).IdInList.ToString() + "," + ((Attribute)parent).Changed.ToString()[0] + ")";
            }
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
        ///      Check the key of the NetworkElement in the dictionary or list they are in.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param child (IValueHolder) - The child.
        ///
        /// \return The child key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public dynamic GetChildKey(IValueHolder child)
        {
            return (ElementDictionaries)attributes.IndexOf(attributes.First(a => a == child));
        }
        #endregion
        #region /// \name Attributes Handeling  Utilities

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
        /// \date 26/04/2017
        ///
        /// \param keyString (string) - The key string.
        ///
        /// \return The found attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute FindAttribute(string keyString)
        {
            Attribute result;
            List<AttributeDictionary> dictionariesList = Dictionaries();
            foreach (AttributeDictionary dictionary in dictionariesList)
            {
                if ((result = dictionary.FindAttribute(keyString)) != null)
                    return result;
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void RemoveAttributeIfExist(Attribute attribute, ElementDictionaries elementDictionary)
        ///
        /// \brief Removes the attribute if exist.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param attribute         (Attribute) - The attribute.
        /// \param elementDictionary (ElementDictionaries) - Dictionary of elements.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveAttributeIfExist(Attribute attribute, ElementDictionaries elementDictionary)
        {
            AttributeDictionary dictionary = GetDictionaryFromKey(elementDictionary);
            foreach (var entry in dictionary)
            {
                if (attribute == entry.Value)
                {
                    dictionary.Remove(entry.Key);
                    return;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddAttributeIfNotExist(Attribute attribute, dynamic key, ElementDictionaries elementDictionary)
        ///
        /// \brief Adds an attribute if not exist.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param attribute         (Attribute) - The attribute.
        /// \param key               (dynamic) - The key.
        /// \param elementDictionary (ElementDictionaries) - Dictionary of elements.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddAttributeIfNotExist(Attribute attribute, dynamic key, ElementDictionaries elementDictionary)
        {
            AttributeDictionary dictionary = GetDictionaryFromKey(elementDictionary);
            foreach (var entry in dictionary)
            {
                if (!dictionary.ContainsKey(entry.Key))
                {
                    dictionary.Add(key, attribute);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void AddOrReplaceOperationResult(dynamic key, dynamic attributeValue, bool includeInShortDescription = true)
        ///
        /// \brief Adds an or replace operation result.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param key                       (dynamic) - The key.
        /// \param attributeValue            (dynamic) - The attribute value.
        /// \param includeInShortDescription (Optional)  (bool) - true to include, false to exclude the in
        ///                                  short description.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddOrReplaceOperationResult(dynamic key, dynamic attributeValue, bool includeInShortDescription = true)
        {
            if (or.ContainsKey(key))
            {
                or[key] = attributeValue;
            }
            else
            {
                or.Add(key, new Attribute { Value = attributeValue, IncludedInShortDescription = includeInShortDescription });
            }
        }
        #endregion
        #region /// \name Dictionaries handeling utilities

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public List<List<dynamic>> GetAllKeys()
        ///
        /// \brief Gets all keys.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \return all keys.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<List<dynamic>> GetAllKeys()
        {
            List<List<dynamic>> allKeys = new List<List<dynamic>>();
            allKeys.Add(ea.Keys.ToList());
            allKeys.Add(pa.Keys.ToList());
            allKeys.Add(or.Keys.ToList());
            allKeys.Add(orb.Keys.ToList());
            allKeys.Add(pp.Keys.ToList());
            allKeys.Add(ppb.Keys.ToList());
            return allKeys;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn private AttributeDictionary GetDictionaryFromKey(ElementDictionaries dictionaryKey)
        ///
        /// \brief Gets dictionary from key.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param dictionaryKey (ElementDictionaries) - The dictionary key.
        ///
        /// \return The dictionary from key.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private AttributeDictionary GetDictionaryFromKey(ElementDictionaries dictionaryKey)
        {
            List<AttributeDictionary> dictionariesList = Dictionaries();
            return dictionariesList[(int)dictionaryKey];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public Attribute GetDictionaryAttribute(ElementDictionaries dictionaryKey)
        ///
        /// \brief Gets dictionary attribute.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 05/07/2017
        ///
        /// \param dictionaryKey  (ElementDictionaries) - The dictionary key.
        ///
        /// \return The dictionary attribute.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Attribute GetDictionaryAttribute(ElementDictionaries dictionaryKey)
        {
            return attributes[(int)dictionaryKey];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void Clear(bool clearPresentation = true)
        ///
        /// \brief Clears this object to its blank/initial state.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 26/04/2017
        ///
        /// \param clearPresentation (Optional)  (bool) - true to clear presentation.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Clear(bool clearPresentation = true)
        {
            List<AttributeDictionary> dictionariesList = Dictionaries();
            foreach (AttributeDictionary dictionary in dictionariesList)
            {
                dictionary.Clear();
            }
            if (clearPresentation)
            {
                if (Presentation != null)
                {
                    Presentation.RemoveOnePresentation(this);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void SetDictionaryEdittable(ElementDictionaries dictionary, bool edittable)
        ///
        /// \brief Sets dictionary edittable.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 04/07/2017
        ///
        /// \param dictionary (ElementDictionaries) - The dictionary.
        /// \param edittable  (bool) - true if edittable.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetDictionaryEdittable(ElementDictionaries dictionary, bool edittable)
        {
            attributes[(int)dictionary].Editable = edittable;
        }
        #endregion
        #region /// \name Add algorithm window support (creating c# code)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public string CreateGettersSetters(string enumClass, string targetSubject = "", string targetAlgorithm = "")
        ///
        /// \brief Creates getters setters.
        ///
        /// \par Description.
        ///      This method will generate a string that includes getters/setters to all
        ///      the attribute values in the dictionaries in the NetworkElement for access
        ///      In the following way:
        ///~~~{.cs}
        /// //If, for example, the OperationResults includes a key p.ork.SomeKey
        /// // The regular call is:
        /// ObjectName.or[p.ork.SomeKey]
        /// 
        /// // The Setter/Getter call if
        /// ObjectName.SomeKey
        ///~~~
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 21/03/2018
        ///
        /// \param enumClass        (string) - The enum class.
        /// \param targetSubject   (Optional)  (string) - Target subject.
        /// \param targetAlgorithm (Optional)  (string) - Target algorithm.
        ///
        /// \return The new getters setters.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string CreateGettersSetters(string enumClass, 
            string targetSubject = "", 
            string targetAlgorithm = "",
            string sourceSubject = "",
            string sourceAlgorithm = "")
        {
            string result = "";
            bool isMessage = false;
            if (this is BaseMessage)
            {
                isMessage = true;
            }
            for (int idx = 0; idx < Dictionaries().Count; idx ++)
            {
                if (OperationalDictionaries[idx])
                {
                    result += Dictionaries()[idx].CreateGettersSetters(enumClass, targetSubject, targetAlgorithm, sourceSubject, sourceAlgorithm,isMessage);
                }
            }
            return result;
        }
        #endregion

        #region /// \name Presentation

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public void ReportText(string text)
        ///
        /// \brief
        ///
        /// \par Description.
        ///      Report Text to the message window.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 27/05/2018
        ///
        /// \param text (string) - The text.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReportText(string text)
        {
            MessageRouter.ReportMessage(ToString(), "", text);
        }
        #endregion
    }
}
