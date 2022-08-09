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

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}