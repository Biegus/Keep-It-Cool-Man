using System;
using System.Text;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using UnityEngine;

namespace LetterBattle
{
#if UNITY_EDITOR
    public partial class CutsceneAsset
    {

        [SerializeField] [ResizableTextArea] private string simplified;
        private Regex lineRegex = new Regex(@"->\s* \[(?'who'.*?)\] (?'text'.*?)(?=\n*->|$)", RegexOptions.Singleline);

        [NaughtyAttributes.Button("Refresh")]
        public void Get()
        {
            simplified = GetSimplified();
        }

        [Button("Inject")]
        public void InjectFromBox()
        {
            Inject(simplified);
        }
        
        
        public string GetSimplified()
        {
            StringBuilder builder = new StringBuilder();
            int index = 0;
            foreach (CutsceneElement element in elements)
            {
                if (element.Type == CutsceneElementType.Plain)
                    builder.Append($"-> [{element.GetPlain().Who}] {element.GetPlain().Text.Replace("\n","\\n").Replace("\r","")}");
                else
                    builder.Append($"-> [_{element.GetSpecial().Time}] {element.GetSpecial().ShowText.Replace("\n","\\n").Replace("\r","")} ");
                index++;
                builder.Append("\n");
            }

            return builder.ToString();
        }
        public void Inject(string code)
        {
            int id = 0;
            MatchCollection matches = lineRegex.Matches(code.Replace("\\n", "\n"));
            Array.Resize(ref elements,matches.Count);
            foreach ( Match match in matches)
            {
                string who = match.Groups["who"].Value;
             
                string text = match.Groups["text"].Value;
                if (who[0] == '_')
                {
                    who = who.Substring(1, who.Length-1);
                    this.elements[id].Type = CutsceneElementType.Special;
                    var special = this.elements[id].GetSpecial();
                    special.Time = float.Parse(who);
                    special.ShowText = text;
                }
                else
                {
                    this.elements[id].Type = CutsceneElementType.Plain;
                    var plain = this.elements[id].GetPlain();
                    plain.Who = who;
                    plain.Text = text;
                }

                id++;
            }

         
        }

    }
#endif
}