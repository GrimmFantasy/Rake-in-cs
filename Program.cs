using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace rakentlk{

    class main{

        static void Main() {
              
            Rake r = new(punctuation: new List<string> {". "});
            
            List<string> lines = new List<string>();
            // Open the stream and read it back.
            using (FileStream fs = File.Open("C:\\Users\\capse\\Documents\\GitHub\\Rake_NTLK_IN_CS\\test.txt", FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b,0,b.Length) > 0)
                {
                    lines.Add(temp.GetString(b));
                }
                
            }
            r.extract_keywords_from_text(string.Join(' ', lines));
            List<string> phrases = r.get_ranked_phrases_without_scores();
            foreach (var item in phrases)
            {
                Console.WriteLine(item);
            }
        }
    }

}