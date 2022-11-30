using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace LoadsonInternal
{
    public static class MenuCustom
    {
        public static void _scene()
        {
            GameObject ScrollView = UnityEngine.Object.Instantiate(GameObject.Find("/UI").transform.Find("About").Find("SteamBtn").gameObject);
            UnityEngine.Object.Destroy(ScrollView.GetComponent<Button>());
            ScrollView.transform.parent = GameObject.Find("/UI/Menu").transform;
            ScrollView.transform.localPosition = new Vector3(200, -10, -41);
            ScrollView.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            ScrollView.transform.localScale = new Vector3(0.2069f, 0.4553f, 0.5f);
            ScrollView.name = "ScrollView";
            ScrollView.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
            ScrollRect sr = ScrollView.AddComponent<ScrollRect>();

            GameObject ScrollContainer = new GameObject("ScrollContainer");
            ScrollContainer.transform.parent = ScrollView.transform;
            ScrollContainer.transform.localPosition = new Vector3(0, 0, 0);
            ScrollContainer.transform.rotation = Quaternion.Euler(0, 0, 0);
            ScrollContainer.transform.localScale = new Vector3(1f, 1f, 1f);

            sr.content = ScrollContainer.AddComponent<RectTransform>();
            sr.horizontal = false;
            sr.content.localScale = new Vector3(0.85f, 1f, 1f);
            sr.scrollSensitivity = 20f;
            ScrollView.AddComponent<Mask>();
            selected = -1;
            RenderButtons();
        }

        private static List<(string, List<(string, Action)>)> mainmenu = new List<(string, List<(string, Action)>)>();
        private static int selected = -1;

        public static void AddCategory(string category, List<(string, Action)> children)
        {
            children.Insert(0, ("back", () =>
            {
                selected = -1;
                RenderButtons();
            }));
            mainmenu.Add((category, children));
        }
        public static void RemoveCategory(string category)
        {
            mainmenu.Remove((from x in mainmenu where x.Item1 == category select x).FirstOrDefault());
        }
        public static void UpdateCategory(string category, List<(string, Action)> newChildren)
        {
            mainmenu.Remove((from x in mainmenu where x.Item1 == category select x).FirstOrDefault());
            newChildren.Insert(0, ("back", () =>
            {
                selected = -1;
                RenderButtons();
            }
            ));
            mainmenu.Add((category, newChildren));
        }

        private static void RenderButtons()
        {
            for (int i = 0; i < GameObject.Find("/UI/Menu/ScrollView/ScrollContainer").transform.childCount; i++)
                UnityEngine.Object.Destroy(GameObject.Find("/UI/Menu/ScrollView/ScrollContainer").transform.GetChild(i).gameObject);
            if (selected == -1)
            {
                for (int i = 0; i < mainmenu.Count; i++)
                {
                    GameObject btn = UnityEngine.Object.Instantiate(GameObject.Find("/UI/Menu/Options"));
                    btn.transform.parent = GameObject.Find("/UI/Menu/ScrollView/ScrollContainer").transform;
                    btn.transform.localPosition = new Vector3(-170, 50f * mainmenu.Count - 100f * i, 0f);
                    btn.transform.rotation = Quaternion.Euler(0, 0, 0);
                    btn.transform.localScale = new Vector3(5.6f, 2.1f, 2.1f);
                    ((TextMeshProUGUI)btn.GetComponent<Button>().targetGraphic).enableWordWrapping = false;
                    ((TextMeshProUGUI)btn.GetComponent<Button>().targetGraphic).text = mainmenu[i].Item1;

                    var remi = i;
                    InterceptButton(btn.GetComponent<Button>(), () => { selected = remi; RenderButtons(); });
                }
                GameObject.Find("/UI/Menu/ScrollView").GetComponent<ScrollRect>().content.sizeDelta = new Vector2(1000, 100 + 100 * mainmenu.Count);
                return;
            }
            for (int i = 0; i < mainmenu[selected].Item2.Count; i++)
            {
                GameObject btn = UnityEngine.Object.Instantiate(GameObject.Find("/UI/Menu/Options"));
                btn.transform.parent = GameObject.Find("/UI/Menu/ScrollView/ScrollContainer").transform;
                btn.transform.localPosition = new Vector3(-170, 50f * mainmenu[selected].Item2.Count - 100f * i, 0f);
                btn.transform.rotation = Quaternion.Euler(0, 0, 0);
                btn.transform.localScale = new Vector3(5.6f, 2.1f, 2.1f);
                ((TextMeshProUGUI)btn.GetComponent<Button>().targetGraphic).enableWordWrapping = false;
                ((TextMeshProUGUI)btn.GetComponent<Button>().targetGraphic).text = mainmenu[selected].Item2[i].Item1;

                var remi = i;
                InterceptButton(btn.GetComponent<Button>(), () => { mainmenu[selected].Item2[remi].Item2(); RenderButtons(); });
            }
            GameObject.Find("/UI/Menu/ScrollView").GetComponent<ScrollRect>().content.sizeDelta = new Vector2(1000, 100 + 100 * mainmenu[selected].Item2.Count);
        }

        private static void InterceptButton(Button b, UnityAction onClick)
        {
            for (int i = 0; i < b.onClick.GetPersistentEventCount(); i++)
                b.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            b.onClick.AddListener(onClick);
        }
    }
}
