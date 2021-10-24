using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameManager
{
    public const string WORLDS_PATH = "/Worlds/";
    public const string PLAYERS_PATH = "/Players/";
    public const string WORLD_FILE_TYPE = ".wld";
    public const string PLAYER_FILE_TYPE = ".plr";

    public static StreamData Stream => _stream;

    private static StreamData _stream = new StreamData();
    private static InitalizeGame _initalizeGame = new InitalizeGame();

    private static HashSet<IStreamer> _streamers = new HashSet<IStreamer>();

    public static bool AddStreamer(IStreamer streamer)
    {
        streamer.UpLoad();
        return _streamers.Add(streamer);
    }

    public static bool RemoveStreamer(IStreamer streamer) => _streamers.Remove(streamer);

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
        foreach (IStreamer streamer in _streamers)
            streamer.BeforeSaving();

        byte[] bytes = SerializationUtility.SerializeValue(_stream.StreamedData, DataFormat.Binary);
        File.WriteAllBytes(SaveWorldPath + "debug" + WORLD_FILE_TYPE, bytes);
    }

    public static void LoadGame()
    {

        byte[] bytes = File.ReadAllBytes(SaveWorldPath + "debug" + WORLD_FILE_TYPE);
        _stream.StreamedData = (Dictionary<Guid, object>)SerializationUtility.DeserializeValueWeak(bytes, DataFormat.Binary);

        foreach (IStreamer streamer in _streamers)
            streamer.Load();
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

        private Dictionary<Guid, object> _streamedData = new Dictionary<Guid, object>();

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

            return true;
        }

        public object Get(Guid guid)
        {
            _streamedData.TryGetValue(guid, out object obj);

            return obj;
        }
    }
}
