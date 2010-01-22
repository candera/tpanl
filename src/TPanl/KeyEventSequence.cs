using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPanl
{
    public class KeyEventSequence : List<KeyEvent>
    {
        internal Win32.INPUT32[] ToInputArray32()
        {
            Win32.INPUT32[] inputs = new Win32.INPUT32[this.Count];

            for (int i = 0; i < Count; i++)
            {
                inputs[i] = this[i].ToInput32();
            }

            return inputs;
        }
        internal Win32.INPUT64[] ToInputArray64()
        {
            Win32.INPUT64[] inputs = new Win32.INPUT64[this.Count];

            for (int i = 0; i < Count; i++)
            {
                inputs[i] = this[i].ToInput64();
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
