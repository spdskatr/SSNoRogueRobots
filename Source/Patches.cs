using Verse;
using RimWorld.BaseGen;
using Harmony;
using System.Reflection;
using RimWorld;
/* Below lies the usings for the transpiler to work
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
*/
namespace NoRogueRobots
{
    [StaticConstructorOnStartup, HarmonyPatch(typeof(Building_CrashedShipPart), "<TrySpawnMechanoids>m__49F")]
    public static class Patches
    {
        static Patches()
        {
            HarmonyInstance.Create("com.spdskatr.NoRogueRobots.patches").PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("SS No Rogue Robots initialized. Patched:\nBuilding_CrashedShipPart.<TrySpawnMechanoids>m__4A1 (Non-destructive postfix)\nSymbolResolver_RandomMechanoidGroup.<Resolve>m__270 (Non-destructive postfix)\n\n");
        }
        static void Postfix(PawnKindDef def, ref bool __result)
        {
            if (ShouldNotBelong(def)) __result = false;
        }
        /*That was a waste of brainpower VV
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = instructions.ToList();
            bool first = true;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Brfalse && first)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patches).GetMethod("ShouldNotBelong"));
                    yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);//Consumes bool, jumps to another il line which pushes false again
                    first = false;
                }
            }
        }
        */
        public static bool ShouldNotBelong(PawnKindDef def)
        {
            return def.defaultFactionType != Faction.OfMechanoids.def;
        }
    }
    [HarmonyPatch(typeof(SymbolResolver_RandomMechanoidGroup), "<Resolve>m__26E")]
    public static class Patches_2
    {
        static void Postfix(PawnKindDef kind, ref bool __result)
        {
            if (Patches.ShouldNotBelong(kind)) __result = false;
        }
    }
}
