//using System;
//using System.Speech.Synthesis;

//namespace BDanMuLib.Extensions
//{
//    public static class SystemSpeechExtensions
//    {

//        /// <summary>
//        /// 将字符串转换为语音并进行播放，可以指定转换速率。
//        /// </summary>
//        /// <param name="text2speak">要转换为语音的字符串</param>
//        public static void Speak(this string text2speak)
//        {
//            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
//            {
//                using var voice = new SpeechSynthesizer();

//                voice.SetOutputToDefaultAudioDevice();

//                //声音大一点
//                if (voice.Volume + 10 <= 100)
//                {
//                    voice.Volume += 10;
//                }

//                voice.Speak(text2speak);
//            }
//        }
//    }
//}
