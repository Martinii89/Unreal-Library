using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UELib;

namespace UELibTest
{
    [TestClass]
    public class LoadPackageUnitTest
    {
        [TestMethod]
        public void LoadEncryptedPackage()
        {
            //Just testing for no unhandled exceptions
            string testPackage = "TestData\\AkAudio.upk";
            var package = UnrealLoader.LoadFullPackage(testPackage, System.IO.FileAccess.Read);

        }

        [TestMethod]
        public void LoadDecryptedPackage()
        {
            //Just testing for no unhandled exceptions
            string testPackage = "TestData\\AkAudio_decrypted.upk";
            var package = UnrealLoader.LoadFullPackage(testPackage, System.IO.FileAccess.Read);

        }
    }
}
