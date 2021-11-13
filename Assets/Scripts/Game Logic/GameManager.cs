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
    public const string SAVES_PATH = "/Saves/";
    public const string SAVE_FILE_TYPE = ".sav";

    public static string SAVE_NAME = string.Empty;

    public static StreamData Stream => _stream;

    private static StreamData _stream = new StreamData();
    private static InitializeGame _initializeGame = new InitializeGame();

    private static HashSet<IStreamer> _streamers = new HashSet<IStreamer>();

    public static bool AddStreamer(IStreamer streamer)
    {
        streamer.UpLoad();
        return _streamers.Add(streamer);
    }

    public static bool RemoveStreamer(IStreamer streamer) => _streamers.Remove(streamer);


    public static string SavePath
    {
        get
        {
#if UNITY_STANDALONE_WIN
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/My Games/" + Application.productName + SAVES_PATH;
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
        File.WriteAllBytes(SavePath + SAVE_NAME + SAVE_FILE_TYPE, bytes);
    }

    public static void LoadGame()
    {
        byte[] bytes = File.ReadAllBytes(SavePath + SAVE_NAME + SAVE_FILE_TYPE);
        _stream.StreamedData = (Dictionary<Guid, object>)SerializationUtility.DeserializeValueWeak(bytes, DataFormat.Binary);

        if (_stream.StreamedData == null) // if failed to load
            _stream.StreamedData = new Dictionary<Guid, object>();

        foreach (IStreamer streamer in _streamers)
            streamer.Load();
    }

    public static string[] GetSaves()
    {
        return Directory.GetFiles(SavePath, string.Format("*{0}", SAVE_FILE_TYPE));
    }
    public static bool SaveExists(string fileName)
    {
        return File.Exists(SavePath + fileName + SAVE_FILE_TYPE);
    }

    /// <summary>
    /// Creates an empty save and also overrides the current one with the same name if found
    /// </summary>
    public static void CreateSave(string fileName)
    {
        try
        {
            File.Create(SavePath + fileName + SAVE_FILE_TYPE).Close();
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Deletes specified save if found
    /// </summary>
    public static void DeleteSave(string fileName)
    {
        try
        {
            File.Delete(SavePath + fileName + SAVE_FILE_TYPE);
        } 
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public class InitializeGame
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            _initializeGame.LoadOrder();
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

        public bool TryGet<T>(Guid guid, out T value) where T : class
        {
            bool result = _streamedData.TryGetValue(guid, out object obj);

            if ((value = (T)obj) == null)
                return false;

            return result;
        }
    }
}
