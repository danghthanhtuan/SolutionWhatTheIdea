﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SWTI.DbUpgrade.RunOnceScripts {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class RunOnce {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RunOnce() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SWTI.DbUpgrade.RunOnceScripts.RunOnce", typeof(RunOnce).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE [dbo].[Accounts](
        ///	[ID] [int] IDENTITY(1,1) NOT NULL,
        ///	[FullName] [nvarchar](250) NULL,
        ///	[UserName] [varchar](50) NULL,
        ///	[Password] [varchar](550) NULL,
        ///	[CreatedDate] [datetime] NULL,
        ///	[UpdatedDate] [datetime] NULL,
        ///	[UpdatedUser] [varchar](50) NULL,
        ///	[CreatedUser] [varchar](50) NULL,
        ///	[Status] [int] NULL,
        ///	[Token] [varchar](500) NULL,
        /// CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
        ///(
        ///	[ID] ASC
        ///)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_ [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _0001_Initial_Database {
            get {
                return ResourceManager.GetString("_0001_Initial_Database", resourceCulture);
            }
        }
    }
}
