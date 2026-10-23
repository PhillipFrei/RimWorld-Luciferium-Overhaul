using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace LuciferiumOverhaul
{
    public class LOSettings : ModSettings
    {
        public const int DefaultMoodBonus = 30;
        public const int DefaultWithdrawalMood = -60;
        public const float DefaultNeedFallPerDay = 0.10f;
        public const int DefaultSmallHealingHours = 4;
        public const int DefaultMajorHealingDays = 3;
        public const float DefaultSmallInjuryMaxSeverity = 5f;
        public const int DefaultTargetAgeYears = 21;
        public const int DefaultRejuvenationYearsPerDay = 1;

        public List<string> blockedMajorHediffs = new List<string>();
        public List<string> forcedMajorHediffs = new List<string>();
        public int moodBonus = DefaultMoodBonus;
        public int severeWithdrawalMood = DefaultWithdrawalMood;
        public float luciferiumFallPerDay = DefaultNeedFallPerDay;
        public int smallHealingHours = DefaultSmallHealingHours;
        public int majorHealingDays = DefaultMajorHealingDays;
        public float smallInjuryMaxSeverity = DefaultSmallInjuryMaxSeverity;
        public int targetAgeYears = DefaultTargetAgeYears;
        public int rejuvenationYearsPerDay = DefaultRejuvenationYearsPerDay;

        public void ResetToDefaults()
        {
            moodBonus = DefaultMoodBonus;
            severeWithdrawalMood = DefaultWithdrawalMood;
            luciferiumFallPerDay = DefaultNeedFallPerDay;
            smallHealingHours = DefaultSmallHealingHours;
            majorHealingDays = DefaultMajorHealingDays;
            smallInjuryMaxSeverity = DefaultSmallInjuryMaxSeverity;
            targetAgeYears = DefaultTargetAgeYears;
            rejuvenationYearsPerDay = DefaultRejuvenationYearsPerDay;
            blockedMajorHediffs = new List<string>();
            forcedMajorHediffs = new List<string>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref blockedMajorHediffs, "blockedMajorHediffs", LookMode.Value);
            Scribe_Collections.Look(ref forcedMajorHediffs, "forcedMajorHediffs", LookMode.Value);
            if (blockedMajorHediffs == null) blockedMajorHediffs = new List<string>();
            if (forcedMajorHediffs == null) forcedMajorHediffs = new List<string>();
            Scribe_Values.Look(ref moodBonus, "moodBonus", DefaultMoodBonus);
            Scribe_Values.Look(ref severeWithdrawalMood, "severeWithdrawalMood", DefaultWithdrawalMood);
            Scribe_Values.Look(ref luciferiumFallPerDay, "luciferiumFallPerDay", DefaultNeedFallPerDay);
            Scribe_Values.Look(ref smallHealingHours, "smallHealingHours", DefaultSmallHealingHours);
            Scribe_Values.Look(ref majorHealingDays, "majorHealingDays", DefaultMajorHealingDays);
            Scribe_Values.Look(ref smallInjuryMaxSeverity, "smallInjuryMaxSeverity", DefaultSmallInjuryMaxSeverity);
            Scribe_Values.Look(ref targetAgeYears, "targetAgeYears", DefaultTargetAgeYears);
            Scribe_Values.Look(ref rejuvenationYearsPerDay, "rejuvenationYearsPerDay", DefaultRejuvenationYearsPerDay);
        }
    }

    public class LOMod : Mod
    {
        private readonly LOSettings settings;
        private Vector2 settingsScrollPosition;

        public LOMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<LOSettings>();
            LongEventHandler.ExecuteWhenFinished(ApplyRuntimeSettings);
        }

        private void ApplyRuntimeSettings()
        {
            ApplyMood();
            NeedDef need = DefDatabase<NeedDef>.GetNamedSilentFail("Chemical_Luciferium");
            if (need != null)
                need.fallPerDay = Mathf.Clamp(settings.luciferiumFallPerDay, 0.10f, 0.30f);
        }

        private void ApplyMood()
        {
            ThoughtDef thought = DefDatabase<ThoughtDef>.GetNamedSilentFail("LO_LuciferiumMood");
            if (thought != null && thought.stages != null && thought.stages.Count > 0)
                thought.stages[0].baseMoodEffect = settings.moodBonus;

            ThoughtDef withdrawal = DefDatabase<ThoughtDef>.GetNamedSilentFail("LO_SevereLuciferiumWithdrawal");
            if (withdrawal != null && withdrawal.stages != null && withdrawal.stages.Count > 0)
                withdrawal.stages[0].baseMoodEffect = settings.severeWithdrawalMood;
        }

        public override string SettingsCategory() => "Litmitless Luciferium";

        public override void DoSettingsWindowContents(Rect rect)
        {
            const float bottomArea = 52f;
            const float contentHeight = 1030f;
            Rect viewRect = new Rect(rect.x, rect.y, rect.width, Math.Max(100f, rect.height - bottomArea));
            Rect contentRect = new Rect(0f, 0f, Math.Max(200f, rect.width - 22f), contentHeight);

            Widgets.BeginScrollView(viewRect, ref settingsScrollPosition, contentRect);
            var list = new Listing_Standard();
            list.Begin(contentRect);

            list.Label("LUCIFERIUM EFFECTS");
            list.Label("Mood modifier: " + settings.moodBonus.ToString("+0;-0;0") + " (default +30)");
            int mood = (int)list.Slider(settings.moodBonus, -100, 100);
            if (mood != settings.moodBonus)
            {
                settings.moodBonus = mood;
                ApplyMood();
            }
            list.Gap();

            list.Label("LUCIFERIUM WITHDRAWAL (when need reaches 0%)");
            list.Label("Additional mood penalty: " + settings.severeWithdrawalMood + " (default -60)");
            int withdrawalMood = (int)list.Slider(settings.severeWithdrawalMood, -100, 0);
            if (withdrawalMood != settings.severeWithdrawalMood)
            {
                settings.severeWithdrawalMood = withdrawalMood;
                ApplyMood();
            }
            list.Label("Also applies: Moving -50%, Manipulation -35%, Pain x1.5.");
            list.Label("The original withdrawal and lethal consequences remain unchanged.");
            list.Gap();

            list.Label("LUCIFERIUM DOSING");
            list.Label("Daily need decay: " + settings.luciferiumFallPerDay.ToString("0.00") + " (default 0.10)");
            float fall = (float)Math.Round(list.Slider(settings.luciferiumFallPerDay, 0.10f, 0.30f), 2);
            if (Math.Abs(fall - settings.luciferiumFallPerDay) > 0.0001f)
            {
                settings.luciferiumFallPerDay = fall;
                ApplyRuntimeSettings();
            }
            list.Label("Estimated time between doses: " + (0.90f / settings.luciferiumFallPerDay).ToString("0.0") + " days");
            list.Gap();

            list.Label("HEALING");
            list.Label("Minor injury healing: every " + settings.smallHealingHours + " hours (default 4)");
            settings.smallHealingHours = (int)list.Slider(settings.smallHealingHours, 1, 4);
            list.Label("Major healing: every " + settings.majorHealingDays + " days (default 3)");
            settings.majorHealingDays = (int)list.Slider(settings.majorHealingDays, 1, 9);
            list.Label("Maximum minor injury severity: " + settings.smallInjuryMaxSeverity.ToString("0") + " (default 5)");
            settings.smallInjuryMaxSeverity = (int)list.Slider(settings.smallInjuryMaxSeverity, 1, 20);
            list.Gap();

            list.Label("BIOLOGICAL AGE");
            list.Label("Target biological age: " + settings.targetAgeYears + " years (default 21)");
            settings.targetAgeYears = (int)list.Slider(settings.targetAgeYears, 18, 100);
            list.Label("Age reversal: 1 biological year per in-game day.");
            list.Gap();

            if (list.ButtonText("Configure Major Healing: Allowed / Blocked Conditions"))
                Find.WindowStack.Add(new LOHediffFilterDialog(settings));
            list.Gap();

            list.Label("GLITTERWORLD MEDICINE");
            list.Label("Medical potency: 200% | Maximum tending quality: 150%.");
            list.Label("After tending: 5x injury healing and 10x immunity gain for 12 hours.");

            list.End();
            Widgets.EndScrollView();

            Rect resetButton = new Rect(rect.xMax - 190f, rect.yMax - 43f, 190f, 35f);
            if (Widgets.ButtonText(resetButton, "Reset to Defaults"))
            {
                settings.ResetToDefaults();
                settingsScrollPosition = Vector2.zero;
                WriteSettings();
                Messages.Message("Litmitless Luciferium: Default settings restored.", MessageTypeDefOf.NeutralEvent, false);
            }
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            ApplyRuntimeSettings();
        }
    }
}
