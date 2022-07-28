#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cyberultimate;
using Cyberultimate.Unity;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


#pragma warning disable IDE0067
namespace Cyberultimate.Unity
{
    public sealed class MonoCyberConsole : MonoSingleton<MonoCyberConsole>
    {
        
        public static event EventHandler OnOpen = delegate { };
        public static event EventHandler OnClose = delegate { };
        [SerializeField] private Text outputEntity = null!;
        [SerializeField] private ProtipObject protipObjectPrefab = null!;
        [SerializeField] private Transform protipParent = null!;
        [FormerlySerializedAs("input")] [SerializeField]
        private InputField inputField = null!;
        public InputField InputField => inputField;
        
        private Cint? select = null;
        private readonly List<ProtipObject> tips = new List<ProtipObject>();

        private void OnEnable()
        {
            InputFieldNullCheck();
            
            inputField.Select();
            inputField.text = string.Empty;
            OnOpen(this, EventArgs.Empty);
        }
        private void InputFieldNullCheck()
        {

            if (inputField == null)
            {
               throw new InvalidOperationException( "Input field was null, aborted");
            }
        }

        private void OnDisable()
        {
            OnClose(this, EventArgs.Empty);
        }

        private void Start()
        {
            InputFieldNullCheck();
            inputField.onValueChanged.AddListener(OnChange);
            inputField.onEndEdit.AddListener(OnEndEdit);
        }

        private void Update()
        {
            InputFieldNullCheck();
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (select == null)
                {
                    outputEntity.text+= Unity.CyberCommandSystem.CallCommand(inputField!.text);
                    inputField.text = String.Empty;
                    inputField.Select();
                    inputField.ActivateInputField();
                }


            }

            if (select != null)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (select > 0)
                        select--;
                    else
                        select = null;
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    tips[(int) (uint) (Cint) select].Put();

                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                select = null;


            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (select != null)
                {
                    select++;
                }
                else
                    select = 0;
            }

            if (tips.Count != 0 && select >= tips.Count)
                select = (uint) tips.Count - 1;
            for (int i = 0; i < tips.Count; i++)
            {
                ProtipObject obj = (ProtipObject) tips[i];
                obj.TextEntity.color = i == select ? Color.yellow : Color.green;
            }

        }

#pragma warning disable IDE0051
        private void RefuseOrClose()
#pragma warning restore IDE0051
        {
            outputEntity.text = string.Empty;
            outputEntity.text = "Starting a developer console ... \n";

            this.gameObject.SetActive(!this.gameObject.activeSelf);
            inputField!.Select();
            inputField.ActivateInputField();
        }

        private void OnChange(string val)
        {
            //TODO: should be merge with CyberConsole.GetTips()
            select = null;
            protipParent.KillAllChildren();
            tips.Clear();
            string original = val;
            val = val.ToUpper() ?? string.Empty;
            foreach (KeyValuePair<string, CyberCommand> equalElement in
                from element in CyberCommandSystem.Commands
                where element.Key != original && element.Key.ToUpper().Contains(val)
                select element)
            {
                var obj = Instantiate(protipObjectPrefab, protipParent);
                obj.Init($"{equalElement.Key}" +
                         $"{equalElement.Value.MetaData.ArgumentDescription.Aggregate(new StringBuilder(), (b, e) => b.Append($" [{e}] ")).ToString()} ",
                    equalElement.Value);

                tips.Add(obj);
            }
        }

        public void WriteLine(string value)
        {

            this.outputEntity.text += $">>{value}\n";
        }

        public void Put(string text)
        {
            this.inputField.text = text;
            this.inputField.caretPosition = inputField.text.Length;
        }
        
        private void OnEndEdit(string val)
        {
            if (protipParent.GetComponentsInChildren<ProtipObject>()
                .All(item => item.IsMouseColliding == false))
                protipParent.KillAllChildren();

        }

      
    }


}