using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FT_DEVICE_LIST_INFO_NODE {
        public uint Flags;
        public uint Type;
        public uint ID;
        public uint LocId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string SerialNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Description;
        public IntPtr ftHandle;
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct I2C_ChannelConfig {
        public I2C_SPI_CLOCKRATE ClockRate;
        public uint LatencyTimer;
        public uint Options;
    }
}