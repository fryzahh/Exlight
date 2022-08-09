using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

namespace Exlight.Common.Players
{
    public class PlayerAccessoryUpdates : ModPlayer
    {
        public bool PhoenixRing;

        public bool PhoenixRingReadyForRevive;
        public int PhoenixRingCooldownTimer;
        public int PhoenixRingCooldownDuration = 180;

        public override void ResetEffects()
        {
            PhoenixRing = false;

            PhoenixRingReadyForRevive = false;
            PhoenixRingCooldownTimer = PhoenixRingCooldownDuration;
            PhoenixRingCooldownDuration = default;
        }

        public override void UpdateDead()
        {
            PhoenixRing = false;

            PhoenixRingReadyForRevive = false;
            PhoenixRingCooldownTimer = PhoenixRingCooldownDuration;
            PhoenixRingCooldownDuration = default;
        }


        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (PhoenixRingReadyForRevive)
            {
                PhoenixRingReadyForRevive = false;
                if (Player.statLife < 10)
                {
                    Player.statLife = 50;
                }
                return false;
            }
            return true;
        }

        public override void PostUpdate()
        {
                     
        }
    }
}
