using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LuciferiumOverhaul
{
    public class LOHealingProperties : HediffCompProperties
    {
        public LOHealingProperties() { compClass = typeof(LOHealingComp); }
    }

    public class LOHealingComp : HediffComp
    {
        private int nextSmallTick;
        private int nextMajorTick;
        private int nextAgeTick;

        private static LOSettings Settings => LoadedModManager.GetMod<LOMod>().GetSettings<LOSettings>();

        private static readonly HashSet<string> SafeTemporaryConditions = new HashSet<string>(StringComparer.Ordinal)
        {
            "ToxicBuildup", "FoodPoisoning", "Heatstroke",
            "Hypothermia", "BloodLoss", "Malnutrition"
        };

        // Genetic implantation, gene extraction and their recovery timers are deliberately
        // outside the scope of the Luciferium overhaul.
        private static bool IsProtectedGeneticState(string name)
        {
            return name.StartsWith("Xenogerm", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("GeneRegrow", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("GenesRegrow", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("GeneExtract", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("GeneRecover", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EligibleMajorHealing(Hediff h)
        {
            if (h?.def == null) return false;
            string n = h.def.defName ?? "";
            if (n.Equals("LuciferiumAddiction", StringComparison.OrdinalIgnoreCase)
                || n.Equals("LuciferiumHigh", StringComparison.OrdinalIgnoreCase)
                || IsProtectedGeneticState(n))
                return false;

            if (h is Hediff_MissingPart || h is Hediff_AddedPart)
                return false;

            // Explicit fallbacks support addiction defs that do not inherit Hediff_Addiction.
            // Do not remove gene-related dependencies or arbitrary positive Hediffs.
            bool isOtherAddiction = n.EndsWith("Addiction", StringComparison.OrdinalIgnoreCase);
            bool isDrugTolerance = n.EndsWith("Tolerance", StringComparison.OrdinalIgnoreCase);
            // BloodRot and many Royalty/modded illnesses are tendable HediffWithComps
            // without a HediffComp_Immunizable. Explicitly include treatable bad states.
            bool isTreatableIllness = h.def.isBad && h.def.tendable;
            return h is Hediff_Injury
                || h is Hediff_Addiction
                || isOtherAddiction
                || isDrugTolerance
                || isTreatableIllness
                || h.TryGetComp<HediffComp_Immunizable>() != null
                || h.def.chronic
                || SafeTemporaryConditions.Contains(n);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            ResetTimers();
        }

        private void ResetTimers()
        {
            int now = Find.TickManager?.TicksGame ?? 0;
            nextSmallTick = now + Settings.smallHealingHours * GenDate.TicksPerHour;
            nextMajorTick = now + Settings.majorHealingDays * GenDate.TicksPerDay;
            nextAgeTick = now + GenDate.TicksPerDay;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn == null || Pawn.Dead || Find.TickManager == null) return;
            int now = Find.TickManager.TicksGame;
            if (nextSmallTick <= 0 || nextMajorTick <= 0 || nextAgeTick <= 0) ResetTimers();

            if (now >= nextSmallTick)
            {
                HealOneSmallInjury();
                nextSmallTick = now + Settings.smallHealingHours * GenDate.TicksPerHour;
            }
            if (now >= nextMajorTick)
            {
                HealOneMajorHediff();
                nextMajorTick = now + Settings.majorHealingDays * GenDate.TicksPerDay;
            }
            if (now >= nextAgeTick)
            {
                Rejuvenate();
                nextAgeTick = now + GenDate.TicksPerDay;
            }
        }

        private void HealOneSmallInjury()
        {
            var candidates = new List<Hediff>();
            foreach (Hediff h in Pawn.health.hediffSet.hediffs)
                if (h is Hediff_Injury && !h.IsPermanent()
                    && h.Severity > 0f && h.Severity <= Settings.smallInjuryMaxSeverity)
                    candidates.Add(h);

            if (candidates.TryRandomElement(out Hediff selected))
                Pawn.health.RemoveHediff(selected);
        }

        private void HealOneMajorHediff()
        {
            var candidates = Pawn.health.hediffSet.hediffs.Where(EligibleMajorHealing).ToList();
            if (Prefs.DevMode)
                Log.Message("[Luciferium Overhaul] Major healing for " + Pawn.LabelShort
                    + ": eligible=" + string.Join(", ", candidates.Select(h => h.def.defName).ToArray()));

            if (candidates.TryRandomElement(out Hediff selected))
            {
                if (Prefs.DevMode)
                    Log.Message("[Luciferium Overhaul] Major healing removed: " + selected.def.defName);
                Pawn.health.RemoveHediff(selected);
            }
        }

        private void Rejuvenate()
        {
            if (!Pawn.RaceProps.Humanlike || Pawn.ageTracker == null) return;
            long target = (long)Settings.targetAgeYears * GenDate.TicksPerYear;
            long age = Pawn.ageTracker.AgeBiologicalTicks;
            if (age > target)
                Pawn.ageTracker.AgeBiologicalTicks = Math.Max(target, age - (long)Settings.rejuvenationYearsPerDay * GenDate.TicksPerYear);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref nextSmallTick, "LO_nextSmallTick", 0);
            Scribe_Values.Look(ref nextMajorTick, "LO_nextMajorTick", 0);
            Scribe_Values.Look(ref nextAgeTick, "LO_nextAgeTick", 0);
        }
    }
}
