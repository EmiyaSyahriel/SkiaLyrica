using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using System;
using System.IO;

namespace SkiaLyrica
{
    class Program : GameWindow
    {
        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            GLRenderer.InitAudio();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GLRenderer.OnResize(e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GLRenderer.Render((float)args.Time);
            SwapBuffers();
            Title = GLRenderer.Title;
        }

        protected override void OnUnload()
        {
            GLRenderer.CleanUp(true);
            base.OnUnload();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            GLRenderer.OnKeyDown(e.Key, e.Control, e.Shift, e.Alt);
            base.OnKeyDown(e);
        }

        static void Main(string[] args)
        {
            GameWindowSettings gws = new GameWindowSettings();
            gws.IsMultiThreaded = true;
            gws.RenderFrequency = 60.0f;
            gws.UpdateFrequency = 100.0f;
            NativeWindowSettings nws = NativeWindowSettings.Default;
            nws.Size = new OpenTK.Mathematics.Vector2i(1280, 720);
            using (var prog = new Program(gws, nws))
            {
                prog.Run();
            }
        }
    }
}
