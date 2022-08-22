using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Exlight.Common.Utilities;
using Terraria.GameContent;
using System.IO;

namespace Exlight.NPCs.Bosses.TheFearedEmpress
{
    public class IchorRingSpawner : ModProjectile
    {
        public override string Texture => "Exlight/Textures/BlankSprite";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Ring Spawner");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
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

        //Im calling KillProjectiles and Kill() at the same time because for some fucking reason the projectile just decides to desync and not die
        //I'll probably change this later but man this sucks
        public override void AI()
        {
            NPC empress = Main.npc[(int)Projectile.ai[0]];
            Player player = Main.player[empress.target];
            TheFearedEmpress.EmpressAttacks empressAttackType = (TheFearedEmpress.EmpressAttacks)empress.ai[0];
            if (!empress.active)
            {
                Projectile.Kill();
                ExlightUtils.KillProjectiles(ModContent.ProjectileType<Projectiles.OrbitingIchorOrb>(), Projectile.type);
                return;
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                if (Main.masterMode)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int MasterModeOrbCount = i != 0 ? 16 : 8;
                        int width = i != 0 ? Projectile.width + 200 : Projectile.width + 150;
                        float speed = i != 0 ? 5f : 3f;
                        ExlightUtils.SpawnProjectileRotatersForProjectile(Projectile, width, speed, ModContent.ProjectileType<Projectiles.OrbitingIchorOrb>(), Projectile.damage, MasterModeOrbCount, i == 0);
                    }
                }
                else
                {
                    int numProj = Main.expertMode ? 16 : 8;
                    ExlightUtils.SpawnProjectileRotatersForProjectile(Projectile, Projectile.width + 200, 3f, ModContent.ProjectileType<Projectiles.OrbitingIchorOrb>(), Projectile.damage, numProj, true);
                }
                Projectile.netUpdate = true;
            }

            if (empressAttackType == TheFearedEmpress.EmpressAttacks.ShootIchorRing)
            {
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] == 180)
                {
                    Projectile.netUpdate = true;
                    Vector2 vectorToPlayer = Main.masterMode ? (player.Center + player.velocity * 30f) : player.Center;
                    float speed = Main.masterMode ? 15f : Main.expertMode ? 13f : 10f;
                    Projectile.velocity = Projectile.DirectionTo(vectorToPlayer) * speed;
                }
               
                if (Projectile.localAI[1] >= 300)
                {
                    Projectile.Kill();
                    ExlightUtils.KillProjectiles(ModContent.ProjectileType<Projectiles.OrbitingIchorOrb>(), Projectile.type);
                    return;
                }

                if (Projectile.localAI[1] > 180 && Projectile.localAI[1] < 360)
                {
                    Projectile.velocity *= 0.98f;
                }
            }

            if (empressAttackType != TheFearedEmpress.EmpressAttacks.ShootIchorRing || empressAttackType == TheFearedEmpress.EmpressAttacks.ShootIchorRing && Projectile.localAI[1] < 180)
            {
                Projectile.velocity = Vector2.Zero.MoveTowards(empress.Center - Projectile.Center, 25f);
            }
        }
    }
}
