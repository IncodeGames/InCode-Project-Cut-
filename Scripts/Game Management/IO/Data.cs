using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;


namespace GenericData
{

    /// Generic Data class
    public static class Data
    {

        /// <summary>Save Generic Data.
        /// <para>Save file as Object in Persistent Data Path. <see cref="UnityEngine.Application.persistentDataPath"/> for more information.</para>
        /// </summary>
        public static bool SavePDP(System.Object data, string fileName) { return Save(data, Application.persistentDataPath + fileName); }
        /// <summary>Save Generic Data.
        /// <para>Save file as Object in custom Path.</para>
        /// </summary>
        public static bool Save(System.Object data, string pathFileName)
        {

            FileStream file;
            
            try { file = File.Create(pathFileName); }
            catch { Debug.Log("Failed to create file."); return false; }

            BinaryFormatter bf = new BinaryFormatter();

            try { bf.Serialize(file, data); }
            catch
            {

                file.Close();
                File.Delete(pathFileName);
                Debug.Log("Failed to serialize file.");
                return false;

            }
            Debug.Log("File saved Successfully");
            file.Close();
            return true;

        }

        /// <summary>Load Generic Data.
        /// <para>Load file as Object from Persistent Data Path. <see cref="UnityEngine.Application.persistentDataPath"/> for more information.</para>
        /// </summary>
        public static System.Object LoadPDP(string fileName) { return Load(Application.persistentDataPath + fileName); }
        /// <summary>Load Generic Data.
        /// <para>Load file as Object from custom Path.</para>
        /// </summary>
        public static System.Object Load(string pathFileName)
        {

            if (!File.Exists(pathFileName))
            {
                Debug.Log("No file of given name exists.");
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(pathFileName, FileMode.Open);

            System.Object data;

            try { data = bf.Deserialize(file); }
            catch
            {

                file.Close();
                return null;

            }
            Debug.Log("File loaded Successfully");
            file.Close();
            return data;

        }

    }

}