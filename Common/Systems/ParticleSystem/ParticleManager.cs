using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Exlight.Common.Systems.ParticleSystem
{
    public static class ParticleManager
    {
        const int MaxParticles = 3000;

        private static List<Particle> Particles;
        private static List<Particle> ParticlesToBeKilled;
        private static List<Particle> ParticleInstances;
        private static Dictionary<Type, int> ParticleTypes;
        private static Dictionary<int, Texture2D> ParticleTextures;
        private static List<Particle> ParticlesUsingAlphaBlend;
        private static List<Particle> ParticlesUsingAdditiveBlend;

        public static void LoadParticleInstances(Mod mod)
        {
            Type typeFromHandle = typeof(Particle);
            Type[] loadableTypes = AssemblyManager.GetLoadableTypes(mod.Code);
            foreach (Type type in loadableTypes)
            {
                if (type.IsSubclassOf(typeFromHandle) && !type.IsAbstract && type != typeFromHandle)
                {
                    int count = ParticleTypes.Count;
                    ParticleTypes[type] = count;
                    Particle particle = (Particle)FormatterServices.GetUninitializedObject(type);
                    ParticleInstances.Add(particle);
                    string name = type.Namespace!.Replace('.', '/') + "/" + type.Name;
                    if (particle.Texture != "")
                    {
                        name = particle.Texture;
                    }
                    ParticleTextures[count] = ModContent.Request<Texture2D>(name, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                }
            }
        }

        internal static void Load()
        {
            Particles = new List<Particle>();
            ParticlesToBeKilled = new List<Particle>();
            ParticleInstances = new List<Particle>();
            ParticleTypes = new Dictionary<Type, int>();
            ParticleTextures = new Dictionary<int, Texture2D>();
            ParticlesUsingAlphaBlend = new List<Particle>();
            ParticlesUsingAdditiveBlend = new List<Particle>();
            LoadParticleInstances(ModContent.GetInstance<Exlight>());
        }

        internal static void Unload()
        {
            Particles = null;
            ParticlesToBeKilled = null;
            ParticleInstances = null;
            ParticleTypes = null;
            ParticleTextures = null;
            ParticlesUsingAlphaBlend = null;
            ParticlesUsingAdditiveBlend = null;
        }

        public static void SpawnParticle(Particle particle)
        {
            if (!Main.gamePaused && !Main.dedServ && Particles != null && (Particles.Count < MaxParticles))
            {
                Particles.Add(particle);
                particle.Type = ParticleTypes[particle.GetType()];
            }
        }

        public static void Update()
        {
            if (Particles != null)
            {
                foreach (Particle particle in Particles)
                {
                    particle.Position += particle.Velocity;
                    particle.LifeTime++;
                    particle.Update();
                }
                Particles.RemoveAll((Particle particle) => (particle.LifeTime >= particle.MaxTime) || (ParticlesToBeKilled.Contains(particle)));
                ParticlesToBeKilled.Clear();
            }
        }

        public static void KillParticle(Particle particle)
        {
            ParticlesToBeKilled.Add(particle);
        }

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in Particles)
            {
                if (particle != null)
                {
                    if (particle.UseAdditiveBlending)
                    {
                        ParticlesUsingAdditiveBlend.Add(particle);
                    }
                    else
                    {
                        ParticlesUsingAlphaBlend.Add(particle);
                    }
                }
            }

            spriteBatch.End();
            if (ParticlesUsingAdditiveBlend.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                foreach (Particle particle in ParticlesUsingAdditiveBlend)
                {
                    if (particle.UseCustomDrawing)
                        particle.CustomDrawing(spriteBatch);
                    else
                        spriteBatch.Draw(ParticleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);
                }
                spriteBatch.End();
            }
            if (ParticlesUsingAlphaBlend.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                foreach (Particle particle in ParticlesUsingAlphaBlend)
                {
                    if (particle.UseCustomDrawing)
                        particle.CustomDrawing(spriteBatch);
                    else
                        spriteBatch.Draw(ParticleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);
                }
                spriteBatch.End();
            }

            ParticlesUsingAdditiveBlend.Clear();
            ParticlesUsingAlphaBlend.Clear();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
        }

        public static Texture2D GetParticleTexture(int type) => ParticleTextures[type];
    }
}
