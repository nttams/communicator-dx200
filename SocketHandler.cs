//NOT CATCH ALL EXCEPTION YET.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace HighSpeedDX200
{
    class SocketHandler
    {
        private Socket socketClient;
        private IPEndPoint ipEndPoint;

        private String ip;
        private int port;


        public SocketHandler(String ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public int OpenSocket()
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socketClient = new Socket(ipEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socketClient.ReceiveTimeout = 100;
            socketClient.SendTimeout = 100;
            socketClient.Connect(ipEndPoint);

            if (socketClient.Connected)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int Send(byte[] payload_send)
        {
            try
            {
                socketClient.Send(payload_send);
                return 1;
            } catch(Exception e)
            {
                return 0;
            }
        }

        public bool IsConnected()
        {
            return socketClient.Connected;
        }

        public byte[] Receive()
        {
            byte[] payload_recv = new byte[520]; //HEADER SIZE = 32bytes. DATA SIZE MAXIMUM = 479bytes. Total
            try
            {
                socketClient.Receive(payload_recv);
            }
            catch (Exception e)
            {
                return new byte[1] { 0 };
            }

            byte[] result = new byte[payload_recv[7] * 256 + payload_recv[6] + 32];
            Array.Copy(payload_recv, 0, result, 0, result.Length);
            return result;
        }

        public void CloseSocket()
        {
            socketClient.Close();
        }
    }
}

