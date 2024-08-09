using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace SharinganOP
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
                if (SharinganOPLoader.debugMode) SharinganOPLoader.Log($"RELEASE!!!");

                //if (SharinganOPLoader.debugMode) SharinganOPLoader.Log($"GetBestCreature() Count: {Creature.allActive.Count}");

                bool effectedAnyCreature = false;
                foreach (var creature in Creature.allActive)
                {
                    if (UseShariganOnCreature(creature))
                    {
                        effectedAnyCreature = true;
                    }
                }

                if (effectedAnyCreature)
                {
                    SharinganOPLoader.sharinganAudioSource.Play();
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

            SharinganOPLoader.sharinganEffectedCreatures.Add(creature);

            return true;
        }

        public static bool IsValidTarget(Creature creature, bool allowLogging = true)
        {
            if (creature == null)
            {
                if (allowLogging && SharinganOPLoader.debugMode) SharinganOPLoader.Log($"IsValidTarget() creature == null");
                return false;
            }

            if (creature.brain == null)
            {
                if (allowLogging && SharinganOPLoader.debugMode) SharinganOPLoader.Log($"IsValidTarget() creature.brain == null");
                return false;
            }

            if (creature.isPlayer)
            {
                if (allowLogging && SharinganOPLoader.debugMode) SharinganOPLoader.Log($"IsValidTarget() creature.isPlayer");
                return false;
            }

            if (creature.isKilled)
            {
                if (allowLogging && SharinganOPLoader.debugMode) SharinganOPLoader.Log($"IsValidTarget() creature.isKilled");
                return false;
            }

            if (SharinganOPLoader.sharinganEffectedCreatures.Contains(creature))
            {
                if (allowLogging && SharinganOPLoader.debugMode) SharinganOPLoader.Log($"IsValidTarget() sharinganEffectedCreatures Contains");
                return false;
            }

            return true;
        }
    }
}
