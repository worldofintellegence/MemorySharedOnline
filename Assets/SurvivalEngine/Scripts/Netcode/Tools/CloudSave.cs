using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

namespace NetcodePlus
{
    /// <summary>
    /// Functions to easily access cloud save system
    /// </summary>

    public class CloudSave
    {
        public static async Task<T> Load<T>(string key) where T : class
        {
            string sval = await LoadString(key);
            if (sval != null)
                return NetworkTool.DeserializeFromString<T>(sval);
            return null;
        }

        public static async Task<bool> Save<T>(string key, T data) where T : class
        {
            if (data != null)
            {
                string sval = NetworkTool.SerializeToString(data);
                return await SaveString(key, sval);
            }
            return false;
        }

        public static async Task<string> LoadString(string key)
        {
            HashSet<string> keys = new HashSet<string> { { key } };
            Dictionary<string, string> dict = await LoadDict(keys);
            bool succ = dict.TryGetValue(key, out string sval);
            return succ ? sval : null;
        }

        public static async Task<bool> SaveString(string key, string data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object> { { key, data } };
            return await SaveDict(dict);
        }

        public static async Task<bool> SaveObject(string key, object data)
        {
            Dictionary<string, object> dict = new Dictionary<string, object> { { key, data } };
            return await SaveDict(dict);
        }

        public static async Task<Dictionary<string, string>> LoadDict(HashSet<string> keys)
        {
            if (keys == null || !Authenticator.Get().IsConnected() || !Authenticator.Get().IsUnityServices())
                return null; //Not connected to unity services

            try
            {
                Dictionary<string, string> dict = await CloudSaveService.Instance.Data.LoadAsync(keys);
                return dict;

            }
            catch (CloudSaveException e)
            {
                Debug.LogError("Cloud Load Error: " + e);
            }
            return null;
        }

        public static async Task<bool> SaveDict(Dictionary<string, object> dict)
        {
            if (dict == null || !Authenticator.Get().IsConnected() || !Authenticator.Get().IsUnityServices())
                return false; //Not connected to unity services

            try
            {
                await CloudSaveService.Instance.Data.ForceSaveAsync(dict);
                return true;
            }
            catch (CloudSaveException e)
            {
                Debug.LogError("Cloud Save Error: " + e);
            }
            return false;
        }

        public static async Task<bool> SaveDictBatch(Dictionary<string, object> dict)
        {
            if (dict == null || !Authenticator.Get().IsConnected() || !Authenticator.Get().IsUnityServices())
                return false; //Not connected to unity services

            //Unity complains that batch size is too big with SaveDict(), so instead save one by one
            bool success = true;
            foreach (KeyValuePair<string, object> pair in dict)
            {
                bool s = await SaveObject(pair.Key, pair.Value); //Save one by one because unity dont let me (batch size too big error)
                success = success && s;
            }
            return success;
        }

        public static async Task<bool> DeleteAll(HashSet<string> keys)
        {
            if (keys == null)
                return false;

            bool success = true;
            foreach (string key in keys)
            {
                bool s = await Delete(key);
                success = success && s;
            }
            return success;
        }

        public static async Task<bool> Delete(string key)
        {
            if (!Authenticator.Get().IsConnected() || !Authenticator.Get().IsUnityServices())
                return false; //Not connected to unity services

            try
            {
                await CloudSaveService.Instance.Data.ForceDeleteAsync(key);
                return true;
            }
            catch (CloudSaveException e)
            {
                Debug.LogError("Cloud Delete Error: " + e);
            }
            return false;
        }
    }
}
