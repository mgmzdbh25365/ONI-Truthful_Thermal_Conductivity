using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace ONI_Truthful_Thermal_Conductivity {
	public static class ThermalConductivity_Patch {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr) {
			List<CodeInstruction> code = instr.ToList();
			foreach (CodeInstruction codeInstruction in code) {
				if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == "0.000")
					codeInstruction.operand = Main.format;
				yield return codeInstruction;
			}
		}
	}
}