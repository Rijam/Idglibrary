using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class CurseOfRed : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Curse of Red");
			Description.SetDefault("Your about to drink a Red Potion! Toss it out! Now!!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override bool Autoload(ref string name, ref string texture)
		{
			texture = "Terraria/Item_"+ItemID.RedPotion;
			return true;
		}

		public override void Update(Player player, ref int buffIndex)
		{

			if (player.buffTime[buffIndex] > 180)
			{
				if (player.CountItem(ItemID.RedPotion) < 1)
					player.QuickSpawnItem(ItemID.RedPotion);
			}
			if (player.buffTime[buffIndex] == 2)
			{
			if (player.CountItem(ItemID.RedPotion) > 0)
				{
					player.AddBuff(mod.BuffType("NoImmunities"), 180);					
					int Itemthis = player.FindItem(ItemID.RedPotion);
					Item thisitem = player.inventory[Itemthis];
					player.selectedItem = Itemthis;
					player.controlUseItem = true;
					ItemLoader.UseItem(thisitem, player);
					player.AddBuff(mod.BuffType("CurseOfRed"), 60);
				}


			}

				//IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
			//idgplayer.souldrainlevel += 1;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}
}
