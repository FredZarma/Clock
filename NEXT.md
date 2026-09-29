# Prochaine version — installation et désinstallation

À ajouter, pas encore implémenté.

## Installation

- Créer un sous-répertoire dans Program Files, par ex. `C:\Program Files\Clock\` (ou `C:\Program Files\Fred Zarma\Clock\`).
- Y copier `Clock.exe` (et l’icône si besoin).
- Créer un raccourci sur le bureau (nom : Clock).
- Idéalement aussi : raccourci menu Démarrer, entrée « Programmes et fonctionnalités » pour désinstaller.

L’installeur demandera probablement les droits administrateur (écriture dans Program Files).

## Désinstallation (propre)

- Supprimer le répertoire d’installation.
- Supprimer le raccourci du bureau.
- Supprimer le raccourci du menu Démarrer.
- Supprimer la clé de désinstallation dans le registre.
- Ne pas laisser de fichiers orphelins. Les réglages utilisateur (`%AppData%\DesktopClock`) : à proposer (conserver ou tout effacer).
