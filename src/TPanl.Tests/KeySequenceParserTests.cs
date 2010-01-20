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
        public void SingleAlphaParsesCorrectly()
        {
            Test("a", Keys.A);
        }
        [TestMethod]
        public void SingleDigitParsesCorrectly()
        {
            Test("3", Keys.D3);
        }
        [TestMethod]
        public void CaratForControlParsesCorrectly()
        {
            Test("^a", Keys.ControlKey, Keys.A);
        }
        [TestMethod]
        public void PlusForShiftParsesCorrectly()
        {
            Test("+a", Keys.ShiftKey, Keys.A);
        }
        [TestMethod]
        public void PercentForMenuParsesCorrectly()
        {
            Test("%c", Keys.Menu, Keys.C);
        }
        [TestMethod]
        public void TwoModifiersCombinedParseCorrectly()
        {
            Test("+^a", Keys.ShiftKey, Keys.ControlKey, Keys.A);
        }
        [TestMethod]
        public void ModifierOnlyModifiesFollowingItem()
        {
            Test("+ab",
                MakeExpectedKeySequence(Keys.ShiftKey, Keys.A),
                MakeExpectedKeySequence(Keys.B));
        }
        [TestMethod]
        public void CurlyBracesParseCorrectlyForSimpleKey()
        {
            Test("a{b}",
                MakeExpectedKeySequence(Keys.A),
                MakeExpectedKeySequence(Keys.B));
        }
        [TestMethod]
        public void CurlyBracesCombineCorrectlyWithMultipleModifiers()
        {
            Test("^%{NumPad0}", Keys.ControlKey, Keys.Menu, Keys.NumPad0);
        }
        [TestMethod]
        public void ScanCodesInsideCurlyBracesParseCorrectly()
        {
            Test("{65}",
                new KeyEventSequence 
                {
                    new KeyEvent { Direction = KeyDirection.Down, ScanCode = 65 },
                    new KeyEvent { Direction = KeyDirection.Up, ScanCode = 65 }
                });
        }
        [TestMethod]
        public void HexidecimalScanCodesInsideCurlyBracesParseCorrectly()
        {
            Test("{0x0a}",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, ScanCode = 0x0a },
                    new KeyEvent { Direction = KeyDirection.Up, ScanCode = 0x0a },
                });
        }
        [TestMethod]
        public void MultipleModifiersWithNumericParseCorrectly()
        {
            Test("+^2", Keys.ShiftKey, Keys.ControlKey, Keys.D2); 
        }
        [TestMethod]
        public void NamedKeyWithScanCodeParseCorrectly()
        {
            Test("{divide.0xb5}",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, Key = Keys.Divide, ScanCode = 0xb5 },
                    new KeyEvent { Direction = KeyDirection.Up, Key = Keys.Divide, ScanCode = 0xb5 },
                });
        }
        [TestMethod]
        public void NamedKeyWithExtendedFlagParseCorrectly()
        {
            Test("{*divide}",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, Key = Keys.Divide, Extended = true },
                    new KeyEvent { Direction = KeyDirection.Up, Key = Keys.Divide, Extended = true },
                });
        }
        [TestMethod]
        public void KeyDownOnlyParsesCorrectlyForSingleAlpha()
        {
            Test("`A", 
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, Key = Keys.A }
                }); 
        }

        [TestMethod]
        public void KeyUpOnlyParsesCorrectlyForSingleAlpha()
        {
            Test("/A",
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Up, Key = Keys.A }
                });
            
        }

        [TestMethod]
        public void MultipleKeyUpAndDownParsesCorrectly()
        {
            // Note that A is held down for the duration of B being pressed
            Test("`A`B/B/A", 
                new KeyEventSequence
                {
                    new KeyEvent { Direction = KeyDirection.Down, Key=Keys.A },
                    new KeyEvent { Direction = KeyDirection.Down, Key=Keys.B },
                    new KeyEvent { Direction = KeyDirection.Up, Key=Keys.B },
                    new KeyEvent { Direction = KeyDirection.Up, Key=Keys.A },
                }); 
        }

        [TestMethod]
        public void NonExistentKeyParsesNull()
        {
            Assert.IsNull(KeySequenceParser.Parse("{nosuch}"));
            Assert.IsNull(KeySequenceParser.Parse("-"));
        }

        private void AssertEqual(KeyEventSequence expected, KeyEventSequence actual)
        {
            Assert.IsNotNull(actual, "Checking that parsed key sequence was not null.");
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
