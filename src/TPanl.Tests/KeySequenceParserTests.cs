using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TPanl;

namespace Wangdera.Keylay.Test
{
    [TestClass]
    public class KeySequenceParserTests
    {
        [TestMethod]
        public void SuccessCases()
        {
            Test("a", Keys.A);
            Test("3", Keys.D3);
            Test("^a", Keys.ControlKey, Keys.A);
            Test("+a", Keys.ShiftKey, Keys.A);
            Test("%c", Keys.Menu, Keys.C);
            Test("+^a", Keys.ShiftKey, Keys.ControlKey, Keys.A);
            Test("+ab",
                MakeExpectedKeySequence(Keys.ShiftKey, Keys.A),
                MakeExpectedKeySequence(Keys.B));
            Test("a{b}",
                MakeExpectedKeySequence(Keys.A),
                MakeExpectedKeySequence(Keys.B));
            Test("^%{NumPad0}", Keys.ControlKey, Keys.Menu, Keys.NumPad0);
            Test("{65}",
                new KeyEventSequence 
                {
                    new KeyEvent { Direction = KeyDirection.Down, ScanCode = 65 },
                    new KeyEvent { Direction = KeyDirection.Up, ScanCode = 65 }
                });
            Test("{0x0a}",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, ScanCode = 0x0a },
                    new KeyEvent { Direction = KeyDirection.Up, ScanCode = 0x0a },
                });
            Test("+^2", Keys.ShiftKey, Keys.ControlKey, Keys.D2); 
            Test("{divide.0xb5}",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, Key = Keys.Divide, ScanCode = 0xb5 },
                    new KeyEvent { Direction = KeyDirection.Up, Key = Keys.Divide, ScanCode = 0xb5 },
                });
            Test("{*divide}",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, Key = Keys.Divide, Extended = true },
                    new KeyEvent { Direction = KeyDirection.Up, Key = Keys.Divide, Extended = true },
                });

        }

        [TestMethod]
        public void NoSuchKey()
        {
            Assert.IsNull(KeySequenceParser.Parse("{nosuch}"));
            Assert.IsNull(KeySequenceParser.Parse("-"));
        }

        private void AssertEqual(KeyEventSequence expected, KeyEventSequence actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Checking that lengths are equal.");

            for (int i = 0; i < expected.Count; i++)
            {
                KeyEvent expectedEvent = expected[i];
                KeyEvent actualEvent = actual[i];

                AssertEqual(expectedEvent, actualEvent);
            }
        }
        private void AssertEqual(KeyEvent expectedEvent, KeyEvent actualEvent)
        {
            Assert.AreEqual(expectedEvent.ToString(), actualEvent.ToString(), "Checking that events are equal.");
        }
        private KeyEventSequence MakeExpectedKeySequence(params Keys[] keys)
        {
            KeyEventSequence sequence = new KeyEventSequence();
            for (int i = 0; i < keys.Length; i++)
            {
                sequence.Add(new KeyEvent { Direction = KeyDirection.Down, Key = keys[i] });
            }

            for (int i = keys.Length - 1; i >= 0; i--)
            {
                sequence.Add(new KeyEvent { Direction = KeyDirection.Up, Key = keys[i] });
            }

            return sequence;
        }
        private void Test(string input, params Keys[] keys)
        {
            AssertEqual(MakeExpectedKeySequence(keys), KeySequenceParser.Parse(input));
        }
        private void Test(string input, params KeyEventSequence[] expectedSequences)
        {
            KeyEventSequence expectedSequence = new KeyEventSequence();
            foreach (KeyEventSequence sequence in expectedSequences)
            {
                expectedSequence.AddRange(sequence);
            }

            AssertEqual(expectedSequence, KeySequenceParser.Parse(input));
        }

    }
}
