using Verse;
using RimWorld.BaseGen;
using Harmony;
using System.Reflection;
using RimWorld;
using System;
using System.Runtime.CompilerServices;
/* Below lies the usings for the transpiler to work
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
*/
namespace NoRogueRobots
{
    [StaticConstructorOnStartup, HarmonyPatch]
    public static class Patches
    {
        public const BindingFlags compilerGeneratedBindingFlags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        static Patches()
        {
            Log.Message("SS No Rogue Robots :: Starting patches...");
            HarmonyInstance.Create("com.spdskatr.NoRogueRobots.patches").PatchAll(Assembly.GetExecutingAssembly());
        }
        // Thanks to erdelf for that quick tip about compiler generated methods
        static MethodInfo TargetMethod()
        {
            var methods = typeof(CompSpawnerMechanoidsOnDamaged).GetMethods(compilerGeneratedBindingFlags);
            foreach (var method in methods)
            {
                if (method.HasAttribute<CompilerGeneratedAttribute>() && method.GetParameters()[0].ParameterType == typeof(PawnKindDef))
                {
                    Log.Message($"SS No Rogue Robots :: (PATCH 1) Patched compiler generated method {method.DeclaringType}.{method.Name}");
                    return method;
                }
            }
            throw new Exception("SS No Rogue Robots :: Method not found for patch 1 in typeof(Building_CrashedShipPart)");
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
    [HarmonyPatch]
    public static class Patches_2
    {
        static MethodInfo TargetMethod()
        {
            var methods = typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(Patches.compilerGeneratedBindingFlags);
            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(bool) && method.HasAttribute<CompilerGeneratedAttribute>())
                {
                    Log.Message($"SS No Rogue Robots :: (PATCH 2) Patched compiler generated method {method.DeclaringType}.{method.Name}");
                    return method;
                }
            }
            throw new Exception("SS No Rogue Robots :: Method not found for patch 2 in typeof(SymbolResolver_RandomMechanoidGroup)");
        }
        static void Postfix(PawnKindDef kind, ref bool __result)
        {
            if (Patches.ShouldNotBelong(kind)) __result = false;
        }
    }
}
