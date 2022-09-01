using System;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Exlight.Common.Systems.ParticleSystem;
using Terraria.ModLoader;
using ReLogic.Content;

namespace Exlight.Particles
{
    public class ScaleWithTimeParticle : Particle
    {
        public override string Texture => "Exlight/Textures/BloomLight";

        public override bool UseAdditiveBlending => true;
        public override bool UseCustomDrawing => true;
        public override bool HasSetLifetime => true;

        private float Opacity;
        private Color BaseColor;
        private bool AffectedByWindSpeed;

        public ScaleWithTimeParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime, bool affectedByWindSpeed = false)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            Scale = scale;
            MaxTime = maxTime;
            AffectedByWindSpeed = affectedByWindSpeed;
        }

        public override void Update()
        {
            Lighting.AddLight(Position, Color.ToVector3() * Scale);
            Opacity = (float)Math.Sin((double)((MaxTime - LifeTime) / 255f) * (float)Math.PI);
            Color = BaseColor * Opacity;

            Scale = Utils.GetLerpValue(MaxTime, LifeTime / 3, LifeTime, true);

            Velocity *= 0.97f;
            if (AffectedByWindSpeed)
                Velocity.X = Velocity.X + Main.windSpeedCurrent * 0.07f;
        }

        public override void CustomDrawing(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
            Texture2D bloom = ModContent.Request<Texture2D>("Exlight/Textures/BloomCircle", (AssetRequestMode)2).Value;
            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, Color, Rotation, bloom.Size() / 2, Scale / 5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation, texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
        }
    }
}
