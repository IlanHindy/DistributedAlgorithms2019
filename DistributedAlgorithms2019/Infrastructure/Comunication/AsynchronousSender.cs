////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\AsynchronousSender.cs
///
///\brief   Implements the asynchronous sender class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{

    /*
     * Class AsynchronousSender
     * This is a class which contains static methods to send one message
     * The channel holds the sending socket
     * The initialize value of the sending socket is null
     * If the value of the sending socket is null (That meens that this is
     *      the first sending in the channel) a new socket is created and 
     *      a connection to the receive process listening socket is made
     *      then the new socket is saved in the channel
     * If the value of the sending process is not null the sending socket is
     *      retrieved from the channel
     * Then a sending is done from the socket the termination character for the sending 
     *      message is '#'
     *      
     * The process of the connecting and the sending is as follows:
     *      1. (Only if this is the first sending from the socket) 
     *          The StartSending methods calls the BeginConnect method of the socket
     *      2. When the socket created the connection it calls the ConnectCallback method 
     *      3. The ConnectCallback method calls the EndConnect method of the socket
     *      4. After the connection done the StartSending method calls the Send method
     *      5. The Send method calls the BeginSend method of the socket
     *      6. When the socket finished the sending it calls SendCallback method
     *      7. The SendCallback method calls the EndSend of the socket
     *      
     * All the data needed for the method operation is passed in an instance of SocketStateObject
     * Which is filled in StartSending and passed between the methods of this object and the
     * socket's object. The AsynchronousSender class does not contain member variables because
     * the methods are static and one instance serves all the processes
     * 
     */

    /**********************************************************************************************//**
     * The asynchronous sender.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class AsynchronousSender
    {
        /*
         * Start sending - the main method of the sending
         */

        /**********************************************************************************************//**
         * Starts a sending.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   message The message.
         * \param   process The process.
         * \param   channel The channel.
         
         **************************************************************************************************/

        public static void StartSending(BaseMessage message, BaseProcess process, BaseChannel channel)
        {
            try
            {

                // Establish the remote endpoint for the socket.
                string localAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(localAddress), channel.or[bc.ork.DestPort]);

                // Build the state object
                SocketStateObject state = new SocketStateObject();
                state.process = process;
 
                Socket client;

                //If this is the first time that there is a sending through the channel
                if (channel.sendingSocket == null)
                {
                    //Establish the local end point for the socket
                    int localPort = channel.or[bc.ork.SourcePort];
                    IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(localAddress), localPort);

                    //Create a new socket
                    client = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                    //Bind the local address
                    client.Bind(localEndPoint);

                    //Set the socket in the channel and in the state object
                    channel.sendingSocket = client;
                    state.clientSocket = client;


                    // Connect to the remote endpoint.
                    Logger.Log(Logger.LogMode.MainLogAndProcessLogAndMessageTrace, process.GetProcessDefaultName(), "AsynchronousSender.StartSending()", "before BeginConnect()", "", "ProcessSend");
                    client.BeginConnect(remoteEP,
                        new AsyncCallback(ConnectCallback), state);
                    state.connectDone.WaitOne();

                }
                else //This is not the first sending from the channel
                {
                    //Get the socket from the channel and set it in the state object
                    client = channel.sendingSocket;
                    state.clientSocket = client;

                }

                // Send test data to the remote device.
                MessageRouter.ReportMessageSent(process, new object[] { message });
                Send(message, state);
                state.sendDone.WaitOne();
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError, process.GetProcessDefaultName(), "AsynchronousSender.StartSending()", "Error", e.ToString(), "Error");
            }
        }

        /*
         * ConnectCallback
         * This function is called by the socket when it ended the connection
         */

        /**********************************************************************************************//**
         * Connects a callback.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   ar  The archive.
         *              
         **************************************************************************************************/

        private static void ConnectCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            SocketStateObject state = (SocketStateObject)ar.AsyncState;
            Socket client = state.clientSocket;
            BaseProcess process = state.process;

            try
            {
                // Complete the connection.
                client.EndConnect(ar);

                // Signal that the connection has been made.
                state.connectDone.Set();
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError, process.GetProcessDefaultName(), "AsynchronousSender.ConnectCallback()", e.ToString(), "Error");
            }
        }

        /*
         * Send
         * This function sends one packet
         */

        /**********************************************************************************************//**
         * Send this message.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   message The message.
         * \param   state   The state.
         
         **************************************************************************************************/

        private static void Send(BaseMessage message, SocketStateObject state)
        {
            // Retrieve the socket from the state object.
            Socket client = state.clientSocket;

            try
            {
                //Logging the send message
                Logger.Log(Logger.LogMode.MainLogAndProcessLogAndMessageTrace, state.process.GetProcessDefaultName(), "AsynchronousSender.Send()", "SEND", message, "");

                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(message.MessageString() + "#");

                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), state);
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError, state.process.GetProcessDefaultName(), "AsynchronousSender.Send()", e.ToString(), "Error");
            }

        }

        /*
         * SendCallback
         * This function is called by the socket when it ended the sending
         */

        /**********************************************************************************************//**
         * Sends a callback.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   ar  The archive.
         *              
         **************************************************************************************************/

        private static void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            SocketStateObject state = (SocketStateObject)ar.AsyncState;
            Socket client = state.clientSocket;
            BaseProcess process = state.process;
            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.
                state.sendDone.Set();
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError, process.GetProcessDefaultName(), "AsynchronousSender.SendCallback()", e.ToString(), "Error");
            }
        }
        /*
         * Check if the socket is still connected by sending a zero-length
         * message and checking the Connected attribute after the send
         */

        /**********************************************************************************************//**
         * Query if 'socket' is socket connected.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   socket  The socket.
         *
         * \return  true if socket connected, false if not.
         .
         **************************************************************************************************/

        public static bool IsSocketConnected(Socket socket)
        {
            bool blockingState = socket.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                socket.Blocking = false;
                socket.Send(tmp, 0, 0);
                return true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                socket.Blocking = blockingState;
            }

        }
    }
}