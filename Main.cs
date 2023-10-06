﻿using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace ONI_Truthful_Thermal_Conductivity {
	public class Main : UserMod2 {
		public static string[] modeName_EN = new string[]{
			"low",
			"normal",
			"high",
			"strict",
			"crazy"
		};
		public string[] modeName = modeName_EN;
		public static string[] modeName_ZH = new string[]{
			"低",
			"标准",
			"高",
			"严格",
			"疯狂"
		};
		public static string[] modeTips_EN = new string[]{
			"Retained 3 decimal places, consistent with the original version.",
			"Fixed to 3 decimal places, displaying up to 8 decimal places.",
			"Fixed to 3 decimal places, displaying at least 7 significant digits.",
			"At this precision, it ensures that the string can be converted to the same single float value.",
			"Retain up to 99 significant digits."
		};
		public string[] modeTips = modeTips_EN;
		public static string[] modeTips_ZH = new string[]{
			"保留了3位小数，跟原版保持一致。",
			"固定保留3位小数，至多显示8位小数。",
			"固定保留3位小数，至少显示7位有效数字。",
			"该精度下，确保根据字符串能转换为相同的浮点数值。",
			"至多保留99位有效数字（但单精度浮点数的最小精度为2e-149，有着105位有效数字）。"
		};
		public static string tooltip_EN = "this mode,\"1/9\"will be displayed as:{0}\nClick to switch the accuracy of the thermal conductivity display.\nLong press and hold the mouse to switch the units displayed for thermal conductivity.";
		public string tooltip = tooltip_EN;
		public static string tooltip_ZH = "该精度下，“1/9”将会显示为：{0}\n点击以切换热导率显示的精准程度。\n长按以切换热导率显示的单位。";
		public static string format = "0.000#####";
		public static string[] formats = new string[] {
			"0.000",
			"0.000#####",
			"0.000################################################",//“#”只会识别7位有效数字？似乎足以表示最小精度float.Epsilon≈0.000000000000000000000000000000000000000000001401298
			//float.Epsilon=0.00000000000000000000000000000000000000000000140129846432481707092372958328991613128026194187651577175706828388979108268586060148663818836212158203125
			//"0.000################################################################################################################################################################################################################################################################################################################################################",
			"r",
			"G99"//只能识别到这么多，但最小精度有105位有效数字。
		};
		public static Main main = null;
		public static Options options = null;
		public static Harmony main_harmony = null;
		public KButton button = null;
		bool data_patch = false;
		public override void OnLoad(Harmony harmony) {
			main = this;
			main_harmony = harmony;
			try {
				options = JsonConvert.DeserializeObject<Options>(File.ReadAllText(Path.Combine(path , "config.json")));
				if (options.mode < 0)
					options.mode = 0;
				else if (options.mode >= formats.Length)
					options.mode = formats.Length - 1;
				format = formats[options.mode];
				if (options.custom != null) {
					if (options.custom.scaleChanged)
						format = options.custom.scale_format;
					if (options.custom.tipsChanged) {
						Tips_Patch.format = options.custom.tips_format;
						harmony.Patch(typeof(GameUtil).GetMethod("GetFormattedThermalConductivity") , transpiler: new HarmonyMethod(typeof(Tips_Patch) , nameof(Tips_Patch.Transpiler)));
						harmony.Patch(typeof(GameUtil).GetMethod("GetThermalConductivitySuffix") , transpiler: new HarmonyMethod(typeof(Tips_Patch) , nameof(Tips_Patch.Transpiler)));
					}
				}
				harmony.Patch(typeof(GameUtil).GetMethod("GetFormattedThermalConductivity") , transpiler: new HarmonyMethod(typeof(ThermalConductivity_Patch) , nameof(ThermalConductivity_Patch.Transpiler)));
				harmony.Patch(typeof(AdditionalDetailsPanel).GetMethod("RefreshDetails" , BindingFlags.NonPublic | BindingFlags.Instance) , transpiler: new HarmonyMethod(typeof(Details_Patch) , nameof(Details_Patch.Transpiler)));
				harmony.Patch(typeof(ModsScreen).GetMethod("BuildDisplay" , BindingFlags.NonPublic | BindingFlags.Instance) , postfix: new HarmonyMethod(typeof(Options_add) , nameof(Options_add.Postfix)));
				data_patch = true;
			}
			catch {
				if (options == null)
					options = new Options();
				base.OnLoad(harmony);
			}
		}
		public override void OnAllModsLoaded(Harmony harmony , System.Collections.Generic.IReadOnlyList<Mod> mods) {
			if (!data_patch)
				ReLoad();
			base.OnAllModsLoaded(harmony , mods);
		}
		public static void SwitchMode(KButton kButton) {
			options.mode++;
			if (options.mode >= formats.Length)
				options.mode = 0;
			format = formats[options.mode];
			File.WriteAllText(Path.Combine(main.path , "config.json") , JsonConvert.SerializeObject(options));
			main.ReLoad();
			kButton.GetComponentInChildren<LocText>().text = main.modeName[options.mode];
			string tooltip = Main.main.modeTips[Main.options.mode] + '\n' + Main.main.tooltip;
			try {
				float oneNinth = 1f / 9f;
				tooltip = string.Format(tooltip , GameUtil.GetFormattedThermalConductivity(oneNinth));
			}
			catch { }
			kButton.GetComponent<ToolTip>().toolTip = tooltip;
		}
		public void ReLoad() {
			try {
				options = JsonConvert.DeserializeObject<Options>(File.ReadAllText(Path.Combine(path , "config.json")));
				if (options.mode < 0)
					options.mode = 0;
				else if (options.mode >= formats.Length)
					options.mode = formats.Length - 1;
				format = formats[options.mode];
				if (options.custom != null) {
					if (options.custom.scaleChanged)
						format = options.custom.scale_format;
					if (main_harmony != null) {
						main_harmony.Unpatch(typeof(GameUtil).GetMethod("GetFormattedThermalConductivity") , typeof(Tips_Patch).GetMethod(nameof(Tips_Patch.Transpiler)));
						main_harmony.Unpatch(typeof(GameUtil).GetMethod("GetThermalConductivitySuffix") , typeof(Tips_Patch).GetMethod(nameof(Tips_Patch.Transpiler)));
					}
					if (options.custom.tipsChanged) {
						Tips_Patch.format = options.custom.tips_format;
						main_harmony.Patch(typeof(GameUtil).GetMethod("GetFormattedThermalConductivity") , transpiler: new HarmonyMethod(typeof(Tips_Patch) , nameof(Tips_Patch.Transpiler)));
						main_harmony.Patch(typeof(GameUtil).GetMethod("GetThermalConductivitySuffix") , transpiler: new HarmonyMethod(typeof(Tips_Patch) , nameof(Tips_Patch.Transpiler)));
					}
				}
			}
			catch {
			}
			try {
				if (main_harmony != null) {
					main_harmony.Unpatch(typeof(GameUtil).GetMethod("GetFormattedThermalConductivity") , typeof(ThermalConductivity_Patch).GetMethod(nameof(ThermalConductivity_Patch.Transpiler)));
					main_harmony.Unpatch(typeof(AdditionalDetailsPanel).GetMethod("RefreshDetails" , BindingFlags.NonPublic | BindingFlags.Instance) , typeof(Details_Patch).GetMethod(nameof(Details_Patch.Transpiler)));
					main_harmony.Unpatch(typeof(ModsScreen).GetMethod("BuildDisplay" , BindingFlags.NonPublic | BindingFlags.Instance) , typeof(Options_add).GetMethod(nameof(Options_add.Postfix)));
					main_harmony.Patch(typeof(GameUtil).GetMethod("GetFormattedThermalConductivity") , transpiler: new HarmonyMethod(typeof(ThermalConductivity_Patch) , nameof(ThermalConductivity_Patch.Transpiler)));
					main_harmony.Patch(typeof(AdditionalDetailsPanel).GetMethod("RefreshDetails" , BindingFlags.NonPublic | BindingFlags.Instance) , transpiler: new HarmonyMethod(typeof(Details_Patch) , nameof(Details_Patch.Transpiler)));
					main_harmony.Patch(typeof(ModsScreen).GetMethod("BuildDisplay" , BindingFlags.NonPublic | BindingFlags.Instance) , postfix: new HarmonyMethod(typeof(Options_add) , nameof(Options_add.Postfix)));
				}
			}
			catch {
			}
		}
	}
}