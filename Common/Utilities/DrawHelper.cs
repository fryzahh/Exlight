using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Exlight.Common.Utilities
{
    public static class DrawHelper
    {
        public static void DrawProjectileTrail(int whoAmI, Texture2D texture, Color color, float rotation, bool useProgressiveScaling, float textScaling, bool animated = false, bool useFlipSpriteEffects = false, bool useAdditiveBlend = false, bool useAlphaBlend = false)
        {
            Projectile projectile = Main.projectile[whoAmI];
            int num = Terraria.GameContent.TextureAssets.Projectile[projectile.type].Value.Height / Main.projFrames[projectile.type];
            int y = num * projectile.frame;
            Rectangle rectangle = animated ? new Rectangle(0, y, texture.Width, num) : new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Color glowColor = color;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (useAdditiveBlend)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                }

                Color drawColor = glowColor;
                drawColor *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i] - (Vector2.Normalize(projectile.velocity) * i * 2);
                var effects = useFlipSpriteEffects ? projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Main.EntitySpriteDraw(texture, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), drawColor,
                    rotation, origin, useProgressiveScaling ? scale * textScaling : projectile.scale, effects, 0);

                if (useAlphaBlend)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                }
            }
        }

        public static void DrawProjectileTexture(int whoAmI, Texture2D texture, Color color, float rotation, float scale = 1f, bool animated = false, bool useFlipSpriteEffects = false, bool useAdditiveBlend = false, bool useAlphaBlend = false)
        {
            Projectile projectile = Main.projectile[whoAmI];

            if (useAdditiveBlend)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            int num = Terraria.GameContent.TextureAssets.Projectile[projectile.type].Value.Height / Main.projFrames[projectile.type];
            int y = num * projectile.frame;
            Rectangle rectangle = animated ? new Rectangle(0, y, texture.Width, num) : new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color drawColor = projectile.GetAlpha(color);
            var effects = useFlipSpriteEffects ? projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(drawColor), rotation, origin2, scale, effects, 0);

            if (useAlphaBlend)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }

        public static void DrawNPCTrail(int whoAmI, Texture2D texture, Color color, float rotation, bool useProgressiveScaling, float textScaling = 1f, bool animated = false, bool useFlipSpriteEffects = false, bool useAdditiveBlend = false, bool useAlphaBlend = false)
        {
            NPC npc = Main.npc[whoAmI];
            Rectangle rectangle = animated ? npc.frame : new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Color glowColor = color;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                if (useAdditiveBlend)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                }

                Color drawColor = glowColor;
                drawColor *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                float scale = npc.scale * (NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i] - (Vector2.Normalize(npc.velocity) * i * 2);
                var effects = useFlipSpriteEffects ? npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Main.EntitySpriteDraw(texture, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), drawColor,
                    rotation, origin, useProgressiveScaling ? scale * textScaling : npc.scale, effects, 0);

                if (useAlphaBlend)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                }
            }
        }

        public static void DrawNPCTexture(int whoAmI, Texture2D texture, Color color, float rotation, float scale = 1f, bool animated = false, bool useFlipSpriteEffects = false, bool useAdditiveBlend = false, bool useAlphaBlend = false)
        {
            NPC npc = Main.npc[whoAmI];

            if (useAdditiveBlend)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            Rectangle rectangle = animated ? npc.frame : new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color drawColor = npc.GetAlpha(color);
            var effects = useFlipSpriteEffects ? npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(drawColor), rotation, origin2, scale, effects, 0);

            if (useAlphaBlend)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }
}
