using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameManager
{
    public const string WORLDS_PATH = "/Worlds/";
    public const string PLAYERS_PATH = "/Players/";

    public static StreamData Stream => _stream;

    [NonSerialized]
    private static StreamData _stream = new StreamData();

    private static InitalizeGame _initalizeGame = new InitalizeGame();

    public static string SavePlayerPath
    {
        get
        {
#if UNITY_STANDALONE_WIN
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/My Games/" + Application.productName + PLAYERS_PATH;
                Directory.CreateDirectory(path);
                return path;
            }
            catch (Exception e)
            {
                throw e;
            }
#else
            try
            {
                string path = Application.persistentDataPath + PLAYERS_PATH;
                Directory.CreateDirectory(path);
                return path;
            }
            catch (Exception e)
            {
                throw e;
            }
#endif
        }
    }

    public static string SaveWorldPath
    {
        get
        {
#if UNITY_STANDALONE_WIN
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/My Games/" + Application.productName + WORLDS_PATH;
                Directory.CreateDirectory(path);
                return path;
            }
            catch (Exception e)
            {
                throw e;
            }
#else
            try
            {
                string path = Application.persistentDataPath + WORLD_PATH;
                Directory.CreateDirectory(path);
                return path;
            }
            catch (Exception e)
            {
                throw e;
            }
#endif
        }
    }

    public static void SaveGame()
    {
        //byte[] bytes = SerializationUtility.SerializeValueWeak(, DataFormat.Binary);
       // File.WriteAllBytes(SaveWorldPath + "hej.data", bytes);

    }

    public static void LoadGame()
    {

    }

    public class InitalizeGame
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initalize()
        {
            _initalizeGame.LoadOrder();
        }

        private void LoadOrder()
        {
            void LoadPlayerData()
            {

            }

            LoadPlayerData();
        }

    }

    public class StreamData
    {
        private HashSet<Guid> _dirtyData = new HashSet<Guid>();
        private Dictionary<Guid, object> _streamedData = new Dictionary<Guid, object>();

        public bool IsDirty(Guid guid) => _dirtyData.Contains(guid);

        public bool Load(Guid guid, object obj)
        {
            if (_streamedData.ContainsKey(guid))
                return false;

            _streamedData.Add(guid, obj);

            return true;
        }

        public bool Release(Guid guid)
        {
            if (!_streamedData.ContainsKey(guid))
                return false;

            _streamedData.Remove(guid);
            _dirtyData.Remove(guid);

            return true;
        }

        public object Get(Guid guid)
        {
            _streamedData.TryGetValue(guid, out object obj);

            return obj;
        }

        /// <summary>
        /// Set dirty to save the asset on next save.
        /// </summary>
        /// <returns>If streamed object exist.</returns>
        public bool SetDirty(Guid guid)
        {
            if (!_streamedData.ContainsKey(guid))
                return false;

            _dirtyData.Add(guid);

            return true;
        }
    }
}
