#!/usr/bin/env python3
"""Static verification of RimWorld 1.6 mod content (not a game runtime test)."""
from pathlib import Path
import xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[1]
xmls=list(root.glob("About/*.xml"))+list(root.glob("Defs/**/*.xml"))+list(root.glob("Patches/**/*.xml"))
for file in xmls:
    ET.parse(file)
assert xmls, "No XML files"
rec=ET.parse(root/"Defs/RecipeDefs/LO_Recipes.xml").getroot()
expected={
 "LO_MakeLuciferium":({"MedicineHerbal":2,"PsychoidLeaves":10,"Ambrosia":1},"Luciferium",1),
 "LO_MakeLuciferium10":({"MedicineHerbal":20,"PsychoidLeaves":100,"Ambrosia":10},"Luciferium",10),
 "LO_MakeGlitterworldMedicine":({"MedicineIndustrial":1,"Luciferium":1},"MedicineUltratech",1),
 "LO_MakeGlitterworldMedicine10":({"MedicineIndustrial":10,"Luciferium":10},"MedicineUltratech",10),
}
found={}
for recipe in rec.findall("RecipeDef"):
    name=recipe.findtext("defName")
    ingred={}
    for li in recipe.findall("./ingredients/li"):
        thing=li.findtext("./filter/thingDefs/li")
        ingred[thing]=int(li.findtext("count"))
    prod=recipe.find("./products")[0]
    found[name]=(ingred,prod.tag,int(prod.text))
assert found==expected,(found,expected)
about=ET.parse(root/"About/About.xml").getroot()
assert about.findtext("./supportedVersions/li")=="1.6"
recovery=ET.parse(root/"Defs/HediffDefs/LO_GlitterworldRecovery.xml").getroot().find("HediffDef")
assert recovery.findtext("./comps/li/disappearsAfterTicks")=="30000"
assert recovery.findtext("./stages/li/statFactors/InjuryHealingFactor")=="5"
assert recovery.findtext("./stages/li/statFactors/ImmunityGainSpeed")=="10"
print(f"PASS: {len(xmls)} XML files well formed; 4 recipes correct; 1.6 metadata and 12h effect checked")

# English in-game labels and hard-reset defaults.
assert about.findtext("./name") == "Litmitless Luciferium"
for file in root.glob("Defs/**/*.xml"):
    node=ET.parse(file).getroot()
    for tag in ("label", "description", "jobString"):
        for element in node.iter(tag):
            value=element.text or ""
            assert not any(word in value for word in
                ("herstellen", "Stelle ", "Glitzerwelt-", "Heilung", "Schwerer ", "Körper", "Psyche", "Behandlung mit")), (file,tag,value)
cs=(root/"Source/LuciferiumOverhaul.cs").read_text()
assert 'SettingsCategory() => "Litmitless Luciferium"' in cs
assert 'Widgets.ButtonText(resetButton, "Reset to Defaults")' in cs
assert 'settings.ResetToDefaults();' in cs
assert 'WriteSettings();' in cs
for field,default in (
    ("DefaultMoodBonus", "30"),
    ("DefaultWithdrawalMood", "-60"),
    ("DefaultNeedFallPerDay", "0.10f"),
    ("DefaultSmallHealingHours", "4"),
    ("DefaultMajorHealingDays", "3"),
    ("DefaultSmallInjuryMaxSeverity", "5f"),
    ("DefaultTargetAgeYears", "21"),
    ("DefaultRejuvenationYearsPerDay", "1"),
):
    assert f" {field} = {default};" in cs, field
reset=cs.split("public void ResetToDefaults()",1)[1].split("public override void ExposeData()",1)[0]
for value in ("moodBonus", "severeWithdrawalMood", "luciferiumFallPerDay", "smallHealingHours", "majorHealingDays", "smallInjuryMaxSeverity", "targetAgeYears", "rejuvenationYearsPerDay", "blockedMajorHediffs", "forcedMajorHediffs"):
    assert value in reset, value
print("PASS: English game text, mod name, reset controls and all agreed defaults")
