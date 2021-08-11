using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static ACE.LocalData;
using Terraria.ID;
using ACE.Utilities;

namespace ACE.Systems.XPRewards
{
    class NPCRewards : GlobalNPC
    {
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constants ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        private const uint TICKS_BETWEEN_CHECK_LOOKUP = 3600; //1 minute

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Static Fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        private static Dictionary<int, float> XP_lookup = new Dictionary<int, float>();
        private static uint time_next_check_lookup = TICKS_BETWEEN_CHECK_LOOKUP;

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Overrides ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        /// <summary>
        /// Instance per entity to store base xp, etc.
        /// </summary>
        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            //calculate xp value on base stats (uses non-expert, single-player stats)
            UpdateXPValue(npc);
        }

        public override bool CheckDead(NPC npc)
        {
            bool dying = base.CheckDead(npc);

            if (dying && ACE.NETMODE_PLAYER && LOCAL_PLAYER_VALID && !npc.friendly && (XP_lookup[npc.type] > 0))
            {
                Rewards.GiveXP(XP_lookup[npc.type], npc.getRect());
            }

            return dying;
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        private static void UpdateXPValue(NPC npc)
        {
            //get base xp
            if (!XP_lookup.ContainsKey(npc.type))
            {
                //calculate if not in lookup
                XP_lookup[npc.type] = CalcNPCValue(npc);
            }
            else if (Main.GameUpdateCount > time_next_check_lookup)
            {
                //every so often, check values in lookup - if mismatch then clear entire lookup
                float current_value = CalcNPCValue(npc);
                if (XP_lookup[npc.type] != current_value)
                {
                    ClearLoookup();
                    XP_lookup[npc.type] = current_value;
                }
                time_next_check_lookup = Main.GameUpdateCount + TICKS_BETWEEN_CHECK_LOOKUP;
            }
        }

        private static float CalcNPCValue(NPC npc)
        {
            //no exp from statue, critter, or friendly
            if (npc.SpawnedFromStatue || npc.lifeMax <= 5 || npc.friendly || !npc.active) return 0;

            //calculate
            float xp = 0;
            if (npc.defense >= 1000)
                xp = (npc.lifeMax / 80.0f) * (1.0f + (npc.damage / 20.0f));
            else
                xp = (npc.lifeMax / 100.0f) * (1.0f + (npc.defense / 10.0f)) * (1.0f + (npc.damage / 25.0f));

            //adjustment to keep xp approx where it was pre-revamp
            xp *= 3.0f;

            //special cases
            switch (npc.type)
            {
                case NPCID.EaterofWorldsHead:
                    xp *= 1.801792115f;
                    break;

                case NPCID.EaterofWorldsBody:
                    xp *= 1.109713024f;
                    break;

                case NPCID.EaterofWorldsTail:
                    xp *= 0.647725809f;
                    break;
            }

            //return
            return xp;
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        public static void ClearLoookup()
        {
            XP_lookup.Clear();
        }

    }
}
