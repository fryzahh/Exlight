using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class BloodBomb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_757";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Projectile.alpha -= 20;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.X * 0.085f;

            for (int k = 0; k < 4; k++)
            {
                Vector2 val5 = Projectile.Center + Main.rand.NextVector2CircularEdge(100, 100);
                Vector2 v = val5 - Projectile.Center;
                v = v.SafeNormalize(Vector2.Zero) * -8f;
                int num16 = Dust.NewDust(val5, 2, 2, DustID.Blood, v.X, v.Y, 40, default, 1.8f);
                Main.dust[num16].position = val5;
                Main.dust[num16].noGravity = true;
                Main.dust[num16].alpha = 250;
                Main.dust[num16].velocity = v;
                Main.dust[num16].customData = Projectile;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Vector2.UnitX.RotatedBy((Math.PI * 2 / 4 * i) + Projectile.rotation);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, speed * 2, ModContent.ProjectileType<BloodThorn>(), Projectile.damage / 2, 0f, Main.myPlayer);
            }
        }
    }
}
