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
    public class EstriteTurret : ModNPC
    {
        float customFrame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Estrite Survey Turret");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 20;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0.45f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(silver: Main.rand.Next(4, 11), copper: Main.rand.Next(10, 51));
            NPC.npcSlots = 1f;
            NPC.aiStyle = -1;
            NPC.Opacity = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[3]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("A small machine created for defending its kind. They are quite weak alone but can leave dangerous results when fighting in groups.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (int)customFrame * frameHeight;
        }

        public void UpdateFrame(float frameSpeed, int minFrame, int maxFrame)
        {
            customFrame += frameSpeed;
            if (customFrame < minFrame)
            {
                customFrame = minFrame;
            }

            if (customFrame > maxFrame)
            {
                customFrame = maxFrame;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            NPC npc = Main.npc[(int)NPC.ai[0]];

            Vector2 pos = npc.Center + NPC.DirectionFrom(npc.Center) * 100;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 4f, 12f);

            NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.3f, 0f, 1f);
            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();

            NPC.ai[1]++;
            if (NPC.ai[1] < NPC.ai[2])
            {
                UpdateFrame(0.3f, 0, 4);
            }

            if (NPC.ai[1] % NPC.ai[2] == 0)
            {
                NPC.velocity = -NPC.DirectionTo(player.Center) * 10f;
                Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    ExlightUtils.NewProjectileBetter(NPC.Center, dir * 9f, ModContent.ProjectileType<EstriteDroneLaser>(), NPC.damage / 2, 0f);
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
