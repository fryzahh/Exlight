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

            for (int num161 = 0; num161 < 2; num161++)
            {
                Dust obj74 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, Projectile.velocity.X, Projectile.velocity.Y, 100)];
                obj74.velocity = obj74.velocity / 4f + Projectile.velocity / 2f;
                obj74.scale = 1.2f;
                obj74.position = Projectile.Center + Main.rand.NextFloat() * Projectile.velocity * 2f;
            }
            for (int num162 = 1; num162 < Projectile.oldPos.Length && !(Projectile.oldPos[num162] == Vector2.Zero); num162++)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Dust obj75 = Main.dust[Dust.NewDust(Projectile.oldPos[num162], Projectile.width, Projectile.height, 5, Projectile.velocity.X, Projectile.velocity.Y, 100)];
                    obj75.velocity = obj75.velocity / 4f + Projectile.velocity / 2f;
                    obj75.scale = 1.2f;
                    obj75.position = Projectile.oldPos[num162] + Projectile.Size / 2f + Main.rand.NextFloat() * Projectile.velocity * 2f;
                }
            }

            Projectile.alpha -= 20;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint7 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale, ref collisionPoint7))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value243 = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle value181 = value243.Frame(1, 6, 0, Projectile.frame);
            Vector2 origin10 = new Vector2(16f, (float)(value181.Height / 2));
            Color alpha14 = Projectile.GetAlpha(lightColor);
            Vector4 vector31 = lightColor.ToVector4();
            Color val7 = new Color(67, 17, 17);
            Vector4 vector32 = val7.ToVector4();
            vector32 *= vector31;
            Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity * Projectile.scale * 0.5f, null, Projectile.GetAlpha(new Color(vector32.X, vector32.Y, vector32.Z, vector32.W)) * 1f, Projectile.rotation + (float)Math.PI / 2f, TextureAssets.Extra[98].Value.Size() / 2f, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(value243, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), value181, alpha14, Projectile.rotation, origin10, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
