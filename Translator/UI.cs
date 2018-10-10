using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Translator
{
    public class UI
    {

        string dir;
        private Dictionary<string, string> languageLetters;
        private Dictionary<string, string> languageLetterPairs;
        private Dictionary<string, string> languageLetterTripels;

        public void MainMenu()
        {
            dir = Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().
            Location);
            languageLetters = new Dictionary<string, string>();
            languageLetterPairs = new Dictionary<string, string>();
            languageLetterTripels = new Dictionary<string, string>();
            bool quitProgram = false;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\t\t\t\tThe Campain World Translator");
                Console.ResetColor();
                Console.WriteLine("1: Translate to language");
                Console.WriteLine("2: Create a dictionary");
                Console.WriteLine("3: Check Key File");
                Console.WriteLine("4: Quick Translate");
                Console.WriteLine("0: Quit");
                string input = Console.ReadKey().KeyChar.ToString();
                switch (input)
                {
                    case "1":
                        TranslateTo();
                        break;
                    case "2":
                        CreateDictionary();
                        break;
                    case "3":
                        CheckKeyFile();
                        break;
                    case "4":
                        TranslateNow();
                        break;
                    case "0":
                        quitProgram = true;
                        break;
                    default:
                        Console.WriteLine(">INVALID INPUT<\n>TRY AGAIN<");
                        break;
                }
            } while (!quitProgram);
            Console.ReadLine();
        }


        public void CheckKeyFile()
        {
            TranslatorFuncitonStart();
            string[] langTextArray = ReadInDictionaryFile();

            string toPrint = "";
            List<string> translated = new List<string>();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (string word in langTextArray.OrderBy(x => x))
            {
                string temp = TranslateWord(word);
                translated.Add(temp);
                pairs.Add(word, temp);
                toPrint += word + "\t:\t" + temp + "\r\n";
            }

            var groupings = translated.GroupBy(x => x).
                Where(group => group.Count() > 1);

            if (groupings.Count() > 0)
            {
                Console.WriteLine("You might want to look at the following:");
                foreach (var s in groupings)
                {
                    string output = "";
                    output = s.Key + " " + s.Count() + "[";
                    foreach (string ss in pairs.Where(x => x.Value == s.Key).Select(x => x.Key))
                    {
                        output += ss + ", ";
                    }
                    output = output.Remove(output.Length - 2);
                    Console.WriteLine(output + "]\n");
                }
            }
            else
            {
                Console.WriteLine("No duplicats found in this dictionary");
            }

            Console.Write("If you wish to save the dictionary press y: ");
            if (Console.ReadKey().Key.Equals(ConsoleKey.Y))
            {
                Console.Write("\nPlease name your dictionary file: ");

                string dictionary = Console.ReadLine();
                using (StreamWriter file = new StreamWriter(dir + @"\Translations\" + dictionary + ".txt", false))
                {
                    file.WriteLine(toPrint);
                }
            }
        }

        private void CreateDictionary()
        {
            TranslatorFuncitonStart();
            string[] langTextArray = ReadInDictionaryFile();

            string toPrint = "";

            foreach (string word in langTextArray.OrderBy(x => x))
            {
                toPrint += word + "\t:\t" + TranslateWord(word) + "\r\n";
            }

            Console.Write("Please name your dictionary file: ");

            string dictionary = Console.ReadLine();
            using (StreamWriter file = new StreamWriter(dir + @"\Translations\" + dictionary + ".txt", false))
            {
                file.WriteLine(toPrint);
            }
        }

        public void TranslateTo()
        {
            TranslatorFuncitonStart();

            string textInput;
            Console.Write("Input the full name on the text file you wish to translate (including file end): ");
            textInput = Console.ReadLine();
            string[] langTextArray;
            using (StreamReader sr = new StreamReader(dir + @"\Texts to translate\" + textInput, true))
            {
                string langTextString = sr.ReadToEnd();
                langTextArray = langTextString.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            string toPrint = "";
            //Regex checkForI = new Regex(@"(\b[iI]\b)");
            foreach (string word in langTextArray)
            {
                toPrint += TranslateWord(word) + " ";
            }

            Console.Write("Please name your translated file: ");

            string translation = Console.ReadLine();
            using (StreamWriter file = new StreamWriter(dir + @"\Translations\" + translation, true))
            {
                file.WriteLine(toPrint);
            }
        }

        public void TranslateNow()
        {
            bool? quitLoop = true;
            do
            {
                TranslatorFuncitonStart();

                Console.Write("What do you wish to know the translation for: ");
                string input = Console.ReadLine().ToLower();
                string[] text = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string output = "";
                foreach (string word in text)
                {
                    string temp = TranslateWord(word);
                    Console.WriteLine(word + " : " + temp);
                    output += temp + " ";
                }
                Console.WriteLine(input + " : " + output);
                Console.ReadLine();
                do
                {
                    Console.WriteLine("Do you wish to translate anything else? (Y/N):");
                    switch (Console.ReadKey().KeyChar.ToString().ToLower())
                    {
                        case "y":
                            quitLoop = false;
                            break;
                        case "n":
                            quitLoop = true;
                            break;
                        default:
                            quitLoop = null;
                            Console.WriteLine("Please input Y or N");
                            break;
                    }
                } while (quitLoop == null);
            } while (quitLoop != true);
        }

        public string TranslateWord(string word)
        {
            string toPrint = "";
            char[] w = word.ToCharArray();
            Regex onlyCheckLetters = new Regex(@"^[a-z]");

            if (word == "i"
                //(word.Split(new char[] { '\r', '\n' },
                //StringSplitOptions.RemoveEmptyEntries).
                //Where(x=> x=="i").Count() == 1)
                && languageLetterPairs.ContainsKey(">i"))
            {
                toPrint += languageLetterPairs[">i"];
            }
            else
            {
                for (int i = 0; i < w.Length; i++)
                {
                    if (!onlyCheckLetters.IsMatch(w[i].ToString()))
                    {
                        toPrint += w[i].ToString();
                    }
                    else if (i + 2 < w.Length &&
                        languageLetterTripels.ContainsKey(
                            w[i].ToString() +
                            w[i + 1].ToString() +
                            w[i + 2].ToString()))
                    {
                        string s = w[i].ToString() + w[++i].ToString() + w[++i].ToString();
                        toPrint += languageLetterTripels[s];
                    }
                    else if (i + 1 < w.Length &&
                        languageLetterPairs.ContainsKey(
                            w[i].ToString() + w[i + 1].ToString()))
                    {
                        string s = w[i].ToString() + w[++i].ToString();
                        toPrint += languageLetterPairs[s];
                    }
                    else if (languageLetterPairs.ContainsKey(">i") && i > 0 &&
                                (w[i] == 'i' && (w[i - 1] == '\r' || w[i - 1] == '\n')))
                    {
                        toPrint += languageLetterPairs[">i"];
                    }
                    else if (languageLetters.ContainsKey(w[i].ToString()))
                    {
                        toPrint += languageLetters[w[i].ToString()];
                    }
                    else
                    {
                        toPrint += w[i].ToString();
                    }
                }
            }
            return toPrint;
        }

        #region util
        private void TranslatorFuncitonStart()
        {
            Console.Clear();

            if (CheckToReuseKey() != true)
            {
                CreateTranslationKeyDictionarys();
            }
        }

        public bool? CheckToReuseKey()
        {
            bool? reUseKey = null;
            if (languageLetterPairs.Count > 0 ||
                languageLetters.Count > 0 ||
                languageLetterTripels.Count > 0)
            {
                do
                {
                    Console.WriteLine("Do you wish to reuse the loaded Key file (Y/N): ");
                    switch (Console.ReadKey().KeyChar.ToString().ToLower())
                    {
                        case "y":
                            reUseKey = true;
                            break;
                        case "n":
                            reUseKey = false;
                            break;
                        default:
                            Console.WriteLine("Please input Y or N");
                            break;
                    }
                    Console.WriteLine();
                } while (reUseKey == null);
            }
            return reUseKey;
        }

        private void CreateTranslationKeyDictionarys()
        {
            string keyInput;
            Console.Write("Input the name on the Key .txt file: ");
            keyInput = Console.ReadLine();
            if (!keyInput.Contains(".txt"))
            {
                keyInput = keyInput + ".txt";
            }
            languageLetters = new Dictionary<string, string>();
            languageLetterPairs = new Dictionary<string, string>();
            languageLetterTripels = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(dir + @"\Key\" + keyInput))
            {
                string langKeyString = sr.ReadToEnd();
                string[] langKeyArray = langKeyString.ToLower().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string lang in langKeyArray)
                {
                    string[] temp = lang.Split(':');
                    if (temp.Length == 2 && temp[0].Length == 1)
                    {
                        languageLetters.Add(temp[0], temp[1]);
                    }
                    else if (temp.Length == 2 && temp[0].Length == 2)
                    {
                        languageLetterPairs.Add(temp[0], temp[1]);
                    }
                    else if (temp.Length == 2 && temp[0].Length == 3)
                    {
                        languageLetterTripels.Add(temp[0], temp[1]);
                    }
                }
            }
        }

        private string[] ReadInDictionaryFile()
        {
            using (StreamReader sr = new StreamReader(dir + @"\Texts to translate\Dictonary.txt", true))
            {
                string langTextString = sr.ReadToEnd();
                return langTextString.ToLower().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #endregion
    }
}
