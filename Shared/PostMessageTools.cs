﻿using Decal.Adapter;
using System;
using System.Windows.Forms;

namespace Mag.Shared
{
	public static class PostMessageTools
	{
		// http://msdn.microsoft.com/en-us/library/dd375731%28v=vs.85%29.aspx

		#region Private Fields + Events

		private const byte VK_CONTROL = 0x11;
		private const byte VK_PAUSE = 0x13;
		private const byte VK_RETURN = 0x0D;
		private const byte VK_SHIFT = 0x10;
		private const byte VK_SPACE = 0x20;

		private static int _movementHoldTimeMilliseconds;

		private static char _movementKey;

		private static Timer _movementReleaseTimer;

		private static DateTime _movementSendTime;

		private static bool _spaceAddC;

		private static bool _spaceAddShift;

		private static bool _spaceAddW;

		private static bool _spaceAddX;

		private static bool _spaceAddZ;

		private static int _spaceHoldTimeMilliseconds;

		private static Timer _spaceReleaseTimer;

		private static DateTime _spaceSendTime;

		#endregion Private Fields + Events

		#region Public Methods

		public static void ClickNo()
		{
			User32.RECT rect = new User32.RECT();

			User32.GetWindowRect(CoreManager.Current.Decal.Hwnd, ref rect);

			// The reason why we click at both of these positions is some clients will be running
			// windowed, and some windowless. This will hit both locations
			SendMouseClick(rect.Width / 2 + 80, rect.Height / 2 + 18);
			SendMouseClick(rect.Width / 2 + 80, rect.Height / 2 + 25);
			SendMouseClick(rect.Width / 2 + 80, rect.Height / 2 + 31);
		}

		public static void ClickOK()
		{
			User32.RECT rect = new User32.RECT();

			User32.GetWindowRect(CoreManager.Current.Decal.Hwnd, ref rect);

			// The reason why we click at both of these positions is some clients will be running
			// windowed, and some windowless. This will hit both locations
			SendMouseClick(rect.Width / 2, rect.Height / 2 + 18);
			SendMouseClick(rect.Width / 2, rect.Height / 2 + 25);
			SendMouseClick(rect.Width / 2, rect.Height / 2 + 31);
		}

		public static void ClickYes()
		{
			User32.RECT rect = new User32.RECT();

			User32.GetWindowRect(CoreManager.Current.Decal.Hwnd, ref rect);

			// 800x600 +32 works, +33 does not work on single/double/tripple line boxes 1600x1200 +31
			// works, +32 does not work on single/double/tripple line boxes The reason why we click
			// at both of these positions is some clients will be running windowed, and some
			// windowless. This will hit both locations
			SendMouseClick(rect.Width / 2 - 80, rect.Height / 2 + 18);
			SendMouseClick(rect.Width / 2 - 80, rect.Height / 2 + 25);
			SendMouseClick(rect.Width / 2 - 80, rect.Height / 2 + 31);
		}

		public static void SendAltF4()
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_DESTROY, new IntPtr(0), new UIntPtr(0));
		}

		public static void SendCntrl(char ch)
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)VK_CONTROL, (UIntPtr)0x001D0001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode(ch), (UIntPtr)0x00100001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode(ch), (UIntPtr)0xC0100001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_CONTROL, (UIntPtr)0xC01D0001);
		}

		public static void SendEnter()
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)VK_RETURN, (UIntPtr)0x001C0001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_RETURN, (UIntPtr)0xC01C0001);
		}

		/// <summary>
		/// Opens/Closes main pack view
		/// </summary>
		public static void SendF12()
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)0x0000007B, (UIntPtr)0x00580001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)0x0000007B, (UIntPtr)0xC0580001);
		}

		/// <summary>
		/// Opens/Closes allegiance view
		/// </summary>
		public static void SendF3()
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)0x00000072, (UIntPtr)0x003D0001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)0x00000072, (UIntPtr)0xC03D0001);
		}

		/// <summary>
		/// Opens/Closes fellowship view
		/// </summary>
		public static void SendF4()
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)0x00000073, (UIntPtr)0x003E0001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)0x00000073, (UIntPtr)0xC03E0001);
		}

		public static void SendMouseClick(int x, int y)
		{
			int loc = (y * 0x10000) + x;

			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_MOUSEMOVE, (IntPtr)0x00000000, (UIntPtr)loc);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_LBUTTONDOWN, (IntPtr)0x00000001, (UIntPtr)loc);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_LBUTTONUP, (IntPtr)0x00000000, (UIntPtr)loc);
		}

		public static void SendMovement(char ch, int msToHoldDown = 0)
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode(ch), (UIntPtr)(0x00000001 + ScanCode(ch) * 0x10000));
			if (msToHoldDown == 0)
			{
				User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode(ch), (UIntPtr)(0xC0000001 + ScanCode(ch) * 0x10000));
			}
			else
			{
				if (_movementReleaseTimer == null)
				{
					_movementReleaseTimer = new Timer();
					_movementReleaseTimer.Tick += new EventHandler(MovementReleaseTimer_Tick);
					_movementReleaseTimer.Interval = 1;
				}

				_movementSendTime = DateTime.Now;
				_movementHoldTimeMilliseconds = msToHoldDown;
				_movementKey = ch;
				_movementReleaseTimer.Start();
			}
		}

		public static void SendMsg(string msg)
		{
			foreach (char ch in msg)
			{
				byte code = CharCode(ch);
				uint lparam = (uint)((ScanCode(ch) << 0x10) | 1);
				User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)code, (UIntPtr)(lparam));
				User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)code, (UIntPtr)(0xC0000000 | lparam));
			}
		}

		public static void SendPause()
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)VK_PAUSE, (UIntPtr)0x00450001);
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_PAUSE, (UIntPtr)0xC0450001);
		}

		public static void SendSpace(int msToHoldDown = 0, bool addShift = false, bool addW = false, bool addZ = false, bool addX = false, bool addC = false)
		{
			User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)VK_SPACE, (UIntPtr)0x00390001);
			if (msToHoldDown == 0)
			{
				if (addShift) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)VK_SHIFT, (UIntPtr)0x002A0001);
				if (addW) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('w'), (UIntPtr)0x00110001);
				if (addZ) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('z'), (UIntPtr)0x002C0001);
				if (addX) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('x'), (UIntPtr)0x002D0001);
				if (addC) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('c'), (UIntPtr)0x002E0001);
				User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_SPACE, (UIntPtr)0xC0390001);
				if (addW) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('w'), (UIntPtr)0xC0110001);
				if (addZ) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('z'), (UIntPtr)0xC02C0001);
				if (addX) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('x'), (UIntPtr)0xC02D0001);
				if (addC) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('c'), (UIntPtr)0xC02E0001);
				if (addShift) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_SHIFT, (UIntPtr)0xC02A0001);
			}
			else
			{
				if (_spaceReleaseTimer == null)
				{
					_spaceReleaseTimer = new Timer();
					_spaceReleaseTimer.Tick += new EventHandler(SpaceReleaseTimer_Tick);
					_spaceReleaseTimer.Interval = 1;
				}

				_spaceSendTime = DateTime.UtcNow;
				_spaceHoldTimeMilliseconds = msToHoldDown;
				_spaceAddShift = addShift;
				_spaceAddW = addW;
				_spaceAddZ = addZ;
				_spaceAddX = addX;
				_spaceAddC = addC;
				_spaceReleaseTimer.Start();
			}
		}

		#endregion Public Methods

		#region Private Methods

		private static byte CharCode(char Char)
		{
			switch (char.ToLower(Char))
			{
				case 'a': return 0x41;
				case 'b': return 0x42;
				case 'c': return 0x43;
				case 'd': return 0x44;
				case 'e': return 0x45;
				case 'f': return 0x46;
				case 'g': return 0x47;
				case 'h': return 0x48;
				case 'i': return 0x49;
				case 'j': return 0x4A;
				case 'k': return 0x4B;
				case 'l': return 0x4C;
				case 'm': return 0x4D;
				case 'n': return 0x4E;
				case 'o': return 0x4F;
				case 'p': return 0x50;
				case 'q': return 0x51;
				case 'r': return 0x52;
				case 's': return 0x53;
				case 't': return 0x54;
				case 'u': return 0x55;
				case 'v': return 0x56;
				case 'w': return 0x57;
				case 'x': return 0x58;
				case 'y': return 0x59;
				case 'z': return 0x5A;
				case '/': return 0xBF;
				case ' ': return 0x20;
			}
			return 0x20;
		}

		private static void MovementReleaseTimer_Tick(object sender, EventArgs e)
		{
			if (_movementSendTime.AddMilliseconds(_movementHoldTimeMilliseconds) <= DateTime.Now)
			{
				_movementReleaseTimer.Stop();
				User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode(_movementKey), (UIntPtr)(0xC0000001 + ScanCode(_movementKey) * 0x10000));
			}
		}

		private static byte ScanCode(char Char)
		{
			switch (char.ToLower(Char))
			{
				case 'a': return 0x1E;
				case 'b': return 0x30;
				case 'c': return 0x2E;
				case 'd': return 0x20;
				case 'e': return 0x12;
				case 'f': return 0x21;
				case 'g': return 0x22;
				case 'h': return 0x23;
				case 'i': return 0x17;
				case 'j': return 0x24;
				case 'k': return 0x25;
				case 'l': return 0x26;
				case 'm': return 0x32;
				case 'n': return 0x31;
				case 'o': return 0x18;
				case 'p': return 0x19;
				case 'q': return 0x10;
				case 'r': return 0x13;
				case 's': return 0x1F;
				case 't': return 0x14;
				case 'u': return 0x16;
				case 'v': return 0x2F;
				case 'w': return 0x11;
				case 'x': return 0x2D;
				case 'y': return 0x15;
				case 'z': return 0x2C;
				case '/': return 0x35;
				case ' ': return 0x39;
			}
			return 0;
		}

		private static void SpaceReleaseTimer_Tick(object sender, EventArgs e)
		{
			if (_spaceSendTime.AddMilliseconds(_spaceHoldTimeMilliseconds) <= DateTime.UtcNow)
			{
				_spaceReleaseTimer.Stop();
				if (_spaceAddShift) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)VK_SHIFT, (UIntPtr)0x002A0001);
				if (_spaceAddW) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('w'), (UIntPtr)0x00110001);
				if (_spaceAddZ) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('z'), (UIntPtr)0x002C0001);
				if (_spaceAddX) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('x'), (UIntPtr)0x002D0001);
				if (_spaceAddC) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYDOWN, (IntPtr)CharCode('c'), (UIntPtr)0x002E0001);
				User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_SPACE, (UIntPtr)0xC0390001);
				if (_spaceAddW) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('w'), (UIntPtr)0xC0110001);
				if (_spaceAddZ) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('z'), (UIntPtr)0xC02C0001);
				if (_spaceAddX) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('x'), (UIntPtr)0xC02D0001);
				if (_spaceAddC) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)CharCode('c'), (UIntPtr)0xC02E0001);
				if (_spaceAddShift) User32.PostMessage(CoreManager.Current.Decal.Hwnd, User32.WM_KEYUP, (IntPtr)VK_SHIFT, (UIntPtr)0xC02A0001);
			}
		}

		#endregion Private Methods
	}
}