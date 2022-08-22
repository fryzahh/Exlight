using System;
using System.IO;
using Exlight.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class OrbitingIchorOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Festering Ichor Orb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile center = Main.projectile[(int)Projectile.ai[0]];
            if (!center.active)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;

            if (Projectile.velocity.X != 0f)
            {
                Projectile.localAI[0] = Projectile.velocity.X;
                Projectile.velocity.X = 0f;
            }
            if (Projectile.velocity.Y != 0f)
            {
                Projectile.localAI[1] = Projectile.velocity.Y;
                Projectile.velocity.Y = 0f;
            }
            
            Projectile.timeLeft = 2;
            Projectile.ai[1] += 2f * (float)Math.PI / 600f * Projectile.localAI[1];
            Projectile.ai[1] %= 2f * (float)Math.PI;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
            Projectile.Center = center.Center + Projectile.localAI[0] * new Vector2((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1]));
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                ExlightUtils.NewProjectileBetter(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OrbitingIchorOrbFumes>(), 50, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Yellow), Projectile.rotation, Projectile.scale, false, false);
            //DrawHelper.DrawProjectileTrail(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Yellow), Projectile.rotation, true, Projectile.scale, false, true, true);
            return false;
        }
    }
}
