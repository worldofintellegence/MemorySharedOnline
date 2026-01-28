using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using NetcodePlus;


namespace SurvivalEngine
{
    /// <summary>
    /// Tool to serialize / deserialize PlayerData and WorldData for the cloud save
    /// This is overly complicated but Unity Services doesn't allow a field to be more than 16Kb (which is tiny)
    /// So the save file needs to be split/divided into so many sections
    /// For regular save-to-file to your pc you can just use NetworkTool.Serialize instead and it will be much simpler
    /// But for cloud save need to go through these insane steps
    /// This may not be complete and you may need to edit this script if you change the data inside PlayerData or WorldData
    /// </summary>

    public class CloudSaveTool
    {

        public static async Task<PlayerData> LoadPlayer(string file)
        {
            if (!SaveTool.IsValidFilename(file))
                return null;

            HashSet<string> keys = await GetPlayerKeys(file);
            Dictionary<string, string> dict = await CloudSave.LoadDict(keys);
            PlayerData player = DictToPlayer(file, dict);
            if (player != null)
                player.filename = file;
            return player;
        }

        public static async Task<bool> SavePlayer(string file, PlayerData data)
        {
            if (!SaveTool.IsValidFilename(file))
                return false;

            if (data != null)
            {
                data.filename = file;
                Dictionary<string, object> dict = PlayerToDict(file, data);
                return await CloudSave.SaveDictBatch(dict);
            }
            return false;
        }

        public static async Task<bool> DeletePlayer(string file)
        {
            HashSet<string> keys = await GetPlayerKeys(file);
            return await CloudSave.DeleteAll(keys);
        }

        //------------------------

        public static async Task<WorldData> LoadWorld(string file)
        {
            if (!SaveTool.IsValidFilename(file))
                return null;

            HashSet<string> keys = await GetWorldKeys(file);
            Dictionary<string, string> dict = await CloudSave.LoadDict(keys);
            WorldData world = DictToWorld(file, dict);
            if (world != null)
                world.filename = file;
            return world;
        }

        public static async Task<bool> SaveWorld(string file, WorldData data)
        {
            if (!SaveTool.IsValidFilename(file))
                return false;

            if (data != null)
            {
                data.filename = file;
                Dictionary<string, object> dict = WorldToDict(file, data);
                return await CloudSave.SaveDictBatch(dict);
            }
            return false;
        }

        public static async Task<bool> DeleteWorld(string file)
        {
            HashSet<string> keys = await GetWorldKeys(file);
            return await CloudSave.DeleteAll(keys);
        }

        //-----------------------

        public static async Task<HashSet<string>> GetPlayerKeys(string file)
        {
            string skeys = await CloudSave.LoadString("player_" + file);
            KeyList keys = WebTool.JsonToObject<KeyList>(skeys);
            HashSet<string> set = keys != null ? new HashSet<string>(keys.keys) : null;
            if(set != null)
                set.Add("player_" + file); //Add keys set
            return set;
        }

        private static Dictionary<string, object> PlayerToDict(string file, PlayerData player)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            WritePlayer("player_" + file + "_core", dict, player);

            //Add keys
            KeyList keys = GetKeys(dict);
            string skeys = WebTool.ToJson(keys);
            dict.Add("player_" + file, skeys);

            return dict;
        }

        private static PlayerData DictToPlayer(string file, Dictionary<string, string> dict)
        {
            if (dict == null)
                return null;

            PlayerData player = ReadPlayer("player_" + file + "_core", dict);
            return player;
        }

        //------------------------

        public static async Task<HashSet<string>> GetWorldKeys(string file)
        {
            string skeys = await CloudSave.LoadString("world_" + file);
            KeyList keys = WebTool.JsonToObject<KeyList>(skeys);
            HashSet<string> set = keys != null ? new HashSet<string>(keys.keys) : null;
            if (set != null)
                set.Add("world_" + file); //Add keys set
            return set;
        }

        private static Dictionary<string, object> WorldToDict(string file, WorldData world)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            WriteWorld("world_" + file, dict, world);

            //Add keys
            KeyList keys = GetKeys(dict);
            string skeys = WebTool.ToJson(keys);
            dict.Add("world_" + file, skeys);

            return dict;
        }

        private static WorldData DictToWorld(string file, Dictionary<string, string> dict)
        {
            if (dict == null)
                return null;

            WorldData world = ReadWorld("world_" + file, dict);
            return world;
        }

        //---------------

        private static void WriteWorld(string wkey, Dictionary<string, object> dict, WorldData world)
        {
            WorldDataCore core = GetWorldCore(world);
            dict.Add(wkey + "_core", NetworkTool.SerializeToString(core));
            foreach (KeyValuePair<int, PlayerData> pair in world.players)
            {
                WritePlayer(wkey + "_player_" + pair.Key, dict, pair.Value);
            }
            int iinv = 0;
            foreach (KeyValuePair<string, InventoryData> pair in world.inventories)
            {
                WriteData(wkey + "_inv_" + iinv, dict, pair.Value);
                iinv++;
            }

            WriteDictData(wkey + "_construct", dict, world.built_constructions);
            WriteDictData(wkey + "_plant", dict, world.sowed_plants);
            WriteDictData(wkey + "_item", dict, world.dropped_items);
            WriteDictData(wkey + "_character", dict, world.trained_characters);
            WriteDictData(wkey + "_spawn", dict, world.spawned_objects);
            WriteDictData(wkey + "_scene", dict, world.scene_objects);
            WriteDictData(wkey + "_regrow", dict, world.world_regrowth);

            WriteDictInt(wkey + "_ids", dict, world.unique_ids);
            WriteDictFloat(wkey + "_float", dict, world.unique_floats);
            WriteDictString(wkey + "_string", dict, world.unique_strings);
            WriteDictInt(wkey + "_robj", dict, world.removed_objects);
        }

        private static WorldData ReadWorld(string wkey, Dictionary<string, string> dict)
        {
            WorldDataCore core = ReadData<WorldDataCore>(wkey + "_core", dict);
            if (core != null)
            {
                WorldData world = GetWorld(core);
                for (int i = 0; i < core.player_count; i++)
                {
                    PlayerData player = ReadPlayer(wkey + "_player_" + i, dict);
                    if (player != null)
                        world.players.Add(i, player);
                }
                for (int i = 0; i < core.inventory_count; i++)
                {
                    InventoryData inv = ReadData<InventoryData>(wkey + "_inv_" + i, dict);
                    if (inv != null)
                        world.inventories.Add(inv.uid, inv);
                }

                world.built_constructions = ReadDictData<BuiltConstructionData>(wkey + "_construct", dict);
                world.sowed_plants = ReadDictData<SowedPlantData>(wkey + "_plant", dict);
                world.dropped_items = ReadDictData<DroppedItemData>(wkey + "_item", dict);
                world.trained_characters = ReadDictData<TrainedCharacterData>(wkey + "_character", dict);
                world.spawned_objects = ReadDictData<SpawnedData>(wkey + "_spawn", dict);
                world.scene_objects = ReadDictData<SceneObjectData>(wkey + "_scene", dict);
                world.world_regrowth = ReadDictData<RegrowthData>(wkey + "_regrow", dict);

                world.unique_ids = ReadDictInt(wkey + "_ids", dict);
                world.unique_floats = ReadDictFloat(wkey + "_float", dict);
                world.unique_strings = ReadDictString(wkey + "_string", dict);
                world.removed_objects = ReadDictInt(wkey + "_robj", dict);

                return world;
            }
            return null;
        }

        private static void WritePlayer(string pkey, Dictionary<string, object> dict, PlayerData player)
        {
            PlayerDataCore core = GetPlayerCore(player);
            dict.Add(pkey, NetworkTool.SerializeToString(core));
            int iinv = 0;
            foreach (KeyValuePair<string, InventoryData> pair in player.inventories)
            {
                WriteData(pkey + "_inv_" + iinv, dict, pair.Value);
                iinv++;
            }

            DictAttr adict = new DictAttr();
            adict.dict = player.attributes;
            WriteData(pkey + "_attr", dict, adict);

            WriteDictData(pkey + "_level", dict, player.levels);

            WriteDictInt(pkey + "_craft", dict, player.crafted_count);
            WriteDictInt(pkey + "_kill", dict, player.kill_count);
            WriteDictInt(pkey + "_unlock", dict, player.unlocked_ids);
        }

        private static PlayerData ReadPlayer(string pkey, Dictionary<string, string> dict)
        {
            PlayerDataCore pcore = ReadData<PlayerDataCore>(pkey, dict);
            if (pcore != null)
            {
                PlayerData player = GetPlayer(pcore);
                for (int i = 0; i < pcore.inventory_count; i++)
                {
                    InventoryData inv = ReadData<InventoryData>(pkey + "_inv_" + i, dict);
                    if (inv != null)
                        player.inventories.Add(inv.uid, inv);
                }

                DictAttr attr = ReadData<DictAttr>(pkey + "_attr", dict);
                player.attributes = attr?.dict;

                player.levels = ReadDictData< PlayerLevelData >(pkey + "_level", dict);

                player.crafted_count = ReadDictInt(pkey + "_craft", dict);
                player.kill_count = ReadDictInt(pkey + "_kill", dict);
                player.unlocked_ids = ReadDictInt(pkey + "_unlock", dict);

                return player;
            }
            return null;
        }

        private static T ReadData<T>(string key, Dictionary<string, string> dict) where T : class
        {
            bool success = dict.TryGetValue(key, out string sval);
            if (success)
            {
                T data = NetworkTool.DeserializeFromString<T>(sval);
                return data;
            }
            return null;
        }

        private static void WriteData<T>(string pkey, Dictionary<string, object> dict, T data) where T : class
        {
            dict.Add(pkey, NetworkTool.SerializeToString<T>(data));
        }

        private static Dictionary<string, int> ReadDictInt(string dkey, Dictionary<string, string> dict)
        {
            DictInt tdict = ReadData<DictInt>(dkey, dict);
            return tdict?.dict;
        }

        private static void WriteDictInt(string dkey, Dictionary<string, object> dict, Dictionary<string, int> tdict)
        {
            DictInt idict = new DictInt();
            idict.dict = tdict;
            WriteData<DictInt>(dkey, dict, idict);
        }

        private static Dictionary<string, float> ReadDictFloat(string dkey, Dictionary<string, string> dict)
        {
            DictFloat tdict = ReadData<DictFloat>(dkey, dict);
            return tdict?.dict;
        }

        private static void WriteDictFloat(string dkey, Dictionary<string, object> dict, Dictionary<string, float> tdict)
        {
            DictFloat idict = new DictFloat();
            idict.dict = tdict;
            WriteData<DictFloat>(dkey, dict, idict);
        }

        private static Dictionary<string, string> ReadDictString(string dkey, Dictionary<string, string> dict)
        {
            DictString tdict = ReadData<DictString>(dkey, dict);
            return tdict?.dict;
        }

        private static void WriteDictString(string dkey, Dictionary<string, object> dict, Dictionary<string, string> tdict)
        {
            DictString idict = new DictString();
            idict.dict = tdict;
            WriteData<DictString>(dkey, dict, idict);
        }

        private static Dictionary<string, T> ReadDictData<T>(string dkey, Dictionary<string, string> dict) where T : class
        {
            DictData<T> tdict = ReadData<DictData<T>>(dkey, dict);
            return tdict?.dict;
        }

        private static void WriteDictData<T>(string dkey, Dictionary<string, object> dict, Dictionary<string, T> tdict) where T : class
        {
            DictData<T> idict = new DictData<T>();
            idict.dict = tdict;
            WriteData(dkey, dict, idict);
        }

        //---------------

        public static KeyList GetKeys(Dictionary<string, object> dict)
        {
            KeyList list = new KeyList();
            list.keys = new string[dict.Count];
            int index = 0;
            foreach (KeyValuePair<string, object> pair in dict)
            {
                list.keys[index] = (string) pair.Key;
                index++;
            }
            return list;
        }

        private static PlayerDataCore GetPlayerCore(PlayerData player)
        {
            PlayerDataCore core = new PlayerDataCore();
            core.filename = player.filename;
            core.player_id = player.player_id;
            core.username = player.username;
            core.character = player.character;
            core.position = player.position;
            core.scene = player.scene;
            core.world = player.world;
            core.gold = player.gold;
            core.inventory_count = player.inventories.Count;
            return core;
        }

        private static PlayerData GetPlayer(PlayerDataCore core)
        {
            PlayerData player = new PlayerData(core.filename);
            player.player_id = core.player_id;
            player.username = core.username;
            player.character = core.character;
            player.position = core.position;
            player.scene = core.scene;
            player.world = core.world;
            player.gold = core.gold;
            player.FixData();
            return player;
        }

        private static WorldDataCore GetWorldCore(WorldData world)
        {
            WorldDataCore core = new WorldDataCore();
            core.filename = world.filename;
            core.version = world.version;
            core.last_save = world.last_save;
            core.world_id = world.world_id;
            core.world_seed = world.world_seed;
            core.scene = world.scene;
            core.entry = world.entry;
            core.day = world.day;
            core.day_time = world.day_time;
            core.play_time = world.play_time;
            core.new_day = world.new_day;
            core.player_count = world.players.Count;
            core.inventory_count = world.inventories.Count;
            return core;
        }

        private static WorldData GetWorld(WorldDataCore core)
        {
            WorldData world = new WorldData(core.filename);
            world.version = core.version;
            world.last_save = core.last_save;
            world.world_id = core.world_id;
            world.world_seed = core.world_seed;
            world.scene = core.scene;
            world.entry = core.entry;
            world.day = core.day;
            world.day_time = core.day_time;
            world.play_time = core.play_time;
            world.new_day = core.new_day;
            world.FixData();
            return world;
        }
    }

    [System.Serializable]
    public class PlayerDataCore
    {
        public string filename;     //File name, usually same as username

        public int player_id;       //Player ID assigned to player by NetcodePlus (usually 0 is the host)
        public string user_id = ""; //Authentication ID of the player
        public string username = ""; //Username of the player
        public string character = ""; //Character prefab selected by player

        public Vector3Data position;   //Saved position within scene, if PlayerData scene/world dont match WorldData scene/world, this will be ignored
        public string scene;           //Save last visited scene, if it doesn't match the current scene, will go to default spawn location instead of position
        public ulong world;            //World id also need to match this to spawn at position (otherwise go to default spawn pos)
        public int gold = 0;

        public int inventory_count;
    }

    [System.Serializable]
    public class WorldDataCore
    {
        public string filename;
        public string version;
        public System.DateTime last_save;

        public ulong world_id;        //Random number differentiating each world file
        public int world_seed = 0;  //Randomly Generated world
        public string scene;        //Current scene
        public string entry = "";   //If empty, will spawn at default spawning position

        public int day = 0;
        public float day_time = 0f; // 0 = midnight, 24 = end of day
        public float play_time = 0f; //total play time in actual seconds
        public bool new_day = false;

        public int player_count;
        public int inventory_count;
    }

    [System.Serializable]
    public class DictInt
    {
        public Dictionary<string, int> dict;
    }

    [System.Serializable]
    public class DictFloat
    {
        public Dictionary<string, float> dict;
    }

    [System.Serializable]
    public class DictString
    {
        public Dictionary<string, string> dict;
    }

    [System.Serializable]
    public class DictAttr
    {
        public Dictionary<AttributeType, float> dict;
    }

    [System.Serializable]
    public class DictData<T> where T : class
    {
        public Dictionary<string, T> dict;
    }

    [System.Serializable]
    public class KeyList
    {
        public string[] keys;
    }
}
