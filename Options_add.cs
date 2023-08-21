using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using KMod;
using UnityEngine;
using UnityEngine.UI;

namespace ONI_Truthful_Thermal_Conductivity {
	[HarmonyPatch(typeof(ModsScreen) , "BuildDisplay")]
	class Options_add {
		private static void Postfix(IList ___displayedMods) {
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
			if (Options.main == null)
				return;
			string staticID = Options.main.mod.staticID;
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
				Options.main.ReLoad();
				if (Localization.GetLocale().Code.ToUpper().StartsWith("ZH")) {
					Options.main.modeName = Options.modeName_ZH;
					hierarchyReferences.GetReference<LocText>("Title").text = "精确的热导率 (Truthful Thermal Conductivity)";
					Options.main.tooltip = "点击以切换热导率显示的精准程度。";
					hierarchyReferences.GetReference<ToolTip>("Description").toolTip = "- 让热导率正常显示，包括属性，数据库，工具栏，信息卡片（如果有）等，基本上没有四舍五入。\n- 可以点击模组界面的按钮来切换热导率显示的精准程度。";
				}
			}
			catch { }
			if (Options.main.button != null)
				return;
			KButton kButton = Options.main.button = Util.KInstantiateUI<KButton>(hierarchyReferences.GetReference<KButton>("ManageButton").gameObject , hierarchyReferences.gameObject , false);
			kButton.name = "SwitchButton";
			kButton.GetComponentInChildren<LocText>().text = Options.main.modeName[Options.main.mode];
			kButton.GetComponent<ToolTip>().toolTip = Options.main.tooltip;
			kButton.GetComponent<LayoutElement>().minWidth = 50f;
			kButton.transform.SetSiblingIndex(4);
			kButton.gameObject.SetActive(true);
			kButton.onClick += delegate {
				Options.SwitchMode(kButton);
			};
		}
	}
}