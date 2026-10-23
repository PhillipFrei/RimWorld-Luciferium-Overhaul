# Erster Spieltest – RimWorld 1.6.4871 (2026-10-23)

## Vom Tester beobachtet
- Mod-Einstellungen öffnen und werden angezeigt.
- Drogenlabor: alle vier Rezepte (1x und 10x) erscheinen und funktionieren.
- Glitterworld-Behandlung: Wundstillung, erhöhte Wundheilung und Immunitätsgeschwindigkeit wurden beobachtet.
- Narben verschwanden, aber **Deadpool's Healing Factor war ebenfalls aktiv**. Dies beweist **nicht**, dass Glitzerwelt-Medizin die Narben entfernt hat.
- Ambrosia-Abhängigkeit verschwand während des Tests nicht; unbekannt, ob ein großer Heilungszyklus ablief und wie viele andere geeignete Hediffs vorhanden waren.

## Auswertung des Player.log
- RimWorld 1.6.4871 rev597.
- `[Luciferium Overhaul] Patched 1 tending method(s).` beim Start.
- Keine direkt Luciferium Overhaul zuordenbare Exception im vorliegenden Log.
- Deadpool's Healing Factor aktiv, ebenso Faster Gene Coma Recovery und Faster Genes Regrowing.
- `Reduced Xenogerm Replication time to 600000 Ticks.` stammt nicht erkennbar vom Luciferium Overhaul.
- Andere Modfehler bestehen unabhängig davon (u. a. Realistic Planets, Archotech Race, Integrated Implants, HediffComp_Invisibility).

## Beschlossene Änderungen
- Luciferium Overhaul **entfernt keine Xenokeim-/Genregenerations-Hediffs mehr gezielt**.
- Keine Veränderung der Genextraktion oder Genimplantation.
- Standardmäßig andere Sucht-Hediffs einschließlich `AmbrosiaAddiction` als Kandidaten erkennen. Luciferium-Abhängigkeit bleibt geschützt.
- Bei aktiviertem Entwicklermodus: Vor der großen Heilung Kandidaten und entfernten Hediff protokollieren.

## Nächster isolierter Test
1. Entwickler-Modus aktivieren; neuen Testpawn mit Luciferium und **AmbrosiaAddiction** erstellen.
2. Große Heilung für Testzwecke auf 1 Tag einstellen. Ein bereits laufender Timer ändert sich derzeit nicht unmittelbar mit dem Slider; daher frischen Pawn benutzen oder den vorherigen Intervallablauf abwarten.
3. Einen vollen Spieltag durchlaufen lassen, nach `[Luciferium Overhaul] Major healing` im Player.log suchen. Entfernte Hediffs stehen dort, wenn der Code aktiviert ist.
4. Narben-Heilung in getrennten Tests ohne Deadpool's Healing Factor prüfen (nur bei separat gesicherter Modliste).
5. Glitzerwelt-Medizin allein mit einer Wunde und ohne Luciferium auf dem Pawn testen, um die Effekte zu isolieren.

**Hinweis:** Logeinträge belegen nur ausgeführte Codepfade und keine vollständige In-Game-Verifikation.
