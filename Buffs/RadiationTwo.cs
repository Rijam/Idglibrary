using Terraria;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class RadiationTwo: ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Radiation II");
			Description.SetDefault("You are suffering moderate radiation poisoning");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
			idgplayer.radationlevel += 3;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}
}
