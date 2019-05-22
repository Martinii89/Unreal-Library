using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UELib;

namespace UELibTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestLoadFullPackage()
        {
            //Just testing for no unhandled exceptions
            string testPackage = "TestData\\AkAudio_decrypted.upk";
            var package = UnrealLoader.LoadFullPackage(testPackage, System.IO.FileAccess.Read);

        }
    }
}
