using System;

namespace Engine.Events;

public class LocaleChangeEventArgs(string locale) : EventArgs {

    public string Locale = locale;

}
