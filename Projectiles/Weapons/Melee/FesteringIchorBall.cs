using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Exlight.Projectiles.Weapons.Melee
{
    public class FesteringIchorBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Festering Ichor Mass");
        }

        public override void SetDefaults()
        {
            Projectile.width = 23;
            Projectile.height = 23;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += Projectile.velocity.X * 0.06f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            if (Projectile.velocity.Y <= 6f)
            {
                if (Projectile.velocity.X > 0f && Projectile.velocity.X < 5f)
                {
                    Projectile.velocity.X += 0.05f;
                }
                if (Projectile.velocity.X < 0f && Projectile.velocity.X > -5f)
                {
                    Projectile.velocity.X -= 0.05f;
                }
            }

            if (Projectile.wet)
            {
                if (Projectile.velocity.Y >= -7)
                    Projectile.velocity.Y -= 0.85f;
            }
            Projectile.velocity.Y += 0.1f;
            if (++Projectile.ai[1] == 1)
            {
                Projectile.scale = Main.rand.NextFloat(1f, 2f);
                Projectile.position = Projectile.Center;
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;
            }

            if (Projectile.ai[1] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FesteringIchorTrail>(), Projectile.damage / 2, 0f, Projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Zombie116, Projectile.position);
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -7f;
                }
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 0, 100) * Projectile.Opacity;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath64, Projectile.position);
            for (int i = 0; i < 80; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.t_Slime, 0f, 0f, 100, new Color(255, 255, 0, 100), 1f);
                Main.dust[dustIndex].velocity *= 5f;
            }
        }
    }
}
