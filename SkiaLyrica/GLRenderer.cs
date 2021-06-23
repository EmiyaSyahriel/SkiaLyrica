using SkiaSharp;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;
using System.IO;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SkiaLyrica
{
    public static class GLRenderer
    {
        private static SKSurface surface;
        private static GRContext context;
        private static SKPaint dpaint = new SKPaint() {
            Color = SKColors.Lime,
            TextSize = 20f,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("monospace"),
            TextEncoding = SKTextEncoding.Utf32
        };
        private static MediaPlayer player;
        private static GRBackendRenderTarget target;
        private static List<Subrip.SubtitleText> subtitle;
        private static float RefWidth = 1280F;
        private static float RefHeight = 720F;
        private static float WinWidth = 1280F;
        private static float WinHeight = 720F;
        public static string Title = "Window";

        public static List<VParticle> particles = new List<VParticle>();

        public static float LineWidth {get => dpaint.StrokeWidth; set => dpaint.StrokeWidth = value;}
        public static SKColor Color {get => dpaint.Color; set => dpaint.Color = value;}
        public static SKPaintStyle Style {get => dpaint.Style; set => dpaint.Style = value;}

        public static void InitAudio()
        {
            Bass.Init();
            player = new MediaPlayer();
            if (File.Exists("Lyrica_Data/source.mp3"))
            {
                player.LoadAsync("Lyrica_Data/source.mp3").ContinueWith((it) => { if (it.Result) player.Play(); });
            }

            for(int i = 0; i < 256; i++) particles.Add(new VParticle());

            if (File.Exists("Lyrica_Data/source.srt"))
            {
                subtitle = Subrip.SubripParser.LoadFile("Lyrica_Data/source.srt");
            }
        }

        public static void InitSKIA()
        {
            CleanUp();
            var interf = GRGlInterface.Create();
            while(interf == null) interf = GRGlInterface.Create(); // Bad Practice - Stall until interface is not null
            context = GRContext.CreateGl(interf);
            while (context == null) context = GRContext.CreateGl(interf); // Bad Practice - Stall until context is not null
        }

        public static void OnResize(int w, int h)
        {
            if (context == null) InitSKIA();

            GRGlFramebufferInfo fbinfo = new GRGlFramebufferInfo()
            {
                FramebufferObjectId = 0,
                Format = (uint)InternalFormat.Rgba8
            };

            SKColorType color = SKColorType.Rgba8888;
            target = new GRBackendRenderTarget(w, h, 0, 0, fbinfo);
            surface = SKSurface.Create(context, target, GRSurfaceOrigin.BottomLeft, color);

            WinWidth = w;
            WinHeight = h;
        }

        public static void CleanUp(bool includePlayer = false)
        {
            surface?.Dispose();
            context?.Dispose();
            if (includePlayer)
            {
                player?.Dispose();
            } 
        }

        private static byte ebyte(uint b, int s, int m = 0xFF) { return (byte)((b >> s) & m); }
        private static byte ebyte(int b, int s, int m = 0xFF) { return (byte)((b >> s) & m); }
        private static SKColor rgba(byte r, byte g, byte b, byte a = 0x00) => new SKColor(r, g, b, a);
        private static SKColor c4(uint s = 0xFFFFFFFF) => new SKColor(ebyte(s,24), ebyte(s,16), ebyte(s,8), ebyte(s,0));
        private static SKColor c3(int s = 0xFFFFFF) => new SKColor(ebyte(s,16), ebyte(s,8), ebyte(s,0), 0xFF);
        private static SKColor c3a(int s = 0xFFFFFF, byte a = 0xFF) => new SKColor(ebyte(s,16), ebyte(s,8), ebyte(s,0), a);
        private static SKPoint pf(float x, float y) => new SKPoint(x,y);
        private static SKPointI pi(int x, int y) => new SKPointI(x,y);
        private static SKPoint3 p3(float x, float y, float z) => new SKPoint3(x,y,z);
        private static SKRect rect(float x, float y, float r, float b) => new SKRect(x, y, r, b);
        private static SKRectI recti(int x, int y, int r, int b) => new SKRectI(x, y, r, b);

        public static void OnKeyDown(Keys key, bool ctrl, bool shift, bool alt)
        {
            int seek = shift ? 10 : 1;
            switch (key)
            {
                case Keys.Space:
                    if (player.IsPaused()) player.Play(); else player.Pause();
                    break;
                case Keys.Left:
                    player.Position -= TimeSpan.FromSeconds(seek);
                    break;
                case Keys.Right:
                    player.Position += TimeSpan.FromSeconds(seek);
                    break;
                case Keys.Up: player.Volume += 0.1; break;
                case Keys.Down: player.Volume -= 0.1; break;
            }
        }

        private static void DrawDebugInfo(SKCanvas ctx, float time)
        {
            float FPS = 1 / time;
            Color = c3(0x00FF00);
            dpaint.TextSize = 12;
            ctx.DrawText($"PSW Lyrica Project running over NET 5.0", pf(0, dpaint.TextSize * 1), dpaint);
            ctx.DrawText($"Library : OpenTK, SkiaSharp, ManagedBass, ILRepack", pf(0, dpaint.TextSize * 2), dpaint);
            Title = $"[{player.State}@{FPS:00.0}FPS] {player.Title} - {player.Album} - PSW Lyrica Project (net5.0)";
        }

        private static SKBitmap bg0, bg1, prtbm;
        private static SKShader gradientShader,defShader;
        private static SKMaskFilter blurFilter;

        private static void DrawBackground(SKCanvas ctx)
        {
            if (bg0 == null && File.Exists("Lyrica_Data/bg0.jpg")) bg0 = SKBitmap.Decode(File.OpenRead("Lyrica_Data/bg0.jpg"));
            if (bg1 == null && File.Exists("Lyrica_Data/bg1.jpg")) bg1 = SKBitmap.Decode(File.OpenRead("Lyrica_Data/bg1.jpg"));

            TimeSpan fadeStart = new TimeSpan(0, 3, 10), fadeEnd = new TimeSpan(0, 3, 12);

            float fadei = clamp01(ilerp(fadeStart.s(), fadeEnd.s(), player.Position.s()));
            
            Color = c3a(0xFFFFFF, (byte)lerp(0xFF, 0x00, fadei));
            ctx.DrawBitmap(bg1, rect(0, 0, 1280, 720), rect(0, 0, 1280, 720), dpaint);
            Color = c3a(0xFFFFFF, (byte)lerp(0x00, 0xFF, fadei));
            ctx.DrawBitmap(bg0, rect(0, 0, 1280, 720), rect(0, 0, 1280, 720), dpaint);

            if (defShader == null)
            {
                defShader = dpaint.Shader;
            }

            if(gradientShader == null)
            {
                gradientShader = SKShader.CreateLinearGradient(
                    pf(0f,0f), 
                    pf(1280f,0f), 
                    new[] { c4(0xffffffcc), c4(0xffffff00) }, 
                    new[] { 0.8f, 1.0f }, 
                    SKShaderTileMode.Clamp);
            }

            Color = c4(0xFFFFFFFF);
            DrawParticles(ctx);
            dpaint.Shader = gradientShader;
            ctx.DrawRect(rect(0f,0f,1280f,720f), dpaint);
            dpaint.Shader = defShader;
        }

        private static SKBitmap coverImage;

        private static void DrawSongInfo(SKCanvas ctx)
        {
            if(coverImage == null && File.Exists("Lyrica_Data/cover.jpg"))
            {
                using (var imgf = File.OpenRead("Lyrica_Data/cover.jpg")) coverImage = SKBitmap.Decode(imgf);
            }

            if (coverImage != null) ctx.DrawBitmap(coverImage, rect(20, 600, 120, 700));

            var fill = c3a(0xFF88FF, 0xFF);
            var stroke = c3a(0xFFFFFF, 0xFF);

            var yOffset = 100 / 5f;
            dpaint.TextAlign = SKTextAlign.Left;

            string title = player?.Title ?? "No Media", artist = player?.Artist ?? "", album = player?.Album ?? "";

            dpaint.TextSize = 30f;
            ctx.StrokeText(title, 140f, 600 + (yOffset*2), stroke, dpaint);
            ctx.FillText(title, 140f, 600 + (yOffset * 2), fill, dpaint);

            dpaint.TextSize = 20f;
            ctx.StrokeText(artist, 140f, 600 + (yOffset * 3), stroke, dpaint);
            ctx.FillText(artist, 140f, 600 + (yOffset * 3), fill, dpaint);

            dpaint.TextSize = 15f;
            ctx.StrokeText(album, 140f, 600 + (yOffset * 4), stroke, dpaint);
            ctx.FillText(album, 140f, 600 + (yOffset * 4), fill, dpaint);

            Color = c3a(0xFF88FF, 0x33);
            ctx.DrawRect(rect(0,720-5,1280,720), dpaint);
            Color = c3a(0xFF88FF, 0xFF);
            ctx.DrawRect(rect(0, 720 - 5, lerp(0,1280,player.Position.s() / player.Duration.s()), 720), dpaint);
        }

        private static float lastPlayerTime = 0f;
        private static void DrawParticles(SKCanvas ctx)
        {
            if (prtbm == null && File.Exists("Lyrica_Data/particle.png")) prtbm = SKBitmap.Decode(File.OpenRead("Lyrica_Data/particle.png"));
            var ctime = (float)(player?.Position ?? TimeSpan.FromSeconds(0)).TotalSeconds;
            var deltaTime = ctime - lastPlayerTime;

            lastPlayerTime = ctime;
            //Color = c4(0xFF000055);
            particles.ForEach((it) => {
                it.Move(deltaTime);
                dpaint.TextSize = it.size;
                // ctx.DrawText(it.Char, pf(it.x, it.y), dpaint);
                if(prtbm != null) ctx.DrawBitmap(prtbm, rect(0,0,16,16), rect(it.x, it.y, it.x + it.size, it.y+ it.size));
            });
        }
        private static float clamp(float t, float min, float max) { return Math.Min(max, Math.Max(min, t)); }
        private static float clamp01(float t) { return clamp(t, 0, 1); }
        private static float lerp(float a, float b, float t) { return a + ((b - a) * t); }
        private static float ilerp(float a, float b, float v) { return (v - a) / (b - a); }
        private static float lerpc(float a, float b, float t) { return a + ((b - a) * clamp01(t)); }
        private static float ilerpc(float a, float b, float v) { return clamp01((v - a) / (b - a)); }

        private static void DrawSubtitles(SKCanvas ctx)
        {
            if(blurFilter == null)
            {
                blurFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 10f);
            }

            if(subtitle != null)
            {
                dpaint.TextAlign = SKTextAlign.Center;
                var currentTime = TimeSpan.FromSeconds(lastPlayerTime);
                subtitle.ForEach((it) => {

                    if (it.Text.Length > 0)
                    {
                        var lines = it.Text.Split('\n');
                        byte alpha = 255;
                        float yOffset = 0;
                        
                        if(currentTime < it.Start)
                        {
                            var dlerp = ilerp(it.Start.s() - 0.3f, it.Start.s(), currentTime.s());
                            alpha = (byte)(clamp01(dlerp) * 255);
                            yOffset = clamp(25 * (1 - clamp01(dlerp)), 0, 25);
                        }
                        else if (it.End < currentTime)
                        {
                            var dlerp = ilerp(it.End.s() + 0.3f, it.End.s(), currentTime.s());
                            alpha = (byte)(clamp01(dlerp) * 255);
                            yOffset = clamp(-25 * (1 - clamp01(dlerp)), -25, 0);
                        }

                        var shadow = c3a(0xFF88FF, (byte)(Math.Max(0, alpha - 32)));
                        var fill = c3a(0xFF88FF, alpha);
                        var stroke = c3a(0xFFFFFF, alpha);
                        dpaint.StrokeWidth = 5;
                        // blur filter
                        dpaint.MaskFilter = blurFilter;
                        if (lines.Length >= 1)
                        {
                            dpaint.TextSize = 30;
                            ctx.FillText(lines[0], 640, 360 + yOffset, shadow, dpaint);
                        }
                        if (lines.Length >= 2)
                        {
                            dpaint.TextSize = 18;
                            ctx.FillText(lines[1], 640, 380 + yOffset, shadow, dpaint);
                        }

                        dpaint.MaskFilter = null;
                        if (lines.Length >= 1)
                        {
                            dpaint.TextSize = 30;
                            ctx.StrokeText(lines[0], 640, 360 + yOffset, stroke, dpaint);
                            ctx.FillText(lines[0], 640, 360 + yOffset, fill, dpaint);
                        }
                        if(lines.Length >= 2)
                        {
                            dpaint.TextSize = 18;
                            ctx.StrokeText(lines[1], 640, 380 + yOffset, stroke, dpaint);
                            ctx.FillText(lines[1], 640, 380 + yOffset, fill, dpaint);
                        }
                    }

                });
            }
        }

        private static float DrawScale => Math.Min(WinWidth / RefWidth, WinHeight / RefHeight);

        public static void Render(float time)
        {
            try
            {
                while(context == null) { };
                while(surface == null) { };
                var ctx = surface.Canvas;
                ctx.Clear(c4(0xFF00FF00));

                var scale = DrawScale;

                ctx.WithScale(scale, () => {
                    DrawBackground(ctx);
                    DrawSubtitles(ctx);
                    DrawSongInfo(ctx);
                    DrawDebugInfo(ctx, time);
                });

                context.Flush();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
