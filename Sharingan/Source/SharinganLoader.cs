using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Networking;

namespace Sharingan
{
    public class SharinganLoader : ThunderScript
    {
        // Setup
        //public static string folder => Application.persistentDataPath + "/Mods/Sharingan/";       // Quest
        public static string folder => Application.dataPath + "/StreamingAssets/Mods/Sharingan/";   // PC
        public static GameObject goForAudio;
        public static AudioSource sharinganAudioSource;
        public static AudioClip sharinganAudioClip;

        // Config
        public static float minimumPlayerToCreatureDotProduct = 0.77f;
        public static float minimumEyeToEyeDotProduct = 0.5f;

        public static float damageWakeThreshold = 5.0f;

        // Runtime
        public static List<Creature> sharinganEffectedCreatures = new List<Creature>();

        public static TexturedSpellIcon spellWheelLeft = null;
        public static TexturedSpellIcon spellWheelRight = null;

        public override void ScriptEnable()
        {
            goForAudio = new GameObject("Sharingan");
            GameObject.DontDestroyOnLoad(goForAudio);

            sharinganAudioSource = goForAudio.AddComponent<AudioSource>();
            sharinganAudioSource.loop = false;
            sharinganAudioSource.volume = 1.0f;
            sharinganAudioSource.spatialize = false;

            var soundLoader = goForAudio.AddComponent<SoundLoader>();

            spellWheelLeft = new TexturedSpellIcon("Sharingan", folder + "Sharingan.png", 0.5f);
            spellWheelRight = new TexturedSpellIcon("Sharingan", folder + "Sharingan.png", 0.5f);

            EventManager.onLevelLoad += EventManager_onLevelLoad;
            EventManager.onCreatureHit += EventManager_onCreatureHit;
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

        void EventManager_onCreatureHit(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
        {
            /*SharinganLoader.Log($"collisionInstance.damageStruct.damage = {collisionInstance.damageStruct.damage}");
            if (collisionInstance.damageStruct.damage < damageWakeThreshold)
            {
                return;
            }*/

            if (sharinganEffectedCreatures.Contains(creature))
            {
                if (creature.isKilled)
                {
                    return;
                }
                creature.brain.instance.Start();
                creature.brain.SetState(Brain.State.Idle);

                sharinganEffectedCreatures.Remove(creature);
            }
        }

        void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (sharinganEffectedCreatures.Contains(creature))
            {
                sharinganEffectedCreatures.Remove(creature);
            }
        }

        int GetValidTargets()
        {
            int validTargets = 0;

            foreach (var creature in Creature.allActive)
            {
                if (SpawnItemSpell.IsValidTarget(creature, false))
                {
                    validTargets++;
                }
            }

            return validTargets;
        }

        public override void ScriptUpdate()
        {
            base.ScriptUpdate();

            spellWheelLeft.Update(WheelMenuSpell.left);
            spellWheelRight.Update(WheelMenuSpell.right);
        }

        public override void ScriptDisable()
        {
            EventManager.onLevelLoad -= EventManager_onLevelLoad;
            EventManager.onCreatureHit -= EventManager_onCreatureHit;
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
            UnityEngine.GameObject.Destroy(goForAudio);
        }
    }
}
