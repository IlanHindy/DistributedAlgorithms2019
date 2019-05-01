////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\AsynchronousListener.cs
///
///\brief   Implements the asynchronous listener class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /*
     * Class AsynchronousListener
     * Listen to a receive socket for new connections and creates an AsynchronousReader
     * for each new connection
     * The main function of the class listen to a socket in an infinit loop and when a new
     * connection is done it calls AddAsynchronoueReader of the Process for creating an AsynchronousReader
     * for receiving the messages
     * By this a receive socket is created for each send socket that was connected to 
     * this listener
     * The infinit loop is terminated when the Process has set it's StopFlag to true
     * In order to set this flag the process sends a message to itself after setting the StopFlag
     * All the methods in this class are stetic and there for no member variables are found
     * All the data is passed using an instance of SocketStateObject
     * The following is the process of the listening
     * 1. In StartListening the socket is created and the socket's BeginAccept method is called
     * 2. After the socket received a connection request and made the connection it calles AcceptCallbeck
     * 3. The acceptCallback method calls the Process's method to create a reader that will be connected
     *    to the socket that requested the connection
     */

    /**********************************************************************************************//**
     * The asynchronous listener.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class AsynchronousListener
    {
        /**********************************************************************************************//**
         * Default constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public AsynchronousListener()
        {
        }

        /*
         * StartListening
         * This method listen to new connections n an infinit loop
         * The loop terminates when the StopFlag of the process is set to true
         * There is one listener for each process
         */

        /**********************************************************************************************//**
         * Starts a listening.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   localPort   The local port.
         * \param   process     The process.
         *                      
         **************************************************************************************************/

        public static void StartListening(int localPort, BaseProcess process)
        {
            // Establish the local endpoint for the socket.
            string localAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(localAddress), localPort);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Bind the socket to the local endpoint and listen for incoming connections.
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Create state
                    SocketStateObject state = new SocketStateObject();
                    state.process = process;
                    state.listenSocket = listener;

                    // Set the event to nonsignaled state.
                    state.allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        state);

                    //Wait untill the connection is done
                    state.allDone.WaitOne();

                    // Check if the process ended it's algorithm
                    Logger.Log(Logger.LogMode.MainLogAndProcessLog, process.GetProcessDefaultName(), "AsynchronousSocketListener.StartListening()", "StopFlag = " + process.StopFlag.ToString(), "", "ProcessReceive");
                    if (process.StopFlag == true)
                    {
                        Thread.Sleep(100);
                        Logger.Log(Logger.LogMode.MainLogAndProcessLog, process.GetProcessDefaultName(), "AsynchronousSocketListener.StartListening()", "listen thread terminated StopFlag = " + process.StopFlag.ToString(), "", "ProcessReceive");

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError, process.GetProcessDefaultName(), "AsynchronousSocketListener.StartListening()", e.ToString(), "", "Error");
            }
        }

        /*
         * AcceptCallback
         * This function is called by the socket when the connection was established
         */

        /**********************************************************************************************//**
         * Callback, called when the accept.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   ar  The archive.
         *              
         **************************************************************************************************/

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request (The receive socket).
            SocketStateObject state = (SocketStateObject)ar.AsyncState;
            Socket listener = state.listenSocket;
            Socket handler = listener.EndAccept(ar);
            try
            {
                state.workSocket = handler;

                //Create asynchronouse reader by the process
                state.process.AddAsynchronousReader(state);

                // Signal the main thread to continue.
                state.allDone.Set();

            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogMode.MainLogProcessLogAndError, state.process.GetProcessDefaultName(), "AsynchronousSocketListener.AcceptCallback()", e.ToString(), "Error");
            }

        }
    }
}