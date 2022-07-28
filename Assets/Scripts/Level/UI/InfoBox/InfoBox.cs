using Cyberultimate.Unity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace LetterBattle
{
    public class InfoBox : MonoBehaviour
    {
        [SerializeField][SceneObjectOnly] private TextMeshProUGUI textEntity;

       
        public void Init(string text, Color color, float timeToDestroy)
        {
            this.textEntity.text = text;
            this.textEntity.color = color;
            this.transform.localScale = Vector2.zero;
            this.gameObject.transform.DOScale(Vector3.one, timeToDestroy)
                .SetLink(this.gameObject);
               //  .OnComplete(() => Destroy(this.gameObject));

            DOVirtual.DelayedCall(timeToDestroy / 2, () =>
            {
                Image[] images = this.gameObject.GetComponentsInChildren<Image>();
                TMP_Text[] meshes = this.gameObject.GetComponentsInChildren<TMP_Text>();
                DOVirtual.Float(1, 0, timeToDestroy / 2, (value) =>
                {
                    foreach (var image in images)
                    {
                        image.color = new Clr(image.color, value);
                    }
                    foreach (var item in meshes)
                    {
                        item.color = new Clr(item.color, value);
                    }
                }).SetLink(this.gameObject);
            }, false).SetLink(this.gameObject);        
        }
        public void Kill()
        {
            Destroy(this.gameObject);
        }
    }
}