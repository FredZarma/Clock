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
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

[assembly: AssemblyTitle("Clock")]
[assembly: AssemblyDescription("Horloge numerique de bureau")]
[assembly: AssemblyProduct("Clock")]
[assembly: AssemblyCompany("Fred Zarma")]
[assembly: AssemblyCopyright("Copyright Fred Zarma 2026")]
[assembly: AssemblyVersion("2.1.0.0")]
[assembly: AssemblyFileVersion("2.1.0.0")]

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        bool created;
        Mutex mutex = new Mutex(true, @"Local\DesktopClock.IconWidget", out created);
        if (!created)
            return;

        Application app = new Application();
        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        app.Run(new ClockWindow());
        GC.KeepAlive(mutex);
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
    public string LabelText = "Clock";
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
    static readonly CultureInfo Fr = new CultureInfo("fr-FR");
    static readonly double[] FaceW = { 76, 168, 224, 292 };
    static readonly double[] FaceH = { 54, 86, 116, 156 };
    static readonly string[] SizeNames = { "Petite", "Normale", "Grande", "Extra" };
    static readonly string[] ThemeNames = { "Vert", "Ambre", "Rouge", "Bleu", "Cyan", "Blanc" };
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

    public ClockWindow()
    {
        _exePath = Assembly.GetExecutingAssembly().Location;
        _settingsPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DesktopClock",
            "settings.txt");
        _startupLnk = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            "Clock.lnk");

        LoadSettings();

        Title = "Clock";
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

        _date = new TextBlock();
        _date.FontFamily = new FontFamily("Segoe UI");
        _date.FontSize = 11;
        _date.HorizontalAlignment = HorizontalAlignment.Center;
        _date.TextAlignment = TextAlignment.Center;
        _date.Margin = new Thickness(4, 0, 4, 5);
        TextOptions.SetTextFormattingMode(_date, TextFormattingMode.Display);

        _ampm = new TextBlock();
        _ampm.FontFamily = new FontFamily("Segoe UI Semibold, Segoe UI");
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
            TextBlock halo = IconLabel("Clock", Brushes.Black);
            halo.Margin = new Thickness(ox[i], oy[i], -ox[i], -oy[i]);
            _labelHost.Children.Add(halo);
            _labelParts.Add(halo);
        }
        TextBlock label = IconLabel("Clock", Brushes.White);
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
    }

    ContextMenu BuildMenu()
    {
        ContextMenu menu = new ContextMenu();
        menu.Items.Add(Item("Ouvrir le calendrier", false, false, delegate { OpenCalendar(); }));
        menu.Items.Add(Item("Copier l'heure", false, false, delegate { CopyTime(); }));
        menu.Items.Add(Item("Alarme...", false, false, delegate { OpenAlarm(); }));
        menu.Items.Add(new Separator());

        MenuItem sizes = new MenuItem();
        sizes.Header = "Taille";
        for (int i = 0; i < SizeNames.Length; i++)
        {
            int idx = i;
            sizes.Items.Add(Item(SizeNames[i], true, _s.Size == i, delegate { _s.Size = idx; ApplySize(); SaveSettings(); }));
        }
        menu.Items.Add(sizes);

        MenuItem themes = new MenuItem();
        themes.Header = "Couleur";
        for (int i = 0; i < ThemeNames.Length; i++)
        {
            int idx = i;
            themes.Items.Add(Item(ThemeNames[i], true, _s.Theme == i, delegate { _s.Theme = idx; ApplyTheme(); TickClock(); SaveSettings(); }));
        }
        menu.Items.Add(themes);

        MenuItem display = new MenuItem();
        display.Header = "Affichage";
        display.Items.Add(Item("Secondes", true, _s.Seconds, delegate { _s.Seconds = !_s.Seconds; TickClock(); SaveSettings(); }));
        display.Items.Add(Item("Date", true, _s.Date, delegate { _s.Date = !_s.Date; TickClock(); SaveSettings(); }));
        display.Items.Add(Item("Format 24 heures", true, _s.TwentyFour, delegate { _s.TwentyFour = !_s.TwentyFour; TickClock(); SaveSettings(); }));
        display.Items.Add(Item("Carillon des heures", true, _s.Chime, delegate { _s.Chime = !_s.Chime; SaveSettings(); }));
        MenuItem opac = new MenuItem();
        opac.Header = "Opacite";
        int[] pcts = { 100, 80, 60 };
        for (int i = 0; i < pcts.Length; i++)
        {
            int p = pcts[i];
            opac.Items.Add(Item(p + " %", true, _s.OpacityPct == p, delegate { _s.OpacityPct = p; ApplyOpacity(); SaveSettings(); }));
        }
        display.Items.Add(opac);
        menu.Items.Add(display);

        MenuItem nom = new MenuItem();
        nom.Header = "Nom";
        nom.Items.Add(Item("Afficher le nom", true, _s.ShowLabel, delegate
        {
            _s.ShowLabel = !_s.ShowLabel;
            ApplyLabel();
            SaveSettings();
        }));
        nom.Items.Add(Item("Modifier le nom...", false, false, delegate { RenameLabel(); }));
        menu.Items.Add(nom);

        menu.Items.Add(new Separator());
        menu.Items.Add(Item("Aligner sur la grille", true, _s.Snap, delegate
        {
            _s.Snap = !_s.Snap;
            if (_s.Snap) SnapToGrid();
            SaveSettings();
        }));
        menu.Items.Add(Item("Verrouiller la position", true, _s.Locked, delegate { _s.Locked = !_s.Locked; SaveSettings(); }));
        menu.Items.Add(Item("Toujours visible", true, _s.Topmost, delegate
        {
            _s.Topmost = !_s.Topmost;
            Topmost = _s.Topmost;
            ReassertTopmost();
            SaveSettings();
        }));
        menu.Items.Add(Item("Lancer au demarrage", true, File.Exists(_startupLnk), delegate
        {
            if (File.Exists(_startupLnk))
            {
                try { File.Delete(_startupLnk); }
                catch { }
            }
            else
                CreateStartupShortcut();
        }));
        menu.Items.Add(Item("Date et heure Windows", false, false, delegate
        {
            try { Process.Start("ms-settings:dateandtime"); }
            catch { }
        }));
        menu.Items.Add(new Separator());
        menu.Items.Add(Item("© Fred Zarma 2026  (@FredZarma)", false, false, delegate
        {
            try { Process.Start("https://x.com/FredZarma"); }
            catch { }
        }));
        menu.Items.Add(Item("Fermer", false, false, delegate { Close(); }));
        return menu;
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

    void OnSourceInit(object sender, EventArgs e)
    {
        HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
        _hwnd = source.Handle;
        _dpi = source.CompositionTarget.TransformToDevice.M11;
        if (_dpi <= 0)
            _dpi = 1.0;

        IntPtr ex = Native.GetWindowLongPtr(_hwnd, GwlExstyle);
        Native.SetWindowLongPtr(_hwnd, GwlExstyle, new IntPtr(ex.ToInt64() | WsExToolwindow));

        ApplySize();

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

    void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        StopAlarmUi();
        if (_cal != null)
        {
            try { _cal.Close(); }
            catch { }
        }
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
            t = "Clock";
        for (int i = 0; i < _labelParts.Count; i++)
            _labelParts[i].Text = t;
        _labelHost.Visibility = _s.ShowLabel ? Visibility.Visible : Visibility.Collapsed;
    }

    void RenameLabel()
    {
        RenameDialog dlg = new RenameDialog(_s.LabelText);
        dlg.Owner = this;
        bool? ok = dlg.ShowDialog();
        if (ok == true)
        {
            _s.LabelText = dlg.LabelText;
            ApplyLabel();
            SaveSettings();
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
            _ampm.Text = pm ? "PM" : "AM";
            _ampm.Visibility = Visibility.Visible;
        }

        if (_s.Date)
        {
            _date.Text = now.ToString(_s.Size == 0 ? "ddd d" : "ddd d MMM", Fr);
            _date.Visibility = Visibility.Visible;
        }
        else
        {
            _date.Text = "";
            _date.Visibility = Visibility.Collapsed;
        }

        _bell.Visibility = _s.AlarmOn ? Visibility.Visible : Visibility.Collapsed;
        _statusRow.Visibility = (_s.AlarmOn || !_s.TwentyFour) ? Visibility.Visible : Visibility.Collapsed;

        ToolTip = now.ToString("dddd d MMMM yyyy", Fr) + Environment.NewLine + now.ToString(_s.TwentyFour ? "HH:mm:ss" : "h:mm:ss tt", Fr);

        if (_s.Chime && now.Minute == 0 && now.Second == 0 && _lastChimeHour != now.Hour)
        {
            _lastChimeHour = now.Hour;
            SystemSounds.Asterisk.Play();
        }
        if (now.Second != 0)
            _lastChimeHour = -1;

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
        _cal.Closed += delegate { _cal = null; };
        UpdateLayout();
        Point p = _face.PointToScreen(new Point(0, _face.ActualHeight + 6));
        _cal.Left = p.X / _dpi;
        _cal.Top = p.Y / _dpi;
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
        AlarmDialog dlg = new AlarmDialog(_s.AlarmOn, _s.AlarmH, _s.AlarmM);
        dlg.Owner = this;
        bool? ok = dlg.ShowDialog();
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

    void CopyTime()
    {
        try
        {
            DateTime now = DateTime.Now;
            Clipboard.SetText(now.ToString("dddd d MMMM yyyy HH:mm:ss", Fr));
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
            if (!File.Exists(_settingsPath))
                return;
            string[] lines = File.ReadAllLines(_settingsPath);
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
                _s.LabelText = "Clock";
            if (_s.LabelText.Length > 32)
                _s.LabelText = _s.LabelText.Substring(0, 32);
        }
        catch { }
    }

    void SaveSettings()
    {
        try
        {
            _s.X = Left;
            _s.Y = Top;
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
                "showLabel=" + B(_s.ShowLabel) + "\r\n");
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
            return "Clock";
        return s.Replace("\\", "\\\\").Replace("\r", "").Replace("\n", " ").Replace("=", "\\=");
    }

    static string UnescapeLabel(string s)
    {
        if (s == null)
            return "Clock";
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
            lnk.Description = "Clock";
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
        Title = "Calendrier";
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        ShowActivated = true;

        System.Windows.Controls.Calendar cal = new System.Windows.Controls.Calendar();
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

internal sealed class RenameDialog : Window
{
    readonly TextBox _box;
    public string LabelText;

    public RenameDialog(string current)
    {
        Title = "Nom";
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        TextBlock hint = new TextBlock();
        hint.Text = "Nom sous l'icone";
        hint.Foreground = Brushes.White;
        hint.FontFamily = new FontFamily("Segoe UI");
        hint.FontSize = 13;
        hint.Margin = new Thickness(0, 0, 0, 10);

        _box = new TextBox();
        _box.Text = current == null ? "Clock" : current;
        _box.FontFamily = new FontFamily("Segoe UI");
        _box.FontSize = 14;
        _box.MaxLength = 32;
        _box.Padding = new Thickness(6, 4, 6, 4);
        _box.Margin = new Thickness(0, 0, 0, 16);

        Button ok = DarkButton("OK");
        Button cancel = DarkButton("Annuler");
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
            t = "Clock";
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
        Title = "Alarme";
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        _on = new CheckBox();
        _on.Content = "Activer l'alarme";
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
        hint.Text = "Heure (24 h)  —  tous les jours";
        hint.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
        hint.FontSize = 11;
        hint.Margin = new Thickness(0, 8, 0, 14);
        hint.HorizontalAlignment = HorizontalAlignment.Center;

        Button ok = DarkButton("OK");
        Button cancel = DarkButton("Annuler");
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
        Title = "Alarme";
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
        title.Text = "Alarme";
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
        snooze.Content = "Reporter 5 min";
        snooze.Width = 120;
        snooze.Height = 30;
        snooze.Click += delegate { Snooze = true; Close(); };

        Button stop = new Button();
        stop.Content = "Arreter";
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
