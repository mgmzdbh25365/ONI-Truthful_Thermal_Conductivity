using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace ONI_Truthful_Thermal_Conductivity {
	public static class Tips_Patch {
		public static string format = "(DTU/(m*s))/";
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr) {
			List<CodeInstruction> code = instr.ToList();
			foreach (CodeInstruction codeInstruction in code) {
				if (codeInstruction.opcode == OpCodes.Ldstr && ((string)codeInstruction.operand).Contains("(DTU/(m*s))/")) {
					codeInstruction.operand = ((string)codeInstruction.operand).Replace("(DTU/(m*s))/" , format);
				}
				yield return codeInstruction;
			}
		}
	}
}