﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kaeos.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Kaeos.Models.TimeFormat DateTimeModule_TimeFormat {
            get {
                return ((global::Kaeos.Models.TimeFormat)(this["DateTimeModule_TimeFormat"]));
            }
            set {
                this["DateTimeModule_TimeFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Kaeos.Models.DateFormat DateTimeModule_DateFormat {
            get {
                return ((global::Kaeos.Models.DateFormat)(this["DateTimeModule_DateFormat"]));
            }
            set {
                this["DateTimeModule_DateFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string WeatherModule_Location {
            get {
                return ((string)(this["WeatherModule_Location"]));
            }
            set {
                this["WeatherModule_Location"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Kaeos.Models.UnitFormat WeatherModule_UnitFormat {
            get {
                return ((global::Kaeos.Models.UnitFormat)(this["WeatherModule_UnitFormat"]));
            }
            set {
                this["WeatherModule_UnitFormat"] = value;
            }
        }
    }
}
