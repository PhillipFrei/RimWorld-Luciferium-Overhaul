using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
namespace LuciferiumOverhaul {
 public class LOHealingProperties : HediffCompProperties { public LOHealingProperties(){ compClass=typeof(LOHealingComp); } }
 public class LOHealingComp : HediffComp {
  private int nextSmallTick, nextMajorTick, nextAgeTick;
  private static LOSettings Settings => LoadedModManager.GetMod<LOMod>().GetSettings<LOSettings>();
  public override void CompPostMake(){base.CompPostMake(); ResetTimers();}
  private void ResetTimers(){int now=Find.TickManager?.TicksGame ?? 0; nextSmallTick=now+Settings.smallHealingHours*GenDate.TicksPerHour; nextMajorTick=now+Settings.majorHealingDays*GenDate.TicksPerDay; nextAgeTick=now+GenDate.TicksPerDay;}
  public override void CompPostTick(ref float severityAdjustment){base.CompPostTick(ref severityAdjustment);if(Pawn==null||Pawn.Dead||Find.TickManager==null)return;int now=Find.TickManager.TicksGame;if(nextSmallTick<=0||nextMajorTick<=0||nextAgeTick<=0)ResetTimers();if(now>=nextSmallTick){HealOneSmallInjury();nextSmallTick=now+Settings.smallHealingHours*GenDate.TicksPerHour;}if(now>=nextMajorTick){HealOneMajorHediff();nextMajorTick=now+Settings.majorHealingDays*GenDate.TicksPerDay;}if(now>=nextAgeTick){Rejuvenate();nextAgeTick=now+GenDate.TicksPerDay;}}
  private void HealOneSmallInjury(){var candidates=new List<Hediff>();foreach(Hediff h in Pawn.health.hediffSet.hediffs){if(h is Hediff_Injury&&!h.IsPermanent()&&h.Severity>0f&&h.Severity<=Settings.smallInjuryMaxSeverity)candidates.Add(h);}if(candidates.TryRandomElement(out Hediff selected))Pawn.health.RemoveHediff(selected);}
  private static readonly HashSet<string> SafeTemporaryConditions = new HashSet<string>(StringComparer.Ordinal) { "ToxicBuildup", "FoodPoisoning", "Heatstroke", "Hypothermia", "BloodLoss", "Malnutrition", "XenogerminationComa" };
  private void HealOneMajorHediff(){var candidates=new List<Hediff>();foreach(Hediff h in Pawn.health.hediffSet.hediffs){if(h.def==null||h.def.defName=="LuciferiumAddiction"||h.def.defName=="LuciferiumHigh")continue;if(h is Hediff_MissingPart||h is Hediff_AddedPart)continue;if(h is Hediff_Injury||h is Hediff_Addiction||h.TryGetComp<HediffComp_Immunizable>() != null||h.def.chronic||SafeTemporaryConditions.Contains(h.def.defName))candidates.Add(h);}if(candidates.TryRandomElement(out Hediff selected))Pawn.health.RemoveHediff(selected);}
  private void Rejuvenate(){if(!Pawn.RaceProps.Humanlike||Pawn.ageTracker==null)return;long target=(long)Settings.targetAgeYears*GenDate.TicksPerYear;long age=Pawn.ageTracker.AgeBiologicalTicks;if(age>target)Pawn.ageTracker.AgeBiologicalTicks=Math.Max(target,age-(long)Settings.rejuvenationYearsPerDay*GenDate.TicksPerYear);}
  public override void CompExposeData(){base.CompExposeData();Scribe_Values.Look(ref nextSmallTick,"LO_nextSmallTick",0);Scribe_Values.Look(ref nextMajorTick,"LO_nextMajorTick",0);Scribe_Values.Look(ref nextAgeTick,"LO_nextAgeTick",0);}
 }
}
