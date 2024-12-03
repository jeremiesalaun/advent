using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Scoreboard
    {
        private class board
        {
            public Dictionary<string,member> members {  get; set; }

            public List<member> GetTopMembersByLocalStore(int count)
            {
                return members.OrderByDescending(m => m.Value.local_score).Take(count).Select(m => m.Value).ToList();
            }
        }

        private class member
        {
            public int id { get; set; }
            public string name { get; set; }
            public int stars { get; set; }
            public int local_score { get; set; }
            public int global_score { get; set; }
            public int last_star_ts { get; set; }

            public DateTime last_star
            {
                get
                {
                    return DateTime.UnixEpoch.AddSeconds(last_star_ts);
                }
            }

            public Dictionary<int,day_completion> completion_day_level { get; set; }
        }

        private class day_completion
        {
            public int num { get; set; }
            [JsonPropertyName("1")]
            public star star1 { get; set; }
            [JsonPropertyName("2")]
            public star star2 { get; set; }
        }

        private class star
        {
            public int star_index { get; set; }
            public int get_star_ts { get; set; }
            public DateTime get_star
            {
                get
                {
                    return DateTime.UnixEpoch.AddSeconds(get_star_ts);
                }
            }
        }

        public async static Task Show()
        {
            board b;
            var path = @"Inputs\scoreboard.json";
            using (var fr = new FileStream(path, FileMode.Open))
            {
                b = JsonSerializer.Deserialize<board>(fr);
            }

            //TODO Find how to implement authentication
            //var url = @"https://adventofcode.com/2023/leaderboard/private/view/1147666.json";
            //using (var client = new HttpClient())
            //{
            //    using (var s = await client.GetStreamAsync(url))
            //    {
            //        b = JsonSerializer.Deserialize<board>(s);
            //    }
            //}
            var topmembers = b.GetTopMembersByLocalStore(20);
            for(int i=0;i<topmembers.Count;i++)
            {
                Console.WriteLine($"#{i+1} {topmembers[i].local_score} {topmembers[i].name}");
            }
            Console.WriteLine($"{b.members.Count} members");
        }
    }
}
