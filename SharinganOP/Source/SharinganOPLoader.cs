using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Networking;

namespace SharinganOP
{
    public class SharinganOPLoader : ThunderScript
    {
        // Setup
        //public static string folder => Application.persistentDataPath + "/Mods/SharinganOP/";     // Quest
        public static string folder => Application.dataPath + "/StreamingAssets/Mods/SharinganOP/"; // PC
        public static GameObject goForAudio;
        public static AudioSource sharinganAudioSource;
        public static AudioClip sharinganAudioClip;

        // Runtime
        public static List<Creature> sharinganEffectedCreatures = new List<Creature>();

        public static TexturedSpellIcon spellWheelLeft = null;
        public static TexturedSpellIcon spellWheelRight = null;

        // Debug
        public static bool debugMode = false;

        public static void Log(string message)
        {
            if (debugMode)
            {
                UnityEngine.Debug.Log($"[SharinganOP] {message}");
            }
        }

        public override void ScriptEnable()
        {
            debugMode = HasDebugArg();

            SharinganOPLoader.Log($"ScriptEnable()");

            goForAudio = new GameObject("SharinganOP");
            GameObject.DontDestroyOnLoad(goForAudio);

            sharinganAudioSource = goForAudio.AddComponent<AudioSource>();
            sharinganAudioSource.loop = false;
            sharinganAudioSource.volume = 1.0f;
            sharinganAudioSource.spatialize = false;

            var soundLoader = goForAudio.AddComponent<SoundLoader>();

            spellWheelLeft = new TexturedSpellIcon("SharinganOP", folder + "Sharingan.png", 0.5f);
            spellWheelRight = new TexturedSpellIcon("SharinganOP", folder + "Sharingan.png", 0.5f);

            EventManager.onLevelLoad += EventManager_onLevelLoad;
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
        }

        void EventManager_onCreatureSpawn(Creature creature)
        {
            if (sharinganEffectedCreatures.Contains(creature))
            {
                sharinganEffectedCreatures.Remove(creature);
            }
        }

        void EventManager_onLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            sharinganEffectedCreatures.Clear();
        }

        void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (sharinganEffectedCreatures.Contains(creature))
            {
                sharinganEffectedCreatures.Remove(creature);
            }
        }

        public override void ScriptUpdate()
        {
            base.ScriptUpdate();

            //spellWheelLeft.Update(WheelMenuSpell.left);
            //spellWheelRight.Update(WheelMenuSpell.right);
        }

        public override void ScriptDisable()
        {
            SharinganOPLoader.Log($"ScriptDisable()");

            EventManager.onLevelLoad -= EventManager_onLevelLoad;
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            UnityEngine.GameObject.Destroy(goForAudio);
        }

        public static bool HasDebugArg()
        {
            string[] args = Environment.GetCommandLineArgs();

            foreach (var arg in args)
            {
                if (arg.Equals("-DebugSharinganOP", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
