using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    class En_Stemmer
    {
        private char[]? wordToStemm;
        private int i, j, k;

        private static int INC = 50;
        /* unit of size whereby wordToStemm is increased */

        public string Execute(string word)
        {
            i = 0;
            wordToStemm = new char[word.Length];
            for (int c = 0; c < word.Length; c++)
                wordToStemm[c] = word[c];
            i = word.Length;
            k = i - 1;
            if (k > 1)
            {
                Step1();
                Step2();
                Step3();
                Step4();
                Step5();
                Step6();
            }
            i = 0;
            string s = "";
            for (int i = 0; i < k; i++)
            {
                s += wordToStemm[i];
            }
            return s;
        }
        private bool IsCons(int i)
        {
            switch (wordToStemm[i])
            {
                case 'a': case 'e': case 'i': case 'o': case 'u': return false;
                case 'y': return (i == 0) ? true : !IsCons(i - 1);
                default: return true;
            }
        }

        /// <summary>
        /// Measures() measures the number of consonant sequences between 0 and j
        /// </summary>
        private int Measures()
        {
            int n = 0;
            int i = 0;
            while (true)
            {
                if (i > j) return n;
                if (!IsCons(i)) break; i++;
            }
            i++;
            while (true)
            {
                while (true)
                {
                    if (i > j) return n;
                    if (IsCons(i)) break;
                    i++;
                }
                i++;
                n++;
                while (true)
                {
                    if (i > j) return n;
                    if (!IsCons(i)) break;
                    i++;
                }
                i++;
            }
        }


        /// <summary>
        /// VowelInStem() is true <=> 0,...j contains a vowel
        /// </summary>
        /// <returns></returns>
        private bool VowelInStem()
        {
            int i;
            for (i = 0; i <= j; i++)
                if (!IsCons(i))
                    return true;
            return false;
        }

        /// <summary>
        /// DoubleC(j) is true <=> j and (j-1) contain a double consonant.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool DoubleC(int j)
        {
            if (j < 1)
                return false;
            if (wordToStemm[j] != wordToStemm[j - 1])
                return false;
            return IsCons(j);
        }

        private bool RestoreE(int i)
        {
            if (i < 2 || !IsCons(i) || IsCons(i - 1) || !IsCons(i - 2))
                return false;
            int ch = wordToStemm[i];
            if (ch == 'w' || ch == 'x' || ch == 'y')
                return false;
            return true;
        }

        private bool Ends(String s)
        {
            int l = s.Length;
            int o = k - l + 1;
            if (o < 0)
                return false;
            char[] sc = s.ToCharArray();
            for (int i = 0; i < l; i++)
                if (wordToStemm[o + i] != sc[i])
                    return false;
            j = k - l;
            return true;
        }

        /// <summary>
        /// SetTo(s) sets (j+1),...k to the characters in the string s, readjusting k.
        /// </summary>
        /// <param name="s"></param>
        private void SetTo(String s)
        {
            int l = s.Length;
            int o = j + 1;
            char[] sc = s.ToCharArray();
            for (int i = 0; i < l; i++)
                wordToStemm[o + i] = sc[i];
            k = j + l;
        }

        private void SetIfCons(String s)
        {
            if (Measures() > 0)
                SetTo(s);
        }

        /// <summary>
        /// Delete plurals and -ed or -ing
        /// </summary>
        private void Step1()
        {
            if (wordToStemm[k] == 's')
            {
                if (Ends("sses"))
                    k -= 2;
                else if (Ends("ies"))
                    SetTo("i");
                else if (wordToStemm[k - 1] != 's')
                    k--;
            }
            if (Ends("eed"))
            {
                if (Measures() > 0)
                    k--;
            }
            else if ((Ends("ed") || Ends("ing")) && VowelInStem())
            {
                k = j;
                if (Ends("at"))
                    SetTo("ate");
                else if (Ends("bl"))
                    SetTo("ble");
                else if (Ends("iz"))
                    SetTo("ize");
                else if (DoubleC(k))
                {
                    k--;
                    int ch = wordToStemm[k];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        k++;
                }
                else if (Measures() == 1 && RestoreE(k)) SetTo("e");
            }
        }

        /// <summary>
        /// Change 'y' to 'i' if there is another vowel
        /// </summary>
        private void Step2()
        {
            if (Ends("y") && VowelInStem())
                wordToStemm[k] = 'i';
        }

        /// <summary>
        /// Change double suffice to single ones: -ization( -ize + -ation) = -ize
        /// </summary>
        private void Step3()
        {
            if (k == 0)
                return;
            switch (wordToStemm[k - 1])
            {
                case 'a':
                    if (Ends("ational")) { SetIfCons("ate"); break; }
                    if (Ends("tional")) { SetIfCons("tion"); break; }
                    break;
                case 'c':
                    if (Ends("enci")) { SetIfCons("ence"); break; }
                    if (Ends("anci")) { SetIfCons("ance"); break; }
                    break;
                case 'e':
                    if (Ends("izer")) { SetIfCons("ize"); break; }
                    break;
                case 'l':
                    if (Ends("bli")) { SetIfCons("ble"); break; }
                    if (Ends("alli")) { SetIfCons("al"); break; }
                    if (Ends("entli")) { SetIfCons("ent"); break; }
                    if (Ends("eli")) { SetIfCons("e"); break; }
                    if (Ends("ousli")) { SetIfCons("ous"); break; }
                    break;
                case 'o':
                    if (Ends("ization")) { SetIfCons("ize"); break; }
                    if (Ends("ation")) { SetIfCons("ate"); break; }
                    if (Ends("ator")) { SetIfCons("ate"); break; }
                    break;
                case 's':
                    if (Ends("alism")) { SetIfCons("al"); break; }
                    if (Ends("iveness")) { SetIfCons("ive"); break; }
                    if (Ends("fulness")) { SetIfCons("ful"); break; }
                    if (Ends("ousness")) { SetIfCons("ous"); break; }
                    break;
                case 't':
                    if (Ends("aliti")) { SetIfCons("al"); break; }
                    if (Ends("iviti")) { SetIfCons("ive"); break; }
                    if (Ends("biliti")) { SetIfCons("ble"); break; }
                    break;
                case 'g':
                    if (Ends("logi")) { SetIfCons("log"); break; }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Similar to Step3 but with other terminations
        /// </summary>
        private void Step4()
        {
            switch (wordToStemm[k])
            {
                case 'e':
                    if (Ends("icate")) { SetIfCons("ic"); break; }
                    if (Ends("ative")) { SetIfCons(""); break; }
                    if (Ends("alize")) { SetIfCons("al"); break; }
                    break;
                case 'i':
                    if (Ends("iciti")) { SetIfCons("ic"); break; }
                    break;
                case 'l':
                    if (Ends("ical")) { SetIfCons("ic"); break; }
                    if (Ends("ful")) { SetIfCons(""); break; }
                    break;
                case 's':
                    if (Ends("ness")) { SetIfCons(""); break; }
                    break;
            }
        }

        /// <summary>
        /// Delete others terminations if happen a condition
        /// </summary>
        private void Step5()
        {
            if (k == 0)
                return;

            /* for Bug 1 */
            switch (wordToStemm[k - 1])
            {
                case 'a':
                    if (Ends("al")) break; return;
                case 'c':
                    if (Ends("ance")) break;
                    if (Ends("ence")) break; return;
                case 'e':
                    if (Ends("er")) break; return;
                case 'i':
                    if (Ends("ic")) break; return;
                case 'l':
                    if (Ends("able")) break;
                    if (Ends("ible")) break; return;
                case 'n':
                    if (Ends("ant")) break;
                    if (Ends("ement")) break;
                    if (Ends("ment")) break;
                    /* element etc. not stripped before the Measures */
                    if (Ends("ent")) break; return;
                case 'o':
                    if (Ends("ion") && j >= 0 && (wordToStemm[j] == 's' || wordToStemm[j] == 't')) break;
                    /* j >= 0 fixes Bug 2 */
                    if (Ends("ou")) break; return;
                /* takes care of -ous */
                case 's':
                    if (Ends("ism")) break; return;
                case 't':
                    if (Ends("ate")) break;
                    if (Ends("iti")) break; return;
                case 'u':
                    if (Ends("ous")) break; return;
                case 'v':
                    if (Ends("ive")) break; return;
                case 'z':
                    if (Ends("ize")) break; return;
                default:
                    return;
            }
            if (Measures() > 1)
                k = j;
        }

        /// <summary>
        /// Remove a final 'e' if Measures() > 1
        /// </summary>
        private void Step6()
        {
            j = k;

            if (wordToStemm[k] == 'e')
            {
                int a = Measures();
                if (a > 1 || a == 1 && !RestoreE(k - 1))
                    k--;
            }
            if (wordToStemm[k] == 'l' && DoubleC(k) && Measures() > 1)
                k--;
        }
    }
}
