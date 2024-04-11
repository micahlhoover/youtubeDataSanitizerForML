// See https://aka.ms/new-console-template for more information

using System.IO;
using System.Text;

enum ResultType
{
    Views,
    Classification
};

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //ResultType resultType = ResultType.Views;
            ResultType resultType = ResultType.Classification;

            string fileloc = @"C:\Users\m.l.hoover\Desktop\youTubeData\youtube.csv";
            IEnumerable<string> lines = File.ReadLines(fileloc);

            StringBuilder sb = new StringBuilder();

            long sampleCount = 0;
            long rejectCount = 0;
            long lowestViewCount = 10000L;
            long highestViewCount = 0L;
            double runningSum = 0.0;

            foreach (var line in lines)
            {
                if (line.StartsWith("index,video_id,trending_date"))
                    continue;

                int commaCount = line.Count(f => f == ',');
                //if (commaCount > 17)
                //{
                //    rejectCount++;
                //    continue;
                //}

                // example : 0,2kyS6SvSYSE,17.14.11,WE WANT TO TALK ABOUT OUR MARRIAGE,CaseyNeistat,22,13/11/2017,17:00 to 17:59,Monday,US,SHANtell martin,748374,57527,2966,15954,False,False,False
                // views is 748374 and title is WE WANT TO TALK ABOUT OUR MARRIAGE
                //var title = line.Split(',')[3];
                var title = line.Split(',')[3];
                //var views = line.Split(",")[11];        // OR line[line.Length - 6] 
                var views = line.Split(",")[line.Split(",").Length - 7];

                // bah ! if you're going to rescue the lines with commas in the titles ... the above works to get the views but to get the full title
                // you have to span several columns
                // example : "What $4,145921        (on row 25)
                // from 23,JBZTZZAcFTw,17.14.11,"What $4,800 Will Get You In NYC | Sweet Digs Home Tour | Refinery29",Refinery29,26,12/11/2017,16:00 to 16:59,Sunday,US,"refinery29|""refinery 29""""|""""r29""""|""""r29 video""""|""""video""""|""""refinery29 video""""|""""female""""|""""empowerment""""|""""house tour""""|""""sweet digs""""|""""new york city""""|""""apartment decor""""|""""interior design""""|""""home tour""""|""""big apple""""|""""real estate""""|""""apartment""""|""""new apartment tour""""|""""bedroom tour""""|""""new apartment""""|""""house tour 2017""""|""""living room""""|""""living room decor""""|""""walk through""""|""""nyc apartment""""|""""new house""""|""""kitchen tour""""|""""modern home""""|""""decorating on a budget""""|""""my new house""""|""""moving out""""|""""video blog""""|""""manhattan""""|""""NY""""|""""central park""""""",145921,1707,578,673,False,False,False
                // has 18 commas

                if (line.Contains("Will Get You In NYC"))
                {
                    Console.WriteLine("this is the Will Get You In NYC one");
                }

                int extraColsForTitle = line.Split(",").Length - 18;
                //int startColForTitle = line.Split(",").Length - 14;
                int startColForTitle = 4;
                for (int i = 0; i < extraColsForTitle; i++)
                {
                    title += line.Split(",")[startColForTitle + i];
                }

                var isNumeric = int.TryParse(views, out _);
                if (!isNumeric)
                {
                    Console.WriteLine($"Comma count: {commaCount}");
                    Console.WriteLine($"This number of views is not numeric : {views} ?");
                    Console.WriteLine($"original line : '{line}' \r\n");
                    rejectCount++;
                    continue;
                }

                long viewCount = long.Parse(views);
                if (viewCount < lowestViewCount)
                {
                    lowestViewCount = viewCount;
                }
                if (viewCount > highestViewCount)
                {
                    highestViewCount = viewCount;
                }
                runningSum += viewCount;


                //Console.Write(line);
                // Console.WriteLine($"Title : {title}, Views : {views}");  // frabulous

                sampleCount++;
                if (resultType == ResultType.Views)
                {
                    sb.AppendLine($"{title},{views}");
                } else if (resultType == ResultType.Classification)
                {
                    var highOrLow = (viewCount > 2419854.09) ? "1" : "0";
                    sb.AppendLine($"{title}, {highOrLow}");
                }
            }

            if (resultType == ResultType.Views)
            {
                File.WriteAllText("C:\\Users\\m.l.hoover\\Desktop\\youTubeData\\cleasedData.csv", sb.ToString());
            } else if (resultType == ResultType.Classification)
            {
                File.WriteAllText("C:\\Users\\m.l.hoover\\Desktop\\youTubeData\\cleasedDataClassification.csv", sb.ToString());
            }
            Console.WriteLine($"Highest view count : {highestViewCount} items");    //  424538912 items
            Console.WriteLine($"Lowest view count : {lowestViewCount} items");      // 223 items
            Console.WriteLine($"Rejected {rejectCount} items");
            Console.WriteLine($"Accepted {sampleCount} items");
            Console.WriteLine($"Mean views: {runningSum / sampleCount}");           // Mean views: 2419854.0957391467

            Console.WriteLine("Press ENTER to continue.");

            Console.ReadLine();

        }
    }
}