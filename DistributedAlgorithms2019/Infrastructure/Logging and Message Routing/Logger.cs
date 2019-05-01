////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\Logger.cs
///
///\brief   Implements the logger class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /*
     * Class Logger
     * This class handles log files and error messages
     * The following is the features of this class
     * 1. This class handle a main log file that receives logs from all the objects.
     * 2. This class handles a separate log file receives logs according to the fields
     * 3. Multipile threads can send logs to one file
     * 4. The log methods have a filter parameter according which the class can decide if to write
     *    or not to write the log
     * 5. The log methods have a LogMode parameter that handle the targets of the logs
     *    5.1 Whether to write the log to the main log file
     *    5.2 Whether to write the log to the process log file
     *    5.3 Whether to generate a CustomizedMessageBox with the log
     * 6. The following are the log file name conventions
     *    6.1 The main log file name is composed from 3 parts : path + name + "log"
     *    6.2 The process log file is composed from 4 parts : path + main log name + process name + "log
     * 7. There are 2 main log functions
     *    7.1 log a string
     *    7.2 log an XmlDocument object
     */

    /**********************************************************************************************//**
     * A logger.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class Logger 
    {
        /** Filename of the log file. */
        public static string LogFileName;
        /** Full pathname of the log file. */
        public static string LogPath;
        /** The trace listener. */
        private static TextWriterTraceListener traceListener = null;
        /** The trace log file. */
        private static FileStream traceLogFile = null;
        /** The filters. */
        public static List<string> Filters = new List<string>();

        /** The edit trace listener. */
        private static TextWriterTraceListener editTraceListener = null;
        /** The edit log file. */
        private static FileStream editLogFile = null;
        /** The logs before configuration load. */
        private static List<string> logsBeforeConfigLoad = new List<string>();

        /**********************************************************************************************//**
         * Values that represent log modes.
        
         **************************************************************************************************/

        public enum LogMode { MainLog, MainLogAndProcessLog, MainLogAndError, MainLogProcessLogAndError, MainLogAndMessageTrace, MainLogAndProcessLogAndMessageTrace };
        /** The process log files. */
        private static Dictionary<string, TextWriterTraceListener> processLogFiles = new Dictionary<string, TextWriterTraceListener>();
        /** The write log lock object. */
        private static object WriteLogLockObject = new object();
        /** The vector clock. */
        private static List<int> vectorClock = new List<int>();   

       /*
        * Methods for initializing loading and saving the Logger members
        */

        /**********************************************************************************************//**
         * S this object.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public static void Init()
        {
            LogFileName = Config.Instance[Config.Keys.SelectedLogFileName].Replace(".log","");
            LogPath = Config.Instance[Config.Keys.SelectedLogPath];
            //Open main log file
            OpenMainLogFile();

            //Default filters - filters are string which role is to decide whether a log 
            //will be printed
            InitFilters();
        }

        public static void LoadFilters(XmlNode xmlNode, Type hostType = null)
        {
            AttributeList attributeList = new AttributeList();
            attributeList.Load(xmlNode);
            Filters.FromAttributeList(attributeList);
        }

        public static void SaveFilters(XmlDocument xmlDoc, XmlNode xmlNode)
        {
            AttributeList attributeList = Filters.ToAttributeList();
            attributeList.Save(xmlDoc, xmlNode);
        }

        /**********************************************************************************************//**
         * Edit log init.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         **************************************************************************************************/

        public static void EditLogInit()
        {
            string editLogFileName = Config.Instance[Config.Keys.EditLogFileName];
            string editLogFilePath = Config.Instance[Config.Keys.EditLogFilePath];
            CloseEditLogFile();

            string fileName = editLogFilePath + editLogFileName + "." + "log";

            //The log file operates by appending text so it has to be deleted at the beginning of the operation 
            if (File.Exists(fileName)) File.Delete(fileName);

            //Create the file stream that the main log file will be written to
            editLogFile = new FileStream(fileName, FileMode.OpenOrCreate);

            // Creates the new trace listener.
            editTraceListener = new System.Diagnostics.TextWriterTraceListener(editLogFile);

            // Write the logs that where generated before opening the file
            foreach (string l in logsBeforeConfigLoad)
            {
                editTraceListener.WriteLine(l);
                editTraceListener.Flush();
            }
            logsBeforeConfigLoad = new List<string>();

        }

        /**********************************************************************************************//**
         * Closes edit log file.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         **************************************************************************************************/

        public static void CloseEditLogFile()
        {
            if (editTraceListener != null)
            {
                editTraceListener.Flush();
                Trace.Listeners.Remove(editTraceListener);
                editTraceListener.Close();
            }
        }

        /**********************************************************************************************//**
         * Sets vector clock.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   vectorClock The vector clock.
         **************************************************************************************************/

        public static void SetVectorClock(List<int> vectorClock)
        {
            Logger.vectorClock = vectorClock;
        }

        /**********************************************************************************************//**
         * Writes to edit log file.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender          The sender.
         * \param   messageLines    The message lines.
         * \param   result          The result.
         * \param   icon            The icon.
         **************************************************************************************************/

        public static string WriteOperationToEditLogFile(string sender, string [] messageLines, Icons icon, bool newLineAtTheEnd = true)
        {
            string timeString = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            string iconString = TypesUtility.GetKeyToString(icon);
            string header = timeString + " " + sender + iconString + " : ";
            string emptySpaces = new string(' ', header.Length);
            header += messageLines[0] + "\r\n";
            string log = header;
            for (int idx = 1; idx < messageLines.Length; idx++)
            {
                log += emptySpaces + messageLines[idx];
            }

            if (!newLineAtTheEnd)
            {
                log = log.Remove(log.Length - 3);
            }
            
            // The config is loaded before the edit log file is opened so we save the
            // logs untill the the file is opened and then dump them at the beginning of the log
            if (editTraceListener == null)
            {
                logsBeforeConfigLoad.Add(log);
            }
            else
            {
                editTraceListener.WriteLine(log);
                editTraceListener.Flush();
            }
            return emptySpaces;
        }

        public static void WriteOperationResultToEditLogFile(string emptySpaces, string result)
        {            
            string log = emptySpaces + "Returned : " + result + "\r\n";

            // The config is loaded before the edit log file is opened so we save the
            // logs untill the the file is opened and then dump them at the beginning of the log
            if (editTraceListener == null)
            {
                logsBeforeConfigLoad.Add(log);
            }
            else
            {
                editTraceListener.WriteLine(log);
                editTraceListener.Flush();
            }
        }

        /**********************************************************************************************//**
         * Init filters.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public static void InitFilters()
        {
            Filters.Clear();
            Filters.Add("ProcessSend");
            Filters.Add("ProcessReceive");
            Filters.Add("Error");
            Filters.Add("RunningHandler");
            Filters.Add("RunningHandler TerminationAlgorithmStatus");
            Filters.Add("General");
        }

        /**********************************************************************************************//**
         * Loads the given XML node.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   xmlNode The XML node to load.
         
         **************************************************************************************************/

        public static void Load(XmlNode xmlNode)
        {
            LogFileName = xmlNode.Attributes["LogFileName"].Value;
            LogPath = xmlNode.Attributes["LogPath"].Value;
            OpenMainLogFile();
            XmlNodeList childNodes = xmlNode.ChildNodes;
            Filters.Clear();
            foreach (XmlNode childNode in childNodes)
            {
                if (childNode.Name == "Filters")
                {
                    foreach (XmlNode filterNode in childNode.ChildNodes)
                    {
                        Filters.Add(filterNode.InnerText);
                    }
                }
            }
        }

        /**********************************************************************************************//**
         * Opens main log file.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private static void OpenMainLogFile()
        {
            CloseLogFiles();

            string fileName = LogPath + LogFileName + "." + "log";

            //The log file operates by appending text so it has to be deleted at the beginning of the operation 
            if (File.Exists(fileName)) File.Delete(fileName);

            //Create the file stream that the main log file will be written to
            traceLogFile = new FileStream(fileName, FileMode.OpenOrCreate);
            // Creates the new trace listener.
            traceListener = new System.Diagnostics.TextWriterTraceListener(traceLogFile);

        }

        /**********************************************************************************************//**
         * Closes log files.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        private static void CloseLogFiles()
        {
            if (traceListener != null)
            {
                traceListener.Flush();
                Trace.Listeners.Remove(traceListener);
                traceListener.Close();
            }

            foreach (TextWriterTraceListener listener in processLogFiles.Values)
            {
                listener.Flush();
                Trace.Listeners.Remove(listener);
                listener.Close();
            }
            processLogFiles.Clear();
        }

        /**********************************************************************************************//**
         * Closes all log files.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public static void CloseAllLogFiles()
        {
            if (traceLogFile != null)
            {
                traceLogFile.Close();
                traceListener.Close();
                traceLogFile = null;
                traceListener = null;
            }

            while (processLogFiles.Count > 0)
            {
                var entry = processLogFiles.ElementAt(0);
                entry.Value.Close();
                processLogFiles.Remove(entry.Key);
            }
        }

        /**********************************************************************************************//**
         * Saves the given XML document.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   xmlDoc  The XML document to save.
         *
         * \return  An XmlNode.
         .
         **************************************************************************************************/

        //public static XmlNode Save(XmlDocument xmlDoc)
        //{
        //    XmlNode xmlNode = xmlDoc.CreateElement("Logger");
        //    XmlAttribute xmlAttribute;
        //    xmlAttribute = xmlDoc.CreateAttribute("LogFileName");
        //    xmlAttribute.Value = LogFileName;
        //    xmlNode.Attributes.Append(xmlAttribute);
        //    xmlAttribute = xmlDoc.CreateAttribute("LogPath");
        //    xmlAttribute.Value = LogPath;
        //    xmlNode.Attributes.Append(xmlAttribute);
        //    XmlNode filtersNode = xmlDoc.CreateElement("Filters");
        //    xmlNode.AppendChild(filtersNode);
        //    foreach (string filter in Filters)
        //    {
        //        XmlNode filterNode = xmlDoc.CreateElement("Filter");
        //        filterNode.InnerText = filter;
        //        filtersNode.AppendChild(filterNode);
        //    }
        //    return xmlNode;
        //}

        /*
         * Logging functions
         * Note that in order to avoid log fregments all the log parts are collected to string and 
         *      thare is only one write command in WriteLog
         * WriteLogFileHeader
         * The log file header contains a line with the date + time + miliseconds the method name and the message header
         * This method outputs a string with empty spaces at length of the time string for other line headers
         */

        /**********************************************************************************************//**
         * Writes a log header.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param       sourceObject    Source object.
         * \param       function        The function.
         * \param       messageHeader   The message header.
         * \param [out] emptySpaces     The empty spaces.
         *
         * \return  A String.
         .
         **************************************************************************************************/

        private static String WriteLogHeader(string sourceObject, string function, string messageHeader,out string emptySpaces)
        {

            string timeString = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff");
            string vectorClockString = " " + VectorClockString();
            string log = timeString + " " + "VC=" + VectorClockString() + " Message from : " + sourceObject + "\r\n";
            emptySpaces = new String(' ', timeString.Length + 1);
            log += emptySpaces + "Function :" + function +"\r\n";
            log += emptySpaces + "Message :" + messageHeader +"\r\n";
            return log;

        }

        /**********************************************************************************************//**
         * Vector clock string.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \return  A string.
         **************************************************************************************************/

        public static string VectorClockString()
        {
            string result = "[";
            for (int idx = 0; idx < vectorClock.Count; idx++)
            {
                result += " " + vectorClock[idx].ToString() + ",";
            }
            result = result.Substring(0, result.Length - 1) + "]";
            return result;
        }


        /*
         * WriteLog
         * This method actually writes the log to all the log destinations
         */

        /**********************************************************************************************//**
         * Writes a log.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   logMode         The log mode.
         * \param   sourceObject    Source object.
         * \param   log             The log.
         * \param   messageTraceLog The message trace log.
         * \param   logFilter       A filter specifying the log.
         
         **************************************************************************************************/

        private static void WriteLog(LogMode logMode, string sourceObject, string log, string messageTraceLog, string logFilter)
        {
            lock (WriteLogLockObject)
            {
                //Find Whether to log according to the filter
                //If the Filters list is empty write all the logs othere wise the logFilter parameter has
                //to be found in the Filters list in order to write the log
                    if (Filters.Count > 0)
                    {
                        if (Filters.FirstOrDefault(s => s == logFilter) is null)
                        {
                            return;
                        }
                    }

                //Write to the main log file
                if (traceLogFile == null)
                {
                    OpenMainLogFile();
                }
                traceListener.WriteLine(log);
                traceListener.Flush();

                //Write to the process private log file
                if (logMode == LogMode.MainLogAndProcessLog ||
                    logMode == LogMode.MainLogProcessLogAndError ||
                    logMode == LogMode.MainLogAndProcessLogAndMessageTrace)
                {
                    TextWriterTraceListener processTraceListener = GetProcessTraceListener(sourceObject);
                    processTraceListener.WriteLine(log);
                    processTraceListener.Flush();
                }

                //Create error message if needed
                if (logMode == LogMode.MainLogAndError ||
                    logMode == LogMode.MainLogProcessLogAndError)
                {
                    MessageRouter.MessageBox(new List<String> { log },"Log Report", null, Icons.Error);
                }
            }

            //Write to message trace window
            if (logMode == LogMode.MainLogAndProcessLogAndMessageTrace || logMode == LogMode.MainLogAndMessageTrace)
            {
                MessageRouter.ReportMessage(sourceObject, VectorClockString(), messageTraceLog);
            }
        }

        /*
         * Get the TextWriterTraceListener of the private log file
         * The Listeners are found in a dictionary with the process file name as the key
         */

        /**********************************************************************************************//**
         * Gets process trace listener.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   sourceObject    Source object.
         *
         * \return  The process trace listener.
         .
         **************************************************************************************************/

        private static TextWriterTraceListener GetProcessTraceListener(string sourceObject)
        {
            //Generate the private log file name
            string processLogFileName = LogPath + LogFileName + "." + sourceObject + "." + "log";

            //Search the process log file name in the dictionary
            if (processLogFiles.ContainsKey(processLogFileName) == true)
            {
                //If the process log file name is found in the dictionary return the listener
                return processLogFiles[processLogFileName];
            }
            else
            {
                //If the process log file name does not exist - Generate a new one
                //Delete the file if exist
                if (File.Exists(processLogFileName))
                {
                    System.IO.File.Delete(processLogFileName);
                }

                // Creates the text file that the trace listener will write to.
                traceLogFile = new FileStream(processLogFileName, FileMode.OpenOrCreate);

                // Create and return the new trace listener.
                processLogFiles.Add(processLogFileName, new System.Diagnostics.TextWriterTraceListener(traceLogFile));
                return processLogFiles[processLogFileName];
            }
        }

        /*
         * Log a string message
         */

        /**********************************************************************************************//**
         * Logs.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   logMode         The log mode.
         * \param   sourceObject    Source object.
         * \param   function        The function.
         * \param   messageHeader   The message header.
         * \param   messageBody     (Optional) The message body.
         * \param   logFilter       (Optional) A filter specifying the log.
         
         **************************************************************************************************/

        public static void Log(LogMode logMode, 
            string sourceObject, 
            string function, 
            string messageHeader, 
            string messageBody = "",
            string logFilter = "")
        {
            String emptySpaces = null;
            String log = WriteLogHeader(sourceObject, function, messageHeader, out emptySpaces);
            string[] messageLines = Regex.Split(messageBody, @"\r?\n|\r");
            foreach (string line in messageLines )
            {
                log += emptySpaces + line +"\r\n";
            }
            String messageTraceLog = messageHeader + " " + messageBody;
            WriteLog(logMode, sourceObject, log, messageTraceLog, logFilter);
        }
     
        /*
         * Log a XmlDocument
         */

        /**********************************************************************************************//**
         * Logs.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   logMode         The log mode.
         * \param   sourceObject    Source object.
         * \param   function        The function.
         * \param   messageHeader   The message header.
         * \param   networkElement  The network element.
         * \param   logFilter       (Optional) A filter specifying the log.
         
         **************************************************************************************************/

        public static void Log(LogMode logMode,
            string sourceObject, 
            string function, 
            string messageHeader, 
            NetworkElement networkElement,
            string logFilter = "")
        {
            string messageTraceLog;
            string emptySpaces = null;
            string log = WriteLogHeader(sourceObject, function, messageHeader, out emptySpaces);
            if (networkElement != null)
            {
                string[] messageLines = formatXml(networkElement.ElementXml());
                foreach (string line in messageLines)
                {
                    log += emptySpaces + line + "\r\n";
                }
                messageTraceLog = messageHeader + " " + networkElement.ShortDescription();
            }
            else
            {
                messageTraceLog = messageHeader + " " + "Null network element";
            }       
            WriteLog(logMode, sourceObject, log, messageTraceLog, logFilter);
        }
 
        /*
         * Convert XmlDocument to string and divide it to lines
         */

        /**********************************************************************************************//**
         * Format XML.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   xmlDoc  The XML document.
         *
         * \return  The formatted XML.
         .
         **************************************************************************************************/

        public static String[] formatXml(XmlDocument xmlDoc)
        {
            String xmlMessage = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);

            writer.Formatting = Formatting.Indented;

            // Write the XML into a formatting XmlTextWriter
            xmlDoc.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;

            // Read MemoryStream contents into a StreamReader.
            StreamReader sReader = new StreamReader(mStream);

            // Extract the text from the StreamReader.
            String FormattedXML = sReader.ReadToEnd();

            xmlMessage = FormattedXML;
            string[] splittedXmlMessage = Regex.Split(xmlMessage, @"\r?\n|\r");
            mStream.Close();
            writer.Close();
            return splittedXmlMessage;
        }
  }
}
