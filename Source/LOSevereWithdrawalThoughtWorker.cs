using RimWorld;
using Verse;

namespace LuciferiumOverhaul
{
    /// <summary>
    /// This is a situational thought, never a permanent mood memory.
    /// It activates only while an addicted pawn's chemical need has reached zero.
    /// A new dose replenishes the need and immediately disables this thought.
    /// </summary>
    public class LOSevereWithdrawalThoughtWorker : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            if (pawn?.health?.hediffSet == null || pawn.needs == null)
                return ThoughtState.Inactive;

            HediffDef addictionDef = DefDatabase<HediffDef>.GetNamedSilentFail("LuciferiumAddiction");
            NeedDef needDef = DefDatabase<NeedDef>.GetNamedSilentFail("Chemical_Luciferium");
            if (addictionDef == null || needDef == null ||
                pawn.health.hediffSet.GetFirstHediffOfDef(addictionDef) == null)
                return ThoughtState.Inactive;

            Need need = pawn.needs.TryGetNeed(needDef);
            return need != null && need.CurLevel <= 0.0001f
                ? ThoughtState.ActiveDefault
                : ThoughtState.Inactive;
        }
    }
}
