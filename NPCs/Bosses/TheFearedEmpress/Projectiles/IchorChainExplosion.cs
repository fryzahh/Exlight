using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Exlight.Common.Utilities;
using Terraria.GameContent;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class IchorChainExplosion : ModProjectile
    {
        public override string Texture => "Exlight/Textures/Explosion";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Festering Explosion");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }

            //Split into even more explosions
            if (++Projectile.localAI[1] == 5 && Projectile.ai[1] > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.ai[1]--;
                Vector2 baseDirection = Projectile.ai[0].ToRotationVector2();
                float random = MathHelper.ToRadians(15);
                Vector2 offset = Projectile.width * 0.65f * baseDirection.RotatedBy(Main.rand.NextFloat(-random, random));
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + offset, Vector2.Zero, Projectile.type,
                    Projectile.damage, 0f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.scale = Main.rand.NextFloat(1.25f, 1.51f);

                Projectile.position = Projectile.Center;
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Yellow), Projectile.rotation, Projectile.scale, true, false);
            return false;
        }
    }
}
