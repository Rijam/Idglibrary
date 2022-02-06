using Terraria;
using Terraria.ModLoader;

namespace Idglibrary.Buffs
{
	public class RadiationThree: ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Radiation III");
			Description.SetDefault("You are suffering lethal radiation poisoning");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			IdgPlayer idgplayer = player.GetModPlayer(mod,typeof(IdgPlayer).Name) as IdgPlayer;
			idgplayer.radationlevel += 10;
		}

		//public override void Update(NPC npc, ref int buffIndex)
		//{
		//	npc.GetGlobalNPC<SGAWorld>(mod).eFlames = true;
		//}
	}
}
