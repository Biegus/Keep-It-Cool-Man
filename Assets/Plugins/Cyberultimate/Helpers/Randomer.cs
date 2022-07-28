#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections;
using Random = System.Random;

namespace Cyberultimate.Unity
{
    

    public class Randomer
    {
        public int? Seed { get; }
        private readonly Random random;
        public static readonly Randomer Base = new Randomer();

        public Randomer(int seed)
        {
            Seed = seed;
            random = new Random(seed);
        }

        public Randomer()
        {
            Seed = null;
            random = new Random();
        }

        public int NextInt(int min = int.MinValue, int max = int.MaxValue)
        {
            return random.Next(min, max);
        }
        public int NextPositiveOrNegative()
        {
            return NextInt(0, 1) * 2 - 1;
        }

        public double NextDouble(double min = double.MinValue, double max = double.MaxValue)
        {
            return random.NextDouble() * (max - min) + min;

        }

        public float NextFloat(float min = float.MinValue, float max = float.MaxValue)
        {
            return (float) (NextDouble(min, max));
        }

        public Vector2 NextVector2(Vector2 min, Vector2 max)
        {
            float x = NextFloat(min.x, max.x);
            float y = NextFloat(min.y, max.y);
            return new Vector2(x, y);
        }

        public Vector2 NextVector2()
        {
            return NextVector2(new Vector2(float.MinValue / 1000, float.MinValue / 1000),
                new Vector2(float.MaxValue / 1000, float.MaxValue / 1000));
        }

        public Direction NextDirection()
        {
            return Direction.FromAngle(NextFloat(0, 360));
        }
        public bool NextBool()
        {
            return NextInt(0, 2) != 0;
        }
        public T NextRandomElement<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (collection.Any() == false)
                return default;
            var ar = collection.ToArray(); //count takes the same amount of time.
            return ar[NextInt(0, ar.Length)];


        }
        public float RandomAngleInDegree()
        {
            return this.NextFloat(0, 360);
        }
        public void ShuffleIn<T>(IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            for (int i = 0; i < collection.Count; i++)
            {
                 int place= this.NextInt(i,collection.Count);
                 Swap(collection,i,place);
            }
        }
        private void Swap<T>(IList<T> collection,int a, int b)
        {
            if (a == b) return;
            T temp = collection[a];
            collection[a] = collection[b];
            collection[b] = temp;

        }

        public T NextRandomElement<T>(IList<T> collection)
        {
            return collection[NextInt(0, collection.Count)];
        }

        public IEnumerable<T> NextOrder<T>(IList<T> collection)
        {
            List<int> ready = Enumerable.Range(0, collection.Count).ToList();

            for (int x = 0; x < collection.Count; x++)
            {
                var next = NextRandomElement(ready);
                ready.Remove(next);
                yield return collection[next];
            }
        }
        public Vector2 NextRandomRotation(Vector2 pos, float angle, Vector2 target)
        {
            return NextRandomRotation((target - pos), angle);
        }
        public Vector2 NextRandomRotation(Vector2 vec, float angle)
        {
            return (vec).GetRotated(this.NextFloat(-angle, +angle));
        }

    }
}