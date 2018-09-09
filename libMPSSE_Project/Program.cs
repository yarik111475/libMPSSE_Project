using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace libMPSSE_Project {
    class Program {
        static void Main(string[] args) {
            FT_STATUS status = FT_STATUS.FT_OK;
            MPSSE mpsse = new MPSSE();
            //status |= mpsse.SPI_Init(0);
            //status |= mpsse.SPI_Close();
          
            status |= mpsse.I2C_Init(0);

            FT_DEVICE_LIST_INFO_NODE listInfo;
            status |= MPSSE.I2C_GetChannelInfo(0, out listInfo);
            Console.WriteLine("Description: {0}", listInfo.Description);
            Console.WriteLine("SerialNumber: {0}", listInfo.SerialNumber);

            byte[] inBuffer = new byte[0x100];// File.ReadAllBytes(@"C:\BWD 129 (ALL)_283321400510_6401.bin");
            //Console.WriteLine("writing...");

            //for (int i = 0; i < inBuffer.Length; i++) {
            //    status |= mpsse.WriteByte(0x50, (uint)i, 0xAC);
            //}

            Console.WriteLine("reading...");
            byte[] outBuffer = new byte[inBuffer.Length];
            for (int i = 0; i < outBuffer.Length; i++) {
                byte data = 0;
                status |= mpsse.I2C_ReadByte(0x50, (uint)i, ref data);
                outBuffer[i] = data;
            }
            mpsse.I2C_Close();


            Console.WriteLine("Completed...");
            Console.ReadLine();
        }
    }
}
