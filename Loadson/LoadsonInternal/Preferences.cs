using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace LoadsonInternal
{
    public static class Preferences
    {
        public static void Load()
        {
            if(!PlayerPrefs.HasKey("LoadsonPrefs"))
            {
                instance = new _Save();
                Save();
            }
            instance = Deserialize<_Save>(PlayerPrefs.GetString("LoadsonPrefs"));
        }

        public static void Save()
        {
            PlayerPrefs.SetString("LoadsonPrefs", Serialize(instance));
        }

        public class _Save
        {
            public bool unityLog = true;
            public bool fileLog = true;
            public bool forceFpsAndSpeed = false;
        }
        public static _Save instance;

        // ripped straight from karlson
        public static string Serialize<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, toSerialize);
            return stringWriter.ToString();
        }
        public static T Deserialize<T>(string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }
    }
}
