using System;

[Serializable]
public class LocalizationData
{
    public string languageName;
    public LocalizationItem[] items;
}

[Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}

[Serializable]
public class LocalizationLanguage
{
    public string name;
    public string code;
}