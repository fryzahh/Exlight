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
using Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles;
using Exlight.Particles;

namespace Exlight.NPCs.Bosses.TheFearedEmpress
{
    public class TheFearedEmpressHandRight : ModNPC
    {
        float[] AttackTimer = new float[6];
        float[] VisualTimer = new float[5];

        bool PhaseTwo;
        bool HalfHealth;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Righty");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 42;
            NPC.damage = 100;
            NPC.defense = 75;
            NPC.lifeMax = 75000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < 4; i++)
            {
                writer.Write(NPC.localAI[i]);
            }
            for (int i = 0; i < 6; i++)
            {
                writer.Write(AttackTimer[i]);
            }
            for (int i = 0; i < 5; i++)
            {
                writer.Write(VisualTimer[i]);
            }

            writer.Write(PhaseTwo);
            writer.Write(HalfHealth);
            writer.Write(NPC.rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < 4; i++)
            {
                NPC.localAI[i] = reader.ReadSingle();
            }
            for (int i = 0; i < 7; i++)
            {
                AttackTimer[i] = reader.ReadSingle();
            }
            for (int i = 0; i < 5; i++)
            {
                VisualTimer[i] = reader.ReadSingle();
            }
            PhaseTwo = reader.ReadBoolean();
            HalfHealth = reader.ReadBoolean();
            NPC.rotation = reader.ReadSingle();
        }

        public void ResetVariables()
        {
            NPC.ai[0] = 0f;
            for (int i = 0; i < 5; i++)
            {
                AttackTimer[i] = 0;
            }
            for (int i = 0; i < 5; i++)
            {
                VisualTimer[i] = 0;
            }
            for (int i = 0; i < 4; i++)
            {
                NPC.localAI[i] = 0;
            }      
            NPC.ai[3] = 0f;
            NPC.netUpdate = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            NPC empress = Main.npc[(int)NPC.ai[1]];
            Player player = Main.player[NPC.target];
            if (!Main.npc.IndexInRange(empress.whoAmI))
            {
                NPC.life = 0;
                NPC.checkDead();
                NPC.HitEffect();
                NPC.netUpdate = true;
            }

            HalfHealth = !PhaseTwo && NPC.life < NPC.lifeMax * 0.5f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC otherHand = Main.npc[i];
                if (otherHand.active && otherHand.type == ModContent.NPCType<TheFearedEmpressHandLeft>())
                {
                    PhaseTwo = NPC.life < NPC.lifeMax * 0.5f && otherHand.life < otherHand.lifeMax * 0.5f;
                }
            }

            TheFearedEmpress.EmpressAttacks empressAttackType = (TheFearedEmpress.EmpressAttacks)empress.ai[0];
            if (NPC.ai[2] == -1)
            {

            }
            else
            {
                if (empressAttackType == TheFearedEmpress.EmpressAttacks.BasicFlyMovement || empressAttackType == TheFearedEmpress.EmpressAttacks.SwordBarrages || empressAttackType == TheFearedEmpress.EmpressAttacks.ShootIchorRing)
                {
                    DoMoveToEmpressPosition(empress);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.BasicIchorShots || empressAttackType == TheFearedEmpress.EmpressAttacks.HomingIchorShots || empressAttackType == TheFearedEmpress.EmpressAttacks.BloodBombs)
                {
                    DoTurnToEmpressCenter(empress);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.ExplosiveIchorClones || empressAttackType == TheFearedEmpress.EmpressAttacks.SwordBarrages)
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.2f, 0f, 1f);
                    DoMoveToEmpressPosition(empress);
                }
                else
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.2f, 0f, 1f);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.DualHandSmashes)
                {
                    DoHandSmashes(2, player, PhaseTwo, HalfHealth);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.RightHandSmash)
                {
                    DoHandSmashes(1, player, PhaseTwo, HalfHealth);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.LeftHandSmash)
                {
                    DoExtraAttacks(2, player, false);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.DualHandDashes)
                {
                    DoHandDashes(3, player, PhaseTwo);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.RightHandDash)
                {
                    DoHandDashes(1, player, PhaseTwo);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.LeftHandDash)
                {
                    DoExtraAttacks(1, player, false);
                }
            }
        }

        public void DoMoveToEmpressPosition(NPC empress)
        {
            ResetVariables();
            NPC.rotation = NPC.velocity.X * 0.015f;
            Vector2 pos = empress.Center + new Vector2(65, 20);
            NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 14);
        }

        public void DoTurnToEmpressCenter(NPC empress)
        {
            const float PI = (float)Math.PI;
            float targetRotation = MathHelper.WrapAngle(NPC.DirectionTo(empress.Center).ToRotation() - PI / 2);
            NPC.rotation = MathHelper.WrapAngle(MathHelper.Lerp(NPC.rotation, targetRotation, 0.06f));
            Vector2 pos = empress.Center + new Vector2(20, 0);
            NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 14);
        }

        public void DoExtraAttacks(int type, Player player, bool phaseTwo)
        {
            //Left hand is doing dashes, in this case create a diagonal border around the player
            if (type == 1)
            {
                float dashDist = Main.masterMode ? 200f : Main.expertMode ? 250f : 300f;
                Vector2 pos = player.Center + NPC.DirectionFrom(player.Center) * dashDist;

                NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 1f);
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - (float)Math.PI / 4f;

                AttackTimer[1]++;
                if (AttackTimer[1] == 1f)
                {
                    NPC.Center = pos;
                    SoundEngine.PlaySound(SoundID.Item2, player.Center);
                    for (int i = 0; i < 180; i++)
                    {
                        Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2 / 180 * i) * 10f;
                        Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 1f));
                        ParticleManager.SpawnParticle(new BasicGlowParticle(NPC.Center, vel, color, Main.rand.NextFloat(0.95f, 1.51f), 300));
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double angle = MathHelper.ToRadians(30f);
                        Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                        Vector2 vel = dir.RotatedBy((i == 0) ? -angle : angle) * 8f;
                        if (AttackTimer[1] % 10 == 0)
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShot2>(), 70, 10f, ai1: player.whoAmI);
                    }
                }
            }

            //Left Hand is doing Smashes, in this case shoot projectiles at the player
            if (type == 2)
            {
                AttackTimer[1]++;
                if (AttackTimer[1] % 30 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float chargeUpSpeed = Main.masterMode ? 0.04f : Main.expertMode ? 0.03f : 0.02f;
                    Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                    ExlightUtils.NewProjectileBetter(NPC.Center, dir, ModContent.ProjectileType<LargeIchorDart>(), 100, 15f, ai1: chargeUpSpeed);
                }

                float speed = Main.masterMode ? 12f : Main.expertMode ? (phaseTwo ? 12f : 10f) : (phaseTwo ? 10f : 8f);
                Vector2 pos = player.Center + new Vector2(250, -300);
                NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, speed);
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - (float)Math.PI / 4f;
            }
        }

        public void DoHandSmashes(int type, Player player, bool phaseTwo, bool halfHealth)
        {
            if (type == 1)
            {
                int time = Main.masterMode ? 120 : Main.expertMode ? 150 : 180;
                if (AttackTimer[0] == 0)
                {
                    AttackTimer[1]++;
                    if (AttackTimer[1] < time)
                    {
                        float speed = Main.masterMode ? 10f : Main.expertMode ? (phaseTwo ? 8f : 6f) : (phaseTwo ? 7f : 4f);
                        NPC.rotation = NPC.velocity.X * 0.015f;
                        Vector2 pos = player.Center - Vector2.UnitY * 300;
                        NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, speed);
                    }

                    if (AttackTimer[1] == time - 5)
                    {
                        NPC.noTileCollide = false;
                    }

                    if (AttackTimer[1] > time - 60 && AttackTimer[1] < time - 30)
                    {
                        NPC.velocity *= 0.9f;
                    }

                    if (AttackTimer[1] > time)
                    {
                        NPC.velocity.X *= 0.9f;
                        if (NPC.velocity.Y < 16)
                        {
                            NPC.velocity.Y += 3f;
                        }

                        if (NPC.collideY)
                        {
                            AttackTimer[0] = 1f;
                            AttackTimer[1] = 0f;
                            NPC.velocity.Y = -7;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int numProj = Main.masterMode ? (halfHealth ? 45 : 30) : Main.expertMode ? (halfHealth ? 35 : 25) : (halfHealth ? 25 : 15);
                                for (int i = 0; i < numProj; i++)
                                {
                                    float speed = Main.masterMode ? 10f : Main.expertMode ? 7f : 5f;
                                    Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2 / numProj * i) * speed;
                                    ExlightUtils.NewProjectileBetter(NPC.Bottom, vel, ModContent.ProjectileType<IchorShot>(), 40, 0f);
                                }
                            }
                            NPC.netUpdate = true;
                        }
                    }
                }

                if (AttackTimer[0] == 1f)
                {
                    int cooldown = Main.masterMode ? 30 : Main.expertMode ? 45 : 60;
                    AttackTimer[1]++;
                    if (AttackTimer[1] >= cooldown)
                    {
                        int howManySmashes = Main.masterMode ? 5 : Main.expertMode ? 4 : 3;
                        AttackTimer[2]++;
                        if (AttackTimer[2] >= howManySmashes)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[0] = -1f;
                        }
                        else
                        {
                            AttackTimer[0] = 0f;
                            AttackTimer[1] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                }
            }
            
            if (type > 1)
            {
                int time = Main.masterMode ? 120 : Main.expertMode ? 150 : 180;
                if (AttackTimer[0] == 0)
                {
                    AttackTimer[1]++;
                    if (AttackTimer[1] < time)
                    {
                        NPC.rotation = NPC.velocity.X * 0.015f;
                        Vector2 pos = player.Center + new Vector2(300, -300);
                        NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 35f, 50f);
                    }

                    if (AttackTimer[1] == time - 5)
                    {
                        NPC.noTileCollide = false;
                    }

                    if (AttackTimer[1] > time - 60 && AttackTimer[1] < time - 30)
                    {
                        NPC.velocity *= 0.9f;
                    }

                    if (AttackTimer[1] > time)
                    {
                        NPC.velocity.X *= 0.9f;
                        if (NPC.velocity.Y < 16)
                        {
                            NPC.velocity.Y += 3f;
                        }

                        if (NPC.collideY)
                        {
                            AttackTimer[0] = 1f;
                            AttackTimer[1] = 0f;
                            NPC.velocity.Y = -7;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (phaseTwo)
                                {
                                    int numProj = Main.masterMode ? 12 : Main.expertMode ? 14 : 16;
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        float ai1 = Main.masterMode ? 0.03f : Main.expertMode ? 0.02f : 0.01f;
                                        Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                                        Vector2 vel = dir.RotatedBy(Math.PI * 2 / numProj * i) * 1f;
                                        ExlightUtils.NewProjectileBetter(NPC.Bottom, vel, ModContent.ProjectileType<LargeIchorDart>(), 40, 0f, ai1: ai1);
                                    }
                                }

                                if (halfHealth)
                                {
                                    int numProj = Main.masterMode ? 25 : Main.expertMode ? 20 : 15;
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        float speed = 6f;
                                        Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2 / numProj * i) * speed;
                                        ExlightUtils.NewProjectileBetter(NPC.Bottom, vel, ModContent.ProjectileType<IchorShot>(), 40, 0f);
                                    }
                                }


                            }
                            NPC.netUpdate = true;
                        }
                    }
                }

                if (AttackTimer[0] == 1f)
                {
                    NPC.velocity *= 0.9f;
                    int cooldown = Main.masterMode ? 30 : Main.expertMode ? 45 : 60;
                    AttackTimer[1]++;
                    if (AttackTimer[1] >= cooldown)
                    {
                        int howManySmashes = Main.masterMode ? 5 : Main.expertMode ? 4 : 3;
                        AttackTimer[2]++;
                        if (AttackTimer[2] >= howManySmashes)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[0] = -1f;
                        }
                        else
                        {
                            AttackTimer[0] = 0f;
                            AttackTimer[1] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                }
            }
        }

        public void DoHandDashes(int type, Player player, bool phaseTwo)
        {
            //singular
            if (type == 1)
            {
                if (AttackTimer[0] == 0f)
                {
                    int time = Main.masterMode ? (phaseTwo ? 30 : 60) : Main.expertMode ? (phaseTwo ? 45 : 60) : (phaseTwo ? 60 : 120);
                    AttackTimer[1]++;
                    if (AttackTimer[1] >= time)
                    {
                        AttackTimer[0] = 1f;
                        AttackTimer[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        float dashDist = Main.masterMode ? 200f : Main.expertMode ? 250f : 300f;
                        Vector2 pos = player.Center + Vector2.UnitX * dashDist;
                        NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 25f);
                    }
                }

                if (AttackTimer[0] == 1f)
                {
                    AttackTimer[1]++;
                    if (AttackTimer[1] == 1f)
                    {
                        float dashSpeed = Main.masterMode ? 24f : Main.expertMode ? 20f : 18f;
                        NPC.velocity = NPC.DirectionTo(player.Center) * dashSpeed;
                        NPC.netUpdate = true;
                    }

                    if (AttackTimer[1] >= 80)
                    {
                        NPC.velocity *= 0.9f;
                    }
                    else
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - (float)Math.PI / 2f;
                        //maybe spawn some shit idk
                    }

                    if (AttackTimer[1] >= 120)
                    {
                        int howManyDashes = Main.masterMode ? 8 : Main.expertMode ? 6 : 4;
                        if (++AttackTimer[2] >= howManyDashes)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[0] = -1f;
                        }
                        else
                        {
                            AttackTimer[0] = 0f;
                            AttackTimer[1] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                }
            }

            //double
            if (type > 1)
            {
                if (phaseTwo)
                {
                    if (AttackTimer[0] == 0)
                    {
                        AttackTimer[1]++;
                        if (AttackTimer[1] < 120)
                        {
                            Vector2 targetPos = player.Center;
                            targetPos.X += 600;
                            targetPos.Y += 300 * (NPC.Center.Y < targetPos.Y ? -1 : 1);
                            NPC.velocity = Vector2.Zero.MoveTowards(targetPos - NPC.Center, 24f);
                        }
                        else
                        {
                            AttackTimer[0] = 1f;
                            AttackTimer[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }

                    if (AttackTimer[0] == 1f)
                    {
                        float dashSpeed = Main.masterMode ? 28f : Main.expertMode ? 24f : 20f;
                        NPC.velocity = NPC.DirectionTo(player.Center) * dashSpeed;
                        AttackTimer[0] = 2f;

                        if (type == 3)
                        {
                            int numProj = Main.masterMode ? 16 : Main.expertMode ? 12 : 8;
                            for (int i = 0; i < numProj; i++)
                            {
                                float speed = Main.expertMode ? 10f : 8f;
                                Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                                Vector2 vel = dir.RotatedBy(Math.PI * 2 / numProj * i) * speed;
                                /*if (Main.netMode != NetmodeID.MultiplayerClient)
                                    InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<GemShot>(), NPC.damage / 2, 0f);*/
                            }
                        }
                        NPC.netUpdate = true;
                    }

                    if (AttackTimer[0] == 2f)
                    {
                        AttackTimer[1]++;
                        if (AttackTimer[1] >= 60)
                        {
                            NPC.velocity *= 0.9f;
                        }
                        else
                        {
                            NPC.rotation = NPC.velocity.X * 0.02f;
                            if (type == 2)
                            {
                                if (AttackTimer[1] % 6 == 0)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        float speed = (i == 0) ? 2f : -2f;
                                        Vector2 vel = Vector2.UnitY * speed;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            //InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<PrismaticBolt2>(), NPC.damage / 2, 0f, ai1: 0.025f);
                                        }
                                    }
                                }
                            }
                        }

                        if (Main.masterMode)
                        {
                            if (AttackTimer[1] == 60f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient) //chain explosions
                                {
                                    Vector2 baseDirection = NPC.DirectionTo(player.Center);
                                    const int max = 6;
                                    for (int i = 0; i < max; i++)
                                    {
                                        Vector2 offset = NPC.height / 2 * baseDirection.RotatedBy(Math.PI * 2 / max * i);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), Vector2.Zero, ModContent.ProjectileType<IchorChainExplosion>(),
                                            60, 0f, Main.myPlayer, MathHelper.WrapAngle(offset.ToRotation()), 32);
                                    }
                                }
                            }
                        }

                        if (AttackTimer[1] == 1f || AttackTimer[1] == 60f)
                        {
                            int numProj = Main.masterMode ? 16 : Main.expertMode ? 14 : 12;
                            for (int i = 0; i < numProj; i++)
                            {
                                float speed = 6f;
                                Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2 / numProj * i) * speed;
                                ExlightUtils.NewProjectileBetter(NPC.Bottom, vel, ModContent.ProjectileType<IchorShot>(), 40, 0f);
                            }
                        }

                        int time2 = Main.masterMode ? 100 : Main.expertMode ? 120 : 150;
                        if (AttackTimer[1] >= time2)
                        {
                            int howManyDashes = Main.masterMode ? 8 : Main.expertMode ? 6 : 4;
                            if (++AttackTimer[2] >= howManyDashes)
                            {
                                NPC.netUpdate = true;
                                NPC.ai[0] = -1f;
                            }
                            else
                            {
                                AttackTimer[0] = 0f;
                                AttackTimer[1] = 0f;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }
                else
                {
                    int time = Main.masterMode ? 45 : Main.expertMode ? 60 : 100;
                    if (AttackTimer[0] == 0)
                    {
                        AttackTimer[1]++;
                        if (AttackTimer[1] < time)
                        {
                            NPC.rotation = NPC.AngleTo(player.Center) - (float)Math.PI / 2f;
                            if (Main.masterMode)
                            {
                                Vector2 pos = Vector2.Zero;
                                switch ((int)NPC.localAI[0])
                                {
                                    case 0:
                                        pos = player.Center - Vector2.UnitX * 250f;
                                        break;

                                    case 1:
                                        pos = player.Center + Vector2.UnitX * 250f;
                                        break;

                                    case 2:
                                        pos = player.Center - Vector2.UnitY * 250f;
                                        break;

                                    case 3:
                                        pos = player.Center + Vector2.UnitY * 250f;
                                        break;

                                    case 4:
                                        pos = player.Center + new Vector2(-250f, 250f);
                                        break;

                                    case 5:
                                        pos = player.Center + new Vector2(250f, -250f);
                                        break;
                                }

                                NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 35f);
                            }
                            else
                            {
                                Vector2 pos = player.Center + Vector2.UnitX * 300;
                                NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 35f, 40f);
                            }
                        }

                        if (AttackTimer[1] == time)
                        {
                            AttackTimer[0] = 1f;
                            AttackTimer[1] = 0f;
                            float dashSpeed = Main.masterMode ? 25f : Main.expertMode ? 20f : 15f;
                            NPC.velocity = NPC.DirectionTo(player.Center) * dashSpeed;
                        }
                    }

                    if (AttackTimer[0] == 1f)
                    {
                        AttackTimer[1]++;
                        if (AttackTimer[1] >= 30)
                        {
                            NPC.velocity *= 0.9f;
                        }
                        else
                        {
                            NPC.rotation = NPC.velocity.ToRotation() - (float)Math.PI / 4;
                        }

                        if (AttackTimer[1] >= 80)
                        {
                            if (Main.masterMode)
                            {
                                NPC.localAI[0]++;
                            }

                            int howManyDashes = Main.masterMode ? 6 : Main.expertMode ? 6 : 4;
                            if (++AttackTimer[2] >= howManyDashes)
                            {
                                NPC.netUpdate = true;
                                NPC.ai[0] = -1f;
                            }
                            else
                            {
                                AttackTimer[0] = 0f;
                                AttackTimer[1] = 0f;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D IdleTexture = Mod.Assets.Request<Texture2D>("NPCs/Bosses/TheFearedEmpress/TheFearedEmpressHandRight").Value;
            DrawHelper.DrawNPCTrail(NPC.whoAmI, IdleTexture, NPC.GetAlpha(drawColor * 0.35f), NPC.rotation, true, animated: true);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, IdleTexture, NPC.GetAlpha(drawColor), NPC.rotation, animated: true);
            return false;
        }
    }
}
