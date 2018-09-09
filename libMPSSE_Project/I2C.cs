using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    public class I2C {
        IntPtr handle = IntPtr.Zero;
        private const uint I2C_READ_WRITE_COMPLETION_RETRY = 10;

        private const uint I2C_TRANSFER_OPTIONS_START_BIT = 0x01;           //00000001
        private const uint I2C_TRANSFER_OPTIONS_STOP_BIT = 0x02;            //00000010
        private const uint I2C_TRANSFER_OPTIONS_BREAK_ON_NACK = 0x03;       //00000100
        private const uint I2C_TRANSFER_OPTIONS_NACK_LAST_BYTE = 0x08;      //00001000
        private const uint I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES = 0x10; //00010000
        private const uint I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS = 0x20;  //00100000
        private const uint I2C_TRANSFER_OPTIONS_NO_ADDRESS = 0x40;          //01000000

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_GetNumChannels", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_GetNumChannels(ref uint NumChannels);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_GetChannelInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_GetChannelInfo(uint index, ref FT_DEVICE_LIST_INFO_NODE chanInfo);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_OpenChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_OpenChannel(uint index, ref IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_InitChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_InitChannel(IntPtr handler, ref ChannelConfig config);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_CloseChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_CloseChannel(IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceRead", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_DeviceRead(IntPtr handle, uint deviceAddress, uint sizeToTransfer,
            byte[] buffer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_DeviceWrite(IntPtr handle, uint deviceAddress, uint sizeToTransfer,
            byte[] buffer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Init_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init_libMPSSE();

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Cleanup_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup_libMPSSE();

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_WriteGPIO", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS FT_WriteGPIO(IntPtr handle, byte dir, byte value);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_ReadGPIO", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS FT_ReadGPIO(IntPtr handle, ref byte value);

        public FT_STATUS Init(uint channel) {
            FT_STATUS status = FT_STATUS.FT_OK;
            ChannelConfig channelConfig= new ChannelConfig();
            channelConfig.ClockRate = I2C_CLOCKRATE.I2C_CLOCK_STANDARD_MODE;
            channelConfig.LatencyTimer = 255;
            channelConfig.Options = 3;

            Init_libMPSSE();

            status |= I2C_OpenChannel(channel, ref handle);
            status |= I2C_InitChannel(handle, ref channelConfig);
            return status;
        }

        public FT_STATUS WriteByte(byte slaveAddress, uint deviceAddress, byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            uint bytesToTransfer = 0;
            uint bytesTransfered = 0;
            uint retry = 0;
            bool writeCompleted = false;

            byte[] buffer = new byte[3];

            buffer[bytesToTransfer++] = (byte)deviceAddress;
            buffer[bytesToTransfer++] = (byte)(deviceAddress >> 8);
            buffer[bytesToTransfer++] = data;

            status |= I2C_DeviceWrite(handle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                I2C_TRANSFER_OPTIONS_START_BIT |
                I2C_TRANSFER_OPTIONS_STOP_BIT);

            APP_CHECK_STATUS(status);

            while (!writeCompleted && retry < I2C_READ_WRITE_COMPLETION_RETRY) {
                bytesToTransfer = 0;
                bytesTransfered = 0;
                buffer = new byte[3];

                buffer[bytesToTransfer++] = (byte)deviceAddress;
                buffer[bytesToTransfer++] = (byte)(deviceAddress >> 8);
                buffer[bytesToTransfer++] = data;

                status |= I2C_DeviceWrite(handle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                    I2C_TRANSFER_OPTIONS_START_BIT |
                    I2C_TRANSFER_OPTIONS_NACK_LAST_BYTE);

                if (status == FT_STATUS.FT_OK & bytesTransfered == bytesToTransfer) {
                    writeCompleted = true;
                }
                retry++;
            }
            APP_CHECK_STATUS(status);

            return status;
        }

        public FT_STATUS ReadByte(byte slaveAddress, uint deviceAddress, ref byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            uint bytesToTransfer = 0;
            uint bytesTransfered = 0;
            byte[] buffer = new byte[2];

            buffer[bytesToTransfer++] = (byte)deviceAddress;
            buffer[bytesToTransfer++] = (byte)(deviceAddress >> 8);

            status = I2C_DeviceWrite(handle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                I2C_TRANSFER_OPTIONS_START_BIT);

            APP_CHECK_STATUS(status);

            bytesToTransfer = 1;
            bytesTransfered = 0;
            buffer = new byte[1];
            status |= I2C_DeviceRead(handle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                I2C_TRANSFER_OPTIONS_START_BIT | I2C_TRANSFER_OPTIONS_NACK_LAST_BYTE);

            APP_CHECK_STATUS(status);

            data = buffer[0];
            return status;
        }

        public FT_STATUS Close() {
            FT_STATUS status = FT_STATUS.FT_OK;
            if (handle != IntPtr.Zero) {
                status = I2C_CloseChannel(handle);
            }
            Cleanup_libMPSSE();
            return status;
        }

        static void APP_CHECK_STATUS(FT_STATUS status) {
            if (status != FT_STATUS.FT_OK) {
                Console.WriteLine("Error: {0}", status.ToString());
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ChannelConfig {
            public I2C_CLOCKRATE ClockRate;
            public uint LatencyTimer;
            public uint Options;
        }
    }
}
