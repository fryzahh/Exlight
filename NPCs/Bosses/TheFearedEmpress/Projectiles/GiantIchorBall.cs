using System;
using System.IO;
using Exlight.Common.Systems.ParticleSystem;
using Exlight.Common.Utilities;
using Exlight.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class GiantIchorBall : ModProjectile
    {
        public override string Texture => "Exlight/NPCs/Bosses/TheFearedEmpress/Projectiles/OrbitingIchorOrb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Ichor Orb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.scale = 0f;
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
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 180)
            {
                Projectile.velocity = Vector2.Zero.MoveTowards(npc.Center - Projectile.Center, 20f);
                if (Projectile.scale < 3f)
                {
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 3f, Projectile.localAI[0] / 180);
                }
            }

            if (Projectile.localAI[0] == 180)
            {
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 15f;
                Projectile.netUpdate = true;
            }

            if (Projectile.localAI[0] > 180)
            {
                Projectile.velocity *= 0.9f;
            }

            if (Projectile.localAI[0] > 240)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 baseDirection = Projectile.DirectionTo(player.Center);
                const int max = 6;
                for (int i = 0; i < max; i++)
                {
                    Vector2 offset = Projectile.height / 2 * baseDirection.RotatedBy(Math.PI * 2 / max * i);
                    ExlightUtils.NewProjectileBetter(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), Vector2.Zero, ModContent.ProjectileType<IchorChainExplosion>(), 120, 0f, default, MathHelper.WrapAngle(offset.ToRotation()), 32);
                }

                int numProj = Main.masterMode ? 40 : Main.expertMode ? 30 : 20;
                for (int i = 0; i < numProj; i++)
                {
                    float speed = 6f;
                    Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2 / numProj * i) * speed;
                    ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShot>(), 70, 0f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Yellow), Projectile.rotation, Projectile.scale, false, false);
            DrawHelper.DrawProjectileTrail(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Yellow * 0.45f), Projectile.rotation, true, Projectile.scale / 2, false, true, true);
            return false;
        }
    }
}
