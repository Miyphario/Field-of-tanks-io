﻿namespace Mycom.Target.Unity.Internal.Interfaces
{
    using System;
    using Ads;

    internal interface ICustomParamsProxy : IDisposable
    {
        void SetAge(UInt32? value);

        void SetEmails(String[] value);

        void SetGender(GenderEnum value);

        void SetLang(String value);

        void SetIcqIds(UInt32[] value);

        void SetMrgsAppId(String value);

        void SetMrgsId(String value);

        void SetMrgsUserId(String value);

        void SetOkIds(String[] value);

        void SetVkIds(String[] value);

        void SetCustomUserIds(String[] value);

        void SetCustomParam(String key, String value);
    }
}