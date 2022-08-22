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
    public class EmpressSword : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Festering Blade");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 32;
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
            if (Projectile.localAI[0] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                ExlightUtils.NewProjectileBetter(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EmpressProjectileTelegraph>(), 0, 0f, ai0: Projectile.whoAmI, ai1: 60);
            }

            if (Projectile.localAI[0] >= 60f)
            {
                Projectile.velocity = Projectile.ai[0].ToRotationVector2() * 40f;
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
            if (Projectile.localAI[0] >= 360f)
            {
                Projectile.Kill();
                return;
            }
            Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Utils.GetLerpValue(0f, 20f, Projectile.localAI[0], clamped: true));
            Projectile.rotation = Projectile.ai[0];
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
            int num = 40;
            int num2 = 180 * num;
            num2 /= 2;
            Color color = Color.Yellow;
            Color value = color;
            color.A = 0;
            value.A = (byte)(value.A / 2);
            Texture2D value2 = TextureAssets.Extra[178].Value;
            Vector2 origin = value2.Frame().Size() * new Vector2(0f, 0.5f);
            Vector2 scale = new Vector2((float)(num2 / value2.Width), 2f);
            Vector2 scale2 = new Vector2((float)(num2 / value2.Width) * 0.5f, 2f);
            Color color2 = color * Utils.GetLerpValue(60f, 55f, Projectile.localAI[0], clamped: true) * Utils.GetLerpValue(0f, 10f, Projectile.localAI[0], clamped: true);
            Main.spriteBatch.Draw(value2, vector, (Rectangle?)null, color2, Projectile.rotation, origin, scale2, (SpriteEffects)0, 0f);
            Main.spriteBatch.Draw(value2, vector, (Rectangle?)null, color2 * 0.3f, Projectile.rotation, origin, scale, (SpriteEffects)0, 0f);
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
