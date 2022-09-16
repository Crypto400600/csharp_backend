using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Engine
{
    /// <summary>
    /// The Stemmer class transforms a word into its root form.
    /// Implementing the Porter Stemming Algorithm
    /// </summary>
    /// <remarks>
    /// Modified from: http://tartarus.org/martin/PorterStemmer/csharp2.txt
    /// </remarks>
    /// <example>
    /// var stemmer = new Stemmer();
    /// var stem = stemmer.StemWord(word);
    /// </example>
    public class Stemmer
    {
        // The passed in word turned into a char array.
        // Quicker to use to rebuilding strings each time a change is made.
        private char[] _wordArray;

        // Current index to the end of the word in the character array. This will
        // change as the end of the string gets modified.
        private int _endIndex;

        // Index of the (potential) end of the stem word in the char array.
        private int _stemIndex;

        /// <summary>
        /// Stem the passed in word.
        /// </summary>
        /// <param name="word">Word to evaluate</param>
        /// <returns></returns>
        public string StemWord(string word)
        {
            // Do nothing for empty strings or short words.
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2) return word;
            _wordArray = word.ToCharArray();
            _stemIndex = 0;
            _endIndex = word.Length - 1;
            Step1();
            Step2();
            Step3();
            Step4();
            Step5();
            Step6();
            var length = _endIndex + 1;
            return new String(_wordArray, 0, length);
        }

        // Step1() gets rid of plurals and -ed or -ing.
        /* Examples:
        caresses -> caress
        ponies -> poni
        ties -> ti
        caress -> caress
        cats -> cat
        feed -> feed
        agreed -> agree
        disabled -> disable
        matting -> mat
        mating -> mate
        meeting -> meet
        milling -> mill
        messing -> mess
        meetings -> meet */

        private void Step1()
        {
            // If the word ends with s take that off
            if (_wordArray[_endIndex] == 's')
            {
                if (EndsWith("sses"))
                {
                    _endIndex -= 2;
                }
                else if (EndsWith("ies"))
                {
                    SetEnd("i");
                }
                else if (_wordArray[_endIndex - 1] != 's')
                {
                    _endIndex--;
                }
            }
            if (EndsWith("eed"))
            {
                if (MeasureConsontantSequence() > 0)
                    _endIndex--;
            }
            else if ((EndsWith("ed") || EndsWith("ing")) && VowelInStem())
            {
                _endIndex = _stemIndex;
                if (EndsWith("at"))
                    SetEnd("ate");
                else if (EndsWith("bl"))
                    SetEnd("ble");
                else if (EndsWith("iz"))
                    SetEnd("ize");
                else if (IsDoubleConsontant(_endIndex))
                {
                    _endIndex--;
                    int ch = _wordArray[_endIndex];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        _endIndex++;
                }
                else if (MeasureConsontantSequence() == 1 && IsCvc(_endIndex)) SetEnd("e");
            }
        }

        // Step2() turns terminal y to i when there is another vowel in the stem.
        private void Step2()
        {
            if (EndsWith("y") && VowelInStem())
                _wordArray[_endIndex] = 'i';
        }

        // Step3() maps double suffices to single ones. so -ization ( = -ize plus
        // -ation) maps to -ize etc. note that the string before the suffix must give m() > 0.
        private void Step3()
        {
            if (_endIndex == 0) return;
            /* For Bug 1 */
            switch (_wordArray[_endIndex - 1])
            {
                case 'a':
                    if (EndsWith("ational")) { ReplaceEnd("ate"); break; }
                    if (EndsWith("tional")) { ReplaceEnd("tion"); }
                    break;

                case 'c':
                    if (EndsWith("enci")) { ReplaceEnd("ence"); break; }
                    if (EndsWith("anci")) { ReplaceEnd("ance"); }
                    break;

                case 'e':
                    if (EndsWith("izer")) { ReplaceEnd("ize"); }
                    break;

                case 'l':
                    if (EndsWith("bli")) { ReplaceEnd("ble"); break; }
                    if (EndsWith("alli")) { ReplaceEnd("al"); break; }
                    if (EndsWith("entli")) { ReplaceEnd("ent"); break; }
                    if (EndsWith("eli")) { ReplaceEnd("e"); break; }
                    if (EndsWith("ousli")) { ReplaceEnd("ous"); }
                    break;

                case 'o':
                    if (EndsWith("ization")) { ReplaceEnd("ize"); break; }
                    if (EndsWith("ation")) { ReplaceEnd("ate"); break; }
                    if (EndsWith("ator")) { ReplaceEnd("ate"); }
                    break;

                case 's':
                    if (EndsWith("alism")) { ReplaceEnd("al"); break; }
                    if (EndsWith("iveness")) { ReplaceEnd("ive"); break; }
                    if (EndsWith("fulness")) { ReplaceEnd("ful"); break; }
                    if (EndsWith("ousness")) { ReplaceEnd("ous"); }
                    break;

                case 't':
                    if (EndsWith("aliti")) { ReplaceEnd("al"); break; }
                    if (EndsWith("iviti")) { ReplaceEnd("ive"); break; }
                    if (EndsWith("biliti")) { ReplaceEnd("ble"); }
                    break;

                case 'g':
                    if (EndsWith("logi"))
                    {
                        ReplaceEnd("log");
                    }
                    break;
            }
        }

        /* step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */

        private void Step4()
        {
            switch (_wordArray[_endIndex])
            {
                case 'e':
                    if (EndsWith("icate")) { ReplaceEnd("ic"); break; }
                    if (EndsWith("ative")) { ReplaceEnd(""); break; }
                    if (EndsWith("alize")) { ReplaceEnd("al"); }
                    break;

                case 'i':
                    if (EndsWith("iciti")) { ReplaceEnd("ic"); }
                    break;

                case 'l':
                    if (EndsWith("ical")) { ReplaceEnd("ic"); break; }
                    if (EndsWith("ful")) { ReplaceEnd(""); }
                    break;

                case 's':
                    if (EndsWith("ness")) { ReplaceEnd(""); }
                    break;
            }
        }

        /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */

        private void Step5()
        {
            if (_endIndex == 0) return;
            switch (_wordArray[_endIndex - 1])
            {
                case 'a':
                    if (EndsWith("al")) break; return;
                case 'c':
                    if (EndsWith("ance")) break;
                    if (EndsWith("ence")) break; return;
                case 'e':
                    if (EndsWith("er")) break; return;
                case 'i':
                    if (EndsWith("ic")) break; return;
                case 'l':
                    if (EndsWith("able")) break;
                    if (EndsWith("ible")) break; return;
                case 'n':
                    if (EndsWith("ant")) break;
                    if (EndsWith("ement")) break;
                    if (EndsWith("ment")) break;
                    /* element etc. not stripped before the m */
                    if (EndsWith("ent")) break; return;
                case 'o':
                    if (EndsWith("ion") && _stemIndex >= 0 && (_wordArray[_stemIndex] == 's' || _wordArray[_stemIndex] == 't')) break;
                    /* j >= 0 fixes Bug 2 */
                    if (EndsWith("ou")) break; return;
                /* takes care of -ous */
                case 's':
                    if (EndsWith("ism")) break; return;
                case 't':
                    if (EndsWith("ate")) break;
                    if (EndsWith("iti")) break; return;
                case 'u':
                    if (EndsWith("ous")) break; return;
                case 'v':
                    if (EndsWith("ive")) break; return;
                case 'z':
                    if (EndsWith("ize")) break; return;
                default:
                    return;
            }
            if (MeasureConsontantSequence() > 1)
                _endIndex = _stemIndex;
        }

        /* step6() removes a final -e if m() > 1. */

        private void Step6()
        {
            _stemIndex = _endIndex;
            if (_wordArray[_endIndex] == 'e')
            {
                var a = MeasureConsontantSequence();
                if (a > 1 || a == 1 && !IsCvc(_endIndex - 1))
                    _endIndex--;
            }
            if (_wordArray[_endIndex] == 'l' && IsDoubleConsontant(_endIndex) && MeasureConsontantSequence() > 1)
                _endIndex--;
        }

        // Returns true if the character at the specified index is a consonant.
        // With special handling for 'y'.
        private bool IsConsonant(int index)
        {
            var c = _wordArray[index];
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') return false;
            return c != 'y' || (index == 0 || !IsConsonant(index - 1));
        }

        /* m() measures the number of consonant sequences between 0 and j. if c is
        a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
        presence,
        <c><v> gives 0
        <c>vc<v> gives 1
        <c>vcvc<v> gives 2
        <c>vcvcvc<v> gives 3
        .... */

        private int MeasureConsontantSequence()
        {
            var n = 0;
            var index = 0;
            while (true)
            {
                if (index > _stemIndex) return n;
                if (!IsConsonant(index)) break; index++;
            }
            index++;
            while (true)
            {
                while (true)
                {
                    if (index > _stemIndex) return n;
                    if (IsConsonant(index)) break;
                    index++;
                }
                index++;
                n++;
                while (true)
                {
                    if (index > _stemIndex) return n;
                    if (!IsConsonant(index)) break;
                    index++;
                }
                index++;
            }
        }

        // Return true if there is a vowel in the current stem (0 ... stemIndex)
        private bool VowelInStem()
        {
            int i;
            for (i = 0; i <= _stemIndex; i++)
            {
                if (!IsConsonant(i)) return true;
            }
            return false;
        }

        // Returns true if the char at the specified index and the one preceeding it are the same consonants.
        private bool IsDoubleConsontant(int index)
        {
            if (index < 1) return false;
            return _wordArray[index] == _wordArray[index - 1] && IsConsonant(index);
        }

        /* cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
        and also if the second c is not w,x or y. this is used when trying to
        restore an e at the end of a short word. e.g.
        cav(e), lov(e), hop(e), crim(e), but
        snow, box, tray. */

        private bool IsCvc(int index)
        {
            if (index < 2 || !IsConsonant(index) || IsConsonant(index - 1) || !IsConsonant(index - 2)) return false;
            var c = _wordArray[index];
            return c != 'w' && c != 'x' && c != 'y';
        }

        // Does the current word array end with the specified string.
        private bool EndsWith(string s)
        {
            var length = s.Length;
            var index = _endIndex - length + 1;
            if (index < 0) return false;
            for (var i = 0; i < length; i++)
            {
                if (_wordArray[index + i] != s[i]) return false;
            }
            _stemIndex = _endIndex - length;
            return true;
        }

        // Set the end of the word to s.
        // Starting at the current stem pointer and readjusting the end pointer.
        private void SetEnd(string s)
        {
            var length = s.Length;
            var index = _stemIndex + 1;
            for (var i = 0; i < length; i++)
            {
                _wordArray[index + i] = s[i];
            }
            // Set the end pointer to the new end of the word.
            _endIndex = _stemIndex + length;
        }

        // Conditionally replace the end of the word
        private void ReplaceEnd(string s)
        {
            if (MeasureConsontantSequence() > 0) SetEnd(s);
        }
    }
}