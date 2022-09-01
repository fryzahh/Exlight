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
    public class IchorCrystalShard : IchorShot
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Ichor Crystal Shard");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity *= 1f + Projectile.ai[1];
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
