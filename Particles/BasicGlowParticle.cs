using System;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Exlight.Common.Systems.ParticleSystem;
using Terraria.ModLoader;
using ReLogic.Content;

namespace Exlight.Particles
{
    public class BasicGlowParticle : Particle
    {
        public override string Texture => "Exlight/Textures/BloomLight";

        public override bool UseAdditiveBlending => true;
        public override bool UseCustomDrawing => true;

        public override bool HasSetLifetime => false;

        private float Opacity;
        private Color BaseColor;
        private bool AffectedByWindSpeed;
        private bool DynamicScaleDecrease;

        public BasicGlowParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime, bool affectedByWindSpeed = false, bool dynamicScaleDecrease = false)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            Scale = scale;
            MaxTime = maxTime;
            AffectedByWindSpeed = affectedByWindSpeed;
            DynamicScaleDecrease = dynamicScaleDecrease;
        }

        public override void Update()
        {
            Lighting.AddLight(Position, Color.ToVector3() * Scale);
            Opacity = (float)Math.Sin((double)((MaxTime - LifeTime) / 255f) * (float)Math.PI);
            Color = BaseColor * Opacity;
            Velocity *= 0.97f;
            if (AffectedByWindSpeed)
                Velocity.X = Velocity.X + Main.windSpeedCurrent * 0.07f;
            if (DynamicScaleDecrease)
                Scale *= Utils.GetLerpValue(MaxTime, ParticleLifeCompletion, LifeTime, true);
            if (Opacity <= 0)
                Kill();
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
