using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

[assembly: AssemblyTitle("ZTime Setup")]
[assembly: AssemblyDescription("ZTime installer")]
[assembly: AssemblyProduct("ZTime")]
[assembly: AssemblyCompany("Fred Zarma")]
[assembly: AssemblyCopyright("Copyright Fred Zarma 2026")]
[assembly: AssemblyVersion("2.3.1.0")]
[assembly: AssemblyFileVersion("2.3.1.0")]

internal static class SetupUi
{
    static readonly string[] Codes = new string[] { "en", "zh", "hi", "es", "fr", "ru", "ja", "de" };
    static readonly System.Collections.Generic.Dictionary<string, string[]> Map =
        new System.Collections.Generic.Dictionary<string, string[]>(StringComparer.Ordinal);
    static string _code = "en";

    static SetupUi()
    {
        Add("ZTime Setup", "ZTime 安装程序", "ZTime सेटअप", "Instalación de ZTime", "Installation de ZTime", "Установка ZTime", "ZTime セットアップ", "ZTime-Setup");
        Add("Install ZTime", "安装 ZTime", "ZTime इंस्टॉल करें", "Instalar ZTime", "Installer ZTime", "Установить ZTime", "ZTime をインストール", "ZTime installieren");
        Add("Uninstall ZTime", "卸载 ZTime", "ZTime अनइंस्टॉल करें", "Desinstalar ZTime", "Désinstaller ZTime", "Удалить ZTime", "ZTime をアンインストール", "ZTime deinstallieren");
        Add("ZTime will be installed to:", "ZTime 将安装到：", "ZTime यहाँ इंस्टॉल होगा:", "ZTime se instalará en:", "ZTime sera installé dans :", "ZTime будет установлен в:", "ZTime のインストール先:", "ZTime wird installiert nach:");
        Add("A desktop shortcut named ZTime will be created.", "将创建名为 ZTime 的桌面快捷方式。", "ZTime नाम का डेस्कटॉप शॉर्टकट बनेगा।", "Se creará un acceso directo de escritorio llamado ZTime.", "Un raccourci bureau nommé ZTime sera créé.", "Будет создан ярлык ZTime на рабочем столе.", "デスクトップに ZTime ショートカットを作成します。", "Es wird eine Desktopverknüpfung namens ZTime erstellt.");
        Add("A Start menu shortcut will be created.", "将创建开始菜单快捷方式。", "स्टार्ट मेनू शॉर्टकट बनेगा।", "Se creará un acceso directo en el menú Inicio.", "Un raccourci du menu Démarrer sera créé.", "Будет создан ярлык в меню Пуск.", "スタート メニューにショートカットを作成します。", "Es wird eine Startmenü-Verknüpfung erstellt.");
        Add("Install", "安装", "इंस्टॉल", "Instalar", "Installer", "Установить", "インストール", "Installieren");
        Add("Uninstall", "卸载", "अनइंस्टॉल", "Desinstalar", "Désinstaller", "Удалить", "アンインストール", "Deinstallieren");
        Add("Cancel", "取消", "रद्द करें", "Cancelar", "Annuler", "Отмена", "キャンセル", "Abbrechen");
        Add("Close", "关闭", "बंद करें", "Cerrar", "Fermer", "Закрыть", "閉じる", "Schließen");
        Add("ZTime was installed.", "ZTime 已安装。", "ZTime इंस्टॉल हो गया।", "ZTime se ha instalado.", "ZTime a été installé.", "ZTime установлен.", "ZTime をインストールしました。", "ZTime wurde installiert.");
        Add("Uninstall ZTime from this computer?", "要从这台电脑卸载 ZTime 吗？", "इस कंप्यूटर से ZTime अनइंस्टॉल करें?", "¿Desinstalar ZTime de este equipo?", "Désinstaller ZTime de cet ordinateur ?", "Удалить ZTime с этого компьютера?", "このコンピューターから ZTime をアンインストールしますか？", "ZTime von diesem Computer deinstallieren?");
        Add("Also delete saved settings", "同时删除已保存的设置", "सहेजी गई सेटिंग भी हटाएँ", "Eliminar también los ajustes guardados", "Supprimer aussi les réglages enregistrés", "Также удалить сохранённые настройки", "保存した設定も削除する", "Gespeicherte Einstellungen ebenfalls löschen");
        Add("ZTime was uninstalled.", "ZTime 已卸载。", "ZTime अनइंस्टॉल हो गया।", "ZTime se ha desinstalado.", "ZTime a été désinstallé.", "ZTime удалён.", "ZTime をアンインストールしました。", "ZTime wurde deinstalliert.");
        Add("ZTime is not installed.", "尚未安装 ZTime。", "ZTime इंस्टॉल नहीं है।", "ZTime no está instalado.", "ZTime n'est pas installé.", "ZTime не установлен.", "ZTime はインストールされていません。", "ZTime ist nicht installiert.");
        Add("ZTime.exe was not found next to Setup.exe.", "未在 Setup.exe 旁找到 ZTime.exe。", "Setup.exe के पास ZTime.exe नहीं मिला।", "No se encontró ZTime.exe junto a Setup.exe.", "ZTime.exe est introuvable à côté de Setup.exe.", "ZTime.exe не найден рядом с Setup.exe.", "Setup.exe と同じ場所に ZTime.exe がありません。", "ZTime.exe wurde neben Setup.exe nicht gefunden.");
        Add("Could not install ZTime.", "无法安装 ZTime。", "ZTime इंस्टॉल नहीं हो सका।", "No se pudo instalar ZTime.", "Impossible d'installer ZTime.", "Не удалось установить ZTime.", "ZTime をインストールできませんでした。", "ZTime konnte nicht installiert werden.");
        Add("Could not uninstall ZTime.", "无法卸载 ZTime。", "ZTime अनइंस्टॉल नहीं हो सका।", "No se pudo desinstalar ZTime.", "Impossible de désinstaller ZTime.", "Не удалось удалить ZTime.", "ZTime をアンインストールできませんでした。", "ZTime konnte nicht deinstalliert werden.");
        Add("Launch ZTime", "启动 ZTime", "ZTime चलाएँ", "Iniciar ZTime", "Lancer ZTime", "Запустить ZTime", "ZTime を起動", "ZTime starten");
        Add("Open ZTime", "打开 ZTime", "ZTime खोलें", "Abrir ZTime", "Ouvrir ZTime", "Открыть ZTime", "ZTime を開く", "ZTime öffnen");
        Add("Folder:", "文件夹：", "फ़ोल्डर:", "Carpeta:", "Dossier :", "Папка:", "フォルダー:", "Ordner:");
        Add("Browse...", "浏览...", "ब्राउज़...", "Examinar...", "Parcourir...", "Обзор...", "参照...", "Durchsuchen...");
        Add("Add a desktop shortcut", "添加桌面快捷方式", "डेस्कटॉप शॉर्टकट जोड़ें", "Añadir un acceso directo en el escritorio", "Ajouter un raccourci sur le bureau", "Добавить ярлык на рабочий стол", "デスクトップにショートカットを追加", "Desktopverknüpfung hinzufügen");
        Add("Add a Start menu shortcut", "添加开始菜单快捷方式", "स्टार्ट मेनू शॉर्टकट जोड़ें", "Añadir un acceso directo al menú Inicio", "Ajouter un raccourci au menu Démarrer", "Добавить ярлык в меню Пуск", "スタート メニューにショートカットを追加", "Startmenü-Verknüpfung hinzufügen");
        Add("Choose a folder", "选择文件夹", "फ़ोल्डर चुनें", "Elegir una carpeta", "Choisir un dossier", "Выберите папку", "フォルダーを選択", "Ordner wählen");
        Add("The folder is not valid.", "文件夹无效。", "फ़ोल्डर मान्य नहीं है।", "La carpeta no es válida.", "Le dossier n'est pas valable.", "Папка недопустима.", "フォルダーが正しくありません。", "Der Ordner ist ungültig.");
    }

    static void Add(string en, string zh, string hi, string es, string fr, string ru, string ja, string de)
    {
        Map[en] = new string[] { zh, hi, es, fr, ru, ja, de };
    }

    public static void Init()
    {
        _code = "en";
        try
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ValheimAdminHelper",
                "settings.json");
            if (!File.Exists(path))
                return;
            string text = File.ReadAllText(path);
            int i = text.IndexOf("\"Language\"");
            if (i < 0)
                return;
            int colon = text.IndexOf(':', i + 10);
            if (colon < 0)
                return;
            int q1 = text.IndexOf('"', colon + 1);
            int q2 = q1 < 0 ? -1 : text.IndexOf('"', q1 + 1);
            if (q1 < 0 || q2 < 0)
                return;
            string v = text.Substring(q1 + 1, q2 - q1 - 1).Trim().ToLowerInvariant();
            if (v == "fr" || v == "french" || v.StartsWith("fr-"))
                _code = "fr";
        }
        catch { }
        try
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string primary = Path.Combine(appData, "ZTime", "settings.txt");
            string legacy = Path.Combine(appData, "DesktopClock", "settings.txt");
            string clockSettings = File.Exists(primary) ? primary : legacy;
            if (!File.Exists(clockSettings))
                return;
            string[] lines = File.ReadAllLines(clockSettings);
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith("lang=", StringComparison.Ordinal))
                    continue;
                string lang = Normalize(lines[i].Substring(5).Trim());
                if (lang != null)
                    _code = lang;
            }
        }
        catch { }
    }

    static string Normalize(string code)
    {
        if (code == null)
            return null;
        string s = code.Trim().ToLowerInvariant();
        if (s == "en" || s.StartsWith("en-")) return "en";
        if (s == "zh" || s.StartsWith("zh") || s == "mandarin" || s == "chinese") return "zh";
        if (s == "hi" || s.StartsWith("hi-") || s == "hindi") return "hi";
        if (s == "es" || s.StartsWith("es-") || s == "spanish") return "es";
        if (s == "fr" || s.StartsWith("fr-") || s == "french") return "fr";
        if (s == "ru" || s.StartsWith("ru-") || s == "russian") return "ru";
        if (s == "ja" || s.StartsWith("ja-") || s == "jp" || s == "japanese") return "ja";
        if (s == "de" || s.StartsWith("de-") || s == "german" || s == "deutsch") return "de";
        return null;
    }

    public static string T(string en)
    {
        if (en == null || _code == "en")
            return en ?? "";
        string[] row;
        if (!Map.TryGetValue(en, out row) || row == null)
            return en;
        int idx = -1;
        for (int i = 0; i < Codes.Length; i++)
        {
            if (Codes[i] == _code)
            {
                idx = i - 1;
                break;
            }
        }
        if (idx < 0 || idx >= row.Length || string.IsNullOrEmpty(row[idx]))
            return en;
        return row[idx];
    }
}

internal static class Paths
{
    public const string UninstallSubKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\ZTime";

    public static string DefaultInstallDir
    {
        get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ZTime"); }
    }

    public static string InstallDir
    {
        get { return ReadInstallDir(); }
    }

    public static string ReadInstallDir()
    {
        try
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(UninstallSubKey))
            {
                if (key != null)
                {
                    object v = key.GetValue("InstallLocation");
                    if (v != null)
                    {
                        string p = v.ToString().Trim();
                        if (p.Length > 0)
                            return p;
                    }
                }
            }
        }
        catch { }
        return DefaultInstallDir;
    }

    public static string ZTimeExe
    {
        get { return Path.Combine(InstallDir, "ZTime.exe"); }
    }

    public static string SetupExe
    {
        get { return Path.Combine(InstallDir, "Setup.exe"); }
    }

    public static string DesktopLnk
    {
        get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "ZTime.lnk"); }
    }

    public static string StartDir
    {
        get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "ZTime"); }
    }

    public static string StartLnk
    {
        get { return Path.Combine(StartDir, "ZTime.lnk"); }
    }

    public static string UserDesktopLnk
    {
        get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "ZTime.lnk"); }
    }

    public static string StartupLnk
    {
        get { return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            "ZTime.lnk"); }
    }

    public static string SettingsDir
    {
        get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZTime"); }
    }

    public static bool IsInstalled()
    {
        return File.Exists(ZTimeExe);
    }
}

internal static class SetupProgram
{
    [STAThread]
    static void Main(string[] args)
    {
        SetupUi.Init();
        bool uninstall = HasArg(args, "--uninstall");
        Application app = new Application();
        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        app.Run(new SetupWindow(uninstall));
    }

    static bool HasArg(string[] args, string name)
    {
        if (args == null)
            return false;
        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}

internal sealed class SetupWindow : Window
{
    readonly bool _uninstall;
    readonly TextBlock _status;
    readonly TextBox _folder;
    readonly CheckBox _desktop;
    readonly CheckBox _start;
    readonly StackPanel _installOpts;
    readonly CheckBox _wipe;
    readonly Button _action;
    readonly Button _cancel;
    readonly Button _launch;

    public SetupWindow(bool uninstall)
    {
        _uninstall = uninstall;

        Title = SetupUi.T("ZTime Setup");
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = true;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.WidthAndHeight;
        FontFamily = new FontFamily("Segoe UI, Microsoft YaHei UI, Nirmala UI, Yu Gothic UI");

        TextBlock title = Label(SetupUi.T(_uninstall ? "Uninstall ZTime" : "Install ZTime"), 16, FontWeights.SemiBold);
        title.Margin = new Thickness(0, 0, 0, 12);

        _status = Label("", 13, FontWeights.Normal);
        _status.TextWrapping = TextWrapping.Wrap;
        _status.Margin = new Thickness(0, 0, 0, 12);
        if (_uninstall && Paths.IsInstalled())
            _status.Text = SetupUi.T("Uninstall ZTime from this computer?");
        else if (_uninstall)
            _status.Text = SetupUi.T("ZTime is not installed.");
        else
            _status.Text = SetupUi.T("ZTime will be installed to:");

        TextBlock folderCap = Label(SetupUi.T("Folder:"), 12, FontWeights.Normal);
        folderCap.Margin = new Thickness(0, 0, 0, 4);

        _folder = new TextBox();
        _folder.Text = Paths.IsInstalled() ? Paths.InstallDir : Paths.DefaultInstallDir;
        _folder.FontSize = 13;
        _folder.Padding = new Thickness(6, 4, 6, 4);
        _folder.VerticalContentAlignment = VerticalAlignment.Center;

        Button browse = DarkButton(SetupUi.T("Browse..."));
        browse.MinWidth = 100;
        browse.Margin = new Thickness(8, 0, 0, 0);
        browse.Click += delegate { BrowseFolder(); };

        DockPanel folderRow = new DockPanel();
        folderRow.LastChildFill = true;
        folderRow.Margin = new Thickness(0, 0, 0, 10);
        DockPanel.SetDock(browse, Dock.Right);
        folderRow.Children.Add(browse);
        folderRow.Children.Add(_folder);

        _desktop = OptBox(SetupUi.T("Add a desktop shortcut"), true);
        _start = OptBox(SetupUi.T("Add a Start menu shortcut"), true);

        _installOpts = new StackPanel();
        _installOpts.Margin = new Thickness(0, 0, 0, 12);
        _installOpts.Children.Add(folderCap);
        _installOpts.Children.Add(folderRow);
        _installOpts.Children.Add(_desktop);
        _installOpts.Children.Add(_start);
        _installOpts.Visibility = _uninstall ? Visibility.Collapsed : Visibility.Visible;

        _wipe = new CheckBox();
        _wipe.Content = SetupUi.T("Also delete saved settings");
        _wipe.Foreground = Brushes.White;
        _wipe.Margin = new Thickness(0, 0, 0, 16);
        _wipe.Visibility = _uninstall ? Visibility.Visible : Visibility.Collapsed;

        _action = DarkButton(SetupUi.T(_uninstall ? "Uninstall" : "Install"));
        _cancel = DarkButton(SetupUi.T("Cancel"));
        _launch = DarkButton(SetupUi.T("Open ZTime"));
        _launch.Visibility = Visibility.Collapsed;
        _launch.Click += delegate { LaunchZTime(); Close(); };
        _action.Click += delegate { DoWork(); };
        _cancel.Click += delegate { Close(); };

        StackPanel buttons = new StackPanel();
        buttons.Orientation = Orientation.Horizontal;
        buttons.HorizontalAlignment = HorizontalAlignment.Right;
        buttons.Children.Add(_cancel);
        buttons.Children.Add(_launch);
        buttons.Children.Add(_action);

        StackPanel body = new StackPanel();
        body.Margin = new Thickness(20);
        body.Children.Add(title);
        body.Children.Add(_status);
        body.Children.Add(_installOpts);
        body.Children.Add(_wipe);
        body.Children.Add(buttons);

        Border chrome = new Border();
        chrome.CornerRadius = new CornerRadius(10);
        chrome.Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E));
        chrome.BorderBrush = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x40));
        chrome.BorderThickness = new Thickness(1);
        chrome.Width = 460;
        chrome.Child = body;
        Content = chrome;

        MouseLeftButtonDown += delegate { try { DragMove(); } catch { } };
        KeyDown += delegate(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        };
    }

    void DoWork()
    {
        _action.IsEnabled = false;
        try
        {
            if (_uninstall)
            {
                if (!Paths.IsInstalled() && _wipe.IsChecked != true)
                {
                    _status.Text = SetupUi.T("ZTime is not installed.");
                    _action.IsEnabled = true;
                    return;
                }
                Actions.Uninstall(_wipe.IsChecked == true);
                _status.Text = SetupUi.T("ZTime was uninstalled.");
                _wipe.Visibility = Visibility.Collapsed;
                _action.Visibility = Visibility.Collapsed;
                _launch.Visibility = Visibility.Collapsed;
                _cancel.Content = SetupUi.T("Close");
            }
            else
            {
                string dest;
                if (!TryFolder(out dest))
                {
                    _status.Text = SetupUi.T("The folder is not valid.");
                    _action.IsEnabled = true;
                    return;
                }
                string err;
                if (!Actions.Install(dest, _desktop.IsChecked == true, _start.IsChecked == true, out err))
                {
                    _status.Text = string.IsNullOrEmpty(err) ? SetupUi.T("Could not install ZTime.") : err;
                    _action.IsEnabled = true;
                    return;
                }
                _status.Text = SetupUi.T("ZTime was installed.");
                _installOpts.Visibility = Visibility.Collapsed;
                _action.Visibility = Visibility.Collapsed;
                _cancel.Content = SetupUi.T("Close");
                _launch.Visibility = Visibility.Visible;
            }
        }
        catch
        {
            _status.Text = SetupUi.T(_uninstall ? "Could not uninstall ZTime." : "Could not install ZTime.");
            _action.IsEnabled = true;
        }
    }

    bool TryFolder(out string dest)
    {
        dest = null;
        string raw = _folder.Text == null ? "" : _folder.Text.Trim();
        if (raw.Length == 0)
            return false;
        try
        {
            dest = Path.GetFullPath(raw);
        }
        catch
        {
            return false;
        }
        if (File.Exists(dest))
            return false;
        return dest.Length > 0;
    }

    void BrowseFolder()
    {
        System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
        dlg.Description = SetupUi.T("Choose a folder");
        dlg.ShowNewFolderButton = true;
        string current;
        if (TryFolder(out current) && Directory.Exists(current))
            dlg.SelectedPath = current;
        else
            dlg.SelectedPath = Paths.DefaultInstallDir;
        System.Windows.Forms.DialogResult r = dlg.ShowDialog();
        if (r == System.Windows.Forms.DialogResult.OK && dlg.SelectedPath != null && dlg.SelectedPath.Length > 0)
            _folder.Text = dlg.SelectedPath;
    }

    static void LaunchZTime()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = "\"" + Paths.ZTimeExe + "\"",
                UseShellExecute = true
            });
        }
        catch { }
    }

    static CheckBox OptBox(string text, bool on)
    {
        CheckBox c = new CheckBox();
        c.Content = text;
        c.IsChecked = on;
        c.Foreground = Brushes.White;
        c.Margin = new Thickness(0, 0, 0, 8);
        return c;
    }

    static TextBlock Label(string text, double size, FontWeight weight)
    {
        TextBlock t = new TextBlock();
        t.Text = text;
        t.Foreground = Brushes.White;
        t.FontSize = size;
        t.FontWeight = weight;
        return t;
    }

    static Button DarkButton(string text)
    {
        Button b = new Button();
        b.Content = text;
        b.MinWidth = 110;
        b.Height = 30;
        b.Margin = new Thickness(8, 0, 0, 0);
        b.FontFamily = new FontFamily("Segoe UI");
        return b;
    }
}

internal static class Actions
{
    public static bool Install(string dest, bool desktop, bool startMenu, out string error)
    {
        error = null;
        string srcDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string clockSrc = Path.Combine(srcDir, "ZTime.exe");
        if (!File.Exists(clockSrc))
        {
            error = SetupUi.T("ZTime.exe was not found next to Setup.exe.");
            return false;
        }
        StopZTime();
        if (!Directory.Exists(dest))
            Directory.CreateDirectory(dest);

        string clockDest = Path.Combine(dest, "ZTime.exe");
        string setupDest = Path.Combine(dest, "Setup.exe");
        File.Copy(clockSrc, clockDest, true);
        string setupSrc = Assembly.GetExecutingAssembly().Location;
        try
        {
            if (!PathsEqual(setupSrc, setupDest))
                File.Copy(setupSrc, setupDest, true);
        }
        catch { }

        string icoSrc = Path.Combine(srcDir, "ztime.ico");
        if (File.Exists(icoSrc))
        {
            try { File.Copy(icoSrc, Path.Combine(dest, "ztime.ico"), true); }
            catch { }
        }

        if (desktop)
            CreateShortcut(Paths.DesktopLnk, clockDest, dest);
        if (startMenu)
        {
            if (!Directory.Exists(Paths.StartDir))
                Directory.CreateDirectory(Paths.StartDir);
            CreateShortcut(Paths.StartLnk, clockDest, dest);
        }
        WriteUninstallKey(dest, clockDest, setupDest);
        return true;
    }

    public static void Uninstall(bool wipeSettings)
    {
        StopZTime();
        DeleteFile(Paths.DesktopLnk);
        DeleteFile(Paths.UserDesktopLnk);
        DeleteFile(Paths.StartLnk);
        DeleteFile(Paths.StartupLnk);
        try
        {
            if (Directory.Exists(Paths.StartDir) && Directory.GetFileSystemEntries(Paths.StartDir).Length == 0)
                Directory.Delete(Paths.StartDir);
        }
        catch { }
        try { Registry.LocalMachine.DeleteSubKeyTree(Paths.UninstallSubKey, false); }
        catch { }
        if (wipeSettings)
        {
            try
            {
                if (Directory.Exists(Paths.SettingsDir))
                    Directory.Delete(Paths.SettingsDir, true);
            }
            catch { }
        }

        string dest = Paths.InstallDir;
        string self = Assembly.GetExecutingAssembly().Location;
        bool selfInside = false;
        try
        {
            selfInside = Path.GetFullPath(self).StartsWith(Path.GetFullPath(dest) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                || PathsEqual(Path.GetDirectoryName(self), dest);
        }
        catch { }

        try
        {
            string[] files = Directory.Exists(dest) ? Directory.GetFiles(dest) : new string[0];
            for (int i = 0; i < files.Length; i++)
            {
                if (selfInside && PathsEqual(files[i], self))
                    continue;
                DeleteFile(files[i]);
            }
        }
        catch { }

        if (selfInside)
        {
            ProcessStartInfo cmd = new ProcessStartInfo();
            cmd.FileName = "cmd.exe";
            cmd.Arguments = "/c ping 127.0.0.1 -n 2 >nul & rmdir /s /q \"" + dest + "\"";
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            try { Process.Start(cmd); }
            catch { }
        }
        else
        {
            try
            {
                if (Directory.Exists(dest))
                    Directory.Delete(dest, true);
            }
            catch { }
        }
    }

    static void WriteUninstallKey(string dest, string clockDest, string setupDest)
    {
        RegistryKey key = Registry.LocalMachine.CreateSubKey(Paths.UninstallSubKey);
        if (key == null)
            return;
        using (key)
        {
            key.SetValue("DisplayName", "ZTime");
            key.SetValue("DisplayVersion", "2.3.1");
            key.SetValue("Publisher", "Fred Zarma");
            key.SetValue("InstallLocation", dest);
            key.SetValue("DisplayIcon", clockDest);
            key.SetValue("UninstallString", "\"" + setupDest + "\" --uninstall");
            key.SetValue("QuietUninstallString", "\"" + setupDest + "\" --uninstall");
            key.SetValue("HelpLink", "https://fredzarma.itch.io/clock");
            key.SetValue("URLInfoAbout", "https://github.com/FredZarma/ZTime");
            key.SetValue("NoModify", 1, RegistryValueKind.DWord);
            key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
            try
            {
                FileInfo fi = new FileInfo(clockDest);
                int kb = (int)Math.Max(1, fi.Length / 1024);
                key.SetValue("EstimatedSize", kb, RegistryValueKind.DWord);
            }
            catch { }
        }
    }

    static void CreateShortcut(string lnk, string target, string workDir)
    {
        Type t = Type.GetTypeFromProgID("WScript.Shell");
        object shell = Activator.CreateInstance(t);
        object shortcut = t.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { lnk });
        Type st = shortcut.GetType();
        st.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortcut, new object[] { target });
        st.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, shortcut, new object[] { workDir });
        st.InvokeMember("Description", BindingFlags.SetProperty, null, shortcut, new object[] { "ZTime" });
        st.InvokeMember("IconLocation", BindingFlags.SetProperty, null, shortcut, new object[] { target + ",0" });
        st.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortcut, null);
    }

    static void StopZTime()
    {
        Process[] list = Process.GetProcessesByName("ZTime");
        for (int i = 0; i < list.Length; i++)
        {
            try
            {
                using (list[i])
                {
                    string path = "";
                    try { path = list[i].MainModule.FileName; }
                    catch { }
                    if (path.IndexOf("VAHZTime", StringComparison.OrdinalIgnoreCase) >= 0)
                        continue;
                    list[i].Kill();
                    list[i].WaitForExit(4000);
                }
            }
            catch { }
        }
        Thread.Sleep(300);
    }

    static void DeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch { }
    }

    static bool PathsEqual(string a, string b)
    {
        if (a == null || b == null)
            return false;
        return string.Equals(Path.GetFullPath(a).TrimEnd('\\'), Path.GetFullPath(b).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);
    }
}
