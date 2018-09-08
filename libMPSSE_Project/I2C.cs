using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    public class I2C {
        IntPtr ftHandle;
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
        public static extern FT_STATUS I2C_OpenChannel(uint index, ref IntPtr handler);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_InitChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_InitChannel(IntPtr handler, ref ChannelConfig config);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_CloseChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_CloseChannel(IntPtr handler);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceRead", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_DeviceRead(IntPtr handler, uint deviceAddress, uint sizeToTransfer,
            byte[] buffer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_DeviceWrite(IntPtr handler, uint deviceAddress, uint sizeToTransfer,
            byte[] buffer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Init_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init_libMPSSE();

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Cleanup_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup_libMPSSE();

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_WriteGPIO", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS FT_WriteGPIO(IntPtr handler, byte dir, byte value);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_ReadGPIO", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS FT_ReadGPIO(IntPtr handler, ref byte value);

        public FT_STATUS Init(uint channel) {
            FT_STATUS status = FT_STATUS.FT_OK;
            ChannelConfig channelConfig;
            channelConfig.ClockRate = I2C_CLOCKRATE.I2C_CLOCK_STANDARD_MODE;
            channelConfig.LatencyTimer = 255;
            channelConfig.Options = 3;

            Init_libMPSSE();

            status |= I2C_OpenChannel(channel, ref ftHandle);
            status |= I2C_InitChannel(ftHandle, ref channelConfig);
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

            status |= I2C_DeviceWrite(ftHandle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                I2C_TRANSFER_OPTIONS_START_BIT |
                I2C_TRANSFER_OPTIONS_STOP_BIT);

            while (!writeCompleted && retry < 10) {
                bytesToTransfer = 0;
                bytesTransfered = 0;
                buffer = new byte[3];

                buffer[bytesToTransfer++] = (byte)deviceAddress;
                buffer[bytesToTransfer++] = (byte)(deviceAddress >> 8);
                buffer[bytesToTransfer++] = data;

                status |= I2C_DeviceWrite(ftHandle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                    I2C_TRANSFER_OPTIONS_START_BIT |
                    I2C_TRANSFER_OPTIONS_BREAK_ON_NACK);

                if (status == FT_STATUS.FT_OK & bytesTransfered == bytesToTransfer) {
                    writeCompleted = true;
                }
                retry++;
            }
            return status;
        }

        public FT_STATUS ReadByte(byte slaveAddress, uint deviceAddress, ref byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            uint bytesToTransfer = 0;
            uint bytesTransfered = 0;

            byte[] buffer = new byte[2];

            buffer[bytesToTransfer++] = (byte)deviceAddress;
            buffer[bytesToTransfer++] = (byte)(deviceAddress >> 8);

            status = I2C_DeviceWrite(ftHandle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                I2C_TRANSFER_OPTIONS_START_BIT);

            bytesToTransfer = 1;
            bytesTransfered = 0;
            buffer = new byte[1];
            status |= I2C_DeviceRead(ftHandle, slaveAddress, bytesToTransfer, buffer, ref bytesTransfered,
                I2C_TRANSFER_OPTIONS_START_BIT);
            data = buffer[0];
            return status;
        }

        public FT_STATUS Close() {
            FT_STATUS status = FT_STATUS.FT_OK;
            if (ftHandle != IntPtr.Zero) {
                status = I2C_CloseChannel(ftHandle);
            }
            Cleanup_libMPSSE();
            return status;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ChannelConfig {
            public I2C_CLOCKRATE ClockRate;
            public uint LatencyTimer;
            public uint Options;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FT_DEVICE_LIST_INFO_NODE {
            uint Flags;
            uint Type;
            uint ID;
            uint LocId;
            char[] SerialNumber;
            char[] Description;
            IntPtr ftHandle;
        }

        public enum I2C_CLOCKRATE : uint {
            I2C_CLOCK_STANDARD_MODE = 100000,                           // 100kb/sec
            I2C_CLOCK_FAST_MODE = 400000,                               // 400kb/sec
            I2C_CLOCK_FAST_MODE_PLUS = 1000000,                         // 1000kb/sec
            I2C_CLOCK_HIGH_SPEED_MODE = 3400000 					    // 3.4Mb/sec
        }

        public enum FT_STATUS : uint {
            FT_OK = 0,
            FT_INVALID_HANDLE = 1,
            FT_DEVICE_NOT_FOUND = 2,
            FT_DEVICE_NOT_OPENED = 3,
            FT_IO_ERROR = 4,
            FT_INSUFFICIENT_RESOURCES = 5,
            FT_INVALID_PARAMETER = 6,
            FT_INVALID_BAUD_RATE = 7,
            FT_DEVICE_NOT_OPENED_FOR_ERASE = 8,
            FT_DEVICE_NOT_OPENED_FOR_WRITE = 9,
            FT_FAILED_TO_WRITE_DEVICE = 10,
            FT_EEPROM_READ_FAILED = 11,
            FT_EEPROM_WRITE_FAILED = 12,
            FT_EEPROM_ERASE_FAILED = 13,
            FT_EEPROM_NOT_PRESENT = 14,
            FT_EEPROM_NOT_PROGRAMMED = 15,
            FT_INVALID_ARGS = 16,
            FT_NOT_SUPPORTED = 17,
            FT_OTHER_ERROR = 18

        }
    }
}
