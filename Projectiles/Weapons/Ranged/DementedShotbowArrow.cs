using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Exlight.Projectiles.Weapons.Ranged
{
    public class DementedShotbowArrow : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_278";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demented Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.99f;
            Projectile.alpha = (int)MathHelper.Clamp((int)(Projectile.alpha - 0.7), 0f, 1f);
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch);
                d.noGravity = true;
            }
        }
    }
}
