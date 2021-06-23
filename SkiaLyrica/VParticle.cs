using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaLyrica
{
    public class VParticle
    {
        private static float lerp(float a, float b, float t) => a + ((b - a) * t);
        private static Random rng = new Random(DateTime.Now.Year * DateTime.Now.Second * DateTime.Now.Minute);
        private static float random => rng.Next(0, 20000) / 20000f;

        public static readonly char[] chars = "かくれんぼつていたかしらおにわたし赤い世界消えるころアイドルマスター泣激燃や栗井感情".ToCharArray();
        public float x = 0;
        public float y = 0;
        public string Char = "";
        public float size = 10;
        public float speed = 10;

        public VParticle()
        {
            Reset(true);
        }

        public void Reset(bool warmed)
        {
            size = lerp(5, 20, random);
            speed = lerp(10, 50, random);
            x = (warmed ? lerp(0, 1280, random) : 1280) - size;
            y = lerp(-720, 720, random) - size;
            Char = $"{chars[rng.Next(0, chars.Length - 1)]}";
        }

        public void Move(float deltaTime)
        {
            y += speed * deltaTime;
            x -= speed * deltaTime;
            if (
                (y < - 720 -(size * 2) || y > (720 + (size * 2))) ||
                (x < -(size * 2) || x > (1280 + (size * 2))) 
                ) Reset(false);
        }
    }
}
