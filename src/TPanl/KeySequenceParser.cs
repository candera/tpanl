using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TPanl
{
    public static class KeySequenceParser
    {
        private class ParseNode
        {
            public readonly List<ParseNode> Children = new List<ParseNode>();
            public bool Extended { get; set; }
            public bool HasContent
            {
                get
                {
                    return Key.HasValue || ScanCode.HasValue; 
                }
            }
            public Keys? Key { get; set; }
            public ushort? ScanCode { get; set; }

            internal KeyEvent ToKeyEvent(KeyDirection keyDirection)
            {
                KeyEvent keyEvent = new KeyEvent();

                if (Key.HasValue)
                {
                    keyEvent.Key = Key.Value; 
                }

                if (ScanCode.HasValue)
                {
                    keyEvent.ScanCode = ScanCode.Value; 
                }

                keyEvent.Direction = keyDirection;
                keyEvent.Extended = Extended; 

                return keyEvent; 
            }
        }

        internal static KeyEventSequence Parse(string specification)
        {
            try
            {
                List<ParseNode> parseTree = new List<ParseNode>();
                ParseNode root = new ParseNode();
                ParseNode currentNode = root;
                for (int i = 0; i < specification.Length; )
                {
                    i += ParseAtom(specification, i, currentNode);
                }

                KeyEventSequence keyEventSequence = new KeyEventSequence();
                AddNode(keyEventSequence, root);
                return keyEventSequence;
            }
            catch (KeySpecificationParseException)
            {
                return null;
            }
        }

        private static void AddNode(KeyEventSequence keyEventSequence, ParseNode node)
        {
            if (node.HasContent)
            {
                keyEventSequence.Add(node.ToKeyEvent(KeyDirection.Down));
            }

            foreach (ParseNode child in node.Children)
            {
                AddNode(keyEventSequence, child);
            }

            if (node.HasContent)
            {
                keyEventSequence.Add(node.ToKeyEvent(KeyDirection.Up));
            }
        }
        private static int ParseAtom(string specification, int i, ParseNode currentNode)
        {
            int consumed = 0;
            char currentChar = specification[i];

            if (IsModifier(currentChar))
            {
                ParseNode newNode = new ParseNode { Key = GetModifier(currentChar) };
                currentNode.Children.Add(newNode);
                consumed = 1 + ParseAtom(specification, i + 1, newNode);
            }
            else if (currentChar == '{')
            {
                int end = specification.IndexOf('}', i + 1);
                if (end == -1)
                {
                    throw new KeySpecificationParseException("Mismatched braces.");
                }

                string keySpecification = specification.Substring(i + 1, end - i - 1);

                bool extended = false;
                if (keySpecification.StartsWith("*"))
                {
                    extended = true;
                    keySpecification = keySpecification.Substring(1); 
                }

                Keys? key = null;
                ushort? scanCode = null;
                if (char.IsDigit(keySpecification[0]))
                {
                    scanCode = ParseNumber(keySpecification);
                }
                else if (keySpecification.Contains("."))
                {
                    string[] parts = keySpecification.Split('.');

                    if (parts.Length != 2)
                    {
                        throw new KeySpecificationParseException("Key specification did not contain exactly two periods."); 
                    }

                    key = (Keys)Enum.Parse(typeof(Keys), parts[0], true);
                    scanCode = ParseNumber(parts[1]); 
                }
                else
                {
                    key = (Keys)Enum.Parse(typeof(Keys), keySpecification, true);
                }


                ParseNode newNode = new ParseNode { Key = key, ScanCode = scanCode, Extended = extended };
                currentNode.Children.Add(newNode);
                consumed = end - i + 1;
            }
            else if (currentChar >= '0' && currentChar <= '9')
            {
                Keys key = Keys.D0 + (currentChar - '0');
                ParseNode newNode = new ParseNode { Key = key };
                currentNode.Children.Add(newNode);
                consumed = 1;
            }
            else
            {
                Keys key = (Keys)Enum.Parse(typeof(Keys), currentChar.ToString(), true);

                ParseNode newNode = new ParseNode { Key = key };
                currentNode.Children.Add(newNode);
                consumed = 1;
            }

            return consumed;
        }
        private static ushort ParseNumber(string keySpecification)
        {
            ushort parsedScanCode;

            if (keySpecification.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!ushort.TryParse(
                        keySpecification.Substring(2),
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out parsedScanCode))
                {
                    throw new KeySpecificationParseException("Hex number in incorrect format.");
                }
            }
            else
            {
                if (!ushort.TryParse(keySpecification, out parsedScanCode))
                {
                    throw new KeySpecificationParseException("Decimal number in incorrect format.");
                }
            }
            return parsedScanCode;
        }
        private static Keys GetModifier(char c)
        {
            switch (c)
            {
                case '+':
                    return Keys.ShiftKey;
                case '%':
                    return Keys.Menu;
                case '^':
                    return Keys.ControlKey;
                default:
                    throw new KeySpecificationParseException("Unrecognized modifier character: " + c);
            }
        }

        private static bool IsModifier(char c)
        {
            if (c == '+' || c == '%' || c == '^')
            {
                return true;
            }

            return false;
        }

    }
}
