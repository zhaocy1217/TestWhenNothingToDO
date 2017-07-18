using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AllColorParser
{
    private string Header_Half_Str = "<color=";
    private List<string> m_result = new List<string>();
    private int m_viewFontSize;
    private string tail_Str = "</color>";

    private string _parse(int lingWidth, string content, List<string> result, out int actualWidth)
    {
        int startIndex = 0;
        bool flag = true;
        bool flag2 = false;
        bool flag3 = false;
        int num2 = 0;
        int num3 = 0;
        string str = string.Empty;
        while (startIndex < content.Length)
        {
            char ch = content[startIndex];
            if (ch == '<')
            {
                flag2 = this.bHeader_Half_Match(startIndex, content);
                if (flag2)
                {
                    num3 = startIndex;
                    startIndex += this.Header_Half_Str.Length;
                    flag = false;
                }
                if (!flag3 || !this.bColorTail_Match(startIndex, content))
                {
                    goto Label_00AA;
                }
                startIndex += this.tail_Str.Length;
                flag3 = false;
                continue;
            }
            if ((ch == '>') && flag2)
            {
                flag = true;
                flag3 = true;
                str = content.Substring(num3, (startIndex - num3) + 1);
                startIndex++;
                continue;
            }
        Label_00AA:
            if (flag)
            {
                int characterWidth = CChatParser.GetCharacterWidth(ch, this.m_viewFontSize);
                if ((num2 + characterWidth) > lingWidth)
                {
                    if (flag3)
                    {
                        string str2 = content.Substring(0, startIndex) + "</color>";
                        actualWidth = num2;
                        result.Add(str2);
                        string str3 = content.Substring(startIndex);
                        str3 = str + str3;
                        int num5 = 0;
                        string str4 = this._parse(lingWidth, str3, result, out num5);
                        if (!string.IsNullOrEmpty(str4))
                        {
                            result.Add(str4);
                        }
                        return string.Empty;
                    }
                    actualWidth = num2;
                    string item = content.Substring(0, startIndex);
                    result.Add(item);
                    string str6 = content.Substring(startIndex);
                    int num6 = 0;
                    string str7 = this._parse(lingWidth, str6, result, out num6);
                    if (!string.IsNullOrEmpty(str7))
                    {
                        result.Add(str7);
                    }
                    return string.Empty;
                }
                num2 += characterWidth;
            }
            startIndex++;
        }
        actualWidth = num2;
        return content;
    }

    private bool bColorTail_Match(int startIndex, string content)
    {
        return this.bMatch(startIndex, content, this.tail_Str);
    }

    private bool bHeader_Half_Match(int startIndex, string content)
    {
        return this.bMatch(startIndex, content, this.Header_Half_Str);
    }

    private bool bMatch(int startIndex, string content, string destStr)
    {
        int num = 0;
        while (num < destStr.Length)
        {
            if (content[startIndex] == destStr[num])
            {
                num++;
                startIndex++;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public List<string> Parse(int lingWidth, string content, int viewFontSize)
    {
        this.m_result.Clear();
        this.m_viewFontSize = viewFontSize;
        int actualWidth = 0;
        string str = this._parse(lingWidth, content, this.m_result, out actualWidth);
        if (!string.IsNullOrEmpty(str))
        {
            this.m_result.Add(str);
        }
        return this.m_result;
    }

    public List<string> Parse(int lingWidth, string content, int viewFontSize, out int actualWidth)
    {
        this.m_result.Clear();
        this.m_viewFontSize = viewFontSize;
        string str = this._parse(lingWidth, content, this.m_result, out actualWidth);
        if (!string.IsNullOrEmpty(str))
        {
            this.m_result.Add(str);
        }
        return this.m_result;
    }

    public string ParseSingleLine(int lingWidth, string content, int viewFontSize, out int actualWidth)
    {
        List<string> list = this.Parse(lingWidth, content, viewFontSize, out actualWidth);
        if (list.Count > 0)
        {
            return list[0];
        }
        return string.Empty;
    }
}

