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
    public const string WORLD_FILE_TYPE = ".wld";
    public const string PLAYER_FILE_TYPE = ".plr";

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
       byte[] bytes = SerializationUtility.SerializeValueWeak(_stream.StreamedData, DataFormat.Binary);
       File.WriteAllBytes(SaveWorldPath + "debug" + WORLD_FILE_TYPE, bytes);
    }

    public static void LoadGame()
    {

        byte[] bytes = File.ReadAllBytes(SaveWorldPath + "debug" + WORLD_FILE_TYPE);
        _stream.StreamedData = (Dictionary<Guid, object>)SerializationUtility.DeserializeValueWeak(bytes, DataFormat.Binary);

        StreamableBehaviour[] streamableBehaviours = UnityEngine.Object.FindObjectsOfType<StreamableBehaviour>();
        for (int i = 0; i < streamableBehaviours.Length; i++)
        {
            streamableBehaviours[i].Load();
        }
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
        public Dictionary<Guid, object> StreamedData
        {
            get => _streamedData;
            set => _streamedData = value;
        }

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
