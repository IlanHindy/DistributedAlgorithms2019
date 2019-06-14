////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\TypesUtility.cs
///
///\brief   Implements the types utility class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * Values that represent type categories.
    
     **************************************************************************************************/

    public enum TypeCategories { Primitive, String, Enum, NetworkElement, AttributeList, AttributeDictionary, Attribute, BaseEnumClasses }

    /**********************************************************************************************//**
     * The types utility.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class TypesUtility
    {
        /**********************************************************************************************//**
         * Gets type from string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   typeString  The type string.
         *
         * \return  The type from string.
         .
         **************************************************************************************************/

        public static System.Type GetTypeFromString(string typeString)
        {
            Type result = Type.GetType(typeString);
            if (result == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    result = assembly.GetType(typeString);
                    if (result == null)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /**********************************************************************************************//**
         * Convert value from string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param       type        The type.
         * \param       valueString The value string.
         * \param [out] converted   The converted.
         *
         * \return  The value converted from string.
         .
         **************************************************************************************************/

        public static dynamic ConvertValueFromString(System.Type type, string valueString, out bool converted)
        {
            dynamic result = GetDefaultValue(type);
            converted = true;
            if (type.IsEnum)
            {
                if (Enum.IsDefined(type, valueString))
                {
                    return Enum.Parse(type, valueString);
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(NetworkElement).IsAssignableFrom(type) ||
                typeof(IList).IsAssignableFrom(type) ||
                typeof(AttributeDictionary).IsAssignableFrom(type))
            {
            }
            else if (type.Equals(typeof(string)))
            {
                return valueString;
            }
            else
            {
                result = Parse(type, valueString, out converted);
                if (converted)
                {
                    return result;
                }
            }
            return result;
        }

        /**********************************************************************************************//**
         * Gets default value.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   t   The Type to process.
         *
         * \return  The default value.
         .
         **************************************************************************************************/

        static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        /**********************************************************************************************//**
         * Gets enum value to string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \tparam  T   Generic type parameter.
         * \param   Value   The value.
         *
         * \return  The enum value to string.
         .
         **************************************************************************************************/

        public static string GetKeyToString<T>(object Value)
        {
            if (typeof(T).IsEnum)
            {
                return Enum.GetName(typeof(T), (int)Value);
            }
            else
            {
                return Value.ToString();
            }
        }

        /**********************************************************************************************//**
         * Gets enum value to string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   Value   The value.
         *
         * \return  The enum value to string.
         .
         **************************************************************************************************/

        public static string                                                                                                                                                                                                                                                     GetKeyToString(object value)
        {
            if (value.GetType().IsEnum)
            {
                return Enum.GetName(value.GetType(), (int)value);
            }
            else
            {
                return value.ToString();
            }
        }

        public static string GetKeyAndTypeToString(object value)
        {
            if (value.GetType().IsEnum)
            {
                return value.GetType().ToString() + "." + Enum.GetName(value.GetType(), (int)value);
            }
            else
            {
                return value.GetType().ToString() + " : " + value.ToString();
            }
        }

        public static string GetKeyInParent(IValueHolder parent, IValueHolder child)
        {
            if (parent == null)
            {
                return "(No key because there is no parent)";
            }
            return TypesUtility.GetKeyToString(parent.GetChildKey(child));
        }

        /**********************************************************************************************//**
         * Gets enum value from string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \tparam  T   Generic type parameter.
         * \param   valueString The value string.
         *
         * \return  The enum value from string.
         .
         **************************************************************************************************/

        public static object GetKeyFromString<T>(string valueString)
        {
            bool converted;
            return ConvertValueFromString(typeof(T), valueString, out converted);
        }

        /**********************************************************************************************//**
         * Gets enum value from string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   type        The type.
         * \param   valueString The value string.
         *
         * \return  The enum value from string.
         .
         **************************************************************************************************/

        public static object GetKeyFromString(Type type, string valueString)
        {
            bool converted;
            return ConvertValueFromString(type, valueString, out converted);
        }


        /**********************************************************************************************//**
         * Parses.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \tparam  T   Generic type parameter.
         * \param       valueString The value string.
         * \param [out] converted   The converted.
         *
         * \return  A T.
         .
         **************************************************************************************************/

        public static T Parse<T>(string valueString, out bool converted)
        {
            T result = default(T);
            converted = true;
            var convertor = TypeDescriptor.GetConverter(typeof(T));
            if (convertor == null || !convertor.IsValid(valueString))
            {
                converted = false;
                return result;
            }
            result = (T)convertor.ConvertFromString(valueString);
            return result;
        }

        /**********************************************************************************************//**
         * Parses.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param       type        The type.
         * \param       valueString The value string.
         * \param [out] converted   The converted.
         *
         * \return  An object.
         .
         **************************************************************************************************/

        public static object Parse(Type type, string valueString, out bool converted)
        {
            if (type.Equals(typeof(string)))
            {
                converted = true;
                return valueString;
            }
            object result = Activator.CreateInstance(type);
            converted = true;
            var convertor = TypeDescriptor.GetConverter(type);
            if (convertor == null || !convertor.IsValid(valueString))
            {
                converted = false;
                return result;
            }
            result = convertor.ConvertFromString(valueString);
            return result;
        }

        /**********************************************************************************************//**
         * Creates list from string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   className   Name of the class.
         *
         * \return  The new list from string.
         .
         **************************************************************************************************/

        public static object CreateListFromString(string className)
        {
            Type listType = Type.GetType(className);
            Type typeArgument = listType.GenericTypeArguments[0];

            Type genericClass = typeof(List<>);
            // MakeGenericType is badly named
            Type constructedClass = genericClass.MakeGenericType(typeArgument);

            return Activator.CreateInstance(constructedClass);
        }

        /**********************************************************************************************//**
         * Creates list from argument type string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   argumentTypeString  The argument type string.
         *
         * \return  The new list from argument type string.
         .
         **************************************************************************************************/

        public static object CreateListFromArgumentTypeString(string argumentTypeString)
        {
            Type argumentType = Type.GetType(argumentTypeString);

            Type genericClass = typeof(List<>);
            // MakeGenericType is badly named
            Type constructedClass = genericClass.MakeGenericType(argumentType);

            return Activator.CreateInstance(constructedClass);
        }

        /**********************************************************************************************//**
         * Copies the list.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sourceList  List of sources.
         * \param   destList    List of destinations.
         *                      
         **************************************************************************************************/

        public static void CopyList(object sourceList, object destList)
        {
            InvokeMethodOfList(destList, "Clear", null, false);
            object itemsArray = InvokeMethodOfList(sourceList, "ToArray", null, true);
            foreach (var item in itemsArray as Array)
            {
                TypesUtility.InvokeMethodOfList(destList, "Add", new object[] { item }, false);
            }
        }

        /**********************************************************************************************//**
         * Executes the method of attribute on a different thread, and waits for the result.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   attribute       The attribute.
         * \param   methodName      Name of the method.
         * \param   parameters      Options for controlling the operation.
         * \param   returnsValue    true to returns value.
         *
         * \return  An object.
         .
         **************************************************************************************************/

        public static object InvokeMethodOfAttribute(object attribute, string methodName, object[] parameters, bool returnsValue)
        {
            object result = null;
            Type t = attribute.GetType();
            if (t.IsGenericType)
            {
                if (t.GetGenericTypeDefinition() == typeof(List<>)) //check the object is our type
                {
                    if (returnsValue)
                    {
                        result = t.GetMethod(methodName).Invoke(attribute, parameters);
                    }
                    else
                    {
                        t.GetMethod(methodName).Invoke(attribute, parameters);
                    }
                }
            }
            return result;
        }

        public static object InvokeMethod(object obj, string methodName, List<object> prms, int optionalPrmsCount, bool returnsValue)
        {
            object result = null;
            Type t = obj.GetType();
            List<object> optionalPrms = Enumerable.Repeat(Type.Missing, optionalPrmsCount).ToList();
            prms = prms.Concat(optionalPrms).ToList();
            if (returnsValue)
            {
                result = t.GetMethod(methodName).Invoke(obj, prms.ToArray());
            }
            else
            {
                t.GetMethod(methodName).Invoke(obj, prms.ToArray());
            }
            return result;
        }

        /**********************************************************************************************//**
         * Executes the method of list on a different thread, and waits for the result.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list            The list.
         * \param   methodName      Name of the method.
         * \param   parameters      Options for controlling the operation.
         * \param   returnsValue    true to returns value.
         *
         * \return  An object.
         .
         **************************************************************************************************/

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static object InvokeMethodOfList(object list, string methodName, object[] parameters, bool returnsValue)
        ///
        /// \brief Executes the method of list on a different thread, and waits for the result.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 01/10/2017
        ///
        /// \param list          (object) - The list.
        /// \param methodName    (string) - Name of the method.
        /// \param parameters    (object[]) - Options for controlling the operation.
        /// \param returnsValue  (bool) - true to returns value.
        ///
        /// \return An object.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static object InvokeMethodOfList(object list, string methodName, object[] parameters, bool returnsValue)
        {
            object result = null;
            Type t = list.GetType();
            if (t.IsGenericType)
            {
                if (t.GetGenericTypeDefinition() == typeof(List<>)) //check the object is our type
                {
                    if (returnsValue)
                    {
                        MethodInfo method = t.GetMethod(methodName);
                        result = method.Invoke(list, parameters);
                    }
                    else
                    {
                        t.GetMethod(methodName).Invoke(list, parameters);
                    }
                }
            }
            return result;
        }

        /**********************************************************************************************//**
         * Gets property of list.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   list            The list.
         * \param   propertyName    Name of the property.
         *
         * \return  The property of list.
         .
         **************************************************************************************************/

        public static object GetPropertyOfList(object list, string propertyName)
        {
            Type t = list.GetType();
            if (t.IsGenericType)
            {
                if (t.GetGenericTypeDefinition() == typeof(List<>)) //check the object is our type
                {
                    //Get the property value
                    return t.GetProperty(propertyName).GetValue(list, null);
                }
            }
            return false;
        }

        /**********************************************************************************************//**
         * Creates object from type string.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   typeString  The type string.
         * \param   args        (Optional) The arguments.
         *
         * \return  The new object from type string.
         .
         **************************************************************************************************/

        public static object CreateObjectFromTypeString(string typeString, object[] args = null)
        {
            if (args == null)
            {
                return Activator.CreateInstance(Type.GetType(typeString));
            }
            else
            {
                return Activator.CreateInstance(Type.GetType(typeString), args);
            }
        }

        /**********************************************************************************************//**
         * Gets an array.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \tparam  T   Generic type parameter.
         * \param   inputs  The inputs.
         *
         * \return  The array.
         .
         **************************************************************************************************/

        public static T GetArray<T>(object[] inputs)
        {
            Type elementType = typeof(T).GenericTypeArguments[0];
            Array array = Array.CreateInstance(elementType, inputs.Length);
            inputs.CopyTo(array, 0);
            T obj = (T)(object)array;
            return obj;
        }

        /**********************************************************************************************//**
         * Adds an end operation method.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   attribute   The attribute.
         * \param   methodName  Name of the method.
         * \param   type        The type.
         *                      
         **************************************************************************************************/

        public static void AddEndOperationMethod(Attribute attribute, string methodFullName)
        {
            Type type = Type.GetType(methodFullName.Substring(0, methodFullName.LastIndexOf('.')));
            string methodName = methodFullName.Substring(methodFullName.LastIndexOf(".") + 1);
            MethodInfo theMethod;
            do
            {
                theMethod = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                if (theMethod != null)
                {
                    attribute.EndInputOperation = (Attribute.EndInputOperationDelegate)Delegate.CreateDelegate(typeof(Attribute.EndInputOperationDelegate), null, theMethod, false);
                    return;
                }
                if (!type.Equals(typeof(NetworkElement)))
                {
                    type = type.BaseType;
                }
                else
                {
                    return;
                }
            } while (true);
        }

        //public static void AddElementWindowPrmsMethod(Attribute attribute, string methodName, Type type)
        //{
        //    if (type == null)
        //    {
        //        return;
        //    }
        //    MethodInfo theMethod;
        //    do
        //    {
        //        theMethod = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        //        if (theMethod != null)
        //        {
        //            attribute.ElementWindowPrmsMethod = (ElementWindowPrmsDelegate)Delegate.CreateDelegate(typeof(ElementWindowPrmsDelegate), null, theMethod, false);
        //            return;
        //        }
        //        if (!type.Equals(typeof(NetworkElement)))
        //        {
        //            type = type.BaseType;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    } while (true);

        //}

        public static void AddElementWindowPrmsMethod(Attribute attribute, string methodFullName)
        {
            Type type = Type.GetType(methodFullName.Substring(0, methodFullName.LastIndexOf('.')));
            string methodName = methodFullName.Substring(methodFullName.LastIndexOf(".") + 1);
            MethodInfo theMethod;
            do
            {
                theMethod = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                if (theMethod != null)
                {
                    attribute.ElementWindowPrmsMethod = (ElementWindowPrmsDelegate)Delegate.CreateDelegate(typeof(ElementWindowPrmsDelegate), null, theMethod, false);
                    return;
                }
                if (!type.Equals(typeof(NetworkElement)))
                {
                    type = type.BaseType;
                }
                else
                {
                    return;
                }
            } while (true);

        }

        /**********************************************************************************************//**
         * Gets all end input operations.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  all end input operations.
         .
         **************************************************************************************************/

        //public static List<MethodInfo> GetAllEndInputOperations()
        //{
        //    Attribute.EndInputOperationDelegate endInputOperationDelegate;
        //    List<Type> networkElementTypes = GetAllNetworkElementsTypes();
        //    networkElementTypes.Add(typeof(Attribute));
        //    List<MethodInfo> result = new List<MethodInfo>();
        //    foreach (Type type in networkElementTypes)
        //    {
        //        MethodInfo[] arrayMethodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
        //        foreach (MethodInfo methodInfo in arrayMethodInfo)
        //        {
        //            try
        //            {
        //                endInputOperationDelegate = (Attribute.EndInputOperationDelegate)Delegate.CreateDelegate(typeof(Attribute.EndInputOperationDelegate), null, methodInfo, true);
        //                if (methodInfo.Name == "DefaultEndInputOperations")
        //                {
        //                    result.Insert(0, methodInfo);
        //                }
        //                else
        //                {
        //                    result.Add(methodInfo);
        //                }
        //            }
        //            catch { }
        //        }
        //    }
        //    return result;
        //}

        public static List<MethodInfo> GetAllEndInputOperations()
        {
            Attribute.EndInputOperationDelegate endInputOperationDelegate;
            List<Type> types = GetAllTypes();
            List<MethodInfo> result = new List<MethodInfo>();
            foreach (Type type in types)
            {
                MethodInfo[] arrayMethodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (MethodInfo methodInfo in arrayMethodInfo)
                {
                    try
                    {
                        endInputOperationDelegate = (Attribute.EndInputOperationDelegate)Delegate.CreateDelegate(typeof(Attribute.EndInputOperationDelegate), null, methodInfo, true);
                        if (methodInfo.Name == "DefaultEndInputOperations")
                        {
                            result.Insert(0, methodInfo);
                        }
                        else
                        {
                            result.Add(methodInfo);
                        }
                    }
                    catch { }
                }
            }
            return result;
        }

        public static List<string> GetAllInternalEventMethods()
        {
            Type type = ClassFactory.GenerateProcess().GetType();
            InternalEvents.InternalEventDelegate internalEventDelegate;
            List<string> result = new List<string>();

            MethodInfo[] arrayMethodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo methodInfo in arrayMethodInfo)
            {
                try
                {
                    internalEventDelegate = (InternalEvents.InternalEventDelegate)Delegate.CreateDelegate(typeof(InternalEvents.InternalEventDelegate), null, methodInfo, true);
                    result.Add(methodInfo.Name);
                }
                catch { }
            }
            return result;
        }
        public static List<string> GetInternalEventMethods()
        {
            Type type = ClassFactory.GenerateProcess().GetType();
            Type delegateType = typeof(InternalEvents.InternalEventDelegate);
            MethodInfo delegateSignature = delegateType.GetMethod("Invoke");
            MethodInfo[] arrayMethodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<string> result = new List<string>();
            foreach (MethodInfo methodInfo in arrayMethodInfo)
            {
                bool parametersEqual = delegateSignature.GetParameters()
                    .Select(x => x.ParameterType)
                    .SequenceEqual(methodInfo.GetParameters().Select(x => x.ParameterType));
                if (delegateSignature.ReturnType == methodInfo.ReturnType && parametersEqual)
                {
                    result.Add(methodInfo.Name);
                }
            }
            return result;
        }

        public static List<MethodInfo> GetAllElementWindowPrmsMethods()
        {
            ElementWindowPrmsDelegate elementWindowPrmsDelegate;
            List<Type> types = GetAllTypes();
            List<MethodInfo> result = new List<MethodInfo>();
            foreach (Type type in types)
            {
                MethodInfo[] arrayMethodInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (MethodInfo methodInfo in arrayMethodInfo)
                {
                    try
                    {
                        elementWindowPrmsDelegate = (ElementWindowPrmsDelegate)Delegate.CreateDelegate(typeof(ElementWindowPrmsDelegate), null, methodInfo, true);
                        if (methodInfo.Name == "DefaultElementWindowPrmsMethod")
                        {
                            result.Insert(0, methodInfo);
                        }
                        else
                        {
                            result.Add(methodInfo);
                        }
                    }
                    catch { }
                }
            }
            return result;
        }

        /**********************************************************************************************//**
         * Gets all network elements types.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  all network elements types.
         .
         **************************************************************************************************/

        public static List<Type> GetAllNetworkElementsTypes()
        {
            string namespaceString = ClassFactory.GenerateNamespace();
            List<Type> listNetworkElements = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                typeof(NetworkElement).IsAssignableFrom(t) &&
                t.Namespace == "DistributedAlgorithms.Base").ToList();
            if (namespaceString != "DistributedAlgorithms.Base")
            {
                List<Type> listNetworkElementsOfAlgorithm = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                    typeof(NetworkElement).IsAssignableFrom(t) &&
                    t.Namespace == namespaceString).ToList();
                listNetworkElements = (List<Type>)listNetworkElements.Concat(listNetworkElementsOfAlgorithm).ToList();
            }
            return listNetworkElements;
        }

        public static List<Type> GetAllTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass &&
                           t.Namespace != null &&
                           t.Namespace.IndexOf("DistributedAlgorithms") == 0).ToList();
        }

        /**********************************************************************************************//**
         * Gets end input operation name.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   attribute   The attribute.
         *
         * \return  The end input operation name.
         .
         **************************************************************************************************/

        public static string GetEndInputOperationName(Attribute attribute)
        {
            var methodInfo = attribute.EndInputOperation.Method;
            return methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
        }

        public static string GetElementWindowPrmsMethodName(Attribute attribute)
        {
            var methodInfo = attribute.ElementWindowPrmsMethod.Method;
            return methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
        }

        public static string GetMethodAndNamespace(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
        }

        /**********************************************************************************************//**
         * Gets types categories.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  The types categories.
         .
         **************************************************************************************************/

        public static List<string> GetTypesCategories()
        {
            return Enum.GetNames(typeof(TypeCategories)).ToList();
        }

        public static List<string> GetKeyTypesCategories()
        {
            List<string> result = new List<string>();
            result.Add(GetKeyToString(TypeCategories.Primitive));
            result.Add(GetKeyToString(TypeCategories.String));
            result.Add(GetKeyToString(TypeCategories.Enum));
            return result;
        }

        /**********************************************************************************************//**
         * Gets all enums values.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  all enums values.
         .
         **************************************************************************************************/

        public static List<dynamic> GetAllEnumsValues()
        {
            List<Type> enums = GetEnums(ClassFactory.GenerateNamespace());
            List<dynamic> allEnumsValues = new List<dynamic>();
            foreach (Type type in enums)
            {
                allEnumsValues = allEnumsValues.Concat(GetAllEnumValues(type)).ToList();
            }
            return allEnumsValues;
        }

        /**********************************************************************************************//**
         * Gets all enum values.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   type    The type.
         *
         * \return  all enum values.
         .
         **************************************************************************************************/

        public static List<dynamic> GetAllEnumValues(Type type)
        {
            List<dynamic> result = new List<dynamic>();
            Array values = Enum.GetValues(type);
            foreach (dynamic key in Enum.GetValues(type))
            {
                result.Add(key);
            }
            return result;
        }

        //public static Type GetEnumType(string enumName)
        //{
        //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //    {
        //        var type = assembly.GetType(enumName);
        //        if (type == null)
        //            continue;
        //        if (type.IsEnum)
        //            return type;
        //    }
        //    return null;
        //}

        /**********************************************************************************************//**
         * Gets types of category.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   categoryString  The category string.
         *
         * \return  The types of category.
         .
         **************************************************************************************************/

        public static List<string> GetTypesOfCategory(string categoryString)
        {
            TypeCategories category = (TypeCategories)GetKeyFromString(typeof(TypeCategories), categoryString);
            List<Type> types = new List<Type>();
            switch (category)
            {
                case TypeCategories.Primitive: types = GetPrimitiveTypes(); break;
                case TypeCategories.String: types.Add(typeof(string)); break;
                case TypeCategories.Enum: types = GetEnums(ClassFactory.GenerateNamespace()); break;
                case TypeCategories.NetworkElement: types = GetNetworkElementInherittedTypes(ClassFactory.GenerateNamespace()); break;
                case TypeCategories.AttributeDictionary: types.Add(typeof(AttributeDictionary)); break;
                case TypeCategories.AttributeList: types.Add(typeof(AttributeList)); break;
                case TypeCategories.Attribute: types.Add(typeof(Attribute)); break;
                case TypeCategories.BaseEnumClasses: types = GetBaseEnumClasses(ClassFactory.GenerateNamespace()); break;
            }
            List<string> typeNames = new List<string>();
            types.ForEach(delegate (Type type) { typeNames.Add(type.ToString()); });
            return typeNames;
        }

        /**********************************************************************************************//**
         * Gets all possible types for attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  all possible types for attribute.
         .
         **************************************************************************************************/

        public static List<string> GetAllPossibleTypesForAttribute()
        {
            List<string> result = new List<string>();
            result = result.Concat(GetTypesOfCategory("Primitive")).ToList();
            //result = result.Concat(GetTypesOfCategory("String")).ToList();
            result = result.Concat(GetTypesOfCategory("Enum")).ToList();
            result = result.Concat(GetTypesOfCategory("NetworkElement")).ToList();
            result = result.Concat(GetTypesOfCategory("AttributeDictionary")).ToList();
            result = result.Concat(GetTypesOfCategory("AttributeList")).ToList();

            return result;
        }

        public static HashSet<string> GetClassesForSyntaxHighlight()
        {
            string currSubject = Config.Instance[Config.Keys.SelectedSubject];
            string currAlgorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            Config.Instance[Config.Keys.SelectedSubject] = "Base";
            Config.Instance[Config.Keys.SelectedAlgorithm] = "Base";
            List<string> classNames = new List<string>();
            List<string> enumFullNames = classNames.Concat(GetTypesOfCategory("Enum")).ToList();
            foreach (string enumFullName in enumFullNames)
            {
                classNames.Add(enumFullName.Substring(enumFullName.IndexOf('+') + 1));
            }
            classNames = classNames.Concat(GetTypesOfCategory("NetworkElement")).ToList();
            classNames = classNames.Concat(GetTypesOfCategory("AttributeDictionary")).ToList();
            classNames = classNames.Concat(GetTypesOfCategory("AttributeList")).ToList();
            classNames = classNames.Concat(GetTypesOfCategory("Attribute")).ToList();
            classNames = classNames.Concat(GetTypesOfCategory("BaseEnumClasses")).ToList();

            HashSet<string> result = new HashSet<string>();
            foreach (string fullFileName in classNames)
            {
                string className = fullFileName;
                className = className.Replace("Algorithms.Base.Base.", "");
                className = className.Replace("DistributedAlgorithms.", "");
                className = className.Replace("+", ".");
                result.Add(className);
            }
            Config.Instance[Config.Keys.SelectedSubject] = currSubject;
            Config.Instance[Config.Keys.SelectedAlgorithm] = currAlgorithm;
            return result;
        }

        public static string GetTypeShortNameToString(Type type)
        {
            string result = type.ToString();
            if (type.IsEnum)
            {
                return result.Substring(result.IndexOf('+') + 1);
            }
            else
            {
                result = result.Replace("Algorithms.Base.Base.", "");
                result = result.Replace("DistributedAlgorithms.", "");
                result = result.Replace("+", ".");
                return result;
            }
        }

        public static string GetTypeNameOnlyToString(Type type)
        {
            string s = type.ToString();
            return s.Substring(s.LastIndexOf('.') + 1);
        }


        public static string GetEnumValueAndType(object key, string subject, string algorithm)
        {
            string result = key.GetType().ToString() + "." + TypesUtility.GetKeyToString(key);
            result = result.Replace('+', '.');
            result = result.Replace("DistributedAlgorithms.Algorithms." + subject + "." + algorithm + ".", "");
            result = result.Replace("DistributedAlgorithms.Algorithms.Base.Base.", "");
            result = result.Replace("DistributedAlgorithms.Algorithms.", "");
            result = result.Replace("DistributedAlgorithms.", "");
            return result;
        }

        /**********************************************************************************************//**
         * Creates an attribute.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param       categoryString  The category string.
         * \param       typeString      The type string.
         * \param       valueString     The value string.
         * \param [out] attribute       The attribute.
         *
         * \return  true if it succeeds, false if it fails.
         .
         **************************************************************************************************/

        public static bool CreateAttribute(string categoryString, string typeString, string valueString, out Attribute attribute)
        {
            TypeCategories category = (TypeCategories)GetKeyFromString(typeof(TypeCategories), categoryString);
            Type type = Type.GetType(typeString);
            dynamic value = 0;
            switch (category)
            {
                case TypeCategories.Primitive: attribute = new Attribute { Value = value }; return true;
                case TypeCategories.String: attribute = new Attribute { Value = value }; return true;
                case TypeCategories.Enum: attribute = new Attribute { Value = value }; return true;
                case TypeCategories.NetworkElement: attribute = new Attribute { Value = CreateObjectFromTypeString(typeString) }; attribute.Value.Init(0); return true;
                case TypeCategories.AttributeList: attribute = new Attribute { Value = CreateObjectFromTypeString(typeString) }; return true;
                case TypeCategories.AttributeDictionary: attribute = new Attribute { Value = CreateObjectFromTypeString(typeString) }; return true;
                default: attribute = null; return false;
            }
        }

        public static List<string> GetEnumKeysToStrings(string typeString)
        {
            Type type = GetTypeFromString(typeString);
            List<dynamic> enumValues = GetAllEnumValues(type);
            List<string> result = new List<string>();
            foreach (dynamic value in enumValues)
            {
                result.Add(GetKeyToString(value));
            }
            return result;
        }

        public static int IndexInKeys(ICollection collection, dynamic value)
        {
            List<dynamic> list = collection.Cast<dynamic>().ToList();
            int result = -1;
            for (int idx = 0; idx < list.Count; idx++)
            {
                if (CompareDynamics(list[idx], value))
                {
                    return idx;
                }
            }
            return result;
        }

        /**********************************************************************************************//**
         * Gets possible values.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   typeString  The type string.
         *
         * \return  The possible values.
         .
         **************************************************************************************************/

        public static List<string> GetPossibleValues(string typeString)
        {
            Type type = Type.GetType(typeString);
            List<string> types = new List<string>();
            if (type.IsEnum)
            {
                return Enum.GetNames(type).ToList();
            }
            else if (type.Equals(typeof(bool)))
            {
                return new List<string>() { "False", "True" };
            }
            else
            {
                return new List<string>();
            }
        }

        /**********************************************************************************************//**
         * Gets enums of type.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   classType   Type of the class.
         *
         * \return  The enums of type.
         .
         **************************************************************************************************/

        public static List<Type> GetEnumsOfType(Type classType)
        {
            List<Type> listEnums = new List<Type>();
            MemberTypes Mymembertypes;

            // Get the MemberInfo array.
            MemberInfo[] Mymembersinfoarray = classType.GetMembers();

            // Get and display the name and the MemberType for each member.
            foreach (MemberInfo Mymemberinfo in Mymembersinfoarray)
            {
                Mymembertypes = Mymemberinfo.MemberType;
                if (Mymembertypes == MemberTypes.NestedType)
                {
                    string className = Mymemberinfo.ReflectedType.ToString() + "+" + Mymemberinfo.Name;
                    Type type = Type.GetType(className);
                    if (type.IsEnum)
                    {
                        listEnums.Add(type);
                        //Console.WriteLine("The member {0} of {1} is a {2}.", Mymemberinfo.Name, classType, Mymembertypes.ToString());
                    }
                }
            }
            return listEnums;
        }

        /**********************************************************************************************//**
         * Gets network element inheritted types.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   namespaceString The namespace string.
         *
         * \return  The network element inheritted types.
         .
         **************************************************************************************************/

        public static List<Type> GetNetworkElementInherittedTypes(string namespaceString)
        {
            List<Type> listTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass &&
                t.Namespace == namespaceString &&
                typeof(NetworkElement).IsAssignableFrom(t) &&
                !t.Equals(typeof(BaseMessage))).ToList();
            listTypes.Add(typeof(BaseMessage));
            return listTypes;
        }

        public static List<Type> GetBaseEnumClasses(string namespaceString)
        {
            List<Type> listTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass &&
                t.Namespace == namespaceString &&
                !t.ToString().Contains("+") &&
                t.ToString().Contains("Base.Base") &&
                !typeof(NetworkElement).IsAssignableFrom(t)).ToList();
            return listTypes;
        }

        /**********************************************************************************************//**
         * Gets the enums.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   namespaceString The namespace string.
         *
         * \return  The enums.
         .
         **************************************************************************************************/

        public static List<Type> GetEnums(string namespaceString)
        {
            List<Type> listEnums = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum &&
                t.Namespace == "DistributedAlgorithms.Base").ToList();
            if (namespaceString != "DistributedAlgorithms.Base")
            {
                List<Type> listEnumsOfAlgorithm = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum &&
                t.Namespace == namespaceString).ToList();
                listEnums = (List<Type>)listEnums.Concat(listEnumsOfAlgorithm).ToList();
            }
            return listEnums;
        }

        public static Type GetEnumType(string enumName)
        {
            List<string> enumNames = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => t.IsEnum).Select(t => t.FullName).ToList();
            List<Type> listEnums = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => t.IsEnum && t.FullName == enumName).ToList();
            return listEnums[0];
        }

        /**********************************************************************************************//**
         * Gets primitive types.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \return  The primitive types.
         .
         **************************************************************************************************/

        public static List<Type> GetPrimitiveTypes()
        {
            return typeof(int).Assembly.GetTypes().Where(type => type.IsPrimitive || type.Equals(typeof(string))).ToList();
        }

        /**********************************************************************************************//**
         * Gets a default.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   type    The type.
         *
         * \return  The default.
         .
         **************************************************************************************************/

        public static object GetDefault(Type type)
        {
            if (type.IsValueType || typeof(IValueHolder).IsAssignableFrom(type))
            {
                return Activator.CreateInstance(type);
            }
            if (type.Equals(typeof(string)))
            {
                return "";
            }
            return null;
        }

        /**********************************************************************************************//**
         * Gets the algorithms.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \return  The algorithms.
         **************************************************************************************************/

        public static List<string> GetAlgorithms()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.Namespace != null)
                        .Where(t => t.Namespace.ToString().Contains("Algorithms"))
                         .Select(t => t.Namespace.ToString().Replace("DistributedAlgorithms.Algorithms.", ""))
                         .Where(s => !s.Contains("DistributedAlgorithms"))
                         .Distinct()
                         .ToList();
        }

        /**********************************************************************************************//**
         * Gets algorithms subjects.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \return  The algorithms subjects.
         **************************************************************************************************/

        public static List<string> GetAlgorithmsSubjects()
        {
            List<Type> types = Assembly.GetExecutingAssembly().GetTypes().ToList();
            types = types.Where(t => t.Namespace != null).ToList();
            types = types.Where(t => t.Namespace.ToString().Contains("DistributedAlgorithms.Algorithms.")).ToList();
            List<string> subjects = types.Select(t => t.Namespace.ToString().Replace("DistributedAlgorithms.Algorithms.", "")).ToList();
            subjects = subjects.Select(s => s.Substring(0, s.IndexOf("."))).ToList();
            subjects = subjects.Distinct().ToList();
            return subjects;
        }

        /**********************************************************************************************//**
         * Gets algorithms of subject.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   subject The subject.
         *
         * \return  The algorithms of subject.
         **************************************************************************************************/

        public static List<string> GetAlgorithmsOfSubject(string subject)
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.Namespace != null)
                        .Where(t => t.Namespace.ToString().Contains("DistributedAlgorithms.Algorithms." + subject))
                         .Select(t => t.Namespace.ToString().Replace("DistributedAlgorithms.Algorithms." + subject + ".", ""))
                         .Distinct()
                         .ToList();
        }

        /**********************************************************************************************//**
         * Compare dynamics.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   prm1    The first prm.
         * \param   prm2    The second prm.
         *
         * \return  True if it succeeds, false if it fails.
         **************************************************************************************************/

        public static bool CompareDynamics(dynamic prm1, dynamic prm2)
        {
            if (prm1 == null)
            {
                if (prm2 == null)
                {
                    return true;
                }
                return false;
            }

            if (prm1.GetType().Equals(prm2.GetType()))
            {
                if (prm1.Equals(prm2))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
