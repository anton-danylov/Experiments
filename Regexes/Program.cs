using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace Regexes
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(Regex.Match("Hello Worrrrld!", "(?i)(w.+?d)").Value);

            Console.WriteLine(Regex.Matches("Hello Worrrrld! test test2", @"\b(\w+)\b")
                .Select(m => m.Value)
                .Aggregate((curr, next) => curr + ", " + next)); // Hello, Worrrrld, test, test2



            string html = "<b>some text in bold</b> normal text <b>text in bold</b> normal again";

            Console.WriteLine(Regex.Matches(html, @"(?<=<b>)(.*?)(?=</b>)")
                .Select(m => m.Value)
                .Aggregate((curr, next) => curr + ", " + next)); // some text in bold, text in bold


            Console.WriteLine("===========================================");
            string html2 = "<h1>Title</h1> <b>some text in bold</b> normal text <b>text in bold</b> normal again <i>itaaaalic</i>";          
            string findTags = @"<(?'tag'\w+)>(?'content'.*?)</\k'tag'>";
            
            Console.WriteLine(Regex.Matches(html2, findTags)
                .Select(m => $"{m.Groups[1]}:  {m.Groups[2]}")
                .Aggregate((curr, next) => curr + "\n" + next));
            // h1:  Title
            // b:  some text in bold
            // b:  text in bold
            // i:  itaaaalic
            

            string text = "asdas 44 tryryt 66 dgdfgdg 77 fgfhjgf fghjg zxcz 33";

            Console.WriteLine(Regex.Replace(text, @"\d+", "($0)")); // asdas (44) tryryt (66) dgdfgdg (77) fgfhjgf fghjg zxcz (33)
            Console.WriteLine("===========================================");


        }
    }
}
