using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMPSSE_Project {
    class Program {
        static void Main(string[] args) {
            I2C ftdi = new I2C();
            I2C.FT_STATUS status = I2C.FT_STATUS.FT_OK;
            status |= ftdi.Init(0);

            //FTDII2C.FT_DEVICE_LIST_INFO_NODE listInfo = new FTDII2C.FT_DEVICE_LIST_INFO_NODE();

            //status |= FTDII2C.I2C_GetChannelInfo(0, ref listInfo);

            Console.WriteLine("writing...");

            for (int i = 0; i < 0x100; i++) {
                status |= ftdi.WriteByte(0x50, (uint)i, 0xFF);
            }

            Console.WriteLine("reading...");
            byte[] buffer = new byte[0x100];
            for (int i = 0; i < 0x100; i++) {
                byte data = 0;
                status |= ftdi.ReadByte(0x50, (uint)i, ref data);
                buffer[i] = data;
            }
            ftdi.Close();

            Console.WriteLine("Completed...");
            Console.ReadLine();
        }
    }
}
