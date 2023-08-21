using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using ONI_Truthful_Thermal_Conductivity;

namespace SuperMinerMod {
	[HarmonyPatch(typeof(AdditionalDetailsPanel) , "RefreshDetails")]
	public class Details_Patch {
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr) {
			bool isThermalConductivity = false;
			List<CodeInstruction> code = instr.ToList();
			foreach (CodeInstruction codeInstruction in code) {
				if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == "0.000") {
					if (isThermalConductivity)
						codeInstruction.operand = Options.format;
					else
						isThermalConductivity = true;
				}
				yield return codeInstruction;
			}
		}
	}
}