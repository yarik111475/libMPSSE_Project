using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    public partial class  MPSSE {
        
        private const uint I2C_READ_WRITE_COMPLETION_RETRY = 10;

        private const uint I2C_TRANSFER_OPTIONS_START_BIT = 0x01;           //00000001
        private const uint I2C_TRANSFER_OPTIONS_STOP_BIT = 0x02;            //00000010
        private const uint I2C_TRANSFER_OPTIONS_BREAK_ON_NACK = 0x03;       //00000100
        private const uint I2C_TRANSFER_OPTIONS_NACK_LAST_BYTE = 0x08;      //00001000
        private const uint I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES = 0x10; //00010000
        private const uint I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS = 0x20;  //00100000
        private const uint I2C_TRANSFER_OPTIONS_NO_ADDRESS = 0x40;          //01000000

        
        public FT_STATUS I2C_Init(uint channel) {
            FT_STATUS status = FT_STATUS.FT_OK;
            I2C_ChannelConfig channelConfig= new I2C_ChannelConfig();
            channelConfig.ClockRate = I2C_SPI_CLOCKRATE.I2C_CLOCK_STANDARD_MODE;
            channelConfig.LatencyTimer = 255;
            channelConfig.Options = 3;

            Init_libMPSSE();

            status |= I2C_OpenChannel(channel, ref handle);
            status |= I2C_InitChannel(handle, out channelConfig);
            return status;
        }

        public FT_STATUS I2C_WriteByte(byte slaveAddress, uint deviceAddress, byte data) {
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

        public FT_STATUS I2C_ReadByte(byte slaveAddress, uint deviceAddress, ref byte data) {
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

        public FT_STATUS I2C_Close() {
            FT_STATUS status = FT_STATUS.FT_OK;
            if (handle != IntPtr.Zero) {
                status = I2C_CloseChannel(handle);
            }
            Cleanup_libMPSSE();
            return status;
        }

        

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct I2C_ChannelConfig {
            public libMPSSE_Project.I2C_SPI_CLOCKRATE ClockRate;
            public uint LatencyTimer;
            public uint Options;
        }
    }
}
