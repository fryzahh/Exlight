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
    public class EstriteDrone : ModNPC
    {
        bool DidSpawnAnimation = false;
        bool IsDead = false;

        public enum EmpressAttacks
        {
            SpawnAnimation = -2,
            DeathAnimation = -1,
            Dashing,
            ShootingAtDistance
        }

        public EmpressAttacks CurrentAttack
        {
            get => (EmpressAttacks)(int)(NPC.ai[0]);
            set => NPC.ai[0] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Estrite Droid");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
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
                new FlavorTextBestiaryInfoElement("A survey drone of unknown origins. It roams the lands and attacks anything it deems a threat. Rumours have said to have seen groups of these strange droids emerging from caves or even small mechanical laboratory-like openings in the ground, but these still are unconfirmed even to this day.")
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

        public override bool CheckDead()
        {
            if (!IsDead)
            {
                IsDead = true;
                NPC.ai[1] = 0f;
                NPC.life = 1;
                NPC.dontTakeDamage = true;
                return false;
            }
            return true;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            NPC.rotation = NPC.velocity.X * 0.08f;
            NPC.spriteDirection = NPC.direction;

            if (!DidSpawnAnimation)
            {
                NPC.ai[0] = -2f;
            }

            if (IsDead)
            {
                NPC.ai[0] = -1f;
            }

            if (NPC.ai[2] > 0)
            {
                NPC.localAI[0] += 0.1f;
                if (NPC.localAI[0] > 1f)
                {
                    NPC.localAI[0] = 1f;
                }
            }

            switch ((int)NPC.ai[0])
            {
                case -2:
                    DoSpawnAnimation();
                    break;

                case -1:
                    DoDeathAnimation();
                    break;

                case 0:
                    DoSideDashes(player);
                    break;

                case 1:
                    DoLaserShots(player);
                    break;
            }
        }

        public void DoSpawnAnimation()
        {
            NPC.ai[1]++;
            if (NPC.ai[1] == 1)
            {
                NPC.velocity.Y -= 7f;
            }

            if (NPC.ai[1] < 120)
            {
                NPC.Opacity = Utils.GetLerpValue(0f, 1f, NPC.ai[1] / 60, true);
                NPC.velocity *= 0.9f;
            }
            else
            {
                NPC.ai[1] = 0f;
                NPC.ai[0] = Utils.SelectRandom(Main.rand, 0, 1);
                DidSpawnAnimation = true;
                NPC.netUpdate = true;
            }
        }

        public void DoDeathAnimation()
        {
            NPC.rotation += NPC.velocity.X * 0.15f;
            NPC.noTileCollide = false;
            NPC.ai[1]++;
            if (NPC.ai[1] >= 180)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                NPC.netUpdate = true;
            }
            else
            {
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.3f, 0f, 16f);
                for (int i = 0; i < 1; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 5f);
                    Color color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.1f, 0.4f));
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.1f, 0.15f), Main.rand.Next(60, 71), true, false));
                }
            }
            NPC.velocity.X *= 0.98f;
        }

        public void DoSideDashes(Player player)
        {
            Vector2 pos = player.Center;
            pos.X += 300 * (NPC.Center.X < player.Center.X ? -1f : 1f);
            pos.Y -= 200;

            int time = NPC.ai[2] > 0f ? 90 : 180;
            NPC.ai[1]++;
            if (NPC.ai[1] < time)
            {
                NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 10f, 5f);
            }
            else
            {
                NPC.spriteDirection = NPC.direction;
                NPC.knockBackResist = 0f;
            }

            if (NPC.ai[1] == time)
            {
                float speed = NPC.ai[2] > 0f ? 15f : 10f;
                NPC.velocity = NPC.DirectionTo(player.Center) * speed;
            }

            if (NPC.ai[1] >= time + 60)
            {
                NPC.ai[1] = 0f;
                NPC.knockBackResist = 0.45f;
                NPC.netUpdate = true;
            }
        }

        public void DoLaserShots(Player player)
        {
            Vector2 pos = player.Center + NPC.DirectionFrom(player.Center) * 200;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 4f, 12f);

            int time = NPC.ai[2] > 0f ? 60 : 120;
            NPC.ai[1]++;
            if (NPC.ai[1] % time == 0f)
            {
                NPC.velocity = -NPC.DirectionTo(player.Center) * 10f;
                Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float speed = NPC.ai[2] > 0f ? 12f : 7f;
                    ExlightUtils.NewProjectileBetter(NPC.Center, dir * speed, ModContent.ProjectileType<EstriteDroneLaser>(), NPC.damage / 2, 0f);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //textures
            Texture2D glowTexture = ModContent.Request<Texture2D>("Exlight/NPCs/Estrite/EstriteDroneGlow").Value;
            Texture2D mainTexture = TextureAssets.Npc[NPC.type].Value;

            //drawing
            Color trailColor = Color.Lerp(Color.Teal, Color.Cyan, NPC.localAI[0]);
            DrawHelper.DrawNPCTrail(NPC.whoAmI, glowTexture, trailColor, NPC.rotation, false, animated: true, useFlipSpriteEffects: true);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, mainTexture, NPC.GetAlpha(drawColor), NPC.rotation, animated: true, useFlipSpriteEffects: true);
            return false;
        }
    }
}
