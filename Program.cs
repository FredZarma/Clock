using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

[assembly: AssemblyTitle("ZTime")]
[assembly: AssemblyDescription("Digital desktop clock")]
[assembly: AssemblyProduct("ZTime")]
[assembly: AssemblyCompany("Fred Zarma")]
[assembly: AssemblyCopyright("Copyright Fred Zarma 2026")]
[assembly: AssemblyVersion("2.3.1.0")]
[assembly: AssemblyFileVersion("2.3.1.0")]

internal static class Ui
{
    public static readonly string[] Codes = new string[] { "en", "zh", "hi", "es", "fr", "ru", "ja", "de" };
    static readonly string[] IetfTags = new string[] { "en-US", "zh-CN", "hi-IN", "es-ES", "fr-FR", "ru-RU", "ja-JP", "de-DE" };
    static readonly string[] Names = new string[] { "English", "中文", "हिन्दी", "Español", "Français", "Русский", "日本語", "Deutsch" };
    static readonly Dictionary<string, string[]> Map = new Dictionary<string, string[]>(StringComparer.Ordinal);

    static string _code = "en";
    static CultureInfo _culture = CultureInfo.GetCultureInfo("en-US");

    static Ui()
    {
        Add("Open calendar", "打开日历", "कैलेंडर खोलें", "Abrir el calendario", "Ouvrir le calendrier", "Открыть календарь", "カレンダーを開く", "Kalender öffnen");
        Add("Copy the time", "复制时间", "समय कॉपी करें", "Copiar la hora", "Copier l'heure", "Копировать время", "時刻をコピー", "Uhrzeit kopieren");
        Add("Alarm...", "闹钟...", "अलार्म...", "Alarma...", "Alarme...", "Будильник...", "アラーム...", "Wecker...");
        Add("Small", "小", "छोटा", "Pequeña", "Petite", "Маленький", "小", "Klein");
        Add("Normal", "标准", "सामान्य", "Normal", "Normale", "Обычный", "標準", "Normal");
        Add("Large", "大", "बड़ा", "Grande", "Grande", "Большой", "大", "Groß");
        Add("Extra", "特大", "अतिरिक्त", "Extra", "Extra", "Extra", "特大", "Extra");
        Add("Size", "大小", "आकार", "Tamaño", "Taille", "Размер", "サイズ", "Größe");
        Add("Green", "绿", "हरा", "Verde", "Vert", "Зелёный", "緑", "Grün");
        Add("Amber", "琥珀", "अंबर", "Ámbar", "Ambre", "Янтарный", "琥珀", "Bernstein");
        Add("Red", "红", "लाल", "Rojo", "Rouge", "Красный", "赤", "Rot");
        Add("Blue", "蓝", "नीला", "Azul", "Bleu", "Синий", "青", "Blau");
        Add("Cyan", "青", "स्यान", "Cian", "Cyan", "Голубой", "シアン", "Cyan");
        Add("White", "白", "सफेद", "Blanco", "Blanc", "Белый", "白", "Weiß");
        Add("Color", "颜色", "रंग", "Color", "Couleur", "Цвет", "色", "Farbe");
        Add("Display", "显示", "प्रदर्शन", "Visualización", "Affichage", "Отображение", "表示", "Anzeige");
        Add("Seconds", "秒", "सेकंड", "Segundos", "Secondes", "Секунды", "秒", "Sekunden");
        Add("Date", "日期", "तारीख", "Fecha", "Date", "Дата", "日付", "Datum");
        Add("24-hour format", "24小时制", "24 घंटे प्रारूप", "Formato 24 horas", "Format 24 heures", "24-часовой формат", "24時間表示", "24-Stunden-Format");
        Add("Hour chime", "整点报时", "घंटे की घंटी", "Campanada", "Carillon des heures", "Бой часов", "時報", "Stundenschlag");
        Add("Opacity", "不透明度", "अपारदर्शिता", "Opacidad", "Opacité", "Непрозрачность", "不透明度", "Deckkraft");
        Add("Name", "名称", "नाम", "Nombre", "Nom", "Имя", "名前", "Name");
        Add("Show the name", "显示名称", "नाम दिखाएँ", "Mostrar el nombre", "Afficher le nom", "Показать имя", "名前を表示", "Namen anzeigen");
        Add("Change the name...", "更改名称...", "नाम बदलें...", "Cambiar el nombre...", "Modifier le nom...", "Изменить имя...", "名前を変更...", "Namen ändern...");
        Add("Snap to grid", "对齐到网格", "ग्रिड पर संरेखित करें", "Ajustar a la cuadrícula", "Aligner sur la grille", "Привязать к сетке", "グリッドに合わせる", "Am Raster ausrichten");
        Add("Lock position", "锁定位置", "स्थिति लॉक करें", "Bloquear la posición", "Verrouiller la position", "Зафиксировать положение", "位置を固定", "Position sperren");
        Add("Always on top", "始终置顶", "हमेशा ऊपर", "Siempre visible", "Toujours visible", "Поверх всех окон", "常に最前面", "Immer im Vordergrund");
        Add("Run at startup", "开机启动", "स्टार्टअप पर चलाएँ", "Ejecutar al inicio", "Lancer au démarrage", "Запускать при старте", "起動時に実行", "Beim Start ausführen");
        Add("Windows date and time", "Windows 日期和时间", "Windows दिनांक और समय", "Fecha y hora de Windows", "Date et heure Windows", "Дата и время Windows", "Windows の日付と時刻", "Windows-Datum und -Uhrzeit");
        Add("Close", "关闭", "बंद करें", "Cerrar", "Fermer", "Закрыть", "閉じる", "Schließen");
        Add("Uninstall...", "卸载...", "अनइंस्टॉल...", "Desinstalar...", "Désinstaller...", "Удалить...", "アンインストール...", "Deinstallieren...");
        Add("Uninstall ZTime from this computer?", "要从这台电脑卸载 ZTime 吗？", "इस कंप्यूटर से ZTime अनइंस्टॉल करें?", "¿Desinstalar ZTime de este equipo?", "Désinstaller ZTime de cet ordinateur ?", "Удалить ZTime с этого компьютера?", "このコンピューターから ZTime をアンインストールしますか？", "ZTime von diesem Computer deinstallieren?");
        Add("Also delete saved settings", "同时删除已保存的设置", "सहेजी गई सेटिंग भी हटाएँ", "Eliminar también los ajustes guardados", "Supprimer aussi les réglages enregistrés", "Также удалить сохранённые настройки", "保存した設定も削除する", "Gespeicherte Einstellungen ebenfalls löschen");
        Add("Uninstall", "卸载", "अनइंस्टॉल", "Desinstalar", "Désinstaller", "Удалить", "アンインストール", "Deinstallieren");
        Add("Calendar", "日历", "कैलेंडर", "Calendario", "Calendrier", "Календарь", "カレンダー", "Kalender");
        Add("Name under the icon", "图标下的名称", "आइकन के नीचे नाम", "Nombre bajo el icono", "Nom sous l'icône", "Имя под значком", "アイコン下の名前", "Name unter dem Symbol");
        Add("Cancel", "取消", "रद्द करें", "Cancelar", "Annuler", "Отмена", "キャンセル", "Abbrechen");
        Add("Alarm", "闹钟", "अलार्म", "Alarma", "Alarme", "Будильник", "アラーム", "Wecker");
        Add("Enable alarm", "启用闹钟", "अलार्म चालू करें", "Activar alarma", "Activer l'alarme", "Включить будильник", "アラームを有効にする", "Wecker aktivieren");
        Add("Time (24 h) — every day", "时间（24小时）— 每天", "समय (24 घंटे) — हर दिन", "Hora (24 h) — todos los días", "Heure (24 h) — tous les jours", "Время (24 ч) — каждый день", "時刻（24時間）— 毎日", "Zeit (24 Std.) — täglich");
        Add("Snooze 5 min", "推迟 5 分钟", "5 मिनट बाद", "Posponer 5 min", "Reporter 5 min", "Отложить 5 мин", "5分後に再通知", "5 Min. zurückstellen");
        Add("Stop", "停止", "रोकें", "Detener", "Arrêter", "Стоп", "停止", "Stopp");
    }

    static void Add(string en, string zh, string hi, string es, string fr, string ru, string ja, string de)
    {
        Map[en] = new string[] { zh, hi, es, fr, ru, ja, de };
    }

    public static string Code
    {
        get { return _code; }
    }

    public static bool Fr
    {
        get { return _code == "fr"; }
    }

    public static CultureInfo Culture
    {
        get { return _culture; }
    }

    public static string Ietf
    {
        get
        {
            int i = IndexOf(_code);
            return IetfTags[i < 0 ? 0 : i];
        }
    }

    public static string LangName(string code)
    {
        int i = IndexOf(code);
        return i < 0 ? code : Names[i];
    }

    public static FontFamily UiFont
    {
        get
        {
            return new FontFamily("Segoe UI, Microsoft YaHei UI, Microsoft YaHei, Nirmala UI, Yu Gothic UI, Yu Gothic, Meiryo, Malgun Gothic");
        }
    }

    public static void ApplyToThread()
    {
        try
        {
            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _culture;
        }
        catch { }
    }

    public static string S(string en, string fr)
    {
        return T(en);
    }

    public static string T(string en)
    {
        if (en == null)
            return "";
        if (_code == "en")
            return en;
        string[] row;
        if (!Map.TryGetValue(en, out row) || row == null)
            return en;
        int i = IndexOf(_code) - 1;
        if (i < 0 || i >= row.Length || string.IsNullOrEmpty(row[i]))
            return en;
        return row[i];
    }

    public static void Init()
    {
        _code = "en";
        _culture = MakeCulture("en");
        try
        {
            string path = System.IO.Path.Combine(
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
            if (colon < 0 || colon > i + 40)
                return;
            int q1 = text.IndexOf('"', colon + 1);
            if (q1 < 0 || q1 > colon + 8)
                return;
            int q2 = text.IndexOf('"', q1 + 1);
            if (q2 < 0)
                return;
            string v = text.Substring(q1 + 1, q2 - q1 - 1);
            if (Normalize(v) == "fr")
            {
                _code = "fr";
                _culture = MakeCulture("fr");
            }
        }
        catch { }
        ApplyToThread();
    }

    public static bool Set(string code)
    {
        string n = Normalize(code);
        if (n == null)
            return false;
        if (n == _code)
            return false;
        _code = n;
        _culture = MakeCulture(n);
        ApplyToThread();
        return true;
    }

    public static void ApplySaved(string code)
    {
        string n = Normalize(code);
        if (n == null)
            return;
        _code = n;
        _culture = MakeCulture(n);
        ApplyToThread();
    }

    static string Normalize(string code)
    {
        if (code == null)
            return null;
        string s = code.Trim().ToLowerInvariant();
        if (s == "en" || s == "eng" || s == "english" || s.StartsWith("en-"))
            return "en";
        if (s == "zh" || s == "cn" || s == "zh-cn" || s == "zh-hans" || s == "mandarin" || s == "chinese")
            return "zh";
        if (s == "hi" || s == "hindi" || s.StartsWith("hi-"))
            return "hi";
        if (s == "es" || s == "spanish" || s == "espanol" || s == "español" || s.StartsWith("es-"))
            return "es";
        if (s == "fr" || s == "french" || s == "francais" || s == "français" || s.StartsWith("fr-"))
            return "fr";
        if (s == "ru" || s == "russian" || s.StartsWith("ru-"))
            return "ru";
        if (s == "ja" || s == "jp" || s == "japanese" || s.StartsWith("ja-"))
            return "ja";
        if (s == "de" || s == "german" || s == "deutsch" || s.StartsWith("de-"))
            return "de";
        return null;
    }

    static int IndexOf(string code)
    {
        for (int i = 0; i < Codes.Length; i++)
        {
            if (Codes[i] == code)
                return i;
        }
        return -1;
    }

    static CultureInfo MakeCulture(string code)
    {
        int i = IndexOf(code);
        string tag = IetfTags[i < 0 ? 0 : i];
        try
        {
            return CultureInfo.GetCultureInfo(tag);
        }
        catch
        {
            return CultureInfo.GetCultureInfo("en-US");
        }
    }
}

internal sealed class FlagDraw : FrameworkElement
{
    readonly string _code;
    readonly bool _on;

    public FlagDraw(string code, bool selected)
    {
        _code = code;
        _on = selected;
        Width = 24;
        Height = 16;
        IsHitTestVisible = true;
        SnapsToDevicePixels = true;
    }

    public string Code
    {
        get { return _code; }
    }

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
    {
        if (hitTestParameters == null)
            return null;
        Point p = hitTestParameters.HitPoint;
        if (p.X < 0 || p.Y < 0 || p.X > ActualWidth || p.Y > ActualHeight)
            return null;
        return new PointHitTestResult(this, p);
    }

    protected override void OnRender(DrawingContext dc)
    {
        double x = 2;
        double y = 2;
        double w = 20;
        double h = 12;
        DrawFlag(dc, _code, new Rect(x, y, w, h));
        Pen edge = new Pen(_on ? Brushes.White : new SolidColorBrush(Color.FromArgb(90, 255, 255, 255)), 1);
        dc.DrawRectangle(null, edge, new Rect(_on ? 0.5 : x - 0.5, _on ? 0.5 : y - 0.5, _on ? 23 : w + 1, _on ? 15 : h + 1));
    }

    static void DrawFlag(DrawingContext dc, string code, Rect r)
    {
        if (code == "en")
            DrawGb(dc, r);
        else if (code == "zh")
        {
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xDE, 0x29, 0x10)), null, r);
            dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(0xFF, 0xDE, 0x00)), null,
                new Point(r.X + r.Width * 0.28, r.Y + r.Height * 0.38), r.Height * 0.18, r.Height * 0.18);
        }
        else if (code == "hi")
        {
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xFF, 0x99, 0x33)), null, new Rect(r.X, r.Y, r.Width, r.Height / 3));
            dc.DrawRectangle(Brushes.White, null, new Rect(r.X, r.Y + r.Height / 3, r.Width, r.Height / 3));
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0x13, 0x88, 0x08)), null, new Rect(r.X, r.Y + 2 * r.Height / 3, r.Width, r.Height / 3));
            dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x80)), null,
                new Point(r.X + r.Width / 2, r.Y + r.Height / 2), r.Height * 0.14, r.Height * 0.14);
        }
        else if (code == "es")
        {
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xAA, 0x15, 0x1B)), null, r);
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xF1, 0xBF, 0x00)), null,
                new Rect(r.X, r.Y + r.Height * 0.25, r.Width, r.Height * 0.5));
        }
        else if (code == "fr")
        {
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0x00, 0x23, 0x95)), null, new Rect(r.X, r.Y, r.Width / 3, r.Height));
            dc.DrawRectangle(Brushes.White, null, new Rect(r.X + r.Width / 3, r.Y, r.Width / 3, r.Height));
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xED, 0x29, 0x39)), null, new Rect(r.X + 2 * r.Width / 3, r.Y, r.Width / 3, r.Height));
        }
        else if (code == "ru")
        {
            dc.DrawRectangle(Brushes.White, null, new Rect(r.X, r.Y, r.Width, r.Height / 3));
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0x00, 0x39, 0xA6)), null, new Rect(r.X, r.Y + r.Height / 3, r.Width, r.Height / 3));
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xD5, 0x2B, 0x1E)), null, new Rect(r.X, r.Y + 2 * r.Height / 3, r.Width, r.Height / 3));
        }
        else if (code == "ja")
        {
            dc.DrawRectangle(Brushes.White, null, r);
            dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(0xBC, 0x00, 0x2D)), null,
                new Point(r.X + r.Width / 2, r.Y + r.Height / 2), r.Height * 0.28, r.Height * 0.28);
        }
        else if (code == "de")
        {
            dc.DrawRectangle(Brushes.Black, null, new Rect(r.X, r.Y, r.Width, r.Height / 3));
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xDD, 0x00, 0x00)), null, new Rect(r.X, r.Y + r.Height / 3, r.Width, r.Height / 3));
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xFF, 0xCE, 0x00)), null, new Rect(r.X, r.Y + 2 * r.Height / 3, r.Width, r.Height / 3));
        }
    }

    static void DrawGb(DrawingContext dc, Rect r)
    {
        dc.PushClip(new RectangleGeometry(r));
        dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0x01, 0x21, 0x69)), null, r);
        Brush red = new SolidColorBrush(Color.FromRgb(0xC8, 0x10, 0x2E));
        double cx = r.X + r.Width / 2;
        double cy = r.Y + r.Height / 2;
        Pen whiteSaltire = new Pen(Brushes.White, r.Height * 0.28);
        dc.DrawLine(whiteSaltire, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y + r.Height));
        dc.DrawLine(whiteSaltire, new Point(r.X + r.Width, r.Y), new Point(r.X, r.Y + r.Height));
        Pen redSaltire = new Pen(red, r.Height * 0.10);
        dc.DrawLine(redSaltire, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y + r.Height));
        dc.DrawLine(redSaltire, new Point(r.X + r.Width, r.Y), new Point(r.X, r.Y + r.Height));
        Pen whiteCross = new Pen(Brushes.White, r.Height * 0.42);
        dc.DrawLine(whiteCross, new Point(cx, r.Y), new Point(cx, r.Y + r.Height));
        dc.DrawLine(whiteCross, new Point(r.X, cy), new Point(r.X + r.Width, cy));
        Pen redCross = new Pen(red, r.Height * 0.22);
        dc.DrawLine(redCross, new Point(cx, r.Y), new Point(cx, r.Y + r.Height));
        dc.DrawLine(redCross, new Point(r.X, cy), new Point(r.X + r.Width, cy));
        dc.Pop();
    }
}

internal static class ClockIpc
{
    const int WmCopyData = 0x004A;

    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr FindWindow(string cls, string title);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref COPYDATASTRUCT data);

    public static void Send(int kind, int x, int y)
    {
        IntPtr hwnd = IntPtr.Zero;
        for (int i = 0; i < 20 && hwnd == IntPtr.Zero; i++)
        {
            hwnd = FindWindow(null, "VAH ZTime");
            if (hwnd == IntPtr.Zero)
                Thread.Sleep(50);
        }
        if (hwnd == IntPtr.Zero)
            return;
        int[] payload = new int[] { x, y };
        GCHandle pin = GCHandle.Alloc(payload, GCHandleType.Pinned);
        try
        {
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = new IntPtr(kind);
            cds.cbData = 8;
            cds.lpData = pin.AddrOfPinnedObject();
            SendMessage(hwnd, WmCopyData, IntPtr.Zero, ref cds);
        }
        finally
        {
            pin.Free();
        }
    }
}

internal static class MenuDismiss
{
    delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
    delegate bool EnumProc(IntPtr hwnd, IntPtr param);

    const int WhMouseLl = 14;

    static HookProc _proc;
    static IntPtr _hook;
    static IntPtr _clock;
    static Action _close;

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint threadId);
    [DllImport("user32.dll")]
    static extern bool UnhookWindowsHookEx(IntPtr hook);
    [DllImport("user32.dll")]
    static extern IntPtr CallNextHookEx(IntPtr hook, int code, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    static extern bool EnumWindows(EnumProc callback, IntPtr param);
    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
    [DllImport("user32.dll")]
    static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

    struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static void Track(IntPtr clockHwnd, Action close)
    {
        Stop();
        _clock = clockHwnd;
        _close = close;
        _proc = OnHook;
        _hook = SetWindowsHookEx(WhMouseLl, _proc, IntPtr.Zero, 0);
    }

    public static void Stop()
    {
        _close = null;
        if (_hook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hook);
            _hook = IntPtr.Zero;
        }
    }

    static IntPtr OnHook(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0 && _close != null && lParam != IntPtr.Zero)
        {
            int msg = wParam.ToInt32();
            if (msg == 0x0201 || msg == 0x0204 || msg == 0x0207 || msg == 0x00A1 || msg == 0x00A4)
            {
                int x = Marshal.ReadInt32(lParam, 0);
                int y = Marshal.ReadInt32(lParam, 4);
                if (!OverMenu(x, y))
                {
                    Action close = _close;
                    _close = null;
                    if (close != null && Application.Current != null)
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, close);
                }
            }
        }
        return CallNextHookEx(_hook, code, wParam, lParam);
    }

    static bool OverMenu(int x, int y)
    {
        bool hit = false;
        uint self = (uint)Process.GetCurrentProcess().Id;
        EnumWindows(delegate(IntPtr hwnd, IntPtr param)
        {
            if (hwnd == _clock)
                return true;
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            if (pid != self || !IsWindowVisible(hwnd))
                return true;
            RECT rect;
            if (!GetWindowRect(hwnd, out rect))
                return true;
            if (rect.Right - rect.Left < 8 || rect.Bottom - rect.Top < 8)
                return true;
            if (x >= rect.Left && x < rect.Right && y >= rect.Top && y < rect.Bottom)
                hit = true;
            return true;
        }, IntPtr.Zero);
        return hit;
    }
}

internal static class Program
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetCurrentPackageFullName(ref int packageFullNameLength, IntPtr packageFullName);

    internal static bool IsPackaged()
    {
        try
        {
            int length = 0;
            int rc = GetCurrentPackageFullName(ref length, IntPtr.Zero);
            return rc != 15700;
        }
        catch
        {
            return false;
        }
    }

    [STAThread]
    private static void Main(string[] args)
    {
        FixMenuDropAlignment();
        SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
        Ui.Init();

        bool vah = false;
        bool reveal = false;
        bool menu = false;
        int menuX = 0;
        int menuY = 0;
        if (args != null)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], "--uninstall", StringComparison.OrdinalIgnoreCase))
                {
                    LaunchSetupUninstall();
                    return;
                }
                if (args[i] == "--vah")
                    vah = true;
                else if (args[i] == "--reveal")
                    reveal = true;
                else if (args[i] == "--menu" && i + 2 < args.Length)
                {
                    menu = true;
                    int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out menuX);
                    int.TryParse(args[i + 2], NumberStyles.Integer, CultureInfo.InvariantCulture, out menuY);
                    i += 2;
                }
            }
        }
        string mutexName = vah ? @"Local\ValheimAdminHelper.ZTime" : @"Local\ZTime.IconWidget";
        bool created;
        Mutex mutex = new Mutex(true, mutexName, out created);
        if (!created)
        {
            if (vah && menu)
                ClockIpc.Send(1, menuX, menuY);
            else if (vah && reveal)
                ClockIpc.Send(2, 0, 0);
            return;
        }

        Application app = new Application();
        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        app.Run(new ClockWindow(vah, vah && menu, menuX, menuY));
        GC.KeepAlive(mutex);
    }

    static void OnSystemParametersChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "MenuDropAlignment")
            FixMenuDropAlignment();
    }

    static void FixMenuDropAlignment()
    {
        if (!SystemParameters.MenuDropAlignment)
            return;
        FieldInfo field = typeof(SystemParameters).GetField(
            "_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
        if (field != null)
            field.SetValue(null, false);
    }

    internal static void LaunchSetupUninstall()
    {
        try
        {
            string setup = FindSetupExe(Assembly.GetExecutingAssembly().Location);
            if (setup == null)
                return;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = setup;
            psi.Arguments = "--uninstall";
            psi.UseShellExecute = true;
            Process.Start(psi);
        }
        catch { }
    }

    internal static string FindSetupExe(string clockExe)
    {
        if (clockExe == null || clockExe.Length == 0)
            return null;
        string dir = System.IO.Path.GetDirectoryName(clockExe);
        string beside = System.IO.Path.Combine(dir, "Setup.exe");
        if (File.Exists(beside))
            return beside;
        try
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\ZTime"))
            {
                if (key != null)
                {
                    object loc = key.GetValue("InstallLocation");
                    if (loc != null)
                    {
                        string fromReg = System.IO.Path.Combine(loc.ToString().Trim(), "Setup.exe");
                        if (File.Exists(fromReg))
                            return fromReg;
                    }
                }
            }
        }
        catch { }
        string installed = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "ZTime",
            "Setup.exe");
        if (File.Exists(installed))
            return installed;
        return null;
    }
}

internal sealed class ClockSettings
{
    public double X = double.NaN;
    public double Y = double.NaN;
    public bool TwentyFour = true;
    public bool Seconds = true;
    public bool Date = true;
    public bool Snap = true;
    public bool Topmost = true;
    public bool Locked;
    public bool Chime;
    public int OpacityPct = 100;
    public int Size = 1;
    public int Theme;
    public bool AlarmOn;
    public int AlarmH = 7;
    public int AlarmM;
    public string LabelText = "ZTime";
    public bool ShowLabel = true;
}

internal sealed class ClockWindow : Window
{
    const int GwlExstyle = -20;
    const int WsExToolwindow = 0x00000080;
    const uint SwpNosize = 0x0001;
    const uint SwpNomove = 0x0002;
    const uint SwpNoactivate = 0x0010;
    const uint SpiIconHorizontalSpacing = 0x000D;
    const uint SpiIconVerticalSpacing = 0x0018;
    const uint MonitorDefaultToNearest = 2;

    static readonly IntPtr HwndTopmost = new IntPtr(-1);
    static readonly double[] FaceW = { 76, 168, 224, 292 };
    static readonly double[] FaceH = { 54, 86, 116, 156 };
    static readonly Color[] ThemeLed =
    {
        Color.FromRgb(0x3D, 0xFF, 0x8A),
        Color.FromRgb(0xFF, 0xB0, 0x20),
        Color.FromRgb(0xFF, 0x4D, 0x4D),
        Color.FromRgb(0x4D, 0xA3, 0xFF),
        Color.FromRgb(0x2E, 0xFF, 0xFF),
        Color.FromRgb(0xF2, 0xF2, 0xF7)
    };
    static readonly Color[] ThemeFace =
    {
        Color.FromRgb(0x10, 0x12, 0x10),
        Color.FromRgb(0x14, 0x10, 0x08),
        Color.FromRgb(0x14, 0x0A, 0x0A),
        Color.FromRgb(0x08, 0x0E, 0x16),
        Color.FromRgb(0x08, 0x12, 0x14),
        Color.FromRgb(0x12, 0x12, 0x14)
    };
    static readonly Color[] ThemeBezel =
    {
        Color.FromRgb(0x2A, 0x3A, 0x2E),
        Color.FromRgb(0x3A, 0x2E, 0x14),
        Color.FromRgb(0x3A, 0x1A, 0x1A),
        Color.FromRgb(0x1A, 0x2A, 0x3A),
        Color.FromRgb(0x14, 0x32, 0x32),
        Color.FromRgb(0x3A, 0x3A, 0x40)
    };

    readonly ClockSettings _s = new ClockSettings();
    readonly LedPanel _led;
    readonly TextBlock _date;
    readonly TextBlock _ampm;
    readonly TextBlock _bell;
    readonly List<TextBlock> _labelParts = new List<TextBlock>();
    readonly Grid _labelHost;
    readonly Grid _statusRow;
    readonly Border _face;
    readonly Border _hit;
    readonly DispatcherTimer _clockTimer;
    readonly DispatcherTimer _topmostTimer;
    readonly DispatcherTimer _flashTimer;
    readonly DispatcherTimer _soundTimer;
    readonly string _settingsPath;
    readonly string _exePath;
    readonly string _startupLnk;
    readonly bool _vah;
    readonly int _menuX;
    readonly int _menuY;
    bool _menuHost;
    int _menuEpoch;
    bool _holdHost;
    readonly SolidColorBrush _faceBrush = new SolidColorBrush();
    readonly SolidColorBrush _bezelBrush = new SolidColorBrush();
    readonly SolidColorBrush _ledBrush = new SolidColorBrush();
    readonly SolidColorBrush _dimBrush = new SolidColorBrush();

    IntPtr _hwnd;
    double _dpi = 1.0;
    bool _hasPos;
    bool _reasserting;
    bool _flashOn;
    DateTime _alarmMutedDay = DateTime.MinValue;
    DateTime _snoozeUntil = DateTime.MinValue;
    int _lastChimeHour = -1;
    CalendarPopup _cal;
    AlarmAlert _alert;

    public ClockWindow(bool vah, bool menuHost, int menuX, int menuY)
    {
        _vah = vah;
        _menuHost = menuHost;
        _menuX = menuX;
        _menuY = menuY;
        _exePath = Assembly.GetExecutingAssembly().Location;
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        if (vah)
        {
            _settingsPath = System.IO.Path.Combine(appData, "ValheimAdminHelper", "clock.txt");
            _startupLnk = System.IO.Path.Combine(startup, "VAH ZTime.lnk");
            _s.Size = 1;
            _s.Theme = 0;
            _s.TwentyFour = true;
            _s.Snap = true;
            _s.Topmost = true;
            _s.OpacityPct = 60;
            _s.ShowLabel = false;
        }
        else
        {
            _settingsPath = System.IO.Path.Combine(appData, "ZTime", "settings.txt");
            _startupLnk = System.IO.Path.Combine(startup, "ZTime.lnk");
        }

        LoadSettings();

        Title = vah ? "VAH ZTime" : "ZTime";
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = _s.Topmost;
        ShowActivated = false;
        WindowStartupLocation = WindowStartupLocation.Manual;
        SizeToContent = SizeToContent.WidthAndHeight;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
        Cursor = Cursors.Arrow;

        _led = new LedPanel();
        _led.MinHeight = 24;
        _led.Margin = new Thickness(8, 2, 8, 0);

        Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);

        _date = new TextBlock();
        _date.FontFamily = Ui.UiFont;
        _date.Language = Language;
        _date.FontSize = 11;
        _date.HorizontalAlignment = HorizontalAlignment.Center;
        _date.TextAlignment = TextAlignment.Center;
        _date.Margin = new Thickness(4, 0, 4, 5);
        TextOptions.SetTextFormattingMode(_date, TextFormattingMode.Display);

        _ampm = new TextBlock();
        _ampm.FontFamily = new FontFamily("Segoe UI Semibold, Segoe UI, Microsoft YaHei UI, Nirmala UI, Yu Gothic UI");
        _ampm.FontSize = 9;
        _ampm.HorizontalAlignment = HorizontalAlignment.Right;
        _ampm.Margin = new Thickness(0, 3, 8, 0);

        _bell = new TextBlock();
        _bell.FontFamily = new FontFamily("Segoe MDL2 Assets");
        _bell.Text = "\uE855";
        _bell.FontSize = 10;
        _bell.HorizontalAlignment = HorizontalAlignment.Left;
        _bell.Margin = new Thickness(8, 3, 0, 0);
        _bell.Visibility = Visibility.Collapsed;

        _statusRow = new Grid();
        _statusRow.MinHeight = 0;
        _statusRow.Children.Add(_bell);
        _statusRow.Children.Add(_ampm);

        Grid inner = new Grid();
        inner.RowDefinitions.Add(new RowDefinition());
        inner.RowDefinitions[0].Height = GridLength.Auto;
        inner.RowDefinitions.Add(new RowDefinition());
        inner.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
        inner.RowDefinitions.Add(new RowDefinition());
        inner.RowDefinitions[2].Height = GridLength.Auto;
        Grid.SetRow(_statusRow, 0);
        Grid.SetRow(_led, 1);
        Grid.SetRow(_date, 2);
        inner.Children.Add(_statusRow);
        inner.Children.Add(_led);
        inner.Children.Add(_date);

        _face = new Border();
        _face.CornerRadius = new CornerRadius(12);
        _face.BorderThickness = new Thickness(1);
        _face.Background = _faceBrush;
        _face.BorderBrush = _bezelBrush;
        _face.Child = inner;
        _face.HorizontalAlignment = HorizontalAlignment.Center;

        _labelHost = new Grid();
        _labelHost.HorizontalAlignment = HorizontalAlignment.Center;
        _labelHost.Margin = new Thickness(0, 3, 0, 0);
        int[] ox = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] oy = { -1, 0, 1, -1, 1, -1, 0, 1 };
        for (int i = 0; i < 8; i++)
        {
            TextBlock halo = IconLabel("ZTime", Brushes.Black);
            halo.Margin = new Thickness(ox[i], oy[i], -ox[i], -oy[i]);
            _labelHost.Children.Add(halo);
            _labelParts.Add(halo);
        }
        TextBlock label = IconLabel("ZTime", Brushes.White);
        _labelHost.Children.Add(label);
        _labelParts.Add(label);

        StackPanel stack = new StackPanel();
        stack.HorizontalAlignment = HorizontalAlignment.Center;
        stack.Margin = new Thickness(0, 2, 0, 0);
        stack.Children.Add(_face);
        stack.Children.Add(_labelHost);

        _hit = new Border();
        _hit.CornerRadius = new CornerRadius(6);
        _hit.Padding = new Thickness(4, 2, 4, 2);
        _hit.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
        _hit.Child = stack;
        Content = _hit;

        ContextMenuOpening += delegate { ContextMenu = BuildMenu(); };
        _hit.ContextMenuOpening += delegate { _hit.ContextMenu = BuildMenu(); };
        ContextMenuService.SetPlacement(this, PlacementMode.Custom);
        ContextMenuService.SetPlacement(_hit, PlacementMode.Custom);
        ContextMenu = BuildMenu();
        _hit.ContextMenu = ContextMenu;

        MouseEnter += delegate { ShowHover(true); };
        MouseLeave += delegate { ShowHover(false); };
        MouseLeftButtonDown += OnLeftDown;
        SourceInitialized += OnSourceInit;
        Closing += OnClosing;

        _clockTimer = new DispatcherTimer();
        _clockTimer.Interval = TimeSpan.FromMilliseconds(100);
        _clockTimer.Tick += delegate { TickClock(); };

        _topmostTimer = new DispatcherTimer();
        _topmostTimer.Interval = TimeSpan.FromSeconds(3);
        _topmostTimer.Tick += delegate { ReassertTopmost(); };

        _flashTimer = new DispatcherTimer();
        _flashTimer.Interval = TimeSpan.FromMilliseconds(280);
        _flashTimer.Tick += OnFlashTick;

        _soundTimer = new DispatcherTimer();
        _soundTimer.Interval = TimeSpan.FromSeconds(2);
        _soundTimer.Tick += delegate { SystemSounds.Exclamation.Play(); };

        ApplyTheme();
        ApplySize();
        ApplyOpacity();
        ApplyLabel();
        TickClock();
        if (_menuHost)
        {
            Left = -20000;
            Top = -20000;
            Topmost = false;
            IsHitTestVisible = false;
        }
    }

    ContextMenu BuildMenu()
    {
        ContextMenu menu = new ContextMenu();
        menu.Placement = PlacementMode.Custom;
        menu.PlacementTarget = _hit;
        menu.CustomPopupPlacementCallback = PlaceMainMenu;
        FillMenu(menu);
        menu.Opened += OnFlagsMenuOpened;
        return menu;
    }

    void OnFlagsMenuOpened(object sender, RoutedEventArgs e)
    {
        ContextMenu menu = sender as ContextMenu;
        if (menu == null)
            return;
        MenuItem wrap = menu.ItemContainerGenerator.ContainerFromIndex(0) as MenuItem;
        if (wrap == null)
            return;
        wrap.StaysOpenOnClick = true;
        wrap.Focusable = false;
        wrap.Padding = new Thickness(4, 2, 4, 2);
    }

    void FillMenu(ContextMenu menu)
    {
        menu.Items.Clear();
        menu.FontFamily = Ui.UiFont;
        menu.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);

        Menu bar = new Menu();
        bar.Background = Brushes.Transparent;
        bar.BorderThickness = new Thickness(0);
        bar.Padding = new Thickness(0);
        bar.IsMainMenu = false;
        FrameworkElementFactory sp = new FrameworkElementFactory(typeof(StackPanel));
        sp.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
        bar.ItemsPanel = new ItemsPanelTemplate(sp);
        for (int i = 0; i < Ui.Codes.Length; i++)
        {
            string code = Ui.Codes[i];
            MenuItem flag = new MenuItem();
            flag.StaysOpenOnClick = true;
            flag.Tag = code;
            flag.ToolTip = Ui.LangName(code);
            flag.Padding = new Thickness(3, 1, 3, 1);
            flag.Header = new FlagDraw(code, Ui.Code == code);
            flag.Click += OnLangMenuClick;
            bar.Items.Add(flag);
        }
        bar.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(OnLangMenuClick), true);
        menu.Items.Add(bar);

        menu.Items.Add(Item(Ui.S("Open calendar", "Ouvrir le calendrier"), false, false, delegate { OpenCalendar(); }));
        menu.Items.Add(Item(Ui.S("Copy the time", "Copier l'heure"), false, false, delegate { CopyTime(); }));
        menu.Items.Add(Item(Ui.S("Alarm...", "Alarme..."), false, false, delegate { OpenAlarm(); }));
        menu.Items.Add(new Separator());

        string[] sizeNames = {
            Ui.S("Small", "Petite"),
            Ui.S("Normal", "Normale"),
            Ui.S("Large", "Grande"),
            Ui.S("Extra", "Extra")
        };
        MenuItem sizes = new MenuItem();
        sizes.Header = Ui.S("Size", "Taille");
        for (int i = 0; i < sizeNames.Length; i++)
        {
            int idx = i;
            sizes.Items.Add(Item(sizeNames[i], true, _s.Size == i, delegate { _s.Size = idx; ApplySize(); SaveSettings(); }));
        }
        PreferSubmenuBeside(sizes);
        menu.Items.Add(sizes);

        string[] themeNames = {
            Ui.S("Green", "Vert"),
            Ui.S("Amber", "Ambre"),
            Ui.S("Red", "Rouge"),
            Ui.S("Blue", "Bleu"),
            Ui.S("Cyan", "Cyan"),
            Ui.S("White", "Blanc")
        };
        MenuItem themes = new MenuItem();
        themes.Header = Ui.S("Color", "Couleur");
        for (int i = 0; i < themeNames.Length; i++)
        {
            int idx = i;
            themes.Items.Add(Item(themeNames[i], true, _s.Theme == i, delegate { _s.Theme = idx; ApplyTheme(); TickClock(); SaveSettings(); }));
        }
        PreferSubmenuBeside(themes);
        menu.Items.Add(themes);

        MenuItem display = new MenuItem();
        display.Header = Ui.S("Display", "Affichage");
        display.Items.Add(Item(Ui.S("Seconds", "Secondes"), true, _s.Seconds, delegate { _s.Seconds = !_s.Seconds; TickClock(); SaveSettings(); }));
        display.Items.Add(Item(Ui.S("Date", "Date"), true, _s.Date, delegate { _s.Date = !_s.Date; TickClock(); SaveSettings(); }));
        display.Items.Add(Item(Ui.S("24-hour format", "Format 24 heures"), true, _s.TwentyFour, delegate { _s.TwentyFour = !_s.TwentyFour; TickClock(); SaveSettings(); }));
        display.Items.Add(Item(Ui.S("Hour chime", "Carillon des heures"), true, _s.Chime, delegate { _s.Chime = !_s.Chime; SaveSettings(); }));
        PreferSubmenuBeside(display);
        menu.Items.Add(display);

        MenuItem opac = new MenuItem();
        opac.Header = Ui.S("Opacity", "Opacite");
        int[] pcts = { 100, 80, 60 };
        for (int i = 0; i < pcts.Length; i++)
        {
            int p = pcts[i];
            opac.Items.Add(Item(p + " %", true, _s.OpacityPct == p, delegate { _s.OpacityPct = p; ApplyOpacity(); SaveSettings(); }));
        }
        PreferSubmenuBeside(opac);
        menu.Items.Add(opac);

        MenuItem nom = new MenuItem();
        nom.Header = Ui.S("Name", "Nom");
        nom.Items.Add(Item(Ui.S("Show the name", "Afficher le nom"), true, _s.ShowLabel, delegate
        {
            _s.ShowLabel = !_s.ShowLabel;
            ApplyLabel();
            SaveSettings();
        }));
        nom.Items.Add(Item(Ui.S("Change the name...", "Modifier le nom..."), false, false, delegate { RenameLabel(); }));
        PreferSubmenuBeside(nom);
        menu.Items.Add(nom);

        menu.Items.Add(new Separator());
        menu.Items.Add(Item(Ui.S("Snap to grid", "Aligner sur la grille"), true, _s.Snap, delegate
        {
            _s.Snap = !_s.Snap;
            if (_s.Snap && !_menuHost) SnapToGrid();
            SaveSettings();
        }));
        menu.Items.Add(Item(Ui.S("Lock position", "Verrouiller la position"), true, _s.Locked, delegate { _s.Locked = !_s.Locked; SaveSettings(); }));
        menu.Items.Add(Item(Ui.S("Always on top", "Toujours visible"), true, _s.Topmost, delegate
        {
            _s.Topmost = !_s.Topmost;
            if (!_menuHost)
            {
                Topmost = _s.Topmost;
                ReassertTopmost();
            }
            SaveSettings();
        }));
        if (Program.IsPackaged())
        {
            menu.Items.Add(Item(Ui.S("Run at startup", "Lancer au demarrage"), false, false, delegate
            {
                try { Process.Start("ms-settings:startupapps"); }
                catch { }
            }));
        }
        else
        {
            menu.Items.Add(Item(Ui.S("Run at startup", "Lancer au demarrage"), true, File.Exists(_startupLnk), delegate
            {
                if (File.Exists(_startupLnk))
                {
                    try { File.Delete(_startupLnk); }
                    catch { }
                    }
                else
                    CreateStartupShortcut();
            }));
        }
        menu.Items.Add(Item(Ui.S("Windows date and time", "Date et heure Windows"), false, false, delegate
        {
            try { Process.Start("ms-settings:dateandtime"); }
            catch { }
        }));
        menu.Items.Add(new Separator());
        menu.Items.Add(Item("© Fred Zarma 2026  (zarma@sylm.info)", false, false, delegate
        {
            try { Process.Start("mailto:zarma@sylm.info"); }
            catch { }
        }));
        if (!_vah && !Program.IsPackaged())
            menu.Items.Add(Item(Ui.S("Uninstall...", "Désinstaller..."), false, false, delegate { RequestUninstall(); }));
        menu.Items.Add(Item(Ui.S("Close", "Fermer"), false, false, delegate { Close(); }));
    }

    void OnLangMenuClick(object sender, RoutedEventArgs e)
    {
        string code = null;
        MenuItem mi = sender as MenuItem;
        if (mi != null)
            code = mi.Tag as string;
        if (code == null)
        {
            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            while (fe != null && code == null)
            {
                code = fe.Tag as string;
                FlagDraw draw = fe as FlagDraw;
                if (draw != null)
                    code = draw.Code;
                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            }
        }
        if (code == null)
            return;
        e.Handled = true;
        Dispatcher.BeginInvoke(new Action(delegate { ApplyLanguage(code); }));
    }

    void ApplyUiLanguage()
    {
        System.Windows.Markup.XmlLanguage lang = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        Language = lang;
        _date.Language = lang;
        _date.FontFamily = Ui.UiFont;
        TickClock();
    }

    void ApplyLanguage(string code)
    {
        if (!Ui.Set(code))
            return;
        SaveSettings();
        ApplyUiLanguage();
        RelocalizeCalendar();
        if (ContextMenu != null)
            FillMenu(ContextMenu);
        if (_hit.ContextMenu != null && _hit.ContextMenu != ContextMenu)
            FillMenu(_hit.ContextMenu);
    }

    void RequestUninstall()
    {
        string setup = Program.FindSetupExe(_exePath);
        if (setup != null)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = setup;
                psi.Arguments = "--uninstall";
                psi.UseShellExecute = true;
                Process.Start(psi);
            }
            catch { }
            return;
        }
        _holdHost = true;
        try
        {
            UninstallDialog dlg = new UninstallDialog();
            dlg.Owner = this;
            bool? ok = dlg.ShowDialog();
            if (ok == true)
            {
                if (dlg.WipeSettings)
                {
                    try
                    {
                        string dir = System.IO.Path.GetDirectoryName(_settingsPath);
                        string folder = dir == null ? "" : System.IO.Path.GetFileName(dir.TrimEnd('\\', '/'));
                        if (dir != null && Directory.Exists(dir) && string.Equals(folder, "ZTime", StringComparison.OrdinalIgnoreCase))
                            Directory.Delete(dir, true);
                    }
                    catch { }
                }
                try
                {
                    if (File.Exists(_startupLnk))
                        File.Delete(_startupLnk);
                }
                catch { }
                Close();
            }
        }
        finally
        {
            _holdHost = false;
        }
    }

    void RelocalizeCalendar()
    {
        if (_cal == null)
            return;
        _holdHost = true;
        try
        {
            try { _cal.Close(); }
            catch { }
            OpenCalendar();
        }
        finally
        {
            _holdHost = false;
        }
    }

    static TextBlock IconLabel(string text, Brush color)
    {
        TextBlock t = new TextBlock();
        t.Text = text;
        t.FontFamily = new FontFamily("Segoe UI");
        t.FontSize = 12;
        t.Foreground = color;
        t.HorizontalAlignment = HorizontalAlignment.Center;
        t.TextAlignment = TextAlignment.Center;
        TextOptions.SetTextFormattingMode(t, TextFormattingMode.Display);
        return t;
    }

    static MenuItem Item(string header, bool checkable, bool isChecked, RoutedEventHandler click)
    {
        MenuItem mi = new MenuItem();
        mi.Header = header;
        mi.IsCheckable = checkable;
        mi.IsChecked = isChecked;
        mi.Click += click;
        return mi;
    }

    static void PreferSubmenuBeside(MenuItem item)
    {
        item.SubmenuOpened += OnSubmenuOpened;
    }

    static void OnSubmenuOpened(object sender, RoutedEventArgs e)
    {
        MenuItem item = sender as MenuItem;
        if (item == null)
            return;
        item.ApplyTemplate();
        Popup popup = null;
        if (item.Template != null)
            popup = item.Template.FindName("PART_Popup", item) as Popup;
        if (popup == null)
            return;

        popup.Placement = PlacementMode.Custom;
        popup.PlacementTarget = item;
        popup.CustomPopupPlacementCallback = delegate(Size popupSize, Size targetSize, Point offset)
        {
            return PlaceFlyoutBeside(item, popupSize, targetSize);
        };
        popup.HorizontalOffset = 0;
        popup.VerticalOffset = 0;
    }

    static CustomPopupPlacement[] PlaceFlyoutBeside(FrameworkElement target, Size popupSize, Size targetSize)
    {
        Rect work = WorkAreaFrom(target);
        Point origin = target.PointToScreen(new Point(0, 0));
        double dpi = DpiFrom(target);
        double left = origin.X / dpi;
        double spaceRight = work.Right - (left + targetSize.Width);
        double spaceLeft = left - work.Left;
        bool openRight = spaceRight >= popupSize.Width || spaceRight >= spaceLeft;
        if (openRight)
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point(targetSize.Width, 0), PopupPrimaryAxis.Vertical)
            };
        return new CustomPopupPlacement[]
        {
            new CustomPopupPlacement(new Point(-popupSize.Width, 0), PopupPrimaryAxis.Vertical)
        };
    }

    CustomPopupPlacement[] PlaceMainMenu(Size popupSize, Size targetSize, Point offset)
    {
        const double submenuRoom = 240;
        Rect work = WorkAreaDip();
        Point screen = _hit.PointToScreen(new Point(0, 0));
        double left = screen.X / _dpi;
        double room = submenuRoom;
        if (popupSize.Width + room > work.Width)
            room = Math.Max(0, work.Width - popupSize.Width);

        double desiredLeft = left;
        if (desiredLeft + popupSize.Width + room > work.Right)
            desiredLeft = work.Right - popupSize.Width - room;
        if (desiredLeft < work.Left)
            desiredLeft = work.Left;
        double dx = desiredLeft - left;

        return new CustomPopupPlacement[]
        {
            new CustomPopupPlacement(new Point(dx, targetSize.Height), PopupPrimaryAxis.Horizontal),
            new CustomPopupPlacement(new Point(dx, -popupSize.Height), PopupPrimaryAxis.Horizontal)
        };
    }

    static double DpiFrom(Visual visual)
    {
        PresentationSource src = PresentationSource.FromVisual(visual);
        if (src != null && src.CompositionTarget != null)
        {
            double dpi = src.CompositionTarget.TransformToDevice.M11;
            if (dpi > 0)
                return dpi;
        }
        return 1.0;
    }

    static Rect WorkAreaFrom(Visual visual)
    {
        PresentationSource src = PresentationSource.FromVisual(visual);
        IntPtr hwnd = IntPtr.Zero;
        double dpi = 1.0;
        if (src != null)
        {
            HwndSource hs = src as HwndSource;
            if (hs != null)
                hwnd = hs.Handle;
            if (src.CompositionTarget != null)
            {
                dpi = src.CompositionTarget.TransformToDevice.M11;
                if (dpi <= 0)
                    dpi = 1.0;
            }
        }
        if (hwnd != IntPtr.Zero)
        {
            IntPtr mon = Native.MonitorFromWindow(hwnd, 2);
            Native.MONITORINFO info = new Native.MONITORINFO();
            info.cbSize = Marshal.SizeOf(typeof(Native.MONITORINFO));
            if (mon != IntPtr.Zero && Native.GetMonitorInfo(mon, ref info))
            {
                Native.RECT r = info.rcWork;
                return new Rect(r.Left / dpi, r.Top / dpi, (r.Right - r.Left) / dpi, (r.Bottom - r.Top) / dpi);
            }
        }
        return SystemParameters.WorkArea;
    }

    void OnSourceInit(object sender, EventArgs e)
    {
        HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
        _hwnd = source.Handle;
        _dpi = source.CompositionTarget.TransformToDevice.M11;
        if (_dpi <= 0)
            _dpi = 1.0;

        IntPtr ex = Native.GetWindowLongPtr(_hwnd, GwlExstyle);
        Native.SetWindowLongPtr(_hwnd, GwlExstyle, new IntPtr(ex.ToInt64() | WsExToolwindow));
        source.AddHook(WndProc);

        ApplySize();

        if (_menuHost)
        {
            Left = -20000;
            Top = -20000;
            Topmost = false;
            _clockTimer.Start();
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate { OpenHostedMenu(_menuX, _menuY); }));
            return;
        }

        if (_hasPos)
            ClampToWorkArea();
        else
            PlaceAsNewIcon();

        if (_s.Snap)
            SnapToGrid();

        _clockTimer.Start();
        if (_s.Topmost)
            _topmostTimer.Start();
        ReassertTopmost();
    }

    IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != 0x004A)
            return IntPtr.Zero;
        ClockIpc.COPYDATASTRUCT cds = (ClockIpc.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(ClockIpc.COPYDATASTRUCT));
        int kind = cds.dwData.ToInt32();
        int x = 0;
        int y = 0;
        if (cds.cbData >= 8 && cds.lpData != IntPtr.Zero)
        {
            x = Marshal.ReadInt32(cds.lpData, 0);
            y = Marshal.ReadInt32(cds.lpData, 4);
        }
        if (kind == 1)
        {
            int px = x;
            int py = y;
            Dispatcher.BeginInvoke(new Action(delegate { OpenHostedMenu(px, py); }));
        }
        else if (kind == 2)
            Dispatcher.BeginInvoke(new Action(delegate { Reveal(); }));
        handled = true;
        return IntPtr.Zero;
    }

    void OpenHostedMenu(int x, int y)
    {
        _menuEpoch++;
        int epoch = _menuEpoch;
        ContextMenu menu = BuildMenu();
        menu.Placement = PlacementMode.Absolute;
        menu.CustomPopupPlacementCallback = null;
        menu.PlacementTarget = this;
        menu.HorizontalOffset = x / _dpi;
        menu.VerticalOffset = y / _dpi;
        menu.StaysOpen = false;
        menu.Closed += delegate
        {
            MenuDismiss.Stop();
            int mine = epoch;
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate { MaybeCloseHost(mine); }));
        };
        ContextMenu = menu;
        MenuDismiss.Track(_hwnd, delegate
        {
            if (menu.IsOpen)
                menu.IsOpen = false;
        });
        menu.IsOpen = true;
    }

    void MaybeCloseHost(int epoch)
    {
        if (epoch != _menuEpoch)
            return;
        if (!_menuHost)
            return;
        if (_holdHost)
            return;
        if (_cal != null)
            return;
        if (OwnedWindows != null && OwnedWindows.Count > 0)
            return;
        Close();
    }

    void Reveal()
    {
        _menuEpoch++;
        if (!_menuHost)
            return;
        _menuHost = false;
        IsHitTestVisible = true;
        if (!double.IsNaN(_s.X) && !double.IsNaN(_s.Y))
        {
            Left = _s.X;
            Top = _s.Y;
            _hasPos = true;
            ClampToWorkArea();
        }
        else
            PlaceAsNewIcon();
        if (_s.Snap)
            SnapToGrid();
        Topmost = _s.Topmost;
        if (_s.Topmost && !_topmostTimer.IsEnabled)
            _topmostTimer.Start();
        ReassertTopmost();
        if (ContextMenu != null)
            ContextMenu.IsOpen = false;
    }

    void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        MenuDismiss.Stop();
        StopAlarmUi();
        if (_cal != null)
        {
            try { _cal.Close(); }
            catch { }
        }
        if (!_menuHost)
            SaveSettings();
    }

    void ApplyTheme()
    {
        int t = _s.Theme;
        if (t < 0 || t >= ThemeLed.Length)
            t = 0;
        Color led = ThemeLed[t];
        _ledBrush.Color = led;
        _dimBrush.Color = Color.FromArgb(40, led.R, led.G, led.B);
        _faceBrush.Color = ThemeFace[t];
        _bezelBrush.Color = ThemeBezel[t];
        _date.Foreground = _ledBrush;
        _ampm.Foreground = _ledBrush;
        _bell.Foreground = _ledBrush;
        _led.OnBrush = _ledBrush;
        _led.OffBrush = _dimBrush;
        DropShadowEffect glow = new DropShadowEffect();
        glow.Color = led;
        glow.BlurRadius = 8;
        glow.ShadowDepth = 0;
        glow.Opacity = 0.7;
        _led.Effect = glow;
        _led.InvalidateVisual();
    }

    void ApplySize()
    {
        int i = _s.Size;
        if (i < 0 || i >= FaceW.Length)
            i = 1;
        _face.Width = FaceW[i];
        _face.Height = FaceH[i];
        _face.CornerRadius = new CornerRadius(Math.Max(8, FaceH[i] * 0.14));
        _date.FontSize = i == 0 ? 9 : (i == 1 ? 11 : (i == 2 ? 13 : 16));
        _ampm.FontSize = i == 0 ? 8 : 10;
        _bell.FontSize = i == 0 ? 9 : 11;
        _led.Margin = new Thickness(i == 0 ? 5 : 10, 1, i == 0 ? 5 : 10, 0);
        _date.Margin = new Thickness(4, 0, 4, i == 0 ? 3 : 6);
    }

    void ApplyOpacity()
    {
        double o = _s.OpacityPct / 100.0;
        if (o < 0.4) o = 0.4;
        if (o > 1) o = 1;
        _face.Opacity = o;
    }

    void ApplyLabel()
    {
        string t = _s.LabelText;
        if (t == null || t.Trim().Length == 0)
            t = "ZTime";
        for (int i = 0; i < _labelParts.Count; i++)
            _labelParts[i].Text = t;
        _labelHost.Visibility = _s.ShowLabel ? Visibility.Visible : Visibility.Collapsed;
    }

    void RenameLabel()
    {
        _holdHost = true;
        bool? ok = false;
        try
        {
            RenameDialog dlg = new RenameDialog(_s.LabelText);
            dlg.Owner = this;
            if (_menuHost)
                dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ok = dlg.ShowDialog();
            if (ok == true)
            {
                _s.LabelText = dlg.LabelText;
                ApplyLabel();
                SaveSettings();
            }
        }
        finally
        {
            _holdHost = false;
            if (_menuHost)
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate { MaybeCloseHost(_menuEpoch); }));
        }
    }

    void ShowHover(bool on)
    {
        if (on)
            _hit.Background = new SolidColorBrush(Color.FromArgb(48, 255, 255, 255));
        else
            _hit.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
    }

    void OnLeftDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;
        if (e.ClickCount == 2)
        {
            OpenCalendar();
            return;
        }
        if (_s.Locked)
            return;
        try { DragMove(); }
        catch (InvalidOperationException) { }
        if (_s.Snap)
            SnapToGrid();
        ClampToWorkArea();
        SaveSettings();
    }

    void TickClock()
    {
        DateTime now = DateTime.Now;
        int hour = now.Hour;
        bool pm = hour >= 12;
        if (!_s.TwentyFour)
        {
            hour = hour % 12;
            if (hour == 0)
                hour = 12;
        }

        string h = hour.ToString("00", CultureInfo.InvariantCulture);
        string m = now.Minute.ToString("00", CultureInfo.InvariantCulture);
        string sec = now.Second.ToString("00", CultureInfo.InvariantCulture);
        _led.Text = _s.Seconds ? (h + ":" + m + ":" + sec) : (h + ":" + m);
        _led.ColonOn = now.Millisecond < 500;

        if (_s.TwentyFour)
        {
            _ampm.Text = "";
            _ampm.Visibility = Visibility.Collapsed;
        }
        else
        {
            string am = Ui.Culture.DateTimeFormat.AMDesignator;
            string pmTxt = Ui.Culture.DateTimeFormat.PMDesignator;
            if (am == null || am.Length == 0) am = "AM";
            if (pmTxt == null || pmTxt.Length == 0) pmTxt = "PM";
            _ampm.Text = pm ? pmTxt : am;
            _ampm.Visibility = Visibility.Visible;
        }

        if (_s.Date)
        {
            _date.Text = now.ToString(_s.Size == 0 ? "ddd d" : "ddd d MMM", Ui.Culture);
            _date.Visibility = Visibility.Visible;
        }
        else
        {
            _date.Text = "";
            _date.Visibility = Visibility.Collapsed;
        }

        _bell.Visibility = _s.AlarmOn ? Visibility.Visible : Visibility.Collapsed;
        _statusRow.Visibility = (_s.AlarmOn || !_s.TwentyFour) ? Visibility.Visible : Visibility.Collapsed;

        ToolTip = now.ToString("dddd d MMMM yyyy", Ui.Culture) + Environment.NewLine + now.ToString(_s.TwentyFour ? "HH:mm:ss" : "h:mm:ss tt", Ui.Culture);

        if (!_menuHost && _s.Chime && now.Minute == 0 && now.Second == 0 && _lastChimeHour != now.Hour)
        {
            _lastChimeHour = now.Hour;
            SystemSounds.Asterisk.Play();
        }
        if (now.Second != 0)
            _lastChimeHour = -1;

        if (!_menuHost)
            CheckAlarm(now);
    }

    void CheckAlarm(DateTime now)
    {
        if (_alert != null)
            return;
        if (_snoozeUntil != DateTime.MinValue)
        {
            if (now >= _snoozeUntil)
            {
                _snoozeUntil = DateTime.MinValue;
                FireAlarm();
            }
            return;
        }
        if (!_s.AlarmOn)
            return;
        if (_alarmMutedDay.Date == now.Date)
            return;
        if (now.Hour == _s.AlarmH && now.Minute == _s.AlarmM && now.Second < 2)
            FireAlarm();
    }

    void FireAlarm()
    {
        SystemSounds.Exclamation.Play();
        _flashOn = false;
        _flashTimer.Start();
        _soundTimer.Start();
        _alert = new AlarmAlert(_s.AlarmH, _s.AlarmM);
        _alert.Owner = this;
        _alert.Closed += delegate
        {
            bool snooze = _alert != null && _alert.Snooze;
            _alert = null;
            StopAlarmUi();
            if (snooze)
                _snoozeUntil = DateTime.Now.AddMinutes(5);
            else
                _alarmMutedDay = DateTime.Today;
        };
        _alert.Show();
    }

    void StopAlarmUi()
    {
        _flashTimer.Stop();
        _soundTimer.Stop();
        _flashOn = false;
        _face.Background = _faceBrush;
        if (_alert != null)
        {
            AlarmAlert a = _alert;
            _alert = null;
            try { a.Close(); }
            catch { }
        }
    }

    void OnFlashTick(object sender, EventArgs e)
    {
        _flashOn = !_flashOn;
        if (_flashOn)
            _face.Background = new SolidColorBrush(Color.FromArgb(80, _ledBrush.Color.R, _ledBrush.Color.G, _ledBrush.Color.B));
        else
            _face.Background = _faceBrush;
    }

    void OpenCalendar()
    {
        if (_cal != null)
        {
            _cal.Activate();
            return;
        }
        _cal = new CalendarPopup(DateTime.Today);
        _cal.Owner = this;
        _cal.Closed += delegate
        {
            _cal = null;
            if (_menuHost)
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate { MaybeCloseHost(_menuEpoch); }));
        };
        UpdateLayout();
        if (_menuHost)
        {
            _cal.Left = _menuX / _dpi;
            _cal.Top = _menuY / _dpi;
        }
        else
        {
            Point p = _face.PointToScreen(new Point(0, _face.ActualHeight + 6));
            _cal.Left = p.X / _dpi;
            _cal.Top = p.Y / _dpi;
        }
        Rect work = WorkAreaDip();
        _cal.Show();
        _cal.UpdateLayout();
        if (_cal.Left + _cal.ActualWidth > work.Right)
            _cal.Left = work.Right - _cal.ActualWidth;
        if (_cal.Top + _cal.ActualHeight > work.Bottom)
            _cal.Top = Top - _cal.ActualHeight - 4;
        if (_cal.Left < work.Left)
            _cal.Left = work.Left;
        if (_cal.Top < work.Top)
            _cal.Top = work.Top;
    }

    void OpenAlarm()
    {
        _holdHost = true;
        bool? ok = false;
        try
        {
            AlarmDialog dlg = new AlarmDialog(_s.AlarmOn, _s.AlarmH, _s.AlarmM);
            dlg.Owner = this;
            if (_menuHost)
                dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ok = dlg.ShowDialog();
            if (ok == true)
            {
                _s.AlarmOn = dlg.AlarmOn;
                _s.AlarmH = dlg.Hour;
                _s.AlarmM = dlg.Minute;
                _alarmMutedDay = DateTime.MinValue;
                TickClock();
                SaveSettings();
            }
        }
        finally
        {
            _holdHost = false;
            if (_menuHost)
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate { MaybeCloseHost(_menuEpoch); }));
        }
    }

    void CopyTime()
    {
        try
        {
            DateTime now = DateTime.Now;
            Clipboard.SetText(now.ToString("dddd d MMMM yyyy HH:mm:ss", Ui.Culture));
        }
        catch { }
    }

    void PlaceAsNewIcon()
    {
        Rect work = WorkAreaDip();
        double gx, gy;
        GridSize(out gx, out gy);
        Left = work.Left + gx;
        Top = work.Top;
    }

    void SnapToGrid()
    {
        double gx, gy;
        GridSize(out gx, out gy);
        if (gx < 8 || gy < 8)
            return;
        Rect work = WorkAreaDip();
        double col = Math.Round((Left - work.Left) / gx);
        double row = Math.Round((Top - work.Top) / gy);
        Left = work.Left + col * gx;
        Top = work.Top + row * gy;
        ClampToWorkArea();
    }

    void ClampToWorkArea()
    {
        Rect work = WorkAreaDip();
        double w = ActualWidth > 1 ? ActualWidth : FaceW[_s.Size] + 16;
        double h = ActualHeight > 1 ? ActualHeight : FaceH[_s.Size] + 28;
        if (Left + w > work.Right)
            Left = work.Right - w;
        if (Top + h > work.Bottom)
            Top = work.Bottom - h;
        if (Left < work.Left)
            Left = work.Left;
        if (Top < work.Top)
            Top = work.Top;
    }

    void GridSize(out double gx, out double gy)
    {
        int hx = 75;
        int vx = 101;
        Native.SystemParametersInfo(SpiIconHorizontalSpacing, 0, ref hx, 0);
        Native.SystemParametersInfo(SpiIconVerticalSpacing, 0, ref vx, 0);
        gx = hx / _dpi;
        gy = vx / _dpi;
        if (gx < 48) gx = 75;
        if (gy < 48) gy = 101;
    }

    Rect WorkAreaDip()
    {
        IntPtr handle = _hwnd;
        if (handle == IntPtr.Zero)
            handle = new WindowInteropHelper(this).Handle;
        IntPtr mon = Native.MonitorFromWindow(handle, MonitorDefaultToNearest);
        Native.MONITORINFO info = new Native.MONITORINFO();
        info.cbSize = Marshal.SizeOf(typeof(Native.MONITORINFO));
        if (mon != IntPtr.Zero && Native.GetMonitorInfo(mon, ref info))
        {
            Native.RECT r = info.rcWork;
            return new Rect(r.Left / _dpi, r.Top / _dpi, (r.Right - r.Left) / _dpi, (r.Bottom - r.Top) / _dpi);
        }
        return SystemParameters.WorkArea;
    }

    void ReassertTopmost()
    {
        if (_reasserting)
            return;
        if (!_s.Topmost)
        {
            Topmost = false;
            return;
        }
        _reasserting = true;
        try
        {
            if (!Topmost)
                Topmost = true;
            if (_hwnd != IntPtr.Zero)
                Native.SetWindowPos(_hwnd, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNoactivate);
        }
        finally
        {
            _reasserting = false;
        }
    }

    void LoadSettings()
    {
        try
        {
            string path = _settingsPath;
            if (!File.Exists(path) && !_vah)
            {
                string legacy = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DesktopClock",
                    "settings.txt");
                if (File.Exists(legacy))
                    path = legacy;
            }
            if (!File.Exists(path))
                return;
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int eq = line.IndexOf('=');
                if (eq <= 0)
                    continue;
                string k = line.Substring(0, eq).Trim();
                string v = line.Substring(eq + 1).Trim();
                if (k == "x") _s.X = double.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "y") _s.Y = double.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "twentyFour") _s.TwentyFour = v != "0";
                else if (k == "seconds") _s.Seconds = v != "0";
                else if (k == "date") _s.Date = v != "0";
                else if (k == "snap") _s.Snap = v != "0";
                else if (k == "topmost") _s.Topmost = v != "0";
                else if (k == "locked") _s.Locked = v != "0";
                else if (k == "chime") _s.Chime = v != "0";
                else if (k == "opacity") _s.OpacityPct = int.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "size") _s.Size = int.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "theme") _s.Theme = int.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "alarmOn") _s.AlarmOn = v != "0";
                else if (k == "alarmH") _s.AlarmH = int.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "alarmM") _s.AlarmM = int.Parse(v, CultureInfo.InvariantCulture);
                else if (k == "label") _s.LabelText = UnescapeLabel(v);
                else if (k == "showLabel") _s.ShowLabel = v != "0";
                else if (k == "lang") Ui.ApplySaved(v);
            }
            if (!double.IsNaN(_s.X) && !double.IsNaN(_s.Y))
            {
                Left = _s.X;
                Top = _s.Y;
                _hasPos = true;
            }
            if (_s.Size < 0 || _s.Size > 3) _s.Size = 1;
            if (_s.Theme < 0 || _s.Theme > 5) _s.Theme = 0;
            if (_s.AlarmH < 0 || _s.AlarmH > 23) _s.AlarmH = 7;
            if (_s.AlarmM < 0 || _s.AlarmM > 59) _s.AlarmM = 0;
            if (_s.OpacityPct != 100 && _s.OpacityPct != 80 && _s.OpacityPct != 60)
                _s.OpacityPct = 100;
            if (_s.LabelText == null || _s.LabelText.Trim().Length == 0)
                _s.LabelText = "ZTime";
            if (!_vah && _s.LabelText == "Clock")
                _s.LabelText = "ZTime";
            if (_s.LabelText.Length > 32)
                _s.LabelText = _s.LabelText.Substring(0, 32);
        }
        catch { }
    }

    void SaveSettings()
    {
        try
        {
            if (!_menuHost)
            {
                _s.X = Left;
                _s.Y = Top;
            }
            string dir = System.IO.Path.GetDirectoryName(_settingsPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(_settingsPath,
                "x=" + _s.X.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "y=" + _s.Y.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "twentyFour=" + B(_s.TwentyFour) + "\r\n" +
                "seconds=" + B(_s.Seconds) + "\r\n" +
                "date=" + B(_s.Date) + "\r\n" +
                "snap=" + B(_s.Snap) + "\r\n" +
                "topmost=" + B(_s.Topmost) + "\r\n" +
                "locked=" + B(_s.Locked) + "\r\n" +
                "chime=" + B(_s.Chime) + "\r\n" +
                "opacity=" + _s.OpacityPct.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "size=" + _s.Size.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "theme=" + _s.Theme.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "alarmOn=" + B(_s.AlarmOn) + "\r\n" +
                "alarmH=" + _s.AlarmH.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "alarmM=" + _s.AlarmM.ToString(CultureInfo.InvariantCulture) + "\r\n" +
                "label=" + EscapeLabel(_s.LabelText) + "\r\n" +
                "showLabel=" + B(_s.ShowLabel) + "\r\n" +
                "lang=" + Ui.Code + "\r\n");
        }
        catch { }
    }

    static string B(bool v)
    {
        return v ? "1" : "0";
    }

    static string EscapeLabel(string s)
    {
        if (s == null)
            return "ZTime";
        return s.Replace("\\", "\\\\").Replace("\r", "").Replace("\n", " ").Replace("=", "\\=");
    }

    static string UnescapeLabel(string s)
    {
        if (s == null)
            return "ZTime";
        return s.Replace("\\=", "=").Replace("\\\\", "\\");
    }

    void CreateStartupShortcut()
    {
        try
        {
            Type t = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(t);
            dynamic lnk = shell.CreateShortcut(_startupLnk);
            lnk.TargetPath = _exePath;
            lnk.WorkingDirectory = System.IO.Path.GetDirectoryName(_exePath);
            lnk.Arguments = _vah ? "--vah" : "";
            lnk.Description = _vah ? "VAH ZTime" : "ZTime";
            lnk.Save();
        }
        catch { }
    }
}

internal sealed class LedPanel : FrameworkElement
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(LedPanel),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ColonOnProperty = DependencyProperty.Register(
        "ColonOn", typeof(bool), typeof(LedPanel),
        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public bool ColonOn
    {
        get { return (bool)GetValue(ColonOnProperty); }
        set { SetValue(ColonOnProperty, value); }
    }

    public Brush OnBrush = Brushes.Lime;
    public Brush OffBrush = Brushes.Transparent;

    static readonly byte[] Maps =
    {
        0x77, 0x24, 0x5D, 0x6D, 0x2E, 0x6B, 0x7B, 0x25, 0x7F, 0x6F
    };

    protected override void OnRender(DrawingContext dc)
    {
        string text = Text ?? "";
        double w = ActualWidth;
        double h = ActualHeight;
        if (w < 8 || h < 8 || text.Length == 0)
            return;

        double units = 0;
        for (int i = 0; i < text.Length; i++)
            units += text[i] == ':' ? 0.42 : 1.12;
        units -= 0.12;

        double scale = Math.Min(w / (units * 10.0), h / 18.0);
        double dw = 10.0 * scale;
        double dh = 18.0 * scale;
        double x = (w - units * 10.0 * scale) / 2.0;
        double y = (h - dh) / 2.0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == ':')
            {
                Brush b = ColonOn ? OnBrush : OffBrush;
                double cw = dw * 0.36;
                double cr = Math.Max(1.1, dh * 0.07);
                dc.DrawEllipse(b, null, new Point(x + cw / 2, y + dh * 0.32), cr, cr);
                dc.DrawEllipse(b, null, new Point(x + cw / 2, y + dh * 0.68), cr, cr);
                x += dw * 0.42;
            }
            else
            {
                DrawDigit(dc, c, x, y, dw, dh);
                x += dw * 1.12;
            }
        }
    }

    void DrawDigit(DrawingContext dc, char c, double x, double y, double w, double h)
    {
        int mask = 0;
        if (c >= '0' && c <= '9')
            mask = Maps[c - '0'];
        else if (c == '-')
            mask = 0x08;

        double t = h * 0.12;
        double gap = t * 0.18;
        double x1 = x + t * 0.55;
        double x2 = x + w - t * 0.55;
        double y1 = y;
        double yMid = y + h / 2.0 - t / 2.0;
        double y2 = y + h - t;
        double xl = x;
        double xr = x + w - t;

        DrawSeg(dc, (mask & 1) != 0, true, x1 + gap, y1, x2 - x1 - 2 * gap, t);
        DrawSeg(dc, (mask & 2) != 0, false, xl, y1 + t * 0.55 + gap, t, yMid - (y1 + t * 0.55) - gap);
        DrawSeg(dc, (mask & 4) != 0, false, xr, y1 + t * 0.55 + gap, t, yMid - (y1 + t * 0.55) - gap);
        DrawSeg(dc, (mask & 8) != 0, true, x1 + gap, yMid, x2 - x1 - 2 * gap, t);
        DrawSeg(dc, (mask & 16) != 0, false, xl, yMid + t + gap, t, y2 - (yMid + t) - gap);
        DrawSeg(dc, (mask & 32) != 0, false, xr, yMid + t + gap, t, y2 - (yMid + t) - gap);
        DrawSeg(dc, (mask & 64) != 0, true, x1 + gap, y2, x2 - x1 - 2 * gap, t);
    }

    void DrawSeg(DrawingContext dc, bool on, bool horiz, double x, double y, double w, double h)
    {
        if (w <= 0.5 || h <= 0.5)
            return;
        Brush b = on ? OnBrush : OffBrush;
        StreamGeometry g = new StreamGeometry();
        using (StreamGeometryContext ctx = g.Open())
        {
            if (horiz)
            {
                double n = Math.Min(h * 0.45, w * 0.2);
                ctx.BeginFigure(new Point(x + n, y), true, true);
                ctx.LineTo(new Point(x + w - n, y), true, false);
                ctx.LineTo(new Point(x + w, y + h / 2), true, false);
                ctx.LineTo(new Point(x + w - n, y + h), true, false);
                ctx.LineTo(new Point(x + n, y + h), true, false);
                ctx.LineTo(new Point(x, y + h / 2), true, false);
            }
            else
            {
                double n = Math.Min(w * 0.45, h * 0.2);
                ctx.BeginFigure(new Point(x, y + n), true, true);
                ctx.LineTo(new Point(x + w / 2, y), true, false);
                ctx.LineTo(new Point(x + w, y + n), true, false);
                ctx.LineTo(new Point(x + w, y + h - n), true, false);
                ctx.LineTo(new Point(x + w / 2, y + h), true, false);
                ctx.LineTo(new Point(x, y + h - n), true, false);
            }
        }
        g.Freeze();
        dc.DrawGeometry(b, null, g);
    }
}

internal sealed class CalendarPopup : Window
{
    public CalendarPopup(DateTime day)
    {
        Title = Ui.S("Calendar", "Calendrier");
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        FontFamily = Ui.UiFont;
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        ShowActivated = true;

        System.Windows.Controls.Calendar cal = new System.Windows.Controls.Calendar();
        cal.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        cal.SelectedDate = day;
        cal.DisplayDate = day;
        cal.Margin = new Thickness(8);

        Border chrome = new Border();
        chrome.CornerRadius = new CornerRadius(8);
        chrome.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x22));
        chrome.BorderBrush = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x40));
        chrome.BorderThickness = new Thickness(1);
        chrome.Padding = new Thickness(4);
        chrome.Child = cal;
        Content = chrome;

        Deactivated += delegate { Close(); };
        KeyDown += delegate(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        };
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        IntPtr hwnd = new WindowInteropHelper(this).Handle;
        IntPtr ex = Native.GetWindowLongPtr(hwnd, -20);
        Native.SetWindowLongPtr(hwnd, -20, new IntPtr(ex.ToInt64() | 0x80));
    }
}

internal sealed class UninstallDialog : Window
{
    readonly CheckBox _wipe;
    public bool WipeSettings;

    public UninstallDialog()
    {
        Title = Ui.S("Uninstall", "Désinstaller");
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        FontFamily = Ui.UiFont;
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        TextBlock hint = new TextBlock();
        hint.Text = Ui.S("Uninstall ZTime from this computer?", "Désinstaller ZTime de cet ordinateur ?");
        hint.Foreground = Brushes.White;
        hint.FontSize = 13;
        hint.TextWrapping = TextWrapping.Wrap;
        hint.Margin = new Thickness(0, 0, 0, 12);

        _wipe = new CheckBox();
        _wipe.Content = Ui.S("Also delete saved settings", "Supprimer aussi les réglages enregistrés");
        _wipe.Foreground = Brushes.White;
        _wipe.Margin = new Thickness(0, 0, 0, 16);
        _wipe.FontFamily = Ui.UiFont;
        _wipe.FontSize = 13;

        Button ok = DarkButton(Ui.S("Uninstall", "Désinstaller"));
        Button cancel = DarkButton(Ui.S("Cancel", "Annuler"));
        ok.Click += delegate
        {
            WipeSettings = _wipe.IsChecked == true;
            DialogResult = true;
            Close();
        };
        cancel.Click += delegate
        {
            DialogResult = false;
            Close();
        };

        StackPanel buttons = new StackPanel();
        buttons.Orientation = Orientation.Horizontal;
        buttons.HorizontalAlignment = HorizontalAlignment.Right;
        buttons.Children.Add(cancel);
        buttons.Children.Add(ok);

        StackPanel body = new StackPanel();
        body.Margin = new Thickness(18);
        body.Children.Add(hint);
        body.Children.Add(_wipe);
        body.Children.Add(buttons);

        Border chrome = new Border();
        chrome.CornerRadius = new CornerRadius(10);
        chrome.Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E));
        chrome.BorderBrush = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x40));
        chrome.BorderThickness = new Thickness(1);
        chrome.Width = 320;
        chrome.Child = body;
        Content = chrome;

        KeyDown += delegate(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        };
    }

    static Button DarkButton(string text)
    {
        Button b = new Button();
        b.Content = text;
        b.Width = 110;
        b.Height = 28;
        b.Margin = new Thickness(8, 0, 0, 0);
        b.FontFamily = new FontFamily("Segoe UI");
        return b;
    }
}

internal sealed class RenameDialog : Window
{
    readonly TextBox _box;
    public string LabelText;

    public RenameDialog(string current)
    {
        Title = Ui.S("Name", "Nom");
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        FontFamily = Ui.UiFont;
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        TextBlock hint = new TextBlock();
        hint.Text = Ui.S("Name under the icon", "Nom sous l'icone");
        hint.Foreground = Brushes.White;
        hint.FontFamily = Ui.UiFont;
        hint.FontSize = 13;
        hint.Margin = new Thickness(0, 0, 0, 10);

        _box = new TextBox();
        _box.Text = current == null ? "ZTime" : current;
        _box.FontFamily = new FontFamily("Segoe UI");
        _box.FontSize = 14;
        _box.MaxLength = 32;
        _box.Padding = new Thickness(6, 4, 6, 4);
        _box.Margin = new Thickness(0, 0, 0, 16);

        Button ok = DarkButton("OK");
        Button cancel = DarkButton(Ui.S("Cancel", "Annuler"));
        ok.Click += delegate { Accept(); };
        cancel.Click += delegate { DialogResult = false; Close(); };

        StackPanel buttons = new StackPanel();
        buttons.Orientation = Orientation.Horizontal;
        buttons.HorizontalAlignment = HorizontalAlignment.Right;
        buttons.Children.Add(cancel);
        buttons.Children.Add(ok);

        StackPanel body = new StackPanel();
        body.Margin = new Thickness(18);
        body.Children.Add(hint);
        body.Children.Add(_box);
        body.Children.Add(buttons);

        Border chrome = new Border();
        chrome.CornerRadius = new CornerRadius(10);
        chrome.Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E));
        chrome.BorderBrush = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x40));
        chrome.BorderThickness = new Thickness(1);
        chrome.Width = 280;
        chrome.Child = body;
        Content = chrome;

        Loaded += delegate { _box.SelectAll(); _box.Focus(); };
        KeyDown += delegate(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
            if (e.Key == Key.Enter)
                Accept();
        };
    }

    void Accept()
    {
        string t = _box.Text == null ? "" : _box.Text.Trim();
        if (t.Length == 0)
            t = "ZTime";
        if (t.Length > 32)
            t = t.Substring(0, 32);
        LabelText = t;
        DialogResult = true;
        Close();
    }

    static Button DarkButton(string text)
    {
        Button b = new Button();
        b.Content = text;
        b.Width = 88;
        b.Height = 28;
        b.Margin = new Thickness(8, 0, 0, 0);
        b.FontFamily = new FontFamily("Segoe UI");
        return b;
    }
}

internal sealed class AlarmDialog : Window
{
    readonly CheckBox _on;
    readonly ComboBox _hours;
    readonly ComboBox _minutes;

    public bool AlarmOn;
    public int Hour;
    public int Minute;

    public AlarmDialog(bool on, int hour, int minute)
    {
        Title = Ui.S("Alarm", "Alarme");
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        FontFamily = Ui.UiFont;
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        _on = new CheckBox();
        _on.Content = Ui.S("Enable alarm", "Activer l'alarme");
        _on.IsChecked = on;
        _on.Foreground = Brushes.White;
        _on.Margin = new Thickness(0, 0, 0, 12);
        _on.FontFamily = new FontFamily("Segoe UI");
        _on.FontSize = 13;

        _hours = Combo(24, hour);
        _minutes = Combo(60, minute);

        TextBlock colon = new TextBlock();
        colon.Text = ":";
        colon.Foreground = Brushes.White;
        colon.FontSize = 18;
        colon.FontWeight = FontWeights.Bold;
        colon.Margin = new Thickness(8, 0, 8, 0);
        colon.VerticalAlignment = VerticalAlignment.Center;

        StackPanel timeRow = new StackPanel();
        timeRow.Orientation = Orientation.Horizontal;
        timeRow.HorizontalAlignment = HorizontalAlignment.Center;
        timeRow.Children.Add(_hours);
        timeRow.Children.Add(colon);
        timeRow.Children.Add(_minutes);

        TextBlock hint = new TextBlock();
        hint.Text = Ui.S("Time (24 h) — every day", "Heure (24 h)  —  tous les jours");
        hint.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
        hint.FontSize = 11;
        hint.Margin = new Thickness(0, 8, 0, 14);
        hint.HorizontalAlignment = HorizontalAlignment.Center;

        Button ok = DarkButton("OK");
        Button cancel = DarkButton(Ui.S("Cancel", "Annuler"));
        ok.Click += delegate
        {
            AlarmOn = _on.IsChecked == true;
            Hour = _hours.SelectedIndex;
            Minute = _minutes.SelectedIndex;
            DialogResult = true;
            Close();
        };
        cancel.Click += delegate
        {
            DialogResult = false;
            Close();
        };

        StackPanel buttons = new StackPanel();
        buttons.Orientation = Orientation.Horizontal;
        buttons.HorizontalAlignment = HorizontalAlignment.Right;
        buttons.Children.Add(cancel);
        buttons.Children.Add(ok);

        StackPanel body = new StackPanel();
        body.Margin = new Thickness(18);
        body.Children.Add(_on);
        body.Children.Add(timeRow);
        body.Children.Add(hint);
        body.Children.Add(buttons);

        Border chrome = new Border();
        chrome.CornerRadius = new CornerRadius(10);
        chrome.Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E));
        chrome.BorderBrush = new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x40));
        chrome.BorderThickness = new Thickness(1);
        chrome.Width = 280;
        chrome.Child = body;
        Content = chrome;

        KeyDown += delegate(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
            if (e.Key == Key.Enter)
            {
                AlarmOn = _on.IsChecked == true;
                Hour = _hours.SelectedIndex;
                Minute = _minutes.SelectedIndex;
                DialogResult = true;
                Close();
            }
        };
    }

    static ComboBox Combo(int count, int selected)
    {
        ComboBox cb = new ComboBox();
        cb.Width = 64;
        cb.FontFamily = new FontFamily("Consolas");
        cb.FontSize = 16;
        for (int i = 0; i < count; i++)
            cb.Items.Add(i.ToString("00", CultureInfo.InvariantCulture));
        cb.SelectedIndex = selected;
        return cb;
    }

    static Button DarkButton(string text)
    {
        Button b = new Button();
        b.Content = text;
        b.Width = 88;
        b.Height = 28;
        b.Margin = new Thickness(8, 0, 0, 0);
        b.FontFamily = new FontFamily("Segoe UI");
        return b;
    }
}

internal sealed class AlarmAlert : Window
{
    public bool Snooze;

    public AlarmAlert(int hour, int minute)
    {
        Title = Ui.S("Alarm", "Alarme");
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(Ui.Ietf);
        FontFamily = Ui.UiFont;
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ShowActivated = true;

        TextBlock title = new TextBlock();
        title.Text = Ui.S("Alarm", "Alarme");
        title.Foreground = Brushes.White;
        title.FontSize = 13;
        title.HorizontalAlignment = HorizontalAlignment.Center;

        TextBlock time = new TextBlock();
        time.Text = hour.ToString("00", CultureInfo.InvariantCulture) + ":" + minute.ToString("00", CultureInfo.InvariantCulture);
        time.Foreground = new SolidColorBrush(Color.FromRgb(0x3D, 0xFF, 0x8A));
        time.FontFamily = new FontFamily("Consolas");
        time.FontSize = 36;
        time.FontWeight = FontWeights.Bold;
        time.HorizontalAlignment = HorizontalAlignment.Center;
        time.Margin = new Thickness(0, 6, 0, 14);

        Button snooze = new Button();
        snooze.Content = Ui.S("Snooze 5 min", "Reporter 5 min");
        snooze.Width = 120;
        snooze.Height = 30;
        snooze.Click += delegate { Snooze = true; Close(); };

        Button stop = new Button();
        stop.Content = Ui.S("Stop", "Arreter");
        stop.Width = 88;
        stop.Height = 30;
        stop.Margin = new Thickness(8, 0, 0, 0);
        stop.Click += delegate { Snooze = false; Close(); };

        StackPanel buttons = new StackPanel();
        buttons.Orientation = Orientation.Horizontal;
        buttons.HorizontalAlignment = HorizontalAlignment.Center;
        buttons.Children.Add(snooze);
        buttons.Children.Add(stop);

        StackPanel body = new StackPanel();
        body.Margin = new Thickness(20);
        body.Children.Add(title);
        body.Children.Add(time);
        body.Children.Add(buttons);

        Border chrome = new Border();
        chrome.CornerRadius = new CornerRadius(12);
        chrome.Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x1C, 0x1E));
        chrome.BorderBrush = new SolidColorBrush(Color.FromRgb(0x3D, 0xFF, 0x8A));
        chrome.BorderThickness = new Thickness(1);
        chrome.Width = 260;
        chrome.Child = body;
        Content = chrome;

        KeyDown += delegate(object s, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        };
    }
}

internal static class Native
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }
}
