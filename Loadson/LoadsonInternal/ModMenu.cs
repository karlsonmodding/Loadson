﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LoadsonInternal
{
    public static class ModMenu
    {
        public static void _scene()
        {
            UnityEngine.Object.Destroy(GameObject.Find("/UI/Menu/Map"));
            GameObject GO_Mods = UnityEngine.Object.Instantiate(GameObject.Find("/UI/Menu/About"));
            GO_Mods.transform.parent = GameObject.Find("/UI/Menu").transform;
            GO_Mods.transform.position = new Vector3(-6.80f, 9.5527f, 185.61f);
            GO_Mods.transform.localScale = new Vector3(1.1707f, 1.1707f, 1.1707f);
            GO_Mods.name = "Mods";
            ((TMP_Text)GO_Mods.GetComponent<Button>().targetGraphic).text = "Mods";

            GameObject GO_ModsUI = UnityEngine.Object.Instantiate(GameObject.Find("/UI").transform.Find("Play").gameObject);
            GO_ModsUI.transform.parent = GameObject.Find("/UI").transform;
            GO_ModsUI.transform.position = new Vector3(-7.9997f, 14.4872f, 188.3855f);
            GO_ModsUI.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            GO_ModsUI.transform.localScale = new Vector3(0.7635f, 0.7635f, 0.7635f);
            GO_ModsUI.name = "Mods";
            GO_ModsUI.SetActive(false);

            for (int i = 1; i < GO_ModsUI.transform.childCount; i++)
                UnityEngine.Object.Destroy(GO_ModsUI.transform.GetChild(i).gameObject);

            GameObject ScrollView = UnityEngine.Object.Instantiate(GameObject.Find("/UI").transform.Find("About").Find("SteamBtn").gameObject);
            UnityEngine.Object.Destroy(ScrollView.GetComponent<Button>());
            ScrollView.transform.parent = GO_ModsUI.transform;
            ScrollView.transform.localPosition = new Vector3(-150, -30, 0);
            ScrollView.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            ScrollView.transform.localScale = new Vector3(0.65f, 0.55f, 0.5f);
            ScrollView.name = "ScrollView";
            ScrollView.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
            ScrollRect sr = ScrollView.AddComponent<ScrollRect>();

            GameObject ScrollContainer = new GameObject("ScrollContainer");
            ScrollContainer.transform.parent = ScrollView.transform;
            ScrollContainer.transform.localPosition = new Vector3(0, 0, 0);
            ScrollContainer.transform.rotation = Quaternion.Euler(0, -90, 0);
            ScrollContainer.transform.localScale = new Vector3(1f, 1f, 1f);

            for(int i = 0; i < ModEntry.List.Count; i++)
            {
                GameObject img = new GameObject("Image");
                img.transform.parent = ScrollContainer.transform;
                img.transform.localPosition = new Vector3(-515, -75 + 85 * ModEntry.List.Count - 170 * i, 0);
                img.transform.rotation = Quaternion.Euler(0, -90, 0);
                img.transform.localScale = new Vector3(1f, 1f, 1f);
                img.AddComponent<Image>().sprite = Sprite.Create(ModEntry.List[i].Icon, new Rect(0, 0, ModEntry.List[0].Icon.width, ModEntry.List[0].Icon.height), new Vector2(0, 0));
                GameObject text = new GameObject("Name");
                text.transform.parent = ScrollContainer.transform;
                text.transform.localPosition = new Vector3(-230, -75 + 85 * ModEntry.List.Count - 170 * i, 0);
                text.transform.rotation = Quaternion.Euler(0, -90, 0);
                text.transform.localScale = new Vector3(1f, 1f, 1f);
                text.AddComponent<TextMeshProUGUI>().text = ModEntry.List[i].DisplayName;
                GameObject desc = new GameObject("Author");
                desc.transform.parent = ScrollContainer.transform;
                desc.transform.localPosition = new Vector3(-230, -75 + 85 * ModEntry.List.Count - 170 * i - 50, 0);
                desc.transform.rotation = Quaternion.Euler(0, -90, 0);
                desc.transform.localScale = new Vector3(1f, 1f, 1f);
                desc.AddComponent<TextMeshProUGUI>().text = "by " + ModEntry.List[i].Author;

                GameObject btn1 = UnityEngine.Object.Instantiate(GO_ModsUI.transform.Find("Back").gameObject);
                btn1.transform.parent = ScrollContainer.transform;
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).text = "Details";
                btn1.transform.localPosition = new Vector3(-377, 85 * ModEntry.List.Count - 170 * i - 50, 0);
                btn1.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                // weird double scaling and resizing hitbox fix, idk how i even figured this out
                btn1.transform.localScale = new Vector3(0.1932f, 1.1707f, 1.1707f);
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).rectTransform.localScale = new Vector3(5f, 0.8255f, 0.8255f);
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).rectTransform.sizeDelta = new Vector2(155, 25);
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).enableWordWrapping = false;
                btn1.name = "Details";

                GameObject btn2 = UnityEngine.Object.Instantiate(GO_ModsUI.transform.Find("Back").gameObject);
                btn2.transform.parent = ScrollContainer.transform;
                ((TextMeshProUGUI)btn2.GetComponent<Button>().targetGraphic).text = "Disable";
                btn2.transform.localPosition = new Vector3(-377, -50 + 85 * ModEntry.List.Count - 170 * i - 50, 0);
                btn2.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                // weird double scaling and resizing hitbox fix, idk how i even figured this out
                btn2.transform.localScale = new Vector3(0.1932f, 1.1707f, 1.1707f);
                ((TextMeshProUGUI)btn2.GetComponent<Button>().targetGraphic).rectTransform.localScale = new Vector3(5f, 0.8255f, 0.8255f);
                ((TextMeshProUGUI)btn2.GetComponent<Button>().targetGraphic).rectTransform.sizeDelta = new Vector2(155, 25);
                ((TextMeshProUGUI)btn2.GetComponent<Button>().targetGraphic).enableWordWrapping = false;
                btn2.name = "Disable";

                int remi = i;
                InterceptButton(btn1.GetComponent<Button>(), () => { });
                InterceptButton(btn2.GetComponent<Button>(), () => {
                    Process.Start(Path.Combine(Loader.LOADSON_ROOT, "Launcher", "Launcher.exe"), "-disable " + ModEntry.List[remi].ModGUID + ".klm");
                    Application.Quit(); // handle quitting properly
                });
            }

            sr.content = ScrollContainer.AddComponent<RectTransform>();
            sr.horizontal = false;
            sr.content.sizeDelta = new Vector2(1000, 170 * ModEntry.List.Count);
            sr.content.localScale = new Vector3(0.85f, 1f, 1f);
            sr.scrollSensitivity = 20f;
            ScrollView.AddComponent<Mask>();

            InterceptButton(GO_Mods.GetComponent<Button>(), () =>
            {
                MenuCamera cam = UnityEngine.Object.FindObjectOfType<MenuCamera>();
                typeof(MenuCamera).GetField("desiredPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cam, new Vector3(-1f, 15.1f, 184.06f));
                typeof(MenuCamera).GetField("desiredRot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cam, Quaternion.Euler(0f, -90f, 0f));

                GameObject.Find("/UI/Menu").SetActive(false);
                GO_ModsUI.SetActive(true);
            });
        }

        private static void InterceptButton(Button b, UnityAction onClick)
        {
            for (int i = 0; i < b.onClick.GetPersistentEventCount(); i++)
                b.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            b.onClick.AddListener(onClick);
        }
    }
}
