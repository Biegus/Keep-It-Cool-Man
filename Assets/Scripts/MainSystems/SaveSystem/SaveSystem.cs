using System;
using System.IO;
using System.Runtime.Serialization;
using Cyberultimate.Unity;
using UnityEngine;
namespace LetterBattle
{
    [CommandContainer]
    public static class SaveSystem
    {
        [CommandProperty(get: true,set:false,name:"state_path")]
        public static string PATH { get; } = Path.Combine(StoreSystem.PATH, "Progress.xml");
        private static DataContractSerializer serializer;
        static SaveSystem()
        {
             serializer = new DataContractSerializer(typeof(PlayerState));
             Directory.CreateDirectory(Path.GetDirectoryName(PATH));
        }

        public static PlayerState Load()
        {
            if (!File.Exists(PATH)) return null;
            FileStream writer=null;
            try
            {
                writer = new FileStream(PATH, FileMode.Open);
                PlayerState state= (PlayerState) serializer.ReadObject(writer);
                return state;
            }
            catch (Exception exception)
            {
                Debug.LogError("During saving:");
                Debug.LogException(exception);
                return null;

            }
            finally
            {
                writer?.Dispose();
            }
        }
        public static void Save(PlayerState obj)
        {
            
            FileStream writer=null;
            try
            {
                writer = new FileStream(PATH, FileMode.Create);
                serializer.WriteObject(writer, obj);
            }
            catch (Exception exception)
            {
                Debug.LogError("During saving:");
                Debug.LogException(exception);

            }
            finally
            {
                writer?.Dispose();
            }
        }
        
        public static void Delete()
        {
            File.Delete(PATH);

        }
    }
}