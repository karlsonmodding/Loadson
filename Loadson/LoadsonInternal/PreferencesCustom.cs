#if !LoadsonAPI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoadsonInternal
{
    public class PreferencesCustom
    {
        public static void _scene()
        {
            CustomOptions.Clear();
            GameObject loadsonSettings = UnityEngine.Object.Instantiate(GameObject.Find("/UI").transform.Find("Settings").gameObject);
            loadsonSettings.transform.parent = GameObject.Find("/UI").transform;
            loadsonSettings.transform.position = GameObject.Find("/UI").transform.Find("Settings").position;
            loadsonSettings.transform.rotation = Quaternion.Euler(0, 0, 0);
            loadsonSettings.transform.localScale = new Vector3(0.7635f, 0.7635f, 0.7635f);
            loadsonSettings.SetActive(false);

            for (int i = 4; i <= 12; i++)
                UnityEngine.Object.Destroy(loadsonSettings.transform.GetChild(i).gameObject);
            for (int i = 15; i <= 20; i++)
                UnityEngine.Object.Destroy(loadsonSettings.transform.GetChild(i).gameObject);
            CustomOptions.Add(new GameObject[]
            {
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(1).gameObject),
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(2).gameObject),
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(3).gameObject)
            });
            CustomOptions.Add(new GameObject[]
            {
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(1).gameObject),
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(2).gameObject),
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(3).gameObject)
            });
            CustomOptions.Add(new GameObject[]
            {
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(1).gameObject),
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(2).gameObject),
                _UIHelper.InstantiateExact(loadsonSettings.transform.GetChild(3).gameObject)
            });
            loadsonSettings.transform.GetChild(1).gameObject.SetActive(false);
            loadsonSettings.transform.GetChild(2).gameObject.SetActive(false);
            loadsonSettings.transform.GetChild(3).gameObject.SetActive(false);
            loadsonSettings.transform.GetChild(13).gameObject.SetActive(false);
            loadsonSettings.transform.GetChild(14).gameObject.SetActive(false);
            _UIHelper.SetCustomOption(CustomOptions[0], "Unity log", Preferences.instance.unityLog, () => Preferences.instance.unityLog = true, () => Preferences.instance.unityLog = false);
            _UIHelper.SetCustomOption(CustomOptions[1], "File log", Preferences.instance.fileLog, () => Preferences.instance.fileLog = true, () => Preferences.instance.fileLog = false, -70f);
            _UIHelper.SetCustomOption(CustomOptions[2], "Enable FPS & Speed", Preferences.instance.forceFpsAndSpeed, () => Preferences.instance.forceFpsAndSpeed = true, () => Preferences.instance.forceFpsAndSpeed = false, -140f, "Yes", "No");


            GameObject loadsonButton = UnityEngine.Object.Instantiate(GameObject.Find("/UI").transform.Find("Settings").Find("Back").gameObject);
            loadsonButton.transform.parent = GameObject.Find("/UI").transform.Find("Settings");
            loadsonButton.transform.localPosition = new Vector3(-5.9769f, 135.1f, -0.0002f);
            loadsonButton.transform.rotation = Quaternion.Euler(0, 0, 0);
            loadsonButton.transform.localScale = new Vector3(1.1707f, 1.1707f, 1.1707f);
            ((TextMeshProUGUI)loadsonButton.GetComponent<Button>().targetGraphic).enableWordWrapping = false;
            ((TextMeshProUGUI)loadsonButton.GetComponent<Button>().targetGraphic).text = "Loadson";

            _UIHelper.InterceptButton(loadsonButton.GetComponent<Button>(), ()=>
            {
                GameObject.Find("/UI").transform.Find("Settings").gameObject.SetActive(false);
                loadsonSettings.SetActive(true);
            });
        }

        private static List<GameObject[]> CustomOptions = new List<GameObject[]>();
    }
}
#endif