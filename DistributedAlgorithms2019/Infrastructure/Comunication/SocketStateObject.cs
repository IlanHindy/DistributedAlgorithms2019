////////////////////////////////////////////////////////////////////////////////////////////////////
///\file    Infrastructure\SocketStateObject.cs
///
///\brief   Implements the socket state object class.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DistributedAlgorithms.Algorithms.Base.Base;

namespace DistributedAlgorithms
{
    /**********************************************************************************************//**
     * A socket state object.
     *
     * \author  Ilan Hindy
     * \date    29/09/2016
     
     **************************************************************************************************/

    public class SocketStateObject
    {
        /**********************************************************************************************//**
         * Constructors.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         
         **************************************************************************************************/

        public SocketStateObject() { }

        /**********************************************************************************************//**
         * Constructor.
         *
         * \author  Ilan Hindy
         * \date    29/09/2016
         *
         * \param   state   The state.
         
         **************************************************************************************************/

        public SocketStateObject(SocketStateObject state)
        {
            workSocket = state.workSocket;
            listenSocket = state.listenSocket;
            process = state.process;
            clientSocket = state.clientSocket;
            connectDone = state.connectDone;
            sendDone = state.sendDone;
            allDone = state.allDone;
            sb = new StringBuilder();
            buffer = new byte[BufferSize];
            connectDone = new ManualResetEvent(false);
            sendDone = new ManualResetEvent(false);
            allDone = new ManualResetEvent(false);
        }
        /** State object for receiving data from remote device. Client socket. */
        public Socket workSocket = null;
        /** Size of receive buffer. */
        public const int BufferSize = 256;
        /** Receive buffer. */
        public byte[] buffer = new byte[BufferSize];
        /** Received data string. */
        public StringBuilder sb = new StringBuilder();
        /** Listenning socket (Used in listening object) */
        public Socket listenSocket = null;
        /** Calling process. */
        public BaseProcess process = null;
        /** client socket (Used in sender process) */
        public Socket clientSocket = null;
        /** Send process events. */
        public ManualResetEvent connectDone = new ManualResetEvent(false);
        /** The send done. */
        public ManualResetEvent sendDone = new ManualResetEvent(false);
        /** Receive process events. */
        public ManualResetEvent allDone = new ManualResetEvent(false);

    }
}
