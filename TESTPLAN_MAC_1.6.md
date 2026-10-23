# Mac test plan — RimWorld 1.6

**Experimental test build only.** Preserve a backup of all existing saves. Never disable older mods in an existing save without testing migration.

## Install
1. GitHub > Actions > latest successful *RimWorld 1.6 build* > Artifacts > `LuciferiumOverhaul-RimWorld-1.6`.
2. Unzip, place `LuciferiumOverhaul` (contains About, Assemblies, Defs, Patches) into `~/Library/Application Support/RimWorld/Mods/` (if it does not exist, create it). If RimWorld uses a different local Mods folder, open it via the game's Mod menu.
3. For the isolated test, use a new save, keep Harmony installed, **deactivate competing Luciferium/research recipe mods only in a separate test mod configuration**.
4. Enable RimWorld's development mode and check red errors at startup.

## Test cases
- Confirm the mod is marked compatible with game 1.6 and Harmony loaded before it.
- Open the drug lab bill menu. Four new recipes present with exact 1x and 10x quantities. Check research requirements.
- Give a pawn Luciferium and inspect stats: pain x0.5, consciousness +0.5, movement +0.5, sight +0.5, blood filtration +1.0, blood pumping +0.5, metabolism +0.5, breathing +0.5.
- Check effective mood +30 and changing the mood setting; need fall 0.10 to 0.30; save/load.
- In test combat, verify only one *eligible* injury heals per 4 hours. Check a 6-severity injury is not fast healed with 5 threshold.
- Test major healing, addiction preservation, missing part preservation, 1-year/day rejuvenation, saved timers.
- Tend with **ordinary medicine** and confirm no glitterworld temporary effect.
- Tend with **glitterworld medicine** and inspect 12-hour Hediff, natural healing and immunity rates.
- Check all bleeding injuries actually become and remain nonbleeding: **not validated by compile**.
- Test tox buildup and food poisoning on a pawn who can normally be tended. Repeated treatments must refresh, not stack, the 12h bonus.

## Known limits
- The critical runtime `TendUtility.DoTend` hook is untested. A successful compile doesn't prove the patch found the correct method or the correct medicine argument.
- The 12-hour recovery is defined for 30,000 ticks (2,500 ticks/hour).
- Regrowing genes extraction cooldown is **not** modified.
- Generic modded Hediff removal is **not** implemented.
- Luciferium mood thought display and all XML patch paths still require real game validation.
- Current bleeding stop is a tentative re-tend, **not** a guaranteed permanent hemostasis mechanic.
- A standard treatment cannot be invoked if vanilla RimWorld offers no valid tending action.

## Troubleshooting
On macOS, logs are commonly in `~/Library/Logs/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`.
Send the earliest red error, the log, the RimWorld version and whether each test condition passed.
