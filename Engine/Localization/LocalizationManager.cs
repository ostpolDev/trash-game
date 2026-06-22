using Engine.Events;
using Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Localization;

public class LocalizationManager {

    public const string LOCALE_EXTENSION = ".locale";

    public static event EventHandler<LocaleChangeEventArgs> OnLocaleChanged;

    public static string CurrentLocale { get; private set; } = "en";
    private static string LocalePath = null;

    private static readonly Dictionary<string, string> LocaleLookup = [];
    private static readonly Dictionary<string, string> FallbackLocaleLookup = [];

    private static Locale[] AvailableLocales = [];

    public static void Initialize() {
        LocalePath = PathHelper.GetLocaleDirectory();
        FindLocales();
        ChangeLocale(CurrentLocale, true);
    }

    public static void ChangeLocale(string locale, bool loadAsFallback = false) {
        if (!HasLocale(locale)) {
            throw new ArgumentException("Locale was not found", nameof(locale));
        }

        string filePath = Path.Join(LocalePath, $"{locale}{LOCALE_EXTENSION}");
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("Locale file was not found", filePath);
        }

        string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
        LocaleLookup.Clear();

        foreach (string line in lines) {
            if (string.IsNullOrEmpty(line) || line.StartsWith('#')) continue;
            string[] parts = line.Split('=');
            if (parts.Length != 2) continue;
            LocaleLookup[parts[0]] = parts[1];
        }

        if (loadAsFallback) {
            FallbackLocaleLookup.Clear();
            foreach (var item in LocaleLookup) {
                FallbackLocaleLookup[item.Key] = item.Value;
            }
        }

        OnLocaleChanged?.Invoke(null, new(locale));
    }

    private static void FindLocales() {
        string[] files = Directory.GetFiles(LocalePath);
        List<Locale> locales = [];
        foreach (string file in files) {
            if (Path.GetExtension(file) != LOCALE_EXTENSION) continue;

            string languageName = File.ReadLines(Path.Join(LocalePath, file)).First();
            locales.Add(new() { Code = Path.GetFileNameWithoutExtension(file), DisplayName = languageName });
        }
        AvailableLocales = [.. locales];
    }

    public static bool HasLocale(string code) {
        return AvailableLocales.Where(x => x.Code == code).FirstOrDefault().Code != null;
    }

    public static string Get(string key) {
        if (LocaleLookup.TryGetValue(key, out string val)) {
            return val;
        }
        if (FallbackLocaleLookup.TryGetValue(key, out string fallbackVal)) {
            return fallbackVal;
        }
        return key;
    }

    public struct Locale {
        public string DisplayName;
        public string Code;
    }

}
