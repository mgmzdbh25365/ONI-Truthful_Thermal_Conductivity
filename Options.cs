using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using System.IO;

namespace ONI_Truthful_Thermal_Conductivity {
	[JsonObject(MemberSerialization.OptIn)]
	public class Options : UserMod2 {
		public string[] modeName = new string[]{
			"low",
			"normal",
			"high",
			"strict"
		};
		public static string[] modeName_EN = new string[]{
			"low",
			"normal",
			"high",
			"strict"
		};
		public static string[] modeName_ZH = new string[]{
			"低",
			"标准",
			"高",
			"严格"
		};
		public static string format = "0.000#####";
		public static string[] formats = new string[] {
			"0.000",
			"0.000#####",
			"0.000################################################################################################################################################################################################################################################################################################################################################",
			"r"
		};
		public static Options main = null;
		public static Harmony main_harmony = null;
		public KButton button = null;
		public string tooltip = "Click to switch the accuracy of the thermal conductivity display.";
		[JsonProperty]
		public int mode=1;
		public override void OnLoad(Harmony harmony) {
			main = this;
			main_harmony = harmony;
			base.OnLoad(harmony);

		}
		public static void SwitchMode(KButton kButton) {
			main.mode++;
			if (main.mode >= formats.Length)
				main.mode = 0;
			format = formats[main.mode];
			if (main_harmony != null)
				main_harmony.PatchAll();
			string data = JsonConvert.SerializeObject(main);
			File.WriteAllText(main.path + "\\config.json" , data);
			kButton.GetComponentInChildren<LocText>().text = main.modeName[main.mode];
		}
		public override void OnAllModsLoaded(Harmony harmony , System.Collections.Generic.IReadOnlyList<Mod> mods) {
			ReLoad();
		}
		public void ReLoad() {
			try {
				Options data = JsonConvert.DeserializeObject<Options>(File.ReadAllText(main.path + "\\config.json"));
				if (data != null)
					mode = data.mode;
				if (mode < 0 || mode >= formats.Length)
					mode = 1;
				format = formats[mode];
			}
			catch {
			}
			if (main_harmony != null)
				main_harmony.PatchAll();
		}
	}
}