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
        public string ProsecsPassage { get; set; }
        public Passage(string fullPassage, string prosecsPassage)
        {
            ID = ID + 1;
            FullPassage = fullPassage;
            ProsecsPassage = prosecsPassage;
        }
    }
}
