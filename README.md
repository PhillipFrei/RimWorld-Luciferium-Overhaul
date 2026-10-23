# Litmitless Luciferium — RimWorld 1.6

An enhanced Luciferium overhaul featuring powerful body capacity modifiers, configurable need decay, fast and major healing cycles, biological age reversal, intensified withdrawal, new crafting recipes, and enhanced glitterworld medicine.

## Default settings

| Setting | Default |
| --- | --- |
| Luciferium mood modifier | +30 (adjustable -100 to +100) |
| Luciferium need decay | 0.10 per day (adjustable 0.10–0.30, roughly 9 days per 90% refill) |
| Additional withdrawal mood penalty | -60 (adjustable -100 to 0; active at zero need) |
| Withdrawal movement modifier | -0.50 |
| Withdrawal manipulation modifier | -0.35 |
| Withdrawal pain factor | x1.5 |
| Minor healing interval | 4 in-game hours (adjustable 1–4) |
| Minor injury maximum severity | 5 (adjustable 1–20) |
| Major healing interval | 3 in-game days (adjustable 1–9) |
| Biological age reversal | 1 biological year per in-game day |
| Target biological age | 21 (adjustable 18–100) |

Use **Reset to Defaults** in the mod settings to restore all sliders and clear custom major-healing allow/block filters.

## Medicine and crafting

Glitterworld medicine increases medical potency to 200% and maximum tending quality to 150%. After a normal glitterworld medicine tend, the patient gets 5x injury healing and 10x immunity gain for 12 in-game hours; repeated treatments refresh rather than stack the bonus. The mod also includes 1x and 10x crafting recipes for Luciferium and glitterworld medicine at the drug lab.

## Installation

Get the artifact from the latest **successful** GitHub Actions build. The contained `LuciferiumOverhaul` folder can replace the previous local mod folder. Keep **only one copy of this mod** installed. Harmony must be enabled first.

Internal assembly name and packageId (`LuciferiumOverhaul.dll`, `local.luciferiumoverhaul`) intentionally remain unchanged for compatibility with existing saves and mod settings.

## Build

GitHub Actions compiles against RimWorld 1.6.4871 reference assemblies from the Ludeon-authorized `Krafs.Rimworld.Ref` package. No original game DLLs are checked in.

As always, back up existing saves before changing your mod list. Compiler success does not replace a real in-game test.
