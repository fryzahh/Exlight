using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace Exlight.Common.Utilities
{
    public static class NPCHelper
    {
        public static void SmoothTurnMovement(int whoAmI, Vector2 targetPosition, float speed, float turnResistance)
        {
            NPC npc = Main.npc[whoAmI];
            Vector2 move = targetPosition - npc.Center;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            npc.velocity = move;
        }

        public static void WilderMovement(int whoAmI, Vector2 target, float speedX, float speedY, float capX = 10f, float capY = 5f)
        {
            NPC npc = Main.npc[whoAmI];
            npc.velocity += Vector2.Normalize(target - npc.Center) * new Vector2(speedX, speedY);
            npc.velocity.X = Utils.Clamp(npc.velocity.X, -capX, capX);
            npc.velocity.Y = Utils.Clamp(npc.velocity.Y, -capY, capY);
        }
    }
}
