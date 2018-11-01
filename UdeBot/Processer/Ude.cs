﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdeBot.Helper;

namespace UdeBot.Processer
{
    class Ude
    {
        internal delegate void ReplyMethod(string msg);
        internal static void BindUser(string toQQ,string username, ReplyMethod reply)
        {
            if (string.IsNullOrEmpty(username))
            {
                reply("请输入大佬在ude中的游戏id");
                return;
            }
            if (Verify.VerificationDictionary.ContainsKey(toQQ))
            {
                reply("你仍在一个验证流程中，请将ude绑定邮箱中的验证码通过私聊 !验证 233333 的方式发送给我");
                return;
            }
            if (Convert.ToBoolean(Database.RunQueryOne($"SELECT user_id is not null FROM phpbb_users WHERE QQ ='{toQQ}'")))
            {
                reply("你已经绑定过惹");
                return;
            }
            using (var r = Database.RunQuery($"SELECT user_id,(QQ is null) as havQQ FROM phpbb_users WHERE username='{username}'"))//如果没有获得userid将会返回0
            {
                if (r.Read())
                {
                    var userid = r.GetInt32("user_id");
                    var Binded = !r.GetBoolean("havQQ");
                    if (Binded)
                    {
                        reply("此id已经绑定过QQ了");
                        return;
                    }
                    Verify.VerificationDictionary.Add(toQQ, new Verify(userid, toQQ, Verify.VerifyFor.bind));
                    reply("请将ude绑定邮箱中的验证码通过私聊 !验证 233333 的方式发送给我");
                    return;
                }
            }
            reply("未ude找到此游戏id，请检查输入\n还没有注册的话\n可以在 https://osu.zhzi233.cn/p/register 注册\n(ude同样禁止小号)");
        }
        internal static void VerifyUser(string toQQ,string verificationCode, ReplyMethod reply)
        {
            {
                if (!Verify.VerificationDictionary.ContainsKey(toQQ))
                {
                    reply("你并没有要验证的操作，或者你的验证流程已经超时了呢");
                    return;
                }

                try
                {
                    if (Verify.VerificationDictionary[toQQ].VerifyCode(verificationCode))
                    {
                        reply("绑定成功~");
                    }
                    else
                    {
                        reply("验证码错误 请重新输入");
                    }
                }
                catch//三次均验证失败后会抛出异常
                {
                    reply("已达错误次数上限，请重新开始进行验证");
                }
            }
        }
        internal static void ReplyStats(string username,ReplyMethod reply)
        {
            if (string.IsNullOrEmpty(username))
            {
                reply("请输入用户名");
                return;
            }
            try
            {
                var userid = Convert.ToInt32(Database.RunQueryOne($"SELECT user_id from phpbb_users where username='{username}'"));//如果没有获得userid将会返回0
                using (var r = Database.RunQuery("select rank_score,accuracy_new,rank_score_index,level,playcount from osu_user_stats where user_id=" + userid))
                {
                    if (r.Read())
                    {
                        var pp = r.GetFloat("rank_score");
                        var acc = Math.Round(r.GetFloat("accuracy_new"), 2);
                        var rank = r.GetInt32("rank_score_index");
                        var playcount = r.GetInt32("playcount");
                        var level = Math.Round(r.GetFloat("level"), 2);
                        var msg = $"{username}(#{rank})大佬的水平统计为:\n" +
                                $"pp:{pp}\n" +
                                $"acc:{acc}\n" +
                                $"pc:{playcount}\n" +
                                $"level:{level}";
                        reply(msg);
                        return;
                    }
                }
                reply("用户名不存在");
                return;
            }
            catch
            {
                reply("出现未知错误，已转发错误信息给mxr123\n请等待修复");
                throw;//在这里throw后会被switch块外的catch捕获并发送给cfg.lotToQQ
            }
        }
        internal static void ReplyHelp(ReplyMethod reply) => reply(
                                "------------------Help-------------------\n" +
                                $"当前机器人版本: v.{new PluginInfo().Version}\n" +
                                "Bot更新日志：https://gitee.com/mxr123/UdeBot_QQ/commits/master \n" +
                                "查看水平统计：!stat(s) 用户名\n" +
                                "谁是卧底（开发中）:!sswd\n" +
                                "禁言指令：!smoke 目标 时长(秒)\n" +
                                "取消禁言：!unsmoke qq号\n" +
                                "超级管理员列表：!dog\n" +
                                "-----------------------------------------"
                                );

    }
}
