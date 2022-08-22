using Exlight.Common.Systems.ParticleSystem;
using Exlight.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Exlight.Items
{
	public class FesteringIchorBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Festering Ichor Blade"); 
			Tooltip.SetDefault("Fires a mass of ichor that bounces and leaves a trail of smaller ichor projectiles behind it\nIchor masses are killed after 5 bounces\nThe festering power of the crimson...");
		}

		public override void SetDefaults()
		{
			Item.damage = 174;
			Item.DamageType = DamageClass.Melee;
			Item.width = 24;
			Item.height = 24;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Melee.FesteringIchorBall>();
			Item.shootSpeed = 8f;
		}

		public override void PostUpdate()
		{
			for (int k = 0; k < 1; k++)
			{
				Vector2 val5 = Item.Center + Main.rand.NextVector2CircularEdge(100, 100);
				Vector2 v = val5 - Item.Center;
				v = v.SafeNormalize(Vector2.Zero) * -3f;
				Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
				ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.25f, 0.46f), Main.rand.Next(180, 191), false, true));
			}
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			for (int k = 0; k < 3; k++)
			{
				Vector2 val5 = Item.Center + Main.rand.NextVector2CircularEdge(Item.width / 2, Item.height / 2);
				Vector2 v = Vector2.Zero.SafeNormalize(val5 - Item.Center) * -8f;
				Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.1f, 0.9f));
				ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.25f, 0.66f), Main.rand.Next(60, 70), false, false));
			}
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}