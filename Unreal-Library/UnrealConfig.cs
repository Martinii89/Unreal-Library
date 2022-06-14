﻿using System;
using System.Collections.Generic;
using UELib.Types;

namespace UELib
{
    public static class UnrealConfig
    {
        public enum CookedPlatform
        {
            PC,
            Console
        }

        public static bool SuppressComments;
        public static bool SuppressSignature;

        public static string PreBeginBracket = UnrealSyntax.NewLine + "{0}";
        public static string PreEndBracket = UnrealSyntax.NewLine + "{0}";
        public static string Indention = "\t";
        public static CookedPlatform Platform;
        public static Dictionary<string, Tuple<string, PropertyType>> VariableTypes;

        public static string PrintBeginBracket()
        {
            return string.Format(PreBeginBracket, UDecompilingState.Tabs) + UnrealSyntax.BeginBracket;
        }

        public static string PrintEndBracket()
        {
            return string.Format(PreEndBracket, UDecompilingState.Tabs) + UnrealSyntax.EndBracket;
        }

        public static string ToUFloat(this float value)
        {
            return value.ToString("0.0000000000").TrimEnd('0').Replace(',', '.') + '0';
        }
    }
}