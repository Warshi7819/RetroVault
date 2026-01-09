using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RetroVault
{
    public partial class Splash : Form
    {
        // Background fields (new)
        private Bitmap? bgOriginal;
        private Bitmap? bgScaled;
        // Null => original size, otherwise use specified size (preserve aspect when one dim is 0)
        private Size? bgSize = null;

        // P/Invoke to start a window drag from a client-area mouse down
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x02;

        public Splash()
        {
            InitializeComponent();

            // Center Form on screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            // Reduce flicker by enabling double buffering and controlling painting.
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            // Hide until we've applied shape/background
            Opacity = 0;

            // Apply shape during Load (occurs before first show) instead of Shown to avoid the rectangular flash.
            this.Load += RetroVault_Load;

            // Allow dragging by holding left mouse button anywhere on the form client area
            // (controls will still receive their own mouse events and won't trigger this).
            this.MouseDown += RetroVault_MouseDown;
        }

        /// <summary>
        /// Load the background from a relative path (e.g. "skins/christmas.png").
        /// Default draws original size unless SetBackgroundSize is used before drawing.
        /// </summary>
        public void LoadBackground(string relativePath)
        {
            bgOriginal?.Dispose();
            bgOriginal = null;
            bgScaled?.Dispose();
            bgScaled = null;
            bgSize = null;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDir, relativePath);

            if (!File.Exists(path))
                path = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            if (!File.Exists(path))
                return;

            using var src = new Bitmap(path);
            bgOriginal = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bgOriginal))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.DrawImage(src, 0, 0, src.Width, src.Height);
            }
        }

        /// <summary>
        /// Set the desired background size. Call before the form is shown to avoid visual resizing artifacts.
        /// Pass (0,0) or null to revert to original size.
        /// If one dimension is zero the other will be calculated to preserve aspect ratio.
        /// </summary>
        public void SetBackgroundSize(int? width, int? height)
        {
            if (bgOriginal == null)
                return;

            if ((width == null && height == null) || (width == 0 && height == 0))
            {
                bgSize = null;
                bgScaled?.Dispose();
                bgScaled = null;
                return;
            }

            int origW = bgOriginal.Width;
            int origH = bgOriginal.Height;

            int targetW = width ?? 0;
            int targetH = height ?? 0;

            if (targetW <= 0 && targetH > 0)
            {
                targetW = Math.Max(1, (int)Math.Round(origW * (targetH / (double)origH)));
            }
            else if (targetH <= 0 && targetW > 0)
            {
                targetH = Math.Max(1, (int)Math.Round(origH * (targetW / (double)origW)));
            }
            else if (targetW <= 0 && targetH <= 0)
            {
                bgSize = null;
                bgScaled?.Dispose();
                bgScaled = null;
                return;
            }

            var scaled = new Bitmap(targetW, targetH, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(scaled))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(bgOriginal, new Rectangle(0, 0, targetW, targetH), new Rectangle(0, 0, origW, origH), GraphicsUnit.Pixel);
            }

            bgScaled?.Dispose();
            bgScaled = scaled;
            bgSize = new Size(targetW, targetH);
        }

        // <summary>
        // Allow dragging the window by holding left mouse button anywhere on the form client area.
        // (controls will still receive their own mouse events and won't trigger this).
        // </summary>
        private void RetroVault_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            }
        }

        // <summary>
        // Load and apply skin during form load
        // </summary>
        private void RetroVault_Load(object? sender, EventArgs e)
        {
            try
            {

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string backgroundImage = Path.Combine(baseDir, "images", "DarkSplash.png");



                // Suspend layout and apply shape before first paint
                SuspendLayout();

                // Load background into managed fields so we can scale before draw/region creation
                LoadBackground(backgroundImage);

                // you can call SetBackgroundSize(...) here before ApplyImageShape if you want to pre-scale:
                // e.g. SetBackgroundSize(800, 0); // preserve aspect by width

                ApplyImageShapeFromLoadedBackground();

                ResumeLayout();

                // Show the form now that shape/background is applied
                Opacity = 1;
            }
            catch
            {
                // swallow — don't block startup if shaping fails
                Opacity = 1;
            }
        }

        // <summary>
        // Custom background paint to draw pre-scaled images
        // Prevent the default background erase to avoid a white flash AND white boarder around the image
        // </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Bitmap? toDraw = bgScaled ?? bgOriginal;

            if (toDraw == null)
            {
                base.OnPaintBackground(e);
                return;
            }

            // Draw the pre-scaled background image directly (avoids the default background clear)
            e.Graphics.DrawImage(toDraw, 0, 0, toDraw.Width, toDraw.Height);
        }

        /// <summary>
        /// Uses the already-loaded background (bgOriginal/bgScaled) to build the window Region and set the BackgroundImage.
        /// This ensures the image is scaled before being assigned/drawn so no runtime resize flicker occurs.
        /// </summary>
        private void ApplyImageShapeFromLoadedBackground(byte alphaThreshold = 10)
        {
            Bitmap? bmp = bgScaled ?? bgOriginal;
            if (bmp == null)
                return;

            // Build region from opaque spans per row using LockBits
            var gp = new GraphicsPath();
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)data.Scan0;
                    int stride = data.Stride;
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        int x = 0;
                        byte* row = scan0 + y * stride;
                        while (x < bmp.Width)
                        {
                            // skip transparent pixels
                            while (x < bmp.Width)
                            {
                                byte alpha = row[x * 4 + 3];
                                if (alpha > alphaThreshold) break;
                                x++;
                            }
                            if (x >= bmp.Width) break;
                            int xStart = x;
                            // find opaque run end
                            while (x < bmp.Width)
                            {
                                byte alpha = row[x * 4 + 3];
                                if (alpha <= alphaThreshold) break;
                                x++;
                            }
                            int xEnd = x;
                            gp.AddRectangle(new Rectangle(xStart, y, xEnd - xStart, 1));
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            // Apply the image and region to the form (use the pre-scaled image)
            this.BackgroundImage = bmp;
            this.BackgroundImageLayout = ImageLayout.None;
            this.ClientSize = bmp.Size;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Dispose previous region and set new region
            this.Region?.Dispose();
            this.Region = new Region(gp);
        }

        // <summary>
        // Dispose managed resources
        // </summary>
        private void DisposeManagedResources()
        {
            // called by Designer Dispose
            bgOriginal?.Dispose();
            bgScaled?.Dispose();
        }


    }
}
