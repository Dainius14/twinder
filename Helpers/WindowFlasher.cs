using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Twinder.Helpers
{
	/// <summary>
	/// Helper class for flashing window
	/// </summary>
	public static class WindowFlasher
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

		private struct FLASHWINFO
		{        
			/// <summary>
			/// The size of the structure in bytes.
			/// </summary>
			public uint cbSize;

			/// <summary>
			/// A Handle to the Window to be Flashed. The window can be either opened or minimized.
			/// </summary>
			public IntPtr hwnd;

			/// <summary>
			/// The Flash Status.
			/// </summary>
			public uint dwFlags;

			/// <summary>
			/// The number of times to Flash the window.
			/// </summary>
			public uint uCount;

			/// <summary>
			/// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
			/// </summary>
			public uint dwTimeout;
		}

		private enum FlashWindow : uint
		{
			/// <summary>
			/// Stop flashing. The system restores the window to its original stae.
			/// </summary>
			Stop = 0,

			/// <summary>
			/// Flash the window caption.
			/// </summary>
			Caption = 1,

			/// <summary>
			/// Flash the taskbar button.
			/// </summary>
			Tray = 2,

			/// <summary>
			/// Flash both the window caption and taskbar button.
			/// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
			/// </summary>
			All = 3,

			/// <summary>
			/// Flash continuously, until the FLASHW_STOP flag is set.
			/// </summary>
			Timer = 4,

			/// <summary>
			/// Flash continuously until the window comes to the foreground.
			/// </summary>
			TimerNofg = 12
		}

		private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
		{
			FLASHWINFO fi = new FLASHWINFO();
			fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
			fi.hwnd = handle;
			fi.dwFlags = flags;
			fi.uCount = count;
			fi.dwTimeout = timeout;
			return fi;
		}

		/// <summary>
		/// Flash the specified Window for the specified number of times
		/// </summary>
		/// <param name="count">The number of times to Flash.</param>
		public static bool Flash(Window window, uint count)
		{
			if (Win2000OrLater)
			{
				FLASHWINFO fi = Create_FLASHWINFO(CreateHandle(window), (uint) FlashWindow.All, count, 0);
				return FlashWindowEx(ref fi);
			}
			return false;
		}

		/// <summary>
		/// Start flashing the specified Window
		/// </summary>
		public static bool Start(Window window)
		{
			if (Win2000OrLater)
			{
				FLASHWINFO fi = Create_FLASHWINFO(CreateHandle(window), (uint) FlashWindow.All, uint.MaxValue, 0);
				return FlashWindowEx(ref fi);
			}
			return false;
		}

		/// <summary>
		/// Stop flashing the specified Window
		/// </summary>
		public static bool Stop(Window window)
		{
			if (Win2000OrLater)
			{
				FLASHWINFO fi = Create_FLASHWINFO(CreateHandle(window), (uint) FlashWindow.Stop, uint.MaxValue, 0);
				return FlashWindowEx(ref fi);
			}
			return false;
		}

		/// <summary>
		/// A boolean value indicating whether the application is running on Windows 2000 or later.
		/// </summary>
		private static bool Win2000OrLater
		{
			get { return Environment.OSVersion.Version.Major >= 5; }
		}

		/// <summary>
		/// Returns the handle to given window
		/// </summary>
		private static IntPtr CreateHandle(Window window)
		{
			return new WindowInteropHelper(window).Handle;
		}
	}
}
