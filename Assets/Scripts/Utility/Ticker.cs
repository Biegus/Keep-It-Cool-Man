using System;
using UnityEngine;

namespace LetterBattle.Utility
{
    public interface ITicker
    {
        public bool Done { get; }
        public float Passed { get; }
        public float RequiredTime { get; }
    }
    public struct TickerCreator
    {
        public static Ticker CreateNormalTime(float time)
        {
            return new Ticker(time, () => Time.time);
        }

        public static Ticker CreateUnscaledTime(float time)
        {
            return new Ticker(time, () => Time.unscaledTime);
        }
    }
    public struct Ticker : ITicker
    {
        public float RequiredTime { get; set; }
        public float Passed => Time.time - startTime;
        public float EndTime => startTime + RequiredTime;
        public bool Done => Passed >= RequiredTime;
        public Func<float> TimeGetter { get; }

        private float startTime;
        public Ticker(float requiredTime, Func<float> timeGetter)
        {
            RequiredTime = requiredTime;
            this.TimeGetter = timeGetter;
            startTime = TimeGetter();

        }
        public void Reset()
        {
            startTime =  TimeGetter();
        }
        public void SetPassed(float value)
        {
            startTime = TimeGetter() - value;
        }

        public void ForceFinish()
        {
            SetPassed(RequiredTime);
        }
        
        //returns if it's finished and resets in that case
        public bool Push()
        {
            bool res = Done;
            if(res)
                Reset();
            return res;
        }
    }
}