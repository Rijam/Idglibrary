using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Idglibrary.Buffs
{
	public class SoulDrain: ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Drain I");
			Description.SetDefault("Your very magic is being stripped from you");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.souldrainlevel += 1;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}

	public class NoImmunities : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Anti-Immunity");
			Description.SetDefault("Your debuff immunities are nullified!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override string Texture => "Idglibrary/Buffs/SoulDrain";

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.noimmunity = 10;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}

	public class RadCure : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiation Cure");
			Description.SetDefault("A cure for Radiation? That's RADical!");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.radationRecover += IdgNPC.bossAlive ? 0.02f : 0.2f;
			idgplayer.radresist += 0.50f;
		}

	}

	public class Damnation : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Damnation");
			Description.SetDefault("Your health regen is sundered and healing items heal for far less");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override string Texture => "Idglibrary/Buffs/SoulDrain";

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			if (idgplayer.Damnation < 1)
			{
				idgplayer.Damnation = 15;
				SoundEngine.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 84, 0.5f, -0.5f);

			}
			if (idgplayer.Damnation < 10)
				idgplayer.Damnation = 10;
		}
	}

	public class HotBarCurse : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hotbar Curse");
			Description.SetDefault("A hotbar slot is locked!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			int slot = -1;
			for (int i = 0; i < 10; i += 1)
			{
				if (player.inventory[i] == player.HeldItem)
				{
					slot = i;
				}
			}
			if (slot>-1)
			idgplayer.hotbarcurse = slot;
			return true;
		}

		public override string Texture => "Idglibrary/Buffs/SoulDrain";

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			if (idgplayer.hotbarcurse < 0)
				ReApply(player, player.buffTime[buffIndex],buffIndex);
		}
	}

	public class ItemCurse : HotBarCurse
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Item Curse");
			Description.SetDefault("A type of item is locked!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			if (!player.HeldItem.IsAir)
			{
				idgplayer.itemcurse = player.HeldItem.type;
			}
			return true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			if (idgplayer.itemcurse < 0)
				ReApply(player, player.buffTime[buffIndex], buffIndex);
		}
	}

	public class NullExceptionDebuff: ModBuff
	{
		public static int clientthing = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("N0ll U");
			Description.SetDefault("");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override string Texture => "Idglibrary/Buffs/SoulDrain";

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			//IdgPlayer idgplayer = Main.LocalPlayer.GetModPlayer(mod, typeof(IdgPlayer).Name) as IdgPlayer;
			int nullis = NullExceptionDebuff.clientthing;

			string addtotip = "Your accessories don't work";
			if (nullis > 2)
				addtotip = "Only your wings work";
			if (nullis > 3)
				addtotip = "your Equips are ERROR";
			string thetip = "";
			for(int loc = 0; loc < addtotip.Length; loc += 1)
			{
				char character = addtotip[loc];
				if (Main.rand.Next(30) <= 1)
				{
					character = (char)(33 + Main.rand.Next(15));
				}

				thetip += character;
			}
			tip = thetip;


		}

		public override bool ReApply(Player player, int time, int buffIndex)
		{

			if (time > 60 * 1)
			{
				player.buffTime[buffIndex] += (int)(time/2);
				return false;
			}
			else
			{
				return true;
			}
		}


		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer<IdgPlayer>();
			idgplayer.NullU += 1+(int)(player.buffTime[buffIndex]/(60f*7f));
			NullExceptionDebuff.clientthing = idgplayer.NullU;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}

}
