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
    internal class Day7
    {
        private class hand
        {
            public char[] cards { get; set; } = new char[5];
            public int bid { get; set; }
            public Dictionary<char,int> cardCount { get; set; }

            public void CountCards()
            {
                cardCount = new Dictionary<char,int>();
                for(int i = 0; i < cards.Length; i++)
                {
                    if (cardCount.ContainsKey(cards[i]))
                    {
                        cardCount[cards[i]]++;
                    }
                    else
                    {
                        cardCount.Add(cards[i], 1);
                    }
                }
            }

            public void CountCardWithJs()
            {
                cardCount = new Dictionary<char, int>();
                int nbJokers = 0;
                for (int i = 0; i < cards.Length; i++)
                {
                    if (cards[i] == 'J')
                    {
                        nbJokers++;
                    }
                    else if (cardCount.ContainsKey(cards[i]))
                    {
                        cardCount[cards[i]]++;
                    }
                    else
                    {
                        cardCount.Add(cards[i], 1);
                    }
                }
                if (cardCount.Count == 0)
                {
                    //Special case of JJJJJ
                    cardCount.Add('J', 5);
                }
                else if(nbJokers > 0)
                {
                    var maxCard = cardCount.OrderByDescending(kvp => kvp.Value).ThenByDescending(kvp => cardValueWithJs(kvp.Key)).Select(kvp => kvp.Key).First();
                    cardCount[maxCard] += nbJokers;
                }
            }

            public List<(int count,char card)> GetHandStrength()
            {
                if (cardCount == null) return null;
                return cardCount.OrderByDescending(kvp => kvp.Value).Select(kvp => (count:kvp.Value, card:kvp.Key)).ToList();
            }

            public static int cardValue(char card)
            {
                switch (card)
                {
                    case 'A':return 14;
                    case 'K': return 13;
                    case 'Q': return 12;
                    case 'J': return 11;
                    case 'T': return 10;
                    default: return int.Parse(new string(card, 1));
                }
            }
            public static int cardValueWithJs(char card)
            {
                switch (card)
                {
                    case 'A': return 14;
                    case 'K': return 13;
                    case 'Q': return 12;
                    case 'J': return 1;
                    case 'T': return 10;
                    default: return int.Parse(new string(card, 1));
                }
            }
        }

        private class handComparer : IComparer<hand>
        {
            public int Compare(hand? x, hand? y)
            {
                var s1 = x.GetHandStrength();
                var s2 = y.GetHandStrength();
                if (s1[0].count != s2[0].count)
                {
                    return s1[0].count - s2[0].count;
                }
                else
                {
                    if (s1.Count>1 && s1[1].count != s2[1].count)
                    {
                        return s1[1].count - s2[1].count; //Full>Brelan and 2 Pairs> 1 Pair
                    }
                    else
                    {
                        for(int i = 0; i < 5; i++)
                        {
                            if (x.cards[i]!= y.cards[i])
                            {
                                return hand.cardValue(x.cards[i]) - hand.cardValue(y.cards[i]);
                            }
                        }
                        return 0;
                    }
                }
            }
        }

        private class handComparerWithJs : IComparer<hand>
        {
            public int Compare(hand? x, hand? y)
            {
                var s1 = x.GetHandStrength();
                var s2 = y.GetHandStrength();
                if (s1[0].count != s2[0].count)
                {
                    return s1[0].count - s2[0].count;
                }
                else
                {
                    if (s1.Count > 1 && s1[1].count != s2[1].count)
                    {
                        return s1[1].count - s2[1].count; //Full>Brelan and 2 Pairs> 1 Pair
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (x.cards[i] != y.cards[i])
                            {
                                return hand.cardValueWithJs(x.cards[i]) - hand.cardValueWithJs(y.cards[i]);
                            }
                        }
                        return 0;
                    }
                }
            }
        }

        public void Run()
        {
            Console.WriteLine("#################################### WELCOME TO DAY 7 !! ##################################\r\n");
            var hands = new List<hand>();
            //Read and parse file
            var path = @"Inputs\day7.txt";
            using (var sr = new StreamReader(path, true))
            {
                while (!sr.EndOfStream)
                {
                    hands.Add(ParseLine(sr.ReadLine()));
                }
            }
            int total1 = 0;
            hands.ForEach(h => h.CountCards());
            hands.Sort(new handComparer());
            for(int i=hands.Count-1; i>=0; i--)
            {
                //var h = hands[i];
                //Console.WriteLine($"Hand #{i+1} is {new string(h.cards)}, its bid is {h.bid}, strength is {FormatStrength(h.GetHandStrength())}");
                total1 += (i + 1) * hands[i].bid;
            }
            Console.WriteLine($"Final result for 1st star is : {total1}");

            int total2 = 0;
            hands.ForEach(h => h.CountCardWithJs());
            hands.Sort(new handComparerWithJs());
            for (int i = hands.Count - 1; i >= 0; i--)
            {
                //var h = hands[i];
                //Console.WriteLine($"Hand #{i+1} is {new string(h.cards)}, its bid is {h.bid}, strength is {FormatStrength(h.GetHandStrength())}");
                total2 += (i + 1) * hands[i].bid;
            }
            //Print out total result
            Console.WriteLine($"Final result for 2nd start is : {total2}");
            Console.WriteLine($"**************************** END OF DAY 7 ***********************************\r\n");
            Thread.Sleep(1000);
        }

        private string FormatStrength(List<(int count, char card)> list)
        {
            var r = new StringBuilder();
            foreach (var card in list)
            {
                r.Append($"{card.card}:{card.count}-");
            }
            return r.ToString();
        }

        

        private hand ParseLine(string line)
        {
            var cards = line.Substring(0, 5);
            var sbid = line.Substring(6);
            var result = new hand()
            {
                bid = int.Parse(sbid),
                cards = cards.ToCharArray(),
            };
            return result;
        }


    }
}
