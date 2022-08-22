using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Exlight.Projectiles.BaseProjectileClasses;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class EmpressProjectileTelegraph : BaseTelegraphLineProjectile
    {
        public override string Texture => "Exlight/Textures/BloomLine";
        public override Color DrawColor => Color.Yellow;
        public override int Owner => (int)Projectile.ai[0];
        public override int LifeTime => (int)Projectile.ai[1];
        public override float Alpha => 200f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Feared Empress Telegraph");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 0.15f;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation = Main.projectile[Owner].rotation;
        }
    }
}
