////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\AsynchronousReader.cs
///
///\brief   Implements the asynchronous reader class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /*
     * AsynchronousReader
     * This class is responssible to read from a channel
     * Each process has one object from this class for each input channel
     * The object works in a thread. The thread is created in the constructor
     * The thread activate an infinit receive loop
     * The infinit loop ends when a message with MessageType property is "Terminate"
     * Each time data is received by the socket the function ReceiveHandling is called
     * The ReceivedHandling method is responssible to devide the data received to packets
     * And activate the ReceiveHandling method of the process
     */

    /**********************************************************************************************//**
     * The asynchronous reader.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class AsynchronousReader
    {
        /** The state. */
        private SocketStateObject state = null;
        /** The read thread. */
        public Thread ReadThread = null;
        /** true to terminate flag. */
        private bool terminateFlag = false;

        /*
         * AsynchronousReader (constructor)
         * Does the following :
         * 1. Copy the state object which contains all the data needed for reading to member variable
         * 2. Create and activate the reading thread
         */

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   listenState State of the listen.
         *                      
         **************************************************************************************************/

        public AsynchronousReader(SocketStateObject listenState)
        {
            this.state = new SocketStateObject(listenState);
            string sourcePort = ((IPEndPoint)state.workSocket.RemoteEndPoint).Port.ToString();
            ReadThread = new Thread(ReadLoop);
            ReadThread.Name = listenState.process.ea[bp.eak.Name] + " Read port " + sourcePort;
            ReadThread.Start();
        }

        /*
         * ReceiveLoop
         * This function reads in an infinit loop the data from the channel
         * Each time a data is read the method ReceiveHandling is called
         * The infinit loop ends when the member terminateFlag is set to true by
         * ReceiveHandling
         */

        /**********************************************************************************************//**
         * Reads the loop.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public void ReadLoop()
        {
            try
            {
                Socket handler = state.workSocket;
                handler.NoDelay = true;
                while (true)
                {
                    //The Receive function of the socket waits untill data is received
                    int bytesReceived = handler.Receive(state.buffer, 0, SocketStateObject.BufferSize - 1, SocketFlags.None);
                    ReceiveHandling(bytesReceived);

                    //Check the terminateFlag if true close the socket
                    if (terminateFlag)
                    {
                        state.workSocket.Shutdown(SocketShutdown.Both);
                        state.workSocket.Close();
                        return;
                    }
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError,
                    state.process.GetProcessDefaultName(),
                    "AsynchronousReader.ReeadLoop()",
                    "Error : " + e.Message, "ErrorCode : " + e.ErrorCode.ToString(), "Error");
            }
        }

        /*
         * ReceiveHandling
         * This method is responssible to devide the data received to packets and call 
         * the ReceiveHandling method of the process
         * Note that there no connection from the way the data is sent to the way it is
         * Received. That meens that a several packets can be received in one Receive and
         * There is no garentee that the last packet in the data will end.
         * The packets (messages) are send with '#' termination flag
         * The received data is collected in state.buffer
         * Then it is copied to state.sb . The role of this variable is to hold all the
         *      data that was not processed
         * Then it converted to string 
         * Then it is devided to packets
         * Then each packet except the last packet is handled
         * Then the last packet is checked :
         *      If the data received ends with '#' - do nothing because that meens that the last 
         *          packet is empty
         *      else clear the state.data and fill it with the unterminated last packet to be joined
         *          by the next receive data
         * The following is the handling of a complete packet
         * 1. Generate a message object from the message data
         * 2. If the MessageType attribute of the message is "Terminate" set the termnateFlag and return
         * 3. wait untill the ReceiveHandling method of the process is not locked by another
         *      AsynchronousReader object
         * 4. Activate the ReceiveHandling method of the process with the data.
         */

        /**********************************************************************************************//**
         * Receive handling.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   bytesReceived   The bytes received.
         
         **************************************************************************************************/

        public void ReceiveHandling(int bytesReceived)
        {
            String content = String.Empty;
            
            // There  might be more data, so store the data received so far.
            state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesReceived));
            
            //Convert the data to string
            content = state.sb.ToString();

            // Get the process
            BaseProcess process = state.process;

            //If any end of packets was received
            if (content.IndexOf("#") > -1)
            {
                //Devide the data to packets
                string[] packets = Regex.Split(content, "#");

                //Packet handling
                for (int idx = 0; idx < packets.Length - 1; idx++)
                {
                    //Create message from the packet
                    //BaseMessage message = new BaseMessage(packets[idx]);
                    string processName = "Process_" + process.ea[ne.eak.Id].ToString();
                    Logger.Log(Logger.LogMode.MainLogAndProcessLog, processName, "Receive", "Received message - packets[idx]", packets[idx], "ProcessReceive");
                    BaseMessage message = BaseMessage.CreateMessage(process.Network, packets[idx]);
                    

                    if (TypesUtility.CompareDynamics(message.GetHeaderField(bm.pak.MessageType),bm.MessageTypes.Terminate))
                    {
                        terminateFlag = true;
                    }
                    process.MessageQHandling(ref message, MessageQOperation.AddMessage);
                    MessageRouter.ReportMessageReceive(process, new object[] { message });
                    //Logger.Log(Logger.LogMode.MainLogAndProcessLog, process.ToString(), "Receive", "Received message", message);
                }

                //If there is a unfinished packet
                if (content[content.Length - 1] != '#')
                {
                    //This is the last packet received and it is fregmented with the next package
                    state.sb.Clear();
                    state.sb.Append(packets[packets.Length - 1]);
                }
                else
                {
                    state.sb.Clear();
                }
            }
        }

    }
}
