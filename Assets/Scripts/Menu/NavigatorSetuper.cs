using UnityEngine;
using UnityEngine.UI;
namespace Menu
{
    public class NavigatorSetuper : MonoBehaviour
    {
        [SerializeField] private Selectable[] selectable= new Selectable[0];
        [NaughtyAttributes.Button()]
        public void Set()
        {
            for (int i = 0; i < selectable.Length; i++)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;
                if (i > 0)
                    navigation.selectOnLeft = selectable[i - 1];
                if (i < selectable.Length - 1)
                    navigation.selectOnRight = selectable[i + 1];
                selectable[i].navigation = navigation;

            }
        }
    }
}