using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KikitanTranslator.Overlay;

public partial class Form1 : Form
{
    private Image img;
    private int time;
    private Bitmap _backBuffer;

    public Form1(Rectangle bounds)
    {
        InitializeComponent();
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.Bounds = bounds;
        
        this.DoubleBuffered = true;
        this.SetStyle(
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint,
            true);
        this.UpdateStyles();

        SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

        int initialStyle = (int)GetWindowLong(this.Handle, GWL_EXSTYLE);
        SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
    }
    
    protected override void OnPaintBackground(PaintEventArgs e) { }

    // Painting goes through UpdateLayeredWindow instead of the normal paint cycle.
    protected override void OnPaint(PaintEventArgs e) { }
    
    private void RebuildBackBuffer()
    {
        var newBuffer = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);

        using (var g = Graphics.FromImage(newBuffer))
        {
            g.Clear(Color.Transparent);

            if (img != null)
            {
                int w = this.Width / 5;
                int h = img.Height / (img.Width / (this.Width / 5));
                int x = (this.Width - w) / 2;
                int y = this.Height - h - 50;

                g.CompositingMode = CompositingMode.SourceCopy;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, x, y, w, h);
            }
        }

        var old = Interlocked.Exchange(ref _backBuffer, newBuffer);
        old?.Dispose();

        PushToLayeredWindow(newBuffer);
    }

    /// <summary>
    /// Hands the whole frame to the window with its alpha channel intact. A colour keyed
    /// window can only cut pixels fully in or out, which is why captions used to sit on an
    /// opaque slab with hard edges.
    /// </summary>
    private void PushToLayeredWindow(Bitmap bitmap)
    {
        var screenDc = GetDC(IntPtr.Zero);
        var memDc = CreateCompatibleDC(screenDc);
        var hBitmap = IntPtr.Zero;
        var oldBitmap = IntPtr.Zero;

        try
        {
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            oldBitmap = SelectObject(memDc, hBitmap);

            var size = new SIZE { cx = bitmap.Width, cy = bitmap.Height };
            var source = new POINT { x = 0, y = 0 };
            var destination = new POINT { x = this.Left, y = this.Top };
            var blend = new BLENDFUNCTION
            {
                BlendOp = AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = 255,
                AlphaFormat = AC_SRC_ALPHA
            };

            UpdateLayeredWindow(this.Handle, screenDc, ref destination, ref size, memDc, ref source,
                0, ref blend, ULW_ALPHA);
        }
        finally
        {
            if (hBitmap != IntPtr.Zero)
            {
                SelectObject(memDc, oldBitmap);
                DeleteObject(hBitmap);
            }

            DeleteDC(memDc);
            ReleaseDC(IntPtr.Zero, screenDc);
        }
    }

    public void SetImage(Image newImg, int timeLeft)
    {
        var old = Interlocked.Exchange(ref img, newImg);
        old?.Dispose();

        this.time = timeLeft;
        
        Invoke(RebuildBackBuffer);

        Task.Run(() =>
        {
            Image captured = this.img;
            Console.WriteLine($"Waiting {timeLeft}ms...");
            Thread.Sleep(timeLeft);
            if (this.img != captured) return;

            var cleared = Interlocked.Exchange(ref img, null);
            cleared?.Dispose();

            Invoke(RebuildBackBuffer);
            Console.WriteLine("Image cleared.");
        });
    }

    private const byte AC_SRC_OVER = 0;
    private const byte AC_SRC_ALPHA = 1;
    private const int ULW_ALPHA = 2;

    [StructLayout(LayoutKind.Sequential)] private struct SIZE { public int cx; public int cy; }
    [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x; public int y; }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize,
        IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x80000;
    private const int WS_EX_TRANSPARENT = 0x20;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const UInt32 SWP_NOSIZE = 0x0001;
    private const UInt32 SWP_NOMOVE = 0x0002;
    private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}