using System;
using Android.Views;

namespace VisualGameController
{
	//A class to represent the different Axes values
	public class AxesMapping
	{
		public static Axis AXIS_X=MotionEvent.AxisFromString("AXIS_X");
		public static Axis AXIS_Y=MotionEvent.AxisFromString("AXIS_Y");
		public static Axis AXIS_Z=MotionEvent.AxisFromString("AXIS_Z");
		public static Axis AXIS_RZ=MotionEvent.AxisFromString("AXIS_RZ");
		public static Axis AXIS_HAT_X=MotionEvent.AxisFromString("AXIS_HAT_X");
		public static Axis AXIS_HAT_Y=MotionEvent.AxisFromString("AXIS_HAT_Y");
		public static Axis AXIS_LTRIGGER=MotionEvent.AxisFromString("AXIS_LTRIGGER");
		public static Axis AXIS_RTRIGGER=MotionEvent.AxisFromString("AXIS_RTRIGGER");
		public static Axis AXIS_BRAKE=MotionEvent.AxisFromString("AXIS_BRAKE");
		public static Axis AXIS_GAS=MotionEvent.AxisFromString("AXIS_GAS");

		//Right Axis for xbox gamepads
		public static Axis AXIS_RX = MotionEvent.AxisFromString ("AXIS_RX");
		public static Axis AXIS_RY = MotionEvent.AxisFromString ("AXIS_RY");

		public static int size = 12;
		private static int motion_event;

		public static int OrdinalValue(Axis axis)
		{
			if (axis == AXIS_X) {
				return 0;
			} else if (axis == AXIS_Y) {
				return 1;
			} else if (axis == AXIS_Z) {
				return 2;
			} else if (axis == AXIS_RZ) {
				return 3;
			} else if (axis == AXIS_HAT_X) {
				return 4;
			} else if (axis == AXIS_HAT_Y) {
				return 5;
			} else if (axis == AXIS_LTRIGGER) {
				return 6;
			} else if (axis == AXIS_RTRIGGER) {
				return 7;
			} else if (axis == AXIS_BRAKE) {
				return 8;
			} else if (axis == AXIS_GAS) {
				return 9;
			} else if (axis == AXIS_RX) {
				return 10;
			} else if (axis == AXIS_RY) {
				return 11;
			} else {
				return -1;
			}


		}
		public static Axis OrdinalValueAxis(int val)
		{
			switch (val) {
			case 0:
				return AXIS_X;
			case 1:
				return AXIS_Y;
			case 2:
				return AXIS_Z;
			case 3:
				return AXIS_RZ;
			case 4:
				return AXIS_HAT_X;
			case 5:
				return AXIS_HAT_Y;
			case 6:
				return AXIS_LTRIGGER;
			case 7:
				return AXIS_RTRIGGER;
			case 8:
				return AXIS_BRAKE;
			case 9:
				return AXIS_GAS;
			case 10:
				return AXIS_RX;
			case 11:
				return AXIS_RY;

			}
			return new Axis();
		}
		public AxesMapping(int motionevent)
		{
			motion_event = motionevent;
		}
		public static int getMotionEvent()
		{
			return AxesMapping.motion_event;
		}

	}
}

