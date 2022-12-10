using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

namespace LoadsonInternal
{
    public static class _UIHelper
    {
        public static void InterceptButton(Button b, UnityAction onClick)
        {
            for (int i = 0; i < b.onClick.GetPersistentEventCount(); i++)
                b.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            b.onClick.AddListener(onClick);
        }

        public static void SetCustomOption(GameObject[] list, string name, bool value, Action on, Action off, float offY = 0f, string _on = "on", string _off = "off")
        {
            list[0].GetComponent<TextMeshProUGUI>().text = name;
            list[0].GetComponent<TextMeshProUGUI>().enableWordWrapping = false;
            ((TextMeshProUGUI)list[1].GetComponent<Button>().targetGraphic).text = _on;
            ((TextMeshProUGUI)list[2].GetComponent<Button>().targetGraphic).text = _off;
            list[0].transform.localPosition += new Vector3(0, offY, 0);
            list[1].transform.localPosition += new Vector3(0, offY, 0);
            list[2].transform.localPosition += new Vector3(0, offY, 0);

            SetButtonsColors(list, value);

            InterceptButton(list[1].GetComponent<Button>(), () =>
            {
                on();
                SetButtonsColors(list, true);
            });
            InterceptButton(list[2].GetComponent<Button>(), () =>
            {
                off();
                SetButtonsColors(list, false);
            });
        }

        private static void SetButtonsColors(GameObject[] list, bool value)
        {
            if (value)
            {
                ((TextMeshProUGUI)list[1].GetComponent<Button>().targetGraphic).color = Color.white;
                ((TextMeshProUGUI)list[2].GetComponent<Button>().targetGraphic).color = (Color.clear + Color.white) / 2f;
            }
            else
            {
                ((TextMeshProUGUI)list[2].GetComponent<Button>().targetGraphic).color = Color.white;
                ((TextMeshProUGUI)list[1].GetComponent<Button>().targetGraphic).color = (Color.clear + Color.white) / 2f;
            }
        }

        public static GameObject InstantiateExact(GameObject obj)
        {
            GameObject ret = UnityEngine.Object.Instantiate(obj);
            ret.transform.parent = obj.transform.parent;
            ret.transform.position = obj.transform.position;
            ret.transform.rotation = obj.transform.rotation;
            ret.transform.localScale = obj.transform.localScale;
            return ret;
        }
    }
}
