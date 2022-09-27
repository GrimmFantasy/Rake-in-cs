using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace rakentlk{

    class main{

        static void Main() {
              
            Rake r = new(punctuation: new List<string> {". "});
            
            
            string fileName = @"C:\Users\capse\Documents\GitHub\Rake_NTLK_IN_CS\test.txt";

            string text = File.ReadAllText(fileName);
            r.extract_keywords_from_text(text);
            List<string> phrases = r.get_ranked_phrases_without_scores();
            foreach (string phrase in phrases)
            {
                Console.WriteLine(phrase);
            }

        }
    }

}