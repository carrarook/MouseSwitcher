using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new InputWatcherForm());
    }
}

public class InputWatcherForm : Form
{
    private HashSet<IntPtr> seenDevices = new();
    private IntPtr mouseHandle = IntPtr.Zero; // define o dispositivo do mouse, por padrão é nulo
    private IntPtr touchpadHandle = IntPtr.Zero; // define o dispositivo do note, por padrão é nulo
    private IntPtr lastDevice = IntPtr.Zero; // armazena o ultimo dispositivo usado

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    public InputWatcherForm()
    {
        this.StartPosition = FormStartPosition.Manual;
        this.Location = new Point(-32000, -32000);
        this.Size = new Size(1, 1);
        this.ShowInTaskbar = false;
        this.FormBorderStyle = FormBorderStyle.None;
        this.Load += (_, _) => RegisterRawInput();
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_INPUT = 0x00FF;

        if (m.Msg == WM_INPUT)
        {
            uint dwSize = 0;
            GetRawInputData(m.LParam, 0x10000003, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {
                if (GetRawInputData(m.LParam, 0x10000003, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                {
                    var raw = Marshal.PtrToStructure<RAWINPUT>(buffer);
                    if (raw.header.dwType == 0) // Verifica se o input vem do mouse
                    {
                        IntPtr device = raw.header.hDevice;
                        if (!seenDevices.Contains(device))
                        {
                            seenDevices.Add(device);
                            if (mouseHandle == IntPtr.Zero)
                            {
                                mouseHandle = device;
                            }
                            else if (touchpadHandle == IntPtr.Zero)
                            {
                                touchpadHandle = device;
                            }
                        }

                        // Verificar se o dispositivo é o mesmo do último
                        if (device != lastDevice)
                        {
                            if (device == mouseHandle)
                            {
                                Console.WriteLine("Movimento do MOUSE externo detectado.");
                                CentralizarNaTela(1); 
                            }
                            else if (device == touchpadHandle)
                            {
                                Console.WriteLine("Movimento do TOUCHPAD detectado.");
                                CentralizarNaTela(0); 
                            }
                            lastDevice = device; // Atualiza o último dispositivo
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        base.WndProc(ref m);
    }

    private void CentralizarNaTela(int screenIndex)
    {
        if (Screen.AllScreens.Length > screenIndex)
        {
            var bounds = Screen.AllScreens[screenIndex].Bounds;

            // Verifica se o cursor está na tela correta
            Point cursorPos = Cursor.Position;
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(cursorPos))
                {
                    int currentScreenIndex = Array.IndexOf(Screen.AllScreens, screen);
                    if (currentScreenIndex == screenIndex)
                    {
                        // Cursor ta na tela certa
                        return;
                    }
                    break;
                }
            }

            // Centralizar o cursor na tela
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            SetCursorPos(centerX, centerY);
        }
        else
        {
            Console.WriteLine($"Tela {screenIndex + 1} não encontrada.");
        }
    }

    private void RegisterRawInput()
    {
        RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
        rid[0].usUsagePage = 0x01;
        rid[0].usUsage = 0x02;     
        rid[0].dwFlags = 0;
        rid[0].hwndTarget = this.Handle;

        if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            MessageBox.Show("Erro ao registrar Raw Input.");
    }

    #region Interop Structs

    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTDEVICE
    {
        public ushort usUsagePage;
        public ushort usUsage;
        public uint dwFlags;
        public IntPtr hwndTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTHEADER
    {
        public uint dwType;
        public uint dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWMOUSE
    {
        public ushort usFlags;
        public uint ulButtons;
        public ushort usButtonFlags;
        public ushort usButtonData;
        public uint ulRawButtons;
        public int lLastX;
        public int lLastY;
        public uint ulExtraInformation;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct RAWINPUT
    {
        [FieldOffset(0)] public RAWINPUTHEADER header;
        [FieldOffset(16)] public RAWMOUSE mouse;
    }

    [DllImport("User32.dll")]
    static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

    [DllImport("User32.dll")]
    static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    #endregion
}
