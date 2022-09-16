using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui
{
    public class SearchResult
    {
        public string Title;
        public string Description;
        public string Link;

        public SearchResult(string title, string desc, string url)
        {
            Title = title;
            Description = desc;
            Link = url;
        }

    }
}
