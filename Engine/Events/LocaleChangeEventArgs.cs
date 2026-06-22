using System;

namespace Engine.Events;

public class LocaleChangeEventArgs(string locale, bool isFallback) : EventArgs {

    public string Locale = locale;
    public bool IsFallback = isFallback;

}
