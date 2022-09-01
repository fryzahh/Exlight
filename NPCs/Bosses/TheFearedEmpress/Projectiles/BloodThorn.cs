using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;
using Exlight.Common.Utilities;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class BloodThorn : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_756";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Thorn");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.45f;
        }

        public override void AI()
        {
            const int vel = 60;
            if (Projectile.velocity.Length() < vel)
                Projectile.velocity *= 1.01f;
            else
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * vel;

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item171, Projectile.Center);
                Projectile.localAI[0] = 1f;
                Projectile.frame = Main.rand.Next(5);
                for (int num160 = 0; num160 < 8; num160++)
                {
                    Dust obj73 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, Projectile.velocity.X, Projectile.velocity.Y, 100)];
                    obj73.velocity = (Main.rand.NextFloatDirection() * (float)Math.PI).ToRotationVector2() * 2f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 2f;
                    obj73.scale = 0.9f;
                    obj73.fadeIn = 1.1f;
                    obj73.position = Projectile.Center;
                }
            }
            
            Projectile.alpha -= 20;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float collisionPoint7 = 0f;
            Vector2 offset = 200 / 2 * Projectile.scale * Projectile.rotation.ToRotationVector2();
            Vector2 end = Projectile.Center - offset;
            Vector2 tip = Projectile.Center + offset;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, 6f * Projectile.scale, ref collisionPoint7))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, false, false);
            DrawHelper.DrawProjectileTrail(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Red), Projectile.rotation, true, Projectile.scale, true);
            return false;
        }
    }
}
