/**********************************************************************************************//**
 * \file    Infrastructure\MessageRouter.cs
 *
 * Implements the message router class.
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * A message router.
     *
     * \author  Ilan Hindy
     * \date    16/01/2017
     **************************************************************************************************/

    public static class MessageRouter
    {
        /** Event queue for all listeners interested in reportMessage events. */
        public static event EventHandler<object[]> ReportMessageEvent;
        /** Event queue for all listeners interested in reportFinish events. */
        public static event EventHandler<string> ReportFinishEvent;
        /** Event queue for all listeners interested in reportStart events. */
        public static event EventHandler<string> ReportStartEvent;
        /** Event queue for all listeners interested in reportChangePresentation events. */
        public static event EventHandler<object[]> ReportChangePresentationEvent;
        /** Event queue for all listeners interested in reportMessageSent events. */
        public static event EventHandler<object[]> ReportMessageSentEvent;
        /** Event queue for all listeners interested in reportMessageReceive events. */
        public static event EventHandler<object[]> ReportMessageReceiveEvent;
        /** Event queue for all listeners interested in checkFinishProcessingStep events. */
        public static event EventHandler<object[]> CheckFinishProcessingStepEvent;
        public static event EventHandler<object[]> UpdateInitRunningPresentationEvent;
        public static event EventHandler<object[]> UpdateRunningPresentationEvent;
        /** Event queue for all listeners interested in addEditOperation events. */
        public static event EventHandler<object[]> AddEditOperationEvent;
        public static event EventHandler<object[]> AddEditOperationResultEvent;
        //public static event EventHandler<object[]> MessageBoxEvent;
        public delegate string MessageBoxDelegate(object sender, object[] prms);
        public static event MessageBoxDelegate MessageBoxEvent;
        public static event MessageBoxDelegate CustomizedMessageBoxEvent;
        //public static event EventHandler<object[]> CustomizedMessageBoxEvent;

        /**********************************************************************************************//**
         * Reports finish running.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public static void ReportFinishRunning()
        {
            ReportFinishEvent(null, null);
        }

        /**********************************************************************************************//**
         * Reports start running.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public static void ReportStartRunning()
        {
            ReportStartEvent(null, null);
        }

        /**********************************************************************************************//**
         * Reports change presentation of component.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   networkElement  The network element.
         * \param   parameters      Options for controlling the operation.
         
         **************************************************************************************************/

        public static void ReportChangePresentationOfComponent(NetworkElement networkElement, object[] parameters)
        {
            ReportChangePresentationEvent(networkElement, parameters);
        }

        /**********************************************************************************************//**
         * Reports message sent.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process     The process.
         * \param   parameters  Options for controlling the operation.
         *                      
         **************************************************************************************************/

        public static void ReportMessageSent(BaseProcess process, object[] parameters)
        {
            Logger.Log(Logger.LogMode.MainLogAndMessageTrace, process.GetProcessDefaultName(), "", "In ReportMessageSent Process type = " + process.GetType().ToString());
            ReportMessageSentEvent(process, parameters);
        }

        /**********************************************************************************************//**
         * Reports message receive.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process     The process.
         * \param   parameters  Options for controlling the operation.
         *                      
         **************************************************************************************************/

        public static void ReportMessageReceive(BaseProcess process, object[] parameters)
        {
            ReportMessageReceiveEvent(process, parameters);
        }

        /**********************************************************************************************//**
         * Check finish processing step.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   process     The process.
         * \param   parameters  Options for controlling the operation.
         *                      
         **************************************************************************************************/

        public static void CheckFinishProcessingStep(BaseProcess process, object[] parameters)
        {
            CheckFinishProcessingStepEvent(process, parameters);
        }

        public static void UpdateInitRunningPresentation()
        {
            UpdateInitRunningPresentationEvent(null, null);
        }

        public static void UpdateRunningPresentation()
        {
            UpdateRunningPresentationEvent(null, null);
        }

        /**********************************************************************************************//**
         * Reports a message.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender      The sender.
         * \param   vectorClock The vector clock.
         * \param   message     The message.
         **************************************************************************************************/

        public static void ReportMessage(string sender, string vectorClock, string message)
        {
            ReportMessageEvent(null, new object[] { sender, vectorClock, message });
        }

        /**********************************************************************************************//**
         * Adds an edit operation.
         *
         * \author  Ilan Hindy
         * \date    16/01/2017
         *
         * \param   sender      The sender.
         * \param   message     The message.
         * \param   result      The result.
         * \param   imageIcon   The image icon.
         * \param   font        The font.
         **************************************************************************************************/

        public static void AddEditOperation(string sender, string message, Icons imageIcon, Font font)
        {
            AddEditOperationEvent(null, new object[] { sender, message, imageIcon, font});
        }

        public static void AddEditOperationResult(string result, Font font)
        {
            AddEditOperationResultEvent(null, new object[] { result, font });
        }

        public static string MessageBox(List<string> labelsStrings, string title = "", List<string> buttonsStrings = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        {
            return MessageBoxEvent(null, new object[] { labelsStrings, title, buttonsStrings, imageIcon, sizeToContent });            
        }

        public static string CustomizedMessageBox(List<MessageBoxElementData> labels, string title = "", List<MessageBoxElementData> buttons = null, Icons imageIcon = Icons.Info, bool sizeToContent = false)
        {
            return CustomizedMessageBoxEvent(null, new object[] { labels, title, buttons, imageIcon, sizeToContent });
        }
    }
}
