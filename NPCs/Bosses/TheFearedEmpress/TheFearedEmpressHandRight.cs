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

namespace Exlight.NPCs.Bosses.TheFearedEmpress
{
    public class TheFearedEmpressHandRight : ModNPC
    {
        float[] AttackTimer = new float[6];
        float[] VisualTimer = new float[5];      

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Righty");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
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

        public override void AI()
        {
            NPC empress = Main.npc[(int)NPC.ai[1]];
            Player player = Main.player[NPC.target];
            if (!Main.npc.IndexInRange(empress.whoAmI))
            {
                NPC.life = 0;
                NPC.checkDead();
                NPC.HitEffect();
                NPC.netUpdate = true;
            }
            else
            {
                NPC.timeLeft = 2;
            }

            TheFearedEmpress.EmpressAttacks empressAttackType = (TheFearedEmpress.EmpressAttacks)empress.ai[0];
            if (NPC.ai[2] == -1)
            {

            }
            else
            {
                if (empressAttackType == TheFearedEmpress.EmpressAttacks.BasicFlyMovement || empressAttackType == TheFearedEmpress.EmpressAttacks.SwordBarrages || empressAttackType == TheFearedEmpress.EmpressAttacks.ShootIchorRing)
                {
                    NPC.rotation = NPC.velocity.X * 0.015f;
                    Vector2 pos = empress.Center + new Vector2(65, 20);
                    NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 14);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.BasicIchorShots || empressAttackType == TheFearedEmpress.EmpressAttacks.HomingIchorShots || empressAttackType == TheFearedEmpress.EmpressAttacks.BloodBombs)
                {
                    const float PI = (float)Math.PI;
                    float targetRotation = MathHelper.WrapAngle(NPC.DirectionTo(empress.Center).ToRotation() - PI / 2);
                    NPC.rotation = MathHelper.WrapAngle(MathHelper.Lerp(NPC.rotation, targetRotation, 0.06f));
                    Vector2 pos = empress.Center + new Vector2(20, 0);
                    NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 14);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.ExplosiveIchorClones || empressAttackType == TheFearedEmpress.EmpressAttacks.SwordBarrages)
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.2f, 0f, 1f);
                    NPC.rotation = NPC.velocity.X * 0.015f;
                    Vector2 pos = empress.Center + new Vector2(65, 20);
                    NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 14);
                }
                else
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.2f, 0f, 1f);
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.DualHandSmashes)
                {
                    NPC.TargetClosest(true);
                    int time = Main.masterMode ? 120 : Main.expertMode ? 150 : 180;
                    if (NPC.ai[0] == 0)
                    {
                        NPC.ai[3]++;
                        if (NPC.ai[3] < time)
                        {
                            NPC.rotation = NPC.velocity.X * 0.015f;
                            Vector2 pos = player.Center - Vector2.UnitY * 300;
                            NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 25f, 80f);
                        }

                        if (NPC.ai[3] == time - 5)
                        {
                            NPC.noTileCollide = false;
                        }

                        if (NPC.ai[3] > time - 60 && NPC.ai[3] < time - 30)
                        {
                            NPC.velocity *= 0.95f;
                        }

                        if (NPC.ai[3] > time)
                        {
                            if (NPC.velocity.Y < 16)
                            {
                                NPC.velocity.Y += 3f;
                            }

                            if (NPC.collideY)
                            {
                                NPC.ai[0] = 1f;
                                NPC.ai[3] = 0f;
                                NPC.velocity.Y = -7;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int numProj = Main.masterMode ? 30 : Main.expertMode ? 20 : 15;
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

                    if (NPC.ai[0] == 1f)
                    {
                        int cooldown = Main.masterMode ? 30 : Main.expertMode ? 45 : 60;
                        NPC.velocity *= 0.9f;
                        NPC.ai[3]++;
                        if (NPC.ai[3] == cooldown)
                        {
                            NPC.ai[0] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.noTileCollide = true;
                            NPC.netUpdate = true;
                        }
                    }
                }
                else
                {
                    ResetVariables();
                    NPC.noTileCollide = true;
                }

                if (empressAttackType == TheFearedEmpress.EmpressAttacks.DualHandDashes)
                {
                    NPC.spriteDirection = -NPC.direction;
                    NPC.TargetClosest(true);
                    int time = Main.masterMode ? 120 : Main.expertMode ? 150 : 180;
                    if (AttackTimer[0] == 0)
                    {
                        AttackTimer[1]++;
                        if (AttackTimer[1] < time)
                        {
                            NPC.rotation = NPC.AngleTo(player.Center) - (float)Math.PI / 4f;
                            if (Main.masterMode)
                            {
                                Vector2 pos = Vector2.Zero;
                                switch ((int)AttackTimer[2])
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

                                NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 30f);
                            }
                            else
                            {
                                Vector2 pos = player.Center + Vector2.UnitX * 300;
                                NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 30f);
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
                            NPC.rotation = NPC.velocity.ToRotation() - 1.57f;
                        }

                        if (AttackTimer[1] >= 80)
                        {
                            int howManyDashes = Main.masterMode ? 6 : Main.expertMode ? 5 : 3;
                            AttackTimer[2]++;
                            if (AttackTimer[2] >= howManyDashes)
                            {
                                AttackTimer[0] = 2f;
                                AttackTimer[1] = 0f;
                                AttackTimer[2] = 0f;
                                NPC.noTileCollide = true;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AttackTimer[0] = 0f;
                                AttackTimer[1] = 0f;
                                NPC.netUpdate = true;
                            }
                        }
                    }

                    if (AttackTimer[0] == 2f)
                    {
                        ResetVariables();
                        NPC.rotation = NPC.velocity.X * 0.015f;
                        Vector2 pos = empress.Center + new Vector2(65, 20);
                        NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 14);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D IdleTexture = Mod.Assets.Request<Texture2D>("NPCs/Bosses/TheFearedEmpress/TheFearedEmpressHandRight").Value;
            DrawHelper.DrawNPCTrail(NPC.whoAmI, IdleTexture, NPC.GetAlpha(Color.Yellow * 0.45f), NPC.rotation, true, animated: true);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, IdleTexture, NPC.GetAlpha(drawColor), NPC.rotation, animated: true);
            return false;
        }
    }
}
