//NOT CATCH ALL EXCEPTION YET.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighSpeedDX200
{
    class RequestPackageHandler
    {
        //FIXED
        private byte[] HEADER = new byte[4] { 0x59, 0x45, 0x52, 0x43 };
        private byte[] HEADER_SIZE = new byte[2] { 0x20, 0x00 };
        private byte[] RESERVE1 = new byte[1] { 0x03 };
        private byte[] BLOCK_NO = new byte[4] { 0x00, 0x00, 0x00, 0x00 }; //0x00000000 fixed when request.
        private byte[] REVERSE2 = new byte[8] { 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39 };
        private byte[] PADDING = new byte[2] { 0x00, 0x00 };

        private const byte PROCESSING_DIVISION_ROBOT_CONTROL = 1;
        private const byte PROCESSING_DIVISION_FILE_CONTROL = 2;

        private const ushort GET_PLURAL_VAR_P_PACKAGE = 0x307;
        private const ushort GET_PLURAL_VAR_D_PACKAGE = 0x304;

        private const ushort GET_SINGLE_VAR_P_PACKAGE = 0x7F;
        private const ushort GET_SINGLE_VAR_D_PACKAGE = 0x7D;

        //VARIABLE

        private byte[] data_size = new byte[2];
        private byte[] processing_division = new byte[1];
        private byte[] ack = new byte[1];
        private byte[] request_id = new byte[1];

        private byte[] command_no = new byte[2];
        private byte[] instance = new byte[2];
        private byte[] attribute = new byte[1];
        private byte[] service = new byte[1];

        private byte[] CreateHeaderSendPackage(
            ushort data_size,
            byte processing_division,
            byte ack,
            byte request_id,
            ushort command_no,
            ushort instance,
            byte attribute,
            byte service
            )
        {

            this.data_size = BitConverter.GetBytes(data_size);
            this.processing_division[0] = processing_division;
            this.ack[0] = ack;
            this.request_id[0] = request_id;
            this.command_no = BitConverter.GetBytes(command_no);
            this.instance = BitConverter.GetBytes(instance);
            this.attribute[0] = attribute;
            this.service[0] = service;

            return HEADER
            .Concat(HEADER_SIZE)
            .Concat(this.data_size)
            .Concat(RESERVE1)
            .Concat(this.processing_division)
            .Concat(this.ack)
            .Concat(this.request_id)
            .Concat(BLOCK_NO)
            .Concat(REVERSE2)
            .Concat(this.command_no)
            .Concat(this.instance)
            .Concat(this.attribute)
            .Concat(this.service)
            .Concat(PADDING)
            .ToArray();
        }

        //TESTED
        public byte[] CreateGetVarDValuePackage(ushort index)
        {
            return CreateHeaderSendPackage(0, 1, 0, 0, 0x7C, index, 0x01, 0x0E);
        }

        //TESTED
        public byte[] CreateGetPluralVarDPackage(ushort index, int length)
        {
            byte[] data = BitConverter.GetBytes(length);
            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x304, index, 0x00, 0x33);

            return header.Concat(data).ToArray();
        }

        //TESTED
        public byte[] CreateGetPluralVarPPackage(ushort index, int length)
        {
            byte[] data = BitConverter.GetBytes(length);
            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x307, index, 0x00, 0x33);

            return header.Concat(data).ToArray();
        }

        //TESTED
        public byte[] CreateGetVarPValuePackage(ushort index)
        {
            return CreateHeaderSendPackage(0, 1, 0, 0, GET_SINGLE_VAR_P_PACKAGE, index, 0x00 /*robot coordinate*/, 0x01 /*Read Var P*/);
        }

        //TESTED
        public byte[] CreateSetVarDValuePackage(ushort index, int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x7C, index, 0x01, 0x10);

            return header.Concat(data).ToArray();
        }

        public byte[] CreateGetCurrentPositionValuePackage()
        {
            //return createHeaderSendPackage(0, 1, 0, 0, 0x75, index, 0x00, 0x01);
            return CreateHeaderSendPackage(0, 1, 0, 0, 0x75, 0x65, 0x00, 0x01);
        }

        //TESTED
        public byte[] CreateSetVarPValuePackage(ushort index, int x, int y, int z, int rx, int ry, int rz, int ex1, int ex2)
        {
            byte[] data_type = BitConverter.GetBytes(0x11); //Robot coordinate
            byte[] form = BitConverter.GetBytes(0);
            byte[] tool_number = BitConverter.GetBytes(0);
            byte[] user_coordinate_number = BitConverter.GetBytes(0);
            byte[] extended_form = BitConverter.GetBytes(0);
            byte[] x_bytes = BitConverter.GetBytes(x);
            byte[] y_bytes = BitConverter.GetBytes(y);
            byte[] z_bytes = BitConverter.GetBytes(z);
            byte[] rx_bytes = BitConverter.GetBytes(rx);
            byte[] ry_bytes = BitConverter.GetBytes(ry);
            byte[] rz_bytes = BitConverter.GetBytes(rz);
            byte[] ex1_bytes = BitConverter.GetBytes(ex1);
            byte[] ex2_bytes = BitConverter.GetBytes(ex2);

            byte[] data = data_type
            .Concat(form)
            .Concat(tool_number)
            .Concat(user_coordinate_number)
            .Concat(extended_form)
            .Concat(x_bytes)
            .Concat(y_bytes)
            .Concat(z_bytes)
            .Concat(rx_bytes)
            .Concat(ry_bytes)
            .Concat(rz_bytes)
            .Concat(ex1_bytes)
            .Concat(ex2_bytes)
            .ToArray();

            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x7F, index, 0x00 /*robot coordinate*/, 0x02 /*Write Var P*/);
            return header.Concat(data).ToArray();
        }

        //TESTED
        public byte[] CreateServoOnPackage()
        {
            byte[] data = new byte[] { 1 /*servo ON*/, 0, 0, 0 };
            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x83, 0x02, 0x01, 0x10);

            return header.Concat(data).ToArray();
        }

        //TESTED
        public byte[] CreateServoOffPackage()
        {
            byte[] data = new byte[4] { 2 /*servo OFF*/, 0, 0, 0 };
            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x83, 0x02, 0x01, 0x10);

            return header.Concat(data).ToArray();
        }

        //UNTESTED
        public byte[] CreateGetTorqueDataPackage()
        {
            return CreateHeaderSendPackage(0, 1, 0, 0, 0x77, 0x01, 0x00, 0x0E); ;
        }

        public byte[] CreateMoveToPositionPackage(int x, int y, int z, int rx, int ry, int rz, int speed)
        {
            byte[] RobotNo = BitConverter.GetBytes(1); //Robot No.
            byte[] StationNo = BitConverter.GetBytes(0); //Station No.
            byte[] cartesian_operation = BitConverter.GetBytes(1);
            byte[] speed_bytes = BitConverter.GetBytes(speed);// speed 500mm/s
            byte[] coordinate_operation = BitConverter.GetBytes(17);
            byte[] x_bytes = BitConverter.GetBytes(x);
            byte[] y_bytes = BitConverter.GetBytes(y);
            byte[] z_bytes = BitConverter.GetBytes(z);
            byte[] rx_bytes = BitConverter.GetBytes(rx);
            byte[] ry_bytes = BitConverter.GetBytes(ry);
            byte[] rz_bytes = BitConverter.GetBytes(rz);
            byte[] reservation1 = BitConverter.GetBytes(0); //reservation1
            byte[] reservation2 = BitConverter.GetBytes(0); //reservation1
            byte[] type = BitConverter.GetBytes(0x04); // type
            byte[] expanded_type = BitConverter.GetBytes(0);
            byte[] tool_no = BitConverter.GetBytes(0);
            byte[] user_coordinate = BitConverter.GetBytes(0); //user coordinate
            byte[] base_1 = BitConverter.GetBytes(0); //base 1st axis position
            byte[] base_2 = BitConverter.GetBytes(0); //base 2nd axis position
            byte[] base_3 = BitConverter.GetBytes(0); //base 3rd axis position
            byte[] station_1 = BitConverter.GetBytes(0); //station 1st axis position
            byte[] station_2 = BitConverter.GetBytes(0); //station 2nd axis position
            byte[] station_3 = BitConverter.GetBytes(0); //station 3rd axis position
            byte[] station_4 = BitConverter.GetBytes(0); //station 4th axis position
            byte[] station_5 = BitConverter.GetBytes(0); //station 5th axis position
            byte[] station_6 = BitConverter.GetBytes(0); //station 6th axis position

            byte[] data = RobotNo
            .Concat(StationNo)
            .Concat(cartesian_operation)
            .Concat(speed_bytes)
            .Concat(coordinate_operation)
            .Concat(x_bytes)
            .Concat(y_bytes)
            .Concat(z_bytes)
            .Concat(rx_bytes)
            .Concat(ry_bytes)
            .Concat(rz_bytes)
            .Concat(reservation1)
            .Concat(reservation2)
            .Concat(type)
            .Concat(expanded_type)
            .Concat(tool_no)
            .Concat(user_coordinate)
            .Concat(base_1)
            .Concat(base_2)
            .Concat(base_3)
            .Concat(station_1)
            .Concat(station_2)
            .Concat(station_3)
            .Concat(station_4)
            .Concat(station_5)
            .Concat(station_6)
            .ToArray();

            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x8A, 0x02, 0x01 /*attribute*/, 0x02 /*service */);
            return header.Concat(data).ToArray();
        }

        public byte[] CreateMoveDistancePackage(int x, int y, int z, int rx, int ry, int rz, int speed)
        {
            byte[] RobotNo = BitConverter.GetBytes(1); //Robot No.
            byte[] StationNo = BitConverter.GetBytes(0); //Station No.
            byte[] cartesian_operation = BitConverter.GetBytes(1);
            byte[] speed_bytes = BitConverter.GetBytes(speed);// speed 500mm/s
            byte[] coordinate_operation = BitConverter.GetBytes(17);
            byte[] x_bytes = BitConverter.GetBytes(x);
            byte[] y_bytes = BitConverter.GetBytes(y);
            byte[] z_bytes = BitConverter.GetBytes(z);
            byte[] rx_bytes = BitConverter.GetBytes(rx);
            byte[] ry_bytes = BitConverter.GetBytes(ry);
            byte[] rz_bytes = BitConverter.GetBytes(rz);
            byte[] reservation1 = BitConverter.GetBytes(0); //reservation1
            byte[] reservation2 = BitConverter.GetBytes(0); //reservation1
            byte[] type = BitConverter.GetBytes(0x04); // type
            byte[] expanded_type = BitConverter.GetBytes(0);
            byte[] tool_no = BitConverter.GetBytes(0);
            byte[] user_coordinate = BitConverter.GetBytes(0); //user coordinate
            byte[] base_1 = BitConverter.GetBytes(0); //base 1st axis position
            byte[] base_2 = BitConverter.GetBytes(0); //base 2nd axis position
            byte[] base_3 = BitConverter.GetBytes(0); //base 3rd axis position
            byte[] station_1 = BitConverter.GetBytes(0); //station 1st axis position
            byte[] station_2 = BitConverter.GetBytes(0); //station 2nd axis position
            byte[] station_3 = BitConverter.GetBytes(0); //station 3rd axis position
            byte[] station_4 = BitConverter.GetBytes(0); //station 4th axis position
            byte[] station_5 = BitConverter.GetBytes(0); //station 5th axis position
            byte[] station_6 = BitConverter.GetBytes(0); //station 6th axis position

            byte[] data = RobotNo
            .Concat(StationNo)
            .Concat(cartesian_operation)
            .Concat(speed_bytes)
            .Concat(coordinate_operation)
            .Concat(x_bytes)
            .Concat(y_bytes)
            .Concat(z_bytes)
            .Concat(rx_bytes)
            .Concat(ry_bytes)
            .Concat(rz_bytes)
            .Concat(reservation1)
            .Concat(reservation2)
            .Concat(type)
            .Concat(expanded_type)
            .Concat(tool_no)
            .Concat(user_coordinate)
            .Concat(base_1)
            .Concat(base_2)
            .Concat(base_3)
            .Concat(station_1)
            .Concat(station_2)
            .Concat(station_3)
            .Concat(station_4)
            .Concat(station_5)
            .Concat(station_6)
            .ToArray();

            byte[] header = CreateHeaderSendPackage((ushort)data.Length, 1, 0, 0, 0x8A, 0x03, 0x01, 0x02);
            return header.Concat(data).ToArray();
        }

        public String ConvertByteArrayToString(byte[] bytes)
        {
            String result = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                result += bytes[i].ToString("X2");
                if ((i + 1) % 4 == 0)
                {
                    result += Environment.NewLine;
                }
            }
            return result;
        }
    }
}
/*
-----REQUEST PACKAGE-----

//HEADER PART
0x59, 0x45, 0x52, 0x43, //header FIXED
0x20, 0x00, //header size FIXED
0x04, 0x00, //data size
0x03, //reserve 1 FIXED
0x01, //processing division. 0x01 robot control; 0x02 file control
0x00, //ACK
0x00, //Request ID
0x00, 0x00, 0x00, 0x00, //block No. //FIXED 0x00000000 when REQUEST
0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, //reverse2 FIXED "99999999"
0x7C, 0x00, //command No
0x00, 0x00, // Instance
0x01, //atribute
0x10, //service
0x00, 0x00, //padding
//DATA PART (479byte at maximum)
...
*/

/*
-----ANSWER PACKAGE-----
//HEADER PART
0x59, 0x45, 0x52, 0x43, //header FIXED
0x20, 0x00, //header size FIXED
0x04, 0x00, //data size
0x03, //reserve1 FIXED
0x01, //processing division. 0x01 robot control; 0x02 file control
0x00, //ACK
0x00, //Request ID
0x00, 0x00, 0x00, 0x00, //block No.
0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, 0x39, //reverse2 FIXED "99999999"
0x7C, //service (only when relying)
0x00, //status (0x00: NORMAL; otherwise error);
0x00, //added status size
0x00, //padding
0x01, 0x10, //added status size
0x00, 0x00, //padding
DATA PART (479byte at maximum)
...
*/
