using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Exlight.Projectiles.BaseProjectileClasses
{
    public abstract class BaseTelegraphLineProjectile : ModProjectile
    {
        public abstract int Owner { get; }

        public abstract Color DrawColor { get; }

        public abstract int LifeTime { get; }

        public abstract float Alpha { get; }

        public virtual bool AttachToOwner => false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Projectile owner = Main.projectile[Owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + owner.rotation;
            if (AttachToOwner)
            {
                Projectile.Center = owner.Center;
            }

            if (++Projectile.localAI[0] >= LifeTime)
            {
                Projectile.Kill();
                return;
            }

            if (Alpha >= 0)
            {
                Projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / LifeTime * Projectile.localAI[0]) * Alpha);
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return DrawColor * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture = TextureAssets.Extra[197].Value;
            int num = texture.Height / Main.projFrames[Projectile.type];
            int y = num * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, y, texture.Width, num);
            Vector2 origin = rectangle.Size() / 2f;
            float length = 256f * Projectile.scale;
            for (float i = 0; i <= 3000f; i += length)
            {
                Vector2 offset = Projectile.rotation.ToRotationVector2() * (i + length / 2);
                Main.EntitySpriteDraw(texture, offset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}
