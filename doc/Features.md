# F1. Architecture & Fondations
Mise en place de la solution, des 4 projets, et de l'injection de dépendance pour supporter le mode Hybride (MAUI) et le mode Web (Test).

## US1.1 Création de la Solution et des Projets
Mise en place de la structure de dossier et des références croisées.

**En tant que** Développeur (Toi)
**Je veux** initialiser la solution avec les 4 projets distincts (.NET 9)
**Afin de** séparer proprement la logique, l'UI et l'implémentation native.

**Auteur:** User
**Priorité:** Critique
**Estimation:** 2 points

**Critères d'acceptation:**
[ ] La solution contient `DevToolbar.Core` (ClassLib), `DevToolbar.UI` (RCL), `DevToolbar.Maui` (MAUI) et `DevToolbar.Web` (Blazor Web App).
[ ] `DevToolbar.UI` référence `DevToolbar.Core`.
[ ] `DevToolbar.Maui` et `DevToolbar.Web` référencent `DevToolbar.UI` et `DevToolbar.Core`.
[ ] Le projet Web lance une page Blazor vide sans erreur.

**Fichiers/Projets concernés:**
- 📁 `Solution/`
- 📄 `DevToolbar.sln`

---

## US1.2 Définition des Interfaces Core (Contrats)
Création des interfaces qui abstraient les fonctionnalités natives pour permettre le testing web.

**En tant que** Architecte
**Je veux** définir les interfaces `IProcessService`, `IFileSystemService` et `ISettingsService`
**Afin de** pouvoir injecter des mocks dans le projet Web et des services réels dans MAUI.

**Auteur:** User
**Priorité:** Haute
**Estimation:** 3 points

**Exemple de code :**
''' csharp
namespace DevToolbar.Core.Interfaces;

public interface IProcessService {
    Task<int> StartProcessAsync(string path, string arguments);
    bool FocusWindowByTitle(string titleRegex);
}
'''

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Core/Interfaces/`
- 📄 `IProcessService.cs`
- 📄 `IPlugin.cs`

---

# F2. Système de Plugins & Configuration
Le moteur qui permet de charger des fonctionnalités dynamiquement et de gérer les contextes projets.

## US2.1 Engine de Chargement de Plugins
Mécanisme pour découvrir et charger les plugins (internes ou DLL externes).

**En tant que** Système
**Je veux** scanner et instancier toutes les classes implémentant `IPlugin` au démarrage
**Afin de** rendre l'application extensible.

**Critères d'acceptation:**
[ ] Service `PluginLoader` capable de lister les plugins.
[ ] Chaque plugin a un `UniqueId` et un `Name`.
[ ] Injection de dépendance fonctionnelle pour les plugins.

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Core/Services/`
- 📄 `PluginService.cs`

## US2.2 Gestion des Contextes Projets (Templating)
Permettre la configuration par projet avec héritage (Global -> Type -> Projet -> User).

**En tant que** Utilisateur
**Je veux** que la configuration de la toolbar change quand je sélectionne un projet différent
**Afin de** voir les outils pertinents pour ce projet (ex: pas de bouton "Deploy" sur une lib).

**Critères d'acceptation:**
[ ] Définition du modèle `ProjectConfig` (JSON).
[ ] Support des dossiers : Si le `Path` du projet existe, tenter de lire un `.devtoolbar.json` local.
[ ] Le changement de projet notifie tous les plugins via `OnProjectChanged`.

**Exemple de code :**
''' json
// template-webapi.json
{
  "projectType": "WebApi",
  "theme": { "accentColor": "#0078D7" },
  "enabledPlugins": ["Git", "SwaggerLauncher"]
}
'''

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Core/Models/`
- 📄 `ProjectConfig.cs`
- 📄 `SettingsService.cs`

---

# F3. UI Framework & Theming
La coquille visuelle de l'application, utilisant Blazor.

## US3.1 Layout Principal (Shell)
Création de la structure visuelle : Barre latérale (Projets), Zone centrale (Plugins), Zone droite (Actions).

**En tant que** Utilisateur
**Je veux** une interface compacte et organisée
**Afin de** accéder rapidement à mes outils sans perdre d'espace écran.

**Critères d'acceptation:**
[ ] Layout Responsive (CSS Grid/Flexbox).
[ ] Composant `ProjectSelector` (Dropdown).
[ ] Composant `PluginZone` (DynamicComponent).
[ ] Composant `ActionDeck` (Liste de boutons).

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.UI/Layouts/`
- 📄 `MainLayout.razor`
- 📄 `ToolbarShell.razor`

## US3.2 Personnalisation CSS (Théming dynamique)
Injection de variables CSS basées sur la configuration du projet actif.

**En tant que** Designer
**Je veux** que la couleur d'accentuation change selon le projet
**Afin de** identifier visuellement le contexte immédiat (ex: Prod = Rouge, Dev = Vert).

**Critères d'acceptation:**
[ ] Service `ThemeService` qui génère un bloc `<style>`.
[ ] Support des couleurs hexadécimales et des fonts.
[ ] Si un CSS custom est fourni dans la config, il est injecté.

**Exemple de code :**
''' csharp
// Dans MainLayout.razor
protected override void OnParametersSet() {
   _cssVariables = $"--accent-color: {CurrentProject.Color}; --font-family: {CurrentProject.Font};";
}
'''

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.UI/Services/`
- 📄 `ThemeService.cs`

---

# F4. Quick Actions (Custom Buttons)
Le "Deck" programmable par l'utilisateur.

## US4.1 Bouton "Lancer Processus"
Création d'un bouton qui lance un exe.

**En tant que** Développeur
**Je veux** configurer un bouton pour lancer Postman ou Visual Studio
**Afin de** gagner du temps.

**Critères d'acceptation:**
[ ] Configuration : Icone, Label, Path, Arguments.
[ ] Appel via `IProcessService.StartProcessAsync`.
[ ] Gestion d'erreur si le path est invalide (Toast notification).

## US4.2 Bouton "Lier à une fenêtre" (Smart Focus)
Bouton qui met le focus sur une fenêtre existante ou la lance si absente.

**En tant que** Utilisateur
**Je veux** cliquer sur un bouton qui m'amène à ma fenêtre de logs déjà ouverte
**Afin de** ne pas ouvrir 10 instances de la même application.

**Critères d'acceptation:**
[ ] Configuration : Regex du titre de la fenêtre (ex: ".*Log..*").
[ ] Si fenêtre trouvée -> `NativeMethods.SetForegroundWindow`.
[ ] Si non trouvée -> Lancer l'exécutable configuré.
[ ] (Bonus) Si plusieurs fenêtres -> Menu contextuel.

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Maui/Services/` (Implémentation native Windows API)
- 📄 `WindowsProcessService.cs`
- 📁 `DevToolbar.UI/Components/Buttons/`
- 📄 `SmartProcessButton.razor`

## US4.3 Bouton Script (Powershell/Python)
Exécution de scripts avec feedback visuel.

**En tant que** DevOps
**Je veux** lancer un script de build et voir le résultat
**Afin de** vérifier que la compilation a réussi.

**Critères d'acceptation:**
[ ] Choix de l'interpréteur (pwsh, cmd, python).
[ ] Ouverture d'une modale "Terminal Output" pour voir les logs en temps réel (Stream).

---

# F5. Plugins par Défaut
Les outils essentiels intégrés.

## US5.1 Plugin Git Tools
Affichage de l'état de la branche courante.

**En tant que** Développeur
**Je veux** voir ma branche actuelle et si j'ai des changements
**Afin de** ne pas commiter dans la mauvaise branche.

**Critères d'acceptation:**
[ ] Utilisation de `LibGit2Sharp` ou CLI Git.
[ ] Affichage : Nom branche, Indicateur (Clean/Dirty).
[ ] Bouton "Quick Sync" (Pull/Push).

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Core/Plugins/Git/`
- 📄 `GitPlugin.cs`

## US5.2 Plugin Work Items (TFS/GitHub)
Lien avec le ticketing.

**En tant que** Développeur
**Je veux** voir sur quel ticket je travaille et pouvoir le changer
**Afin de** tracker mon temps correctement.

**Critères d'acceptation:**
[ ] Affichage "ID - Titre".
[ ] Dropdown pour chercher/sélectionner un autre item.
[ ] Lien clicable vers le web (Azure DevOps / GitHub Issues).
[ ] Interface `IWorkItemProvider` pour supporter les 2 systèmes.

## US5.3 Plugin Time Management
Tracking automatique du temps.

**En tant que** Freelance/Employé
**Je veux** que le timer s'arrête quand je verrouille ma session windows
**Afin de** avoir des feuilles de temps précises.

**Critères d'acceptation:**
[ ] Écoute de `SystemEvents.SessionSwitch` (MAUI seulement).
[ ] Stockage local (SQLite) des entrées (Start, Stop, ProjectId, WorkItemId).
[ ] Détection d'inactivité (clavier/souris) après 15 min.

---

# F6. Testing & Qualité
Infrastructure pour les tests E2E avec Playwright.

## US6.1 Configuration Playwright sur le projet Web
Préparer le terrain pour les tests automatisés.

**En tant que** QA / Développeur
**Je veux** pouvoir lancer l'interface de la toolbar dans un navigateur standard via le projet `Test-Blazor-WebApp`
**Afin de** écrire des scripts Playwright qui valident l'UI.

**Critères d'acceptation:**
[ ] Le projet `Test-Blazor-WebApp` utilise des Mocks pour `IProcessService` (ne lance rien, log juste).
[ ] Script Playwright de base qui lance l'app et vérifie le titre.
[ ] CI/CD pipeline capable d'exécuter ces tests.

**Exemple de code :**
''' csharp
// Dans Program.cs du projet Web
builder.Services.AddSingleton<IProcessService, MockProcessService>();
builder.Services.AddSingleton<IPluginLoader, MockPluginLoader>();
'''

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Web/Mocks/`
- 📁 `DevToolbar.Tests/E2E/`

# F7. Plugin : GitHub Agent Sessions (Oubli de la liste précédente)
Suivi des tâches d'intégration continue/déploiement continu (CI/CD) liées au projet courant.

## US7.1 Suivi des tâches en arrière-plan
**En tant que** Développeur DevOps
**Je veux** que la toolbar interroge régulièrement l'API GitHub pour le projet en cours
**Afin de** savoir si mes builds/déploiements sont terminés sans avoir à garder la page web ouverte.

**Critères d'acceptation:**
[ ] Polling de l'API GitHub (Octokit) en tâche de fond (intervalle configurable).
[ ] Affichage d'une icône d'état global du CI/CD dans la toolbar.
[ ] Affichage d'un badge numérique rouge/vert pour le nombre de tâches terminées "non lues".

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Plugins/GithubAgents/`
- 📄 `GithubAgentPlugin.cs`

## US7.2 Consultation et Acquittement (Mark as Read)
**En tant que** Développeur
**Je veux** cliquer sur le badge des agents pour voir les résultats et les marquer comme lus
**Afin de** vider mes notifications et ouvrir les logs détaillés si nécessaire.

**Critères d'acceptation:**
[ ] Clic sur l'icône ouvre un menu déroulant (dropdown) listant les dernières sessions.
[ ] Clic sur une session spécifique l'ouvre dans le navigateur web.
[ ] Clic sur une session spécifique la marque comme `read` (stocké dans les données locales du plugin pour ce projet).


---

# F5. Plugins par Défaut (Suite - Ajout des Rapports)

## US5.4 Rapports de Temps (Time Reporting)
**En tant que** Utilisateur
**Je veux** pouvoir consulter le temps passé par projet et par WorkItem
**Afin de** remplir mes feuilles de temps ou facturer mes clients.

**Critères d'acceptation:**
[ ] Modale de rapport accessible depuis le plugin "Time Management".
[ ] Filtres par : Jour, Semaine, Mois.
[ ] Regroupement par : Projet, WorkItem, ou Custom Action.
[ ] Bouton d'export en CSV.

**Exemple de code :**
''' csharp
// Exemple de requête Entity Framework pour le rapport
var weeklyReport = await _dbContext.TimeEntries
    .Where(t => t.StartTime >= DateTime.Now.AddDays(-7))
    .GroupBy(t => new { t.ProjectId, t.WorkItemId })
    .Select(g => new { 
        Context = g.Key, 
        TotalMinutes = g.Sum(x => (x.EndTime - x.StartTime).TotalMinutes) 
    }).ToListAsync();
'''

---

# F8. Intégration Desktop & Base de données (Fondations MAUI)
Ces fonctionnalités sont indispensables pour que l'application se comporte comme une vraie "Toolbar" et sauvegarde correctement tes données.

## US8.1 Gestion de la Fenêtre Native (Window Chrome & TopMost)
**En tant que** Utilisateur Desktop
**Je veux** que la barre soit ancrée en haut ou en bas de l'écran sans les bordures Windows classiques
**Afin de** s'intégrer parfaitement à mon espace de travail comme une vraie barre d'outils.

**Critères d'acceptation:**
[ ] Retrait de la barre de titre standard Windows (TitleBar = Collapsed).
[ ] Mode "Always on Top" (Toujours au premier plan) activable via les paramètres globaux.
[ ] (Bonus) Appels API Win32 (SetWindowPos) pour ancrer l'application comme une AppBar Windows.

**Fichiers/Projets concernés:**
- 📁 `DevToolbar.Maui/Platforms/Windows/`
- 📄 `App.xaml.cs` (Configuration du `MauiWinUIWindow`)

## US8.2 System Tray (Zone de notification)
**En tant que** Utilisateur Desktop
**Je veux** que l'application reste active dans la zone de notification (près de l'horloge) quand je la ferme ou la minimise
**Afin de** garder le tracker de temps et les plugins d'arrière-plan actifs sans encombrer ma barre des tâches.

**Critères d'acceptation:**
[ ] Icône présente dans le System Tray.
[ ] Double clic sur l'icône restaure la Toolbar.
[ ] Clic droit offre un menu contextuel (Afficher, Paramètres globaux, Quitter).

## US8.3 Système de Stockage Local (SQLite + JSON)
**En tant que** Système
**Je veux** un mécanisme unifié pour stocker les données selon leur portée (Global, Par Projet, Par Type de Projet)
**Afin de** persister la configuration, les rapports de temps et les états des plugins (comme les agents lus).

**Critères d'acceptation:**
[ ] Mise en place d'une base SQLite locale (ex: `%APPDATA%\DevToolbar\data.db`) pour les données relationnelles (TimeTracking, Logs).
[ ] Service de configuration (JSON) hiérarchique : fusionne `global.json`, `template.json`, et `.devtoolbar.json` (dans le dossier du projet lié).
