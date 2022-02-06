using Terraria;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class BossFightPurity: ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Boss Fight Purity");
			Description.SetDefault("You can only use vanilla and modded from the boss's mod\n");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
			canBeCleared = false;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			IdgPlayer idgplayer = Main.LocalPlayer.GetModPlayer(mod, typeof(IdgPlayer).Name) as IdgPlayer;
			rare = 5;
			tip = tip + "mod: " + idgplayer.bossmodstring;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;

			Mod npcmod = null;
			for (int i = 0; i < Main.maxNPCs; i += 1)
			{
				NPC npc = Main.npc[i];
				if (npc.active && npc.boss)
				{
					if (npc.modNPC != null)
					{
						npcmod = npc.modNPC.mod;
						break;
					}


				}

			}
				
				if (npcmod == null)
				{
					player.DelBuff(buffIndex);
				}
				else
				{
					idgplayer.bossmod = npcmod;
					idgplayer.bossmodstring = npcmod.DisplayName;
				}
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}
}
