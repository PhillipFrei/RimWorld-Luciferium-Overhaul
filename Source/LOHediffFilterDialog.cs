using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LuciferiumOverhaul
{
    // Independently implemented per-Hediff filter, inspired by the usability
    // concept of Deadpool's Healing Factor; no third-party source copied.
    public sealed class LOHediffFilterDialog : Window
    {
        private readonly LOSettings settings;
        private string search = "";
        private Vector2 scrollPosition;

        public override Vector2 InitialSize => new Vector2(790f, 680f);

        public LOHediffFilterDialog(LOSettings settings)
        {
            this.settings = settings;
            doCloseX = true;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 25f),
                "Major Healing: Select Health Conditions");
            Widgets.Label(new Rect(rect.x, rect.y + 28f, rect.width, 45f),
                "Auto = safe default rules; Always = include; Never = exclude. " +
                "Gene recovery, pregnancy, missing body parts and Luciferium addiction always remain protected.");
            Widgets.Label(new Rect(rect.x, rect.y + 77f, 68f, 27f), "Search:");
            search = Widgets.TextField(new Rect(rect.x + 70f, rect.y + 77f, rect.width - 70f, 27f), search);

            var visible = DefDatabase<HediffDef>.AllDefsListForReading
                .Where(d => d != null && d.defName != null &&
                    (string.IsNullOrEmpty(search) ||
                     d.defName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                     (d.label != null && d.label.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)))
                .OrderBy(d => d.label ?? d.defName)
                .ToList();

            var viewport = new Rect(rect.x, rect.y + 109f, rect.width, rect.height - 151f);
            float rowHeight = 29f;
            var contentRect = new Rect(0f, 0f, viewport.width - 22f, visible.Count * rowHeight);
            Widgets.BeginScrollView(viewport, ref scrollPosition, contentRect);
            int first = Math.Max(0, (int)(scrollPosition.y / rowHeight) - 1);
            int last = Math.Min(visible.Count, first + (int)(viewport.height / rowHeight) + 4);
            for (int i = first; i < last; i++)
            {
                var def = visible[i];
                var line = new Rect(0f, i * rowHeight, contentRect.width, rowHeight - 2f);
                string n = def.defName;
                bool protectedState = LOHealingComp.IsProtectedGeneticState(n)
                    || n.Equals("LuciferiumAddiction", StringComparison.OrdinalIgnoreCase)
                    || n.Equals("LuciferiumHigh", StringComparison.OrdinalIgnoreCase)
                    || n.IndexOf("Pregnan", StringComparison.OrdinalIgnoreCase) >= 0;

                Widgets.Label(new Rect(line.x + 5f, line.y + 2f, line.width - 140f, 24f),
                    (def.label ?? n) + " [" + n + "]");
                var button = new Rect(line.xMax - 126f, line.y, 123f, 26f);
                if (protectedState)
                {
                    Widgets.Label(button, "Protected");
                    continue;
                }

                bool blocked = settings.blockedMajorHediffs.Contains(n);
                bool forced = settings.forcedMajorHediffs.Contains(n);
                string state = blocked ? "Never" : forced ? "Always" : "Auto";
                if (Widgets.ButtonText(button, state))
                {
                    settings.blockedMajorHediffs.Remove(n);
                    settings.forcedMajorHediffs.Remove(n);
                    if (!blocked && !forced)
                        settings.forcedMajorHediffs.Add(n);
                    else if (forced)
                        settings.blockedMajorHediffs.Add(n);
                }
            }
            Widgets.EndScrollView();

            if (Widgets.ButtonText(new Rect(rect.xMax - 160f, rect.yMax - 34f, 160f, 30f), "Close"))
                Close();
        }

        public override void PostClose()
        {
            base.PostClose();
            var mod = LoadedModManager.GetMod<LOMod>();
            if (mod != null) mod.WriteSettings();
        }
    }
}
