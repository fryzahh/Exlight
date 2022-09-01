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
    public class IchorArenaOrb : ModProjectile
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
            Projectile.alpha = 255;
            Projectile.scale = 2f;
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
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!Main.npc.IndexInRange(npc.whoAmI))
            {
                Projectile.active = false;
                return;
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 1f));
                ParticleManager.SpawnParticle(new ScaleWithTimeParticle(spawnPos, vel, color, Main.rand.NextFloat(0.2f, 0.3f), Main.rand.Next(180, 190)));
            }

            Projectile.localAI[0]++;
            if (Projectile.ai[1] == 0f)
            {
                if (Projectile.localAI[0] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 dir = i != 1 ? -Vector2.UnitX : -Vector2.UnitY;
                        Vector2 vel = dir * 10f;
                        ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShot>(), 10, 20f);
                    }
                }
            }

            if (Projectile.ai[1] == 1f)
            {
                if (Projectile.localAI[0] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 dir = i != 1 ? Vector2.UnitX : Vector2.UnitY;
                        Vector2 vel = dir * 10f;
                        ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShot>(), 10, 20f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Yellow), Projectile.rotation, Projectile.scale, false, false);
            return false;
        }
    }
}
