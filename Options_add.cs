using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace ONI_Truthful_Thermal_Conductivity {
	[HarmonyPatch(typeof(ModsScreen) , "BuildDisplay")]
	class Options_add {
		public static void Postfix(IList ___displayedMods) {
			if (___displayedMods == null)
				return;
			object displayedMod = ___displayedMods[0];
			if (displayedMod == null)
				return;
			Type type = displayedMod.GetType();
			FieldInfo[] fields = type.GetFields();
			FieldInfo field_index = null;
			FieldInfo field_rect = null;
			foreach (FieldInfo field in fields) {
				switch (field.Name) {
					case "mod_index":
						field_index = field;
						break;
					case "rect_transform":
						field_rect = field;
						break;
				}
			}
			if (field_index == null || field_rect == null)
				return;
			Mod mod = null;
			if (Main.main == null)
				return;
			string staticID = Main.main.mod.staticID;
			foreach (object current in ___displayedMods) {
				mod = Global.Instance.modManager.mods[(int)field_index.GetValue(current)];
				if (mod != null && mod.staticID == staticID) {
					displayedMod = current;
					break;
				}
			}
			if (displayedMod == null)
				return;
			RectTransform rect = (RectTransform)field_rect.GetValue(displayedMod);
			if (rect == null)
				return;
			HierarchyReferences hierarchyReferences = rect.GetComponent<HierarchyReferences>();
			if (hierarchyReferences == null)
				return;
			try {
				if (Localization.GetLocale().Code.ToUpper().StartsWith("ZH")) {
					Main.main.modeName = Main.modeName_ZH;
					Main.main.modeTips = Main.modeTips_ZH;
					hierarchyReferences.GetReference<LocText>("Title").text = "精确的热导率 (Truthful Thermal Conductivity)";
					Main.main.tooltip = Main.tooltip_ZH;
					hierarchyReferences.GetReference<ToolTip>("Description").toolTip = "- 让热导率正常显示，包括属性，数据库，工具栏，信息卡片（如果有）等，基本上没有四舍五入。\n- 可以点击模组界面的按钮来切换热导率显示的精准程度。";
				}
			}
			catch { }
			if (Main.main.button != null)
				return;
			KButton kButton = Main.main.button = Util.KInstantiateUI<KButton>(hierarchyReferences.GetReference<KButton>("ManageButton").gameObject , hierarchyReferences.gameObject , false);
			kButton.name = "SwitchButton";
			kButton.GetComponentInChildren<LocText>().text = Main.main.modeName[Main.options.mode];
			string tooltip = Main.main.modeTips[Main.options.mode]+'\n'+Main.main.tooltip;
			try {
				float oneNinth = 1f / 9f;
				tooltip = string.Format(tooltip , GameUtil.GetFormattedThermalConductivity(oneNinth));
			}
			catch { }
			kButton.GetComponent<ToolTip>().toolTip = tooltip;
			kButton.GetComponent<LayoutElement>().minWidth = 50f;
			kButton.transform.SetSiblingIndex(4);
			kButton.gameObject.SetActive(true);
			kButton.onClick += delegate {
				Main.SwitchMode(kButton);
			};
			Thread longClick = null;
			kButton.onPointerDown += delegate {
				if(longClick!=null)
					longClick.Abort();
				longClick = new Thread(() => {
					try {///TODO:长按弹出自定义菜单。
						Thread.Sleep(2000);
                        Options.Custom custom = Main.options.GetCustomOrCreate();
						if (custom.tipsChanged) {
							custom.tipsChanged = false;
						}
						else {
							custom.tipsChanged = true;
							custom.tips_format = "(J/(m*s))/";
						}
						File.WriteAllText(Path.Combine(Main.main.path , "config.json") , JsonConvert.SerializeObject(Main.options));
						Main.main.ReLoad();
						kButton.GetComponentInChildren<LocText>().text = Main.main.modeName[Main.options.mode];
						string tooltip2 = Main.main.modeTips[Main.options.mode] + '\n' + Main.main.tooltip;
						try {
							float oneNinth = 1f / 9f;
							tooltip2 = string.Format(tooltip2 , GameUtil.GetFormattedThermalConductivity(oneNinth));
						}
						catch { }
						kButton.GetComponent<ToolTip>().toolTip = tooltip2;

						kButton.PlayPointerDownSound();
							kButton.ClearOnClick();
							kButton.onClick += delegate {
								kButton.ClearOnClick();
								kButton.onClick += delegate {
									Main.SwitchMode(kButton);
								};
							};
					}
					catch (ThreadAbortException) { }
				});
				longClick.Start();
			};
			kButton.onPointerUp += delegate {
				if (longClick != null)
					longClick.Abort();
			};
		}
	}
}