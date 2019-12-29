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

        public static string[] propss =
        //new string[] { "gongFaNameText","gongFaLevelBarText","readBookLevelText","actorIntText","studyVauleText","readTurnText","needIntText","samsaraChangeText","gongFaFLevelText"};
        new string[] { "newBuildingNameText", "shopStorageText", "makeStateText", "levelUPName", "levelUPLevelText", "levelUPFLevelText", "levelUPTypText", "levelUPSizeText", "levelUPUseExpText", "levelUPActorLevelText", "levelUPNeedLevelText", "levelUPSpText", "gongFaLevelText", "gongFaFLevelText", "readTurnText", "bookTypText", "actorIntText", "readBookName", "readBookLevelText", "readBookFLevelText", "actorLevelText", "needLevelText", "gongFaNameText", "actorGenerationText", "actorNameText", "genderText", "charmText", "gongFaTypText", "studyVauleText", "samsaraChangeText", "buildingPctNameText", "favorChangeText", "gongFaNeedCostText", "gongFaLevelBarText", "newBuildingMassage", "newNeedTimeText", "haveMenpowerText", "needMenpowerText", "useMenpowerText", "makButtonText", "buildingName", "buildingMassage", "buildingState", "needTimeText", "buildingLevelUPText", "buildingLevelMoveText", "buildingNeedResourceText", "warehouseSizeText", "buildingLevelText", "buildingHpText", "actorName", "actorGoodness", "actorMood", "actorFavor", "actorSamsara", "actorAge", "actorHealth", "skillLevelText", "actorFame", "buildingUpNameText", "buildingUpMassage", "buildingUpNeedTimeText", "buildingUpHaveMenpowerText", "buildingUpNeedMenpowerText", "buildingUpUseMenpowerText", "levelChanageText", "buildingRemoveNameText", "buildingRemoveMassage", "buildingRemoveNeedTimeText", "buildingRemoveHaveMenpowerText", "buildingRemoveNeedMenpowerText", "buildingRemoveUseMenpowerText" };
        public static string[] textArrProps = new string[] { "levelUPNameText", "readedPageText", "pageNameText", "mianActorAgeText", "studyWindowName", "needResourceText" };

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
        public static void PrintAllInfo(BuildingWindow instance, string source)
        {
            Type type = instance.GetType();
            int hash = instance.GetHashCode();

            foreach (var prop in propss)
            {
                Text text = (Text)type.InvokeMember(prop, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, instance, null);
                Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", prop, text.text, hash, source));
            }
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "levelUPSkillId", instance.levelUPSkillId, hash, source));

            var readBoodId = instance.useValue;
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "chosenBookId", readBoodId, hash, source));

            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "buyStudyValue", DateFile.instance.buyStudyValue, hash, source));
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "addGongFaStudyValue", DateFile.instance.addGongFaStudyValue, hash, source));
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "addSkillStudyValue", DateFile.instance.addSkillStudyValue, hash, source));
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "chosenBookId", readBoodId, hash, source));

            var gonfaName = instance.gongFaNameText.text;
            var foundDict = DateFile.instance.gongFaDate.Where(dict => dict.Value[0] == gonfaName).FirstOrDefault();
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", "foundGongfa", foundDict.Key, hash, source));

            foreach (var prop in textArrProps)
            {
                Text[] textArr = (Text[])type.InvokeMember(prop, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, instance, null);
                var textStr = textArr.Select(t => t.text).Join();
                Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", prop, textStr, hash, source));
            }
        }

        public static void PrintDict(int hash, string source, Dictionary<int, string> dict, string name)
        {
            var lines = dict.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
            var dictString = string.Join(Environment.NewLine, lines);
            Main.Logger.Log(String.Format("大家好，我找到地方了 [{3}] [{2}]: [{0}] : [{1}]", name, dictString, hash, source));
        }
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

            //Main.Logger.Log(String.Format("大家好，我找到地方了 [{0}]: [{1}]", "Daytime", dayTime));
            //Main.Logger.Log(String.Format("大家好，我找到地方了 [{0}]: [{1}]", "修习进度", level));
            //Main.Logger.Log(String.Format("大家好，我找到地方了 [{0}]: [{1}]", "修习书籍", ___studySkillId));
            //Main.Logger.Log(String.Format("大家好，我找到地方了 [{0}]: [{1}]", "修习突破点", string.Join(", ", tupoSet.ToArray())));

            //Main.Logger.Log(String.Format("大家好，我找到地方了 [{0}]: [{1}]", "修习进度设置", Main.settings.progress));

        }
    }

    //// 选定书，那个圈圈
    //[HarmonyPatch(typeof(BuildingWindow), "StartStudy")]
    //static class StartStudy
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;

    //        int mainActorId = DateFile.instance.MianActorID();

    //        Main.PrintAllInfo(BuildingWindow.instance, "StartStudy");

    //    }
    //}

    //// 选定书，那个圈圈，发生于 StartStudy前面
    //[HarmonyPatch(typeof(BuildingWindow), "Study")]
    //static class Study
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "Study");
    //    }
    //}
    //[HarmonyPatch(typeof(BuildingWindow), "StudyNeedPrestige")]
    //static class StudyNeedPrestige
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "StudyNeedPrestige");
    //    }
    //}



    //[HarmonyPatch(typeof(BuildingWindow), "StartStudyGongFa")]
    //static class StartStudyGongFa
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "StartStudyGongFa");
    //    }
    //}
    //[HarmonyPatch(typeof(BuildingWindow), "SetStudy")]
    //static class SetStudy
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "SetStudy");
    //    }

    //}

    //// 选书读
    //[HarmonyPatch(typeof(BuildingWindow), "SetStudySkill")]
    //static class SetStudySkill
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "SetStudySkill");
    //    }

    //}
    //[HarmonyPatch(typeof(BuildingWindow), "SetSetStudyWindow")]
    //static class SetSetStudyWindow
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "SetSetStudyWindow");
    //    }

    //}

    //// 删掉正在读的书
    //[HarmonyPatch(typeof(BuildingWindow), "RemoveStudySkill")]
    //static class RemoveStudySkill
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "RemoveStudySkill");
    //    }

    //}

    //// 打开修习界面
    //[HarmonyPatch(typeof(BuildingWindow), "OpenStudyWindow")]
    //static class OpenStudyWindow
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "OpenStudyWindow");
    //    }

    //}

    //// 关闭选书窗口
    //[HarmonyPatch(typeof(BuildingWindow), "CloseSetStudyWindow")]
    //static class CloseSetStudyWindow
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "CloseSetStudyWindow");
    //    }

    //}
    //// 关闭修习界面
    //[HarmonyPatch(typeof(BuildingWindow), "CloseStudyWindow")]
    //static class CloseStudyWindow
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "CloseStudyWindow");
    //    }

    //}
    //[HarmonyPatch(typeof(BuildingWindow), "MoveStudyImage")]
    //static class MoveStudyImage
    //{
    //    static void Postfix()
    //    {
    //        if (!Main.enabled)
    //            return;
    //        Main.PrintAllInfo(BuildingWindow.instance, "MoveStudyImage");
    //    }

    //}    
}