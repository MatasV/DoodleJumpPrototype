using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ThemeDatabase : ScriptableObject
{
    private const string LatestThemeKey = "currentTheme";
    public List<ThemeData> themes;
    private ThemeData _currentTheme;
    
    public ThemeData CurrentTheme
    {
        get
        {
            if (PlayerPrefs.HasKey(LatestThemeKey))
            {
                var foundTheme = themes.Find(x => x.name == PlayerPrefs.GetString(LatestThemeKey));
                _currentTheme = foundTheme == null ? themes[0] : foundTheme;
            }
            else
            {
                _currentTheme = themes[0];
            }

            return _currentTheme;
        }
    }

    public void SetCurrentTheme(string theme)
    {
        if (themes.Any(x => x.name == theme))
        {
            _currentTheme = themes.FirstOrDefault(x => x.name == theme);
            PlayerPrefs.SetString(LatestThemeKey, theme);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Theme Couldn't Be Set");
        }
    }

}
