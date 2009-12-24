using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TPanl
{
    public abstract class Command
    {
        private static readonly Dictionary<string, CommandFactory> s_commandMap = new Dictionary<string, CommandFactory>(StringComparer.InvariantCultureIgnoreCase); 

        public abstract void Execute(MainForm window);
        public static Command Parse(string line)
        {
            string[] parts = line.Split(' ');
            if (parts.Length == 0)
            {
                return null; 
            }

            string verb = parts[0]; 
            string[] parameters = null; 

            if (parts.Length > 1)
            {
                parameters = new string[parts.Length - 1];
                Array.Copy(parts, 1, parameters, 0, parts.Length - 1); 
            }

            if (!s_commandMap.ContainsKey(parts[0]))
            {
                return null; 
            }
            else
            {
                return s_commandMap[verb].Create(parameters); 
            }
        }
        public static void Register(string verb, CommandFactory factory)
        {
            s_commandMap[verb] = factory; 
        }

        internal static void UnregisterAll()
        {
            s_commandMap.Clear(); 
        }
    }
}
