// Copyright (C) 2024 Peter Guld Leth

#region

using UnityEngine;

#endregion

namespace Formfinder
{
    public static class ProjectConfig
    {
        private static ProjectConfigurationObject _instance;

        public static ProjectConfigurationObject InstanceConfig
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ProjectConfigurationObject>("Formfinder/ProjectConfigurationObject");
                    if (_instance == null)
                        Debug.LogError("ProjectConfigurationObject asset not found in Resources/Colorcrush folder.");
                }

                return _instance;
            }
        }
    }
}