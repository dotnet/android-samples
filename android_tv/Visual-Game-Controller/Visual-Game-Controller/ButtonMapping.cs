using System;
using Android.Views;

namespace VisualGameController
{
	//A class used to hold the button keycode mapping of the controller
	public class ButtonMapping
	{
		public static Keycode BUTTON_A = Keycode.ButtonA;
		public static Keycode BUTTON_B = Keycode.ButtonB;
		public static Keycode BUTTON_X = Keycode.ButtonX;
		public static Keycode BUTTON_Y = Keycode.ButtonY;
		public static Keycode BUTTON_L1 = Keycode.ButtonL1;
		public static Keycode BUTTON_R1 = Keycode.ButtonR1;
		public static Keycode BUTTON_L2=Keycode.ButtonL2;
		public static Keycode BUTTON_R2 = Keycode.ButtonR2;
		public static Keycode BUTTON_SELECT = Keycode.ButtonSelect;
		public static Keycode BUTTON_START=Keycode.ButtonStart;
		public static Keycode BUTTON_THUMBL = Keycode.ButtonThumbl;
		public static Keycode BUTTON_THUMBR = Keycode.ButtonThumbr;
		public static Keycode BACK = Keycode.Back;
		public static Keycode POWER = Keycode.ButtonMode;
		public static int size = 14;
		private static int key_code;

		public ButtonMapping(int keyCode)
		{
			key_code = keyCode;
		}

		public static int getKeyCode()
		{
			return key_code;
		}

		public static int OrdinalValue(Keycode key)
		{
			if (key == BUTTON_A) {
				return 0;
			} else if (key == BUTTON_B) {
				return 1;
			} else if (key == BUTTON_X) {
				return 2;
			} else if (key == BUTTON_Y) {
				return 3;
			} else if (key == BUTTON_L1) {
				return 4;
			} else if (key == BUTTON_R1) {
				return 5;
			} else if (key == BUTTON_L2) {
				return 6;
			} else if (key == BUTTON_R2) {
				return 7;
			} else if (key == BUTTON_SELECT) {
				return 8;
			} else if (key == BUTTON_START) {
				return 9;
			} else if (key == BUTTON_THUMBL) {
				return 10;
			} else if (key == BUTTON_THUMBR) {
				return 11;
			} else if (key == BACK) {
				return 12;
			} else if (key == POWER) {
				return 13;
			} else {
				return -1;
			}

		}

		public static Keycode OrdinalValueButton(int val)
		{
			switch (val) {
			case 0:
				return BUTTON_A;
			case 1:
				return BUTTON_B;
			case 2:
				return BUTTON_X;
			case 3:
				return BUTTON_Y;
			case 4:
				return BUTTON_L1;
			case 5:
				return BUTTON_R1;
			case 6:
				return BUTTON_L2;
			case 7:
				return BUTTON_R2;
			case 8:
				return BUTTON_SELECT;
			case 9:
				return BUTTON_START;
			case 10:
				return BUTTON_THUMBL;
			case 11:
				return BUTTON_THUMBR;
			case 12:
				return BACK;
			case 13:
				return POWER;
			}
			return new Keycode();
		}
	}
}

