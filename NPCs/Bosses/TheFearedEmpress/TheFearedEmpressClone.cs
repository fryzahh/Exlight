using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Exlight.Common.Utilities;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Exlight.Common.Systems.ParticleSystem;
using Exlight.Particles;
using Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles;

namespace Exlight.NPCs.Bosses.TheFearedEmpress
{
    public class TheFearedEmpressClone : ModNPC
    {
        public ref float DeathTimer => ref NPC.ai[2];
        public ref float VisualTimer => ref NPC.ai[3];

        public override string Texture => "Exlight/NPCs/Bosses/TheFearedEmpress/TheFearedEmpressGlow";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Feared Empress");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 334;
            NPC.damage = 120;
            NPC.defense = 10;
            NPC.lifeMax = 120000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0f;
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.Opacity = 0;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) { return false; }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void AI()
        {
            NPC empress = Main.npc[(int)NPC.ai[0]];
            Player player = Main.player[empress.target];
            NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.3f, 0f, 1f);
            NPC.velocity *= 0.98f;
            if (!Main.npc.IndexInRange(empress.whoAmI))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[1] = 1f;
                    NPC.checkDead();
                    NPC.HitEffect();
                    NPC.netUpdate = true;
                }
            }         

            if (NPC.ai[1] == 0f)
            {
                NPC.velocity = Vector2.Zero.MoveTowards((player.Center + Vector2.UnitX * 250) - NPC.Center, 20f);
                DeathTimer++;
                if (DeathTimer >= 180 || NPC.justHit)
                {
                    NPC.ai[1] = 2f;
                    DeathTimer = 0;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                }

                if (DeathTimer < 180 && empress.ai[1] == 2f)
                {
                    NPC.ai[1] = 1f;
                    DeathTimer = 0;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                }
            }

            //Killed by the player/Player stopped Empress, in this case don't explode into Ichor
            if (NPC.ai[1] == 1f)
            {
                DeathTimer++;
                if (DeathTimer >= 180)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.netUpdate = true;
                }
                else
                {
                    //No telegraph, just particles
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 val5 = NPC.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(50, 300), Main.rand.NextFloat(50, 300));
                        Vector2 v = val5 - NPC.Center;
                        v = v.SafeNormalize(Vector2.Zero) * -8f;
                        Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                        ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.15f, 0.35f), Main.rand.Next(60, 70), false, true));
                    }
                }
            }
            
            //Wasn't killed by the player, explode into Ichor
            if (NPC.ai[1] == 2f)
            {
                DeathTimer++;
                if (DeathTimer >= 180)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.netUpdate = true;
                }
                else
                {
                    //Increase VisualTimer to show the explosion telegraph
                    VisualTimer += 0.3f;
                    if (VisualTimer > 1f)
                    {
                        VisualTimer = 1f;
                    }

                    //Charge up Particles
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 val5 = NPC.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(50, 300), Main.rand.NextFloat(50, 300));
                        Vector2 v = val5 - NPC.Center;
                        v = v.SafeNormalize(Vector2.Zero) * -8f;
                        Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                        ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.15f, 0.35f), Main.rand.Next(60, 70), false, true));
                    }
                }
            }
        }

       
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (NPC.ai[1] == 2f)
                {
                    int numProj = Main.masterMode ? 50 : Main.expertMode ? 40 : 30;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2 / numProj * i) * 10f;
                        ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShot>(), 60, 0f);
                    }
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                    Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(12f, 14f);
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(1f, 2f), Main.rand.Next(180, 200), false, true));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                    Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(7f, 14f);
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.75f, 1.5f), Main.rand.Next(100, 120), false, true));
                }
            }
        }
    }
}
