////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Algorithms\Base\ClassFactory.cs
///
///\brief   Implements the network element class factory class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DistributedAlgorithms.Algorithms.Base.Base;
using System.IO;

namespace DistributedAlgorithms
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class ClassFactory
    ///
    /// \brief The class factory.
    ///
    /// \par Description.
    ///
    /// \par Usage Notes.
    ///
    /// \author Ilanh
    /// \date 19/11/2017
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ClassFactory
    {  
        #region

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GenerateNamespace(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a namespace.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The namespace.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GenerateNamespace(string subject = null, string algorithm = null)
        {
            if (subject is null)
            {
                subject = Config.Instance[Config.Keys.SelectedSubject];
            }

            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return "DistributedAlgorithms.Algorithms." + subject + "." + algorithm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseNetwork GenerateNetwork(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a network.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The network.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseNetwork GenerateNetwork(string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseNetwork)TypesUtility.CreateObjectFromTypeString(GenerateNamespace(subject, algorithm) + "." + algorithm + "Network");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseProcess GenerateProcess(string subject = null, string algorithm = null)
        ///
        /// \brief Generates the process.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseProcess GenerateProcess(string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }

            BaseProcess process = (BaseProcess)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace() + "." + Config.Instance[Config.Keys.SelectedAlgorithm] + "Process");
            return process;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseProcess GenerateProcess(BaseNetwork network, string subject = null, string algorithm = null)
        ///
        /// \brief Generates the process.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network    (BaseNetwork) - The network.
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseProcess GenerateProcess(BaseNetwork network, string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }

            BaseProcess process = (BaseProcess)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace() + "." + Config.Instance[Config.Keys.SelectedAlgorithm] + "Process", new object[] { network });
            return process;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseProcess GenerateProcess(bool permissionsValue, string subject = null, string algorithm = null)
        ///
        /// \brief Generates the process.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param permissionsValue  (bool) - true to permissions value.
        /// \param subject          (Optional)  (string) - The subject.
        /// \param algorithm        (Optional)  (string) - The algorithm.
        ///
        /// \return The process.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseProcess GenerateProcess(bool permissionsValue, string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }

            BaseProcess process = (BaseProcess)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace() + "." + Config.Instance[Config.Keys.SelectedAlgorithm] + "Process", new object[] { permissionsValue });
            return process;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static NetworkElement BaseNetworkElement(string classType)
        ///
        /// \brief Base network element.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param classType  (string) - Type of the class.
        ///
        /// \return A NetworkElement.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static NetworkElement BaseNetworkElement(string classType)
        {
            NetworkElement result = (NetworkElement)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace("Base", "Base") + "." + "Base" + classType);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseChannel GenerateChannel(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a channel.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseChannel GenerateChannel(string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            BaseChannel channel = (BaseChannel)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Channel");
            return channel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseChannel GenerateChannel(BaseNetwork network, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a channel.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network    (BaseNetwork) - The network.
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseChannel GenerateChannel(BaseNetwork network, string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            BaseChannel channel = (BaseChannel)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Channel", new object[] { network });
            return channel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseChannel GenerateChannel(bool permissionsValue, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a channel.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param permissionsValue  (bool) - true to permissions value.
        /// \param subject          (Optional)  (string) - The subject.
        /// \param algorithm        (Optional)  (string) - The algorithm.
        ///
        /// \return The channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseChannel GenerateChannel(bool permissionsValue, string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            BaseChannel channel = (BaseChannel)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Channel", new object[] { permissionsValue });
            return channel;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseChannel GenerateChannel(BaseNetwork network, int id, int sourceProcessId, int destProcessId, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a channel.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network          (BaseNetwork) - The network.
        /// \param id               (int) - The identifier.
        /// \param sourceProcessId  (int) - Identifier for the source process.
        /// \param destProcessId    (int) - Identifier for the destination process.
        /// \param subject         (Optional)  (string) - The subject.
        /// \param algorithm       (Optional)  (string) - The algorithm.
        ///
        /// \return The channel.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseChannel GenerateChannel(BaseNetwork network, 
            int id, 
            int sourceProcessId, 
            int destProcessId,
            string subject = null, 
            string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseChannel)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Channel",
                new object[] { network, id, sourceProcessId, destProcessId});
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Type GenerateMessageType()
        ///
        /// \brief Generates a message type.
        ///
        /// \par Description.
        ///      Generate the type of the message of the current parameter
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 14/05/2018
        ///
        /// \return The message type.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Type GenerateMessageType()
        {
            string algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            return Type.GetType(GenerateNamespace() + "." + algorithm + "Message");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static BaseMessage GenerateMessage(BaseNetwork network, dynamic messageType, AttributeDictionary fields, BaseChannel channel, string messageName = "", string subject = null, string algorithm = null, int round = 0, int logicalClock = 0)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network       (BaseNetwork) - The network.
        /// \param messageType   (dynamic) - Type of the message.
        /// \param fields        (AttributeDictionary) - The fields.
        /// \param channel       (BaseChannel) - The channel.
        /// \param messageName  (Optional)  (string) - Name of the message.
        /// \param subject      (Optional)  (string) - The subject.
        /// \param algorithm    (Optional)  (string) - The algorithm.
        /// \param round        (Optional)  (int) - The round.
        /// \param logicalClock (Optional)  (int) - The logical clock.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static BaseMessage GenerateMessage(BaseNetwork network,
            dynamic messageType,
            AttributeDictionary fields,
            BaseChannel channel,            
            string messageName = "",
            string subject = null, 
            string algorithm = null,
            int round = 0, int logicalClock = 0)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }

            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                new object[] { network, messageType, fields, channel, "", round, logicalClock });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                           GenerateNamespace(subject, algorithm) + "." + algorithm + "Message");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, Permissions permissions = null, IValueHolder parent = null, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network      (BaseNetwork) - The network.
        /// \param permissions (Optional)  (Permissions) - The permissions.
        /// \param parent      (Optional)  (IValueHolder) - The parent.
        /// \param subject     (Optional)  (string) - The subject.
        /// \param algorithm   (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network,
            Permissions permissions = null,
           IValueHolder parent = null,
            string subject = null,
            string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                new object[] { network, permissions, parent });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, BaseMessage sourceMessage, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network        (BaseNetwork) - The network.
        /// \param sourceMessage  (BaseMessage) - Message describing the source.
        /// \param subject       (Optional)  (string) - The subject.
        /// \param algorithm     (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network, 
            BaseMessage sourceMessage,
            string subject = null, 
            string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                new object[] { network, sourceMessage});
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, BaseMessage sourceMessage, BaseChannel sendingChannel, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network         (BaseNetwork) - The network.
        /// \param sourceMessage   (BaseMessage) - Message describing the source.
        /// \param sendingChannel  (BaseChannel) - The sending channel.
        /// \param subject        (Optional)  (string) - The subject.
        /// \param algorithm      (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network,
            BaseMessage sourceMessage,
            BaseChannel sendingChannel,
            string subject = null, 
            string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                new object[] { network, sourceMessage, sendingChannel });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, dynamic messageType, AttributeDictionary fields, BaseChannel channel, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network      (BaseNetwork) - The network.
        /// \param messageType  (dynamic) - Type of the message.
        /// \param fields       (AttributeDictionary) - The fields.
        /// \param channel      (BaseChannel) - The channel.
        /// \param subject     (Optional)  (string) - The subject.
        /// \param algorithm   (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network,
            dynamic messageType,
            AttributeDictionary fields,
            BaseChannel channel,
            string subject = null, 
            string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                           GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                           new object[] { network, messageType, fields, channel });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, string messageString, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network        (BaseNetwork) - The network.
        /// \param messageString  (string) - The message string.
        /// \param subject       (Optional)  (string) - The subject.
        /// \param algorithm     (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network, string messageString, string subject = null, string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                                       GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                                       new object[] { network, messageString });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, dynamic messageType, int sourceProcess, int sourcePort, int destProcess, int destPort, int round, int logicalClock, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network        (BaseNetwork) - The network.
        /// \param messageType    (dynamic) - Type of the message.
        /// \param sourceProcess  (int) - Source process.
        /// \param sourcePort     (int) - Source port.
        /// \param destProcess    (int) - Destination process.
        /// \param destPort       (int) - Destination port.
        /// \param round          (int) - The round.
        /// \param logicalClock   (int) - The logical clock.
        /// \param subject       (Optional)  (string) - The subject.
        /// \param algorithm     (Optional)  (string) - The algorithm.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network,
            dynamic messageType,
            int sourceProcess,
            int sourcePort,
            int destProcess,
            int destPort,
            int round,
            int logicalClock,
            string subject = null, 
            string algorithm = null)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                           GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                           new object[] { network, messageType, 
                           sourceProcess, sourcePort, destProcess, destPort, round, logicalClock});
 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public BaseMessage GenerateMessage(BaseNetwork network, dynamic messageType, BaseChannel channel, string subject = null, string algorithm = null, int round = 0, int logicalClock = 0)
        ///
        /// \brief Generates a message.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param network       (BaseNetwork) - The network.
        /// \param messageType   (dynamic) - Type of the message.
        /// \param channel       (BaseChannel) - The channel.
        /// \param subject      (Optional)  (string) - The subject.
        /// \param algorithm    (Optional)  (string) - The algorithm.
        /// \param round        (Optional)  (int) - The round.
        /// \param logicalClock (Optional)  (int) - The logical clock.
        ///
        /// \return The message.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public BaseMessage GenerateMessage(BaseNetwork network,
            dynamic messageType,
            BaseChannel channel,
            string subject = null, 
            string algorithm = null,
            int round = 0, int logicalClock = 0)
        {
            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            return (BaseMessage)TypesUtility.CreateObjectFromTypeString(
                           GenerateNamespace(subject, algorithm) + "." + algorithm + "Message",
                           new object[] { network, messageType, channel, round, logicalClock });
        }

        #endregion
        #region

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Type GenerateMessageTypeEnum(string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message type enum.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The message type enum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Type GenerateMessageTypeEnum(string subject = null, string algorithm = null)
        {
            if (subject is null)
            {
                subject = Config.Instance[Config.Keys.SelectedSubject];
            }

            if (algorithm is null)
            {
                algorithm = Config.Instance[Config.Keys.SelectedAlgorithm];
            }
            if (subject == "Base" && algorithm == "Base")
            {
                return Type.GetType(GenerateNamespace("Base", "Base") + "." + "bm+messageTypes");
            }
            else
            {
                return Type.GetType(GenerateNamespace(subject, algorithm) + "." + "m+MessageTypes");
            }            
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Type GenerateMessageEnum(string messageKey, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message enum.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param messageKey  (string) - The message key.
        /// \param subject    (Optional)  (string) - The subject.
        /// \param algorithm  (Optional)  (string) - The algorithm.
        ///
        /// \return The message enum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Type GenerateMessageEnum(string messageKey, string subject = null, string algorithm = null)
        {
            return Type.GetType(GenerateNamespace(subject, algorithm) + "." + "m+" + messageKey);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static Type GenerateEnum(string enumName, string enumClass, string subject = null, string algorithm = null)
        ///
        /// \brief Generates a message enum.
        ///
        /// \par Description.
        ///      Return the type of enum from strings
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 31/12/2017
        ///
        /// \param enumName  (string) - The message key.
        /// \param enumClass  (string) - The enum class.
        /// \param subject   (Optional)  (string) - The subject.
        /// \param algorithm (Optional)  (string) - The algorithm.
        ///
        /// \return The message enum.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Type GenerateEnum(string enumName, string enumClass, string subject = null, string algorithm = null)
        {
            return Type.GetType(GenerateNamespace(subject, algorithm) + "." + enumClass + "+" + enumName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GenerateAlgorithmPath(string subject, string algorithm)
        ///
        /// \brief Generates an algorithm path.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        ///
        /// \return The algorithm path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GenerateAlgorithmPath(string subject, string algorithm)
        {           
            return Path.GetFullPath(Config.Instance[Config.Keys.AlgorithmsPath] + subject + "\\" + algorithm);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GenerateSubjectPath(string subject)
        ///
        /// \brief Generates a subject path.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 13/12/2017
        ///
        /// \param subject  (string) - The subject.
        ///
        /// \return The subject path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GenerateSubjectPath(string subject)
        {
            return Path.GetFullPath(Config.Instance[Config.Keys.AlgorithmsPath] + subject);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string GenerateAlgorithmBackupPath(string subject, string algorithm)
        ///
        /// \brief Generates an algorithm backup path.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \param subject    (string) - The subject.
        /// \param algorithm  (string) - The algorithm.
        ///
        /// \return The algorithm backup path.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GenerateAlgorithmBackupPath(string subject, string algorithm)
        {
            return Path.GetFullPath(Config.Instance[Config.Keys.AlgorithmsPath] + subject + "\\" + algorithm + " - Backup");
        }
        #endregion

        #region /// \name Create Message from the enum

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static AttributeDictionary MessageDictionary(dynamic messageKey, NetworkElement messageDemo, BaseProcess process)
        ///
        /// \brief Message dictionary.
        ///        Create a message dictionary according to the message type key
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 24/12/2017
        ///
        /// \param messageKey   (dynamic) - The message key.
        /// \param messageDemo  (NetworkElement) - The message demo.
        /// \param process      (BaseProcess) - The process.
        ///
        /// \return An AttributeDictionary.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static AttributeDictionary MessageDictionary(dynamic messageKey, NetworkElement messageDemo, BaseProcess process)
        {
            // Add an empty dictionary to the OperationResults
            messageDemo.or.Add(messageKey, new Attribute { Value = new AttributeDictionary() });

            // Generate the method name
            string methodName = ((AttributeDictionary)messageDemo.or[messageKey]).MessageMethodName();

            // Generate the enum name of the message. Note that Enum names begin with small letter and 
            // Enum values begin with big letter so there is a need to set the first letter to lower
            // In order to get the enum name
            string keyString = TypesUtility.GetKeyToString(messageKey);
            keyString = char.ToLower(keyString[0]) + keyString.Substring(1);

            // The method for getting the data is invoked by it's name. When invoking a method in this
            // way the exact number of parameters has to be given. Each method has a different number of parameters
            // (according to the number of entries in the message enum so this number has to be retrieved
            // from the enum
            int optionalPrmsCount = Enum.GetValues(GenerateMessageEnum(keyString)).Length;

            // Activating the method
            return (AttributeDictionary)TypesUtility.InvokeMethod(process, methodName, new List<object> { bm.PrmSource.Default, null }, optionalPrmsCount, true);
        }
        #endregion
        #region

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn public static string IconsPath()
        ///
        /// \brief Icons path.
        ///
        /// \par Description.
        ///
        /// \par Algorithm.
        ///
        /// \par Usage Notes.
        ///
        /// \author Ilanh
        /// \date 19/11/2017
        ///
        /// \return A string.
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string IconsPath()
        {
            return Path.GetFullPath("~\\..\\..\\..\\Icons\\");
        }

#endregion
    }
}
