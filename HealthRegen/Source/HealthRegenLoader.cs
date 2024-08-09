using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace HealthRegen
{
    public class HealthRegenLoader : ThunderScript
    {
        [ModOption("Enabled", "Is this mod enabled?", valueSourceName: nameof(ModOptionBool.defaultValues), valueSourceType = typeof(ModOptionBool), defaultValueIndex = 1, categoryOrder = 99)]
        public static bool enableMod = true;

        [ModOption("Time To Fully Regen Health", "Amount of time in seconds that it takes to go from 0-100% health (after delay)", nameof(timeToFullyRegenHealth_Slider), defaultValueIndex = 15, categoryOrder = 1)]
        [ModOptionSlider]
        public static float timeToFullyRegenHealth = 4.0f;

        [ModOption("Health Regen Delay", "Amount in seconds before health starts regenerating after taking damage", nameof(healthRegenDelay_Slider), defaultValueIndex = 20, categoryOrder = 2)]
        [ModOptionSlider]
        public static float healthRegenDelay = 5.0f;

        public static ModOptionFloat[] timeToFullyRegenHealth_Slider()
        {
            var options = new List<ModOptionFloat>();

            float minValue = 0.25f;
            float maxValue = 60.0f;
            float increment = 0.25f;
            int numOptions = (int)((maxValue - minValue) / increment);

            for (int i = 0; i <= numOptions; i++)
            {
                float val = minValue + (i * increment);
                options.Add(new ModOptionFloat(val.ToString("0.00"), val));
            }

            return options.ToArray();
        }

        public static ModOptionFloat[] healthRegenDelay_Slider()
        {
            var options = new List<ModOptionFloat>();

            float minValue = 0.0f;
            float maxValue = 60.0f;
            float increment = 0.25f;
            int numOptions = (int)((maxValue - minValue) / increment);

            for (int i = 0; i <= numOptions; i++)
            {
                float val = minValue + (i * increment);
                string valStr = val.ToString("0.00");
                options.Add(new ModOptionFloat(valStr, val));
            }

            return options.ToArray();
        }

        public override void ScriptUpdate()
        {
            if (!enableMod)
            {
                return;
            }

            if (Player.currentCreature == null)
            {
                return;
            }

            if (Player.currentCreature.currentHealth >= Player.currentCreature.maxHealth)
            {
                return;
            }

            float timeSinceDamage = Time.time - Player.currentCreature.lastDamageTime;

            if (timeSinceDamage < healthRegenDelay)
            {
                return;
            }

            float healthRegenPerSecond = Player.currentCreature.maxHealth / timeToFullyRegenHealth;
            float healthRegenAmount = healthRegenPerSecond * Time.deltaTime;

           // Debug.Log($"HealthRegen healAmount '{healthRegenAmount}', current health = {Player.currentCreature.currentHealth}, healthRegenDelay = {healthRegenDelay}, timeToFullyRegenHealth = {timeToFullyRegenHealth}, timeSinceDamage = {timeSinceDamage}");

            Player.currentCreature.Heal(healthRegenAmount, Player.currentCreature);
        }
    }
}
