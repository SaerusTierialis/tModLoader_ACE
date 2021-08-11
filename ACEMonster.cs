using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ACE
{
    public class ACEMonster : GlobalNPC
    {
        public Containers.Entity Entity { get; private set; }
        public NPC npc { get; private set; }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Overrides ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        /// <summary>
        /// Instance per entity to store base xp, etc.
        /// </summary>
        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            this.npc = npc;
            Entity = new Containers.Entity(this);
        }

        public override void ResetEffects(NPC npc)
        {
            Entity.PreUpdate();
        }

    }
}
