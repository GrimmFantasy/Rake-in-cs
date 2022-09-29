using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rakentlk
{
    public class Passage
    {
        public static int ID = 0;
        public string FullPassage { get; set; }
        public List<string> ProsecsPassage { get; set; }
        public string ProsecsPassageAsString { get { return string.Join(' ', ProsecsPassage); } }
        public float rank { get; set; }
        public Passage(string fullPassage, List<string> prosecsPassage)
        {
            ID = ID + 1;
            FullPassage = fullPassage;
            ProsecsPassage = prosecsPassage;
        }
    }
}
