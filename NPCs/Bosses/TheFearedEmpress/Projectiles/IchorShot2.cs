using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Exlight.Common.Utilities;
using Terraria.GameContent;
using Exlight.Common.Systems.ParticleSystem;
using Exlight.Particles;


namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class IchorShot2 : IchorShot
    {
        public override string Texture => "Exlight/NPCs/Bosses/TheFearedEmpress/Projectiles/IchorShot";

        public override void AI()
        {
            base.AI();
            Vector2 vel = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
            Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(Projectile.velocity.ToRotation().AngleLerp(vel.ToRotation(), 0.045f));
        }
    }
}
