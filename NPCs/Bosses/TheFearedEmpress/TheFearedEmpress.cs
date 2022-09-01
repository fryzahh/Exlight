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
using Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles;
using Terraria.Audio;
using Exlight.Common.Systems.ParticleSystem;
using Terraria.GameContent.Drawing;
using Exlight.Particles;
using System.Linq;

namespace Exlight.NPCs.Bosses.TheFearedEmpress
{
    public class TheFearedEmpress : ModNPC
    {
        public bool PhaseTwo;
        public bool Enraged;

        float[] AttackTimer = new float[6];
        float[] VisualTimer = new float[5];
        float[] ExtraTimer = new float[4];

        bool SpawnIchorRing = false;
        bool HasSpawnedArmsYet = false;
        bool HasDonePhaseTransition = false;
        bool HasDoneDeathAnimation = false;
        bool DrawIllusions = false;
        bool canHitPlayer;
        bool drawHealthBar;

        public enum EmpressAttacks
        {
            PhaseTransition = -2,
            DeathAnimation = -1,
            BasicFlyMovement,
            BasicIchorShots,
            HomingIchorShots,
            ShootIchorRing,
            BloodBombs,
            ExplosiveIchorClones,
            DualHandSmashes,
            DualHandDashes,
            LeftHandSmash,
            LeftHandDash,
            RightHandSmash,
            RightHandDash,
            DualSwordSlashes,
            RightHandSwordSlash,
            LeftHandSwordSlash,
            SwordBarrages,
            GiantIchorBall,
            HandBladeSummons,
            IchorBulletRain,
            TeleportAndSpawnBalls,
            IchorMeteorShower,
            TrueCrimeras,
            SwordGrids,
            TeleportAndLeaveSword
        }

        public EmpressAttacks CurrentAttack
        {
            get => (EmpressAttacks)(int)(NPC.ai[0]);
            set => NPC.ai[0] = (int)value;
        }

        public EmpressAttacks[] PhaseOneAttackPattern = new EmpressAttacks[]
        {
            EmpressAttacks.BasicFlyMovement,
            EmpressAttacks.BasicIchorShots,
            EmpressAttacks.ShootIchorRing,
            EmpressAttacks.BasicFlyMovement,
            EmpressAttacks.ExplosiveIchorClones,
            EmpressAttacks.SwordBarrages
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Feared Empress");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 334;
            NPC.damage = 120;
            NPC.defense = 60;
            NPC.lifeMax = 120000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 50);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
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
            for (int i = 0; i < 4; i++)
            {
                writer.Write(ExtraTimer[i]);
            }
            writer.Write(SpawnIchorRing);
            writer.Write(HasSpawnedArmsYet);
            writer.Write(HasDonePhaseTransition);
            writer.Write(HasDoneDeathAnimation);
            writer.Write(DrawIllusions);
            writer.Write(PhaseTwo);
            writer.Write(Enraged);
            writer.Write(canHitPlayer);
            writer.Write(drawHealthBar);
            writer.Write(NPC.dontTakeDamage);
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
            for (int i = 0; i < 4; i++)
            {
                ExtraTimer[i] = reader.ReadSingle();
            }
            SpawnIchorRing = reader.ReadBoolean();
            HasSpawnedArmsYet = reader.ReadBoolean();
            HasDonePhaseTransition = reader.ReadBoolean();
            HasDoneDeathAnimation = reader.ReadBoolean();
            DrawIllusions = reader.ReadBoolean();
            PhaseTwo = reader.ReadBoolean();
            Enraged = reader.ReadBoolean();
            canHitPlayer = reader.ReadBoolean();
            drawHealthBar = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) { return canHitPlayer; }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) { return drawHealthBar; }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            if (!HasSpawnedArmsYet)
            {
                int lefty = ExlightUtils.NewNPCBetter(NPC.Center, ModContent.NPCType<TheFearedEmpressHandLeft>(), ai1: NPC.whoAmI);
                int righty = ExlightUtils.NewNPCBetter(NPC.Center, ModContent.NPCType<TheFearedEmpressHandRight>(), ai1: NPC.whoAmI);
                HasSpawnedArmsYet = true;
            }
            else
            {
                PhaseTwo = NPC.CountNPCS(ModContent.NPCType<TheFearedEmpressHandLeft>()) < 1 && ModContent.NPCType<TheFearedEmpressHandRight>() < 1;
            }

            if (PhaseTwo)
            {
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.dontTakeDamage = true;
            }

            /*IEnumerable<Projectile> ringSpawner = ExlightUtils.IsProjectileActive(ModContent.ProjectileType<IchorRingSpawner>());
            if (ringSpawner.Count() < 1)
            {
                ExtraTimer[0]++;
                if (ExtraTimer[0] == 300 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    ExtraTimer[0] = 0f;
                    ExlightUtils.NewProjectileBetter(NPC.Center, Vector2.Zero, ModContent.ProjectileType<IchorRingSpawner>(), 100, 0f, ai0: NPC.whoAmI);
                    NPC.netUpdate = true;
                }
            }*/

            switch (CurrentAttack)
            {                
                case EmpressAttacks.BasicFlyMovement:
                    DoBasicFlyMovement(player);
                    break;

                case EmpressAttacks.BasicIchorShots:
                    DoBasicIchorShots(player, PhaseTwo);
                    break;

                case EmpressAttacks.HomingIchorShots:
                    DoHomingIchorShots(player, PhaseTwo);
                    break;
  
                case EmpressAttacks.ShootIchorRing:
                    DoShootIchorRing(player);
                    break;

                case EmpressAttacks.BloodBombs:
                    DoBloodBombs(player, PhaseTwo, false);
                    break;


                case EmpressAttacks.ExplosiveIchorClones:
                    DoExplosiveIchorClones(player, PhaseTwo, false);
                    break;

                case EmpressAttacks.DualHandSmashes:
                case EmpressAttacks.DualHandDashes:
                case EmpressAttacks.LeftHandSmash:
                case EmpressAttacks.LeftHandDash:
                case EmpressAttacks.RightHandSmash:
                case EmpressAttacks.RightHandDash:
                    DoHandAttacks(player, false);
                    break;

                case EmpressAttacks.SwordBarrages:
                    DoSwordCircles(player, PhaseTwo, false);
                    break;
            }
        }

        public void SelectNextAttack(params int[] attack)
        {
            NPC.ai[0] = Utils.SelectRandom(Main.rand, attack);
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
            for (int i = 1; i < 4; i++)
            {
                ExtraTimer[i] = 0;
            }
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.netUpdate = true;
            NPC.dontTakeDamage = false;
            drawHealthBar = true;
        }

        public void DoBasicFlyMovement(Player player)
        {
            float speed = Main.masterMode ? 8f : Main.expertMode ? 7f : 6f;
            float turnResist = Main.masterMode ? 45f : Main.expertMode ? 30f : 25f;
            Vector2 target = player.Center - Vector2.UnitY * 200;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, target, speed, turnResist);
            if (++AttackTimer[0] >= 180)
            {
                SelectNextAttack(1, 2, 4, 5, 6, 7, 8, 9, 10, 11, 15);
            }
        }

        public void DoBasicIchorShots(Player player, bool phaseTwo)
        {
            AttackTimer[0]++;
            float speed = Main.masterMode ? 5f : Main.expertMode ? 4f : 3f;
            float turnResist = Main.masterMode ? 55f : Main.expertMode ? 40f : 30f;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, player.Center, speed, turnResist);

            int time = Main.masterMode ? (phaseTwo ? 60 : 45) : (phaseTwo ? (Main.expertMode ? 7 : 5) : 10);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.masterMode)
                {
                    int numProj = phaseTwo ? 16 : 8;
                    if (AttackTimer[0] % time == 0)
                    {
                        for (int i = 0; i < numProj; i++)
                        {
                            float projSpeed = PhaseTwo ? 5f : 10f;
                            Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                            Vector2 vel = dir.RotatedBy(Math.PI * 2 / numProj * i) * projSpeed;
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShot>(), 70, 0f);
                        }
                    }

                    float rotationalSpeed = phaseTwo ? 30f : 45f;
                    NPC.localAI[0] += (float)Math.PI / rotationalSpeed;
                    if (AttackTimer[0] % 5 == 0)
                    {
                        int numProj2 = phaseTwo ? 4 : 2;
                        for (int i = 0; i < numProj2; i++)
                        {
                            float projSpeed = PhaseTwo ? 5f : 10f;
                            Vector2 vel = Vector2.UnitX.RotatedBy(NPC.localAI[0] + (Math.PI * 2 / numProj2 * i)) * projSpeed;
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShot>(), 60, 0f);
                        }
                    }
                }
                else
                {
                    float rotationalSpeed = phaseTwo ? 30f : 45f;
                    NPC.localAI[0] += (float)Math.PI / rotationalSpeed;
                    if (AttackTimer[0] % time == 0)
                    {
                        int numProj = Main.expertMode ? (phaseTwo ? 8 : 6) : (phaseTwo ? 6 : 4);
                        for (int i = 0; i < numProj; i++)
                        {
                            float projSpeed = phaseTwo ? 5f : 10f;
                            Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                            Vector2 vel = dir.RotatedBy(NPC.localAI[0] + (Math.PI * 2 / numProj * i)) * projSpeed;
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShot>(), 60, 0f);
                        }
                    }
                }
            }

            for (int k = 0; k < 4; k++)
            {
                Vector2 val5 = NPC.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(50, 300), Main.rand.NextFloat(50, 300));
                Vector2 v = val5 - NPC.Center;
                v = v.SafeNormalize(Vector2.Zero) * -8f;
                Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.25f, 0.66f), Main.rand.Next(60, 70), false, true));
            }

            if (AttackTimer[0] >= 240)
            {
                SelectNextAttack(0);
            }
        }

        public void DoHomingIchorShots(Player player, bool phaseTwo)
        {
            AttackTimer[0]++;
            float speed = Main.masterMode ? 4f : Main.expertMode ? 3f : 2f;
            float turnResist = Main.masterMode ? 55f : Main.expertMode ? 40f : 30f;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, player.Center, speed, turnResist);

            int time = Main.masterMode ? (phaseTwo ? 60 : 45) : (phaseTwo ? (Main.expertMode ? 75 : 60) : 45);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.masterMode)
                {
                    if (AttackTimer[0] % time == 0)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            float projSpeed = PhaseTwo ? 5f : 10f;
                            Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                            Vector2 vel = dir.RotatedBy(Math.PI * 2 / 8 * i) * projSpeed;
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShotHoming>(), 50, 0f);
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            float projSpeed = PhaseTwo ? 10f : 15f;
                            Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                            Vector2 vel = dir.RotatedBy(Math.PI * 2 / 8 * i) * projSpeed;
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShotHoming>(), 50, 0f);
                        }
                    }
                }
                else
                {
                    if (AttackTimer[0] % time == 0)
                    {
                        int numProj = Main.expertMode ? (phaseTwo ? 24 : 16) : (phaseTwo ? 16 : 8);
                        for (int i = 0; i < numProj; i++)
                        {
                            float projSpeed = phaseTwo ? 5f : 10f;
                            Vector2 dir = Vector2.Normalize(player.Center - NPC.Center);
                            Vector2 vel = dir.RotatedBy(Math.PI * 2 / numProj * i) * projSpeed;
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<IchorShotHoming>(), 60, 0f);
                        }
                    }
                }
            }
            if (AttackTimer[0] >= 240)
            {
                SelectNextAttack(0);
            }
        }

        public void DoShootIchorRing(Player player)
        {
            AttackTimer[0]++;
            NPC.velocity *= 0.9f;
            if (AttackTimer[0] == 180)
            {
                NPC.velocity = -NPC.DirectionTo(player.Center) * 15f;
                NPC.netUpdate = true;
            }

            if (AttackTimer[0] >= 420)
            {
                SelectNextAttack(0);
            }
            
            if (AttackTimer[0] < 360)
            {
                VisualTimer[1] = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(0f, 60f, AttackTimer[0], true));
            }
            else
            {
                VisualTimer[1] = MathHelper.Lerp(1f, 0f, Utils.GetLerpValue(0f, 60f, AttackTimer[0] - 300f, true));
            }
        }

        public void DoExplosiveIchorClones(Player player, bool phaseTwo, bool enraged)
        {
            drawHealthBar = false;
            Vector2 moveTo = player.Center - Vector2.UnitY * 250;
            AttackTimer[0]++;
            if (NPC.ai[1] == 0f)
            {
                if (AttackTimer[0] > 60)
                {
                    //rapidly move towards the player then sequence the side pick after
                    NPC.velocity *= 0.9f;
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.1f, 0f, 1f);
                    VisualTimer[0] = MathHelper.Clamp(VisualTimer[0] + 0.1f, 0f, 1f);
                    VisualTimer[2] = MathHelper.Clamp(VisualTimer[2] + 0.1f, 0f, 1f);
                }
                else
                {
                    NPC.velocity = Vector2.Zero.MoveTowards(moveTo - NPC.Center, 20f);
                }

                if (AttackTimer[0] == 60)
                {
                    //spawn the particle to indicate which side she'll be on
                }

                if (AttackTimer[0] == 120)
                {
                    //switch AI states
                    NPC.ai[1] = 1f;
                    AttackTimer[0] = 0f;
                    NPC.netUpdate = true;
                } 
            }

            if (NPC.ai[1] == 1f)
            {
                //basic movement phase
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.3f, 0f, 1f);
                Vector2 spawnPos = player.Center + Vector2.UnitX * 250;
                Vector2 moveTo2 = player.Center - Vector2.UnitX * 250;
                NPC.velocity = Vector2.Zero.MoveTowards(moveTo2 - NPC.Center, 20f);
                if (AttackTimer[0] == 1)
                {
                    //Spawn Clone and teleport 
                    int num = ExlightUtils.NewNPCBetter(spawnPos, ModContent.NPCType<TheFearedEmpressClone>(), ai0: NPC.whoAmI);
                    if (Main.npc.IndexInRange(num))
                    {
                        Main.npc[num].Center = spawnPos;
                    }
                    NPC.Center = moveTo2;
                }
                if (NPC.justHit && AttackTimer[0] < 180)
                {
                    NPC.ai[1] = 2f;
                    NPC.netUpdate = true;
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.type == ModContent.NPCType<TheFearedEmpressClone>())
                    {
                        if (npc.justHit || npc.ai[2] >= 180)
                        {
                            NPC.ai[1] = 3f;
                            NPC.netUpdate = true;
                        }
                    }
                }
            }

            if (NPC.ai[1] == 2f)
            {
                //player found the real empress
                NPC.velocity *= 0.9f;
                NPC.dontTakeDamage = true;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                    Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                    Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(7f, 14f);
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.75f, 1.5f), Main.rand.Next(100, 120), false, true));
                }
            }

            if (NPC.ai[1] == 3f)
            {
                //player found the wrong empress
                //they die
                NPC.velocity *= 0.9f;
                NPC.dontTakeDamage = true;
                if (AttackTimer[0] < 360)
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.3f, 0f, 1f);
            }

            if (AttackTimer[0] >= 360)
            {
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.1f, 0f, 1f);
                VisualTimer[0] = MathHelper.Clamp(VisualTimer[0] - 0.3f, 0f, 1f);
                VisualTimer[2] = MathHelper.Clamp(VisualTimer[2] - 0.3f, 0f, 1f);
            }

            if (AttackTimer[0] >= 420)
            {
                SelectNextAttack(0);
            }
        }

        public void DoSwordCircles(Player player, bool phaseTwo, bool enraged)
        {          
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                drawHealthBar = false;
                NPC.dontTakeDamage = true;
                NPC.velocity *= 0.9f;
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.3f, 0f, 1f);
            }
            else
            {
                Vector2 pos = player.Center - Vector2.UnitY * 350f;
                NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 20f, 50f);
                int time = Main.masterMode ? 120 : 60;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (enraged)
                    {
                        AttackTimer[1]++;
                        //sword circle 1
                        if (AttackTimer[1] == 60)
                        {
                            int numProj = Main.masterMode ? 8 : 12;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 dir = player.DirectionTo(player.Center);
                                Vector2 spawnPos = player.Center + dir.RotatedBy(Math.PI * 2f) * 350f;
                                float ai1 = (float)(Math.PI * 2f / numProj * i);
                                ExlightUtils.NewProjectileBetter(spawnPos, Vector2.Zero, ModContent.ProjectileType<EmpressSword2>(), 45, 0f, default, dir.ToRotation(), ai1);
                            }
                        }

                        //sword circle 2
                        if (AttackTimer[1] == 90)
                        {
                            int numProj = Main.masterMode ? 6 : 12;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 dir = player.DirectionTo(player.Center);
                                Vector2 spawnPos = player.Center + dir.RotatedBy(Math.PI * 2f) * 350f;
                                float ai1 = (float)(Math.PI * 2f / numProj * i);
                                ExlightUtils.NewProjectileBetter(spawnPos, Vector2.Zero, ModContent.ProjectileType<EmpressSword2>(), 45, 0f, default, dir.ToRotation(), ai1);
                            }
                        }

                        if (AttackTimer[1] >= 180)
                        {
                            AttackTimer[1] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        int numProj = Main.masterMode ? 8 : Main.expertMode ? (phaseTwo ? 12 : 8) : (phaseTwo ? 8 : 6);
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 spawnPos = player.Center + Vector2.UnitX.RotatedBy(Math.PI * 2f / numProj * i) * 350f;
                            Vector2 dir = player.Center - Vector2.UnitX;
                            float ai1 = (float)(Math.PI * 2f / numProj * i);
                            if (AttackTimer[0] % time == 0)
                                ExlightUtils.NewProjectileBetter(spawnPos, dir, ModContent.ProjectileType<EmpressSword2>(), 45, 0f, default, dir.ToRotation(), ai1);
                        }
                    }
                }               
            }
            
            if (AttackTimer[0] >= 480)
            {
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.3f, 0f, 1f);
            }

            if (AttackTimer[0] >= 520)
            {
                SelectNextAttack(0);
            }
        }

        public void DoBloodBombs(Player player, bool phaseTwo, bool enraged)
        {
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
                if (AttackTimer[0] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 pos = i != 1 ? new Vector2(1000, 500) : new Vector2(-1000, -500);
                        ExlightUtils.NewProjectileBetter(NPC.Center + pos, Vector2.Zero, ModContent.ProjectileType<IchorArenaOrb>(), 0, 0f, ai0: NPC.whoAmI, ai1: i);
                    }
                }
            }
            else
            {
                for (int k = 0; k < 4; k++)
                {
                    Vector2 val5 = NPC.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(50, 300), Main.rand.NextFloat(50, 300));
                    Vector2 v = val5 - NPC.Center;
                    v = v.SafeNormalize(Vector2.Zero) * -8f;
                    Color color = Color.Lerp(Color.Red, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
                    ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.25f, 0.66f), Main.rand.Next(60, 70), false, true));
                }

                int time = enraged ? 30 : Main.masterMode ? (phaseTwo ? 30 : 45) : (phaseTwo ? 15 : 30);
                if (AttackTimer[0] % time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (enraged)
                    {
                        int numProj = Main.masterMode ? (phaseTwo ? 12 : 8) : (phaseTwo ? 8 : 4);
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2f / numProj * i) * Main.rand.NextFloat(4f, 10f);
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<BloodBomb>(), 10, 0f);
                        }

                        int numProj2 = Main.masterMode ? (phaseTwo ? 8 : 6) : (phaseTwo ? 6 : 3);
                        for (int i = 0; i < numProj2; i++)
                        {
                            Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2f / numProj2 * i) * Main.rand.NextFloat(7f, 15f);
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<BloodBomb>(), 10, 0f);
                        }
                    }
                    else
                    {
                        int numProj = Main.masterMode ? 4 : Main.expertMode ? 2 : 1;
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 vel = Vector2.UnitX.RotatedBy(Math.PI * 2f / numProj * i) * Main.rand.NextFloat(3f, 7f);
                            ExlightUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<BloodBomb>(), 10, 0f);
                        }
                    }
                }
            }

            if (AttackTimer[0] >= 360)
            {
                SelectNextAttack(0);
                ExlightUtils.KillProjectiles(ModContent.ProjectileType<IchorArenaOrb>());
            }
        }

        public void DoHandAttacks(Player player, bool shouldDecreaseOpacity)
        {
            Vector2 pos = player.Center - Vector2.UnitY * 250f;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, pos, 35f, 50f);
            if (AttackTimer[0] == 0f)
            {
                drawHealthBar = false;
                NPC.Opacity = shouldDecreaseOpacity ? MathHelper.Clamp(NPC.Opacity - 0.3f, 0f, 1f) : 1f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.ai[0] == -1 && (npc.type == ModContent.NPCType<TheFearedEmpressHandLeft>() || npc.type == ModContent.NPCType<TheFearedEmpressHandLeft>()))
                    {
                        AttackTimer[0] = 1f;
                        NPC.netUpdate = true;
                    }
                }
            }

            if (AttackTimer[0] == 1f)
            {
                drawHealthBar = true;
                AttackTimer[1]++;
                if (AttackTimer[1] >= 60)
                {
                    SelectNextAttack(0);
                }
                else
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.3f, 0f, 1f);
                }
            }
        }

        public void DoHandDashes(Player player)
        {
            
        }

        public void DoLeftHandSwordSlash(Player player)
        {
            float speed = Main.masterMode ? 5f : Main.expertMode ? 4f : 3f;
            float turnResist = Main.masterMode ? 45f : Main.expertMode ? 30f : 25f;
            Vector2 target = player.Center + Vector2.UnitX * 200;
            NPCHelper.SmoothTurnMovement(NPC.whoAmI, target, speed, turnResist);
            if (++AttackTimer[0] >= 200)
            {
                SelectNextAttack(0);
            }
        }

        public void DoSwordBarrages(Player player, bool phaseTwo, bool enraged)
        {
            float num68 = 20f;
            float num69 = 60f;
            float num70 = num69 * 4f;
            if (Main.expertMode)
            {
                num68 = 60f;
                num70 = num69 * 6f;
            }
            Vector2 pos = player.Center - Vector2.UnitY * 350f;
            NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, 15f);

            if ((float)(int)NPC.ai[1] % num69 == 0f && NPC.ai[1] < num70)
            {
                SoundEngine.PlaySound(SoundID.Item162, NPC.Center);
                Main.rand.NextFloat();
                int num71 = (int)NPC.ai[1] / (int)num69;
                float num72 = 13f;
                float num73 = 150f;
                float num74 = num72 * num73;
                Vector2 center3 = player.Center;
                if (NPC.Distance(center3) <= 3200f)
                {
                    Vector2 vector5 = Vector2.Zero;
                    Vector2 vector6 = Vector2.UnitY;
                    float num75 = 0.4f;
                    float num76 = 1.4f;
                    float num77 = 1f;
                    if (Main.expertMode)
                    {
                        num72 += 5f;
                        num73 += 50f;
                        num77 *= 1f;
                    }
                    switch (num71)
                    {
                        case 0:
                            center3 += new Vector2((0f - num74) / 2f, 0f) * num77;
                            vector5 = new Vector2(0f, num74);
                            vector6 = Vector2.UnitX;
                            break;
                        case 1:
                            center3 += new Vector2(num74 / 2f, num73 / 2f) * num77;
                            vector5 = new Vector2(0f, num74);
                            vector6 = -Vector2.UnitX;
                            break;
                        case 2:
                            center3 += new Vector2(0f - num74, 0f - num74) * num75 * num77;
                            vector5 = new Vector2(num74 * num76, 0f);
                            vector6 = new Vector2(1f, 1f);
                            break;
                        case 3:
                            center3 += new Vector2(num74 * num75 + num73 / 2f, (0f - num74) * num75) * num77;
                            vector5 = new Vector2((0f - num74) * num76, 0f);
                            vector6 = new Vector2(-1f, 1f);
                            break;
                        case 4:
                            center3 += new Vector2(0f - num74, num74) * num75 * num77;
                            vector5 = new Vector2(num74 * num76, 0f);
                            vector6 = center3.DirectionTo(player.Center);
                            break;
                        case 5:
                            center3 += new Vector2(num74 * num75 + num73 / 2f, num74 * num75) * num77;
                            vector5 = new Vector2((0f - num74) * num76, 0f);
                            vector6 = center3.DirectionTo(player.Center);
                            break;
                    }
                    for (float num79 = 0f; num79 <= 1f; num79 += 1f / num72)
                    {
                        Vector2 vector7 = center3 + vector5 * (num79 - 0.5f);
                        Vector2 v2 = vector6;
                        
                        float ai2 = num79;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ExlightUtils.NewProjectileBetter(vector7, Vector2.Zero, ModContent.ProjectileType<EmpressSword>(), 60, 0f, default, v2.ToRotation(), ai2);
                        }
                    }
                }
            }
            NPC.ai[1] += 1f;
            if (NPC.ai[1] >= num70 + num68)
            {
                SelectNextAttack(0);
            }
        }

        public void DoGiantIchorBall(Player player)
        {
            AttackTimer[0]++;
            NPC.velocity = Vector2.Zero;
            if (AttackTimer[0] == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    ExlightUtils.NewProjectileBetter(NPC.Center, Vector2.Zero, ModContent.ProjectileType<GiantIchorBall>(), 0, 0f, ai1: NPC.whoAmI);
                }
            }

            if (AttackTimer[0] > 180)
            {
                float speed = Main.masterMode ? 4f : Main.expertMode ? 3f : 2f;
                float turnResist = Main.masterMode ? 55f : Main.expertMode ? 40f : 30f;
                NPCHelper.SmoothTurnMovement(NPC.whoAmI, player.Center, speed, turnResist);
            }
            else
            {
                NPC.velocity *= 0.9f;
            }
            
            if (AttackTimer[0] >= 420)
            {
                SelectNextAttack(0);
            }

            if (AttackTimer[0] < 360)
            {
                VisualTimer[1] = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(0f, 60f, AttackTimer[0], true));
            }
            else
            {
                VisualTimer[1] = MathHelper.Lerp(1f, 0f, Utils.GetLerpValue(0f, 60f, AttackTimer[0] - 300f, true));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //textures
            Texture2D glowTexture = ModContent.Request<Texture2D>("Exlight/NPCs/Bosses/TheFearedEmpress/TheFearedEmpressGlow").Value;
            Texture2D mainTexture = TextureAssets.Npc[NPC.type].Value;

            //colors
            Color glowColor = NPC.GetAlpha(Color.Lerp(Color.Transparent, Color.White, VisualTimer[0]));
            Color trailColor = NPC.GetAlpha(Color.Lerp(Color.Yellow, Color.Red, VisualTimer[1]));
            Color trailTransparency = NPC.GetAlpha(Color.Lerp(trailColor, Color.Transparent, VisualTimer[2]));

            //drawing
            DrawHelper.DrawNPCTrail(NPC.whoAmI, glowTexture, trailTransparency, NPC.rotation, false, animated: true);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, mainTexture, NPC.GetAlpha(drawColor), NPC.rotation, animated: true);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, glowTexture, glowColor, NPC.rotation, animated: true);
            return false;
        }
    }
}
