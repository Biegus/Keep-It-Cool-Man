#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyberultimate.Unity;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Cyberultimate.Unity
{
    [RequireComponent(typeof(EventTrigger))]
    public class ProtipObject:MonoBehaviour
    {
        [SerializeField] private Text textEntity = null;
        public bool IsMouseColliding { get; private set; }
        public EventTrigger EventTrigger { get; private set; }
        public CyberCommand Command { get; private set; }
        public Text TextEntity => textEntity;
        public void Init(string text,CyberCommand command)
        {
            textEntity.text = text;
            Command = command;
        }

        private void Awake()
        {
            EventTrigger = this.GetComponent<EventTrigger>();
            EventTrigger.Add(EventTriggerType.PointerEnter,PointerGuiAreaEnter);
            EventTrigger.Add(EventTriggerType.PointerExit,PointerGuiAreaEnter);
        }
        public void Put()
        {
            MonoCyberConsole.Current.Put($"{Command.MetaData.Name} ");
      
        }
        protected  void PointerGuiAreaEnter(BaseEventData data)
        {
       
            IsMouseColliding = true;
        }
        protected  void PointerGuiAreaExit(BaseEventData data)
        {
       
            IsMouseColliding = false;
        }
    }


}

