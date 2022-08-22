using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Exlight.Common.Systems.ParticleSystem
{
    public class Particle
    {
        public int ID;
        public int Type;
        public int MaxTime;
        public int LifeTime;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Scale;

        public float ParticleLifeCompletion
        {
            get
            {
                if (LifeTime == 0)
                {
                    return 0;
                }
                return LifeTime / MaxTime;
            }
        }
        public virtual string Texture => "";

        public virtual bool HasSetLifetime => false;

        public virtual bool UseCustomDrawing => false;

        public virtual bool UseAdditiveBlending => false;

        public virtual void Update()
        {

        }

        public virtual void CustomDrawing(SpriteBatch spriteBatch)
        {

        }

        public void Kill()
        {
            ParticleManager.KillParticle(this);
        }
    }
}
