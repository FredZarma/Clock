# Clock

Horloge numérique de bureau pour Windows, gratuite.

Petit gadget toujours visible, déplaçable comme une icône, avec affichage LED 7 segments, date, calendrier, alarme et thèmes.

**Copyright Fred Zarma 2026** — zarma@sylm.info

## Téléchargement

La dernière version se trouve dans les [Releases](../../releases) : fichier `Clock.exe`.

Aucun installeur. Double-clic pour lancer. Copiez l’exe où vous voulez (bureau, etc.).

Windows 10 ou 11, 64 bits. Nécessite .NET Framework 4 (déjà présent sur Windows).

Au premier lancement, Windows SmartScreen peut afficher un avertissement (application non signée). Choisissez *Informations complémentaires* puis *Exécuter quand même*.

## Utilisation

| Action | Effet |
| --- | --- |
| Glisser | Déplacer (aligné sur la grille des icônes) |
| Double-clic | Calendrier |
| Clic droit | Menu |

Dans le menu : drapeaux de langue (anglais, mandarin, hindi, espagnol, français, russe, japonais, allemand), calendrier, copier l’heure, taille, couleur, secondes, date, format 12/24 h, alarme, nom de l’icône (modifier ou masquer), toujours visible, lancer au démarrage, copyright (zarma@sylm.info).

## Compiler

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Build.ps1
```

## Prochaine version

Voir `NEXT.md` : installeur (copie dans Program Files + raccourci bureau) et désinstallation propre.

## Licence

Gratuit, licence MIT. Voir `LICENSE`.
