using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using System.Linq;
using System.Collections.Generic;

namespace Exlight.Common.Utilities
{
    public static class ExlightUtils
    {
		public static int NewProjectileBetter(float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, int owner = -1, float ai0 = 0, float ai1 = 0)
        {
			if (owner == -1)
            {
				owner = Main.myPlayer;
            }
			damage = (int)((double)damage * 0.5);
			if (Main.expertMode)
            {
				damage = (int)((double)damage * 0.5);
			}
			if (Main.masterMode)
            {
				damage = (int)((double)damage * 0.75);
			}
			return Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1);
        }

		public static int NewProjectileBetter(Vector2 center, Vector2 velocity, int type, int damage, float knockback, int owner = -1, float ai0 = 0, float ai1 = 0)
        {
			return NewProjectileBetter(center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, owner, ai0, ai1);
        }

		public static int NewNPCBetter(float spawnX, float spawnY, int type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255)
		{
			return NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnX, (int)spawnY, type, Start, ai0, ai1, ai2, ai3, target);
		}

		public static int NewNPCBetter(Vector2 spawn, int type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255)
		{
			return NewNPCBetter(spawn.X, spawn.Y, type, Start, ai0, ai1, ai2, ai3, target);
		}

		public static void KillNPCs(params int[] types)
		{
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				for (int j = 0; j < types.Length; j++)
				{
					if (npc.active && npc.type == types[j])
					{
						npc.life = 0;
						npc.HitEffect();
						npc.checkDead();
					}
				}
			}
		}

		public static void KillProjectiles(params int[] types)
        {
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile projectile = Main.projectile[i];
				for (int j = 0; j < types.Length; j++)
				{
					if (projectile.active && projectile.type == types[j])
					{
						projectile.Kill();
					}
				}
			}
		}

		public static IEnumerable<Projectile> IsProjectileActive(params int[] types)
        {
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active && types.Contains(projectile.type))
				{
					yield return projectile;
				}
			}
		}

		public static void ExpandProjectileHitbox(Projectile projectile, int width, int height)
		{
			projectile.position = projectile.Center;
			projectile.width = width;
			projectile.height = height;
			projectile.position -= projectile.Size * 0.5f;
		}

		public static void ExpandProjectileHitbox(Projectile projectile, int newSize)
		{
			ExpandProjectileHitbox(projectile, newSize, newSize);
		}

		public static void SpawnNPCRotatersForNPC(NPC npc, int radius, float speed, int type, int damage, int amount, bool clockwise)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			Vector2 center = npc.Center;
			for (int k = 0; k < amount; k++)
			{
				float angle = 2f * (float)Math.PI / amount * k;
				Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				int child = NPC.NewNPC(npc.GetSource_FromAI(), (int)pos.X, (int)pos.Y, type, 0, npc.whoAmI, angle, 0f, 0f, Main.myPlayer);
				Main.npc[child].localAI[0] = radius;
				Main.npc[child].localAI[1] = clockwise ? speed : -speed;
				Main.npc[child].damage = damage;
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, child);
			}
		}

		public static void SpawnProjectileRotatersForNPC(NPC npc, int radius, float speed, int type, int damage, int amount, bool clockwise)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			Vector2 center = npc.Center;
			for (int k = 0; k < amount; k++)
			{
				float angle = 2f * (float)Math.PI / amount * k;
				Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				Projectile.NewProjectile(npc.GetSource_FromAI(), pos.X, pos.Y, radius, clockwise ? speed : -speed, type, damage, 0f, Main.myPlayer, npc.whoAmI, angle);
			}
		}

		public static void SpawnProjectileRotatersForProjectile(Projectile projectile, int radius, float speed, int type, int damage, int amount, bool clockwise)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			Vector2 center = projectile.Center;
			for (int k = 0; k < amount; k++)
			{
				float angle = 2f * (float)Math.PI / amount * k;
				Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				Projectile.NewProjectile(projectile.GetSource_FromAI(), pos.X, pos.Y, radius, clockwise ? speed : -speed, type, damage, 0f, Main.myPlayer, projectile.whoAmI, angle);
			}
		}

		public static void SpawnNPCRotatersForProjectile(Projectile projectile, int radius, float speed, int type, int damage, int amount, bool clockwise)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			Vector2 center = projectile.Center;
			for (int k = 0; k < amount; k++)
			{
				float angle = 2f * (float)Math.PI / amount * k;
				Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				int child = NPC.NewNPC(projectile.GetSource_FromAI(), (int)pos.X, (int)pos.Y, type, 0, projectile.whoAmI, angle, 0f, 0f, Main.myPlayer);
				Main.npc[child].localAI[0] = radius;
				Main.npc[child].localAI[1] = clockwise ? speed : -speed;
				Main.npc[child].damage = damage;
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, child);
			}
		}
	}
}
