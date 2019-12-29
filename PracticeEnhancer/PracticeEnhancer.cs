using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace PracticeEnhancer
{
    public class Settings : UnityModManager.ModSettings
    {
        public bool goalOriented = false;
        public bool daySetup = false;
        public int progress = 100;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

    }

    static class Main
    {
        public static bool enabled;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger Logger;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical("Box");
            settings.goalOriented = GUILayout.Toggle(settings.goalOriented, "选择修习程度");
            settings.daySetup = GUILayout.Toggle(settings.daySetup, "选择修习天数");
            GUILayout.Label("修习进度(1~100)：", GUILayout.Width(370));
            int.TryParse(GUILayout.TextArea(settings.progress.ToString(), GUILayout.Width(60)), out settings.progress);
            GUILayout.Label("说明： ");
            GUILayout.Label("若选择修习程度，会弹出窗口确认要修习的程度");
            GUILayout.Label("若选择修习天数，会弹出窗口确认要修习的天数");
            GUILayout.Label("若同时选择修习程度和修习天数，只有程度会优先。");
            GUILayout.Label("若遇到以下情况，修习会停止：");
            GUILayout.Label("   当月天数用完，遇到玄关，达到程度，达到天数");

            GUILayout.EndVertical();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
            
        public static Dictionary<int, HashSet<int>> TupoNeed = new Dictionary<int, HashSet<int>>()
        {
            {1, new HashSet<int>(){ 50 } },
            {2, new HashSet<int>(){ 50 } },
            {3, new HashSet<int>(){ 50 } },
            {4, new HashSet<int>(){ 30, 70 } },
            {5, new HashSet<int>(){ 30, 70 } },
            {6, new HashSet<int>(){ 25, 50, 75 } },
            {7, new HashSet<int>(){ 25, 50, 75 } },
            {8, new HashSet<int>(){ 20, 40, 60, 80 } },
            {9, new HashSet<int>(){ 20, 40, 60, 80 } }
        };        
    }

    [HarmonyPatch(typeof(BuildingWindow), "StudySkillUp")]
    static class StudySkillUp
    {
        static void Postfix(int ___studySkillId, int ___studySkillTyp, ref BuildingWindow __instance)
        {
            if (!Main.enabled)
                return;

            int mainActorId = DateFile.instance.MianActorID();
            int level;
            HashSet<int> tupoSet;
            if (___studySkillTyp == 17)
            {
                var gonfa = DateFile.instance.gongFaDate[___studySkillId];
                level = DateFile.instance.GetGongFaLevel(mainActorId, ___studySkillId);
                tupoSet = Main.TupoNeed[int.Parse(gonfa[2])];

            }
            else
            {
                level = DateFile.instance.GetSkillLevel(___studySkillId);
                var skill = DateFile.instance.skillDate[___studySkillId];
                tupoSet = Main.TupoNeed[int.Parse(skill[2])];
            }

            var dayTime = DateFile.instance.dayTime;
            
            if (!
                    (
                        tupoSet.Contains(level) || dayTime == 0 || level >= 100
                        ||
                        (Main.settings.goalOriented && level >= Main.settings.progress)
                    )
                )
            {
                __instance.StudySkillUp();
            }
        }
    }
}