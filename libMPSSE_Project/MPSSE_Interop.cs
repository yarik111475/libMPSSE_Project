using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    public partial class MPSSE {
        IntPtr handle = IntPtr.Zero;

        #region I2C
        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_GetNumChannels", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_GetNumChannels(ref uint NumChannels);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_GetChannelInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_GetChannelInfo(uint index, out FT_DEVICE_LIST_INFO_NODE chanInfo);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_OpenChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_OpenChannel(uint index, ref IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_InitChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_InitChannel(IntPtr handler, out I2C_ChannelConfig config);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_CloseChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_CloseChannel(IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceRead", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_DeviceRead(IntPtr handle, uint deviceAddress, uint sizeToTransfer,
            byte[] buffer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS I2C_DeviceWrite(IntPtr handle, uint deviceAddress, uint sizeToTransfer,
            byte[] buffer, ref uint sizeTransfered, uint options);
        #endregion

        #region SPI
        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_GetNumChannels", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_GetNumChannels(ref uint numChannels);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_GetChannelInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_GetChannelInfo(uint index, out FT_DEVICE_LIST_INFO_NODE chanInfo);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_OpenChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_OpenChannel(uint index, ref IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_InitChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_InitChannel(IntPtr handle, out I2C_ChannelConfig config);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_CloseChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_CloseChannel(IntPtr handle);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_Read", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_Read(IntPtr handle, ref byte buffer, uint sizeToTransfer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_Write", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_Write(IntPtr handle, byte buffer, uint sizeToTransfer, ref uint sizeTransfered, uint options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_ReadWrite", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_ReadWrite(IntPtr handle, byte inBuffer, ref byte outBuffer, uint sizeToTransfer,
                                                     ref uint sizeTransferred, uint transferOptions);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_IsBusy", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_IsBusy(IntPtr handle, ref bool state);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "SPI_ToggleCS", CallingConvention = CallingConvention.Cdecl)]
        public static extern FT_STATUS SPI_ToggleCS(IntPtr handle, bool state);
        #endregion

        #region COMMON
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
        #endregion

        static void APP_CHECK_STATUS(FT_STATUS status) {
            if (status != FT_STATUS.FT_OK) {
                Console.WriteLine("Error: {0}", status.ToString());
            }
        }
    }
}
