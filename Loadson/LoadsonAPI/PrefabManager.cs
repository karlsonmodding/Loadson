using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace LoadsonAPI
{
    public class PrefabManager
    {
        /* Weapons */

        private static GameObject pistol;
        public static GameObject NewPistol()
        {
            GameObject _pistol = UnityEngine.Object.Instantiate(pistol);
            _pistol.name = "Pistol #" + UnityEngine.Random.Range(0, 32767);
            _pistol.SetActive(true);
            return _pistol;
        }

        private static GameObject ak47;
        public static GameObject NewAk47()
        {
            GameObject _ak47 = UnityEngine.Object.Instantiate(ak47);
            _ak47.name = "Ak47 #" + UnityEngine.Random.Range(0, 32767);
            _ak47.SetActive(true);
            return _ak47;
        }

        private static GameObject shotgun;
        public static GameObject NewShotgun()
        {
            GameObject _shotgun = UnityEngine.Object.Instantiate(shotgun);
            _shotgun.name = "Shotgun #" + UnityEngine.Random.Range(0, 32767);
            _shotgun.SetActive(true);
            return _shotgun;
        }

        private static GameObject boomer;
        public static GameObject NewBoomer()
        {
            GameObject _boomer = UnityEngine.Object.Instantiate(boomer);
            _boomer.name = "Boomer #" + UnityEngine.Random.Range(0, 32767);
            _boomer.SetActive(true);
            return _boomer;
        }

        private static GameObject grappler;
        private static GameObject grappler_aim;
        public static GameObject NewGrappler()
        {
            GameObject _grappler = UnityEngine.Object.Instantiate(grappler);
            _grappler.name = "Grappler #" + UnityEngine.Random.Range(0, 32767);
            _grappler.SetActive(true);
            _grappler.GetComponent<Grappler>().aim = UnityEngine.Object.Instantiate(grappler_aim);
            return _grappler;
        }
        public static GameObject NewDummyGrappler()
        {
            GameObject _grappler = UnityEngine.Object.Instantiate(grappler);
            _grappler.name = "DummyGrappler #" + UnityEngine.Random.Range(0, 32767);
            _grappler.SetActive(true);
            return _grappler;
        }


        /* Entities */

        private static GameObject table;
        public static GameObject NewTable()
        {
            GameObject _table = UnityEngine.Object.Instantiate(table);
            _table.name = "Table #" + UnityEngine.Random.Range(0, 32767);
            _table.SetActive(true);
            return _table;
        }

        private static GameObject barrel;
        public static GameObject NewBarrel()
        {
            GameObject _barrel = UnityEngine.Object.Instantiate(barrel);
            _barrel.name = "Barrel #" + UnityEngine.Random.Range(0, 32767);
            _barrel.SetActive(true);
            return _barrel;
        }

        private static GameObject locker;
        public static GameObject NewLocker()
        {
            GameObject _locker = UnityEngine.Object.Instantiate(locker);
            _locker.name = "Locker #" + UnityEngine.Random.Range(0, 32767);
            _locker.SetActive(true);
            return _locker;
        }

        private static GameObject screen;
        public static GameObject NewScreen()
        {
            GameObject _screen = UnityEngine.Object.Instantiate(screen);
            _screen.name = "Screen #" + UnityEngine.Random.Range(0, 32767);
            _screen.SetActive(true);
            return _screen;
        }


        /* Misc */

        private static GameObject enemy;
        public static GameObject NewEnemy()
        {
            GameObject _enemy = UnityEngine.Object.Instantiate(enemy);
            _enemy.name = "Enemy #" + UnityEngine.Random.Range(0, 32767);
            _enemy.SetActive(true);
            _enemy.GetComponent<NavMeshAgent>().enabled = true;
            /*Enemy e = _enemy.GetComponent<Enemy>();
            e.startGun = NewPistol();
            typeof(Enemy).GetMethod("GiveGun", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(e, Array.Empty<object>());*/
            return _enemy;
        }

        private static GameObject milk;
        public static GameObject NewMilk()
        {
            GameObject _milk = UnityEngine.Object.Instantiate(milk);
            _milk.name = "Milk #" + UnityEngine.Random.Range(0, 32767);
            _milk.SetActive(true);
            return _milk;
        }

        private static bool _init = false;
        public static void Init()
        {
            if (_init) return; _init = true;
            if (SceneManager.GetActiveScene().name != "0Tutorial") return;
            foreach (var o in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                switch (o.name)
                {
                    case "Enemy":
                    {
                        Animator animator = o.GetComponentInChildren<Animator>();
                        animator.SetBool("Running", false);
                        animator.SetBool("Aiming", false);
                        o.GetComponent<NavMeshAgent>().enabled = false;
                        GameObject _enemy = UnityEngine.Object.Instantiate(o);
                        _enemy.name = "Loadson-Instance Enemy";
                        UnityEngine.Object.DontDestroyOnLoad(_enemy);
                        _enemy.SetActive(false);
                        _enemy.transform.position = new Vector3(-10000f, -10000f, -10000f);
                        enemy = _enemy;
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Enemy");
                        break;
                    }
                    case "Pistol":
                    {
                        pistol = UnityEngine.Object.Instantiate(o);
                        pistol.name = "Loadson-Instance Pistol";
                        UnityEngine.Object.DontDestroyOnLoad(pistol);
                        pistol.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Pistol");
                        break;
                    }
                    case "Ak47":
                    {
                        ak47 = UnityEngine.Object.Instantiate(o);
                        ak47.name = "Loadson-Instance Ak47";
                        UnityEngine.Object.DontDestroyOnLoad(ak47);
                        ak47.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Ak47");
                        break;
                    }
                    case "Shotgun":
                    {
                        shotgun = UnityEngine.Object.Instantiate(o);
                        shotgun.name = "Loadson-Instance Shotgun";
                        UnityEngine.Object.DontDestroyOnLoad(shotgun);
                        shotgun.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Shotgun");
                        break;
                    }
                    case "Boomer":
                    {
                        boomer = UnityEngine.Object.Instantiate(o);
                        boomer.name = "Loadson-Instance Boomer";
                        UnityEngine.Object.DontDestroyOnLoad(boomer);
                        boomer.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Boomer");
                        break;
                    }
                    case "Grappler":
                    {
                        grappler = UnityEngine.Object.Instantiate(o);
                        grappler.name = "Loadson-Instance Grappler";
                        UnityEngine.Object.DontDestroyOnLoad(grappler);
                        grappler.SetActive(false);
                        grappler_aim = UnityEngine.Object.Instantiate(o.GetComponent<Grappler>().aim);
                        UnityEngine.Object.DontDestroyOnLoad(grappler_aim);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Grappler");
                        break;
                    }
                    case "Table":
                    {
                        table = UnityEngine.Object.Instantiate(o);
                        table.name = "Loadson-Instance Table";
                        UnityEngine.Object.DontDestroyOnLoad(table);
                        table.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Table");
                        break;
                    }
                    case "Barrel":
                    {
                        barrel = UnityEngine.Object.Instantiate(o);
                        barrel.name = "Loadson-Instance Barrel";
                        UnityEngine.Object.DontDestroyOnLoad(barrel);
                        barrel.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Barrel");
                        break;
                    }
                    case "Locker":
                    {
                        locker = UnityEngine.Object.Instantiate(o);
                        locker.name = "Loadson-Instance Locker";
                        UnityEngine.Object.DontDestroyOnLoad(locker);
                        locker.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Locker");
                        break;
                    }
                    case "Screen":
                    {
                        screen = UnityEngine.Object.Instantiate(o);
                        screen.name = "Loadson-Instance Screen";
                        UnityEngine.Object.DontDestroyOnLoad(screen);
                        screen.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Screen");
                        break;
                    }
                    case "Milk":
                    {
                        milk = UnityEngine.Object.Instantiate(o);
                        milk.name = "Loadson-Instance Milk";
                        UnityEngine.Object.DontDestroyOnLoad(milk);
                        milk.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Milk");
                        break;
                    }
                }
            }
        }
    }
}
