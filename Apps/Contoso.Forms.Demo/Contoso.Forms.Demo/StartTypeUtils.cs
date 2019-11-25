// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public enum StartType
    {
        AppCenter,
        OneCollector,
        Both
    };

    public class StartTypeUtils
    {
        public const StartType DefaultStartType = StartType.AppCenter;
        private const string StartTypeSettingKey = "startType";

        public static StartType GetPersistedStartType()
        {
            if (Application.Current.Properties.TryGetValue(StartTypeSettingKey, out object persistedStartTypeObject))
            {
                string persistedStartTypeString = (string)persistedStartTypeObject;
                if (Enum.TryParse<StartType>(persistedStartTypeString, out var persistedStartTypeEnum))
                {
                    return persistedStartTypeEnum;
                }
            }
            return DefaultStartType;
        }

        public static void SetPersistedStartType(StartType choice)
        {
            Application.Current.Properties[StartTypeSettingKey] = choice.ToString();
        }

        public static IEnumerable<string> GetStartTypeChoiceStrings()
        {
            foreach (var startTypeObject in Enum.GetValues(typeof(StartType)))
            {
                yield return startTypeObject.ToString();
            }
        }
    }
}
