using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Exlight.Common.Systems.ParticleSystem;
using Exlight.Particles;
using System;
using Exlight.Common.Utilities;
using Terraria.GameContent;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class EmpressSword2 : ModProjectile
    {
        public override string Texture => "Exlight/NPCs/Bosses/TheFearedEmpress/Projectiles/EmpressSword";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Festering Blade");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;       
            if (Projectile.localAI[0] == 60)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.player[Projectile.owner].Center) * 15f;
                Projectile.netUpdate = true;
            }

            if (Projectile.localAI[0] >= 60f)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 267);
                    dust.fadeIn = 1f;
                    dust.noGravity = true;
                    dust.alpha = 100;
                    dust.color = Color.Lerp(Color.Yellow, Color.White, Main.rand.NextFloat() * 0.4f);
                    dust.noLightEmittence = true;
                    dust.scale *= 1.5f;
                }
            }
            else
            {
                if (Main.masterMode)
                {
                    Projectile.ai[1] -= 0.015f;
                }
                Vector2 pos = Main.player[Projectile.owner].Center + Vector2.UnitX.RotatedBy(Projectile.ai[1]) * 250f;
                Projectile.velocity = Vector2.Zero.MoveTowards(pos - Projectile.Center, 20f);
            }

            if (Projectile.localAI[0] >= 180)
            {
                Projectile.Kill();
                return;
            }
            Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Utils.GetLerpValue(0f, 20f, Projectile.localAI[0], clamped: true));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float f4 = Projectile.rotation;
            float collisionPoint10 = 0f;
            float scaleFactor3 = 40f;
            Vector2 value5 = f4.ToRotationVector2();
            projHitbox.X = (int)Projectile.position.X - projHitbox.Width / 2;
            projHitbox.Y = (int)Projectile.position.Y - projHitbox.Height / 2;
            if (projHitbox.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - value5 * scaleFactor3, Projectile.Center + value5 * scaleFactor3, 8f, ref collisionPoint10))
            {
                return true;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.15f, 0.30f), Main.rand.Next(40, 50), false, true));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Color color = Color.Yellow;
            Color value = color;
            color.A = 0;
            value.A = (byte)(value.A / 2);
            Texture2D value3 = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin2 = value3.Frame().Size() / 2f;
            Color color3 = Color.White * Utils.GetLerpValue(0f, 20f, Projectile.localAI[0], clamped: true);
            color3.A = (byte)(color3.A / 2);
            float num3 = MathHelper.Lerp(0.7f, 1f, Utils.GetLerpValue(55f, 60f, Projectile.localAI[0], clamped: true));
            float lerpValue = Utils.GetLerpValue(10f, 60f, Projectile.localAI[0]);
            if (lerpValue > 0f)
            {
                float lerpValue2 = Utils.GetLerpValue(0f, 1f, Projectile.velocity.Length(), clamped: true);
                for (float num4 = 1f; num4 > 0f; num4 -= 1f / 6f)
                {
                    Vector2 value4 = Projectile.rotation.ToRotationVector2() * -120f * num4 * lerpValue2;
                    Main.spriteBatch.Draw(value3, vector + value4, (Rectangle?)null, color * lerpValue * (1f - num4), Projectile.rotation, origin2, num3, (SpriteEffects)0, 0f);
                    Main.spriteBatch.Draw(value3, vector + value4, (Rectangle?)null, new Color(255, 255, 255, 0) * 0.15f * lerpValue * (1f - num4), Projectile.rotation, origin2, num3 * 0.85f, (SpriteEffects)0, 0f);
                }
                for (float num5 = 0f; num5 < 1f; num5 += 0.25f)
                {
                    Vector2 value5 = (num5 * ((float)Math.PI * 2f) + Projectile.rotation).ToRotationVector2() * 2f * num3;
                    Main.spriteBatch.Draw(value3, vector + value5, (Rectangle?)null, value * lerpValue, Projectile.rotation, origin2, num3, (SpriteEffects)0, 0f);
                }
                Main.spriteBatch.Draw(value3, vector, (Rectangle?)null, value * lerpValue, Projectile.rotation, origin2, num3 * 1.1f, (SpriteEffects)0, 0f);
            }
            Main.spriteBatch.Draw(value3, vector, (Rectangle?)null, color3, Projectile.rotation, origin2, num3, (SpriteEffects)0, 0f);
            return false;
        }
    }
}
