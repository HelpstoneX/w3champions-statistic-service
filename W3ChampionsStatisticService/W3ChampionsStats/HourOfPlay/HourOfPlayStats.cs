using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using W3ChampionsStatisticService.Matches;

namespace W3ChampionsStatisticService.W3ChampionsStats.HourOfPlay
{
    public class HourOfPlayStats
    {
        public void Apply(GameMode gameMode, DateTimeOffset day)
        {
            var gameLengthPerMode = PlayTimesPerMode.SingleOrDefault(m => m.GameMode == gameMode
                                                          && m.Day == day.Date);

            if (gameLengthPerMode == null)
            {
                PlayTimesPerMode.Remove(PlayTimesPerMode[0]);
                PlayTimesPerMode.Remove(PlayTimesPerMode[1]);
                PlayTimesPerMode.Remove(PlayTimesPerMode[2]);
                PlayTimesPerMode.Remove(PlayTimesPerMode[3]);

                AddDay(PlayTimesPerMode, GameMode.GM_1v1, 0);
                AddDay(PlayTimesPerMode, GameMode.GM_2v2, 0);
                AddDay(PlayTimesPerMode, GameMode.GM_4v4, 0);
                AddDay(PlayTimesPerMode, GameMode.FFA, 0);
            }

            gameLengthPerMode = PlayTimesPerMode.Single(m => m.GameMode == gameMode
                                                          && m.Day == day.Date);

            gameLengthPerMode.Record(day);
        }

        [JsonIgnore]
        public List<HourOfPlayPerMode> PlayTimesPerMode { get; set; } = new List<HourOfPlayPerMode>();

        public string Id { get; set; } = nameof(HourOfPlayStats);

        public static HourOfPlayStats Create()
        {
            return new HourOfPlayStats
            {
                PlayTimesPerMode = Create14DaysOfPlaytime()
            };
        }

        private static List<HourOfPlayPerMode> Create14DaysOfPlaytime()
        {
            var hours = new List<HourOfPlayPerMode>();
            for (int i = 0; i < 14; i++)
            {
                AddDay(hours, GameMode.GM_1v1, i);
                AddDay(hours, GameMode.GM_2v2, i);
                AddDay(hours, GameMode.GM_4v4, i);
                AddDay(hours, GameMode.FFA, i);
            }

            return hours;
        }

        private static void AddDay(List<HourOfPlayPerMode> hours, GameMode gameMode, int i)
        {
            hours.Add(new HourOfPlayPerMode
            {
                GameMode = gameMode,
                PlayTimePerHour = CreateLengths(),
                Day = DateTime.Today.AddDays(-i)
            });
        }

        private static List<HourOfPlay> CreateLengths()
        {
            var lengths = new List<HourOfPlay>();
            var now = DateTimeOffset.UtcNow;
            for (var i = 0; i <= 96; i++) // every 15 minutes
            {
                lengths.Add(new HourOfPlay { Time = now.AddMinutes(i * 15)});
            }

            return lengths;
        }
    }
}