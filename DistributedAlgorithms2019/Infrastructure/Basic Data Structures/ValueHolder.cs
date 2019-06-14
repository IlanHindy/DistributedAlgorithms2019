////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file Infrastructure\ValueHolder.cs
///
/// \brief Implements the value holder class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;

namespace DistributedAlgorithms
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ValueHolder
    ///
    /// \brief A value holder.
    ///
    /// \par Description.
    ///      -  This class is an extension class for the classes that inherit from IValueHolder.
    ///      -  This is an implementation of multi inheritance (The classes can use the methods
    ///         as if they are theirs)
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 25/07/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class ValueHolder
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckMembers(IValueHolder checkedValueHolder, BaseNetwork network, Permissions permissions, IValueHolder parent, int nestingLevel, string key, bool checkPermissions = true)
        ///
        /// \brief Check members.
        ///
        /// \par Description.
        ///      This is a method that recursively check the members of a tree under the IValueHolder
        ///
        /// \par Algorithm.
        ///      -# Check the members of the IValueHolder:
        ///         -#  The network (Has to be the same as the parent)
        ///         -#  The permissions (Has to be the same as the parent)
        ///         -#  The parent
        ///      -# Cal the CheckMembers of the IValueHolder to implement the recursive check
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param checkedValueHolder  (IValueHolder) - The checked value holder.
        /// \param network             (BaseNetwork) - The network.
        /// \param permissions          (Permissions) - The permissions.
        /// \param parent              (IValueHolder) - The parent.
        /// \param nestingLevel        (int) - The nesting level.
        /// \param key                 (string) - The key.
        /// \param checkPermissions    (Optional)  (bool) - true to check permissions.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        #region /// \name Check building of members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckMembers(this IValueHolder checkedValueHolder, BaseNetwork network, Permissions permissions, IValueHolder parent, int nestingLevel, string key, bool checkPermissions = true)
        ///
        /// \brief An IValueHolder extension method that check members.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 10/09/2017
        ///
        /// \param checkedValueHolder The checkedValueHolder to act on.
        /// \param network             (BaseNetwork) - The network.
        /// \param permissions          (Permissions) - The permissions.
        /// \param parent              (IValueHolder) - The parent.
        /// \param nestingLevel        (int) - The nesting level.
        /// \param key                 (string) - The key.
        /// \param checkPermissions    (Optional)  (bool) - true to check permissions.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool CheckMembers(this IValueHolder checkedValueHolder,
            BaseNetwork network,
            NetworkElement element,
            Permissions permissions,
            IValueHolder parent,
            int nestingLevel,
            string key,
            bool checkPermissions = true)
        {
            checkedValueHolder.GenerateCheckMessage(nestingLevel, key, " Start");
            if (checkedValueHolder.Network != network)
            {
                checkedValueHolder.GenerateCheckMessage(nestingLevel, key, "The network is not equal");
                return false;
            }
            if (checkedValueHolder.Element != element)
            {
                checkedValueHolder.GenerateCheckMessage(nestingLevel, key, "The element is not equal");
                return false;
            }
            if (checkPermissions)
            {
                if (!checkedValueHolder.Permissions.Equals(permissions))
                {
                    checkedValueHolder.GenerateCheckMessage(nestingLevel, key, "The permissions are not equal");
                    return false;
                }
            }
            if (checkedValueHolder.Parent != parent)
            {
                checkedValueHolder.GenerateCheckMessage(nestingLevel, key, "The parent is wrong");
                return false;
            }
            if (checkedValueHolder.CheckMembers(nestingLevel + 1))
            {
                checkedValueHolder.GenerateCheckMessage(nestingLevel, key, " End - True");
                return true;
            }
            else
            {
                checkedValueHolder.GenerateCheckMessage(nestingLevel, key, " End - False");
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool EqualsTo(this IValueHolder valueHolder, int nestingLevel, object key, IValueHolder other, bool checkNotSameObject = false)
        ///
        /// \brief Equals to.
        ///
        /// \par Description.
        ///      -  This method recursively checks if the first IValueHolder is equal to the second  
        ///      -  The compare is for the values and not the pointers of the IValueHolder  
        ///      -  The parameter checkNotSameObject is for checking that there was a copy (means created  
        ///         new objects) and not assignments.
        ///
        /// \par Algorithm.
        ///      -# Common checks for all the IValueHolder
        ///         -#  The type of the 2 IValueHolder is the same
        ///         -#  If checkNotSameObject check that the objects are not the same objects
        ///      -# Activate the recursive compare by calling the IValueHolder's EqualsTo method.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 25/07/2017
        ///
        /// \param valueHolder        The valueHolder to act on.
        /// \param nestingLevel        (int) - The nesting level.
        /// \param key                 (object) - The key.
        /// \param other               (IValueHolder) - The other.
        /// \param checkNotSameObject (Optional)  (bool) - true to check not same object.
        ///
        /// \return True if equals to, false if not.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool CheckEqual(this IValueHolder valueHolder,
            int nestingLevel,
            object key,
            IValueHolder other,
            bool print = false,
            bool checkNotSameObject = false)
        {
            string keyString = TypesUtility.GetKeyToString(key);
            if (print)
            {
                if (nestingLevel == 0)
                {
                    MessageRouter.ReportMessage("----- Start EqualsTo -----", "", "");
                }

                valueHolder.GenerateCheckMessage(nestingLevel, keyString, " Start");
            }
            if (!valueHolder.GetType().Equals(other.GetType()))
            {
                valueHolder.GenerateCheckMessage(nestingLevel, keyString, "The type of one is : " + valueHolder.GetType() +
                    " The type of two is : " + other.GetType());
                return false;
            }

            if (checkNotSameObject)
            {
                if (valueHolder == other)
                {
                    valueHolder.GenerateCheckMessage(nestingLevel, keyString, "The pointer of one and two is equal and it should not");
                    return false;
                }
            }

            string error = "";
            if (valueHolder.EqualsTo(nestingLevel, ref error, other, print, checkNotSameObject))
            {
                if (print)
                {
                    if (nestingLevel == 0)
                    {
                        MessageRouter.ReportMessage("----- Start EqualsTo -----", "", "");
                    }
                    valueHolder.GenerateCheckMessage(nestingLevel, keyString, " End - True");
                }
                return true;
            }
            else
            {
                if (print)
                {
                    valueHolder.GenerateCheckMessage(nestingLevel, keyString, " " + error);
                    valueHolder.GenerateCheckMessage(nestingLevel, keyString, " End - False");
                    if (nestingLevel == 0)
                    {
                        MessageRouter.ReportMessage("----- End EqualsTo -----", "", "");
                    }
                }
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static void GenerateCheckMessage(this IValueHolder valueHolder, int nestingLevel, string key, string message)
        ///
        /// \brief Generates a check message.
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
        /// \param valueHolder  The valueHolder to act on.
        /// \param nestingLevel  (int) - The nesting level.
        /// \param key           (string) - The key.
        /// \param message       (string) - The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void GenerateCheckMessage(this IValueHolder valueHolder, int nestingLevel, string key, string message)
        {
            MessageRouter.ReportMessage(new string('\t', nestingLevel) + "[" + key + "] (" + valueHolder.GetType().ToString().Replace("DistributedAlgorithms.", "") + ") ", "", message);
        }
        #endregion
        #region /// \name Add algorithm window support (creating c# code)

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GetKeyString(this IValueHolder valueHolder, bool uc, bool isMessage = false)
        ///
        /// \brief An IValueHolder extension method that gets key string.
        ///
        /// \par Description.
        ///      The key string is a string composed from all the keys of the parents chain.
        ///
        /// \par Algorithm.
        ///      -# The method starts with the value holder
        ///      -# While not reached the main NetworkElement (it's parent is null)
        ///         -#  Get the key of the value holder in the parent (actually it get's the
        ///             key of the attribute that is the parent of the value holder)
        ///         -#  jump 2 value holders (the attribute and the parent of the attribute)
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        /// \param uc          (bool) - true to use upper case for keys.
        /// \param isMessage   (Optional) (bool) - true if this object is message.
        ///
        /// \return The key string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetKeyString(this IValueHolder valueHolder, bool uc, bool isMessage = false)
        {
            string keyString = "";
            while (true)
            {
                object keyInParent = valueHolder.Parent.GetChildKey(valueHolder);
                keyString = valueHolder.GetEnumName(keyInParent, uc) + "_" + keyString;

                // In order to get the previous dictionary or list we get the attribute
                // of the current dictionary or list and then gets it's parent
                valueHolder = valueHolder.Parent.Parent;

                if (valueHolder is null)
                {
                    break;
                }

                // The condition for termination is that we reached the main NetworkElement
                // The parent of that NetworkElement is null
                if (valueHolder.Parent == null)
                {
                    break;
                }


                // In the message the upper key (MessageType enum) is not included
                if (isMessage)
                {
                    if (valueHolder.Parent.Parent.Parent == null)
                    {
                        UcKeyString(valueHolder, keyString);
                        break;
                    }
                }
            }
            keyString = keyString.Remove(keyString.Length - 1);
            if (isMessage && keyString == "ork")
            {
                keyString = "MessageTypes";
            }
            return keyString;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string MainDictionaryInitMethodName(this IValueHolder valueHolder)
        ///
        /// \brief An IValueHolder extension method that main dictionary init method name.
        ///
        /// \par Description.
        ///      -  The method name of the method that initializes a main dictionary  
        ///      -  Example `InitPrivateAttribute`
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string MainDictionaryInitMethodName(this IValueHolder valueHolder)
        {
            string dictionaryName = TypesUtility.GetKeyToString(valueHolder.Parent.GetChildKey(valueHolder));
            return "Init" + dictionaryName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string MainDictionaryName(this IValueHolder valueHolder)
        ///
        /// \brief An IValueHolder extension method that main dictionary name.
        ///
        /// \par Description.
        ///      -  Get the name of the main dictionary.  
        ///      -  The name of the main dictionary is the name of the enum for that dictionary without  
        ///         the last letter (for example for PrivateAttribute dictionary the enum name is pak 
        ///         and the dictionary name is pa)
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string MainDictionaryName(this IValueHolder valueHolder)
        {
            string keyString = valueHolder.GetEnumName((object)valueHolder.Parent.GetChildKey(valueHolder), false);
            return keyString.Substring(0, keyString.Length - 1);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string InitMethodName(this IValueHolder valueHolder)
        ///
        /// \brief An IValueHolder extension method that init method name.
        ///
        /// \par Description.
        ///      -  Method name for methods used in the init phase.  
        ///      -  Example : `Init_EnumName`
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string InitMethodName(this IValueHolder valueHolder)
        {
            return "Init_" + valueHolder.GetKeyString(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string MessageMethodName(this IValueHolder valueHolder)
        ///
        /// \brief An IValueHolder extension method that message method name.
        ///
        /// \par Description.
        ///      -  Method name for methods used to gather data for sending  
        ///      -  Example `MessageDataFor_KeyName`.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string MessageMethodName(this IValueHolder valueHolder)
        {
            if (valueHolder is Attribute)
            {
                if (Attribute.GetValueCategory(valueHolder) ==
                    Attribute.AttributeCategory.PrimitiveAttribute)
                {
                    return "";
                }
                else
                {
                    valueHolder = ((Attribute)valueHolder).Value;
                }
            }

            return "MessageDataFor_" + valueHolder.GetKeyString(true, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string SendMethodName(this IValueHolder valueHolder, object key)
        ///
        /// \brief An IValueHolder extension method that sends a method name.
        ///
        /// \par Description.
        ///      -  Method name for sending a message method  
        ///      -  Example : `SendMessageName`
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        /// \param key          (object) - The key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string SendMethodName(this IValueHolder valueHolder, object key)
        {
            return "Send" + valueHolder.GetEnumName(key, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string MessageMainDictionaryMethodName(this IValueHolder valueHolder, dynamic key)
        ///
        /// \brief An IValueHolder extension method that message main dictionary method name.
        ///
        /// \par Description.
        ///      Main dictionary method name (To be used in the generation of the Send... method
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 24/12/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        /// \param key          (dynamic) - The key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string MessageMainDictionaryMethodName(this IValueHolder valueHolder, object key)
        {
            return "MessageDataFor_" + valueHolder.GetEnumName(key, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string AttributeKey(this IValueHolder valueHolder, object attributeKey, string enumClass, bool isMessage)
        ///
        /// \brief An IValueHolder extension method that attribute key.
        ///
        /// \par Description.
        ///      Create a string that holds the key with it's enum with it's class to be used when a dictionary
        ///      adds attribute
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder  The valueHolder to act on.
        /// \param attributeKey  (object) - The attribute key.
        /// \param enumClass     (string) - The enum class.
        /// \param isMessage     (bool) - true if this object is message.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string AttributeKey(this IValueHolder valueHolder, object attributeKey, string enumClass, bool isMessage = false)
        {
            string keyString = valueHolder.UcKeyString(attributeKey);
            string enumString = valueHolder.GetKeyString(false, isMessage);
            string s = enumClass + "." + enumString + "." + keyString;
            return s;
        }

        public static string MessageAttributeKey(this IValueHolder valueHolder, object attributeKey, string enumClass)
        {
            string keyString = valueHolder.UcKeyString(attributeKey);
            object keyInParent = valueHolder.Parent.GetChildKey(valueHolder);
            string enumString = valueHolder.GetEnumName(keyInParent, false);
            return enumClass + "." + enumString + "." + keyString;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string DictionaryEnum(this IValueHolder valueHolder, string enumClass, bool isMessage)
        ///
        /// \brief An IValueHolder extension method that dictionary enum.
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
        /// \param valueHolder The valueHolder to act on.
        /// \param enumClass    (string) - The enum class.
        /// \param isMessage    (bool) - true if this object is message.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string DictionaryEnum(this IValueHolder valueHolder, string nameSpace, string enumClass)
        {
            string enumString = valueHolder.GetKeyString(false);
            string s = nameSpace + "." + enumClass + "+" + enumString;
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string LcKeyString(this IValueHolder valueHolder, object key)
        ///
        /// \brief An IValueHolder extension method that lc key string.
        ///
        /// \par Description.
        ///      Lower case the first letter
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        /// \param key          (object) - The key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string LcKeyString(this IValueHolder valueHolder, object key)
        {
            string str = TypesUtility.GetKeyToString(key);
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);

            return str.ToLower();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string UcKeyString(this IValueHolder valueHolde, object key)
        ///
        /// \brief An IValueHolder extension method that key string.
        ///
        /// \par Description.
        ///      Upper case the first letter
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolde The valueHolde to act on.
        /// \param key         (object) - The key.
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string UcKeyString(this IValueHolder valueHolde, object key)
        {
            string str = TypesUtility.GetKeyToString(key);
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GetEnumName(this IValueHolder valueHolder, object key, bool uc, bool isMessage)
        ///
        /// \brief An IValueHolder extension method that gets enum name.
        ///
        /// \par Description.
        ///      When getting a key this method returns the enum name of the key
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 22/10/2017
        ///
        /// \param valueHolder The valueHolder to act on.
        /// \param key          (object) - The key.
        /// \param uc           (bool) - true to uc.
        /// \param isMessage    (bool) - true if this object is message.
        ///
        /// \return The enum name.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetEnumName(this IValueHolder valueHolder, object key, bool uc)
        {
            string enumName = "";
            if (key is NetworkElement.ElementDictionaries)
            {
                enumName = NetworkElement.ShortDictionariesNames[(int)((NetworkElement.ElementDictionaries)key)] + "k";
            }
            else
            {
                string keyString = TypesUtility.GetKeyToString(key);
                if (uc)
                {
                    enumName = char.ToUpper(keyString[0]) + keyString.Substring(1);
                }
                else
                {
                    enumName = char.ToLower(keyString[0]) + keyString.Substring(1);
                }
            }
            return enumName;
        }
        #endregion
        #region /// \name AddAlgorithmWindow presentation support

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool CheckIfBelongsToBaseAlgorithmEnum(this IValueHolder valueHolder, NetworkElement networkElement, NetworkElement.ElementDictionaries mainDictionary)
        ///
        /// \brief An IValueHolder extension method that determine if belongs to base algorithm enum.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/10/2017
        ///
        /// \param valueHolder    The valueHolder to act on.
        /// \param networkElement  (NetworkElement) - The network element.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        ///
        /// \return True if the value holder belongs to the base network element.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool BelongsToBaseAlgorithmEnum(this IValueHolder valueHolder, NetworkElement networkElement, NetworkElement.ElementDictionaries mainDictionary)
        {
            // If the value holder is not attribute get the attribute of the value holder
            Attribute attribute;
            if (!(valueHolder is Attribute))
            {
                attribute = (Attribute)valueHolder.Parent;
            }
            else
            {
                attribute = (Attribute)valueHolder;
            }

            // The condition is that the attribute which is the ancestor of our attribute
            // which is in the main dictionary belongs to the base class
            // Find the ancestor.
            // The parent sequence is ending in the following way
            // null <- networkElement <- Attribute <- mainDictionary <- Attribute
            // We have to find the last element in the chain described below
            // We do it with a loop that advances 2 parameters: The attribute and the chain end
            // The chain end is 4 times the parent of the attribute
            // Get the first chain end (If we cannot end the chain that means that the valueHolder is
            // one of the value holders at the middle of the chain hence they are not attributes in the 
            // base classes main dictionary
            IValueHolder chainEnd = attribute;
            for (int idx = 0; idx < 4; idx++)
            {
                chainEnd = chainEnd.Parent;
                if (chainEnd == null && idx != 3)
                {
                    return false;
                }
            }

            // Find the ancestor
            while (chainEnd != null)
            {
                chainEnd = chainEnd.Parent.Parent;
                attribute = (Attribute)attribute.Parent.Parent;
            }

            // Decide if the attribute is part of the main dictionary
            if (!((AttributeDictionary)networkElement[mainDictionary]).Values.Any(a => a == attribute))
            {
                return false;
            }

            // Get the key of the attribute
            dynamic key = attribute.Parent.GetChildKey(attribute);
            if (TypesUtility.CompareDynamics(key, bn.ork.SingleStepStatus))
            {
                int x = 1;
            }

            // Get the base NetworkElement
            Type baseNetworkElementType = networkElement.GetType().BaseType;
            NetworkElement baseNetworkElement = (NetworkElement)TypesUtility.CreateObjectFromTypeString(baseNetworkElementType.ToString());
            baseNetworkElement.Init(0);

            // Check if the key is found in the base network element's dictionary
            return ((AttributeDictionary)baseNetworkElement[mainDictionary]).Keys.Any(k => TypesUtility.CompareDynamics(k, key));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static bool AddAlgorithmWindowAttributeEditable(this IValueHolder valueHolder, NetworkElement networkElement, NetworkElement.ElementDictionaries mainDictionary)
        ///
        /// \brief An IValueHolder extension method that decides whether an attribute is editable in AddAlgorithmWindow
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 29/10/2017
        ///
        /// \param valueHolder    The valueHolder to act on.
        /// \param networkElement  (NetworkElement) - The network element.
        /// \param mainDictionary  (ElementDictionaries) - Dictionary of mains.
        ///
        /// \return True if it succeeds, false if it fails.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool AddAlgorithmWindowAttributeEditable(this IValueHolder valueHolder, NetworkElement networkElement, NetworkElement.ElementDictionaries mainDictionary)
        {

            // In AddAlgorithm window the ElementAttributes should be disabled because this window
            // is for adding new keys for a new algorithm and the ElementAttributes dictionary is for
            // the base types only
            if (mainDictionary == NetworkElement.ElementDictionaries.ElementAttributes)
            {
                return false;
            }

            // For other dictionaries if the AttributeDictionary belongs to the base class it should
            // be disabled because the window is allowed to change only attributes that belongs
            // to the Algorithm and not the base algorithm
            // This rule is not applied to the messages NetworkElement because all the messages
            // should be changeable (The base class is empty anyway) 
            else
            {
                if (networkElement.GetType().Equals(typeof(NetworkElement)))
                {
                    return true;
                }
                else
                {
                    return !valueHolder.BelongsToBaseAlgorithmEnum(networkElement, mainDictionary);
                }
            }
        }
        #endregion
    }
}
