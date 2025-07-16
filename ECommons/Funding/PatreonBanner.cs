using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using ECommons.EzSharedDataManager;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System;

namespace ECommons.Funding;
public static class PatreonBanner
{
    public static Func<bool> IsOfficialPlugin = () => false;
    public static string Text = "♥ Patreon";
    public static string DonateLink => "https://www.patreon.com/NightmareXIV";
    public static void DrawRaw()
    {
        DrawButton();
    }

    private static uint ColorNormal
    {
        get
        {
            var vector1 = ImGuiEx.Vector4FromRGB(0x022594);
            var vector2 = ImGuiEx.Vector4FromRGB(0x940238);

            var gen = GradientColor.Get(vector1, vector2).ToUint();
            var data = EzSharedData.GetOrCreate<uint[]>("ECommonsPatreonBannerRandomColor", [gen]);
            if(!GradientColor.IsColorInRange(data[0].ToVector4(), vector1, vector2))
            {
                data[0] = gen;
            }
            return data[0];
        }
    }

    private static uint ColorHovered => ColorNormal;

    private static uint ColorActive => ColorNormal;

    private static readonly uint ColorText = 0xFFFFFFFF;

    public static void DrawButton()
    {
        ImGui.PushStyleColor(ImGuiCol.Button, ColorNormal);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ColorHovered);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, ColorActive);
        ImGui.PushStyleColor(ImGuiCol.Text, ColorText);
        if(ImGui.Button(Text))
        {
            GenericHelpers.ShellStart(DonateLink);
        }
        Popup();
        if(ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        ImGui.PopStyleColor(4);
    }

    public static void RightTransparentTab(string? text = null)
    {
        text ??= Text;
        var textWidth = ImGui.CalcTextSize(text).X;
        var spaceWidth = ImGui.CalcTextSize(" ").X;
        ImGui.BeginDisabled();
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0f);
        if(ImGuiEx.BeginTabItem(" ".Repeat((int)MathF.Ceiling(textWidth / spaceWidth)), ImGuiTabItemFlags.Trailing))
        {
            ImGui.EndTabItem();
        }
        ImGui.PopStyleVar();
        ImGui.EndDisabled();
    }

    public static void DrawRight()
    {
        var cur = ImGui.GetCursorPos();
        ImGui.SetCursorPosX(cur.X + ImGui.GetContentRegionAvail().X - ImGuiHelpers.GetButtonSize(Text).X);
        DrawRaw();
        ImGui.SetCursorPos(cur);
    }

    private static string PatreonButtonTooltip => $"""
				{Svc.PluginInterface.Manifest.Name}가 마음에 드셨다면, 개발자의 Patreon 혹은 다른 방법으로 지원하는걸 고려해주세요! 
				
				이는 플러그인을 개발하는데 도움이 될 것이며 우선적인 기능 요청과 지원, 플러그인 빌드 미리보기, 기능과 다른 것들에 대한 투표 권한 또한 제공합니다. 

				좌클릭 - Patreon으로 연결
				우클릭 - 다른 방법 확인
				""";

    private static string SmallPatreonButtonTooltip => $"""
				{Svc.PluginInterface.Manifest.Name}가 마음에 드셨다면, Pateron을 통해 개발자를 지원해주세요.

				좌클릭 - Patreon으로 연결
				우클릭 - 다른 방법 확인
				""";

    private static void Popup()
    {
        if(ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGuiEx.Text(IsOfficialPlugin() ? SmallPatreonButtonTooltip : PatreonButtonTooltip);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            if(ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("NXPS");
            }
        }
        if(ImGui.BeginPopup("NXPS"))
        {
            if(ImGui.Selectable("Patreon 구독"))
            {
                GenericHelpers.ShellStart("https://subscribe.nightmarexiv.com");
            }
            /*if ("Donate one-time via Ko-Fi")
            {
                GenericHelpers.ShellStart("https://donate.nightmarexiv.com");
            }*/
            if(ImGui.Selectable("Cryptocurrency로 지원"))
            {
                GenericHelpers.ShellStart($"https://crypto.nightmarexiv.com/{(IsOfficialPlugin() ? "?" + Svc.PluginInterface.Manifest.Name : "")}");
            }
            if(!IsOfficialPlugin())
            {
                if(ImGui.Selectable("NightmareXIV의 Discord 참가"))
                {
                    GenericHelpers.ShellStart("https://discord.nightmarexiv.com");
                }
                if(ImGui.Selectable("NightmareXIV의 다른 플러그인 보기"))
                {
                    GenericHelpers.ShellStart("https://explore.nightmarexiv.com");
                }
            }
            ImGui.EndPopup();
        }
    }
}
