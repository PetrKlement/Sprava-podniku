# Správa podniku
Aplikace slouží k řízení podniku, který vyřezává komponenty z plechů různého typu.
Program je napsán v jazyce C# a XAML. Data jsou uložena v XML souborech.
*********************
# Funkce
Aplikace umožňuje evidovat zaměstnance, naskladněný materiál, zakázky a transakce. Transakce vznikají koupí materiálu, přidáním nebo odebráním peněz na účet a splněním zakázek. 
*****
## Hlavní okno
V něm je uživateli zobrazen přehled o penězích na účtě, počtu zaměstnanců a počtu aktuálních zakázek. Kliknutí na centrální obrázek se otevře okno pro správu firmy. 
Dále je zde možné přidávat a odebítat peníze z účtu. Tím jsou vytvářeny transakce. 
## Okno - Správa firmy
Zde jsou kromě informacích zobrazených v Hlavní okně, také rozepsány seznamy jednotlivích zakázek, zaměstnanců a materiálů.
Na levé straně se nachází odkazy pro okna:
- Zpracování zakázky
- Správa zaměstnanců
- Správa materiálu
Kliknutím na obrázek seznamu se otevře Přehled firmy.
## Okno - Přehled firmy
V levé části si uživatel zvolí jaký typ a v jakém časovém období mají být informace zobrazeny. Poté, když klikne na některý sloupec v grafu, tak se v okně vypíší jednotlivé komponenty, které mu náleží.
Jsou-li zvoleny zaměstnanci, materiál nebo zakázky, graf znázorňuje informace o aktuálních, smazaný a všech.
Jsou-li zvoleny transakce, graf znázorňuje informace o penězích, které byly přidány nebo odebrány z účtu, penězích utracených za materiál a o zisku ze splňených zakázek.
## Okno - Zpracování zakázky 
V levé části je sepsán seznam aktuálních zakázek. Pokud uživatel klikne na některou z nich, vypíšou se podrobnosti o ní. Pravá část slouží ke spočítání nákladů. Po vyčíslení lze zakázku zpracovat.
## Okna - Správa zaměstnanců / zakázek / materiálů
Zde se nachází seznam všech zaměstnanců / zakázek / materiálů. Umožňuje je přidávat i odebírat.
************************************


