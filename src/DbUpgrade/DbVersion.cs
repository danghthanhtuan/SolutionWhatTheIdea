using System;
using System.Collections.Generic;
using System.Text;

namespace SWTI.DbUpgrade
{
    public class DbVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public DbVersion(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }
        public static DbVersion Default()
        {
            return new DbVersion(1, 0, 1);
        }
        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Build);
        }

        public int Compare(DbVersion dbVersion)
        {
            if (Major < dbVersion.Major)
            {
                return -1;
            }
            if (Major == dbVersion.Major)
            {
                if (Minor < dbVersion.Minor)
                {
                    return -1;
                }
                if (Minor == dbVersion.Minor)
                {
                    if (Build < dbVersion.Build)
                    {
                        return -1;
                    }
                    if (Build == dbVersion.Build)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }
    }
}
