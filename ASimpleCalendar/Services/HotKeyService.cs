using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ASimpleCalendar.Services;

public class HotKeyService : IDisposable
{
    private const int WM_HOTKEY = 0x0312;

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;

    private const int HOTKEY_SHOW = 1;
    private const int HOTKEY_NOTE = 2;
    private const int HOTKEY_REMINDER = 3;

    private HwndSource? _source;
    private Action? _onShow;
    private Action? _onNewNote;
    private Action? _onNewReminder;

    public void Register(Window window, Action onShow, Action onNewNote, Action onNewReminder)
    {
        _onShow = onShow;
        _onNewNote = onNewNote;
        _onNewReminder = onNewReminder;

        var handle = new WindowInteropHelper(window).Handle;

        RegisterHotKey(handle, HOTKEY_SHOW, MOD_CONTROL | MOD_ALT, 0x43);     // Ctrl+Alt+C
        RegisterHotKey(handle, HOTKEY_NOTE, MOD_CONTROL | MOD_ALT, 0x4E);     // Ctrl+Alt+N
        RegisterHotKey(handle, HOTKEY_REMINDER, MOD_CONTROL | MOD_ALT, 0x54); // Ctrl+Alt+T

        _source = HwndSource.FromHwnd(handle);
        _source?.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_HOTKEY)
        {
            return IntPtr.Zero;
        }

        switch (wParam.ToInt32())
        {
            case HOTKEY_SHOW:
                _onShow?.Invoke();
                handled = true;
                break;
            case HOTKEY_NOTE:
                _onNewNote?.Invoke();
                handled = true;
                break;
            case HOTKEY_REMINDER:
                _onNewReminder?.Invoke();
                handled = true;
                break;
        }

        return IntPtr.Zero;
    }

    public void Dispose()
    {
        if (_source is null)
        {
            return;
        }

        _source.RemoveHook(WndProc);
        var handle = _source.Handle;
        UnregisterHotKey(handle, HOTKEY_SHOW);
        UnregisterHotKey(handle, HOTKEY_NOTE);
        UnregisterHotKey(handle, HOTKEY_REMINDER);
        _source = null;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
