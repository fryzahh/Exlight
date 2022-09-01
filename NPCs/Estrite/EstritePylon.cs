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
using Exlight.NPCs.Estrite.Projectiles;
using Terraria.Audio;
using Exlight.Common.Systems.ParticleSystem;
using Terraria.GameContent.Drawing;
using Exlight.Particles;
using System.Linq;

namespace Exlight.NPCs.Estrite
{
    public class EstritePylon : ModNPC
    {
        bool HasSpawnedTurrets = false;
        bool TurretsDead;

        List<int> SuperchargeableEnemies = new List<int>
        {
            ModContent.NPCType<EstriteDrone>()       
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Estrite Gimbot");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 38;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(silver: Main.rand.Next(4, 11), copper: Main.rand.Next(10, 51));
            NPC.npcSlots = 1f;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[3]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("A pylon of some sorts which supports the various Estrite machines. It can summon a set of Estrite Turrets to defend itself in case of danger.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            if (!HasSpawnedTurrets)
            {
                HasSpawnedTurrets = true;
                int numProj = Main.masterMode ? Main.rand.Next(4, 7) : Main.expertMode ? Main.rand.Next(3, 6) : Main.rand.Next(2, 5);
                for (int i = 0; i < numProj; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(NPC.width * 2, NPC.height * 2);
                    ExlightUtils.NewNPCBetter(spawnPos, ModContent.NPCType<EstriteTurret>(), 0, NPC.whoAmI, 0, Main.rand.Next(45, 76));
                }
            }

            if (HasSpawnedTurrets)
            {
                if (NPC.CountNPCS(ModContent.NPCType<EstriteTurret>()) < 1)
                {
                    TurretsDead = true;
                }
            }

            if (TurretsDead)
            {
                //aura dust
                for (int i = 0; i < 10; i++)
                {
                    Vector2 offset = new Vector2();
                    double circleAngle = Main.rand.NextDouble() * Math.PI * 2d;
                    offset.X += (float)Math.Sin(circleAngle) * 400f;
                    offset.Y += (float)Math.Cos(circleAngle) * 400f;
                    Dust dust = Dust.NewDustPerfect(NPC.Center + offset, DustID.Electric);
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -4f;
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                }

                //supercharge the enemies
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.Distance(NPC.Center) <= 400 && SuperchargeableEnemies.Contains(npc.type))
                    {
                        npc.ai[2] = 1f;
                        npc.netUpdate = true;
                    }
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 45; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(6f, 10f) * hitDirection;
                    Color color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.1f, 0.4f));
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.1f, 0.15f), Main.rand.Next(60, 71), true, false));
                }

                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(4f, 8f) * hitDirection;
                    ParticleManager.SpawnParticle(new BasicSmokeParticle(spawnPos, vel / 2, Color.Orange, Color.Black, Main.rand.NextFloat(0.65f, 1f), Main.rand.NextFloat(130, 150), Main.rand.NextFloat(0.02f, 0.05f), true));
                }
            }
            else
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(2f, 6f) * hitDirection;
                    Color color = Color.Lerp(Color.Teal, Color.LimeGreen, Main.rand.NextFloat(0.1f, 0.4f));
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.1f, 0.15f), Main.rand.Next(30, 61)));
                }
            }
        }
    }
}
