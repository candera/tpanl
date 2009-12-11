using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl
{
    public class ClickCommandFactory : CommandFactory
    {
        private readonly ClickCommandMap _commandMap;

        public ClickCommandFactory(ClickCommandMap commandMap)
        {
            _commandMap = commandMap;
        }

        public override Command Create(string[] parameters)
        {
            if (parameters.Length > 1)
            {
                return null; 
            }
            
            return new ClickCommand(_commandMap, parameters[0]); 
        }
    }
}
