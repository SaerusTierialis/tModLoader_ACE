using Terraria;

namespace ACE.Containers
{
	/// <summary>
	/// Contains a player or monster. Also contains common fields as well as any "Effects".
	/// </summary>
	public class Entity
	{
		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Target Info ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public readonly ACEPlayer ACEPlayer = null;
		public readonly ACEMonster ACEMonster = null;
		public readonly bool IsPlayer = false;
		public readonly bool IsMonster = false;

		/// <summary>
		/// Whether the entity is owned locally. Players are owned by their client. Monsters are owned by the server. Everything is local in singleplayer mode.
		/// 
		/// Player locality is set while joining a world after some initialization. Defaults to false.
		/// </summary>
		public bool Local { get; private set; } = false;

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constructors ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public Entity(ACEPlayer target) : this()
		{
			ACEPlayer = target;
			IsPlayer = true;
			//local is not yet known
		}

		public Entity(ACEMonster target) : this()
		{
			ACEMonster = target;
			IsMonster = true;
			Local = ACE.NETMODE_EFFECTIVELY_SERVER;
		}

		private Entity()
        {
			//any common setup could go here - note that Local is not yet accurate for players
		}

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		/// <summary>
		/// Called at the start of each cycle.
		/// </summary>
		public void PreUpdate()
        {
			if (IsPlayer)
				Utilities.Logger.Trace($"Player {WhoAmI} Local={Local}");
			else
				Utilities.Logger.Trace($"Monster {WhoAmI} Local={Local}");
		}

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Actions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

		public void SetAsLocalPlayer()
        {
			if (!IsPlayer || (WhoAmI != LocalData.WHO_AM_I))
				Utilities.Logger.Error("Attempted to set non-local entity as local.");
			else
				Local = true;
		}

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Shortcuts ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

		/// <summary>
		/// Returns player or npc ID. Should always be the same across clients.
		/// </summary>
		public int WhoAmI { get { return IsPlayer ? BasePlayer.whoAmI : (IsMonster ? BaseNPC.whoAmI : -1); } }

		/// <summary>
		/// Returns the Player if possible, else null.
		/// </summary>
		public Player BasePlayer { get { return ACEPlayer?.player; } }

		/// <summary>
		/// Returns the NPC if possible, else null.
		/// </summary>
		public NPC BaseNPC { get { return ACEMonster?.npc; } }

	}
}
