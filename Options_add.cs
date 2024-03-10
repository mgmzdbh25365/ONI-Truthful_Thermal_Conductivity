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
			if (Main.main.button != null)
				return;
			RectTransform rect = (RectTransform)field_rect.GetValue(displayedMod);
			if (rect == null)
				return;
			HierarchyReferences hierarchyReferences = rect.GetComponent<HierarchyReferences>();
			if (hierarchyReferences == null)
				return;
			try {
				string code = Localization.GetCurrentLanguageCode();
				if (string.IsNullOrEmpty(code)) {
					code = Localization.GetLocale().Code;
				}
				if (code == null)
					code = "";
				string translations_path = Path.Combine(Main.main.path , "translations");
				string[] checklist = new string[]{
					Path.Combine(translations_path , "strings_template.po"),
					Path.Combine(translations_path , code + ".po"),
					Path.Combine(translations_path , code.Substring(0,code.IndexOf('_')) + ".po")
				};
                foreach (var path in checklist) {
					if (File.Exists(path)) {
						STRINGS.Localize(path);
						hierarchyReferences.GetReference<LocText>("Title").text = STRINGS.TITLE;
						hierarchyReferences.GetReference<ToolTip>("Description").toolTip = STRINGS.DESCRIPTION;
						goto done;
					}
                }

				if (code != STRINGS.DEFAULT_LANGUAGE_CODE) {
					string format_code = code.ToLower();
					if (format_code.IndexOf('_') != -1)
						format_code = code.Substring(0 , code.IndexOf('_')).ToLower();

					bool supported = false;
					switch (format_code) {
						case "zh":
							STRINGS.Localize_zh();
							supported = true;
							break;
					}
					if (supported) {
						hierarchyReferences.GetReference<LocText>("Title").text = STRINGS.TITLE;
						hierarchyReferences.GetReference<ToolTip>("Description").toolTip = STRINGS.DESCRIPTION;
					}
					else {
						try {
							if ((!File.Exists(Path.Combine(translations_path , "strings_template.pot"))) || File.GetLastWriteTime(Path.Combine(translations_path , "strings_template.pot")) < File.GetLastWriteTime(Path.Combine(Main.main.path , "TruthfulThermalConductivity.dll"))) {
								using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ONI_Truthful_Thermal_Conductivity.strings_template.pot")) {
									if (stream != null) {
										if(!Directory.Exists(translations_path))
											Directory.CreateDirectory(translations_path);
										using (FileStream file = new FileStream(Path.Combine(translations_path , "strings_template.pot") , FileMode.OpenOrCreate)) {
											stream.CopyTo(file);
										}
									}
								}
							}
						}
						catch { }
					}
				}

			}
			catch { }
		done:
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