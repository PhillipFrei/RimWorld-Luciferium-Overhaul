using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace LuciferiumOverhaul
{
    [StaticConstructorOnStartup]
    public static class LOGlitterworldStartup
    {
        static LOGlitterworldStartup()
        {
            var harmony = new Harmony("local.luciferiumoverhaul.glitterworld");
            var utility = AccessTools.TypeByName("RimWorld.TendUtility");
            if (utility == null) { Log.Error("[Luciferium Overhaul] TendUtility not found."); return; }
            var tendMethods = utility.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == "DoTend" && m.GetParameters().Count(p => typeof(Pawn).IsAssignableFrom(p.ParameterType)) >= 2
                  && m.GetParameters().Any(p => typeof(Thing).IsAssignableFrom(p.ParameterType))).ToList();
            foreach (var method in tendMethods)
                harmony.Patch(method, postfix: new HarmonyMethod(typeof(LOGlitterworldStartup), nameof(TendPostfix)));
            Log.Message("[Luciferium Overhaul] Patched " + tendMethods.Count + " tending method(s).");
        }

        // __args avoids binding to version-specific parameter names.
        public static void TendPostfix(object[] __args)
        {
            try
            {
                var pawns = __args.OfType<Pawn>().ToList();
                var medicine = __args.OfType<Thing>().FirstOrDefault(t => t.def?.defName == "MedicineUltratech");
                if (medicine == null || pawns.Count < 2) return;
                var patient = pawns[1];
                if (patient?.health == null || patient.Dead) return;

                // Refresh the effect rather than allowing multiple copies to stack.
                var effectDef = DefDatabase<HediffDef>.GetNamedSilentFail("LO_GlitterworldRecovery");
                if (effectDef != null)
                {
                    var existing = patient.health.hediffSet.GetFirstHediffOfDef(effectDef);
                    if (existing != null) patient.health.RemoveHediff(existing);
                    patient.health.AddHediff(effectDef);
                }

                // Tend every bleeding injury. Do not delete injuries or body parts.
                // Reflective invocation also supports patch-compatible overload changes.
                var tend = AccessTools.Method(typeof(Hediff), "Tended", new[] { typeof(float), typeof(float), typeof(int) });
                if (tend != null)
                {
                    foreach (var injury in patient.health.hediffSet.hediffs.OfType<Hediff_Injury>().ToList())
                    {
                        if (injury.BleedRate > 0f)
                            tend.Invoke(injury, new object[] { 1.5f, 1.5f, 0 });
                    }
                }

                // Restrict instant cleansing to specifically identified poison/toxicity conditions.
                foreach (var h in patient.health.hediffSet.hediffs.ToList())
                {
                    string name = h.def?.defName;
                    if (name == "ToxicBuildup" || name == "FoodPoisoning")
                        patient.health.RemoveHediff(h);
                }
            }
            catch (Exception ex) { Log.Error("[Luciferium Overhaul] Glitterworld treatment failed: " + ex); }
        }
    }
}
