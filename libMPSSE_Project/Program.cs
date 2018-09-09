using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace libMPSSE_Project {
    class Program {
        static void Main(string[] args) {
            I2C i2c = new I2C();
            FT_STATUS status = FT_STATUS.FT_OK;
            status |= i2c.Init(0);

            //FT_DEVICE_LIST_INFO_NODE listInfo = new FT_DEVICE_LIST_INFO_NODE();
            //status |= I2C.I2C_GetChannelInfo(0, ref listInfo);

            byte[] inBuffer = new byte[0x100];// File.ReadAllBytes(@"C:\BWD 129 (ALL)_283321400510_6401.bin");
            //Console.WriteLine("writing...");

            //for (int i = 0; i < inBuffer.Length; i++) {
            //    status |= i2c.WriteByte(0x50, (uint)i, 0xAD);
            //}

            Console.WriteLine("reading...");
            byte[] outBuffer = new byte[inBuffer.Length];
            for (int i = 0; i < outBuffer.Length; i++) {
                byte data = 0;
                status |= i2c.ReadByte(0x50, (uint)i, ref data);
                outBuffer[i] = data;
            }
            i2c.Close();


            Console.WriteLine("Completed...");
            Console.ReadLine();
        }
    }
}
