using System;

public static class NumToString
{
    public const float SECONDS_IN_DAY = 86400f;
    public const float SECONDS_IN_HOUR = 3600f;
    public const float SECONDS_IN_MINUTE = 60f;

    public static string GetShortFloat(float num, int maxFloatNums)
    {
        string returnZero()
        {
            return num.ToString("f0");
        }

        if (maxFloatNums <= 0)
            return returnZero();

        float t = (float)(num - Math.Truncate(num));
        if (t == 0f)
        {
            return returnZero();
        }
        else
        {
            return num.ToString($"f{maxFloatNums}");
        }
    }

    public static string GetShortFloat(float num)
    {
        return GetShortFloat(num, 2);
    }

    public static string GetReducedTime(float timeInSec)
    {
        string time = "";
        switch (timeInSec)
        {
            case float when timeInSec >= SECONDS_IN_DAY:
                {
                    time += (timeInSec / SECONDS_IN_DAY).ToString("f0") + " " + LocalizationManager.GetLocalizedText("time.d");
                    timeInSec /= SECONDS_IN_DAY;
                    timeInSec -= (int)timeInSec;
                    if (timeInSec > 0)
                    {
                        time += " " + GetReducedTime(timeInSec * SECONDS_IN_DAY);
                    }
                }
                break;

            case float when timeInSec >= SECONDS_IN_HOUR:
                {
                    time += (timeInSec / SECONDS_IN_HOUR).ToString("f0") + " " + LocalizationManager.GetLocalizedText("time.h");
                    timeInSec /= SECONDS_IN_HOUR;
                    timeInSec -= (int)timeInSec;
                    if (timeInSec > 0)
                    {
                        time += " " + GetReducedTime(timeInSec * SECONDS_IN_HOUR);
                    }
                }
                break;

            case float when timeInSec >= SECONDS_IN_MINUTE:
                {
                    time += (timeInSec / SECONDS_IN_MINUTE).ToString("f0") + " " + LocalizationManager.GetLocalizedText("time.min");
                    timeInSec /= SECONDS_IN_MINUTE;
                    timeInSec -= (int)timeInSec;
                    if (timeInSec > 0)
                    {
                        time += " " + GetReducedTime(timeInSec * SECONDS_IN_MINUTE);
                    }
                }
                break;

            default:
                {
                    float t = (float)(timeInSec - Math.Truncate(timeInSec));
                    if (t == 0f)
                    {
                        time += timeInSec.ToString("f0") + " " + LocalizationManager.GetLocalizedText("time.sec");
                    }
                    else
                    {
                        time += timeInSec.ToString("f1") + " " + LocalizationManager.GetLocalizedText("time.sec");
                    }
                }
                break;
        }

        return time;
    }

    public static string GetReducedNumber(float number)
    {
        string time = "";
        int maxNums = 1;
        string suffix;
        float addNum;

        switch (number)
        {
            case float when number >= 1000000000f:
                {
                    addNum = number / 1000000000f;
                    suffix = LocalizationManager.GetLocalizedText("num.bil");
                }
                break;

            case float when number >= 1000000f:
                {
                    addNum = number / 1000000f;
                    suffix = LocalizationManager.GetLocalizedText("num.mln");
                }
                break;

            case float when number >= 1000f:
                {
                    addNum = number / 1000f;
                    suffix = LocalizationManager.GetLocalizedText("num.thous");
                }
                break;

            default:
                {
                    addNum = number;
                    suffix = "";
                }
                break;
        }

        if (addNum - Math.Truncate(addNum) == 0f)
            maxNums = 0;

        time += addNum.ToString("n" + maxNums) + suffix;
        return time;
    }
}
