using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using System;

namespace Exlight.Common.Systems.ParticleSystem
{
    public class ParticleUpdater : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
            {
                ParticleManager.Update();
            }
        }
    }
}
