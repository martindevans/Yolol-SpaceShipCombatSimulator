using System;
using System.Collections.Generic;
using System.Linq;
using StrategyList = System.Collections.Generic.List<System.Tuple<float, System.Func<System.Random, string>>>;

namespace ShipCombatCore.Name
{
    public static class ShipName
    {
        private static readonly IReadOnlyList<string> PremadeNames = new[] {
            "Ever Given",
            "A Bit Slow",
            "Let That Sink In",
            "Wow That Blew Up",
            "Normalise This",
            "Fun Fact",
            "We Need To Talk",
            "It's Not Me, It's You",
            "Spaceship Removal Technician",
            "Social Lubricant",
            "You Have No Authority Here",
            "Peaceful Transition Of Power",
            "Probable Scrap",
            "Never Knowingly Undersold",
            "Unconscious Bias",
            "Conscious Bias",
            "Be Specific",
            "No, Not That Specific",
            "Reasonable Worst Case",
            "Panic Is Fatal",
            "Acceptable Causality Rate",
            "Farewell, Old Friend",
            "The Hand of God",
            "So Much For Subtlety",
            "A Series Of Unlikely Explanations",
            "Only Slightly Bent",
            "Ultimate Ship II",
            "Ultimate Ship III",
            "Ultimate Ship IV",
            "Ultimate Ship V",
            "Ultimate Ship VI",
            "Ultimate Ship VII",
            "You'll Thank Me Later",
            "Outside Context Problem",
            "Problem Child",
            "Attitude Adjuster",
            "I Blame Your Mother",
            "Someone Else's Problem",
            "Back By Popular Demand",
            "Four Sigma Event",
            "Stranger in the Night",
            "They Couldn't Hit An Elephant At This Dist...",
            "Everything I say Is A Lie",
            "The Mind Killer",
            "A sufficiently advanced technology",
            "Worth The Wait",
            "Worth The Weight",
        };

        private static readonly IReadOnlyList<string> String1 = new[] {
            "Gravitas", "Gravity", "Star", "Stellar", "Neutron", "Proton", "Photon", "Plasma",
            "Ethics", "Honor", "Loyalty", "Faith",
            "Love", "Lust", "Gluttony", "Greed", "Sloth", "Wrath", "Envy", "Pride", "Acedia", "Vainglory", "Vanity", "Blasphemy",
            "Temperance", "Charity", "Diligence", "Patience", "Gratitude", "Kindness", "Humility", "Faith",
            "Danger", "Anger", "Combat", "Savage",
            "Jovian", "Lunar", "Martian", "Terran",
            "Warp", "Immaterium", "Aether", "Alien",
            "Wild", "Feral", "Ferocious", "Savage", "Barbaric", "Fierce", "Rampant", "Vicious",
            "Kind", "Tame", "Gentle", "Orderly", "Delicate",
            "Nyarlathothep's", "Yolathothep's", "Dude's", "Rad's", "Matrix's", "Nyefari's", "Pyry's", "Scoundrel's",
            "Zijkhal's", "Opux's", "Graham's", "Drelnoch's", "Azurethi's", "Hourd's", "Ayfid's", "Chronojam's",
            "Genius",
            "Locust", "Wasp", "Beetle", "Dragonfly",
            "Vindictive", "Cruel", "Malicious", "Merciless", "Resentful", "Grim", "Avenging", "Malignant", "Venomous", "Unrelenting", "Nervous", "Prosthetic",
            "Cylon", "Collective", "Frozenbyte",
            "Etnernal", "Neverending", "Everlasting", "Endless", "Perpetual", "Immutable", "Immortal",
            "Desperate", "Bold", "Daring", "Frantic", "Frenzied", "Furious", "Violent", "Audacious", "Scandalous", "Wild"
        };

        private static readonly IReadOnlyList<string> String2 = new[] {
            "Delusion", "Deception", "Fantasy", "Illusion", "Dream", "Ghost", "Phantasm", "Nightmare", "Nonsense",
            "Chimera", "Vampire", "Banshee", "Unicorn", "Phoenix", "Nymph", "Faerie", "Sprite", "Goblin", "Oni", "Demon", "Golem",
            "Factor", "Aspect", "Element",
            "Gradient", "Angle", "Incline", "Reduction", "Reducer", "Tilt", "Margin",
            "Glorification",
            "Avatar", "Icon", "Channel", "Carrier", "Denial", "Essence", "Soul", "Paradigm",
            "Heart", "Spirit", "Aspect", "Kernel", "Axis", "Hub", "Locus", 
            "Revenge", "Reprisal", "Attack", "Retribution", "Vengeance", "Return",
            "Energy",
            "Optimist", "Pessimist",
            "Shortfall", "Demise",
            "Gambit", "Artifice", "Ploy", "Ruse", "Agent",
            "Jam", "Marmalade", "Jelly", "Confection", "Extract", "Spread", "Marmite",
            "Truth",
            "Heart",
            "Charity",
            "Ratio",
        };

        private static readonly IReadOnlyList<string> ShipClass = new[] {
            "GSV", "MSV", "LSV", "GCV", "GCU", "LCU", "ROU", "GOU", "LOU", "VFP"
        };

        private static readonly StrategyList NamingStrategies = new()
        {
            Weighted(1f, TwoWords),
            Weighted(0.1f, PrefixClass(TwoWords)),
            Weighted(0.1f, Premade),
            //Weighted(0.01f, PrefixClass(Premade)),
            //Weighted(0.1f, IdCode),
            //Weighted(0.1f, r => PrefixClass(r, IdCode)),
        };

        // Naming Strategies
        private static string TwoWords(Random random)
        {
            return $"{RandomChoice(random, String1)} {RandomChoice(random, String2)}";
        }

        private static string Premade(Random random)
        {
            return $"{RandomChoice(random, PremadeNames)}";
        }

        private static Func<Random, string> PrefixClass(Func<Random, string> func)
        {
            return random => {
                var prefix = RandomChoice(random, ShipClass);
                return $"{prefix} {func(random)}";
            };
        }

        private static Tuple<float, Func<Random, string>> Weighted(float f, Func<Random, string> s)
        {
            return Tuple.Create(f, s);
        }

        private static T RandomChoice<T>(this Random random, IEnumerable<T> choices)
        {
            var choiceList = choices.ToList();
            var count = choiceList.Count;
            var index = random.Next(0, count);
            return choiceList.ElementAt(index);
        }

        private static T WeightedChoice<T>(this Random random, IReadOnlyCollection<Tuple<float, T>> choices)
        {
            var totalWeight = choices.Sum(x => x.Item1);
            var choice = random.NextDouble() * totalWeight;
            return choices.First(c => {
                choice -= c.Item1;
                return (choice <= 0);
            }).Item2;
        }

        public static string Generate(Random random)
        {
            return random.WeightedChoice(NamingStrategies)(random);
        }
    }
}