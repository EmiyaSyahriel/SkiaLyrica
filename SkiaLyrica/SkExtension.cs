using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaLyrica
{
    public static partial class Extensions
    {
        public static void WithScale(this SKCanvas ctx, float sx, float sy, float px, float py, Action t)
        {
            ctx.Scale(sx, sy, px, py);
            t?.Invoke();
            ctx.Scale(1f / sx, 1f / sy, -px, -py);
        }
        public static void WithScale(this SKCanvas ctx, float sx, float sy, Action t) => ctx.WithScale(sx, sy, 0, 0, t);
        public static void WithScale(this SKCanvas ctx, float s, Action t) => ctx.WithScale(s, s, t);
        public static void WithTranslation(this SKCanvas ctx, float x, float y, Action t)
        {
            ctx.Translate(x, y);
            t?.Invoke();
            ctx.Translate(-x, -y);
        }
        public static void WithRotation(this SKCanvas ctx, float val, float px, float py, bool isRadian, Action t)
        {
            if (isRadian) ctx.RotateRadians(val, px, py);
            else ctx.RotateDegrees(val, px, py);

            t?.Invoke();

            if (isRadian) ctx.RotateRadians(-val, -px, -py);
            else ctx.RotateDegrees(-val, -px, -py);
        }

        public static void WithRotation(this SKCanvas ctx, float r, bool isRadian, Action t) => ctx.WithRotation(r, 0, 0, isRadian, t);
        public static void StrokeText(this SKCanvas ctx, string text, float x, float y, SKColor color, SKPaint paint)
        {
            paint.Color = color;
            paint.Style = SKPaintStyle.Stroke;
            ctx.DrawText(text, x, y, paint);
        }
        public static void FillText(this SKCanvas ctx, string text, float x, float y, SKColor color, SKPaint paint)
        {
            paint.Color = color;
            paint.Style = SKPaintStyle.Fill;
            ctx.DrawText(text, x, y, paint);
        }
    }
}
