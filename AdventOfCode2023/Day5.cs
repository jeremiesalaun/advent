using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day5
    {
        private class range
        {
            public long rangeStart { get; set; }
            public long rangeEnd { get; set; }
            public long targetStart { get; set; }

            public long offset
            {
                get
                {
                    return targetStart - rangeStart;
                }
            }
            public long GetTarget(long start)
            {
                return offset + start;
            }
        }
        private List<long> seedList=[];
        private List<range> seedRanges = [];
        private List<range> seedToSoilMap = [];
        private List<range> soilToFertMap = [];
        private List<range> fertToWaterMap = [];
        private List<range> waterToLightMap = [];
        private List<range> lightToTempMap = [];
        private List<range> tempToHumMap = [];
        private List<range> humToLocMap = [];

        public void Run()
        {
            //Read and parse file
            var path = @"Inputs\day5.txt";
            using (var sr = new StreamReader(path, true))
            {
                List<range> currentDic=null;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Length == 0)
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        switch (line)
                        {
                            case "seed-to-soil map:": currentDic = seedToSoilMap; break;
                            case "soil-to-fertilizer map:": currentDic = soilToFertMap; break;
                            case "fertilizer-to-water map:": currentDic = fertToWaterMap; break;
                            case "water-to-light map:": currentDic = waterToLightMap; break;
                            case "light-to-temperature map:": currentDic = lightToTempMap; break;
                            case "temperature-to-humidity map:": currentDic = tempToHumMap; break;
                            case "humidity-to-location map:": currentDic = humToLocMap; break;
                        }                     
                    }
                    else if (line.StartsWith("seeds:"))
                    {
                        ParseSeeds(line);
                        ParseSeedsAsRanges(line);
                    }
                    else
                    {
                        ParseRange(line, currentDic);
                    }
                }
            }

            long result1 = long.MaxValue;
            foreach(var seed in seedList)
            {
                var soil = GetTarget(seed,seedToSoilMap);
                var fert =  GetTarget(soil,soilToFertMap);
                var water = GetTarget(fert, fertToWaterMap);
                var light = GetTarget(water, waterToLightMap);
                var temp = GetTarget(light, lightToTempMap);
                var hum =   GetTarget(temp, tempToHumMap);
                var loc =   GetTarget(hum, humToLocMap);
                //Console.WriteLine($"Seed:{seed} Soil:{soil} Fert:{fert} Water:{water} Light:{light} Temp:{temp} Hum:{hum} Loc:{loc}");
               result1=long.Min(result1, loc);
           }
           Console.WriteLine($"Final result for 1st star is : {result1}");

            var soilRanges = GetTargetForRanges(seedRanges, seedToSoilMap);
            var fertRanges = GetTargetForRanges(soilRanges, soilToFertMap);
            var waterRanges = GetTargetForRanges(fertRanges, fertToWaterMap);
            var lightRanges = GetTargetForRanges(waterRanges, waterToLightMap);
            var tempRanges = GetTargetForRanges(lightRanges, lightToTempMap);
            var humRanges = GetTargetForRanges(tempRanges, tempToHumMap);
            var locRanges = GetTargetForRanges(humRanges, humToLocMap);
            long result2 = locRanges.Select(r => r.rangeStart).Min();

            //Print out total result
            Console.WriteLine($"Final result for 2nd star is : {result2}");
        }

        private long GetTarget(long source, List<range> dic)
        {
            var range = dic.FirstOrDefault(r => source>=r.rangeStart && source<=r.rangeEnd);
            if (range != null)
            {
                return range.GetTarget(source);
            }
            return source;
        }

        private List<range> GetTargetForRanges(List<range> source, List<range> dic)
        {
            return source.SelectMany(r => GetTargetForRange(r, dic)).ToList();
        }

        private List<range> GetTargetForRange(range source, List<range> dic)
        {
            var ranges = dic.Where(r => r.rangeEnd >= source.rangeStart && r.rangeStart <= source.rangeEnd)
                .Select(r => new range()
                {
                    rangeStart = r.offset + long.Max(r.rangeStart, source.rangeStart),
                    rangeEnd = r.offset + long.Min(r.rangeEnd, source.rangeEnd)
                })
                .ToList();
            return ranges;
        }

        private void ParseSeedsAsRanges(string line)
        {
            var reseeds = new Regex(@"seeds:(?<range> (?<start>\d+) (?<length>\d+))+");
            var m = reseeds.Match(line);
            if (m.Success)
            {
                for(int i = 0; i < m.Groups["start"].Length; i++)
                {
                    var start = long.Parse(m.Groups["start"].Captures[i].Value);
                    var length = long.Parse(m.Groups["length"].Captures[i].Value);
                    seedRanges.Add(new range() { rangeStart = start, rangeEnd = start + length - 1 });
                }
            }
        }
        private void ParseSeeds(string line)
        {
            var reseeds = new Regex(@"seeds:( (?<seednum>\d+))+");
            var m = reseeds.Match(line);
            if (m.Success)
            {
                seedList = m.Groups["seednum"].Captures.Select(c => long.Parse(c.Value)).ToList();
            }
        }

        private void ParseRange(string line,List<range> dic)
        {
            var rerange = new Regex(@"(?<targetstart>\d+) (?<sourcestart>\d+) (?<rangelength>\d+)");
            var m = rerange.Match(line);
            if (m.Success)
            {
                long targetstart = long.Parse(m.Groups["targetstart"].Value);
                long sourcestart = long.Parse(m.Groups["sourcestart"].Value);
                long rangelength = long.Parse(m.Groups["rangelength"].Value);
                dic.Add(new range() {rangeStart=sourcestart,rangeEnd=sourcestart+rangelength-1,targetStart=targetstart });
            }
        }
    }
}
