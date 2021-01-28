using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace EasyCraftPlugin
{
    public class Settings
    {
        public static Type PluginCallback;
        public static string key;
    }

    class PluginCall
    {
        public static dynamic Call(string type, Dictionary<string, string> data)
        {
            return Settings.PluginCallback.GetMethod("Handle")
                ?.Invoke(null, new object[]
                {
                    (object) new PluginCallData
                    {
                        pluginid = Plugin.id,
                        key = Settings.key,
                        type = type,
                        data = data
                    }
                });
        }
    }

    public struct PluginCallData
    {
        public string pluginid;
        public string key;
        public string type;
        public Dictionary<string, string> data;
    }

    class Server
    {
        public bool isnull = true;

        // Server Basic Info Start
        public int Id;
        public string Name;
        public string Core;
        public int Owner;
        public bool Running;
        public int Maxplayer;
        public int Port;

        public DateTime Expiretime;
        // Server Basic Info End

        public Server(int sid)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["sid"] = sid.ToString();
            dynamic s = PluginCall.Call("Server.GetBasicInfo", a);
            if (s is null)
            {
                isnull = true;
                return;
            }

            isnull = false;
            Core = s.Core;
            Expiretime = s.Expiretime;
            Id = s.Id;
            Maxplayer = s.Maxplayer;
            Name = s.Name;
            Owner = s.Owner;
            Port = s.Port;
            Running = s.Running;
        }

        public bool SendCommand(string cmd)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args["cmd"] = cmd;
            return PluginCall.Call("Server.SendCommand",args);
        }
    }

    public class FastConsole
    {
        public static void PrintInfo(string message)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["message"] = message;
            PluginCall.Call("FastConsole.PrintInfo", a);
        }

        public static void PrintTrash(string message)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["message"] = message;
            PluginCall.Call("FastConsole.PrintTrash", a);
        }

        public static void PrintSuccess(string message)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["message"] = message;
            PluginCall.Call("FastConsole.PrintSuccess", a);
        }

        public static void PrintWarning(string message)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["message"] = message;
            PluginCall.Call("FastConsole.PrintWarning", a);
        }

        public static void PrintError(string message)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["message"] = message;
            PluginCall.Call("FastConsole.PrintError", a);
        }

        public static void PrintFatal(string message)
        {
            Dictionary<string, string> a = new Dictionary<string, string>();
            a["message"] = message;
            PluginCall.Call("FastConsole.PrintFatal", a);
        }
    }

    public struct PluginInfo
    {
        public string id;
        public string name;
        public string author;
        public string link;
        public string description;
        public string[] hooks;
        public string[] auth;
    }
}