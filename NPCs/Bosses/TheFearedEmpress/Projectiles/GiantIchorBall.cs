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
            Player player = Main.player[npc.target];
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 180)
            {
                Projectile.velocity = Vector2.Zero.SafeNormalize(Projectile.DirectionTo(npc.Center) * 20f);
                for (int k = 0; k < 4; k++)
                {
                    Vector2 val5 = Projectile.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(50, 300), Main.rand.NextFloat(50, 300));
                    Vector2 v = val5 - Projectile.Center;
                    v = v.SafeNormalize(Vector2.Zero) * -8f;
                    Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                    ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.25f, 0.66f), Main.rand.Next(60, 70), false, true));
                }
                if (Projectile.scale < 3f)
                {
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 3f, 0.012f);
                }
            }

            if (Projectile.localAI[0] == 180)
            {
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 15f;
                Projectile.ai[1] = Utils.SelectRandom(Main.rand, 0, 1, 2);
                Projectile.netUpdate = true;
            }

            if (Projectile.localAI[0] > 180 && Projectile.localAI[0] < 360)
            {
                Projectile.velocity *= 0.9f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Projectile.ai[0] == 0f)
                    {
                        if (Projectile.localAI[0] % 60 == 0)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float projSpeed = 7f;
                                Vector2 dir = Vector2.Normalize(player.Center - Projectile.Center);
                                Vector2 vel = dir.RotatedBy(Math.PI * 2 / 8 * i) * projSpeed;
                                ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShotHoming>(), 50, 0f);
                            }

                            for (int i = 0; i < 8; i++)
                            {
                                float projSpeed = 14f;
                                Vector2 dir = Vector2.Normalize(player.Center - Projectile.Center);
                                Vector2 vel = dir.RotatedBy(Math.PI * 2 / 8 * i) * projSpeed;
                                ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShotHoming>(), 50, 0f);
                            }
                        }
                    }

                    if (Projectile.ai[0] == 1f)
                    {
                        int numProj = Main.masterMode ? 16 : 8;
                        if (Projectile.localAI[0] % 45 == 0)
                        {
                            for (int i = 0; i < numProj; i++)
                            {
                                float projSpeed = 7f;
                                Vector2 dir = Vector2.Normalize(player.Center - Projectile.Center);
                                Vector2 vel = dir.RotatedBy(Math.PI * 2 / numProj * i) * projSpeed;
                                ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShot>(), 70, 0f);
                            }
                        }

                        float rotationalSpeed = 45f;
                        Projectile.localAI[0] += (float)Math.PI / rotationalSpeed;
                        if (Projectile.localAI[0] % 5 == 0)
                        {
                            int numProj2 = 4;
                            for (int i = 0; i < numProj2; i++)
                            {
                                float projSpeed = 5f;
                                Vector2 vel = Vector2.UnitX.RotatedBy(Projectile.localAI[0] + (Math.PI * 2 / numProj2 * i)) * projSpeed;
                                ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShot>(), 60, 0f);
                            }
                        }
                    }

                    if (Projectile.ai[0] == 2f)
                    {
                        float rotationalSpeed = 30f;
                        Projectile.localAI[0] += (float)Math.PI / rotationalSpeed;
                        if (Projectile.localAI[0] % 30 == 0)
                        {
                            int numProj2 = Main.masterMode ? 10 : 6;
                            for (int i = 0; i < numProj2; i++)
                            {
                                float projSpeed = 5f;
                                Vector2 vel = Vector2.UnitX.RotatedBy(Projectile.localAI[0] + (Math.PI * 2 / numProj2 * i)) * projSpeed;
                                ExlightUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<IchorShot>(), 60, 0f);
                            }
                        }
                    }
                } 
            }

            if (Projectile.localAI[0] > 370)
            {
                Projectile.Kill();
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
