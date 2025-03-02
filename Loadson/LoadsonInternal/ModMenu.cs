#if !LoadsonAPI
using LoadsonAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
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

            GameObject GO_InstallButton = UnityEngine.Object.Instantiate(GO_ModsUI.transform.Find("Back").gameObject);
            GO_InstallButton.transform.parent = GO_ModsUI.transform;
            GO_InstallButton.transform.SetSiblingIndex(1);
            GO_InstallButton.transform.position = new Vector3(-7.9997f, 20.3098f, 190.6432f);
            GO_InstallButton.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            GO_InstallButton.transform.localScale = new Vector3(0.7635f, 0.7635f, 0.7635f);
            GO_InstallButton.GetComponentInChildren<TextMeshProUGUI>().text = "<size=30>install</size>";
            _UIHelper.InterceptButton(GO_InstallButton.GetComponent<Button>(), () =>
            {
                GO_ModsUI.SetActive(false);
                FilePicker.PickFile("Choose mod to install (restart after install)", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), new List<(string, string)> { ("KLMI", "*.klmi"), ("KLM", "*.klm") }, (file) =>
                {
                    if(!File.Exists(file)) return;
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if(Path.GetExtension(file) == ".klm")
                    {
                        if (File.Exists(Path.Combine(Loader.LOADSON_ROOT, "Mods", fileName + ".klm")))
                            File.Delete(Path.Combine(Loader.LOADSON_ROOT, "Mods", fileName + ".klm"));
                        if (File.Exists(Path.Combine(Loader.LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm")))
                            File.Delete(Path.Combine(Loader.LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm"));
                        File.Copy(file, Path.Combine(Loader.LOADSON_ROOT, "Mods", fileName + ".klm"));
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(Loader.LOADSON_ROOT, "Mods", fileName + ".klm")))
                            File.Delete(Path.Combine(Loader.LOADSON_ROOT, "Mods", fileName + ".klm"));
                        if (File.Exists(Path.Combine(Loader.LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm")))
                            File.Delete(Path.Combine(Loader.LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm"));
                        using(BinaryReader br = new BinaryReader(File.OpenRead(file)))
                        {
                            int _extDeps = br.ReadInt32();
                            List<(string, byte[])> extDeps = new List<(string, byte[])>();
                            while(_extDeps-- > 0)
                            {
                                var name = br.ReadString();
                                var _len = br.ReadInt32();
                                var data = br.ReadBytes(_len);
                                extDeps.Add((name, data));
                            }
                            int _modSize = br.ReadInt32();
                            byte[] modData = br.ReadBytes(_modSize);
                            foreach (var dep in extDeps)
                                File.WriteAllBytes(Path.Combine(Loader.LOADSON_ROOT, "Internal", "Common deps", dep.Item1), dep.Item2);
                            File.WriteAllBytes(Path.Combine(Loader.LOADSON_ROOT, "Mods", fileName + ".klm"), modData);
                        }
                    }
                    GO_ModsUI.SetActive(true);
                }, () =>
                {
                    GO_ModsUI.SetActive(true);
                });
            });

            for (int i = 2; i < GO_ModsUI.transform.childCount; i++)
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
                GameObject img = new GameObject("Image #" + i);
                img.transform.parent = ScrollContainer.transform;
                img.transform.localPosition = new Vector3(-515, -75 + 85 * ModEntry.List.Count - 170 * i, 0);
                img.transform.rotation = Quaternion.Euler(0, -90, 0);
                img.transform.localScale = new Vector3(1f, 1f, 1f);
                img.AddComponent<Image>().sprite = Sprite.Create(ModEntry.List[i].Icon, new Rect(0, 0, ModEntry.List[0].Icon.width, ModEntry.List[0].Icon.height), new Vector2(0, 0));
                GameObject text = new GameObject("Name #" + i);
                text.transform.parent = ScrollContainer.transform;
                text.transform.localPosition = new Vector3(-400, -75 + 85 * ModEntry.List.Count - 170 * i, 0);
                text.transform.rotation = Quaternion.Euler(0, -90, 0);
                text.transform.localScale = new Vector3(1f, 1f, 1f);
                text.AddComponent<TextMeshProUGUI>().text = ModEntry.List[i].DisplayName;
                GameObject desc = new GameObject("Author #" + i);
                desc.transform.parent = ScrollContainer.transform;
                desc.transform.localPosition = new Vector3(-400, -75 + 85 * ModEntry.List.Count - 170 * i - 50, 0);
                desc.transform.rotation = Quaternion.Euler(0, -90, 0);
                desc.transform.localScale = new Vector3(1f, 1f, 1f);
                desc.AddComponent<TextMeshProUGUI>().text = "by " + ModEntry.List[i].Author;

                GameObject btn1 = UnityEngine.Object.Instantiate(GO_ModsUI.transform.Find("Back").gameObject);
                btn1.transform.parent = ScrollContainer.transform;
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).text = "Details";
                btn1.transform.localPosition = new Vector3(-513, 85 * ModEntry.List.Count - 170 * i - 140, 0);
                btn1.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                // weird double scaling and resizing hitbox fix, idk how i even figured this out
                btn1.transform.localScale = new Vector3(0.1352f, 0.8195f, 1.1707f);
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).rectTransform.localScale = new Vector3(5f, 0.8255f, 0.8255f);
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).rectTransform.sizeDelta = new Vector2(155, 25);
                ((TextMeshProUGUI)btn1.GetComponent<Button>().targetGraphic).enableWordWrapping = false;
                btn1.name = "Details #" + i;

                int remi = i;
                _UIHelper.InterceptButton(btn1.GetComponent<Button>(), () => {
                    viewModIdx = remi;
                    GO_ModsUI.SetActive(false);
                });
            }

            sr.content = ScrollContainer.AddComponent<RectTransform>();
            sr.horizontal = false;
            sr.content.sizeDelta = new Vector2(1000, 170 * ModEntry.List.Count);
            sr.content.localScale = new Vector3(0.85f, 1f, 1f);
            sr.scrollSensitivity = 20f;
            ScrollView.AddComponent<Mask>();

            _UIHelper.InterceptButton(GO_Mods.GetComponent<Button>(), () =>
            {
                MenuCamera cam = UnityEngine.Object.FindObjectOfType<MenuCamera>();
                typeof(MenuCamera).GetField("desiredPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cam, new Vector3(-1f, 15.1f, 184.06f));
                typeof(MenuCamera).GetField("desiredRot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cam, Quaternion.Euler(0f, -90f, 0f));

                GameObject.Find("/UI/Menu").SetActive(false);
                GO_ModsUI.SetActive(true);
            });
        }

        private static int viewModIdx = -1;

        private static Rect window = new Rect((Screen.width - 400) / 2, (Screen.height - 450) / 2, 400, 450);

        public static void _ongui()
        {
            if (viewModIdx == -1) return;
            window = GUI.Window(windowId, window, (_) =>
            {
                GUI.DragWindow(new Rect(0, 0, 350, 20));
                if(GUI.Button(new Rect(350, 0, 50, 20), "Close"))
                {
                    viewModIdx = -1;
                    GameObject.Find("/UI").transform.Find("Mods").gameObject.SetActive(true);
                }
                ModEntry mod = ModEntry.List[viewModIdx];
                GUI.DrawTexture(new Rect(5, 25, 100, 100), mod.Icon);
                GUI.Label(new Rect(110, 25, 280, 100),
                    $"[File] {Path.GetFileName(mod.FilePath)}\n" +
                    $"[GUID] {mod.ModGUID}\n" +
                    $"[Name] {mod.DisplayName}\n" +
                    $"[Author] {mod.Author}\n" +
                    $"[Deps] ({mod.DepsRef.Length}) {string.Join(", ", mod.DepsRef)}\n" +
                    (mod.AssetBundle != null ? "This mod has an AssetBundle" : "This mod does not have an AssetBundle"));
                GUI.Box(new Rect(5, 130, 390, 315), "");
                GUI.Label(new Rect(10, 135, 380, 305), mod.Description);
            }, "Mod Details");
        }

        private static int windowId = -1;
        public static void _init()
        {
            windowId = ImGUI_WID.GetWindowId();
        }
    }

}
#endif