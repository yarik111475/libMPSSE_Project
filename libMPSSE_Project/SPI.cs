using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    public class SPI {
        IntPtr handle = IntPtr.Zero;

        private const uint SPI_TRANSFER_OPTIONS_SIZE_IN_BYTES = 0x00000000;
        private const uint SPI_TRANSFER_OPTIONS_SIZE_IN_BITS = 0x00000001;
        private const uint SPI_TRANSFER_OPTIONS_CHIPSELECT_ENABLE = 0x00000002;
        private const uint SPI_TRANSFER_OPTIONS_CHIPSELECT_DISABLE = 0x00000004;

        private const uint SPI_CONFIG_OPTION_MODE_MASK = 0x00000003;
        private const uint SPI_CONFIG_OPTION_MODE0 = 0x00000000;
        private const uint SPI_CONFIG_OPTION_MODE1 = 0x00000001;
        private const uint SPI_CONFIG_OPTION_MODE2 = 0x00000002;
        private const uint SPI_CONFIG_OPTION_MODE3 = 0x00000003;

        private const uint SPI_CONFIG_OPTION_CS_MASK = 0x0000001C;      /*111 00*/
        private const uint SPI_CONFIG_OPTION_CS_DBUS3 = 0x00000000;     /*000 00*/
        private const uint SPI_CONFIG_OPTION_CS_DBUS4 = 0x00000004;     /*001 00*/
        private const uint SPI_CONFIG_OPTION_CS_DBUS5 = 0x00000008;     /*010 00*/
        private const uint SPI_CONFIG_OPTION_CS_DBUS6 = 0x0000000C;     /*011 00*/
        private const uint SPI_CONFIG_OPTION_CS_DBUS7 = 0x00000010;     /*100 00*/

        private const uint SPI_CONFIG_OPTION_CS_ACTIVELOW = 0x00000020;

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_GetNumChannels", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_GetNumChannels(ref uint numChannels);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_GetChannelInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_GetChannelInfo(uint index,ref FT_DEVICE_LIST_INFO_NODE chanInfo);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_OpenChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_OpenChannel(uint index, ref IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_InitChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_InitChannel(IntPtr handle, ref ChannelConfig config);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_CloseChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_CloseChannel(IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_Read", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_Read(IntPtr handle, ref byte buffer, uint sizeToTransfer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_Write", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_Write(IntPtr handle, byte buffer,uint sizeToTransfer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_ReadWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_ReadWrite(IntPtr handle, byte inBuffer, ref byte outBuffer, uint sizeToTransfer, 
                                                     ref uint sizeTransferred, uint transferOptions);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_IsBusy", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_IsBusy(IntPtr handle, ref bool state);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Init_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init_libMPSSE();

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Cleanup_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup_libMPSSE();

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_ChangeCS", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_ChangeCS(IntPtr handle, uint configOptions);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_WriteGPIO", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS FT_WriteGPIO(IntPtr handle, byte dir, byte value);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_ReadGPIO", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS FT_ReadGPIO(IntPtr handle, ref byte value);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_ToggleCS", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_ToggleCS(IntPtr handle, bool state);

        public FT_STATUS Init(uint channel) {
            FT_STATUS status = FT_STATUS.FT_OK;
            ChannelConfig channelConfig = new ChannelConfig();
            channelConfig.ClockRate = I2C_CLOCKRATE.I2C_CLOCK_STANDARD_MODE;
            channelConfig.LatencyTimer = 255;
            //channelConfig.configOptions=

            Init_libMPSSE();

            status |= SPI_OpenChannel(channel, ref handle);
            status |= SPI_InitChannel(handle, ref channelConfig);
            return status;
        }

        public FT_STATUS WriteByte(byte slaveAddress, byte address, byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            return status;
        }
        public FT_STATUS ReadByte(byte slaveAddress, byte address, ref byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            return status;
        }

        public FT_STATUS Close() {
            FT_STATUS status = FT_STATUS.FT_OK;
            if (handle != IntPtr.Zero) {
                status = SPI_CloseChannel(handle);
            }
            Cleanup_libMPSSE();
            return status;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ChannelConfig {
            public I2C_CLOCKRATE ClockRate;

            public byte LatencyTimer;

            public uint configOptions;   /*This member provides a way to enable/disable features
	                            specific to the protocol that are implemented in the chip
	                            BIT1-0=CPOL-CPHA:	00 - MODE0 - data captured on rising edge, propagated on falling
 						        01 - MODE1 - data captured on falling edge, propagated on rising
 						        10 - MODE2 - data captured on falling edge, propagated on rising
 						        11 - MODE3 - data captured on rising edge, propagated on falling
	                            BIT4-BIT2: 000 - A/B/C/D_DBUS3=ChipSelect
			                              : 001 - A/B/C/D_DBUS4=ChipSelect
 			                              : 010 - A/B/C/D_DBUS5=ChipSelect
 			                              : 011 - A/B/C/D_DBUS6=ChipSelect
 			                              : 100 - A/B/C/D_DBUS7=ChipSelect
 	                            BIT5: ChipSelect is active high if this bit is 0
	                            BIT6 -BIT31		: Reserved
	                            */

            public uint Pin;/*BIT7   -BIT0:   Initial direction of the pins	*/
                       /*BIT15 -BIT8:   Initial values of the pins		*/
                       /*BIT23 -BIT16: Final direction of the pins		*/
                       /*BIT31 -BIT24: Final values of the pins		*/
            public ushort reserved;
        }
    }
}
