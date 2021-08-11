using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace ACE.Systems.XPRewards
{
    public static class Rewards
    {
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Constants ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        private const float BONUS_XP_RATIO_PER_PLAYER = 0.1f;
        private const float WELL_FED_MULTIPLIER = 1.05f;

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        /// <summary>
        /// true while catch-up is active locally
        /// </summary>
        public static bool CATCHUP_ACTIVE { get; private set; } = false;

        private static float XP_MULTIPLIER = 1.0f;

        private static int xp_overhead_index = Main.maxCombatText - 1;

        private static float incomplete_xp = 0;

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Public ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        public static void GiveXP(float xp, Microsoft.Xna.Framework.Rectangle location)
        {
            //apply final modifiers
            xp = CalcFinalXP(xp);

            //apply incomplete xp and calcualte final uint value
            xp += incomplete_xp;
            uint xp_final = (uint)Math.Floor(xp);
            incomplete_xp = xp - xp_final;

            //if at least 1 xp...
            if (xp_final > 0)
            {
                //give xp
                LocalData.LOCAL_PLAYER.AddXP(xp_final);

                //display
                DisplayXPReward(xp_final, location);
            }
        }

        public static void UpdateXPMultiplier()
        {
            if (ACE.NETMODE_PLAYER && LocalData.LOCAL_PLAYER_VALID)
            {
                //calc new mult...
                XP_MULTIPLIER = 1.0f;

                //default to no catchup
                CATCHUP_ACTIVE = false;

                //multiplayer...
                if (ACE.NETMODE_CLIENT)
                {
                    //adjust for number of players
                    int number_players = CountEligiblePlayers();
                    XP_MULTIPLIER *= 1.0f + ((number_players - 1.0f) * BONUS_XP_RATIO_PER_PLAYER);
                    XP_MULTIPLIER /= number_players;

                    //catch-up system
                    if (ConfigServer.Instance.AllowXPCatchup)
                    {
                        //get highest character level
                        int highest_level = 0;
                        foreach (ACEPlayer ACEPlayer in Utilities.Commons.GetACEPlayers())
                        {
                            highest_level = (int)Math.Max(highest_level, ACEPlayer.SyncData.CharacterLevel);
                        }

                        int level_difference = (int)(highest_level - LocalData.LOCAL_PLAYER.SyncData.CharacterLevel);
                        if (level_difference >= 5)
                        {
                            CATCHUP_ACTIVE = true;
                            XP_MULTIPLIER *= 3.0f + ((level_difference - 5.0f) / 50.0f); //300% multiplier plus 2%/level
                        }
                    }
                }

                //apply ModConfig rate
                XP_MULTIPLIER *= ConfigServer.Instance.XPRate;
            }
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Private ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        /// <summary>
        /// Returns the number players eligible for rewards including the local player
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private static int CountEligiblePlayers()
        {
            return Math.Max(1, Utilities.Commons.GetACEPlayers().Count);
        }

        private static void DisplayXPReward(uint xp, Microsoft.Xna.Framework.Rectangle location, bool bonus = false)
        {
            if (Main.combatText[xp_overhead_index].active)
            {
                //update text
                Main.combatText[xp_overhead_index].active = false;
            }
            else
            {
                //new text
                LocalData.xp_overhead.Reset();
            }

            //add to counter
            LocalData.xp_overhead.Add(xp);

            //change location to player if too far
            if (location.Distance(Main.LocalPlayer.position) > 1000f) //TODO use current screen size and check if on-scren rather than cutoff distance
            {
                location = Main.LocalPlayer.getRect();
            }

            //display
            xp_overhead_index = CombatText.NewText(location, UI.Constants.COLOUR_XP_BRIGHT, "+" + LocalData.xp_overhead.Value + " XP");
            Main.combatText[xp_overhead_index].crit = true;
        }

        private static float CalcFinalXP(float base_xp)
        {
            base_xp *= XP_MULTIPLIER;

            if (Main.LocalPlayer.wellFed)
            {
                base_xp *= WELL_FED_MULTIPLIER;
            }

            return base_xp;
        }

    }
}
