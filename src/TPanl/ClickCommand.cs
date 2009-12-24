using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TPanl
{
    public class ClickCommand : Command
    {
        private readonly string _button;
        private readonly ClickCommandMap _commandMap; 

        public ClickCommand(ClickCommandMap commandMap, string button)
        {
            _commandMap = commandMap; 
            _button = button; 
        }

        public override void Execute(MainForm window)
        {
            KeyEventSequence keys = LookupKeys(_button);

            if (keys != null)
            {
                window.SendKeys(keys);
            }
        }

        private KeyEventSequence LookupKeys(string command)
        {
            if (_commandMap.ContainsKey(command))
            {
                return _commandMap[command];
            }
            else
            {
                return null; 
            }
        }

    }
}
