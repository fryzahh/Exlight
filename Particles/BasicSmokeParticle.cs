using Terraria;
using Microsoft.Xna.Framework;
using Exlight.Common.Systems.ParticleSystem;

namespace Exlight.Particles
{
    public class BasicSmokeParticle : Particle
    {
        public override string Texture => "Exlight/Particles/Smoke";
        public override int ParticleFrameCount => 3;

        private float Opacity;
        private float RotationSpeed;
        private Color StartColor;
        private Color FadeOutColor;
        private bool AffectedByWindSpeed;

        public BasicSmokeParticle(Vector2 position, Vector2 velocity, Color startColor, Color fadeColor, float scale, float opacity, float rotationSpeed, bool affectedByWindSpeed = false)
        {
            Position = position;
            Velocity = velocity;
            StartColor = startColor;
            FadeOutColor = fadeColor;
            Frame = Main.rand.Next(3);
            Scale = scale;
            Opacity = opacity;
            RotationSpeed = rotationSpeed;
            AffectedByWindSpeed = affectedByWindSpeed;
        }

        public override void Update()
        {
            Color = Color.Lerp(StartColor, FadeOutColor, MathHelper.Clamp((255f - Opacity - 100f) / 70f, 0f, 1f)) * (Opacity / 255f);
            Rotation += RotationSpeed * ((Velocity.X > 0) ? 1f : -1f);
            Velocity *= 0.96f;
            if (AffectedByWindSpeed)
            {
                Velocity.X += Main.windSpeedCurrent * 0.06f;
            }

            if (Opacity > 80f)
            {
                Scale += 0.01f;
                Opacity -= 3f;
            }
            else
            {
                Scale *= 0.985f;
                Opacity -= 2f;
            }

            if (Opacity < 0f)
                Kill();
        }
    }
}
