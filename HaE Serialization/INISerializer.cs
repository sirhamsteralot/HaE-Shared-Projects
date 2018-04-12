using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class INISerializer
        {
            public string SectionTag;

            private Dictionary<string, object> objectValues;
            private Dictionary<string, string> stringValues;
            private Dictionary<string, Func<string, object>> deserializers;

            public INISerializer(string SectionTag)
            {
                this.SectionTag = SectionTag;

                objectValues = new Dictionary<string, object>();
                stringValues = new Dictionary<string, string>();
                deserializers = new Dictionary<string, Func<string, object>>();
            }

            public void AddValue(string valueName, Func<string, object> deserializer, object value)
            {
                objectValues[valueName] = value;
                stringValues[valueName] = value.ToString();
                deserializers[valueName] = deserializer;
            }

            public object GetValue(string valueName)
            {
                return objectValues[valueName];
            }

            public void SetValue(string valueName, object value)
            {
                objectValues[valueName] = value;
                stringValues[valueName] = value.ToString();
            }

            public void FirstSerialization(ref string Data)
            {
                if (Data.Contains($"[{SectionTag}]"))
                    return;

                StringBuilder data = new StringBuilder();
                data.AppendLine($"\n[{SectionTag}]");

                foreach (var pair in stringValues)
                    data.AppendLine($"{pair.Key} = {pair.Value}");

                Data += data.ToString();
            }

            public void DeSerialize(string Data)
            {
                string[] SeperateLines = Data.Split('\n');
                bool mySection = false;

                for(int i = 0; i < SeperateLines.Length; i++)
                {
                    SeperateLines[i] = SeperateLines[i].Trim();

                    if (SeperateLines[i].StartsWith("["))
                    {
                        if (SeperateLines[i] == $"[{SectionTag}]")
                            mySection = true;
                        else
                            mySection = false;

                        continue;
                    }

                    if (mySection && SeperateLines[i] != "")
                    {
                        string[] keyValuePair = SeperateLines[i].Split('=');

                        stringValues[keyValuePair[0].Trim()] = keyValuePair[1].Trim();
                    }
                }

                foreach (var key in objectValues.Keys.ToList())
                {
                    objectValues[key] = deserializers[key].Invoke(stringValues[key]);
                }
            }

            public static string RemoveWhitespace(string input)
            {
                return new string(input.ToCharArray()
                    .Where(c => !Char.IsWhiteSpace(c))
                    .ToArray());
            }
        }
    }
}
