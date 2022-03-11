using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public class Es_Stemmer
    {
        public string Execute(string word)
        {
            string result = word;
            if (word.Length >= 3)
            {
                StringBuilder sb = new StringBuilder(word.ToLower());

                if (sb[0] == '\'') sb.Remove(0, 1);

                int r1 = 0, r2 = 0, rv = 0;
                ComputeR1R2RV(sb, ref r1, ref r2, ref rv);

                Step0(sb, rv);
                int cont = sb.Length;
                Step1(sb, r1, r2);

                if (sb.Length == cont)
                {
                    Step2a(sb, rv);
                    if (sb.Length == cont)
                    {
                        Step2b(sb, rv);
                    }
                }
                Step3(sb, rv);
                RemoveAcutes(sb);

                result = sb.ToString().ToLower();
            }


            return result;
        }

        private void ComputeR1R2RV(StringBuilder sb, ref int r1, ref int r2, ref int rv)
        {
            r1 = sb.Length;
            r2 = sb.Length;
            rv = sb.Length;

            //R1
            for (int i = 1; i < sb.Length; ++i)
            {
                if ((!IsVowel(sb[i])) && (IsVowel(sb[i - 1])))
                {
                    r1 = i + 1;
                    break;
                }
            }

            //R2
            for (int i = r1 + 1; i < sb.Length; ++i)
            {
                if ((!IsVowel(sb[i])) && (IsVowel(sb[i - 1])))
                {
                    r2 = i + 1;
                    break;
                }
            }

            //RV
            if (sb.Length >= 2)
            {
                if (!IsVowel(sb[1]))
                {
                    for (int i = 1; i < sb.Length; ++i)
                    {
                        if (IsVowel(sb[i]))
                        {
                            rv = sb.Length > i ? i + 1 : sb.Length;
                            break;
                        }
                    }
                }
                else
                {
                    if (IsVowel(sb[0]) && IsVowel(sb[1]))
                    {
                        for (int i = 1; i < sb.Length; ++i)
                        {
                            if (!IsVowel(sb[i]))
                            {
                                rv = sb.Length > i ? i + 1 : sb.Length;
                                break;
                            }
                        }
                    }
                    else
                    {
                        rv = sb.Length >= 3 ? 3 : sb.Length;
                    }
                }
            }
        }

        private bool IsVowel(char c)
        {
            return Specials.Vocales.IndexOf(c) >= 0;
        }

        private void Step0(StringBuilder sb, int rv)
        {
            int index = -1;

            for (int i = 5; i > 1 && index < 0; --i)
            {
                if (sb.Length >= i)
                {
                    //Finding the suffix index
                    index = Specials.Step0.LastIndexOf(sb.ToString(sb.Length - i, i));

                    //If found it
                    if (index >= 0)
                    {
                        string aux = Specials.Step0[index];

                        //Searching for the index of the word that must precede
                        int index_after = Specials.AfterStep0.LastIndexOf(aux);

                        //If found it
                        if (index_after >= 0)
                        {
                            string palabra = Specials.AfterStep0[index_after];

                            //Check if that word actually precedes the suffix
                            if (sb.ToString(0, index).Substring(0, index_after).Length + palabra.Length == sb.ToString(0, index).Length)
                            {
                                if (Specials.AfterStep0[index_after] == "yendo" && sb[index_after - 1] == 'u' && index_after >= rv)
                                {
                                    sb.Remove(sb.Length - index, index);
                                }
                                else
                                {
                                    sb.Remove(sb.Length - index, index);
                                    for (int j = index_after; j < sb.Length; j++)
                                        sb[j] = Specials.EliminaAcento(sb[j]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Step1(StringBuilder sb, int r1, int r2)
        {
            int posicion = -1;
            int coleccion = -1;
            string encontrada = "";
            string buscar = sb.ToString();

            foreach (string s in Specials.Step1_1)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_1.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_1[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_1[aux];
                        posicion = index;
                        coleccion = 1;
                    }
                }
            }

            foreach (string s in Specials.Step1_2)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_2.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_2[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_2[aux];
                        posicion = index;
                        coleccion = 2;
                    }
                }
            }

            foreach (string s in Specials.Step1_3)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_3.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_3[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_3[aux];
                        posicion = index;
                        coleccion = 3;
                    }
                }
            }

            foreach (string s in Specials.Step1_4)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_4.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_4[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_4[aux];
                        posicion = index;
                        coleccion = 4;
                    }
                }
            }

            foreach (string s in Specials.Step1_5)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_5.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_5[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_5[aux];
                        posicion = index;
                        coleccion = 5;
                    }
                }
            }

            foreach (string s in Specials.Step1_6)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_6.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_6[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_6[aux];
                        posicion = index;
                        coleccion = 6;
                    }
                }
            }

            foreach (string s in Specials.Step1_7)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_7.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_7[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_7[aux];
                        posicion = index;
                        coleccion = 7;
                    }
                }
            }

            foreach (string s in Specials.Step1_8)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_8.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_8[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_8[aux];
                        posicion = index;
                        coleccion = 8;
                    }
                }
            }

            foreach (string s in Specials.Step1_9)
            {
                int index = buscar.LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = buscar.Substring(index);
                    int aux = -1;

                    aux = Specials.Step1_9.LastIndexOf(palabra);
                    if (aux >= 0 && Specials.Step1_9[aux].Length > encontrada.Length)
                    {
                        encontrada = Specials.Step1_9[aux];
                        posicion = index;
                        coleccion = 9;
                    }
                }
            }

            if (posicion >= 0)
            {
                switch (coleccion)
                {
                    case 1:
                        if (posicion >= r2)
                            sb.Remove(posicion, sb.Length - posicion);
                        break;
                    case 2:
                        if (posicion >= r2)
                            sb.Remove(posicion, sb.Length - posicion);
                        break;
                    case 3:
                        if (posicion >= r2)
                        {
                            sb.Remove(posicion, sb.Length - posicion);
                            sb.Append("log");
                        }
                        break;
                    case 4:
                        if (posicion >= r2)
                        {
                            sb.Remove(posicion, sb.Length - posicion);
                            sb.Append("u");
                        }
                        break;
                    case 5:
                        if (posicion >= r2)
                        {
                            sb.Remove(posicion, sb.Length - posicion);
                            sb.Append("ente");
                        }
                        break;
                    case 6:
                        if (posicion >= r1)
                            sb.Remove(posicion, sb.Length - posicion);
                        else
                        {
                            string aux = sb.ToString(0, posicion);
                            if (aux.Substring(0, aux.Length - 2) == "iv" &&
                                aux.Substring(0, aux.Length - 2) == "oc" &&
                                aux.Substring(0, aux.Length - 2) == "ic" &&
                                aux.Substring(0, aux.Length - 2) == "ad" && posicion >= r2)
                            {
                                sb.Remove(posicion, sb.Length - posicion);
                            }
                        }
                        break;
                    case 7:
                    case 8:
                    case 9:
                        if (posicion >= r2)
                        {
                            sb.Remove(posicion, sb.Length - posicion);
                        }
                        break;
                }
            }
        }

        private void Step2a(StringBuilder sb, int rv)
        {
            int index = -1;

            //Busco el indice del sufijo
            index = Specials.Step2_a.IndexOf(sb.ToString());

            if (index >= rv && sb.ToString().Substring(sb.Length - index - 1, 1) == "u")
            {
                sb.Remove(sb.Length - index, index);
            }
        }

        private void Step2b(StringBuilder sb, int rv)
        {
            string seleccionado = "";
            int pos = -1;
            int index = -1;

            foreach (string s in Specials.Step2_b1)
            {
                index = sb.ToString().LastIndexOf(s);
                if (index >= 0) // && Specials.Step2_b1[index].Length > seleccionado.Length)
                {
                    string palabra = sb.ToString().Substring(index);
                    int aux = index;

                    index = Specials.Step2_b1.LastIndexOf(palabra);
                    if (index >= 0)
                    {
                        seleccionado = Specials.Step2_b1[index];
                        pos = aux;
                    }
                }
            }

            if (pos >= rv && sb.ToString(sb.Length - pos - 2, pos) == "gu")
                sb.Remove(pos - 1, sb.Length - pos + 1);

            pos = -1;
            index = -1;
            seleccionado = "";

            foreach (string s in Specials.Step2_b2)
            {
                index = sb.ToString().LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = sb.ToString().Substring(index);
                    int aux = index;

                    index = Specials.Step2_b2.LastIndexOf(palabra);
                    if (index >= 0)
                    {
                        seleccionado = Specials.Step2_b2[index];
                        pos = aux;
                    }
                }
            }

            if (pos >= rv)
                sb.Remove(pos, sb.Length - pos);
        }

        private void Step3(StringBuilder sb, int rv)
        {
            string seleccionado = "";
            int pos = -1;
            int index = -1;

            foreach (string s in Specials.Step3_1)
            {
                index = sb.ToString().LastIndexOf(s);
                if (index >= 0) // && Specials.Step3_1[index].Length > seleccionado.Length)
                {
                    string palabra = sb.ToString().Substring(index);
                    int aux = index;

                    index = Specials.Step3_1.LastIndexOf(palabra);
                    if (index >= 0)
                    {
                        seleccionado = Specials.Step3_1[index];
                        pos = aux;
                    }
                }
            }

            if (pos >= rv)
                sb.Remove(pos, sb.Length - pos);

            pos = -1;
            index = -1;
            seleccionado = "";

            foreach (string s in Specials.Step3_2)
            {
                index = sb.ToString().LastIndexOf(s);
                if (index >= 0)
                {
                    string palabra = sb.ToString().Substring(index);
                    int aux = index;

                    index = Specials.Step3_2.LastIndexOf(palabra);
                    if (index >= 0)
                    {
                        seleccionado = Specials.Step3_2[index];
                        pos = index;
                    }
                }
            }

            if (pos >= 0 && sb.ToString(sb.Length - pos - 2, pos) == "gu" && pos - 1 >= rv)
                sb.Remove(pos - 1, sb.Length - pos + 1);
        }

        private void RemoveAcutes(StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; ++i)
            {
                char c = sb[i];
                sb[i] = Specials.EliminaAcento(c);
            }
        }
    }
    public static class Specials
    {
        public static List<char> Vocales = new List<char>() { 'a', 'e', 'i', 'o', 'u' };

        public static List<char> VocalesAcentuadas = new List<char>() {
            'á', 'é', 'í', 'ó', 'ú',
            'à', 'è', 'ì', 'ò', 'ù',
            'ä', 'ë', 'ï', 'ö', 'ü',
            'Á', 'É', 'Í', 'Ó', 'Ú',
            'À', 'È', 'Ì', 'Ò', 'Ù',
            'Ä', 'Ë', 'Ï', 'Ö', 'Ü'
        };

        public static List<string> Step0 = new List<string>() {
            "me", "se", "sela", "selo", "selas", "selos", "la", "le", "lo", "las", "les", "los", "nos"
        };

        public static List<string> AfterStep0 = new List<string>() {
            "iéndo", "ándo", "ár", "ér", "ír", "ando", "iendo", "ar", "er", "ir", "yendo"
        };

        public static List<string> Step1_1 = new List<string>() {
            "anza", "anzas", "ico", "ica", "icos", "icas", "ismo", "ismos", "able", "ables", "ible", "ibles", "ista", "istas", "oso", "osa", "osos", "osas", "amiento", "amientos", "imiento", "imientos"
        };

        public static List<string> Step1_2 = new List<string>() {
            "adora", "ador", "ación", "adoras", "adores", "aciones", "ante", "antes", "ancia", "ancias"
        };

        public static List<string> Step1_3 = new List<string>() {
            "logía", "logías"
        };

        public static List<string> Step1_4 = new List<string>() {
            "ución", "uciones"
        };

        public static List<string> Step1_5 = new List<string>() {
            "encia", "encias"
        };

        public static List<string> Step1_6 = new List<string>() {
            "amente"
        };

        public static List<string> Step1_7 = new List<string>() {
            "mente"
        };

        public static List<string> Step1_8 = new List<string>() {
            "idad", "idades"
        };

        public static List<string> Step1_9 = new List<string>() {
            "iva", "ivo", "ivas", "ivos"
        };

        public static List<string> Step2_a = new List<string>() {
            "yeron", "yendo", "yamos", "yais", "yan", "yen", "yas", "yes", "ya", "ye", "yo", "yó"
        };

        public static List<string> Step2_b1 = new List<string>() {
            "en", "es", "éis", "emos"
        };

        public static List<string> Step2_b2 = new List<string>() {
            "arían", "arías", "arán", "arás", "aríais", "aría", "aréis", "aríamos", "aremos", "ará",
            "aré", "erían", "erías", "erán", "erás", "eríais", "ería", "eréis", "eríamos", "eremos",
            "erá", "eré", "irían", "irías", "irán", "irás", "iríais", "iría", "iréis", "iríamos",
            "iremos", "irá", "iré", "aba", "ada", "ida", "ía", "ara", "iera", "ad", "ed", "id", "ase",
            "iese", "aste", "iste", "an", "aban", "ían", "aran", "ieran", "asen", "iesen", "aron",
            "ieron", "ado", "ido", "ando", "iendo", "ió", "ar", "er", "ir", "as", "abas", "adas",
            "idas", "ías", "aras", "ieras", "ases", "ieses", "ís", "áis", "abais", "íais", "arais",
            "ierais", "aseis", "ieseis", "asteis", "isteis", "ados", "idos", "amos", "ábamos", "íamos",
            "imos", "áramos", "iéramos", "iésemos", "ásemos"
        };

        public static List<string> Step3_1 = new List<string>() {
            "os", "a", "o", "á", "í", "ó"
        };

        public static List<string> Step3_2 = new List<string>() {
            "e", "é"
        };

        public static List<string> stop_words = new List<string>() {
            "de", "la", "que", "el", "en", "y", "a", "los", "del", "se", "las", "por", "un", "para", "con",
            "no", "una", "su", "al", "es", "lo", "como", "más", "pero", "sus", "le", "ya", "o", "fue", "este",
            "ha", "sí", "porque", "esta", "son", "entre", "está", "cuando", "muy", "sin", "sobre", "ser",
            "tiene", "también", "me", "hasta", "hay", "donde", "han", "quien", "están", "estado", "desde",
            "todo", "nos", "durante", "estados", "todos", "uno", "les", "ni", "contra", "otros", "fueron",
            "ese", "eso", "había", "ante", "ellos", "e", "esto", "mí", "antes", "algunos", "qué", "unos",
            "yo", "otro", "otras", "otra", "él", "tanto", "esa", "estos", "mucho", "quienes", "nada", "muchos",
            "cual", "sea", "poco", "ella", "estar", "haber", "estas", "estaba", "estamos", "algunas", "algo",
            "nosotros", "mi", "mis", "tú", "te", "ti", "tu", "tus", "ellas", "nosotras", "vosotros", "vosotras",
            "os", "mío", "mía", "míos", "mías", "tuyo", "tuya", "tuyos", "tuyas", "suyo", "suya", "suyos", "suyas",
            "nuestro", "nuestra", "nuestros", "nuestras", "vuestro", "vuestra", "vuestros", "vuestras", "esos",
            "esas", "estoy", "estás", "está", "estamos", "estáis", "están", "esté", "estés", "estemos", "estéis",
            "estén", "estaré", "estarás", "estará", "estaremos", "estaréis", "estarán", "estaría", "estarías",
            "estaríamos", "estaríais", "estarían", "estaba", "estabas", "estábamos", "estabais", "estaban",
            "estuve", "estuviste", "estuvo", "estuvimos", "estuvisteis", "estuvieron", "estuviera", "estuvieras",
            "estuviéramos", "estuvierais", "estuvieran", "estuviese", "estuvieses", "estuviésemos", "estuvieseis",
            "estuviesen", "estando", "estado", "estada", "estados", "estadas", "estad", "he", "has", "ha", "hemos",
            "habéis", "han", "haya", "hayas", "hayamos", "hayáis", "hayan", "habré", "habrás", "habrá", "habremos",
            "habréis", "habrán", "habría", "habrías", "habríamos", "habríais", "habrían", "había", "habías",
            "habíamos", "habíais", "habían", "hube", "hubiste", "hubo", "hubimos", "hubisteis", "hubieron",
            "hubiera", "hubieras", "hubiéramos", "hubierais", "hubieran", "hubiese", "hubieses", "hubiésemos",
            "hubieseis", "hubiesen", "habiendo", "habido", "habida", "habidos", "habidas", "soy", "eres", "es",
            "somos", "sois", "son", "sea", "seas", "seamos", "seáis", "sean", "seré", "serás", "será", "seremos",
            "seréis", "serán", "sería", "serías", "seríamos", "seríais", "serían", "era", "eras", "éramos", "erais",
            "eran", "fui", "fuiste", "fue", "fuimos", "fuisteis", "fueron", "fuera", "fueras", "fuéramos", "fuerais",
            "fueran", "fuese", "fueses", "fuésemos", "fueseis", "fuesen", "siendo", "sido", "sed", "tengo", "tienes",
            "tiene", "tenemos", "tenéis", "tienen", "tenga", "tengas", "tengamos", "tengáis", "tengan", "tendré",
            "tendrás", "tendrá", "tendremos", "tendréis", "tendrán", "tendría", "tendrías", "tendríamos", "tendríais",
            "tendrían", "tenía", "tenías", "teníamos", "teníais", "tenían", "tuve", "tuviste", "tuvo", "tuvimos",
            "tuvisteis", "tuvieron", "tuviera", "tuvieras", "tuviéramos", "tuvierais", "tuvieran", "tuviese", "tuvieses",
            "tuviésemos", "tuvieseis", "tuviesen", "teniendo", "tenido", "tenida", "tenidos", "tenidas", "tened"
        };

        public static char EliminaAcento(char c)
        {
            char res = c;

            switch (c)
            {
                case 'á':
                case 'à':
                case 'ä':
                    res = 'a';
                    break;
                case 'é':
                case 'è':
                case 'ë':
                    res = 'e';
                    break;
                case 'í':
                case 'ì':
                case 'ï':
                    res = 'i';
                    break;
                case 'ó':
                case 'ò':
                case 'ö':
                    res = 'o';
                    break;
                case 'ú':
                case 'ù':
                case 'ü':
                    res = 'u';
                    break;
            }

            return res;
        }
    }
}
