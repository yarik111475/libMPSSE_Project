using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    public partial class MPSSE {
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

        

        public FT_STATUS SPI_Init(uint channel) {
            FT_STATUS status = FT_STATUS.FT_OK;
            I2C_ChannelConfig channelConfig = new I2C_ChannelConfig();
            channelConfig.ClockRate = I2C_SPI_CLOCKRATE.I2C_CLOCK_STANDARD_MODE;
            channelConfig.LatencyTimer = 255;
            //channelConfig.configOptions=

            Init_libMPSSE();

            status |= SPI_OpenChannel(channel, ref handle);
            status |= SPI_InitChannel(handle, out channelConfig);
            return status;
        }

        public FT_STATUS SPI_WriteByte(byte slaveAddress, byte address, byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            return status;
        }
        public FT_STATUS SPI_ReadByte(byte slaveAddress, byte address, ref byte data) {
            FT_STATUS status = FT_STATUS.FT_OK;
            return status;
        }

        public FT_STATUS SPI_Close() {
            FT_STATUS status = FT_STATUS.FT_OK;
            if (handle != IntPtr.Zero) {
                status = SPI_CloseChannel(handle);
            }
            Cleanup_libMPSSE();
            return status;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SPI_ChannelConfig {
            public I2C_SPI_CLOCKRATE ClockRate;

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
