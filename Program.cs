using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace rakentlk{

    class main{

        static void Main() {
              
            Rake r = new();
            
            
            string fileName = @"..\\..\\..\\test.txt";

            string text = File.ReadAllText(fileName);
            r.extract_keywords_from_text(text);
            List<string> phrases = r.get_ranked_phrases_without_scores();
            foreach (var phrase in r.passages)
            {
                
                Console.WriteLine(phrase.FullPassage);
                Console.WriteLine(phrase.ProsecsPassage);

            }

        }
    }

}