using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day2
    {
        private class Game
        {
            public Game(int id)
            {
                this.id = id;
                this.cubes = new Dictionary<string, int> { { "red", 0 }, { "green", 0 }, { "blue", 0 } };
            }
            public int id { get; set;}
            public Dictionary<string,int> cubes { get; set;}

            public override string ToString()
            {
                return $"Id={id},Red={cubes["red"]},Green={cubes["green"]},Blue={cubes["blue"]},Power={Power()}";
            }

            public void NewMin(int red,int green, int blue)
            {
                this.cubes["red"] = Math.Max(red, this.cubes["red"]);
                this.cubes["green"] = Math.Max(green, this.cubes["green"]);
                this.cubes["blue"] = Math.Max(blue, this.cubes["blue"]);
            }

            public bool IsPossible(int red,int green,int blue)
            {
                return red >= this.cubes["red"] && green >= this.cubes["green"] && blue >= this.cubes["blue"];
            }

            public int Power()
            {
                return this.cubes["red"] * this.cubes["blue"] * this.cubes["green"];
            }
        }
        public void Run()
        {
            var games = new List<Game>();
            //Read file
            var path = @"Inputs\day2.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    //For each line in file, parse the game and add to list
                    games.Add(ParseGame(sr.ReadLine()));
                }
            }
            int total1 = 0;
            int total2 = 0;
            foreach (var game in games)
            {
                if (game.IsPossible(12, 13, 14))
                {
                    total1 += game.id;
                }
                total2 += game.Power();
            }
            //Print out total result
            Console.WriteLine($"Final result for 1st star is : {total1}");
            Console.WriteLine($"Final result for 2nd star is : {total2}");
        }

        private Game ParseGame(string line)
        {
            if (line == null) return null;
            var rered = new Regex(@".* (\d*) red");
            var reblue = new Regex(@".* (\d*) blue");
            var regreen = new Regex(@".* (\d*) green");
            int i = line.IndexOf(":");
            int id = int.Parse(line.Substring(5, i - 5));
            var result = new Game(id);
            line = line.Substring(i + 1);
            var shows = line.Split(";");
            for (int j = 0; j < shows.Length; j++)
            {
                var mred = rered.Match(shows[j]);
                int red = mred.Success ? int.Parse(mred.Groups[1].Value) : 0;
                var mgreen = regreen.Match(shows[j]);
                int green = mgreen.Success ? int.Parse(mgreen.Groups[1].Value) : 0;
                var mblue = reblue.Match(shows[j]);
                int blue = mblue.Success ? int.Parse(mblue.Groups[1].Value) : 0;
                result.NewMin(red,green, blue);
            }
            Console.WriteLine($"Game for line {line} is {result}");
            return result;
        }
    }
}
