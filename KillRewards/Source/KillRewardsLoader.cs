using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace KillRewards
{
    public class KillRewardsLoader : ThunderScript
    {
        [ModOption("Enabled", "Is this mod enabled?", valueSourceName: nameof(ModOptionBool.defaultValues), valueSourceType = typeof(ModOptionBool), defaultValueIndex = 1, categoryOrder = 100)]
        public static bool enableMod = true;

        [ModOption("Enable Health Gains", "Disable to stop health regaining on kill", valueSourceName: nameof(ModOptionBool.defaultValues), valueSourceType = typeof(ModOptionBool), defaultValueIndex = 1, categoryOrder = 99)]
        public static bool enableHealth = true;

        [ModOption("Health Amount", "Amount of health healed per kill", nameof(amount_Slider), defaultValueIndex = 4, categoryOrder = 98)]
        [ModOptionSlider]
        public static float healAmount = 25.0f;

        [ModOption("Enable Focus Gains", "Disable to stop focus regaining on kill", valueSourceName: nameof(ModOptionBool.defaultValues), valueSourceType = typeof(ModOptionBool), defaultValueIndex = 1, categoryOrder = 79)]
        public static bool enableFocus = true;

        [ModOption("Focus Amount", "Amount of focus gained per kill", nameof(amount_Slider), defaultValueIndex = 4, categoryOrder = 78)]
        [ModOptionSlider]
        public static float focusAmount = 25.0f;

        public static ModOptionFloat[] amount_Slider()
        {
            var options = new List<ModOptionFloat>();

            float minValue = 5.0f;
            float maxValue = 100.0f;
            float increment = 5.0f;
            int numOptions = (int)((maxValue - minValue) / increment);

            for (int i = 0; i <= numOptions; i++)
            {
                float val = minValue + (i * increment);
                string valStr = val.ToString("0.00");
                options.Add(new ModOptionFloat(valStr, val));
            }

            return options.ToArray();
        }

        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onCreatureKill += EventManager_onCreatureKill;
        }

        void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            //Debug.Log($"KillRewards enableMod: {enableMod}, enableHealth: {enableHealth}, enableMana: {enableMana}, enableFocus: {enableFocus}");
            if (!enableMod)
            {
                return;
            }

            if (Player.currentCreature == null)
            {
                return;
            }

            //Debug.Log($"KillRewards currentHealth: {Player.currentCreature.currentHealth}, currentMana: {Player.currentCreature.mana.currentMana}, currentFocus: {Player.currentCreature.mana.currentFocus}");

            if (enableHealth)
            {
                Player.currentCreature.Heal(healAmount, Player.currentCreature);
            }

            if (Player.currentCreature.mana == null)
            {
                return;
            }

            if (enableFocus)
            {
                float newFocus = Player.currentCreature.mana.currentFocus + focusAmount;
                newFocus = Mathf.Min(newFocus, Player.currentCreature.mana.MaxFocus);
                Player.currentCreature.mana.currentFocus = newFocus;
            }
        }

        public override void ScriptDisable()
        {
            EventManager.onCreatureKill -= EventManager_onCreatureKill;
            base.ScriptDisable();
        }
    }
}
