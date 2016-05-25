using System;

namespace FelicidApp.Model
{
    public class EmotionData:Data
    {
        public string Emotion { get; }

        public EmotionData(string id, DateTime timestamp, string emotion):base(id,timestamp)
        {
            Emotion = emotion;
        }

    }
}
