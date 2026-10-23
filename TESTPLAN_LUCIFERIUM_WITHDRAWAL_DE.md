# Luciferium-Entzug – Test für RimWorld 1.6

## Auslösung und beabsichtigte Wirkung

Der **zusätzliche** Entzug tritt nur ein, wenn gleichzeitig:
- der Pawn die Hediff `LuciferiumAddiction` besitzt
- die Need `Chemical_Luciferium` auf **0 %** gefallen ist

Die normale Luci-Versorgung ist unverändert: 0,10 Need-Verbrauch je Spieltag (einstellbar 0,10–0,30), eine Dosis für bestehende Süchtige erhöht Need normalerweise um 0,90. Eine Dosis bedeutet bei 0,10 Bedarf/Tag ungefähr 9 Tage Versorgung. Die Vanilla-Entzugsstufe ist ausdrücklich erhalten.

Bei Need = 0 greifen zusätzlich zur bestehenden Vanilla-Entzugsstufe:

1. Situativer Gedanke **Schwerer Luciferium-Entzug**, standardmäßig −60 Stimmung (Mod-Einstellung −100 bis 0).
2. Körperkapazität Bewegung: zusätzlicher Offset −0,50.
3. Körperkapazität Manipulation: zusätzlicher Offset −0,35.
4. Schmerzfaktor: ×1,5 (zusätzlich zum Vanilla-Schmerzoffset).

Der bestehende Vanilla-Berserker- und Todesmechanismus wird **nicht** verstärkt.

## Konkreter In-Game-Test

1. Separaten Testspielstand anlegen und das aktuelle Build installieren.
2. Einem gesunden Testpawn eine Luciferium-Dosis geben; die Abhängigkeit muss bestehen.
3. Die Need vorerst über 0 % lassen: Der neue Entzugs-Gedanke darf **nicht** sichtbar sein, Bewegung und Manipulation dürfen keinen negativen Zusatz durch unser XML haben.
4. Die Need im Entwickler-Modus auf 0 % setzen (oder Spielzeit vergehen lassen): **Schwerer Luciferium-Entzug** soll sofort in der Stimmung erscheinen (−60 voreingestellt).
5. Prüfen: Bewegung −50 %, Manipulation −35 %, Schmerz ×1,5 **zusätzlich zur bestehenden Vanilla-Entzugsstufe**. Die genaue Endkapazität ist durch andere Hediffs, Gesundheit und Kapazitätsgrenzen abhängig.
6. Luciferium erneut verabreichen: Need steigt. Der neue Stimmungsabzug und die zusätzlichen Kapazitätsabzüge sollen ohne bleibenden Debuff verschwinden.
7. Im Einstellungsmenü neuen Stimmungsabzug auf z. B. −30 setzen und bei erneutem Need = 0 prüfen.
8. Spiel speichern, neu laden; Standard-/geänderter Wert muss erhalten bleiben.

## Sicherheit und Einschränkungen

- Fehlende Dosis allein löst *nicht* sofort den Entzug aus: maßgeblich ist Need = 0.
- Wer Entzug testet, riskiert weiterhin die normale Luciferium-Todesmechanik. **Nur mit Testspielstand arbeiten.**
- GitHub Actions stellt sicher, dass die XML-Dateien wohlgeformt sind und C# kompiliert. Es ersetzt keinen In-Game-Test.
- Diese Mod fügt weder eine neue Todeschance noch Gen-/Xenokeim-Manipulation hinzu.
