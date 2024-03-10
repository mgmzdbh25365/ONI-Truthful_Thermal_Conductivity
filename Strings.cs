using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONI_Truthful_Thermal_Conductivity {
	public class STRINGS {
		public static LocString TITLE = "Truthful Thermal Conductivity";
		public static LocString DESCRIPTION = "Make thermal conductivity displayed almost without rounding, including properties, database, tooltips, info cards(if any), etc.\n- You can click the button on the module interface to switch the accuracy of the thermal conductivity display.";

		public static LocString MODE_LOW = "low";
		public static LocString MODE_NORMAL = "normal";
		public static LocString MODE_HIGH = "high";
		public static LocString MODE_STRICT = "strict";
		public static LocString MODE_CRAZY = "crazy";



		public static LocString MODE_TIPS_LOW = "Retained 3 decimal places, consistent with the original version.";
		public static LocString MODE_TIPS_NORMAL = "Fixed to 3 decimal places, displaying up to 8 decimal places.";
		public static LocString MODE_TIPS_HIGH = "Fixed to 3 decimal places, displaying at least 7 significant digits.";
		public static LocString MODE_TIPS_STRICT = "At this precision, it ensures that the string can be converted to the same single float value.";
		public static LocString MODE_TIPS_CRAZY = "Retain up to 99 significant digits.";

		public static LocString MODE_DESCRIPTION = "this mode,\"1/9(0.111111111938953399658203125)\"will be displayed as:{0}\nClick to switch the accuracy of the thermal conductivity display.\nLong press and hold the mouse to switch the units displayed for thermal conductivity.";

		public static string DEFAULT_LANGUAGE_CODE = "en";

		public static void Localize(string path) {
			try {
				Localization.OverloadStrings(Localization.LoadStringsFile(path,false));
			}
			catch {
			}
		}
		public static void Localize_zh() {
			TITLE = "精确的热导率 (Truthful Thermal Conductivity)";
			DESCRIPTION = "可以让热导率正常显示，包括属性，数据库，工具栏，信息卡片（如果有）等，基本上没有四舍五入。\n - 可以点击模组界面的按钮来切换热导率显示的精准程度。";


			MODE_LOW = "低";
			MODE_NORMAL = "标准";
			MODE_HIGH = "高";
			MODE_STRICT = "严格";
			MODE_CRAZY = "疯狂";



			MODE_TIPS_LOW = "保留了3位小数，跟原版保持一致。";
			MODE_TIPS_NORMAL = "固定保留3位小数，至多显示8位小数。";
			MODE_TIPS_HIGH = "固定保留3位小数，至少显示7位有效数字。";
			MODE_TIPS_STRICT = "该精度下，确保根据字符串能转换为相同的浮点数值。";
			MODE_TIPS_CRAZY = "至多保留99位有效数字（但单精度浮点数的最小精度为2^-149，有着105位有效数字）。";

			MODE_DESCRIPTION = "该精度下，“1/9(0.111111111938953399658203125)”将会显示为：{0}\n点击以切换热导率显示的精准程度。\n长按以切换热导率显示的单位。";
		}
	}
}
