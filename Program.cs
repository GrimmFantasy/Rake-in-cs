using System;
using System.IO;
using System.Text;
namespace rakentlk{

    class main{

        static void Main(string[] args) {
              
            Rake r = new(punctuation: new List<string> { ",", ". "});
            
            List<string> lines = new List<string>();
            // Open the stream and read it back.
            using (FileStream fs = File.Open("test.txt", FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b,0,b.Length) > 0)
                {
                    lines.Add(temp.GetString(b));
                }
                
            }
            r.extract_keywords_from_text(String.Join(' ', lines));
            List<string> phrases = r.get_ranked_phrases_without_scores();
            Console.WriteLine(phrases.Count());
            foreach (var item in phrases)
            {
                Console.WriteLine("here");
                Console.WriteLine(item);
            }
        }
    }

}