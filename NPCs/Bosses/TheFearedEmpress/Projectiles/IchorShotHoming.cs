using Terraria;

namespace Exlight.NPCs.Bosses.TheFearedEmpress.Projectiles
{
    public class IchorShotHoming : IchorShot
    {
        public override string Texture => "Exlight/NPCs/Bosses/TheFearedEmpress/Projectiles/IchorShot";
        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            base.AI();
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= Time && Projectile.ai[1] <= Time + 30)
            {
                Projectile.velocity *= 0.9f;
            }

            if (Projectile.ai[1] == Time + 30)
            {
                Player player = Main.player[(Player.FindClosest(Projectile.Center, 0, 0))];
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 10f;
            }
        }
    }
}
