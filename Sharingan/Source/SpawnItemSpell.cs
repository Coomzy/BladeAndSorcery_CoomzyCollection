using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Sharingan
{
    public class SpawnItemSpell : SpellCastCharge
    {
        // This method is called twice during a cast : the first time while holding the trigger ("active" become true) and the second time when releasing the trigger ("active" become false)
        public override void Fire(bool active)
        {
            base.Fire(active);

            // This happens when releasing the trigger
            if (!active)
            {
                //if (SharinganLoader.debugMode) SharinganLoader.Log($"GetBestCreature() Count: {Creature.allActive.Count}");

                var targetCreature = GetBestCreature();

                //if (SharinganLoader.debugMode) SharinganLoader.Log($"Post GetBestCreature() targetCreature: {targetCreature}");
                if (targetCreature != null)
                {
                    SharinganLoader.sharinganAudioSource.Play();
                    UseShariganOnCreature(targetCreature);
                }
            }
        }

        public bool UseShariganOnCreature(Creature creature)
        {
            if (!IsValidTarget(creature))
            {
                return false;
            }

            BrainModuleLookAt moduleLook = null;

            if (creature.brain.instance != null)
            {
                moduleLook = creature.brain.instance.GetModule<BrainModuleLookAt>();
            }

            creature.StopAnimation(true);
            creature.brain.SetState(Brain.State.Idle);
            creature.brain.Stop();

            if (moduleLook != null)
            {
                moduleLook.StopLookAt(true);
            }

            SharinganLoader.sharinganEffectedCreatures.Add(creature);

            return true;
        }

        public Creature GetBestCreature()
        {
            Creature bestCreature = null;
            float bestDotProduct = float.MinValue;

            foreach (var creature in Creature.allActive)
            {                
                if (!IsValidTarget(creature))
                {
                    continue;
                }

                Vector3 directonToCreatureHead = creature.centerEyes.transform.position - Player.currentCreature.centerEyes.transform.position;
                float playerToCreatureDotProd = Vector3.Dot(directonToCreatureHead.normalized, Player.currentCreature.centerEyes.forward);
                float minimumPlayerToCreatureDotProduct = 0.77f;


                if (playerToCreatureDotProd < minimumPlayerToCreatureDotProduct)
                {
                    continue;
                }

                if (playerToCreatureDotProd < bestDotProduct)
                {
                    continue;
                }

                float eyeToEyeDotProduct = Vector3.Dot(creature.centerEyes.forward, -Player.currentCreature.centerEyes.forward);
                float minimumEyeToEyeDotProduct = 0.5f;


                if (eyeToEyeDotProduct <= minimumEyeToEyeDotProduct)
                {
                    continue;
                }

                bestDotProduct = playerToCreatureDotProd;
                bestCreature = creature;
            }

            return bestCreature;
        }

        public static bool IsValidTarget(Creature creature, bool allowLogging = true)
        {
            if (creature == null)
            {
                return false;
            }

            if (creature.brain == null)
            {
                return false;
            }

            if (creature.isPlayer)
            {
                return false;
            }

            if (creature.isKilled)
            {
                return false;
            }

            if (SharinganLoader.sharinganEffectedCreatures.Contains(creature))
            {
                return false;
            }

            return true;
        }
    }
}
