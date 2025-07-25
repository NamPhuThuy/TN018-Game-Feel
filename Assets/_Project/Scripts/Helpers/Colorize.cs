using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy
{
    public class Colorize : MonoBehaviour
    {
        // Color Example

        public static Colorize Red = new Colorize(Color.red);
        public static Colorize Yellow = new Colorize(Color.yellow);
        public static Colorize Green = new Colorize(Color.green);
        public static Colorize Blue = new Colorize(Color.blue);
        public static Colorize Cyan = new Colorize(Color.cyan);
        public static Colorize Magenta = new Colorize(Color.magenta);

        // Hex Example
        public static Colorize Orange = new Colorize("#FFA500");
        public static Colorize Olive = new Colorize("#808000");
        public static Colorize Purple = new Colorize("#800080");
        public static Colorize DarkRed = new Colorize("#8B0000");
        public static Colorize DarkGreen = new Colorize("#006400");
        public static Colorize DarkOrange = new Colorize("#FF8C00");
        public static Colorize Gold = new Colorize("#FFD700");
        public static Colorize Gray = new Colorize("#808080");

        private readonly string _prefix;

        private const string Suffix = "</color>";

        // Convert Color to HtmlString
        private Colorize(Color color)
        {
            _prefix = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
        }
        // Use Hex Color
        private Colorize(string hexColor)
        {
            _prefix = $"<color={hexColor}>";
        }

        public static string operator %(string text, Colorize color)
        {
            return color._prefix + text + Suffix;
        }
    }
}