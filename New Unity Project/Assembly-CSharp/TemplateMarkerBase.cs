using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public abstract class TemplateMarkerBase : MonoBehaviour
{
    public MarkerImportanceType m_markerImportanceType;
    public MarkerType m_markerType = MarkerType.eSubMarker;

    protected TemplateMarkerBase()
    {
    }

    public abstract bool Check(GameObject targetObject, out string errorInfo);
    public static bool IsMainMarker(TemplateMarkerBase marker)
    {
        return ((marker != null) && ((marker.m_markerType == MarkerType.eMainMarker) || (marker.m_markerType == MarkerType.eUniqueMainMarker)));
    }

    public static bool IsUniqueMarker(TemplateMarkerBase marker)
    {
        return ((marker != null) && (marker.m_markerType == MarkerType.eUniqueMainMarker));
    }

    protected bool isWildCardMatch(string targetString, string wildCard, bool ignoreCase)
    {
        string pattern = this.wildcard2Regex(wildCard);
        if (ignoreCase)
        {
            return Regex.IsMatch(targetString, pattern, RegexOptions.IgnoreCase);
        }
        return Regex.IsMatch(targetString, pattern);
    }

    protected string wildcard2Regex(string wildCard)
    {
        return ("^" + Regex.Escape(wildCard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$");
    }

    public enum MarkerImportanceType
    {
        eEssential,
        eOptional
    }

    public enum MarkerType
    {
        eMainMarker,
        eUniqueMainMarker,
        eSubMarker
    }

    [Serializable]
    public class NamePattern
    {
        public bool ignoreCase = true;
        public string namePattern = string.Empty;

        public string IgnoreCaseStr
        {
            get
            {
                return (!this.ignoreCase ? "区分大小写" : "不区分大小写");
            }
        }
    }
}

