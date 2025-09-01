using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighSpeedDX200
{
    class MotionController
    {
        private String ip;
        private int port;

        private SocketHandler socket;

        private RequestPackageHandler request_package_handler = new RequestPackageHandler();
        private AnswerPackageHandler anwser_package_handler = new AnswerPackageHandler();

        private int[] bp = new int[8] { 601791, -100478, 529232, 1730034, -152422, 114409, 0, 0};

        public MotionController(String ip, int port)
        {
            this.ip = ip;
            this.port = port;
            socket = new SocketHandler(ip, port);
            OpenConnectionToRobot();
        }

        public void MoveRobotX(int value, int speed)
        {
            MoveRobot(value, 0, 0, 0, 0, 0, speed);
        }

        public void MoveRobotY(int value, int speed)
        {
            MoveRobot(0, value, 0, 0, 0, 0, speed); 
        }

        public void MoveRobotZ(int value, int speed)
        {
            MoveRobot(0, 0, value, 0, 0, 0, speed);
        }

        private int MoveRobot(int x, int y, int z, int rx, int ry, int rz, int speed)
        {
            if (!socket.IsConnected())
            {
                return 0;
            }
            byte[] package_send = request_package_handler.CreateMoveDistancePackage(x, y, z, rx, ry, rz, speed);
            try
            {
                socket.Send(package_send);
            } catch(Exception e)
            {
                return 0;
            }
            return 1;
        }



        private void OpenConnectionToRobot()
        {
            socket.OpenSocket();
        }

        public void CloseConnectionToRobot()
        {
            socket.CloseSocket();
        }
    }
}
