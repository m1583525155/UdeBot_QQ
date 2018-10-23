﻿using Newbe.Mahua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static UdeBot.MahuaApis.Api;

namespace UdeBot
{
    static class Helper
    {
        internal enum SendType
        {
            friend = 1,
            group = 2,
            discuss = 3,
            groupSingle = 4,
            discussSingle = 5
        }
        internal static string LogonQQ = "2364463549";
        internal static Conf cfg;
        #region naiveApi
        [DllImport("Message.dll")]
        private static extern string Api_UploadVoice(string usingQQ, IntPtr Data);
        [DllImport("Message.dll")]
        private static extern bool Api_IsFriend(string usingQQ, string QQ);
        [DllImport("Message.dll")]
        private static extern bool Api_SendMsg(string usingQQ, int sendType, int subType, string groupDst, string QQDst, string Msg);
        #endregion
        internal static string api_UploadVoice(IntPtr Data)
        {
            return Api_UploadVoice(LogonQQ, Data);
        }
        internal static bool Api_isFriend(string QQ)
        {
            return Api_IsFriend(LogonQQ, QQ);
        }
        internal static bool Api_sendMsg(SendType sendType, string groupDst, string QQDst, string Msg, int subType = 0)
        {
            return Api_SendMsg(LogonQQ, (int)sendType, subType, groupDst, QQDst, Msg);
        }
        internal static bool Api_mute(string fromGroup, string dstQQ,int sec)
        {
                api.BanGroupMember(fromGroup, dstQQ, new TimeSpan(0, 0, Convert.ToInt32(sec)));
                api.SendGroupMessage(fromGroup, $"已将[@{dstQQ}]禁言{sec}秒钟");
                api.SendGroupMessage(fromGroup, "{E85F90EE-FC93-44EF-361D-343BD9BCB6BA}.amr");
                return true;
                api.SendGroupMessage(fromGroup, "你没有权限这么做");
                return false;
        }
        internal static string GetQQThroughAt(string At)
        {
            if (At.Contains("[@"))
                return At.Remove(0, 2).Remove(At.Length - 3);
            else
                return At;
        }
        internal static bool ConvertToPcm(ref string filename)
        {
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo("ffmpeg.exe", $"-i \"{filename}\" -y -f s16le -ar 24000 -ac 1 -acodec pcm_s16le \"{filename}.pcm\"");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    filename += ".pcm";
                    return true;
                }
                filename = "";
                return false;
            }
        }
        internal static bool ConvertPcmToSlik(ref string filename)
        {
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo("silk_v3_encoder.exe", $"\"{filename}\" \"{filename}.silk\" -tencent -rate 48000")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode == 0)
                {

                    filename += ".silk";
                    return true;
                }
                filename = "";
                return false;
            }
        }
        internal static IntPtr BytesToIntptr(byte[] bytes)
        {
            unsafe
            {
                fixed (byte* p = &bytes[0])
                {
                    return (IntPtr)p;
                }
            }
        }
    }
}
