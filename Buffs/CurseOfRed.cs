using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class CurseOfRed : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Curse of Red");
			Description.SetDefault("Your about to drink a Red Potion! Toss it out! Now!!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override string Texture => "Terraria/Images/Item_" +ItemID.RedPotion;

		public override void Update(Player player, ref int buffIndex)
		{

			if (player.buffTime[buffIndex] > 180)
			{
				if (player.CountItem(ItemID.RedPotion) < 1)
					player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.RedPotion);
			}
			if (player.buffTime[buffIndex] == 2)
			{
			if (player.CountItem(ItemID.RedPotion) > 0)
				{
					player.AddBuff(Mod.Find<ModBuff>("NoImmunities").Type, 180);					
					int Itemthis = player.FindItem(ItemID.RedPotion);
					Item thisitem = player.inventory[Itemthis];
					player.selectedItem = Itemthis;
					player.controlUseItem = true;
					ItemLoader.UseItem(thisitem, player);
					player.AddBuff(Mod.Find<ModBuff>("CurseOfRed").Type, 60);
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
