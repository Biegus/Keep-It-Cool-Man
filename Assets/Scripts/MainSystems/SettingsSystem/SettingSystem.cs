using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

namespace LetterBattle
{
    public static class SettingSystem
    {
        public static readonly string PATH = Path.Combine(StoreSystem.PATH, "Settings.xml");
        private static DataContractSerializer serializer=new DataContractSerializer(typeof(SettingsData));

        public static SettingsData Current { get; private set; } = new SettingsData();
        private static readonly XmlWriterSettings writingSettings = new XmlWriterSettings { Indent = true };

        
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (SettingSystem.TryLoad())
            {
                Debug.Log("Settings file found");
            }
            SettingSystem.Save();
        }
        public static void Save()
        {
            using XmlWriter writer = XmlWriter.Create(PATH,writingSettings);
            
            serializer.WriteObject(writer, Current);
        }
        public static void Apply()
        {
            Current.Apply();
        }
        public static bool TryLoad()
        {
            if (!File.Exists(PATH))
            {
                return false;
            }
            using (FileStream reader = new FileStream(PATH, FileMode.Open))
            {
                try
                {
                    Current = (SettingsData) serializer.ReadObject(reader);

                }
                catch(Exception exception)
                {
                    Debug.LogError($"Deserializing failed {exception}");
                    return false;
                }
            }
            Current.Apply();
            return true;
        }
    }
}