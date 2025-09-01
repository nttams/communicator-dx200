using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighSpeedDX200
{
    class AnswerPackageHandler
    {
        public int ParseGetVarDValuePackage(byte[] package)
        {
            return Convert4BytesToInt32(package[32], package[33], package[34], package[35]);
        }

        public int[] ParseGetPluralVarDValuePackage(byte[] package) 
        {
            int[] result = new int[Convert4BytesToInt32(package[32], package[33], package[34], package[35])];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert4BytesToInt32(package[36 + 4 * i], package[37 + 4 * i], package[38 + 4 * i], package[39 + 4 * i]);
            }
            return result;
        }

        public int[] ParseGetVarPValuePackage(byte[] package)
        {
            int[] result = new int[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = Convert4BytesToInt32(package[52 + i * 4], package[53 + i * 4], package[54 + i * 4], package[55 + i * 4]);
            }
            return result;
        }

        public int[] ParseGetCurrentPValuePackage(byte[] package)
        {
            int[] result = new int[6];
            for (int i = 0; i < 6; i++)
            {
                result[i] = Convert4BytesToInt32(package[52 + i * 4], package[53 + i * 4], package[54 + i * 4], package[55 + i * 4]);
            }
            return result;
        }

        public int[,] ParseGetPluralVarPPackage(byte[] package)
        {
            int[,] result = new int[Convert4BytesToInt32(package[32], package[33], package[34], package[35]), 8];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                //result[i] = convert4BytesToInt32(package[52 + i * 4], package[53 + i * 4], package[54 + i * 4], package[55 + i * 4]);
                for (int j = 0; j < 8; j++)
                {
                    result[i, j] = Convert4BytesToInt32(package[56 + i * 52 + j * 4], package[57 + i * 52 + j * 4], package[58 + i * 52 + j * 4], package[59 + i * 52 + j * 4]);
                }
            }
            return result;
        }

        public int[] ParseGetTorqueDataPackage(byte[] package)
        {
            int[] result = new int[8];
            for (int i = 0; i < 6; i++)
            {
                result[i] = Convert4BytesToInt32(package[32 + i * 4], package[33 + i * 4], package[34 + i * 4], package[35 + i * 4]);
            }
            return result;
        }

        private int Convert4BytesToInt32(byte a, byte b, byte c, byte d)
        {
            byte[] result = new byte[4];
            result[0] = a;
            result[1] = b;
            result[2] = c;
            result[3] = d;

            return BitConverter.ToInt32(result, 0);
        }
    }
}
