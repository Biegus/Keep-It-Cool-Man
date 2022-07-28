using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LetterBattle
{
    public static class InputCharHelper
    {
        private static char[] cache;
        private static int cachedFrame = -1;
        public static IList<char> GetPressedKeys(bool includeNumbers = false)
        {
            if (cache == null || cachedFrame != Time.frameCount)
                cache = InternalGetPressedKeys(includeNumbers).ToArray();
            cachedFrame = Time.frameCount;
            return cache;
        }
        private static IEnumerable<char> InternalGetPressedKeys(bool includeNumbers)
        {
            const KeyCode FIRST = KeyCode.A;
            const KeyCode LAST = KeyCode.Z;

            if (!Input.anyKeyDown) yield break;

            if (includeNumbers)
			{
                const KeyCode FIRST_NUM = KeyCode.Alpha0;
                const KeyCode LAST_NUM = KeyCode.Alpha9;
                for (KeyCode i = FIRST_NUM; i <= LAST_NUM; i++)
                {
                    if (Input.GetKeyDown(i))
					{
                        yield return i.ToString()[i.ToString().Length - 1];
                       
                    }

                }
            }

            for (KeyCode i = FIRST; i <= LAST; i++)
            {
                if (Input.GetKeyDown(i))
                    yield return i.ToString()[0];
            }

        }
    }
}