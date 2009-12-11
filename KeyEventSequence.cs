using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl
{
    public class KeyEventSequence : List<KeyEvent>
    {
        internal Win32.INPUT[] ToInputArray()
        {
            Win32.INPUT[] inputs = new Win32.INPUT[this.Count];

            for (int i = 0; i < Count; i++)
            {
                inputs[i] = this[i].ToInput(); 
            }

            return inputs; 
        }

        public override string ToString()
        {
            return string.Join(", ",
                (from item
                 in this
                 select item.ToString()).ToArray()); 
        }
    }
}
