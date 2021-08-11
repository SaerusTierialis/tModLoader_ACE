using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ACE.Systems.XPRewards
{
    class TileRewards : GlobalTile
    {
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Track Placed Tiles ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        public static List<Tuple<int, int>> Tiles_Placed { get; private set; } = new List<Tuple<int, int>>();

        private static bool TileIsPlaced(int x, int y)
        {
            return TileIsPlaced(new Tuple<int, int>(x, y));
        }
        private static bool TileIsPlaced(Tuple<int, int> position)
        {
            return Tiles_Placed.Contains(position);
        }
        public static void PlaceTile(int x, int y)
        {
            Tuple<int, int> position = new Tuple<int, int>(x, y);
            if (!TileIsPlaced(position))
            {
                Tiles_Placed.Add(position);
            }
        }
        private static void UnplaceTile(int x, int y)
        {
            Tuple<int, int> position = new Tuple<int, int>(x, y);
            Tiles_Placed.Remove(position);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);

            int type = Main.tile[i, j].type;
            if (GetValue(type) > 0)
            {
                PlaceTile(i, j);

                //no longer called on server, must relay to server (and then to other clients)
                if (ACE.NETMODE_CLIENT)
                {
                    Utilities.PacketHandler.ClientPlaceValueTile.Send(-1, LocalData.WHO_AM_I, i, j);
                }
            }
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Save/Load/Sync ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        public static List<int> GetCoordList()
        {
            List<int> tiles_placed = new List<int>();
            foreach (Tuple<int, int> tile in Tiles_Placed)
            {
                tiles_placed.Add(tile.Item1);
                tiles_placed.Add(tile.Item2);
            }
            return tiles_placed;
        }

        public static void SetTilesPlaced(List<int> coords)
        {
            //reinit
            Tiles_Placed = new List<Tuple<int, int>>();

            //add each pair
            int count = 0;
            for (int i = 0; i < (coords.Count / 2.0); i++)
            {
                PlaceTile(coords[count++], coords[count++]);
            }
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Tile Value ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        private static Dictionary<int, float> _value_exceptions = new Dictionary<int, float>()
        {
            [TileID.DemonAltar] = 300.0f / 4.0f,    //triggers 4 times (includes crimson variant)
            [TileID.ShadowOrbs] = 50.0f / 4.0f,     //triggers 4 times (includes crimson variant)
            [TileID.Heart] = 20.0f / 4.0f,     //triggers 4 times
            [TileID.LifeFruit] = 30.0f / 4.0f,     //triggers 4 times
            [TileID.Larva] = 75.0f / 4.0f,     //triggers 4 times
            [TileID.BloomingHerbs] = 10,
            [TileID.MatureHerbs] = 2,
            [TileID.DesertFossil] = 4,
            [TileID.Amethyst] = 5,
            [TileID.Topaz] = 6,
            [TileID.Sapphire] = 7,
            [TileID.Emerald] = 8,
            [TileID.Ruby] = 9,
            [TileID.Diamond] = 15,
            //TODO 1.4 amber tile (doesn't exist yet)
        };

        private static float GetValue(int type)
        {
            float value = 0;

            if (_value_exceptions.ContainsKey(type))
                value = _value_exceptions[type];
            else if (IsTreeTile(type))
                value = 0.1f;
            else if (IsPotTile(type))
                value = 3.0f;
            else if (IsOreTile(type))
                value = Math.Max(2.0f, Main.tileValue[type] / 100.0f);

            return value;
        }

        private static bool IsTreeTile(int type)
        {
            return Main.tileAxe[type];
        }

        /// <summary>
        /// True if ore or ore-like (e.g., hellstone). False for gems, containers, pots, etc.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsOreTile(int type)
        {
            switch (type)
            {
                case TileID.Hellstone:
                    return true;
                case TileID.Meteorite:
                    return true;
                case TileID.Crimtane:
                    return true;
                case TileID.Demonite:
                    return true;

                default:
                    return Main.tileSpelunker[type] && !Main.tileContainer[type] && !IsPotTile(type); //&& Main.tileMergeDirt[type];
            }
        }

        private static bool IsPotTile(int type)
        {
            return (type == TileID.Pots);
        }

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Rewards ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        private static double time_last_pot = 0;

        //didn't use Drop because it is only called by server in MP
        //KillTile is called by server and all clients
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);

            if (!fail && !effectOnly && !noItem)
            {
                float value = GetValue(type);
                if (value > 0)
                {
                    //placed tile? (client and server)
                    if (TileIsPlaced(i,j))
                    {
                        //was placed, just unplace
                        UnplaceTile(i, j);
                        return;
                    }

                    //nothing else to do if server
                    if (ACE.NETMODE_SERVER)
                    {
                        return;
                    }

                    //prevent multi-xp from pots (client)
                    if (IsPotTile(type))
                    {
                        if (Main.time == time_last_pot)
                            return;
                        else
                            time_last_pot = Main.time;
                    }

                    //give xp (client)
                    Rewards.GiveXP(value, Main.LocalPlayer.getRect());
                }
            }
        }
    }
}
