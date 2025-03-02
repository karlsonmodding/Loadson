using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace LoadsonAPI
{
    public class PrefabManager
    {
        /* Weapons */

        public static GameObject NewPistol()
        {
#if !LoadsonAPI
            GameObject _pistol = UnityEngine.Object.Instantiate(pistol);
            _pistol.name = "Pistol #" + UnityEngine.Random.Range(0, 32767);
            _pistol.SetActive(true);
            return _pistol;
        }
        private static GameObject pistol;
#else
            return null;
        }
#endif

        public static GameObject NewAk47()
        {
#if !LoadsonAPI
            GameObject _ak47 = UnityEngine.Object.Instantiate(ak47);
            _ak47.name = "Ak47 #" + UnityEngine.Random.Range(0, 32767);
            _ak47.SetActive(true);
            return _ak47;
        }
        private static GameObject ak47;
#else
            return null;
        }
#endif

        public static GameObject NewShotgun()
        {
#if !LoadsonAPI
            GameObject _shotgun = UnityEngine.Object.Instantiate(shotgun);
            _shotgun.name = "Shotgun #" + UnityEngine.Random.Range(0, 32767);
            _shotgun.SetActive(true);
            return _shotgun;
        }
        private static GameObject shotgun;
#else
            return null;
        }
#endif

        public static GameObject NewBoomer()
        {
#if !LoadsonAPI
            GameObject _boomer = UnityEngine.Object.Instantiate(boomer);
            _boomer.name = "Boomer #" + UnityEngine.Random.Range(0, 32767);
            _boomer.SetActive(true);
            return _boomer;
        }
        private static GameObject boomer;
#else
            return null;
        }
#endif

        public static GameObject NewGrappler()
        {
#if !LoadsonAPI
            GameObject _grappler = UnityEngine.Object.Instantiate(grappler);
            _grappler.name = "Grappler #" + UnityEngine.Random.Range(0, 32767);
            _grappler.SetActive(true);
            _grappler.GetComponent<Grappler>().aim = UnityEngine.Object.Instantiate(grappler_aim);
            return _grappler;
#else
            return null;
#endif
        }
        public static GameObject NewDummyGrappler()
        {
#if !LoadsonAPI
            GameObject _grappler = UnityEngine.Object.Instantiate(grappler);
            _grappler.name = "DummyGrappler #" + UnityEngine.Random.Range(0, 32767);
            _grappler.SetActive(true);
            return _grappler;
        }
        private static GameObject grappler;
        private static GameObject grappler_aim;
#else
            return null;
        }
#endif


        /* Entities */

        public static GameObject NewTable()
        {
#if !LoadsonAPI
            GameObject _table = UnityEngine.Object.Instantiate(table);
            _table.name = "Table #" + UnityEngine.Random.Range(0, 32767);
            _table.SetActive(true);
            return _table;
        }
        private static GameObject table;
#else
            return null;
        }
#endif

        public static GameObject NewBarrel()
        {
#if !LoadsonAPI
            GameObject _barrel = UnityEngine.Object.Instantiate(barrel);
            _barrel.name = "Barrel #" + UnityEngine.Random.Range(0, 32767);
            _barrel.SetActive(true);
            return _barrel;
        }
        private static GameObject barrel;
#else
            return null;
        }
#endif

        public static GameObject NewLocker()
        {
#if !LoadsonAPI
            GameObject _locker = UnityEngine.Object.Instantiate(locker);
            _locker.name = "Locker #" + UnityEngine.Random.Range(0, 32767);
            _locker.SetActive(true);
            return _locker;
        }
        private static GameObject locker;
#else
            return null;
        }
#endif

        public static GameObject NewScreen()
        {
#if !LoadsonAPI
            GameObject _screen = UnityEngine.Object.Instantiate(screen);
            _screen.name = "Screen #" + UnityEngine.Random.Range(0, 32767);
            _screen.SetActive(true);
            return _screen;
        }
        private static GameObject screen;
#else
            return null;
        }
#endif


        /* Misc */

        public static GameObject NewEnemy()
        {
#if !LoadsonAPI
            GameObject _enemy = UnityEngine.Object.Instantiate(enemy);
            _enemy.name = "Enemy #" + UnityEngine.Random.Range(0, 32767);
            _enemy.SetActive(true);
            _enemy.GetComponent<NavMeshAgent>().enabled = true;
            return _enemy;
        }
        private static GameObject enemy;
#else
            return null;
        }
#endif

        public static GameObject NewMilk()
        {
#if !LoadsonAPI
            GameObject _milk = UnityEngine.Object.Instantiate(milk);
            _milk.name = "Milk #" + UnityEngine.Random.Range(0, 32767);
            _milk.SetActive(true);
            return _milk;
        }
        private static GameObject milk;
#else
            return null;
        }
#endif

        public static GameObject NewCube()
        {
#if !LoadsonAPI
            GameObject _cube = UnityEngine.Object.Instantiate(cube);
            _cube.name = "Cube #" + UnityEngine.Random.Range(0, 32767);
            _cube.SetActive(true);
            return _cube;
        }
        private static GameObject cube;
#else
            return null;
        }
#endif

        public static PhysicMaterial BounceMaterial()
        {
#if !LoadsonAPI
            PhysicMaterial _bounce = UnityEngine.Object.Instantiate(bounce);
            return _bounce;
        }
        private static PhysicMaterial bounce;
#else
            return null;
        }
#endif

        public static GameObject NewGlass()
        {
#if !LoadsonAPI
            GameObject _glass = UnityEngine.Object.Instantiate(glass);
            _glass.name = "Glass #" + UnityEngine.Random.Range(0, 32767);
            _glass.SetActive(true);
            return _glass;
        }
        private static GameObject glass;
#else
            return null;
        }
#endif

#if !LoadsonAPI
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
                    case "Cube (5)":
                    {
                        cube = UnityEngine.Object.Instantiate(o);
                        cube.name = "Loadson-Instance Cube";
                        UnityEngine.Object.DontDestroyOnLoad(cube);
                        cube.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Cube");
                        break;
                    }
                    case "Cube (11)":
                    {
                        bounce = UnityEngine.Object.Instantiate(o.GetComponent<BoxCollider>().material);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Bounce Material");
                        break;
                    }
                }
            }
        }
        public static void Init2()
        {
            if (SceneManager.GetActiveScene().name != "4Escape0") return;
            foreach (var o in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                switch (o.name)
                {
                    case "Glass":
                    {
                        glass = UnityEngine.Object.Instantiate(o);
                        glass.name = "Loadson-Instance Pistol";
                        UnityEngine.Object.DontDestroyOnLoad(glass);
                        glass.SetActive(false);
                        LoadsonInternal.Console.Log("[PREFABS] Instantiated Glass");
                        break;
                    }
                }
            }
        }
#endif
    }
}
