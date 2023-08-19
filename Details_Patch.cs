using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SuperMinerMod
{
	[HarmonyPatch(typeof(AdditionalDetailsPanel) , "RefreshDetails")]
	public class Details_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
		{
			List<CodeInstruction> code = instr.ToList();
			foreach (CodeInstruction codeInstruction in code)
			{
				if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == "0.000")
					codeInstruction.operand = "0.000################################################################################################################################################################################################################################################################################################################################################";
				yield return codeInstruction;
			}
		}
	}
}